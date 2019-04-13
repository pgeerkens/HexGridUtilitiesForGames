#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Diagnostics;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>.</summary>
    public static class PathTracingExtensions {
        /// <summary>If the conditional constant TRACE is defined: writes the search start- and goal-coords to the trace log.</summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailInit(this IHex start, IHex goal)
        => Tracing.FindPathDetail.Trace(true, "Fwd: Find path from {0} to {1}:", start.Coords, goal.Coords);
        
        /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="vectorGoal"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailDirection(this string searchDirection,
                IntVector2D vectorGoal)
        => Tracing.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}", searchDirection, vectorGoal);
        
        /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="vectorGoal"></param>
        /// <param name="landmarkCoords"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailDirection(this string searchDirection,
                IntVector2D vectorGoal, HexCoords landmarkCoords)
        => Tracing.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}; and landmark at {2}", 
                                        searchDirection, vectorGoal, landmarkCoords);
        /// <summary>If the conditional constant TRACE is defined: writes the current direction pairing to the trace log.</summary>
        /// <param name="pathFwd"></param>
        /// <param name="pathRev"></param>
        /// <param name="bestSoFar"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailBestSoFar(this IDirectedPath pathFwd, IDirectedPath pathRev,
                int bestSoFar)
        => Tracing.FindPathDetail.Trace(
            $"   SetBestSoFar: pathFwd at {pathFwd.PathStep.Coords}; pathRev at {pathRev.PathStep.Coords}; Cost = {bestSoFar}");
        /// <summary>If the conditional constant TRACE is defined: writes the enqueue details to the trace log.</summary>
        /// <param name="coords"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathEnqueue(this HexCoords coords,int priority, int preference)
        => Tracing.FindPathEnqueue.Trace(
              "   Enqueue {0}: estimate={1,4}:{2,4}",coords, priority, preference);

        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="hex"></param>
        /// <param name="path"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDequeue(this IHex hex,
                string searchDirection, IDirectedPath path, int priority, int preference)
        => Tracing.FindPathDequeue.Trace(
                "{0} Dequeue Path at {1} w/ cost={2,4} at {3,-9}; estimate={4,4}:{5,4}.", 
                searchDirection, hex.Coords, path.TotalCost, path.HexsideExit, priority, preference);

        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="coords"></param>
        /// <param name="path"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDequeue(this HexCoords coords,
                string searchDirection, IDirectedPath path, int priority, int preference)
        => Tracing.FindPathDequeue.Trace(
                $"{searchDirection} Dequeue Path at {coords} w/ cost={path.TotalCost,4} at {path.HexsideExit,-9}; estimate={priority,4}:{preference,4}.");
        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="count"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDone(this int count)
        => Tracing.FindPathDequeue.Trace($"Closed: {count,7}");
    }
}
