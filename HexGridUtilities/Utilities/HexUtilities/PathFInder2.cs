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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface INavigableBoard {
    int   StepCost(ICoordsCanon coords, Hexside hexside);
    int   Heuristic(int range);
    bool  IsOnBoard(ICoordsUser coords);
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
  public static class PathFinder2 {
    public static int RangeCutoff { get; set; }
    static PathFinder2() { RangeCutoff = 80; }  // TODO: Set this to FOVRange perhaps?

    public static IPath2 FindPath(
      ICoordsUser     start,
      ICoordsUser     goal,
      INavigableBoard board
    ) {
      return FindPath(start, goal, board.StepCost, board.Heuristic, board.IsOnBoard);
    }

    public static IPath2 FindPath(
      ICoordsUser     start,
      ICoordsUser     goal,
      Func<ICoordsCanon, Hexside, int> stepCost,
      Func<int,int>                    heuristic,
      Func<ICoordsUser,bool>           isOnBoard
    ) {
      var vectorGoal = goal.Canon.Vector - start.Canon.Vector;
      var closed     = new HashSet<ICoordsUser>();
      var queue      = goal.Range(start) > RangeCutoff
          ? (IPriorityQueue<uint, Path2>) new HeapPriorityQueue<uint, Path2>()
          : (IPriorityQueue<uint, Path2>) new DictPriorityQueue<uint, Path2>();
      #if DEBUG
        TraceFlag.FindPath.Trace(true, "Find path from {0} to {1}; vectorGoal = {0}", 
                                      start.Canon, goal.Canon, vectorGoal);
      #endif

      queue.Enqueue (0, new Path2(start));

      while (! queue.IsEmpty) {
        var oldPref = queue.Peek().Key & 0xFFFF; 
        var path = queue.Dequeue();
        if( closed.Contains(path.LastStep) ) continue;

        #if DEBUG
          var previous = (path.PreviousSteps) == null ? HexCoords.EmptyUser : path.PreviousSteps.LastStep;
          TraceFlag.FindPath.Trace("Dequeue Path at {0} from {3} w/ cost={1,4}:{2,3}.", 
            path.LastStep, path.TotalCost, oldPref, previous);
        #endif
        if(path.LastStep!=null  &&  path.LastStep.Equals(goal)) return path;

        closed.Add(path.LastStep);

        foreach (var neighbour in path.LastStep.GetNeighbours(~Hexside.None)) {
          if (isOnBoard(neighbour.Coords.User)) {
            var cost = stepCost(path.LastStep.Canon, neighbour.Direction);
            if (cost > 0) {
              var preference = (ushort)Math.Abs(vectorGoal ^ (goal.Canon.Vector - neighbour.Coords.Vector));
              var newPath    = path.AddStep(neighbour.Coords.User, (ushort)cost, neighbour.Direction); 
              var estimate   = ( (uint)heuristic(goal.Range(neighbour.Coords.User))
                             +   (uint)newPath.TotalCost ) << 16;
              queue.Enqueue(estimate + preference, newPath);
              #if DEBUG
                TraceFlag.FindPath.Trace("   Enqueue {0}: {1,4}:{2,3}",
                        neighbour.Coords, estimate>>16, preference);
              #endif
            }
          }
        }
      }
      return null;
    }
  }
}
