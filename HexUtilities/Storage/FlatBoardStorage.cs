#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexSize     = System.Drawing.Size;

    /// <summary>A row-major <c>BoardStorage</c> implementation optimized for small maps, 
    /// providing both serial and parallel initialization.</summary>
    public sealed class FlatBoardStorage<T> : BoardStorage<T> {
        /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
        /// initialized using <paramref name="factory"/>.</summary>
        /// <param name="sizeHexes"><c>Size</c> structure speccifying the Width and Height of 
        /// the desired board storage.</param>
        /// <param name="factory"></param>
        /// <param name="inParallel">Boolean indicating how the board should be initialized: 
        /// in parallel or serially.</param>
        public FlatBoardStorage(HexSize sizeHexes, Func<HexCoords,T> factory, bool inParallel) 
        : base (sizeHexes) {
          var rowRange = inParallel ? ParallelEnumerable.Range(0,sizeHexes.Height).AsOrdered()
                                    : Enumerable.Range(0,sizeHexes.Height);
          BackingStore = InitializeStoreX(sizeHexes, factory, rowRange);
        }

        private static IFastList<IFastListX<T>> InitializeStoreX(HexSize sizeHexes,
                Func<HexCoords, T> tFactory, IEnumerable<int> rowRange)
        => ( from y in rowRange
             select ( from x in Enumerable.Range(0, sizeHexes.Width)
                      select tFactory(HexCoords.NewUserCoords(x,y))
                    ).ToArray().ToFastListX()
           ).ToArray().ToFastList();

        /// <inheritdoc/>>
        protected override T ItemInner(int x, int y) => BackingStore[y][x];

        /// <inheritdoc/>>
        public override void ForEach(Action<T> action)
        =>  BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                        .ForAll(row => row.ForEach(action));
        
        /// <inheritdoc/>>
        public override void ForEach(FastIteratorFunctor<T> functor) 
        =>  BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                        .ForAll(row => row.ForEach(functor));

        /// <inheritdoc/>>
        public override void ForEachSerial(Action<T> action)
        =>  BackingStore.ForEach(row => row.ForEach(action));
       
        /// <inheritdoc/>>
        public override void ForEachSerial(FastIteratorFunctor<T> functor)
        =>  BackingStore.ForEach(row => row.ForEach(functor));

        /// <summary>TODO</summary>
        /// <remarks>Use carefully - can interfere with iterators.</remarks>
        protected override void SetItemInner(int x,int y,T value) 
        =>  BackingStore[y].SetItem(x, value);

        private IFastList<IFastListX<T>> BackingStore { get; }
    }
}
