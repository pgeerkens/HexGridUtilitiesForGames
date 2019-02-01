#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>.</summary>
    public static class PathfinderExtensions {
        /// <inheritdoc/>>
        public static Maybe<IDirectedPath> GetPath (this IPathfinder @this, IHex target, IHex source)
            => @this.GetPath(target.Coords, source.Coords);

        /// <summary>Calculates an <see cref="IDirectedPath"/> asynchronously for the optimal path from coordinates .</summary>
        /// <param name="this"></param>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        public static async Task<Maybe<IDirectedPath>> GetPathAsync(this IPathfinder @this, IHex source, IHex target)
        => await Task.Run( () => @this.GetPath(source, target) );

        /// <summary>Calculates an <see cref="IDirectedPath"/> asynchronously for the optimal path from coordinates .</summary>
        /// <param name="this"></param>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        public static async Task<Maybe<IDirectedPath>> GetPathAsync(this IPathfinder @this, HexCoords source, HexCoords target)
        => await Task.Run( () => @this.GetPath(source, target) );

        /// <summary>Returns the result of stacking <paramref name="mergePath"/> onto <paramref name="this"/></summary>
        public static Maybe<IDirectedPath> MergePaths(this IDirectedPath @this, IDirectedPath mergePath) {
            if (@this == null  ||  mergePath == null) return null;
            while (mergePath.PathSoFar != null) {
                var hexside = mergePath.PathStep.HexsideExit;
                var cost    = mergePath.TotalCost - (mergePath = mergePath.PathSoFar).TotalCost;
                @this  = @this.AddStep(mergePath.PathStep.Coords, hexside, cost);
          }
            return @this.ToMaybe();
        }

        #region Conditional tracing routines
        /// <summary>If the conditional constant TRACE is defined: writes the search start- and goal-coords to the trace log.</summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailInit(HexCoords start, HexCoords goal)
        => Tracing.FindPathDetail.Trace(true, "Fwd: Find path from {0} to {1}:", start, goal);
        
        /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="vectorGoal"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailDirection(string searchDirection,
                IntVector2D vectorGoal)
        => Tracing.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}", searchDirection, vectorGoal);
        
        /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="vectorGoal"></param>
        /// <param name="landmarkCoords"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailDirection(string searchDirection,
                IntVector2D vectorGoal, HexCoords landmarkCoords)
        => Tracing.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}; and landmark at {2}", 
                                        searchDirection, vectorGoal, landmarkCoords);
        /// <summary>If the conditional constant TRACE is defined: writes the current direction pairing to the trace log.</summary>
        /// <param name="pathFwd"></param>
        /// <param name="pathRev"></param>
        /// <param name="bestSoFar"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDetailBestSoFar(IDirectedPath pathFwd, IDirectedPath pathRev,
                int bestSoFar)
        => Tracing.FindPathDetail.Trace(
            $"   SetBestSoFar: pathFwd at {pathFwd.PathStep.Coords}; pathRev at {pathRev.PathStep.Coords}; Cost = {bestSoFar}");
        /// <summary>If the conditional constant TRACE is defined: writes the enqueue details to the trace log.</summary>
        /// <param name="coords"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathEnqueue(HexCoords coords,int priority, int preference)
        => Tracing.FindPathEnqueue.Trace(
              "   Enqueue {0}: estimate={1,4}:{2,4}",coords, priority, preference);
        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="searchDirection"></param>
        /// <param name="coords"></param>
        /// <param name="path"></param>
        /// <param name="priority"></param>
        /// <param name="preference"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDequeue(string searchDirection,
                HexCoords coords, IDirectedPath path, int priority, int preference)
        => Tracing.FindPathDequeue.Trace(
                "{0} Dequeue Path at {1} w/ cost={2,4} at {3,-9}; estimate={4,4}:{5,4}.", 
                searchDirection, coords, path.TotalCost, path.HexsideExit, priority, preference);
        /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
        /// <param name="count"></param>
        [Conditional("TRACE")]
        public static void TraceFindPathDone(int count)
        => Tracing.FindPathDequeue.Trace($"Closed: {count,7}");
        #endregion
    }
}
