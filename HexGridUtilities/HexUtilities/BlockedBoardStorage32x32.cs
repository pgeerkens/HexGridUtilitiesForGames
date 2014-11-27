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
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities {
    using HexSize     = System.Drawing.Size;

    /// <summary>A <c>BoardStorage</c> implementation optimized for large maps by blocking 
    /// 32 i 32 arrays of hexes for improved memory caching.</summary>
    /// <remarks>This <c>BoardStorage</c> implementation stores the board cells in blocks
    /// that are 32 i 32 cells to provide better localization for the Path-Finding and
    /// Field-of-View algorithms.</remarks>
    public sealed class BlockedBoardStorage32x32<T> : BoardStorage<T> {
      const int _grouping = 32;
      const int _buffer   = _grouping - 1;

      /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
      /// initialized using <paramref name="initializer"/>.</summary>
      public BlockedBoardStorage32x32(HexSize sizeHexes, Func<HexCoords,T> initializer) 
        : base (sizeHexes) {
        backingStore  = new List<List<List<T>>>((MapSizeHexes.Height+_buffer) / _grouping);
        for(var y = 0;  y < backingStore.Capacity;  y++) {
          backingStore.Add(new List<List<T>>((MapSizeHexes.Width+_buffer) / _grouping));
          for(var x = 0; x < backingStore[y].Capacity; x++) {
            backingStore[y].Add(new List<T>(_grouping*_grouping));
          }
        }

        var result = Parallel.For(0, backingStore.Capacity, y => {
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
        foreach(var hex in backingStore.SelectMany(lllh=>lllh.ToList())
                                       .SelectMany(llh =>llh.ToList())
                                       .Where(h=>h!=null)) 
          action(hex);
      }

      /// <inheritdoc/>>
      public override void ForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        foreach(var hex in backingStore.SelectMany(lllh=>lllh.ToList())
                                       .SelectMany(llh =>llh.ToList())
                                       .Where(h=>h!=null && predicate(h))) 
          action(hex);
      }

      /// <inheritdoc/>>
      public override ParallelLoopResult ParallelForEach(Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        return Parallel.ForEach<T>(
          backingStore.SelectMany(lllh=>lllh.ToList())
                      .SelectMany(llh =>llh.ToList())
                      .Where(h=>h!=null), 
          hex => action(hex)
        );
      }

      /// <inheritdoc/>>
      public override ParallelLoopResult ParallelForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");
        return Parallel.ForEach<T>(
          backingStore.SelectMany(lllh=>lllh.ToList())
                      .SelectMany(llh =>llh.ToList())
                      .Where(h=>h!=null && predicate(h)), 
          hex => action(hex)
        );
      }

      private List<List<List<T>>> backingStore { get; set; }
    }
}
