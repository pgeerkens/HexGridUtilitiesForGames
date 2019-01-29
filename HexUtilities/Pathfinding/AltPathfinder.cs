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
    using DirectedPath   = DirectedPathCollection;
    using IDirectedPath  = IDirectedPathCollection;
    using IPriorityQueue = IPriorityQueue<int, IDirectedPathCollection>;
    using IDictionary    = IDictionary<HexCoords, IDirectedPathCollection>;
    using StepCost       = Func<HexCoords, Hexside, short?>;

    /// <summary>This class is the shared algorithm of the forward- and backward-searches, doing the real work.</summary>
    /// <remarks>
    /// C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).
    /// 
    /// Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.
    /// <see cref="StandardPathfinder"/>
    /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
    /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
    /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
    /// </remarks>
    [DebuggerDisplay("")]
    internal sealed class AltPathfinder{
        /// <param name="pathHalves"></param>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="start">Start hex for this half of the bidirectional path search.</param>
        /// <param name="goal">Goal hex for this this half of the bidirectional path search.</param>
        /// <param name="closedSet"></param>
        /// <param name="isForward"></param>
        public AltPathfinder(IPathHalves pathHalves, bool isForward) {
            IsForward   = isForward;
            ClosedSet   = pathHalves.ClosedSet;
            StartCoords = pathHalves.Start;
            GoalCoords  = pathHalves.Goal;
            Landmarks   = pathHalves.Board.Landmarks;
            OpenSet     = new Dictionary<HexCoords, IDirectedPath>();
            Queue       = HotPriorityQueue.New<IDirectedPath>(0,256);
            TryStepCost = IsForward ? (StepCost)pathHalves.Board.TryEntryCost : pathHalves.Board.TryExitCost;
            PathHalves  = pathHalves;

            PathfinderExtensions.TraceFindPathDetailDirection(Direction, GoalCoords - StartCoords);

            StartPath(IsForward ? StartCoords : GoalCoords);
        }

        #region Properties
        /// <summary>The <see cref="ISet{HexCoords}"/> of all hexes expanded in finding the shortest-path.</summary>
        public  ISet<HexCoords>     ClosedSet   { get; }
        public  AltPathfinder       Partner     { get; internal set; }

        private int                 BestSoFar   => PathHalves.BestSoFar;
        private string              Direction   => IsForward ? "Fwd" : "Rev";
        /// <summary>The goal hex for this directional path search (Target for Fwd; Source for Rev).</summary>
        private HexCoords           GoalCoords  { get; }
        private bool                IsForward   { get; }
        private ILandmarkCollection Landmarks   { get; }
        private IDictionary         OpenSet     { get; }
        private IPathHalves         PathHalves  { get; }
        private IPriorityQueue      Queue       { get; }
        /// <summary>The start hex for this directional path search (Source for Fwd; Target for Rev).</summary>
        private HexCoords           StartCoords { get; }
        #endregion

        #region Methods
        public  bool     IsFinished(){
            if(Queue.TryDequeue(out var item)) {
                var path   = item.Value;
                var coords = path.PathStep.Coords;

                OpenSet.Remove(coords);
                if(!ClosedSet.Contains(coords)) {
                    PathfinderExtensions.TraceFindPathDequeue(GetType().Name, coords, path, item.Key, 0);

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

        private void     ExpandHex(IDirectedPath path, Hexside hexside) {
            var here  = path.PathStep.Coords;
            var there = here.GetNeighbour(hexside);
            if ( ! ClosedSet.Contains(there) ) {
                TryStepCost(here, hexside).IfHasValueDo( cost => {
                    if (path.TotalCost + cost < BestSoFar || ! OpenSet.ContainsKey(there)) {
                        Heuristic(there).IfHasValueDo( heuristic => {
                            var key     = path.TotalCost + cost + heuristic;
                            var newPath = path.AddStep(there,HexsideDirection(hexside),cost);

                            PathfinderExtensions.TraceFindPathEnqueue(there, key, 0);

                            if(!OpenSet.TryGetValue(there, out var oldPath)) {
                                OpenSet.Add(there, newPath);
                                Queue.Enqueue(key, newPath);
                            } else if(newPath.TotalCost < oldPath.TotalCost) {
                                OpenSet.Remove(there);
                                OpenSet.Add(there, newPath);
                                Queue.Enqueue(key, newPath);
                            }

                            SetBestSoFar(newPath, PartnerPath(there));
                        } );
                    }
                } );
            }
        }
        private int      FrontierMinimum() => (Queue.TryPeek(out var item) ? item.Key : int.MaxValue);
        private Hexside  HexsideDirection(Hexside hexside) => IsForward ? hexside.Reversed : hexside;
        private short?   Heuristic(HexCoords coords) { 
            var max = null as short?;
            Landmarks.ForEach( landmark => {
                max = from heuristic in LandmarkHeuristic(landmark,coords)
                        select max.Match(m => Math.Max(m,heuristic), ()=>heuristic); 
            } );
            return max;
        }
        private short?   LandmarkHeuristic(ILandmark landmark, HexCoords here)
        => ( from current in LandmarkPotential(landmark,here)
             from target in LandmarkPotential(landmark,GoalCoords)
             select (short)(current - target)
           );
        private short?   LandmarkPotential(ILandmark landmark, HexCoords coords)
                            => IsForward ? landmark.DistanceTo(coords) : landmark.DistanceFrom(coords);
        private void     SetBestSoFar(IDirectedPath pathSelf, IDirectedPath pathOther) {
            if (pathOther != null) {
                if (IsForward) {
                    PathHalves.SetBestSoFar(pathSelf, pathOther);
                } else {
                    PathHalves.SetBestSoFar(pathOther, pathSelf);
                }
            }
        }
        private void     StartPath(HexCoords firstHex) {
            var path = new DirectedPath(firstHex);
            OpenSet.Add(path.PathStep.Coords, path);

            if(Landmarks.Where(l => l.DistanceTo(path.PathStep.Coords).HasValue).Any()) Queue.Enqueue (0, path);
        }
        private StepCost TryStepCost { get; }

        private IDirectedPath PartnerPath(HexCoords coords)
        => Partner.OpenSet.TryGetValue(coords, out var path) ? path : null;
        #endregion
    }
}

