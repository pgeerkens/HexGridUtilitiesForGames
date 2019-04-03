#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities.Storage {
  using HexSize     = System.Drawing.Size;

  /// <summary>A <c>BoardStorage</c> implementation optimized for large maps by blocking 
  /// 32 i 32 arrays of hexes for improved memory caching.</summary>
  /// <remarks>This <c>BoardStorage</c> implementation stores the board cells in blocks
  /// that are 32 i 32 cells to provide better localization for the Path-Finding and
  /// Field-of-View algorithms.</remarks>
  internal abstract class BlockedBoardStorage<T> : BoardStorage<T> {
    /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
    /// initialized using <paramref name="tFactory"/>.</summary>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
      Justification="This is safe because all sub-classes are SEALED INTERNAL and both properties (BlockMask & BlockSide) are just aliases for constants.")]
    internal BlockedBoardStorage(HexSize sizeHexes, Func<HexCoords,T> tFactory, int threadCount) 
    : base (sizeHexes) {

      var rangeY    = ( MapSizeHexes.Height + BlockMask ) / BlockSide;
      var rangeX    = ( MapSizeHexes.Width + BlockMask  ) / BlockSide;
      _backingStore = InitializeStore(tFactory, threadCount, rangeY, rangeX);
    }

    /// <summary>TODO</summary>
    protected abstract int BlockMask { get; }
    /// <summary>TODO</summary>
    protected abstract int BlockSide { get; }

    internal IFastList<IFastList<IFastListX<T>>> InitializeStore(
      Func<HexCoords,T> tFactory, int threadCount, int rangeY, int rangeX
    ) {
      if (threadCount < 1) threadCount = 1;
      var threadRange = ( rangeY + (threadCount-1) ) / threadCount;
      return ( from thread in ParallelEnumerable.Range(0, threadCount).AsOrdered()
               from i in Enumerable.Range(0,threadRange)
               let y = thread*threadRange + i
               where y < rangeY
               let block_j = y * BlockSide
               select ( from x in Enumerable.Range(0, rangeX)
                 select InitializeBlock(tFactory, block_j, x * BlockSide)
               ).ToArray().ToFastList()
             ).ToArray().ToFastList();
    }
    private IFastListX<T> InitializeBlock(Func<HexCoords,T> tFactory, int block_j, int block_i) {
      return ( from j in Enumerable.Range(0, BlockSide)
               from i in Enumerable.Range(0, BlockSide)
               let coords = HexCoords.NewUserCoords(block_i + i, block_j + j)
               select MapSizeHexes.IsOnboard(coords) ? tFactory(coords) : default(T)
             ).ToArray().ToFastListX();
    }

    /// <inheritdoc/>>
    public override void ForEach(Action<T> action) {
      BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                  .ForAll(row => row.ForEach(lh => lh.ForEach(action)));
    }
    /// <inheritdoc/>>
    public override void ForEach(FastIteratorFunctor<T> functor) {
      BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                  .ForAll(row => row.ForEach(lh => lh.ForEach(functor)));
    }

    /// <summary>TOTO</summary>
    public override void ForEachSerial(Action<T> action) {
      BackingStore.ForEach(row => row.ForEach(lh => lh.ForEach(action)));
    }
    /// <summary>TOTO</summary>
    public override void ForEachSerial(FastIteratorFunctor<T> functor) {
      BackingStore.ForEach(row => row.ForEach(lh => lh.ForEach(functor)));
    }

    protected IFastList<IFastList<IFastListX<T>>> BackingStore { get { return _backingStore; } }
    private readonly IFastList<IFastList<IFastListX<T>>> _backingStore;
  }
}
