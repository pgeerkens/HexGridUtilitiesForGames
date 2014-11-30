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
using System.Diagnostics;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {

  /// <summary>TODO</summary>
  public interface IPathfinder {
    /// <summary>The <see cref="INavigableBoard"/> on which the shortest-path has been requested.</summary>
    INavigableBoard Board     { get; }
    /// <summary>The <see cref="ISet{HexCoords}"/> of all hexes expanded in finding the shortest-path.</summary>
    ISet<HexCoords> ClosedSet { get; }
    /// <summary>The target hex for this shortest-path search.</summary>
    IHex            Source    { get; }
    /// <summary>The source hex for this shortest-path search.</summary>
    IHex            Target    { get; }
  }

  /// <summary></summary>
  public abstract class Pathfinder : IPathfinder {
    /// <summary>TODO</summary>
    /// <param name="board">Board on which this shortest-path search is taking place.</param>
    /// <param name="source">Source hex for this shortest-path search.</param>
    /// <param name="target">Target hex for this shortest-path search.</param>
    /// <param name="closedSet">Injected implementation of <see cref="ISet{HexCoords}"/>.</param>
    protected internal Pathfinder(INavigableBoard board, IHex source, IHex target, ISet<HexCoords> closedSet) {
      if (board          ==null) throw new ArgumentNullException("board"); 
      if (board.Landmarks==null) throw new ArgumentNullException("board","Member Landmarks must not be null");
      if (closedSet      ==null) throw new ArgumentNullException("closedSet"); 
      if (source         ==null) throw new ArgumentNullException("source"); 
      if (target         ==null) throw new ArgumentNullException("target"); 

      Board     = board;
      ClosedSet = closedSet;
      Source    = source;
      Target    = target;
    }

    /// <inheritdoc/>
    public          INavigableBoard Board     { get; private set; }
    /// <inheritdoc/>
    public          ISet<HexCoords> ClosedSet { get; private set; }
    /// <inheritdoc/>
    public          IHex            Source    { get; private set; }
    /// <inheritdoc/>
    public          IHex            Target    { get; private set; }

    #region Conditional tracing routines
    /// <summary>If the conditional constant TRACE is defined: writes the search start- and goal-coords to the trace log.</summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    [Conditional("TRACE")]
    protected static void TraceFindPathDetailInit(HexCoords start, HexCoords goal) {
      Traces.FindPathDetail.Trace(true, "Fwd: Find path from {0} to {1}:", start, goal);
    }
    /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
    /// <param name="searchDirection"></param>
    /// <param name="vectorGoal"></param>
    [Conditional("TRACE")]
    protected static void TraceFindPathDetailDirection(string searchDirection, IntVector2D vectorGoal) {
      Traces.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}", searchDirection, vectorGoal);
    }
    /// <summary>If the conditional constant TRACE is defined: writes the search-direction initialization details to the trace log.</summary>
    /// <param name="searchDirection"></param>
    /// <param name="vectorGoal"></param>
    /// <param name="landmarkCoords"></param>
    [Conditional("TRACE")]
    protected static void TraceFindPathDetailDirection(string searchDirection, IntVector2D vectorGoal, HexCoords landmarkCoords) {
      Traces.FindPathDetail.Trace("   {0} Search uses: vectorGoal = {1}; and landmark at {2}", 
                                    searchDirection, vectorGoal, landmarkCoords);
    }
    /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
    /// <param name="searchDirection"></param>
    /// <param name="coords"></param>
    /// <param name="cost"></param>
    /// <param name="exit"></param>
    /// <param name="priority"></param>
    /// <param name="preference"></param>
    [Conditional("TRACE")]
    protected static void TraceFindPathDequeue(string searchDirection,HexCoords coords, int cost, Hexside exit, int priority, int preference) {
      Traces.FindPathDequeue.Trace(
            "{0} Dequeue Path at {1} w/ cost={2,4} at {3,-9}; estimate={4,4}:{5,4}.", 
            searchDirection, coords, cost, exit, priority, preference);
    }
    /// <summary>If the conditional constant TRACE is defined: writes the enqueue details to the trace log.</summary>
    /// <param name="coords"></param>
    /// <param name="priority"></param>
    /// <param name="preference"></param>
    [Conditional("TRACE")]
    protected static void TraceFindPathEnqueue(HexCoords coords,int priority, int preference) {
      Traces.FindPathEnqueue.Trace(
          "   Enqueue {0}: estimate={1,4}:{2,4}",coords, priority, preference);
    }
    /// <summary>If the conditional constant TRACE is defined: writes the current direction pairing to the trace log.</summary>
    /// <param name="coordsFwd"></param>
    /// <param name="coordsRev"></param>
    /// <param name="bestSoFar"></param>
    [Conditional("TRACE")]
    public static void TraceFindPathDetailBestSoFar(HexCoords coordsFwd, HexCoords coordsRev, int bestSoFar) {
      Traces.FindPathDetail.Trace("   SetBestSoFar: pathFwd at {0}; pathRev at {1}; Cost = {2}",
          coordsFwd,coordsRev, bestSoFar);
    }
    /// <summary>If the conditional constant TRACE is defined: writes the dequeue details to the trace log.</summary>
    /// <param name="count"></param>
    [Conditional("TRACE")]
    protected static void TraceFindPathDone(int count) {
      Traces.FindPathDequeue.Trace("Closed: {0,7}", count);
    }
    #endregion
  }
}
