#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
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
using System;
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using DirectedPath   = DirectedPathCollection;
    using IDirectedPath  = IDirectedPathCollection;

    /// <summary>(Adapted) C# implementation of A* path-finding algorithm by Eric Lippert.</summary>
    /// <remarks>
    /// <para>
    /// "A nice property of the A* algorithm is that it finds the optimal path in a reasonable 
    /// amount of time provided that:
    ///   • the estimating function never overestimates the distance to the goal. 
    ///     (Think about what happens if the estimating function sometimes overestimates the distance;
    ///     if the optimal path is one of the ones overestimated then it will possibly not make it to 
    ///     the front of the priority queue before a nonoptimal solution is found.)
    ///   • calling the estimating function does not take very long.
    /// </para><para>
    /// "One way to ensure that the estimating function never overestimates the distance is to always 
    /// estimate zero. If you do so then this becomes Dijkstra's algorithm. However, the better your 
    /// estimating function can get without going over (this really should have been called the 
    /// "The Price Is Right" algorithm...) the faster this will identify the truly optimal path.
    /// </para><para>
    /// "Unfortunately, the space complexity of this algorithm can be really quite high on complicated 
    /// graphs. There are more complex versions of this algorithm which can deal with the space complexity."
    /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/10/path-finding-using-a-in-c-3-0-part-four.aspx"></a>
    /// </para><para>
    /// Adapted to hex-grids, and to weight the most direct path favourably for better (visual) 
    /// behaviour on a hexgrid.
    /// </para><para>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">Notes - A-star Algorithm</a>
    /// <seealso cref="PGNapoleonics.HexUtilities.Pathfinding.BidirectionalAltPathfinder"/>
    /// </para>
    /// </remarks>
    /// <see cref="BidirectionalAltPathfinder"/>
    public sealed class StandardPathfinder : IPathfinder {
        /// <summary>Creates a new <see cref="IPathfinder"/> instance implementing a unidirectional A*.</summary>
        /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
        /// <remarks>
        /// <para>Note that the Heuristic provided by <paramref name="board"/> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly.</para>
        /// </remarks>
        public StandardPathfinder(INavigableBoard board) => Board = board;

        private INavigableBoard Board { get; }

        /// <inheritdoc/>
        public Maybe<IDirectedPath> GetPath(HexCoords source, HexCoords target) {
            PathfinderExtensions.TraceFindPathDetailInit(source, target);

            // Minimize field references by putting these on the stack now
            var vectorGoal   = target.Canon - source.Canon;
            var openSet      = new HashSet<HexCoords>();
            var closedSet    = new HashSet<HexCoords>();
            var queue        = new DictionaryPriorityQueue<int, IDirectedPath>();;

            queue.Enqueue (0, new DirectedPath(target));

            while (queue.TryDequeue(out var item)) {
                var path = item.Value;
                var step = path.PathStep.Coords;

                openSet.Add(step);
                if (closedSet.Contains(step)) continue;

                PathfinderExtensions.TraceFindPathDequeue("Rev",step, path, item.Key>>16, (int)(item.Key & 0xFFFFu) - 0x7FFF);

                if(step == source)     return path.ToMaybe();

                closedSet.Add(step);

                Hexside.HexsideList.ForEach(hexside => {
                    var neighbourCoords = step.GetNeighbour(hexside);
                    TryDirectedCost(step, hexside).IfHasValueDo( cost => {
                        var newPath = path.AddStep(neighbourCoords, hexside, cost);
                        Estimate(vectorGoal, source, neighbourCoords, newPath.TotalCost).IfHasValueDo(key => {
                            PathfinderExtensions.TraceFindPathEnqueue(neighbourCoords, key>>16, (int)(key & 0xFFFFu));

                            queue.Enqueue(key, newPath);
                        } );
                    } );
                } );
            }

            return null;
        }
    
        private int?   Estimate(IntVector2D vectorGoal, HexCoords start, HexCoords hex, int totalCost)
        => from heuristic in Heuristic(start,hex)
           select ((heuristic + totalCost) << 16)
                + Preference(vectorGoal, start.Canon - hex.Canon);

        private short? Heuristic(HexCoords source, HexCoords target) => Board.Heuristic(source,target);

        private short? TryDirectedCost(HexCoords coords, Hexside hexside) => Board.TryExitCost(coords, hexside);
    
        static int Preference(IntVector2D vectorGoal, IntVector2D vectorHex)
        => (0xFFFF & Math.Abs(vectorGoal ^ vectorHex));
    }
}
