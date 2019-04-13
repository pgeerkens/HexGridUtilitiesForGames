#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexSize     = System.Drawing.Size;

    /// <summary>TODO</summary>
    public static class BlockedBoardStorage {
        private static int DefaultProcessorsToUse { get {
            var processors = Environment.ProcessorCount;
            #if DEBUG
                return 1;
            #else
                return processors > 3 ? processors - 2 : 1;
            #endif
        } }

        /// <summary>TODO</summary>
        public static BoardStorage<T> New32x32<T>(HexSize sizeHexes, Func<HexCoords, T> factory)
        =>  new BlockedBoardStorage32x32<T>(sizeHexes, factory, DefaultProcessorsToUse);
    }

    /// <summary>A <c>BoardStorage</c> implementation optimized for large maps by blocking 
    /// 32 x 32 arrays of hexes for improved memory caching.</summary>
    /// <remarks>This <c>BoardStorage</c> implementation stores the board cells in blocks
    /// that are 32 i 32 cells to provide better localization for the Path-Finding and
    /// Field-of-View algorithms.</remarks>
    internal sealed class BlockedBoardStorage32x32<T> : BlockedBoardStorage<T> {
        const int _blockExponent = 5;
        const int _blockSide     = 1 << _blockExponent;
        const int _blockMask     = _blockSide - 1;

        /// <summary>TODO</summary>
        protected override int BlockExponent => _blockExponent;
        /// <inheritdoc/>
        protected override int BlockMask => _blockMask;
        /// <inheritdoc/>
        protected override int BlockSide => _blockSide;

        /// <summary>Construct a new instance of extent <paramref name="sizeHexes"/> and 
        /// initialized using <paramref name="tFactory"/>.</summary>
        internal BlockedBoardStorage32x32(HexSize sizeHexes, Func<HexCoords,T> tFactory, int threadCount = 1) 
        : base (sizeHexes, tFactory, threadCount) {
        }

        /// <inheritdoc/>>
        protected override T ItemInner(int x, int y) {
        #if DIVIDE
            var index = (y & _blockMask) * _blockSide  +  (x & _blockMask);
            var block = BackingStore[y/_blockSide][x/_blockSide];
        #else
            var index = (y & _blockMask) * _blockSide  +  (x & _blockMask);
            var block = BackingStore[y >> _blockExponent][x >> _blockExponent];
        #endif
        return block[index];
        }

        /// <inheritdoc/>>
        protected override void SetItemInner(int x, int y, T value) {
        #if DIVIDE
            var index = (y & _blockMask) * _blockSide  +  (x & _blockMask);
            var block = BackingStore[y/_blockSide][x/_blockSide];
        #else
            var index = (y & _blockMask) * _blockSide  +  (x & _blockMask);
            var block = BackingStore[y >> _blockExponent][x >> _blockExponent];
        #endif
            block.SetItem(index, value);
        }
    }
}
