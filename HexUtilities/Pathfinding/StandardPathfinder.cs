#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
  using IPriorityQueue = IPriorityQueue<int,IDirectedPathCollection>;

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
  /// <seealso cref="PGNapoleonics.HexUtilities.Pathfinding.LandmarkPathfinder"/>
  /// </para>
  /// </remarks>
  /// <see cref="LandmarkPathfinder"/>
  public sealed class StandardPathfinder : Pathfinder {
    /// <summary>Returns an <c>IDirectedPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <returns>A <c>IDirectedPathCollection</c>  for the shortest path found, or null if no path was found.</returns>
    /// <remarks>
    /// <para>Note that the Heuristic provided by <paramref name="board"/> <b>must</b> be monotonic in order for the algorithm to perform properly.</para>
    /// <seealso cref="PGNapoleonics.HexUtilities.Pathfinding.StandardPathfinder"/>
    /// </remarks>
    public static IDirectedPath FindDirectedPathFwd(INavigableBoard board, HexCoords source, HexCoords target) {
      return (new StandardPathfinder(board,source,target)).PathForward;
    }
    /// <summary>TODO</summary>
    /// <param name="board"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns><see cref="StandardPathfinder"/>.</returns>
    public static StandardPathfinder New(INavigableBoard board, HexCoords source, HexCoords target) {
      return new StandardPathfinder(board,source,target);
    }

    /// <summary>Creates a new <see cref="Pathfinder"/> instance implementing a unidirectional A* from 
    /// <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    internal StandardPathfinder(INavigableBoard board, HexCoords source, HexCoords target) 
    : base(source,target,new HashSet<HexCoords>()
    ) {

      _heuristic       = board.Heuristic;
      _openSet         = new HashSet<HexCoords>();
      _queue           = new DictionaryPriorityQueue<int, IDirectedPath>();
      _tryDirectedCost = board.TryEntryCost;
      _vectorGoal      = Target.Canon - Source.Canon;

      _pathForward     = GetPath();

   //   TraceFindPathDone(ClosedSet.Count);
    }
    
    private Maybe<int> Estimate(HexCoords start, HexCoords hex, int totalCost) {
      var vectorStart = start.Canon - hex.Canon;
      //var preference = Preference(_vectorGoal, vectorStart);
      //var estimate   = _heuristic(start,hex) + totalCost;
      //return (estimate << 16) + preference;
      return from heuristic in _heuristic(start,hex)
             let estimate   = heuristic + totalCost
             let preference = Preference(_vectorGoal, vectorStart)
             select (estimate << 16) + preference;
    }

    
    static int Preference(IntVector2D vectorGoal, IntVector2D vectorHex) {
      return (0xFFFF & Math.Abs(vectorGoal ^ vectorHex));
    }

    private IDirectedPath GetPath() {
      TraceFindPathDetailInit(Source, Target);

      // Minimize field references by putting these on the stack now
      ISet<HexCoords> openSet         = _openSet;
      IPriorityQueue  queue           = _queue;
      TryDirectedCost tryDirectedCost = _tryDirectedCost;

     // Func<HexCoords,HexCoords,int> heuristic       = _heuristic;
     // IntVector2D                   vectorGoal      = _vectorGoal;

      queue.Enqueue (0, new DirectedPath(Target));

      HexKeyValuePair<int,IDirectedPath> item;
      while (queue.TryDequeue(out item)) {
        var path = item.Value;
        var step = path.PathStep.Coords;

        openSet.Add(step);
        if( ClosedSet.Contains(step) ) continue;

        TraceFindPathDequeue("Rev",step, path.TotalCost, path.HexsideExit, item.Key>>16, 
                            (int) ((int)(item.Key & 0xFFFFu) - 0x7FFF));

        if(step == Source)     return path;

        ClosedSet.Add(step);

        Hexside.HexsideList.ForEach(hexside => {
          var neighbourCoords = step.GetNeighbour(hexside);
          tryDirectedCost(step, hexside).IfHasValueDo( cost => {
            var newPath = path.AddStep(neighbourCoords, hexside, cost);
            Estimate(Source, neighbourCoords, newPath.TotalCost).IfHasValueDo(key => {
              TraceFindPathEnqueue(neighbourCoords, key>>16, (int)(key & 0xFFFFu));

              queue.Enqueue(key, newPath);
            } );
          } );
        } );
      }

      return null;
    }

    /// <inheritdoc/>
    public  override IDirectedPath  PathForward {
      get { return _pathForward; }
    } readonly IDirectedPath _pathForward;
    /// <inheritdoc/>
    public  override IDirectedPath  PathReverse { 
      get {return Pathfinder.MergePaths(new DirectedPath(Source), PathForward.PathSoFar);}
    }
    /// <see cref="PathForward"/>
    [Obsolete("Deprecated - use property PathForward instead.",true)]
    public  IDirectedPath           Path        { get {return PathForward;} }

    readonly Func<HexCoords,HexCoords,Maybe<short>> _heuristic;
    readonly ISet<HexCoords>                 _openSet;
    readonly IPriorityQueue                  _queue;
    readonly IntVector2D                     _vectorGoal;
    readonly TryDirectedCost                 _tryDirectedCost;
  }
}
