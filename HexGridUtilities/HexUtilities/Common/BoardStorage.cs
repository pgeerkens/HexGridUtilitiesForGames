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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Abstract specification and partial implementation of the <c>BoardStorage</c> required by <c>HexBoard</c>.</summary>
  /// <typeparam name="T">The type of the information being stored.</typeparam>
  public abstract class BoardStorage<T> : IDisposable {
    /// <summary>Initializes a new instance with the specified hex extent.</summary>
    /// <param name="sizeHexes"></param>
    protected BoardStorage(Size sizeHexes) {
      MapSizeHexes   = sizeHexes;
    }

    /// <summary>Extent in hexes of the board, as a <see cref=" System.Drawing.Size"/> struct.</summary>
    public          Size MapSizeHexes           { get; private set; }

    /// <summary>Returns the <c>THex</c> instance at the specified coordinates.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1043:UseIntegralOrStringArgumentForIndexers")]
    public abstract T this[HexCoords coords] { get; internal set; }

    /// <summary>Returns whether the hex with <see cref="HexCoords"/> <c>coords</c> is 
    /// within the extent of the board.</summary>
    public          bool IsOnboard(HexCoords coords) { 
      return 0<=coords.User.X && coords.User.X < MapSizeHexes.Width
          && 0<=coords.User.Y && coords.User.Y < MapSizeHexes.Height;
    }

    /// <summary>Perform the specified <c>action</c> serially on all hexes.</summary>
    public abstract void ForEach(Action<T> action);

    /// <summary>Perform the specified <c>action</c> serially on all hexes satisfying <paramref name="predicate"/>/>.</summary>
    public abstract void ForEach(Func<T,bool> predicate, Action<T> action);

    /// <summary>Perform the specified <c>action</c> in parallel on all hexes.</summary>
    public abstract void ParallelForEach(Action<T> action);

    /// <summary>Perform the specified <c>action</c> in parallel on all hexes satisfying <paramref name="predicate"/>.</summary>
    public abstract void ParallelForEach(Func<T,bool> predicate, Action<T> action);

    #region IDisposable implementation with Finalizeer
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Anchors the Dispose chain for sub-classes.</summary>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
        }
        _isDisposed = true;
      }
    }
    /// <summary>Finalize this instance.</summary>
    ~BoardStorage() { Dispose(false); }
    #endregion

    /// <summary>A row-major <c>BoardStorage</c> implementation optimized for small maps.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1034:NestedTypesShouldNotBeVisible")]
    public sealed class FlatBoardStorage : BoardStorage<T> {
      /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
      /// initialized using <paramref name="initializer"/>.</summary>
      public FlatBoardStorage(Size sizeHexes, Func<HexCoords,T> initializer) 
        : base (sizeHexes) {
        if (initializer==null) throw new ArgumentNullException("initializer");
        backingStore  = new List<List<T>>(MapSizeHexes.Height);
  #if DEBUG
        for(var y=0; y< sizeHexes.Height; y++) {
          backingStore.Add(new List<T>(MapSizeHexes.Width));
          for(var x=0; x<MapSizeHexes.Width; x++) {
            backingStore[y].Add(initializer(HexCoords.NewUserCoords(x,y)));
          }
        }
  #else
        for(var y = 0;  y < backingStore.Capacity;  y++) {
          backingStore.Add(new List<T>(MapSizeHexes.Width));
        }
        Parallel.For(0, backingStore.Capacity, y => {
          var boardRow    = backingStore[y];
          for(var x = 0;  x < boardRow.Capacity;  x++) {
            boardRow.Add(initializer(HexCoords.NewUserCoords(x,y)));
          }
        } );
  #endif

      }

      /// <inheritdoc/>>
      public override T this[HexCoords coords] { 
        get { 
          return IsOnboard(coords) ? backingStore[coords.User.Y][coords.User.X] 
                                   : default(T);
        }
        internal set {
          if (IsOnboard(coords))
            backingStore [coords.User.Y][coords.User.X] = value;
        }
      }

      /// <inheritdoc/>
      public override void ForEach(Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        foreach(var hex in backingStore.SelectMany(lh=>lh)) action(hex);
      }

      /// <inheritdoc/>
      public override void ForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        foreach(var hex in backingStore.SelectMany(lh=>lh).Where(lh=>predicate(lh))) action(hex);
      }

      /// <inheritdoc/>
      public override void ParallelForEach(Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        Parallel.ForEach<T>(backingStore.SelectMany(lh=>lh), hex=>action(hex));
      }
      /// <inheritdoc/>
      public override void ParallelForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        Parallel.ForEach<T>(backingStore.SelectMany(lh=>lh).Where(lh=>predicate(lh)), hex=>action(hex));
      }

      private List<List<T>> backingStore { get; set; }
    }

    /// <summary>A <c>BoardStorage</c> implementation optimized for large maps by blocking 
    /// 32 x 32 arrays of hexes for improved memory caching.</summary>
    /// <remarks>This <c>BoardStorage</c> implementation stores the board cells in blocks
    /// that are 32 x 32 cells to provide better localization for the Path-Finding and
    /// Field-of-View algorithms.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1034:NestedTypesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "x")]
    public sealed class BlockedBoardStorage32x32 : BoardStorage<T> {
      const int _grouping = 32;
      const int _buffer   = _grouping - 1;

      /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
      /// initialized using <paramref name="initializer"/>.</summary>
      public BlockedBoardStorage32x32(Size sizeHexes, Func<HexCoords,T> initializer) 
        : base (sizeHexes) {
        backingStore  = new List<List<List<T>>>((MapSizeHexes.Height+_buffer) / _grouping);
        for(var y = 0;  y < backingStore.Capacity;  y++) {
          backingStore.Add(new List<List<T>>((MapSizeHexes.Width+_buffer) / _grouping));
          for(var x = 0; x < backingStore[y].Capacity; x++) {
            backingStore[y].Add(new List<T>(_grouping*_grouping));
          }
        }

        Parallel.For(0, backingStore.Capacity, y => {
          var boardRow    = backingStore[y];
          for(var x = 0;  x < boardRow.Capacity;  x++) {
            var boardCell = backingStore[y][x];
            for (var i=0; i<_grouping; i++) {
              for (var j=0; j<_grouping; j++) {
                var coords = HexCoords.NewUserCoords(x*_grouping+j,y*_grouping+i);
                boardCell.Add(IsOnboard(coords) ? initializer(coords) : default(T));
              }
            }
          }
        } );
      }

      /// <inheritdoc/>>
      public override T this[HexCoords coords] { 
        get { 
          var v = coords.User;
          return IsOnboard(coords) 
            ? backingStore [v.Y/_grouping]
                           [v.X/_grouping]
                           [(v.Y % _grouping) * _grouping + v.X % _grouping]
            : default(T);
        }
        internal set {
          var v = coords.User;
          backingStore [v.Y/_grouping]
                       [v.X/_grouping]
                       [(v.Y % _grouping) * _grouping + v.X % _grouping] = value;
        }
      }

      /// <inheritdoc/>
      public override void ForEach(Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        foreach(var hex in backingStore.SelectMany(lh=>lh).SelectMany(lh=>lh)) action(hex);
      }

      /// <inheritdoc/>>
      public override void ForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        foreach(var hex in backingStore.SelectMany(lh=>lh).SelectMany(lh=>lh)
                                       .Where(lh=>predicate(lh))) action(hex);
      }

      /// <inheritdoc/>>
      public override void ParallelForEach(Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        Parallel.ForEach<T>(
          backingStore.SelectMany(lh=>lh).SelectMany(lh=>lh).Where(h=>h!=null), 
          hex => action(hex)
        );
      }

      /// <inheritdoc/>>
      public override void ParallelForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        Parallel.ForEach<T>(
          backingStore.SelectMany(lh=>lh).SelectMany(lh=>lh).Where(h=>h!=null && predicate(h)), 
          hex => action(hex)
        );
      }

      List<List<List<T>>> backingStore { get; set; }
    }
  }
}
