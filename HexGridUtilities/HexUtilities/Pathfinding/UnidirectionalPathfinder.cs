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
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  using DirectedPath  = DirectedPathCollection;
  using IDirectedPath = IDirectedPathCollection;

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
  /// <seealso cref="PGNapoleonics.HexUtilities.Pathfinding.BidirectionalPathfinder"/>
  /// </para>
  /// </remarks>
  /// <see cref="BidirectionalPathfinder"/>
  public sealed class UnidirectionalPathfinder : Pathfinder {
    /// <summary>Returns an <c>IDirectedPath</c> for the optimal path from coordinates <c>start</c> to <c>goal</c>.</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    /// <returns>A <c>IDirectedPathCollection</c>  for the shortest path found, or null if no path was found.</returns>
    /// <remarks>
    /// <para>Note that the Heuristic provided by <paramref name="board"/> <b>must</b> be monotonic in order for the algorithm to perform properly.</para>
    /// <seealso cref="PGNapoleonics.HexUtilities.Pathfinding.UnidirectionalPathfinder"/>
    /// </remarks>
    public static IDirectedPath FindDirectedPathFwd(INavigableBoard<IHex> board, IHex source, IHex target) {
      return (new UnidirectionalPathfinder(board,source,target)).Path;
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

    /// <summary>Creates a new <see cref="Pathfinder"/> instance implementing a unidirectional A* from 
    /// <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
    /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
    /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
    public UnidirectionalPathfinder(INavigableBoard<IHex> board, IHex source, IHex target) 
    : base(board,source,target,new HashSet<HexCoords>()
    ) {
      if (board==null) throw new ArgumentNullException("board");

      StepCost  = (hex,hexside) => board.GetDirectedCostToExit(hex,hexside);
      Heuristic = board.Heuristic;
      Path      = GetPath();

      TraceFindPathDone(ClosedSet.Count);
    }

    private IDirectedPath GetPath() {
      VectorGoal  = Target.Coords.Canon - Source.Coords.Canon;
      OpenSet     = new HashSet<HexCoords>();
      Queue       = new DictionaryPriorityQueue<int, IDirectedPath>();

      TraceFindPathDetailInit(Source.Coords, Target.Coords);

      Queue.Enqueue (0, new DirectedPath(Target));

      HexKeyValuePair<int,IDirectedPath> item;
      while (Queue.TryDequeue(out item)) {
        var path = item.Value;
        var step = path.PathStep.Hex;

        OpenSet.Add(step.Coords);
        if( ClosedSet.Contains(step.Coords) ) continue;

        TraceFindPathDequeue("Rev",step.Coords, path.TotalCost, path.HexsideExit, item.Key>>16, 
                            (int) ((int)(item.Key & 0xFFFFu) - 0x7FFF));

        if(step.Equals(Source))     return path;

        ClosedSet.Add(step.Coords);

        foreach (var neighbour in Board.GetNeighbourHexes(step)) {
          ExpandNeighbour(path,neighbour);
        }
      }

      return null;
    }

    void ExpandNeighbour(IDirectedPath path, NeighbourHex neighbour) {
      if ( ! OpenSet.Contains(neighbour.Hex.Coords)) {
        var cost = StepCost(neighbour.Hex, neighbour.HexsideExit);
        if (cost > 0) {
          var newPath = path.AddStep(neighbour, cost);
          var key     = Estimate(Heuristic, VectorGoal, Source.Coords, 
                                 neighbour.Hex.Coords, newPath.TotalCost);

          TraceFindPathEnqueue(neighbour.Hex.Coords, key>>16, (int)(key & 0xFFFFu));

          Queue.Enqueue(key, newPath);
        }
      }
    }

    /// <inheritdoc/>
    public  IDirectedPath                     Path        { get; private set; }

    private Func<int,int>                     Heuristic   { get; set; }
    private ISet<HexCoords>                   OpenSet     { get; set; }
    private IPriorityQueue<int,IDirectedPath> Queue       { get; set; }
    private Func<IHex, Hexside, int>          StepCost    { get; set; }
    private IntVector2D                       VectorGoal  { get; set; }
  }
}
