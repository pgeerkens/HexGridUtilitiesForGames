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

#pragma warning disable 1587
/// <summary>A fast efficient serial implementation of <b>Bidirectional ALT</b> (<b>A*</b> with <b>L</b>andmarks
/// and <b>T</b>riangle-inequality heuristic) <b>path-finding</b> on a <see cref="Hexgrid"/> map.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.Pathfinding {
  using DirectedPath    = DirectedPathCollection;
  using IDirectedPath   = IDirectedPathCollection;
  using IPriorityQueue  = IPriorityQueue<int,IDirectedPathCollection>;
  using IDictionary     = IDictionary<HexCoords,IDirectedPathCollection>;

  /// <summary>Interface of common data structures exposed to <see cref="BidirectionalPathfinder.DirectionalPathfinder"/>s.</summary>
  internal interface IPathHalves {
    int              BestSoFar { get; }
    ISet<HexCoords>  ClosedSet { get; }

    void             SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd);
  }

  /// <summary>C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).</summary>
  /// <remarks>Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.</remarks>
  /// <see cref="UnidirectionalPathfinder"/>
  /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
  /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
  /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
  /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
  public sealed class BidirectionalPathfinder : Pathfinder, IPathHalves {
    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <see cref="FindDirectedPathRev"/>
    public static IDirectedPath FindDirectedPathFwd(INavigableBoard<IHex> board, IHex source, IHex target) {
      return (new BidirectionalPathfinder(board, source, target)).PathRev;
    }
    /// <summary>As <see cref="FindDirectedPathFwd"/>, except with the steps stacked in reverse for more convenient use.</summary>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <remarks>
    /// The path steps are ordered in reverse as the forward half-path has been stacked 
    /// onto the reverse half-path during post-processing, instead of the reverse.
    /// </remarks>
    /// <see cref="FindDirectedPathFwd"/>
    public static IDirectedPath FindDirectedPathRev(INavigableBoard<IHex> board, IHex source, IHex target) {
      return (new BidirectionalPathfinder(board, source, target)).PathFwd;
    }

    /// <summary>Calculates an <c>IPath</c> for the optimal path from coordinates .</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    public BidirectionalPathfinder(INavigableBoard<IHex> board, IHex source, IHex target) 
    : base(board, source, target, new HashSet<HexCoords>()) {
      Pathfinder.TraceFindPathDetailInit(Source.Coords, Target.Coords);

      _bestSoFar        = int.MaxValue;
      var pathfinderFwd = new BidirectionalPathfinder.DirectionalPathfinder.PathfinderRev(board, source, target, this);
      var pathfinderRev = new BidirectionalPathfinder.DirectionalPathfinder.PathfinderFwd(board, source, target, this);

      // Alternate searching from each direction and calling the other direction
      pathfinderFwd.Partner = pathfinderRev;
      var pathfinder        = pathfinderRev.Partner
                            = pathfinderFwd;

      while (! pathfinder.IsFinished())  pathfinder = pathfinder.Partner; 

      TraceFindPathDone(ClosedSet.Count);
    }
   
    #region Properties
    /// <summary>Retrieve the found path in walking order: first step at top of stack to target at bottom.</summary>
    /// <see cref="BidirectionalPathfinder.PathRev"/>
    public    IDirectedPath       PathFwd   { get { return MergePaths(_pathFwd, _pathRev); } }
    /// <summary>Retrieve the found path in reverse walking order: target at top of stack to first step at bottom.</summary>
    /// <see cref="BidirectionalPathfinder.PathFwd"/>
    public    IDirectedPath       PathRev   { get { return MergePaths(_pathRev, _pathFwd); } }

    /// <see cref="PathFwd"/>
    [Obsolete("Deprecated - use property PathFwd instead.")]
    public    IDirectedPath       Path      { get {return PathFwd;} }
    #endregion

    #region IPathHalves implementation
    /// <summary>Retrieves the cost of the shortest path found so far.</summary>
              int     IPathHalves.BestSoFar { get {return _bestSoFar;} } int _bestSoFar;
    /// <summary>Updates the record of the shortest path found so far.</summary>
    /// <param name="pathFwd">The half-path obtained by searching backward from the target (so stacked forwards).</param>
    /// <param name="pathRev">The half-path obtained by searching forward from the source (so stacked backwards).</param>
              void    IPathHalves.SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd) {
      if (pathFwd==null  ||  pathRev==null) return;

      if( pathFwd.TotalCost + pathRev.TotalCost < _bestSoFar) {
        _pathRev    = pathRev; 
        _pathFwd    = pathFwd; 
        _bestSoFar  = _pathRev.TotalCost + _pathFwd.TotalCost;

        Pathfinder.TraceFindPathDetailBestSoFar(pathFwd.PathStep.Hex.Coords, pathRev.PathStep.Hex.Coords, _bestSoFar);
      }
    }

              /// <summary>TODO</summary>
    private static  IDirectedPath MergePaths(IDirectedPath targetPath, IDirectedPath sourcePath) {
      if (sourcePath != null) {
        while (sourcePath.PathSoFar != null) {
          var hexside = sourcePath.PathStep.HexsideExit;
          var cost    = sourcePath.TotalCost - (sourcePath = sourcePath.PathSoFar).TotalCost;
          targetPath  = targetPath.AddStep(sourcePath.PathStep.Hex, hexside, cost);
        }
      }
      return targetPath;
    }

    /// <summary>The half-path obtained by searching backward from the target (so stacked forwards).</summary>
    private   IDirectedPath       _pathFwd;
    /// <summary>The half-path obtained by searching forward from the source (so stacked backwards).</summary>
    private   IDirectedPath       _pathRev;
    #endregion

    /// <summary>The shared algorithm of the forward- and backward-searches.</summary>
    [DebuggerDisplay("")]
    internal abstract class DirectionalPathfinder : Pathfinder {
      // Common settings for both directions
      /// <param name="board">Board on which this path search is taking place.</param>
      /// <param name="start">Start hex for this half of the bidirectional path search.</param>
      /// <param name="goal">Goal hex for this this half of the bidirectional path search.</param>
      /// <param name="pathHalves"></param>
      protected DirectionalPathfinder(INavigableBoard<IHex> board, IHex start, IHex goal, IPathHalves pathHalves)
      : base(board, start, goal, pathHalves.ClosedSet) {
        PathHalves  = pathHalves;
        OpenSet     = new Dictionary<HexCoords, IDirectedPath>();
        Queue       = new HotPriorityQueue<IDirectedPath>(0,256);
      }

      #region Properties
      private     int                   BestSoFar   { get {return PathHalves.BestSoFar;} }
      protected   ILandmarkCollection   Landmarks   { get {return Board.Landmarks;} }
      internal    DirectionalPathfinder Partner     { get; set; }
      protected   IPathHalves           PathHalves  { get; private set; }

      /// <summary>The start hex for this directional path search (Source for Fwd; Target for Rev).</summary>
      protected abstract IHex           Start       { get; }
      /// <summary>The goal hex for this directional path search (Target for Fwd; Source for Rev).</summary>
      protected abstract IHex           Goal        { get; }

      protected     IDictionary         OpenSet     { get; private set; }
      protected     IPriorityQueue      Queue       { get; private set; }
      #endregion

      #region Methods
      private             void          ExpandHex(IDirectedPath path, Hexside hexside) {
        var here  = path.PathStep.Hex;
        var there = Board[here.Coords.GetNeighbour(hexside)];
        if (there != null  &&  ! ClosedSet.Contains(there.Coords) ) {
          var cost = StepCost(here, hexside, there);
          if( (cost > 0)
          &&  (path.TotalCost+cost < BestSoFar  ||  ! OpenSet.ContainsKey(there.Coords))
          ) {
            var key     = path.TotalCost + cost + Heuristic(there.Coords);
            var newPath = path.AddStep(there,HexsideDirection(hexside),cost);

            TraceFindPathEnqueue(there.Coords, key, 0);

            IDirectedPath oldPath;
            if ( ! OpenSet.TryGetValue(there.Coords, out oldPath))  {
              OpenSet.Add(there.Coords, newPath);
              Queue.Enqueue(key, newPath);
            } else if (newPath.TotalCost < oldPath.TotalCost) {
              OpenSet.Remove(there.Coords);
              OpenSet.Add(there.Coords, newPath);
              Queue.Enqueue(key, newPath);
            }

            SetBestSoFar(newPath, GetPartnerPath(there.Coords));
          }
        }
      }
      private             int           FrontierMinimum() { 
        HexKeyValuePair<int,IDirectedPath> item;
        return (Queue.TryPeek(out item) ? item.Key : int.MaxValue); 
      }
      private             IDirectedPath GetPartnerPath(HexCoords coords) {
        IDirectedPath path;
        Partner.OpenSet.TryGetValue(coords,out path);
        return path;
      }
      private             int           Heuristic(HexCoords coords) { 
        return Landmarks.Max(landmark => LandmarkHeuristic(landmark,coords));
      }
      protected abstract  Hexside       HexsideDirection(Hexside hexside);
      internal            bool          IsFinished(){
        HexKeyValuePair<int,IDirectedPath> item;
        if ( Queue.TryDequeue(out item)) {
          var path   = item.Value;
          var coords = path.PathStep.Hex.Coords;

          OpenSet.Remove(coords);
          if( ! ClosedSet.Contains(coords) ) {

            TraceFindPathDequeue(GetType().Name,coords, path.TotalCost, path.HexsideExit, item.Key, 0);

            if (item.Key < BestSoFar
            &&  path.TotalCost + Partner.FrontierMinimum() - Partner.Heuristic(coords) < BestSoFar
            ) {
              HexsideExtensions.HexsideList.ForEach(hexside => ExpandHex(path, hexside));
            }
            ClosedSet.Add(coords);
          }
          return ! Queue.Any();
        }
        return true;
      }
      private             int           LandmarkHeuristic(ILandmark landmark, HexCoords here){
        return LandmarkPotential(landmark,here) - LandmarkPotential(landmark,Goal.Coords);
      }
      protected abstract  int           LandmarkPotential(ILandmark landmark, HexCoords coords);
      protected abstract  void          SetBestSoFar(IDirectedPath fwdPath, IDirectedPath revPath);
      protected           void          StartPath(IHex start) {
        var path            = new DirectedPath(start);
        OpenSet.Add(path.PathStep.Hex.Coords, path);
        Queue.Enqueue (0, path);
      }
      protected abstract  int           StepCost(IHex here, Hexside hexside, IHex there);
      #endregion

      /// <summary>A <see cref="DirectedPath"/> from start to join-point obtained by searching forwards from start.</summary>
      /// <remarks>
      /// <i>Source</i> and <i>Target</i> refer to the path beginning and ending hexes from the client 
      /// perspective; <c>Start</c> and <c>Goal</c> refer to the directional beginning and ending hexes
      /// of the path from the algorithmic perspective. In the case of a reverse half-search, such as
      /// in the implementation of PathfinderRev, the Target becomes the Start, and the Source becomes
      /// the Goal, rather than the usual other way around.
      /// </remarks>
      internal sealed class PathfinderFwd : DirectionalPathfinder {
        /// <summary>Create a new instance of <see cref="PathfinderFwd"/>, a forward-searching <see cref="DirectionalPathfinder"/>.</summary>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="source">Source hex for this path search, the start for the directional path search.</param>
        /// <param name="target">Target hex for this path search, the goal for the directional path search.</param>
        /// <param name="pathHalves"></param>
        internal PathfinderFwd(INavigableBoard<IHex> board, IHex source, IHex target, IPathHalves pathHalves)
        : base (board,source,target,pathHalves) {
          TraceFindPathDetailDirection("Fwd", Goal.Coords - Start.Coords);
          StartPath(Start);
        }

        protected override IHex    Start { get {return Source;} }
        protected override IHex    Goal  { get {return Target;} }

        protected override Hexside HexsideDirection(Hexside hexside) { return hexside; }
        protected override int     LandmarkPotential(ILandmark landmark, HexCoords coords) {
          return landmark.DistanceFrom(coords);
        }
        protected override void    SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
          PathHalves.SetBestSoFar(partnerPath, selfPath);
        }
        protected override int     StepCost(IHex here, Hexside hexside, IHex there) {
          return Board.GetDirectedCostToExit(here, HexsideDirection(hexside));
        }
      }

      /// <summary>A <see cref="DirectedPath"/> from join-point to goal obtained by searching backwards from goal.</summary>
      /// <remarks>
      /// <i>Source</i> and <i>Target</i> refer to the path beginning and ending hexes from the client 
      /// perspective; <c>Start</c> and <c>Goal</c> refer to the directional beginning and ending hexes
      /// of the path from the algorithmic perspective. In the case of a reverse half-search, such as
      /// in the implementation of BidirectionalPathfinder, the Target becomes the Start, and the
      /// Source becomes the Goal. This transition occurs in the constructor of <see cref="Pathfinder"/>.
      /// </remarks>
      internal sealed class PathfinderRev : DirectionalPathfinder {
        /// <summary>Create a new instance of <see cref="PathfinderRev"/>, a backward-searching <see cref="DirectionalPathfinder"/>.</summary>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="source">Source hex for this path search, the goal for the directional path search.</param>
        /// <param name="target">Target hex for this path search, the start for the directional path search.</param>
        /// <param name="pathHalves"></param>
        internal PathfinderRev(INavigableBoard<IHex> board, IHex source, IHex target, IPathHalves pathHalves)
        : base (board,source,target,pathHalves)  {
          TraceFindPathDetailDirection("Fwd", Goal.Coords - Start.Coords);
          StartPath(Start);
        }

        protected override IHex    Start { get {return Target;} }
        protected override IHex    Goal  { get {return Source;} }

        protected override Hexside HexsideDirection(Hexside hexside) { return hexside.Reversed(); }
        protected override int     LandmarkPotential(ILandmark landmark, HexCoords coords) {
          return landmark.DistanceTo(coords);
        }
        protected override void    SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
          PathHalves.SetBestSoFar(selfPath, partnerPath);
        }
        protected override int     StepCost(IHex here, Hexside hexside, IHex there) {
          return Board.GetDirectedCostToExit(there, HexsideDirection(hexside));
        }
      }
    }
  }
}

