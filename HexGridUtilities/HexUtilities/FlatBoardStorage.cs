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

namespace PGNapoleonics.HexUtilities {
    /// <summary>A row-major <c>BoardStorage</c> implementation optimized for small maps.</summary>
    public sealed class FlatBoardStorage<T> : BoardStorage<T> {
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
}
