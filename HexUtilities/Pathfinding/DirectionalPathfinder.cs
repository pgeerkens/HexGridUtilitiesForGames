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
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using DirectedPath    = DirectedPathCollection;
    using IDirectedPath   = IDirectedPathCollection;
    using IPriorityQueue  = IPriorityQueue<int,IDirectedPathCollection>;
    using IDictionary     = IDictionary<HexCoords,IDirectedPathCollection>;

    /// <summary>The shared algorithm of the forward- and backward-searches.</summary>
    [DebuggerDisplay("")]
    internal abstract class DirectionalPathfinder : Pathfinder {
        // Common settings for both directions
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
        /// <param name="start">Start hex for this half of the bidirectional path search.</param>
        /// <param name="goal">Goal hex for this this half of the bidirectional path search.</param>
        /// <param name="pathHalves"></param>
        /// <param name="tryStepCost"></param>
        /// <param name="direction"></param>
        protected DirectionalPathfinder(ILandmarkCollection landmarks,
                                        HexCoords start, HexCoords goal, IPathHalves pathHalves,
                                        Func<HexCoords,Hexside,short?> tryStepCost, string direction)
        : base(start, goal, pathHalves.ClosedSet) {

            StartCoords = start;
            GoalCoords  = goal;
            Landmarks   = landmarks;
            PathHalves  = pathHalves;
            OpenSet     = new Dictionary<HexCoords, IDirectedPath>();
        #if UseSortedDictionary
            Queue       = HotPriorityQueue.NewWithSortedDictionary<IDirectedPath>(0,256);
        #else
            Queue       = HotPriorityQueue.New<IDirectedPath>(0,256);
        #endif
            TryStepCost = tryStepCost;

            TraceFindPathDetailDirection(direction, GoalCoords - StartCoords);
        }

        #region Properties
        /// <summary>The goal hex for this directional path search (Target for Fwd; Source for Rev).</summary>
        protected  HexCoords             GoalCoords  { get; }
        /// <summary>The start hex for this directional path search (Source for Fwd; Target for Rev).</summary>
        protected  HexCoords             StartCoords { get; }
        protected  ILandmarkCollection   Landmarks   { get; }
        protected  IDictionary           OpenSet     { get; }
        protected  IPathHalves           PathHalves  { get; }
        protected  IPriorityQueue        Queue       { get; }

        internal   DirectionalPathfinder Partner     { get; set; }

        private    int                   BestSoFar   => PathHalves.BestSoFar;
        #endregion

        #region Methods
        /// <summary>TODO</summary>
        /// <param name="hexside"></param>
        /// <returns></returns>
        protected abstract  Hexside HexsideDirection(Hexside hexside);
        
        protected abstract  short?  LandmarkPotential(ILandmark landmark, HexCoords coords);
        protected abstract  void    SetBestSoFar(IDirectedPath fwdPath, IDirectedPath revPath);
        protected           void    StartPath(HexCoords firstHex) {
            var path = new DirectedPath(firstHex);
            OpenSet.Add(path.PathStep.Coords, path);

            if(Landmarks.Where(l => l.DistanceTo(path.PathStep.Coords).HasValue).Any()) Queue.Enqueue (0, path);
        }
        protected Func<HexCoords,Hexside,short?>  TryStepCost { get; }

        internal            bool    IsFinished(){
            if(Queue.TryDequeue(out var item)) {
                var path   = item.Value;
                var coords = path.PathStep.Coords;

                OpenSet.Remove(coords);
                if(!ClosedSet.Contains(coords)) {
                    TraceFindPathDequeue(GetType().Name, coords, path.TotalCost, path.HexsideExit, item.Key, 0);

                    if(item.Key < BestSoFar) {
                        Partner.Heuristic(coords).IfHasValueDo(heuristic => {
                            if(path.TotalCost + Partner.FrontierMinimum() - heuristic < BestSoFar) {
                                Hexside.ForEach(hexside => ExpandHex(path, hexside));
                            }
                        });
                    }
                    ClosedSet.Add(coords);
                }
                return ! Queue.Any();
            }
            return true;
        }

        private void  ExpandHex(IDirectedPath path, Hexside hexside) {
            var here  = path.PathStep.Coords;
            var there = here.GetNeighbour(hexside);
            if ( ! ClosedSet.Contains(there) ) {
                TryStepCost(here, hexside).IfHasValueDo( cost => {
                    if (path.TotalCost + cost < BestSoFar || ! OpenSet.ContainsKey(there)) {
                        Heuristic(there).IfHasValueDo( heuristic => {
                            var key     = path.TotalCost + cost + heuristic;
                            var newPath = path.AddStep(there,HexsideDirection(hexside),cost);

                            TraceFindPathEnqueue(there, key, 0);

                            if(!OpenSet.TryGetValue(there, out var oldPath)) {
                                OpenSet.Add(there, newPath);
                                Queue.Enqueue(key, newPath);
                            } else if(newPath.TotalCost < oldPath.TotalCost) {
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
        private int   FrontierMinimum() => (Queue.TryPeek(out var item) ? item.Key : int.MaxValue);
        private IDirectedPath GetPartnerPath(HexCoords coords) {
            Partner.OpenSet.TryGetValue(coords, out var path);
            return path;
        }
        private short? Heuristic(HexCoords coords) { 
            var max = null as short?;
            Landmarks.ForEach( landmark => {
                max = from heuristic in LandmarkHeuristic(landmark,coords)
                      select max.Match(m => Math.Max(m,heuristic), ()=>heuristic); 
            } );
            return max;
        }
        private short? LandmarkHeuristic(ILandmark landmark, HexCoords here)
        => ( from current in LandmarkPotential(landmark,here)
             from target in LandmarkPotential(landmark,GoalCoords)
             select (short)(current - target)
           );
        #endregion

        /// <summary>Create a new instance of <see cref="PathfinderForward"/>, a forward-searching <see cref="DirectionalPathfinder"/>.</summary>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
        /// <param name="source">Source hex for this path search, the start for the directional path search.</param>
        /// <param name="target">Target hex for this path search, the goal for the directional path search.</param>
        /// <param name="pathHalves"></param>
        /// <returns></returns>
        public static DirectionalPathfinder NewForward(INavigableBoard board, ILandmarkCollection landmarks,
                HexCoords source, HexCoords target, IPathHalves pathHalves)
        => new PathfinderForward(board, landmarks, source, target, pathHalves);
        
        /// <summary>Create a new instance of <see cref="PathfinderReverse"/>, a backward-searching <see cref="DirectionalPathfinder"/>.</summary>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="landmarks">The set of landmarks with pre-calculated landmark distances to use for heuristic calculation.</param>
        /// <param name="source">Source hex for this path search, the start for the directional path search.</param>
        /// <param name="target">Target hex for this path search, the goal for the directional path search.</param>
        /// <param name="pathHalves"></param>
        /// <returns></returns>
        public static DirectionalPathfinder NewReverse(INavigableBoard board, ILandmarkCollection landmarks,
                HexCoords source, HexCoords target, IPathHalves pathHalves)
        => new PathfinderReverse(board, landmarks, source, target, pathHalves);
        
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
            /// <param name="start">Source hex for this path search, the start for the directional path search.</param>
            /// <param name="goal">Target hex for this path search, the goal for the directional path search.</param>
            /// <param name="pathHalves"></param>
            internal PathfinderForward(INavigableBoard board, ILandmarkCollection landmarks,
                    HexCoords start, HexCoords goal, IPathHalves pathHalves)
            : base (landmarks, start, goal, pathHalves, board.TryExitCost, "Fwd")
            => StartPath(start);

            protected override Hexside   HexsideDirection(Hexside hexside) => hexside;
            protected override short?    LandmarkPotential(ILandmark landmark, HexCoords coords)
            => landmark.DistanceTo(coords);
           
            protected override void      SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
                if (partnerPath != null) PathHalves.SetBestSoFar(partnerPath, selfPath);
            }

            public    override IDirectedPath PathForward => throw new NotImplementedException();
            public    override IDirectedPath PathReverse => throw new NotImplementedException();
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
            /// <param name="start">Source hex for this path search, the goal for the directional path search.</param>
            /// <param name="goal">Target hex for this path search, the start for the directional path search.</param>
            /// <param name="pathHalves"></param>
            internal PathfinderReverse(INavigableBoard board, ILandmarkCollection landmarks,
                    HexCoords start, HexCoords goal, IPathHalves pathHalves)
            : base (landmarks, start, goal, pathHalves, board.TryEntryCost, "Rev")
            => StartPath(goal);

            protected override Hexside   HexsideDirection(Hexside hexside) => hexside.Reversed;
            protected override short?    LandmarkPotential(ILandmark landmark, HexCoords coords)
            => landmark.DistanceFrom(coords);
            protected override void      SetBestSoFar(IDirectedPath selfPath, IDirectedPath partnerPath) {
                if (partnerPath != null) PathHalves.SetBestSoFar(selfPath, partnerPath);
            }

            public    override IDirectedPath PathForward => throw new NotImplementedException();
            public    override IDirectedPath PathReverse => throw new NotImplementedException();
        }
    }
}
