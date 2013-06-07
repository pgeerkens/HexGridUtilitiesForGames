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

  public interface INavigableBoardFwd : INavigableBoard {
    /// <summary>Cost to extend path by the hex at <c>coords</c> from hex at direction <c>hexside</c>.</summary>
    int  StepCostFwd(HexCoords coords, Hexside hexsideExit);
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
    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// ///<remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    public static IPathFwd FindPathFwd(
      IHex start,
      IHex goal,
      INavigableBoardFwd board,
      bool useShortcuts = false
    ) {
      return FindPathFwd(start, goal, board.RangeCutoff, board.StepCostFwd,
                                      board.Heuristic, board.IsOnBoard, useShortcuts);
    }

    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="stepCost">Cost to extend path by hex at <c>coords</c> from hex at direction <c>hexside</c>.</param>
    /// <param name="heuristic">Returns a monotonic (ie locally admissible) cost estimate from a range value.</param>
    /// <param name="isOnBoard">Returns whether the coordinates specified are "on board".</param>
    /// ///<remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    public static IPathFwd FindPathFwd (
      IHex start,
      IHex goal,
      int  rangeCutoff,
      Func<HexCoords, Hexside, int> stepCost,
      Func<int,int>                 heuristic,
      Func<HexCoords,bool>          isOnBoard,
      bool useShortcuts = false
    ) {
      var vectorGoal = goal.Coords.Canon - start.Coords.Canon;
      var closed     = new HashSet<HexCoords>();
      var open       = new HashSet<HexCoords>();
      var queue      = goal.Coords.Range(start.Coords) > rangeCutoff
          ? (IPriorityQueue<uint, IPathFwd>) new ConcurrentPriorityQueue<uint, IPathFwd>()
          : (IPriorityQueue<uint, IPathFwd>) new DictPriorityQueue<uint, IPathFwd>();
      TraceFlag.FindPathDetail.Trace(true, "Find path from {0} to {1}; vectorGoal = {2}", 
                                      start.Coords, goal.Coords, vectorGoal);

      queue.Enqueue (0, new PathFwd(goal));

      MyKeyValuePair<uint,IPathFwd> item;
      while (queue.TryDequeue(out item)) {
        open.Add(item.Value.Step.Hex.Coords);
        var path = item.Value;
        if( closed.Contains(path.Step.Hex.Coords) ) continue;

        #if DEBUG
          TraceFlag.FindPathDequeue.Trace(
            "Dequeue Path at {0} w/ cost={1,4} at {2}; estimate={3,4}:{4,4}.", 
            item.Value.Step.Hex.Coords, item.Value.TotalCost, item.Value.HexsideExit, item.Key>>16, 
                  (int) ((int)(item.Key & 0xFFFFu) - 0x7FFF));
        #endif

        if(path.Step.Hex.Equals(start)) {
          TraceFlag.FindPathDequeue.Trace("Closed: {0,7}", closed.Count);
          return path;
        }

        closed.Add(path.Step.Hex.Coords);

        foreach (var neighbour in path.Step.Hex.GetNeighbourHexes()
        ) {
          if ( ! open.Contains(neighbour.Hex.Coords)) {
            var cost = stepCost(neighbour.Hex.Coords, neighbour.HexsideExit);
            if (cost > 0) {
              var newPath  = path.AddStep(neighbour, (uint)cost);
              var estimate = Estimate(heuristic, vectorGoal, start.Coords, newPath.Step.Hex.Coords, newPath.TotalCost);
              TraceFlag.FindPathEnqueue.Trace("   Enqueue {0}: estimate={1,4}:{2,3}",
                  neighbour.Hex.Coords, (estimate)>>16, (int) ((int)(estimate & 0xFFFFu) - 0x7FFF));
              queue.Enqueue(estimate, newPath);
            }
          }
        }

        if (useShortcuts)
          ProcessShortcuts(start, goal, vectorGoal, queue, path, 
                rangeCutoff, stepCost, heuristic, isOnBoard);
      }
      return null;
    }

    static void ProcessShortcuts(
      IHex start,
      IHex goal,
      IntVector2D                 vectorGoal,
      IPriorityQueue<uint, IPathFwd> queue,
      IPathFwd path,
      int  rangeCutoff,
      Func<HexCoords, Hexside, int> stepCost,
      Func<int,int>               heuristic,
      Func<HexCoords,bool>          isOnBoard
    ) {
      foreach (var shortcut in path.Step.Hex.Shortcuts) {
        var pathFwd = shortcut.PathFwd;
        var newPath = path;
        var hexside  = pathFwd.Step.HexsideEntry;
        if (path.Step.Equals(pathFwd.Step)) pathFwd = pathFwd.NextSteps;
        while (pathFwd.NextSteps != null) {
          var step     = pathFwd.Step;
          var cost     = pathFwd.TotalCost - pathFwd.NextSteps.TotalCost;
          newPath      = newPath.AddStep(new NeighbourHex(step.Hex, hexside.Reversed()), cost);
          var estimate = Estimate(heuristic, vectorGoal, goal.Coords, newPath.Step.Hex.Coords, newPath.TotalCost);
          queue.Enqueue(estimate, newPath);
          TraceFlag.FindPathDetail.Trace("   Enqueue {0}: {1,4}:{2,3}",
              step.Hex.Coords, (estimate & 0xFFFF0000)>>16, estimate & 0xFFFF);
          hexside      = step.HexsideEntry;
          pathFwd      = pathFwd.NextSteps;
        }
      }
    }
  }
}
