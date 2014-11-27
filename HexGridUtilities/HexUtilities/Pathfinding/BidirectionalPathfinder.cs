#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1587
/// <summary>A fast efficient serial implementation of <b>Bidirectional ALT</b> (<b>A*</b> with <b>L</b>andmarks and <b>T</b>riangle-inequality heuristic) <b>path-finding</b>
/// on a <see cref="Hexgrid"/> map.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.Pathfinding {
  using DirectedPath  = DirectedPathCollection;
  using IDirectedPath = IDirectedPathCollection;

  /// <summary>C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).</summary>
  /// <remarks>Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.</remarks>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
    /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
    /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
    /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
  public sealed class BidirectionalPathfinder : Pathfinder {
    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <paramref name="start"/> to <paramref name="goal"/>.</summary>
    /// <param name="start">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <see cref="FindDirectedPathRev"/>
    public static IDirectedPathCollection FindDirectedPathFwd(IHex start,IHex goal, INavigableBoard board) {
      return (new BidirectionalPathfinder(start, goal, board)).PathRev;
    }
    /// <summary>As <see cref="FindDirectedPathFwd"/>, except with the steps stacked in reverse for more convenient use.</summary>
    /// <param name="start">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <remarks>
    /// The path steps are ordered in reverse as the forward half-path has been stacked 
    /// onto the reverse half-path during post-processing, instead of the reverse.
    /// </remarks>
    /// <see cref="FindDirectedPathFwd"/>
    public static IDirectedPathCollection FindDirectedPathRev(IHex start, IHex goal, INavigableBoard board) {
      return (new BidirectionalPathfinder(start, goal, board)).PathFwd;
    }

    /// <summary>Calculates an <c>IPath</c> for the optimal path from coordinates .</summary>
    /// <param name="start">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    public BidirectionalPathfinder(IHex start,  IHex goal,  INavigableBoard board) : base(start,goal,board) 
    {
      Pathfinder.TraceFindPathDetailInit(Start.Coords, Goal.Coords);

      PathHalves          = new PathHalves(new HashSet<HexCoords>());

      var pathfinderFwd   = new DirectionalPathfinder.PathfinderRev(Start, Goal, Board, PathHalves);
      var pathfinderRev   = new DirectionalPathfinder.PathfinderFwd(Start, Goal, Board, PathHalves);

      // Alternate searching from each direction and calling the other direction
      pathfinderFwd.Partner = pathfinderRev;
      var pathfinder        = pathfinderRev.Partner
                            = pathfinderFwd;

      while (! pathfinder.IsFinished())  pathfinder = pathfinder.Partner; 
    }

    #region Properties
    /// <inheritdoc/>
    public override ISet<HexCoords> ClosedSet   { get {return PathHalves.ClosedSet;} }
    /// <inheritdoc/>
    public ILandmarkCollection      Landmarks   { get {return Board.Landmarks;} }
    /// <inheritdoc/>
    [Obsolete("Deprecated - use property PathRev (or perhaps even PathFwd) instead.")]
    public IDirectedPath            Path        { get {return PathHalves.PathRev;} }
    /// <inheritdoc/>
    public IDirectedPath            PathFwd     { get {return PathHalves.PathFwd;} }
    /// <inheritdoc/>
    public IDirectedPath            PathRev     { get {return PathHalves.PathRev;} }
    /// <summary>TODO</summary>
    private PathHalves              PathHalves  { get; set; }
    #endregion

    [DebuggerDisplay("")]
    internal abstract class DirectionalPathfinder : Pathfinder {
      // Common settings for both directions
      /// <param name="start">Source hex for this half of the bidirectional path search.</param>
      /// <param name="goal">Target hex for this this half of the bidirectional path search.</param>
      /// <param name="board">Board on which this path search is taking place.</param>
      /// <param name="pathHalves"></param>
      protected DirectionalPathfinder(IHex start, IHex goal, INavigableBoard board,  PathHalves pathHalves
      ) : base(start, goal, board) {
        PathHalves          = pathHalves;
        _openSet            = new Dictionary<HexCoords, IDirectedPath>();
        _queue              = new HotPriorityQueue<IDirectedPath>(0,256);
        var path            = new DirectedPath(start);
        _openSet.Add(path.PathStep.Hex.Coords, path);
        _queue.Enqueue (0, path);
      }

      #region Properties
      private                 int                   BestSoFar   { get {return PathHalves.BestSoFar;} }
      public override sealed  ISet<HexCoords>       ClosedSet   { get {return PathHalves.ClosedSet;} }
      protected               ILandmarkCollection   Landmarks   { get {return Board.Landmarks;} }
      internal                DirectionalPathfinder Partner     { get; set; }
      protected               PathHalves            PathHalves  { get; private set; }
      #endregion

      #region Methods
      protected abstract  IDirectedPath AddStep(IDirectedPath path, IHex there, Hexside hexside, int cost);
      protected           void          ExpandHex(IDirectedPath path, Hexside hexside) {
        var here  = path.PathStep.Hex;
        var there = here.Neighbour(hexside);
        if (there != null  &&  ! ClosedSet.Contains(there.Coords) ) {
          var cost = StepCost(here, hexside, there);
          if( (cost > 0)
          &&  (path.TotalCost+cost < BestSoFar  ||  ! _openSet.ContainsKey(there.Coords))
          ) {
            var key     = path.TotalCost + cost + Heuristic(there.Coords);
            var newPath = AddStep(path, there, hexside, cost);

            TraceFindPathEnqueue(there.Coords, key, 0);

            IDirectedPath oldPath;
            if ( ! _openSet.TryGetValue(there.Coords, out oldPath))  {
              _openSet.Add(there.Coords, newPath);
              _queue.Enqueue(key, newPath);
            } else if (newPath.TotalCost < oldPath.TotalCost) {
              _openSet.Remove(there.Coords);
              _openSet.Add(there.Coords, newPath);
              _queue.Enqueue(key, newPath);
            }

            SetBestSoFar(newPath, Partner.GetPartnerPath(there.Coords));
          }
        }
      }
      protected           int           FrontierMinimum() { 
        HexKeyValuePair<int,IDirectedPath> item;
        return (_queue.TryPeek(out item) ? item.Key : int.MaxValue); 
      }
      protected           IDirectedPath GetPartnerPath(HexCoords coords) {
        IDirectedPath path;
        _openSet.TryGetValue(coords,out path);
        return path;
      }
      protected           int           Heuristic(HexCoords coords) { 
        return Landmarks.Max(l => LandmarkHeuristic(l,coords));
      }
      internal            bool          IsFinished(){
        HexKeyValuePair<int,IDirectedPath> item;
        if ( _queue.TryDequeue(out item)) {
          var path   = item.Value;
          var coords = path.PathStep.Hex.Coords;

          _openSet.Remove(coords);
          if( ! ClosedSet.Contains(coords) ) {

            TraceFindPathDequeue(GetType().Name,coords, path.TotalCost, path.HexsideExit, item.Key, 0);

            if (item.Key < BestSoFar
            &&  path.TotalCost + Partner.FrontierMinimum() - Partner.Heuristic(coords) < BestSoFar
            ) {
              HexsideExtensions.HexsideList.ForEach(hexside => ExpandHex(path, hexside));
            }
            ClosedSet.Add(coords);
          }
          return ! _queue.Any();
        }
        return true;
      }
      protected abstract  int           LandmarkHeuristic(ILandmark landmark, HexCoords here);
      protected abstract  void          SetBestSoFar(IDirectedPath fwdPath, IDirectedPath revPath);
      protected abstract  int           StepCost(IHex here, Hexside hexside, IHex there);
      #endregion

      #region Fields
      IDictionary<HexCoords,IDirectedPath>  _openSet;
      IPriorityQueue<int,IDirectedPath>     _queue;
      #endregion

      /// <summary>A <see cref="DirectedPathCollection"/> from start to join-point obtained by searching forwards from start.</summary>
      internal sealed class PathfinderFwd : DirectionalPathfinder {
        /// <summary>Create a new instance of <see cref="PathfinderFwd"/>.</summary>
        /// <param name="start">Source hex for this path search.</param>
        /// <param name="goal">Target hex for this path search.</param>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="pathHalves"></param>
        internal PathfinderFwd(IHex start, IHex goal, INavigableBoard board, PathHalves pathHalves
        ) : base (start, goal, board, pathHalves) {
          TraceFindPathDetailDirection("Fwd",goal.Coords-start.Coords);
        }

        protected override sealed IDirectedPath AddStep(IDirectedPath path, IHex there, Hexside hexside, int cost) {
          return path.AddStep(there,hexside,cost);
        }
        protected override sealed int           LandmarkHeuristic(ILandmark landmark, HexCoords here) {
          return landmark.DistanceFrom(here) - landmark.DistanceFrom(Goal.Coords);
        }
        protected override sealed void          SetBestSoFar(IDirectedPath self, IDirectedPath partner) {
          PathHalves.SetBestSoFar(partner, self);
        }
        protected override sealed int           StepCost(IHex here, Hexside hexside, IHex there) {
          return Board.GetDirectedCostToExit(here, hexside);
        }
      }

      /// <summary>A <see cref="DirectedPathCollection"/> from join-point to goal obtained by searching backwards from goal.</summary>
      internal sealed class PathfinderRev : DirectionalPathfinder {
        /// <summary>Create a new instance of <see cref="PathfinderRev"/></summary>
        /// <param name="start">Source hex for this path search.</param>
        /// <param name="goal">Target hex for this path search.</param>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="pathHalves"></param>
        internal PathfinderRev(IHex start, IHex goal, INavigableBoard board, PathHalves pathHalves
        ) : base (goal, start, board, pathHalves) {
          TraceFindPathDetailDirection("Rev",start.Coords-goal.Coords);
        }

        protected override sealed IDirectedPath AddStep(IDirectedPath path, IHex there, Hexside hexside, int cost) {
          return path.AddStep(there,hexside.Reversed(),cost);
        }
        protected override sealed int           LandmarkHeuristic(ILandmark landmark, HexCoords here) {
          return landmark.DistanceTo(here) - landmark.DistanceTo(Goal.Coords);
        }
        protected override sealed void          SetBestSoFar(IDirectedPath self, IDirectedPath partner) {
          PathHalves.SetBestSoFar(self, partner);
        }
        protected override sealed int           StepCost(IHex here, Hexside hexside, IHex there) {
          return Board.GetDirectedCostToExit(there, hexside.Reversed());
        }
      }
    }
  }

  [DebuggerDisplay("BestSoFar={BestSoFar}; ClosedSet={ClosedSet.Count}; PathFwd={_pathFwd.TotalSteps} steps; PathRev={_pathRev.TotalSteps} steps")]
  internal sealed class PathHalves {
    public PathHalves(ISet<HexCoords> closedSet) { 
      BestSoFar   = int.MaxValue;
      ClosedSet   = closedSet;
    }

    public int              BestSoFar { get; private set; }
    public ISet<HexCoords>  ClosedSet { get; private set; }
    public IDirectedPath    PathFwd   { get { return MergePaths(_pathFwd, _pathRev); } }
    public IDirectedPath    PathRev   { get { return MergePaths(_pathRev, _pathFwd); } }

    public void             SetBestSoFar(IDirectedPath pathFwd, IDirectedPath pathRev) {
      if (pathFwd==null  ||  pathRev==null) return;
      if( pathFwd.TotalCost + pathRev.TotalCost < BestSoFar) {
        _pathRev  = pathFwd; 
        _pathFwd  = pathRev; 
        BestSoFar = _pathRev.TotalCost + _pathFwd.TotalCost;

        Pathfinder.TraceFindPathDetailBestSoFar(pathFwd.PathStep.Hex.Coords, pathRev.PathStep.Hex.Coords, BestSoFar);
      }
    }

    public static IDirectedPath MergePaths(IDirectedPath targetPath, IDirectedPath sourcePath) {
      if (sourcePath != null) {
        while (sourcePath.PathSoFar != null) {
          var hexside = sourcePath.PathStep.HexsideEntry;
          var cost    = sourcePath.TotalCost - (sourcePath = sourcePath.PathSoFar).TotalCost;
          targetPath  = targetPath.AddStep(sourcePath.PathStep.Hex, hexside.Reversed(), cost);
        }
      }
      return targetPath;
    }

    IDirectedPath   _pathFwd;
    IDirectedPath   _pathRev;
  }
}

