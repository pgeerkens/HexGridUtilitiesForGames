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

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Interface required to make use of A* Path Finding utility.</summary>
  public interface IDirectedNavigableBoard {
    /// <summary>The cost of entering the hex at location <c>coords</c> heading <c>hexside</c>.</summary>
    int   StepCost(HexCoords coords, Hexside hexsideExit);

    /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
    /// <remarks>Returning the supplied range multiplied by the cheapest movement 
    /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
    /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
    int   Heuristic(int range);

    /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
    bool  IsOnboard(HexCoords coords);

    /// <summary>Cost to extend path by exiting the hex at <c>coords</c> through <c>hexside</c>.</summary>
    int  DirectedStepCost(IHex hex, Hexside hexsideExit);

    /// <summary>Returns the collecction of defined landmarks on this board.</summary>
    LandmarkCollection Landmarks { get; }
  }

#if FALSE
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
  public static partial class Pathfinder {
    /// <summary>TODO</summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    public static IPath FindPath(
      HexCoords     start,
      HexCoords     goal,
      INavigableBoard board
    ) {
      if (board==null) throw new ArgumentNullException("board");
      return FindPath(start, goal, board.StepCost, 
                                   board.Heuristic, board.IsOnboard);
    }

    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="stepCost">Cost to extend path by hex at <c>coords</c> from hex at direction <c>hexside</c>.</param>
    /// <param name="heuristic">Returns a cost estimate from a range value.</param>
    /// <param name="isOnboard">Returns whether the coordinates specified are "on board".</param>
    public static IPath FindPath(
      HexCoords   start,
      HexCoords   goal,
      Func<HexCoords, Hexside, int> stepCost,
      Func<int,int>                 heuristic,
      Func<HexCoords,bool>          isOnboard
    ) {
      if (stepCost==null) throw new ArgumentNullException("stepCost"); 
      if (heuristic==null) throw new ArgumentNullException("heuristic"); 
      if (isOnboard==null) throw new ArgumentNullException("isOnboard"); 

      var vectorGoal = goal.Canon - start.Canon;
      var closed     = new HashSet<HexCoords>();
      var open       = new HashSet<HexCoords>();
      var queue      = DictionaryPriorityQueue<int, Path>.NewQueue();
      TraceFlags.FindPathDetail.Trace(true, "Find path from {0} to {1}; vectorGoal = {0}", 
                                          start.Canon, goal.Canon, vectorGoal);

      queue.Enqueue (0, new Path(start));

      HexKeyValuePair<int,Path> item;
      while (queue.TryDequeue(out item)) {
        var path = item.Value;
        if( closed.Contains(path.StepCoords) ) continue;

        open.Add(item.Value.StepCoords);

        TraceFlags.FindPathDetail.Trace(
          "Dequeue Path at {0} w/ cost={1,4} at {2}; estimate={3,4}:{4,4}.", 
          item.Value.StepCoords, item.Value.TotalCost, item.Value.HexsideExit, 
          item.Key >> 16, item.Key & 0xFFFF);

        if(path.StepCoords.Equals(goal)) return path;

        closed.Add(path.StepCoords);

        foreach (var neighbour in path.StepCoords.GetNeighbours()
                                      .Where(n => isOnboard(n.Coords))
        ) {
            var cost = stepCost(path.StepCoords, neighbour.Hexside);
            if (cost > 0) {
              var newPath  = path.AddStep(neighbour, (ushort)cost);
              var estimate = Estimate(heuristic,vectorGoal,goal, newPath.StepCoords, newPath.TotalCost);
              TraceFlags.FindPathDetail.Trace("   Enqueue {0}: {1,4}:{2,3}",
                      neighbour.Coords, estimate>>16, estimate & 0xFFFF);
              queue.Enqueue(estimate, newPath);
            }
        }
      }
      return null;
    }

    internal static int Estimate(Func<int,int> heuristic, IntVector2D vectorGoal, HexCoords goal, 
            HexCoords hex, int totalCost) {
      var estimate   = (int)heuristic(goal.Range(hex)) + totalCost;
      var preference = Preference(vectorGoal, goal, hex);
      return (estimate << 16) + preference;
    }
    static int Preference(IntVector2D vectorGoal, HexCoords goal, HexCoords hex) {
      return (0xFFFF & Math.Abs(vectorGoal ^ (goal.Canon - hex.Canon) ));
    }
    //static int DotPreference(IntVector2D vectorGoal, HexCoords goal, HexCoords hex) {
    //  return (int) (0x7FFF + vectorGoal * (goal.Canon - hex.Canon));
    //}
  }
#endif
}
