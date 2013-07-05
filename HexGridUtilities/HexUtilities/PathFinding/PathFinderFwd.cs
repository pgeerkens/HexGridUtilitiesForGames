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
#define FastList
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.PathFinding;

namespace PGNapoleonics.HexUtilities.PathFinding {
  /// <summary>Interface required to make use of A* Path Finding utility.</summary>
  public interface IDirectedNavigableBoard : INavigableBoard {

    /// <summary>Cost to extend path by exiting the hex at <c>coords</c> through <c>hexside</c>.</summary>
    int  DirectedStepCost(IHex hex, Hexside hexsideExit);
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
  public partial class Pathfinder {
    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// ///<remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static IDirectedPath FindDirectedPath(
      IHex start,
      IHex goal,
      IDirectedNavigableBoard board
    ) {
      if (board==null) throw new ArgumentNullException("board"); 
      return FindDirectedPath(start, goal, board.DirectedStepCost,
                                     board.Heuristic, board.IsOnboard);
    }

    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="directedStepCost">Cost to extend path by hex at <c>coords</c> from hex at direction <c>hexside</c>.</param>
    /// <param name="heuristic">Returns a monotonic (ie locally admissible) cost estimate from a range value.</param>
    /// <param name="isOnboard">Returns whether the coordinates specified are "on board".</param>
    /// ///<remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    public static IDirectedPath FindDirectedPath (
      IHex start,
      IHex goal,
      Func<IHex, Hexside, int> directedStepCost,
      Func<int,int>            heuristic,
      Func<HexCoords,bool>     isOnboard
    ) {
      if (start==null)      throw new ArgumentNullException("start"); 
      if (goal==null)       throw new ArgumentNullException("goal"); 
      if (directedStepCost==null)   throw new ArgumentNullException("directedStepCost"); 
      if (heuristic==null)  throw new ArgumentNullException("heuristic"); 
      if (isOnboard==null)  throw new ArgumentNullException("isOnboard"); 

      var queue  = new DictionaryPriorityQueue<int, DirectedPath>();

      var functor = new PathQueueFunctor(start,goal,heuristic,directedStepCost,queue);

      queue.Enqueue (0, new DirectedPath(goal));

      HexKeyValuePair<int,DirectedPath> item;
      while (queue.TryDequeue(out item)) {
        if (functor.PathFound(item)) return functor.Path;
      }
      return null; 
    }

    /// <summary><c>Static List&lt;Hexside></c> for enumerations.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", 
      "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly FastList<Hexside> FastHexsideList 
      = new FastList<Hexside>(Utilities.EnumGetValues<Hexside>().ToArray());

    private class PathQueueFunctor {
      public DirectedPath Path { get; set; }

      public PathQueueFunctor(IHex start, IHex goal, Func<int,int> heuristic, 
        Func<IHex, Hexside, int> stepCost, 
        IPriorityQueue<int, DirectedPath> queue
      ) {
        _vectorGoal = goal.Coords.Canon - start.Coords.Canon;

        TraceFlags.FindPathDetail.Trace(true, "Find path from {0} to {1}; vectorGoal = {2}", 
                                              start.Coords, goal.Coords, _vectorGoal);
        Path        = null;

        _start      = start;
//        _goal       = goal;
        _heuristic  = heuristic;
        _open       = (IDictionary<HexCoords,IDirectedPath>) new Dictionary<HexCoords, IDirectedPath>();
        _stepCost   = stepCost;
        _queue      = queue;
      }

      public bool PathFound(HexKeyValuePair<int,DirectedPath> item) {
        var path = item.Value;
        var hex  = path.PathStep.Hex;
        if( _closed.Contains(hex.Coords) ) return false;

        _open.Add(item.Value.PathStep.Hex.Coords, item.Value);

        TraceFlags.FindPathDequeue.Trace(
          "Dequeue Path at {0} w/ cost={1,4} at {2}; estimate={3,4}:{4,4}.", 
          item.Value.PathStep.Hex.Coords, item.Value.TotalCost, item.Value.HexsideExit, 
          item.Key >> 16, item.Key & 0xFFFF);

        if(hex.Coords.Equals(_start.Coords)) { 
          Path = path;
          return true; 
        }

        _closed.Add(hex.Coords);

        FastHexsideList.ForEach((Action<Hexside>)(hexside => Invoke(path, hexside)));
        return false;
      }

      public void Invoke(DirectedPath path, Hexside hexside) {
        var here  = path.PathStep.Hex;
        var there = here.Board[here.Coords.GetNeighbour(hexside)];
        if (there != null  &&  ! _open.ContainsKey(there.Coords)) {
          var cost = _stepCost(there, hexside.Reversed());
          if (cost > 0) {
            var newPath  = path.AddStep(there, hexside, cost);
            var estimate = Estimate(_heuristic, _vectorGoal, _start.Coords, newPath.PathStep.Hex.Coords, newPath.TotalCost);
            TraceFlags.FindPathEnqueue.Trace("   Enqueue {0}: estimate={1,4}:{2,3}",
                there.Coords, estimate>>16, estimate & 0xFFFF);
            _queue.Enqueue(estimate, newPath);
          }
        }
      }

      IHex                            _start;
//      IHex                            _goal;
      Func<int,int>                   _heuristic;
      Func<IHex, Hexside, int>        _stepCost;

      IDictionary<HexCoords,IDirectedPath> _open;
      ISet<HexCoords>                 _closed = new HashSet<HexCoords>();
      IntVector2D                     _vectorGoal;
      IPriorityQueue<int, DirectedPath>    _queue;
    }
  }
}
