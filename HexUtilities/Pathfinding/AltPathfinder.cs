#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDictionary    = IDictionary<HexCoords,IDirectedPathCollection>;
    using IDirectedPath  = IDirectedPathCollection;
    using IPriorityQueue = IPriorityQueue<int,IDirectedPathCollection>;
    using DirectedPath   = DirectedPathCollection;
    using Potential      = Func<ILandmark,HexCoords,int>;
    using SetBest        = Action<IDirectedPathCollection,IDirectedPathCollection>;
    using StepCost       = Func<IHex,Hexside,int>;

/// <summary>A fast efficient serial implementation of <b>Bidirectional ALT</b> (<b>A*</b> with <b>L</b>andmarks
/// and <b>T</b>riangle-inequality heuristic) <b>path-finding</b> on a <see cref="Hexgrid"/> map.</summary>
    /// <typeparam name="THex"></typeparam>
    public interface IAltPathfinder<THex> where THex: class,IHex {
        /// <summary>.</summary>
        IDictionary          OpenSet { get; }

        /// <summary>.</summary>
        IAltPathfinder<THex> Partner { get; set; }

        /// <summary>.</summary>
        int  FrontierMinimum { get; }

        /// <summary>.</summary>
        bool IsFinished();

        /// <summary>.</summary>
        int Heuristic(HexCoords coords);
    }

    /// <summary>The shared algorithm of the forward- and backward-half-searches using ALT.</summary>
    /// <remarks>
    /// C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).
    /// 
    /// Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.
    /// 
    /// <i>Source</i> and <i>Target</i> refer to the path beginning and ending hexes from the client 
    /// perspective; <c>Start</c> and <c>Goal</c> refer to the directional beginning and ending hexes
    /// of the path from the algorithmic perspective. In the case of a reverse half-search, such as
    /// in the implementation of PathfinderRev, the Target becomes the Start, and the Source becomes
    /// the Goal, rather than the usual other way around.
    /// 
    /// <see cref="BidirectionalAltPathfinder"/>
    /// <see cref="PathHalves{THex}"/>
    /// </remarks>
    /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
    /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
    /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
    [DebuggerDisplay("")]
    internal class AltPathfinder<THex>: IAltPathfinder<THex> where THex:class,IHex {
        /// <summary>.</summary>
        /// <param name="pathHalves"></param>
        /// <param name="isForward"></param>
        public AltPathfinder(IPathHalves<THex> pathHalves, bool isForward) {
            int exitCost (IHex hex, Hexside hexside) => hex.ExitCost(hexside);
            int entryCost(IHex hex, Hexside hexside) => hex.EntryCost(hexside);

            PathHalves   = pathHalves;
            Board        = pathHalves.Board;
            ClosedSet    = pathHalves.ClosedSet;
            Start        = isForward ? pathHalves.Source : pathHalves.Target;
            Goal         = isForward ? pathHalves.Target : pathHalves.Source;
            StepCost     = isForward ? (StepCost)exitCost : entryCost;
            Potential    = isForward ? (Potential)((l,c) => l.DistanceFrom(c))
                                                : ((l,c) => l.DistanceTo(c));
            SetBestSoFar = isForward ? (SetBest)((s,p) => PathHalves.SetBestSoFar(p,s))
                                              : ((s,p) => PathHalves.SetBestSoFar(s,p));
            HexsideDirection = isForward ? (Func<Hexside,Hexside>)(hexside => hexside) 
                                         : (hexside => hexside.Reversed);

            OpenSet    = new Dictionary<HexCoords, IDirectedPath>();
            Queue      = HotPriorityQueue.New<IDirectedPath>(0,256);

            $"ALT {(isForward?"Fwd":"Rev")}".TraceFindPathDetailDirection( Goal.Coords - Start.Coords);

            StartPath(Start);
        }

        public  IDictionary           OpenSet      { get; }
        public  IAltPathfinder<THex>  Partner      { get; set; }
        public  int                   FrontierMinimum
                                      => Queue.TryPeek(out var item) ? item.Key : int.MaxValue;

        private ILandmarkBoard<THex>  Board        { get; }
        private ISet<HexCoords>       ClosedSet    { get; }
        private THex                  Source       { get; }
        private THex                  Target       { get; }
        private int                   BestSoFar    => PathHalves.BestSoFar;
        private ILandmarkCollection   Landmarks    => Board.Landmarks;
        private IPathHalves<THex>     PathHalves   { get; }
        /// <summary>The start hex for this directional path search (Source for Fwd; Target for Rev).</summary>
        private THex                  Start        { get; }
        /// <summary>The goal hex for this directional path search (Target for Fwd; Source for Rev).</summary>
        private THex                  Goal         { get; }
        private IPriorityQueue        Queue        { get; }
        private Potential             Potential    { get; }
        private SetBest               SetBestSoFar { get; }
        private StepCost              StepCost     { get; }
        private Func<Hexside,Hexside> HexsideDirection { get; }

        #region Methods
        public   bool          IsFinished(){
            if (Queue.TryDequeue(out var item)) {
                var path   = item.Value;
                var coords = path.PathStep.Coords;

                OpenSet.Remove(coords);
                if (!ClosedSet.Contains(coords)) {
                    coords.TraceFindPathDequeue(GetType().Name,path,item.Key,0);

                    if (item.Key < BestSoFar
                    &&  path.TotalCost + Partner.FrontierMinimum - Partner.Heuristic(coords) < BestSoFar
                    ) {
                        Hexside.ForEach(hexside => ExpandHex(path,hexside));
                    }
                    ClosedSet.Add(coords);
                }
                return !Queue.Any();
            }
            return true;
        }

        public   int           Heuristic(HexCoords coords) => Landmarks.Max(landmark => LandmarkHeuristic(landmark,coords));

        private  void          StartPath(IHex start) {
            var path = new DirectedPath(start);
            OpenSet.Add(path.PathStep.Coords, path);

            if(Landmarks.Where(l => l.DistanceTo(path.PathStep.Coords) > 0).Any())
                Queue.Enqueue (0, path);
        }
        private  void          ExpandHex(IDirectedPath path, Hexside hexside) {
            ( from here in Board[path.PathStep.Coords]
              from there in Board[here.Coords.GetNeighbour(hexside)]
              where here != null  &&  there != null
              select new {here, there}
            ).IfHasValueDo( tuple => ExpandHex(path, hexside, tuple.here, tuple.there) );
        }
        private  void          ExpandHex(IDirectedPath path, Hexside hexside, IHex here, IHex there) {
            if ( ! ClosedSet.Contains(there.Coords) ) {
                var cost = StepCost(here, hexside);
                if( (cost > 0)
                &&  (path.TotalCost+cost < BestSoFar  ||  ! OpenSet.ContainsKey(there.Coords))
                ) {
                    var key     = path.TotalCost + cost + Heuristic(there.Coords);
                    var newPath = path.AddStep(there,HexsideDirection(hexside),cost);

                    there.Coords.TraceFindPathEnqueue(key, 0);

                    if (!OpenSet.TryGetValue(there.Coords,out var oldPath)) {
                        OpenSet.Add(there.Coords,newPath);
                        Queue.Enqueue(key,newPath);
                    } else if (newPath.TotalCost < oldPath.TotalCost) {
                        OpenSet.Remove(there.Coords);
                        OpenSet.Add(there.Coords,newPath);
                        Queue.Enqueue(key,newPath);
                    }

                    SetBestSoFar(newPath, GetPartnerPath(there.Coords));
                }
            }
        }
        private  IDirectedPath GetPartnerPath(HexCoords coords) {
            Partner.OpenSet.TryGetValue(coords,out var path);
            return path;
        }
        private  int           LandmarkHeuristic(ILandmark landmark,HexCoords here)
        => Potential(landmark,here) - Potential(landmark,Goal.Coords);

        #endregion
    }
}

