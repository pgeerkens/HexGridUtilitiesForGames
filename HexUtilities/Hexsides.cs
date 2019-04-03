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

namespace PGNapoleonics.HexUtilities {
    /// <summary>Bit-flags for combinations of the six hexagonal directions.</summary>
    [Flags]public enum Hexsides {
        /// <summary>The selection of no hexsides.</summary>
        None      = 0x00,
        /// <summary>The hexside on the top of the hex.</summary>
        North     = 1 << 0,
        /// <summary>The hexside on the upper-right of the hex.</summary>
        Northeast = 1 << 1,
        /// <summary>The hexside on the lower-right of the hex</summary>
        Southeast = 1 << 2,
        /// <summary>The hexside on the bottom of the hex.</summary>
        South     = 1 << 3,
        /// <summary>The hexside on the lower-left of the hex.</summary>
        Southwest = 1 << 4,
        /// <summary>The hexside on the upper-left of the hex.</summary>
        Northwest = 1 << 5,
        /// <summary>SHorthand for all hexsides.</summary>
        All       = North | Northeast | Southeast | South | Southwest | Northwest
    }

    /// <summary>Extension methods for <c>Hexsides</c>.</summary>
    public static partial class HexsidesExtensions {
        /// <summary>Returns true exactly if <c>this</c> has the the eponymous bit from <c>hexside</c> set.</summary>
        /// <param name="this">The Hexsides value to test.</param>
        /// <param name="hexside">Specification of the eponymous bit in <c>this</c> to test.</param>
        public static bool IsSet(this Hexsides @this, Hexside hexside)
        => (@this.GetValue() & ((int)hexside.AsHexsides)) != 0;
        
        /// <summary>TODO</summary>
        public static int GetValue(this Hexsides @this) { return (int)@this.ValidBitsMask(); }
        /// <summary>Tests (without boxing) if all requested bits are clear.</summary>
        public static bool AreAllClear(this Hexsides @this, Hexsides testBits) {
          if (@this == 0) throw new ArgumentOutOfRangeException("this","Value must not be 0.");
          return (@this & testBits).ValidBitsMask() == 0;
        }
        /// <summary>Tests (without boxing) if all requested bits are set.</summary>
        public static bool AreAllSet(this Hexsides @this, Hexsides testBits) {
          if (@this == 0) throw new ArgumentOutOfRangeException("this","Value must not be 0.");
          return (@this & testBits).ValidBitsMask() == testBits;
        }
        /// <summary>Tests (without boxing) if any requested bit is set.</summary>
        public static bool IsAnySet(this Hexsides @this, Hexsides testBits) {
          if (@this == 0) throw new ArgumentOutOfRangeException("this","Value must not be 0.");
          return (@this & testBits).ValidBitsMask() != 0;
        }

        /// <summary>Clears specified bits in this.</summary>
        public static Hexsides ClearBits(this Hexsides @this, Hexsides bits)
        => (@this & ~bits).ValidBitsMask();
        
        /// <summary>Sets specified bits in this.</summary>
        public static Hexsides SetBits(this Hexsides @this, Hexsides bits)
        => (@this | bits).ValidBitsMask();

        /// <summary>Returns the result of clearing all invalid bits in <c>this</c>.</summary>
        public static Hexsides ValidBitsMask(this Hexsides @this) { return @this &= Hexsides.All; }

        #region public static int BitCount(this Hexsides @this)
        /// <summary>Returns the count of of set bit-flags in the argument.</summary>
        /// <param name="this">The Hexsides instnace of interest.</param>
        public static int BitCount(this Hexsides @this)
        => BitCountLookup[(int)@this];

        private static readonly IList<int> BitCountLookup =
          ( from value in Enumerable.Range(0, 1 << 6)
            select ( from i in Enumerable.Range(0,8)
                     select (value >> i) & 0x0001
                   ).Sum()
          ).ToList().AsReadOnly();     
        #endregion
    }
}
