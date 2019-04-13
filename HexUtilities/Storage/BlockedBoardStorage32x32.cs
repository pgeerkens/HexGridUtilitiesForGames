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
        internal BlockedBoardStorage32x32(HexSize sizeHexes, Func<HexCoords,T> tFactory, int threadCount) 
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
