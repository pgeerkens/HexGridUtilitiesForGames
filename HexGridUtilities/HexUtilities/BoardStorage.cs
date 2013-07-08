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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PGNapoleonics;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.PathFinding;

namespace PGNapoleonics.HexUtilities {
  /// <summary>The abstract <c>BoardStorage</c> implementation required by <c>HexBoard</c>.</summary>
  /// <typeparam name="THex">The type of the hex being stored.</typeparam>
  public abstract class BoardStorage<THex> : IDisposable {
    protected BoardStorage(Size sizeHexes) {
      MapSizeHexes   = sizeHexes;
    }

    public          Size MapSizeHexes           { get; private set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1043:UseIntegralOrStringArgumentForIndexers")]
    public abstract THex this[HexCoords coords] { get; internal set; }

    public          bool IsOnboard(HexCoords coords) { 
      return 0<=coords.User.X && coords.User.X < MapSizeHexes.Width
          && 0<=coords.User.Y && coords.User.Y < MapSizeHexes.Height;
    }

    public abstract void ParallelForEach(Action<THex> action);

    #region IDisposable implementation with Finalizeer
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
        }
        _isDisposed = true;
      }
    }
    ~BoardStorage() { Dispose(false); }
    #endregion

    /// <summary>A <c>BoardStorage</c> implementation optimized for small maps.</summary>
    /// <typeparam name="THex">Type of the hex being stored.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1034:NestedTypesShouldNotBeVisible")]
    public sealed class FlatBoardStorage : BoardStorage<THex> {
      public FlatBoardStorage(Size sizeHexes, Func<HexCoords,THex> initializer) 
        : base (sizeHexes) {
        if (initializer==null) throw new ArgumentNullException("initializer");
        backingStore  = new List<List<THex>>(MapSizeHexes.Height);
  #if DEBUG
        for(var y=0; y< sizeHexes.Height; y++) {
          backingStore.Add(new List<THex>(MapSizeHexes.Width));
          for(var x=0; x<MapSizeHexes.Width; x++) {
            backingStore[y].Add(initializer(HexCoords.NewUserCoords(x,y)));
          }
        }
  #else
        for(var y = 0;  y < backingStore.Capacity;  y++) {
          backingStore.Add(new List<THex>(MapSizeHexes.Width));
        }
        Parallel.For(0, backingStore.Capacity, y => {
          var boardRow    = backingStore[y];
          for(var x = 0;  x < boardRow.Capacity;  x++) {
            boardRow.Add(initializer(HexCoords.NewUserCoords(x,y)));
          }
        } );
  #endif

      }

      public override THex this[HexCoords coords] { 
        get { 
          return IsOnboard(coords) ? backingStore[coords.User.Y][coords.User.X] 
                                   : default(THex);
        }
        internal set {
          if (IsOnboard(coords))
            backingStore [coords.User.Y][coords.User.X] = value;
        }
      }

      public override void ParallelForEach(Action<THex> action) {
        Parallel.ForEach<THex>(backingStore.SelectMany(lh=>lh), hex=>action(hex));
      }

      private List<List<THex>> backingStore { get; set; }
    }

    /// <summary>A <c>BoardStorage</c> implementation optimized for large maps.</summary>
    /// <typeparam name="THex">Type of the hex being stored.</typeparam>
    /// <remarks>This <c>BoardStorage</c> implementation stores the board cells in blocks
    /// that are 32 x 32 cells to provide better localization for the Path-Finding and
    /// Field-of-View algorithms.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1034:NestedTypesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "x")]
    public sealed class BlockedBoardStorage32x32 : BoardStorage<THex> {
      const int _grouping = 32;
      const int _buffer   = _grouping - 1;

      public BlockedBoardStorage32x32(Size sizeHexes, Func<HexCoords,THex> initializer) 
        : base (sizeHexes) {
        backingStore  = new List<List<List<THex>>>((MapSizeHexes.Height+_buffer) / _grouping);
        for(var y = 0;  y < backingStore.Capacity;  y++) {
          backingStore.Add(new List<List<THex>>((MapSizeHexes.Width+_buffer) / _grouping));
          for(var x = 0; x < backingStore[y].Capacity; x++) {
            backingStore[y].Add(new List<THex>(_grouping*_grouping));
          }
        }

        Parallel.For(0, backingStore.Capacity, y => {
          var boardRow    = backingStore[y];
          for(var x = 0;  x < boardRow.Capacity;  x++) {
            var boardCell = backingStore[y][x];
            for (var i=0; i<_grouping; i++) {
              for (var j=0; j<_grouping; j++) {
                var coords = HexCoords.NewUserCoords(x*_grouping+j,y*_grouping+i);
                boardCell.Add(IsOnboard(coords) ? initializer(coords) : default(THex));
              }
            }
          }
        } );
      }

      public override THex this[HexCoords coords] { 
        get { 
          var v = coords.User;
          return IsOnboard(coords) 
            ? backingStore [v.Y/_grouping]
                           [v.X/_grouping]
                           [(v.Y % _grouping) * _grouping + v.X % _grouping]
            : default(THex);
        }
        internal set {
          var v = coords.User;
          backingStore [v.Y/_grouping]
                       [v.X/_grouping]
                       [(v.Y % _grouping) * _grouping + v.X % _grouping] = value;
        }
      }

      public override void ParallelForEach(Action<THex> action) {
        Parallel.ForEach<THex>(
          backingStore.SelectMany(lh=>lh).SelectMany(lh=>lh).Where(h=>h!=null), 
          hex => action(hex)
        );
      }

      List<List<List<THex>>> backingStore { get; set; }
    }
  }
}
