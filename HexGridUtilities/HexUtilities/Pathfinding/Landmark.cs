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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>A <b>ReadOnlyCollection</b> of defined <see cref="Landmark"/> locations.</summary>
  public sealed class LandmarkCollection : ReadOnlyCollection<Landmark> {
    /// <summary>Creates a populated <see cref="Collection{T}"/> of <see cref="Landmark"/>
    /// instances.</summary>
    /// <param name="board">The board on which the collection of landmarks is to be instantiated.</param>
    /// <param name="landmarkCoords">Board coordinates of the desired landmarks</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", 
      "CA2000:Dispose objects before losing scope")]
    public static LandmarkCollection CreateLandmarks(
      IBoard<IHex> board, 
      IList<HexCoords> landmarkCoords
    ) {
      if (board==null)  throw new ArgumentNullException("board");
      if (landmarkCoords==null) throw new ArgumentNullException("landmarkCoords");

      var list = new List<Landmark>(landmarkCoords.Count);

      for (var i=0; i<landmarkCoords.Count; i++) list.Add(null);

      Parallel.For(0, landmarkCoords.Count, i => {
        Landmark tempLandmark = null;
        try {
          tempLandmark = new Landmark(landmarkCoords[i],board);
          list[i]      = tempLandmark;
          tempLandmark = null;
        } finally { if (tempLandmark!=null) tempLandmark.Dispose(); }
      } );

      return new LandmarkCollection(list);
    }

    LandmarkCollection(List<Landmark> list) : base(list) {}
  }

  /// <summary>A board location storing shortest-path distances to every board hex.</summary>
  /// <remarks>A board location that stores shortest-path distances for every board hex, for use in 
  /// computing an accurate A* heuristic. A simple Dijkstra implementation is used to generate the 
  /// distances upon creation.</remarks>
  public sealed class Landmark : IDisposable {
    /// <summary>Populates and returns a new landmark at the specified board coordinates.</summary>
    /// <param name="board">IBoard{IHex} on which the landmark is to be created.</param>
    /// <param name="coords">Coordinates on <c>board</c> where this landmark is to be created.</param>
    public Landmark(HexCoords coords, IBoard<IHex> board) {
      if (board == null) throw new ArgumentNullException("board"); 

      Coords       = coords;
      backingStore = new BoardStorage<short>.BlockedBoardStorage32x32(board.MapSizeHexes, c => -1);
      FillLandmark(board);
    }

    /// <summary>Board coordinates for the landmark location.</summary>
    public HexCoords Coords { get; private set; }

    /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
    public int HexDistance(HexCoords coords) { return backingStore[coords]; }

    BoardStorage<short> backingStore;

    void FillLandmark(IBoard<IHex> board) {
      var start  = board[Coords];
      var queue  = DictionaryPriorityQueue<int, IHex>.NewQueue();
      TraceFlags.FindPathDetail.Trace(true, "Find distances from {0}", start.Coords);

      queue.Enqueue (0, start);

      HexKeyValuePair<int,IHex> item;
      while (queue.TryDequeue(out item)) {
        var here = item.Value;
        var key  = item.Key;
        if( backingStore[here.Coords] > 0 ) continue;

        TraceFlags.FindPathDetail.Trace("Dequeue Path at {0} w/ cost={1,4}.", here, key);
        backingStore[here.Coords] = (short)key;

        foreach (var there in here.GetAllNeighbours().Where(n => n!=null && n.Hex.IsOnboard())) {
          var cost = board.DirectedStepCost(here, there.HexsideEntry);
          if (cost > 0  &&  backingStore[there.Hex.Coords] == -1) {
            TraceFlags.FindPathDetail.Trace("   Enqueue {0}: {1,4}", there.Hex.Coords, cost);
            queue.Enqueue(key + cost, there.Hex);
          }
        }
      }
    }

    #region IDisposable implementation with Finalizer
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (backingStore!=null) backingStore.Dispose();
        }
        _isDisposed = true;
      }
    }
    /// <summary>TODO</summary>
    ~Landmark() { Dispose(false); }
    #endregion
  }
}
