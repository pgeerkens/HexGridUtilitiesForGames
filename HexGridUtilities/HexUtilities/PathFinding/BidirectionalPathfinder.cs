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
#undef UseParallel
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.PathFinding;

namespace PGNapoleonics.HexUtilities.PathFinding {
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
  public static class BidirectionalPathfinder {
    public static bool UseShortcuts { get; set; }

    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// ///<remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static IDirectedPath FindDirectedPathFwd(
      IHex start,
      IHex goal,
      IDirectedNavigableBoard board
    ) {
      if (board==null) throw new ArgumentNullException("board"); 
      return FindDirectedPath(start, goal, board.DirectedStepCost, board.Heuristic)
              .PathFwd;
    }

    public static IDirectedPath FindDirectedPathRev(
      IHex start,
      IHex goal,
      IDirectedNavigableBoard board
    ) {
      if (board==null) throw new ArgumentNullException("board"); 
      return FindDirectedPath(start, goal, board.DirectedStepCost, board.Heuristic)
              .PathRev;
    }

    /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="start">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <param name="goal">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="stepCost">Cost to extend path by hex at <c>coords</c> from hex at direction <c>hexside</c>.</param>
    /// <param name="heuristic">Returns a monotonic (ie locally admissible) cost estimate from a range value.</param>
    /// ///<remarks>Note that <c>heuristic</c> <b>must</b> be monotonic in order for the algorithm to perform properly.</remarks>
    /// <seealso cref="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html"/>
    static PathHalves FindDirectedPath (
      IHex start,
      IHex goal,
      Func<IHex, Hexside, int> stepCost,
      Func<int,int>            heuristic
    ) {
      if (start==null)      throw new ArgumentNullException("start"); 
      if (goal==null)       throw new ArgumentNullException("goal"); 
      if (stepCost==null)   throw new ArgumentNullException("stepCost"); 
      if (heuristic==null)  throw new ArgumentNullException("heuristic"); 

      var closed        = new HashSet<HexCoords>();

      var pathHalves    = new PathHalves();

      var pathfinderFwd = new PathfinderFwd(start, goal, heuristic, closed, 
                          stepCost, pathHalves.SetBestSoFar, ()=>pathHalves.BestSoFar);

      var pathfinderRev = new PathfinderRev(start, goal, heuristic, closed, 
                          stepCost, pathHalves.SetBestSoFar, ()=>pathHalves.BestSoFar);

      pathfinderFwd.Partner = pathfinderRev;
      pathfinderRev.Partner = pathfinderFwd;

      var pathfinder = pathfinderRev.Partner;
      while (! pathfinder.IsFinished()) {
        pathfinder = pathfinder.Partner;
      }

      return pathHalves;
    }

    /// <summary><c>Static Hexside[]></c> for enumerations.</summary>
    static readonly Hexside[] Hexsides = Utilities.EnumGetValues<Hexside>().ToArray();

    private sealed class PathHalves {
      public PathHalves() { BestSoFar   = (Int32)Int16.MaxValue; }

      public int          BestSoFar { get; private set; }
      public DirectedPath PathFwd   { get { return MergePaths(_pathFwd, _pathRev); } }
      public DirectedPath PathRev   { get { return MergePaths(_pathRev, _pathFwd); } }

      public void SetBestSoFar(DirectedPath pathFwd, DirectedPath pathRev) {
        if (pathFwd!=null  &&  pathRev!=null
        && pathFwd.TotalCost + pathRev.TotalCost < BestSoFar) {
          _pathFwd   = pathFwd; 
          _pathRev   = pathRev; 
          BestSoFar = _pathFwd.TotalCost + pathRev.TotalCost;
        }
      }

      public static DirectedPath MergePaths(DirectedPath fwd, DirectedPath rev) {
        if (rev!=null) {
          while (rev.PathSoFar!=null) {
            var hexside = rev.PathStep.HexsideEntry;
            var cost    = rev.TotalCost - (rev = rev.PathSoFar).TotalCost;
            fwd = fwd.AddStep(rev.PathStep.Hex, hexside, cost);
          }
        }
        return fwd;
      }

      DirectedPath _pathFwd;
      DirectedPath _pathRev;
    }

    private sealed class PathfinderFwd : Pathfinder {
      public PathfinderFwd(IHex start, IHex goal, Func<int,int> heuristic, 
        ISet<HexCoords> closed, 
        Func<IHex, Hexside, int> stepCost,
        Action<DirectedPath,DirectedPath> setBestSoFar,   Func<int> getBestSoFar
      ) : base (start, goal, heuristic, closed, getBestSoFar) {
        _stepCost     = (here,hexside,there) => stepCost(there, hexside.Reversed());
        _addStep      = (path,there,hexside,cost) => path.AddStep(there,hexside,cost);
        _setBestSoFar = (self,partner) => setBestSoFar(self,partner);
      }

      #if UseShortcuts
        protected override void ExpandShortcuts(DirectedPath path) {
          //foreach (var shortcut in path.PathStep.Hex.Shortcuts) {
          //  ExpandShortcut(path, shortcut.DirectedPath);
          //}
        }
      #endif
    }

    private sealed class PathfinderRev : Pathfinder {
      public PathfinderRev(IHex start, IHex goal, Func<int,int> heuristic, 
        ISet<HexCoords> closed, 
        Func<IHex, Hexside, int> stepCost,
        Action<DirectedPath,DirectedPath> setBestSoFar,   Func<int> getBestSoFar
      ) : base (goal, start, heuristic, closed, getBestSoFar) {
        _stepCost     = (here,hexside,there) => stepCost(here, hexside);
        _addStep      = (path,there,hexside,cost) => path.AddStep(there,hexside.Reversed(),cost);
        _setBestSoFar = (self,partner) => setBestSoFar(partner,self);
      }

      #if UseShortcuts
        protected override void ExpandShortcuts(DirectedPath path) {
          foreach (var shortcut in path.PathStep.Hex.Shortcuts) {
            ExpandShortcut(path, shortcut.DirectedPath);
          }
        }
      #endif
    }

    private abstract class Pathfinder {
      public Pathfinder(IHex start, IHex goal, Func<int,int> heuristic, 
        ISet<HexCoords> closed, Func<int> getBestSoFar
      ) {
        _start        = start;
        _heuristic    = heuristic;
        _getBestSoFar = getBestSoFar;

        _vectorGoal   = goal.Coords.Canon - start.Coords.Canon;
        _open         = new Dictionary<HexCoords, DirectedPath>();
        _closed       = closed;
        _queue        = new HotPriorityQueue<DirectedPath>(16);

        TraceFlags.FindPathDetail.Trace(true, "Find path from {0} to {1}; vectorGoal = {2}", 
                                          start.Coords, goal.Coords, _vectorGoal);

        var path = new DirectedPath(goal);
        _queue.Enqueue (0, path);
        _open.Add(goal.Coords, path);
      }

      public Pathfinder   Partner  { get; set; }

      public int          FrontierMinimumLessHeuristic(IHex hex) {
        HexKeyValuePair<int,DirectedPath> item;

        return (_queue.TryPeek(out item) ? item.Key >> 16 : (Int32)Int16.MaxValue)
             - _heuristic(_start.Range(hex));
      }
      public DirectedPath GetPartnerPath(HexCoords coords) {
        DirectedPath path;
        _open.TryGetValue(coords,out path);
        return path;
      }

      int  Estimate(IHex there, int totalCost) {
        var estimate   = (int)_heuristic(_start.Coords.Range(there.Coords)) + totalCost;
        var preference = Preference(_vectorGoal, there.Coords.Canon - _start.Coords.Canon );
        return (estimate << 16) + preference;
      }
      static int Preference(IntVector2D vectorGoal, IntVector2D vectorHex) {
        return (0xFFFF & Math.Abs(vectorGoal ^ vectorHex ));
      }

      public bool IsFinished() {
        HexKeyValuePair<int,DirectedPath> item;
        if ( ! _queue.TryDequeue(out item)) return true;

        var key  = item.Key;
        var path = item.Value;
        var hex  = path.PathStep.Hex;

        _open.Remove(hex.Coords);
        if( ! _closed.Contains(hex.Coords) ) {
          TraceFlags.FindPathDequeue.Trace(
            "Dequeue Path at {0} w/ cost={1,4} at {2}; estimate={3,4}:{4,4}.", 
            hex.Coords, path.TotalCost, path.HexsideExit, key >> 16, key & 0xFFFF);

          if (item.Key>>16 < _getBestSoFar()
          &&  path.TotalCost + Partner.FrontierMinimumLessHeuristic(hex) < _getBestSoFar()) {
            #if UseParallel
              var options = new ParallelOptions();
              options.MaxDegreeOfParallelism = 6;
              ExpandHexResult[] array = new ExpandHexResult[6];
              Parallel.For(0, Hexsides.Length, options,
                index => array[index] = ExpandHex2(path, Hexsides[index]));

              ExpandHexResult.CheckPaths(_open, _queue, _setBestSoFar, Partner, array);
            #else
              for (var index = 0; index < Hexsides.Length; index++) {
//                ExpandHex(path, Hexsides[index]);
                ExpandHex(path, (Hexside)index);
              }
            #endif

            #if UseShortcuts
              if (UseShortcuts)   ExpandShortcuts(path);
            #endif
          }
          _closed.Add(hex.Coords);
        }
        return ! _queue.Any();
      }
#if UseParallel

      private ExpandHexResult ExpandHex2(DirectedPath path, Hexside hexside) {
        var here  = path.PathStep.Hex;
        var there = here.Neighbour(hexside);
        if (there != null  &&  ! _closed.Contains(there.Coords) ) {
          var cost = _stepCost(here, hexside, there);
          if ( cost > 0
          &&  (cost < _getBestSoFar()  ||  ! _open.ContainsKey(there.Coords)) ) {
            var newPath = _addStep(path, there, hexside, cost);
            var key     = Estimate(there, newPath.TotalCost);
            TraceFlags.FindPathEnqueue.Trace("   Enqueue {0}: estimate={1,4}:{2,4}",
                                          there.Coords, key>>16, key & 0xFFFF);

            DirectedPath oldPath;
            _open.TryGetValue(there.Coords, out oldPath);

            return new ExpandHexResult() {
              IsValue     = true,
              ThereCoords = there.Coords,
              OldPath     = oldPath,
              NewPath     = newPath,
              Key         = key
            };
          }
        }
        return new ExpandHexResult() {IsValue = false};
      }

      private struct ExpandHexResult {
        public bool          IsValue;
        public HexCoords     ThereCoords;
        public DirectedPath  OldPath;
        public DirectedPath  NewPath;
        public int           Key;

        public static void CheckPaths(
          IDictionary<HexCoords,DirectedPath> open,
          IPriorityQueue<int, DirectedPath>   queue,
          Action<DirectedPath,DirectedPath>   setBestSoFar,
          Pathfinder                          partner,
          ExpandHexResult[]                   array
        ) {
          for (var i=0; i<array.Length; i++) {
            var item = array[i];
            if (item.IsValue) {
              if (item.OldPath == null)  {
                open.Add(item.ThereCoords, item.NewPath);
                queue.Enqueue(item.Key, item.NewPath);
              } else if (item.NewPath.TotalCost < item.OldPath.TotalCost) {
                open.Remove(item.ThereCoords);
                open.Add(item.ThereCoords, item.NewPath);
                queue.Enqueue(item.Key, item.NewPath);
              }
            }

            setBestSoFar(item.NewPath, partner.GetPartnerPath(item.ThereCoords));
          }
        }
      }
#else
      internal void ExpandHex(DirectedPath path, Hexside hexside) {
        var here  = path.PathStep.Hex;
        var there = here.Neighbour(hexside);
        if (there != null  &&  ! _closed.Contains(there.Coords) ) {
          var cost = _stepCost(here, hexside, there);
          if ( cost > 0
          &&  (cost < _getBestSoFar()  ||  ! _open.ContainsKey(there.Coords)) ) {
            var newPath = _addStep(path, there, hexside, cost);
            var key     = Estimate(there, newPath.TotalCost);
            TraceFlags.FindPathEnqueue.Trace("   Enqueue {0}: estimate={1,4}:{2,4}",
                                          there.Coords, key>>16, key & 0xFFFF);

            DirectedPath oldPath;
            if ( ! _open.TryGetValue(there.Coords, out oldPath))  {
              _open.Add(there.Coords, newPath);
              _queue.Enqueue(key, newPath);
            } else if (newPath.TotalCost < oldPath.TotalCost) {
              _open.Remove(there.Coords);
              _open.Add(there.Coords, newPath);
              _queue.Enqueue(key, newPath);
            }

            _setBestSoFar(newPath, Partner.GetPartnerPath(there.Coords));
          }
        }
      }
#endif

#if UseShortcuts
      protected void ExpandShortcut(DirectedPath path, IDirectedPath shortcut) {
        var hexside = shortcut.PathStep.HexsideExit;
        if (path.PathStep.Hex.Equals(shortcut.PathStep.Hex)) shortcut = shortcut.PathSoFar;

        var here = path.PathStep.Hex;
        while (shortcut.PathSoFar != null) {
          var there = shortcut.PathStep.Hex;

          if (there != null  &&  ! _closed.Contains(there.Coords) ) {
            var cost = _stepCost(here, hexside, there);
            if ( cost > 0
            &&  (cost < _bestSoFar()  ||  ! _open.ContainsKey(there.Coords)) ) {
              path = _addStep(path, there, hexside, cost);
              var key     = Estimate(there, path.TotalCost);
              TraceFlags.FindPathEnqueue.Trace("   Enqueue {0}: estimate={1,4}:{2,4}",
                                            there.Coords, key>>16, key & 0xFFFF);

              DirectedPath oldPath;
              if ( ! _open.TryGetValue(there.Coords, out oldPath))  {
                _open.Add(there.Coords, path);
                _queue.Enqueue(key, path);
              } else if (path.TotalCost < oldPath.TotalCost) {
                _open.Remove(there.Coords);
                _open.Add(there.Coords, path);
                _queue.Enqueue(key, path);
              }

              _setBestSoFar(path, Partner.GetPartnerPath(there.Coords));
            }
          }

          hexside  = shortcut.PathStep.HexsideExit;
          shortcut = shortcut.PathSoFar;
          here     = there;
        }
      }
      protected abstract void ExpandShortcuts(DirectedPath path);
#endif

      protected IHex                                _start;
      protected Func<int,int>                       _heuristic;
      protected Func<IHex, Hexside, IHex, int>      _stepCost;
      protected Func<DirectedPath,IHex,Hexside,int,DirectedPath> _addStep;

      protected IDictionary<HexCoords,DirectedPath> _open;
      protected ISet<HexCoords>                     _closed;
      protected IntVector2D                         _vectorGoal;
      protected IPriorityQueue<int, DirectedPath>   _queue;
      protected Action<DirectedPath,DirectedPath>   _setBestSoFar;
      protected Func<int>                           _getBestSoFar;
    }
  }
}
