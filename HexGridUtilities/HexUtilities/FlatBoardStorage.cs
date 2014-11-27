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

namespace PGNapoleonics.HexUtilities {
  using HexSize     = System.Drawing.Size;

  /// <summary>A row-major <c>BoardStorage</c> implementation optimized for small maps, 
    /// providing both serial and parallel initialization.</summary>
    public sealed class FlatBoardStorage<T> : BoardStorage<T> {
      /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
      /// initialized using <paramref name="initializer"/>.</summary>
      /// <param name="sizeHexes"><c>Size</c> structure speccifying the Width and Height of 
      /// the desired board storage.</param>
      /// <param name="initializer"></param>
      /// <remarks>
      /// If the conditional compilation constant DEBUG is defined then the board
      /// storage is initialized serially; otherwise it is initialized in parallel.
      /// </remarks>
      public FlatBoardStorage(HexSize sizeHexes, Func<HexCoords,T> initializer)
        #if DEBUG
         : this (sizeHexes,initializer,false) {
        #else
         : this (sizeHexes,initializer,true) {
        #endif
      }

      /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
      /// initialized using <paramref name="initializer"/>.</summary>
      /// <param name="sizeHexes"><c>Size</c> structure speccifying the Width and Height of 
      /// the desired board storage.</param>
      /// <param name="initializer"></param>
      /// <param name="inParallel">Boolean indicating how the board should be initialized: 
      /// in parallel or serially.</param>
      public FlatBoardStorage(HexSize sizeHexes, Func<HexCoords,T> initializer, bool inParallel) 
        : base (sizeHexes
      ) {
        if (initializer==null) throw new ArgumentNullException("initializer");

        backingStore  = new List<List<T>>(MapSizeHexes.Height);

        if (inParallel) {
            for(var y = 0;  y < backingStore.Capacity;  y++) {
              backingStore.Add(new List<T>(MapSizeHexes.Width));
            }
            Parallel.For(0, backingStore.Capacity, y => {
              var boardRow    = backingStore[y];
              for(var x = 0;  x < boardRow.Capacity;  x++) {
                boardRow.Add(initializer(HexCoords.NewUserCoords(x,y)));
              }
            } );
        } else {
            for(var y=0; y< sizeHexes.Height; y++) {
              backingStore.Add(new List<T>(MapSizeHexes.Width));
              for(var x=0; x<MapSizeHexes.Width; x++) {
                backingStore[y].Add(initializer(HexCoords.NewUserCoords(x,y)));
              }
            }
        }
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

        foreach(var hex in backingStore.SelectMany(lh => lh.ToList()))
          action(hex);
      }

      /// <inheritdoc/>
      public override void ForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");

        foreach(var hex in backingStore.SelectMany(lh => lh.ToList())
                                       .Where(lh => predicate(lh)))
          action(hex);
      }

      /// <inheritdoc/>
      public override ParallelLoopResult ParallelForEach(Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");

        return Parallel.ForEach<T>(backingStore.SelectMany(lh => lh.ToList()),
          hex=>action(hex));
      }

      /// <inheritdoc/>
      public override ParallelLoopResult ParallelForEach(Func<T,bool> predicate, Action<T> action) {
        if (action==null) throw new ArgumentNullException("action");

        return Parallel.ForEach<T>(backingStore.SelectMany(lh => lh.ToList())
                                               .Where(lh => predicate(lh)),
          hex=>action(hex));
      }

      private List<List<T>> backingStore { get; set; }
    }
}
