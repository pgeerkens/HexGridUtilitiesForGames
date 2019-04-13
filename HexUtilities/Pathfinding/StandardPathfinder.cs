#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;

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
    /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/10/path-finding-using-a-in-c-3-0-part-four.aspx">
    /// Eric Lippert's A* path-finding agorithm
    /// </a>
    /// </para><para>
    /// Adapted to hex-grids, and to weight the most direct path favourably for better (visual) 
    /// behaviour on a hexgrid.
    /// </para><para>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">Notes - A-star Algorithm</a>
    /// <seealso cref="BidirectionalAltPathfinder"/>
    /// </para>
    /// </remarks>
    public static class StandardPathfinderExtensions {
        /// <summary>Returns an <see cref="IPath{THex}"/> for the optimal path using a standard A* algorithm.</summary>
        /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        /// <returns>A <see cref="IPath{THex}"/> for the shortest path found, or null if no path was found.</returns>
        /// <remarks>
        /// <para>Note that the Heuristic provided by <paramref name="board"/> <b>must</b> be monotonic in order for the algorithm to perform properly.</para>
        /// </remarks>
        public static IPath<THex> GetPathStandardAStar<THex>(this INavigableBoard<THex> board,
                THex source, THex target)
        where THex: IHex
        => board.GetPath(source,target,new DictionaryPriorityQueue<int, IDirectedPath>(),new HashSet<HexCoords>());

        /// <summary>Returns an <see cref="IPath{THex}"/> for the optimal path using a standard A* algorithm.</summary>
        /// <param name="board">An object satisfying the interface <see cref="INavigableBoard{THex}"/>.</param>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        /// <param name="queue"></param>
        /// <param name="closedSet"></param>
        /// <returns>A <see cref="IPath{THex}"/> for the shortest path found, or null if no path was found.</returns>
        /// <remarks>
        /// <para>Note that the Heuristic provided by <paramref name="board"/> <b>must</b> be monotonic in order for the algorithm to perform properly.</para>
        /// </remarks>
        public static IPath<THex> GetPath<THex>(this INavigableBoard<THex> board, THex source, THex target,
                IPriorityQueue<int,IDirectedPath> queue, ISet<HexCoords> closedSet)
        where THex: IHex {
            source.TraceFindPathDetailInit(target);

            var vectorGoal = target.Coords.Canon - source.Coords.Canon;
            queue.Enqueue (0, new DirectedPath(target));

            while (queue.TryDequeue(out var item)) {
                var path = item.Value;
                var step = path.PathStep;

                if (closedSet.Contains(step.Coords)) continue;

                step.Hex.TraceFindPathDequeue("A* Rev", path, item.Key>>16, (int)(item.Key & 0xFFFFu) - 0x7FFF);

                if(step.Hex == (IHex)source)
                    return new Path<THex>(path.ToMaybe(),source,target,closedSet,null);

                closedSet.Add(step.Coords);

                Hexside.HexsideList.ForEach(hexside => {
                    board[step.Coords.GetNeighbour(hexside)].IfHasValueDo(neighbour => {
                        var cost = step.Hex.TryDirectedCost(hexside);
                        if (cost > 0 ) {
                            var newPath = path.AddStep(neighbour, hexside.Reversed, cost);
                            board.Estimate(vectorGoal, source, neighbour, newPath.TotalCost)
                                 .IfHasValueDo(key => {
                                     neighbour.Coords.TraceFindPathEnqueue(key>>16, (int)(key & 0xFFFFu));

                                     queue.Enqueue(key, newPath);
                                 } );
                        }
                    });
                } );
            }

            return null;
        }

        static int? Estimate<THex>(this INavigableBoard<THex> board, IntVector2D vectorGoal,
                IHex start, IHex hex, int totalCost)
        where THex: IHex
        => from heuristic in board.Heuristic(start,hex) as int? select ((heuristic + totalCost) << 16)
        + Preference(vectorGoal, start.Coords.Canon - hex.Coords.Canon);

        static int TryDirectedCost(this IHex hex, Hexside hexside) => hex.EntryCost(hexside);

        static int Preference(IntVector2D vectorGoal, IntVector2D vectorHex)
        => (0xFFFF & Math.Abs(vectorGoal ^ vectorHex));
    }
}
