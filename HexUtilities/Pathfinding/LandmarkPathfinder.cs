#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  using IDirectedPath = IDirectedPathCollection;

  /// <summary>C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).</summary>
  /// <remarks>Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.</remarks>
  /// <see cref="StandardPathfinder"/>
  /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
  /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
  /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
  /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
  public sealed class LandmarkPathfinder : Pathfinder, IPathfinder, IPathHalves {
    /// <summary>Calculates an <see cref="IDirectedPath"/> for the optimal path from coordinates .</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <returns></returns>
    public static LandmarkPathfinder New(INavigableBoard board, ILandmarkCollection landmarks,
      HexCoords source, HexCoords target
    ) {
      return new LandmarkPathfinder(board, landmarks, source, target);
    }
    /// <summary>Asynchronously calculates an <see cref="IDirectedPath"/> for the optimal path from coordinates .</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <returns></returns>
    public static async Task<LandmarkPathfinder> NewAsync(INavigableBoard board, ILandmarkCollection landmarks,
      HexCoords source, HexCoords target
    ) {
      return await Task.Run<LandmarkPathfinder>( () => New(board, landmarks, source, target) );
    }

    /// <summary>Calculates an <see cref="IDirectedPath"/> for the optimal path from coordinates .</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    internal LandmarkPathfinder(INavigableBoard board, ILandmarkCollection landmarks,
      HexCoords source, HexCoords target) 
    : base(source, target, new HashSet<HexCoords>()
    ) {
      if (landmarks == null) {
        _pathFwd = _pathRev = null;
      } else {
        Pathfinder.TraceFindPathDetailInit(Source, Target);

        _bestSoFar        = int.MaxValue;
        var pathfinderFwd = DirectionalPathfinder.NewReverse(board, landmarks, Source, Target, this);
        var pathfinderRev = DirectionalPathfinder.NewForward(board, landmarks, Source, Target, this);

        // Alternate searching from each direction and calling the other direction
        pathfinderFwd.Partner = pathfinderRev;
        var pathfinder        = pathfinderRev.Partner
                              = pathfinderFwd;

        while (! pathfinder.IsFinished())  pathfinder = pathfinder.Partner; 

        TraceFindPathDone(ClosedSet.Count);
      }
    }
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod]  [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    }
   
    #region Properties
    /// <summary>The <see cref="ISet{HexCoords}"/> of all hexes expanded in finding the shortest-path.</summary>
    public new ISet<HexCoords>    ClosedSet { get { return base.ClosedSet; } }
    /// <summary>Retrieve the found path in walking order: first step at top of stack to target at bottom.</summary>
    /// <remarks>
    /// The path steps are ordered normally as the forward half-path has been stacked 
    /// onto the reverse half-path during post-processing.
    /// </remarks>
    /// <see cref="PathReverse"/>
    public override IDirectedPath PathForward { get { return Pathfinder.MergePaths(_pathFwd, _pathRev); } }
    /// <summary>Retrieve the found path in reverse walking order: target at top of stack to first step at bottom.</summary>
    /// <remarks>
    /// The path steps are ordered in reverse as the reverse half-path has been stacked 
    /// onto the forward half-path during post-processing.
    /// </remarks>
    /// <see cref="LandmarkPathfinder.PathForward"/>
    public override IDirectedPath PathReverse { get { return Pathfinder.MergePaths(_pathRev, _pathFwd); } }

    /// <see cref="PathForward"/>
    [Obsolete("Deprecated - use property PathFwd instead.",true)]
    public     IDirectedPath      Path      { get {return PathForward;} }
    #endregion

    #region IPathHalves implementation
    /// <summary>Retrieves the cost of the shortest path found so far.</summary>
              int     IPathHalves.BestSoFar { get {return _bestSoFar;} } int _bestSoFar;
    /// <summary>Updates the record of the shortest path found so far.</summary>
    /// <param name="pathFwd">The half-path obtained by searching backward from the target (so stacked forwards).</param>
    /// <param name="pathRev">The half-path obtained by searching forward from the source (so stacked backwards).</param>
              void    IPathHalves.SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd) {
      if( pathFwd.TotalCost + pathRev.TotalCost < _bestSoFar) {
        _pathRev    = pathRev; 
        _pathFwd    = pathFwd; 
        _bestSoFar  = _pathRev.TotalCost + _pathFwd.TotalCost;

        Pathfinder.TraceFindPathDetailBestSoFar(pathFwd.PathStep.Coords, pathRev.PathStep.Coords, _bestSoFar);
      }
    }

    /// <summary>The half-path obtained by searching backward from the target (so stacked forwards).</summary>
    private   IDirectedPath       _pathFwd;
    /// <summary>The half-path obtained by searching forward from the source (so stacked backwards).</summary>
    private   IDirectedPath       _pathRev;
    #endregion
  }
}

