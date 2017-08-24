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
using System.Diagnostics;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>A board location storing shortest-path distances to every board hex.</summary>
  /// <remarks>
  /// A board location that stores shortest-path distances for every board hex, for use in 
  /// computing an accurate A* heuristic. A simple Dijkstra implementation is used to generate the 
  /// distances upon creation.
  /// No Finalizer is implemented as the class possesses no unmanaged resources.
  /// </remarks>
  [DebuggerDisplay("Coords={Coords}")]
  public sealed partial class Landmark : ILandmark, IDisposable {
    /// <summary>Populates and returns a new landmark at the specified board coordinates.</summary>
    /// <param name="board">IBoard{IHex} on which the landmark is to be created.</param>
    /// <param name="coords">Coordinates on <c>board</c> where this landmark is to be created.</param>
    public Landmark(HexCoords coords, IHexBoard<IHex> board) {
      Board  = board;
      Coords = coords;
      FillLandmark();
    }

    /// <summary>TODO</summary>
    public IHexBoard<IHex> Board  { get; private set; }
    /// <summary>Board coordinates for the landmark location.</summary>
    public HexCoords       Coords { get; private set; }

    /// <summary>Returns the shortest-path directed-distance to the specified hex <b>from</b> the landmark.</summary>
    public int DistanceTo  (HexCoords coords) { return backingStore[0][coords]; }
    /// <summary>Returns the shortest-path directed-distance from the specified hex <b>to</b> the landmark.</summary>
    public int DistanceFrom(HexCoords coords) { return backingStore[1][coords]; }

    private static Func<IHexBoard<IHex>, Func<IHex,Hexside,IHex,int>>[] DirectedCostDelegates = 
      new Func<IHexBoard<IHex>, Func<IHex,Hexside,IHex,int>>[] {
          /* Cost from here to there */ (board) => (here,hexside,there) => board.GetDirectedCostToExit(here, hexside),
          /* Cost from there to here */ (board) => (here,hexside,there) => board.GetDirectedCostToExit(there, hexside.Reversed())
      };

    private BoardStorage<short>[] backingStore;

    /// <inheritdoc/>
    public void FillLandmark() {
      if (Board != null) {
        backingStore = new BoardStorage<short>[2] {
          new BlockedBoardStorage32x32<short>(Board.MapSizeHexes, c => -1),
          new BlockedBoardStorage32x32<short>(Board.MapSizeHexes, c => -1)
        };

        for (var i = 0; i < 2; i++) {
          var landmark  = Board[Coords];
          if (landmark != null)
            FindPathDetailTrace(true, "Find distances from {0}", landmark.Coords);

            #if HotPriorityQueue
              var queue = PriorityQueueFactory.NewHotPriorityQueue<IHex>();
            #else
              var queue = PriorityQueueFactory.NewDictionaryQueue<int, IHex>();
            #endif
            queue.Enqueue (0, landmark);

            FillLandmarkDetail(queue, backingStore[i], DirectedCostDelegates[i](Board));
        }
      }
    }

    private void FillLandmarkDetail(IPriorityQueue<int,IHex> queue, 
      BoardStorage<short> store, Func<IHex,Hexside,IHex,int> directedStepCost
    ) {
      HexKeyValuePair<int,IHex> item;
      while (queue.TryDequeue(out item)) {
        var here = item.Value;
        var key  = item.Key;
        if( store[here.Coords] > 0 ) continue;

        FindPathDetailTrace("Dequeue Path at {0} w/ cost={1,4}.", here, key);

        store[here.Coords] = (short)key;

        HexsideExtensions.HexsideList.ForEach( hexside =>
          ExpandNode(store,directedStepCost,queue,here,key,hexside)
        );
      }
    }

    private void ExpandNode(BoardStorage<short> store, Func<IHex,Hexside,IHex,int> directedStepCost, 
      IPriorityQueue<int,IHex> queue, IHex here, int key, Hexside hexside
    ) {
      var neighbourCoords = here.Coords.GetNeighbour(hexside);
      var neighbourHex    = Board[neighbourCoords];

      if (neighbourHex != null) {
        var cost = directedStepCost(here, hexside, neighbourHex);
        if (cost > 0  &&  store[neighbourCoords] == -1) {

          FindPathDetailTrace("   Enqueue {0}: {1,4}", neighbourCoords, cost);

          queue.Enqueue(key + cost, neighbourHex);
        }
      }
    }

    #region IDisposable implementation
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (backingStore!=null) {
            if (backingStore[0] != null) backingStore[0].Dispose();
            if (backingStore[1] != null) backingStore[1].Dispose();
          }
        }
        _isDisposed = true;
      }
    }
    #endregion

    #region Tracing partial methods
    static partial void FindPathDetailTrace(string format, params object[] paramArgs);
    static partial void FindPathDetailTrace(bool newline, string format, params object[] paramArgs);
    #if TRACE
    static partial void FindPathDetailTrace(string format, params object[] paramArgs) {
      Traces.FindPathDetail.Trace(format, paramArgs);
    }
    static partial void FindPathDetailTrace(bool newline, string format, params object[] paramArgs) {
      Traces.FindPathDetail.Trace(newline, format, paramArgs);
    }
    #endif
    #endregion
  }
}
