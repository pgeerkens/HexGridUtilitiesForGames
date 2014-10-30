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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>(Adapted) C# implementation of A* path-finding algorithm by Eric Lippert.</summary>
  /// <remarks>
  /// "A nice property of the A* algorithm is that it finds the optimal path in a reasonable 
  /// amount of time provided that:
  ///   • the estimating function never overestimates the distance to the goal. 
  ///     (Think about what happens if the estimating function sometimes overestimates the distance;
  ///     if the optimal path is one of the ones overestimated then it will possibly not make it to 
  ///     the front of the priority queue before a nonoptimal solution is found.)
  ///   • calling the estimating function does not take very long.
  ///   
  /// "One way to ensure that the estimating function never overestimates the distance is to always 
  /// estimate zero. If you do so then this becomes Dijkstra's algorithm. However, the better your 
  /// estimating function can get without going over (this really should have been called the 
  /// "The Price Is Right" algorithm...) the faster this will identify the truly optimal path.
  ///
  /// "Unfortunately, the space complexity of this algorithm can be really quite high on complicated 
  /// graphs. There are more complex versions of this algorithm which can deal with the space complexity."
  /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/10/path-finding-using-a-in-c-3-0-part-four.aspx"></a>
  /// 
  /// Adapted to hex-grids, and to weight the most direct path favourably for better (visual) 
  /// behaviour on a hexgrid.
  /// </remarks>
  public static class UnidirectionalPathfinder {
    #pragma warning disable 1658, 1584
    /// <summary>Returns an <c>IDirectedPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <returns></returns>
    /// <remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    #pragma warning restore 1633, 1658, 1584
    public static IDirectedPathCollection FindDirectedPathFwd(
      IHex start,
      IHex goal,
      IDirectedNavigableBoard board
    ) {
      if (board==null) throw new ArgumentNullException("board"); 
      return FindDirectedPathFwd(start, goal, board.DirectedStepCost, board.Heuristic);
    }

    #pragma warning disable 1658, 1584
    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="stepCost">Cost to extend path by hex at <c>coords</c> from hex at direction <c>hexside</c>.</param>
    /// <param name="heuristic">Returns a monotonic (ie locally admissible) cost estimate from a range value.</param>
    /// <returns></returns>
    /// <remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    #pragma warning restore 1658, 1584
    public static IDirectedPathCollection FindDirectedPathFwd (
      IHex start,
      IHex goal,
      Func<IHex, Hexside, int> stepCost,
      Func<int,int>            heuristic
    ) {
      if (start==null) throw new ArgumentNullException("start");
      if (goal==null) throw new ArgumentNullException("goal");
      if (stepCost==null) throw new ArgumentNullException("stepCost");

      var vectorGoal = goal.Coords.Canon - start.Coords.Canon;
      var closed     = new HashSet<HexCoords>();
      var open       = new HashSet<HexCoords>();
      var queue      = new DictionaryPriorityQueue<int, DirectedPathCollection>();
      Traces.FindPathDetail.Trace(true, "Find path from {0} to {1}; vectorGoal = {2}", 
                                      start.Coords, goal.Coords, vectorGoal);

      queue.Enqueue (0, new DirectedPathCollection(goal));

      HexKeyValuePair<int,DirectedPathCollection> item;
      while (queue.TryDequeue(out item)) {
        var path = item.Value;
        open.Add(path.PathStep.Hex.Coords);
        if( closed.Contains(path.PathStep.Hex.Coords) ) continue;

        Traces.FindPathDequeue.Trace(
          "Dequeue Path at {0} w/ cost={1,4} at {2}; estimate={3,4}:{4,4}.", 
          path.PathStep.Hex.Coords, path.TotalCost, path.HexsideExit, item.Key>>16, 
                (int) ((int)(item.Key & 0xFFFFu) - 0x7FFF));

        if(path.PathStep.Hex.Equals(start)) {
          Traces.FindPathDequeue.Trace("Closed: {0,7}", closed.Count);
          return path;
        }

        closed.Add(path.PathStep.Hex.Coords);

        foreach (var neighbour in path.PathStep.Hex.GetNeighbourHexes()
        ) {
          if ( ! open.Contains(neighbour.Hex.Coords)) {
            var cost = stepCost(neighbour.Hex, neighbour.HexsideExit);
            if (cost > 0) {
              var newPath = path.AddStep(neighbour, cost);
              var key     = Estimate(heuristic, vectorGoal, start.Coords, neighbour.Hex.Coords, 
                              newPath.TotalCost);
              Traces.FindPathEnqueue.Trace("   Enqueue {0}: estimate={1,4}:{2,3}",
                        neighbour.Hex.Coords, key>>16, key & 0xFFFFu);
              queue.Enqueue(key, newPath);
            }
          }
        }
      }
      return null;
    }

    static int Estimate(Func<int,int> heuristic, IntVector2D vectorGoal, HexCoords start, 
            HexCoords hex, int totalCost) {
      var estimate   = heuristic(start.Range(hex)) + totalCost;
      var preference = Preference(vectorGoal, start.Canon - hex.Canon);
      return (estimate << 16) + preference;
    }

    static int Preference(IntVector2D vectorGoal, IntVector2D vectorHex) {
      return (0xFFFF & Math.Abs(vectorGoal ^ vectorHex ));
    }
  }
}
