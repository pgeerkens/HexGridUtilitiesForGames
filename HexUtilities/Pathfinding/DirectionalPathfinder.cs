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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  using DirectedPath    = DirectedPathCollection;
  using IDirectedPath   = IDirectedPathCollection;
  using IPriorityQueue  = IPriorityQueue<int,IDirectedPathCollection>;
  using IDictionary     = IDictionary<HexCoords,IDirectedPathCollection>;

  /// <summary>The shared algorithm of the forward- and backward-searches.</summary>
  [DebuggerDisplay("")]
  [ContractClass(typeof(DirectionalPathfinderContract))]
  internal abstract class DirectionalPathfinder : Pathfinder {
    // Common settings for both directions
    /// <param name="board">Board on which this path search is taking place.</param>
    /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
    /// <param name="start">Start hex for this half of the bidirectional path search.</param>
    /// <param name="goal">Goal hex for this this half of the bidirectional path search.</param>
    /// <param name="pathHalves"></param>
    protected DirectionalPathfinder(INavigableBoard board, ILandmarkCollection landmarks,
                                    HexCoords start, HexCoords goal, IPathHalves pathHalves)
    : base(start, goal, pathHalves.ClosedSet) {

      _landmarks  = landmarks;
      _pathHalves = pathHalves;
      _openSet    = new Dictionary<HexCoords, IDirectedPath>();
#if UseSortedDictionary
      _queue      = HotPriorityQueue.NewWithSortedDictionary<IDirectedPath>(0,256);
#else
      _queue      = HotPriorityQueue.New<IDirectedPath>(0,256);
#endif
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    }

    #region Properties
    private     int                   BestSoFar   { get {return PathHalves.BestSoFar;} }
    protected   ILandmarkCollection   Landmarks   { get {return _landmarks;} } readonly ILandmarkCollection _landmarks;
    internal    DirectionalPathfinder Partner     { get; set; }
    protected   IPathHalves           PathHalves  { get {return _pathHalves;} } readonly IPathHalves _pathHalves;

    /// <summary>The start hex for this directional path search (Source for Fwd; Target for Rev).</summary>
    protected abstract HexCoords      StartCoords { get; }
    /// <summary>The goal hex for this directional path search (Target for Fwd; Source for Rev).</summary>
    protected abstract HexCoords      GoalCoords  { get; }

    protected     IDictionary         OpenSet     { get {return _openSet;} } readonly IDictionary _openSet;
    protected     IPriorityQueue      Queue       { get {return _queue;} } readonly IPriorityQueue _queue;
    #endregion

    #region Methods
    private             void          ExpandHex(IDirectedPath path, Hexside hexside) {
      var here  = path.PathStep.Coords;
      var there = here.GetNeighbour(hexside);
      if ( ! ClosedSet.Contains(there) ) {
        TryStepCost(here, hexside).IfHasValueDo( cost => {
          if (path.TotalCost + cost < BestSoFar || ! OpenSet.ContainsKey(there)) {
            Heuristic(there).IfHasValueDo( heuristic => {
              var key     = path.TotalCost + cost + heuristic;
              var newPath = path.AddStep(there,HexsideDirection(hexside),cost);

              TraceFindPathEnqueue(there, key, 0);

              IDirectedPath oldPath;
              if ( ! OpenSet.TryGetValue(there, out oldPath))  {
                OpenSet.Add(there, newPath);
                Queue.Enqueue(key, newPath);
              } else if (newPath.TotalCost < oldPath.TotalCost) {
                OpenSet.Remove(there);
                OpenSet.Add(there, newPath);
                Queue.Enqueue(key, newPath);
              }

              SetBestSoFar(newPath, GetPartnerPath(there));
            } );
          }
        } );
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
    private             short?  Heuristic(HexCoords coords) { 
      var max = null as short?;
      Landmarks.ForEach( landmark => {
        max = from heuristic in LandmarkHeuristic(landmark,coords)
              select max.Match(m => Math.Max(m,heuristic), ()=>heuristic); 
      } );
      return max;
    }
    /// <summary>TODO</summary>
    /// <param name="hexside"></param>
    /// <returns></returns>
    protected abstract  Hexside       HexsideDirection(Hexside hexside);
    internal            bool          IsFinished(){
      HexKeyValuePair<int,IDirectedPath> item;
      if ( Queue.TryDequeue(out item)) {
        var path   = item.Value;
        var coords = path.PathStep.Coords;

        OpenSet.Remove(coords);
        if( ! ClosedSet.Contains(coords) ) {

          TraceFindPathDequeue(GetType().Name,coords, path.TotalCost, path.HexsideExit, item.Key, 0);

          if (item.Key < BestSoFar)
            Partner.Heuristic(coords).IfHasValueDo( heuristic => {
              if (path.TotalCost + Partner.FrontierMinimum() - heuristic < BestSoFar)
                Hexside.ForEach(hexside => ExpandHex(path, hexside));
            } );
          ClosedSet.Add(coords);
        }
        return ! Queue.Any();
      }
      return true;
    }
    private             short?  LandmarkHeuristic(ILandmark landmark, HexCoords here){
      return ( from current in LandmarkPotential(landmark,here)
               from goal in LandmarkPotential(landmark,GoalCoords)
               select (short)(current - goal)
             );
    }
    protected abstract  short?  LandmarkPotential(ILandmark landmark, HexCoords coords);
    protected abstract  void          SetBestSoFar(IDirectedPath fwdPath, IDirectedPath revPath);
    protected           void          StartPath(HexCoords startCoords) {
      var path            = new DirectedPath(startCoords);
      OpenSet.Add(path.PathStep.Coords, path);

      if(Landmarks.Where(l => l.DistanceTo(path.PathStep.Coords).HasValue).Any())
        Queue.Enqueue (0, path);
    }
    protected abstract  short?  TryStepCost(HexCoords here, Hexside hexside);
    #endregion

    /// <summary>Create a new instance of <see cref="PathfinderForward"/>, a forward-searching <see cref="DirectionalPathfinder"/>.</summary>
    /// <param name="board">Board on which this path search is taking place.</param>
    /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
    /// <param name="source">Source hex for this path search, the start for the directional path search.</param>
    /// <param name="target">Target hex for this path search, the goal for the directional path search.</param>
    /// <param name="pathHalves"></param>
    /// <returns></returns>
    public static DirectionalPathfinder NewForward(INavigableBoard board, ILandmarkCollection landmarks,
        HexCoords source, HexCoords target, IPathHalves pathHalves
    ) {
      return new PathfinderForward(board, landmarks, source, target, pathHalves);
    }
    /// <summary>Create a new instance of <see cref="PathfinderReverse"/>, a backward-searching <see cref="DirectionalPathfinder"/>.</summary>
    /// <param name="board">Board on which this path search is taking place.</param>
    /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
    /// <param name="source">Source hex for this path search, the start for the directional path search.</param>
    /// <param name="target">Target hex for this path search, the goal for the directional path search.</param>
    /// <param name="pathHalves"></param>
    /// <returns></returns>
    public static DirectionalPathfinder NewReverse(INavigableBoard board, ILandmarkCollection landmarks,
        HexCoords source, HexCoords target, IPathHalves pathHalves
    ) {
      return new PathfinderReverse(board, landmarks, source, target, pathHalves);
    }

    /// <summary>A <see cref="DirectedPath"/> from start to join-point obtained by searching forwards from start.</summary>
    /// <remarks>
    /// <i>Source</i> and <i>Target</i> refer to the path beginning and ending hexes from the client 
    /// perspective; <c>Start</c> and <c>Goal</c> refer to the directional beginning and ending hexes
    /// of the path from the algorithmic perspective. In the case of a reverse half-search, such as
    /// in the implementation of PathfinderRev, the Target becomes the Start, and the Source becomes
    /// the Goal, rather than the usual other way around.
    /// </remarks>
    private sealed class PathfinderForward : DirectionalPathfinder {
      /// <summary>Create a new instance of <see cref="PathfinderForward"/>, a forward-searching <see cref="DirectionalPathfinder"/>.</summary>
      /// <param name="board">Board on which this path search is taking place.</param>
      /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
      /// <param name="source">Source hex for this path search, the start for the directional path search.</param>
      /// <param name="target">Target hex for this path search, the goal for the directional path search.</param>
      /// <param name="pathHalves"></param>
      internal PathfinderForward(INavigableBoard board, ILandmarkCollection landmarks,
        HexCoords source, HexCoords target, IPathHalves pathHalves)
      : base (board,landmarks,source,target,pathHalves) {
        TraceFindPathDetailDirection("Fwd", GoalCoords - StartCoords);

        _tryStepCost = board.TryExitCost;
        StartPath(StartCoords);
      }

      protected override HexCoords     StartCoords { get {return Source;} }
      protected override HexCoords     GoalCoords  { get {return Target;} }

      protected override Hexside       HexsideDirection(Hexside hexside) { return hexside; }
      protected override short?  LandmarkPotential(ILandmark landmark, HexCoords coords) {
        return landmark.DistanceFrom(coords);
      }
      protected override void          SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
        if (partnerPath != null)
          PathHalves.SetBestSoFar(partnerPath, selfPath);
      }
      protected override short?  TryStepCost(HexCoords here, Hexside hexside) {
        return _tryStepCost(here,hexside);
      } readonly Func<HexCoords,Hexside,short?> _tryStepCost;
      public    override IDirectedPath PathForward { get { throw new NotImplementedException(); } }
      public    override IDirectedPath PathReverse { get { throw new NotImplementedException(); } }
    }

    /// <summary>A <see cref="DirectedPath"/> from join-point to goal obtained by searching backwards from goal.</summary>
    /// <remarks>
    /// <i>Source</i> and <i>Target</i> refer to the path beginning and ending hexes from the client 
    /// perspective; <c>Start</c> and <c>Goal</c> refer to the directional beginning and ending hexes
    /// of the path from the algorithmic perspective. In the case of a reverse half-search, such as
    /// in the implementation of BidirectionalPathfinder, the Target becomes the Start, and the
    /// Source becomes the Goal. This transition occurs in the constructor of <see cref="Pathfinder"/>.
    /// </remarks>
    private sealed class PathfinderReverse : DirectionalPathfinder {
      /// <summary>Create a new instance of <see cref="PathfinderReverse"/>, a backward-searching <see cref="DirectionalPathfinder"/>.</summary>
      /// <param name="board">Board on which this path search is taking place.</param>
      /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
      /// <param name="source">Source hex for this path search, the goal for the directional path search.</param>
      /// <param name="target">Target hex for this path search, the start for the directional path search.</param>
      /// <param name="pathHalves"></param>
      internal PathfinderReverse(INavigableBoard board, ILandmarkCollection landmarks,
        HexCoords source, HexCoords target, IPathHalves pathHalves)
      : base (board,landmarks,source,target,pathHalves)  {
        TraceFindPathDetailDirection("Fwd", GoalCoords - StartCoords);

        _tryStepCost = board.TryEntryCost;
        StartPath(StartCoords);
      }

      protected override HexCoords     StartCoords { get {return Target;} }
      protected override HexCoords     GoalCoords  { get {return Source;} }

      protected override Hexside       HexsideDirection(Hexside hexside) { return hexside.Reversed; }
      protected override short?  LandmarkPotential(ILandmark landmark, HexCoords coords) {
        return landmark.DistanceTo(coords);
      }
      protected override void          SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
        if (partnerPath != null)
          PathHalves.SetBestSoFar(selfPath, partnerPath);
      }
      protected override short?  TryStepCost(HexCoords here, Hexside hexside) {
        return _tryStepCost(here,hexside);
      } readonly Func<HexCoords,Hexside,short?> _tryStepCost;
      public    override IDirectedPath PathForward { get { throw new NotImplementedException(); } }
      public    override IDirectedPath PathReverse { get { throw new NotImplementedException(); } }
   }
  }

  [ContractClassFor(typeof(DirectionalPathfinder))]
  internal abstract class DirectionalPathfinderContract : DirectionalPathfinder {
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private DirectionalPathfinderContract() : base(null,null, HexCoords.EmptyUser, HexCoords.EmptyUser, null){ }

    protected override Hexside      HexsideDirection(Hexside hexside) {
      return default(Hexside);
    }
    protected override short? LandmarkPotential(ILandmark landmark, HexCoords coords) {
      return default(int);
    }
    protected override void         SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
    }
    protected override short? TryStepCost(HexCoords here, Hexside hexside) {
      return default(short?);
    }
  }
}
