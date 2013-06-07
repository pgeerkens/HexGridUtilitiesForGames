#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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

using PG_Napoleonics.HexUtilities.Common;
using PG_Napoleonics.HexUtilities.PathFinding;

namespace PG_Napoleonics.HexUtilities.PathFinding {
  /// <summary>Interface required to make use of A* Path Finding utility.</summary>
  public interface INavigableBoard {
    /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
    int RangeCutoff { get; }

    /// <summary>The cost of entering the hex at location <c>coords</c> heading <c>hexside</c>.</summary>
    int   StepCost(HexCoords coords, Hexside hexsideExit);

    /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
    /// <remarks>Returning the supplied range multiplied by the cheapest movement 
    /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    int   Heuristic(int range);

    /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
    bool  IsOnBoard(HexCoords coords);
  }

  /// <summary>(Adapted) C# implementation of A* path-finding algorithm by Eric Lippert.</summary>
  /// <remarks><quote>
  /// A nice property of the A* algorithm is that it finds the optimal path in a reasonable 
  /// amount of time provided that:
  ///   • the estimating function never overestimates the distance to the goal. 
  ///     (Think about what happens if the estimating function sometimes overestimates the distance;
  ///     if the optimal path is one of the ones overestimated then it will possibly not make it to 
  ///     the front of the priority queue before a nonoptimal solution is found.)
  ///   • calling the estimating function does not take very long.
  /// One way to ensure that the estimating function never overestimates the distance is to always 
  /// estimate zero. If you do so then this becomes Dijkstra's algorithm. However, the better your 
  /// estimating function can get without going over (this really should have been called the 
  /// "The Price Is Right" algorithm...) the faster this will identify the truly optimal path.
  ///
  /// Unfortunately, the space complexity of this algorithm can be really quite high on complicated 
  /// graphs. There are more complex versions of this algorithm which can deal with the space complexity.
  /// </quote>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/10/path-finding-using-a-in-c-3-0-part-four.aspx</cref>
  /// 
  /// Adapted to hex-grids, and to weight the most direct path favourably for better (visual) 
  /// behaviour on a hexgrid.
  /// </remarks>
  /// <param name="start"></param>
  /// <param name="goal"></param>
  /// <param name="board"></param>
  /// <returns></returns>
  public static partial class PathFinder {
    public static IPath FindPath(
      HexCoords     start,
      HexCoords     goal,
      INavigableBoard board
    ) {
      return FindPath(start, goal, board.RangeCutoff, board.StepCost, 
                      board.Heuristic, board.IsOnBoard);
    }

    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="stepCost">Cost to extend path by hex at <c>coords</c> from hex at direction <c>hexside</c>.</param>
    /// <param name="heuristic">Returns a cost estimate from a range value.</param>
    /// <param name="isOnBoard">Returns whether the coordinates specified are "on board".</param>
    public static IPath FindPath(
      HexCoords   start,
      HexCoords   goal,
      int         rangeCutoff,
      Func<HexCoords, Hexside, int> stepCost,
      Func<int,int>                 heuristic,
      Func<HexCoords,bool>          isOnBoard
    ) {
      var vectorGoal = goal.Canon - start.Canon;
      var closed     = new HashSet<HexCoords>();
      var queue      = goal.Range(start) > rangeCutoff
          ? (IPriorityQueue<uint, Path>) new ConcurrentPriorityQueue<uint, Path>()
          : (IPriorityQueue<uint, Path>) new DictPriorityQueue<uint, Path>();
      TraceFlag.FindPathDetail.Trace(true, "Find path from {0} to {1}; vectorGoal = {0}", 
                                    start.Canon, goal.Canon, vectorGoal);

      queue.Enqueue (0, new Path(start));

      MyKeyValuePair<uint,Path> item;
      while (queue.TryDequeue(out item)) {
        var path = item.Value;
        if( closed.Contains(path.LastStep) ) continue;

        #if DEBUG
          var previous = (path.PreviousSteps) == null ? HexCoords.EmptyUser : path.PreviousSteps.LastStep;
          TraceFlag.FindPathDetail.Trace("Dequeue Path at {0} from {3} w/ cost={1,4}:{2,3}.", 
            path.LastStep, path.TotalCost, item.Key & 0xFFFF, previous);
        #endif
        if(path.LastStep.Equals(goal)) return path;

        closed.Add(path.LastStep);

        foreach (var neighbour in path.LastStep.GetNeighbours()
                                      .Where(n => isOnBoard(n.Coords))
        ) {
            var cost = stepCost(path.LastStep, neighbour.Index);
            if (cost > 0) {
              var newPath  = path.AddStep(neighbour, (ushort)cost);
              var estimate = Estimate(heuristic,vectorGoal,goal, newPath.LastStep, newPath.TotalCost);
              TraceFlag.FindPathDetail.Trace("   Enqueue {0}: {1,4}:{2,3}",
                      neighbour.Coords, estimate>>16,  (int) ((int)(estimate & 0xFFFFu) - 0x7FFF));
              queue.Enqueue(estimate, newPath);
            }
        }
      }
      return null;
    }

    static uint Estimate(Func<int,int> heuristic, IntVector2D vectorGoal, HexCoords goal, 
            HexCoords hex, uint totalCost) {
      var estimate   = (uint)heuristic(goal.Range(hex)) + totalCost;
      var preference = Preference(vectorGoal, goal, hex);
      return (estimate << 16) + preference;
    }
    static uint Preference(IntVector2D vectorGoal, HexCoords goal, HexCoords hex) {
      return (uint)(0xFFFF & Math.Abs(vectorGoal ^ (goal.Canon - hex.Canon) ));
    }
    static uint DotPreference(IntVector2D vectorGoal, HexCoords goal, HexCoords hex) {
      return (uint) (0x7FFF + vectorGoal * (goal.Canon - hex.Canon));
    }
  }
}
