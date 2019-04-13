#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;

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
        protected abstract int BlockExponent { get; }
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
        private IFastListX<T> InitializeBlock(Func<HexCoords,T> tFactory, int block_j, int block_i)
        => ( from j in Enumerable.Range(0, BlockSide)
             from i in Enumerable.Range(0, BlockSide)
             let coords = HexCoords.NewUserCoords(block_i + i, block_j + j)
             select MapSizeHexes.IsOnboard(coords) ? tFactory(coords) : default
           ).ToArray().ToFastListX();

        /// <inheritdoc/>>
        public override void ForEach(Action<T> action)
        => BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                       .ForAll(row => row.ForEach(lh => lh.ForEach(action)));

        /// <inheritdoc/>>
        public override void ForEach(FastIteratorFunctor<T> functor)
        => BackingStore.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                       .ForAll(row => row.ForEach(lh => lh.ForEach(functor)));

        /// <summary>TOTO</summary>
        public override void ForEachSerial(Action<T> action)
        => BackingStore.ForEach(row => row.ForEach(lh => lh.ForEach(action)));

        /// <summary>TOTO</summary>
        public override void ForEachSerial(FastIteratorFunctor<T> functor)
        => BackingStore.ForEach(row => row.ForEach(lh => lh.ForEach(functor)));

        protected IFastList<IFastList<IFastListX<T>>> BackingStore => _backingStore;
        protected readonly IFastList<IFastList<IFastListX<T>>> _backingStore;
    }
}
