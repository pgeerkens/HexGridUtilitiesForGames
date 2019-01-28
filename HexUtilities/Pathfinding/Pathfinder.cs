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
using System.Collections.Generic;
using System.Diagnostics;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary></summary>
    public abstract class Pathfinder : IPathfinder {
        /// <summary>TODO</summary>
        /// <param name="source">Source hex for this shortest-path search.</param>
        /// <param name="target">Target hex for this shortest-path search.</param>
        /// <param name="closedSet">Injected implementation of <see cref="ISet{HexCoords}"/>.</param>
        protected internal Pathfinder(HexCoords source, HexCoords target, ISet<HexCoords> closedSet) {
            ClosedSet = closedSet;
            Source    = source;
            Target    = target;
        }

        /// <summary>The <see cref="ISet{HexCoords}"/> of all hexes expanded in finding the shortest-path.</summary>
        public ISet<HexCoords>       ClosedSet { get; }
        /// <inheritdoc/>
        public HexCoords             Source    { get; }
        /// <inheritdoc/>
        public HexCoords             Target    { get; }

        /// <summary>Retrieve the found path in walking order: first step at top of stack to target at bottom.</summary>
        /// <remarks>
        /// The path steps are ordered normally as the forward half-path has been stacked 
        /// onto the reverse half-path during post-processing.
        /// </remarks>
        /// <see cref="PathReverse"/>
        public abstract IDirectedPath PathForward { get; }
        /// <summary>Retrieve the found path in reverse walking order: target at top of stack to first step at bottom.</summary>
        /// <remarks>
        /// The path steps are ordered in reverse as the reverse half-path has been stacked 
        /// onto the forward half-path during post-processing.
        /// </remarks>
        /// <see cref="LandmarkPathfinder.PathForward"/>
        public abstract IDirectedPath PathReverse { get; }

        /// <summary>Returns the result of stacking <paramref name="sourcePath"/> onto <paramref name="targetPath"/></summary>
        public static IDirectedPath MergePaths(IDirectedPath targetPath, IDirectedPath sourcePath) {
            if (targetPath == null  ||  sourcePath == null) return null;
            while (sourcePath.PathSoFar != null) {
                var hexside = sourcePath.PathStep.HexsideExit;
                var cost    = sourcePath.TotalCost - (sourcePath = sourcePath.PathSoFar).TotalCost;
                targetPath  = targetPath.AddStep(sourcePath.PathStep.Coords, hexside, cost);
          }
            return targetPath;
        }

        #region Conditional tracing routines
        /// <summary>If the conditional constant TRACE is defined: writes the search start- and goal-coords to the trace log.</summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        [Conditional("TRACE")]
        protected static void TraceFindPathDetailInit(HexCoords start, HexCoords goal)
        => Tracing.FindPathDetail.Trace(true, "Fwd: Find path from {0} to {1}:", start, goal);
        
        /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="vectorGoal"></param>
        [Conditional("TRACE")]
        protected static void TraceFindPathDetailDirection(string searchDirection,
                IntVector2D vectorGoal)
        => Tracing.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}", searchDirection, vectorGoal);
        
        /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="vectorGoal"></param>
        /// <param name="landmarkCoords"></param>
        [Conditional("TRACE")]
        protected static void TraceFindPathDetailDirection(string searchDirection,
                IntVector2D vectorGoal, HexCoords landmarkCoords)
        => Tracing.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}; and landmark at {2}", 
                                        searchDirection, vectorGoal, landmarkCoords);
        /// <summary>If the conditional constant TRACE is defined: writes the current direction pairing to the trace log.</summary>
        /// <param name="coordsFwd"></param>
        /// <param name="coordsRev"></param>
        /// <param name="bestSoFar"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailBestSoFar(HexCoords coordsFwd, HexCoords coordsRev,
                int bestSoFar)
        => Tracing.FindPathDetail.Trace("   SetBestSoFar: pathFwd at {0}; pathRev at {1}; Cost = {2}",
              coordsFwd,coordsRev, bestSoFar);
        /// <summary>If the conditional constant TRACE is defined: writes the enqueue details to the trace log.</summary>
        /// <param name="coords"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        protected static void TraceFindPathEnqueue(HexCoords coords,int priority, int preference)
        => Tracing.FindPathEnqueue.Trace(
              "   Enqueue {0}: estimate={1,4}:{2,4}",coords, priority, preference);
        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="coords"></param>
        /// <param name="cost"></param>
        /// <param name="exit"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        protected static void TraceFindPathDequeue(string searchDirection,
                HexCoords coords, int cost, Hexside exit, int priority, int preference)
        => Tracing.FindPathDequeue.Trace(
                "{0} Dequeue Path at {1} w/ cost={2,4} at {3,-9}; estimate={4,4}:{5,4}.", 
                searchDirection, coords, cost, exit, priority, preference);
        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="count"></param>
        [Conditional("TRACE")]
        protected static void TraceFindPathDone(int count)
        => Tracing.FindPathDequeue.Trace("Closed: {0,7}", count);
        #endregion
    }
}
