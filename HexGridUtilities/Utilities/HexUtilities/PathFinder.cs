#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public static partial class PathFinder {
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
    /// <param name="destination"></param>
    /// <param name="stepCost"></param>
    /// <param name="estimate"></param>
    /// <returns></returns>
    public static IPath<ICoordsCanon> FindPath( 
      ICoordsCanon start, 
      ICoordsCanon goal,
      Func<ICoordsCanon,Hexside,int> stepCost,
      Func<ICoordsCanon,int>         estimate,
      Func<ICoordsCanon,bool>        isOnBoard
    ) {
      var closed = new HashSet<ICoordsCanon>();
      var queue  = new PriorityQueue<uint, Path<ICoordsCanon>>();
      var vectorGoal = new IntVector2D(goal.Vector - start.Vector);
      DebugTracing.Trace(TraceFlag.FindPath, true, "Find path from {0} to {1}; vectorGoal = {0}", 
                                    start.User, goal.User.ToString(), vectorGoal);

      queue.Enqueue (0, new Path<ICoordsCanon>(start));

      while (! queue.IsEmpty) {
        var path = queue.Dequeue();
        if( closed.Contains(path.LastStep) ) continue;

        DebugTracing.Trace(TraceFlag.FindPath, "Dequeue Path at {1} w/ cost={0}.", 
          path.TotalCost, path.LastStep.User);
        if(path.LastStep!=null  &&  path.LastStep.Equals(goal)) return path;

        closed.Add(path.LastStep);

        foreach (var neighbour in path.LastStep.GetNeighbours(~Hexside.None)) {
          if (isOnBoard(neighbour.Coords.Canon)) {
            var preference    = (ushort)vectorGoal.VectorCross(goal.Vector - neighbour.Coords.Canon.Vector);
            var cost          = stepCost(path.LastStep, neighbour.Direction);
            if (cost > 0) {
              var newPath     = path.AddStep(neighbour.Coords.Canon, (ushort)cost, neighbour.Direction); 
              var newEstimate = ((uint)estimate(neighbour.Coords.Canon) + (uint)newPath.TotalCost) << 16;
              queue.Enqueue(newEstimate + preference, newPath);
              DebugTracing.Trace(TraceFlag.FindPath, "   Enqueue {0}: {1} / {2}; / {3}",
                  neighbour.Coords, newEstimate>>16, newEstimate & 0x0FFFF, preference);
            }
          }
        }
      }
      return null;
    }

    public static int VectorCross (this IntVector2D lhs, IntVector2D rhs) {
      return Math.Abs(lhs.X*rhs.Y - lhs.Y*rhs.X);
    }
  }
}
