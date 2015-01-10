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
using System.Linq;

namespace PGNapoleonics.HexUtilities {
  /// <summary>Flags for combinations of the six hexagonal directions.</summary>
  [Flags]public enum Hexsides {
    /// <summary>The selection of no hexsides.</summary>
    None      = 0x00,
    /// <summary>The hexside on the top of the hex.</summary>
    North     = 1 << Hexside.North,
    /// <summary>The hexside on the upper-right of the hex.</summary>
    Northeast = 1 << Hexside.Northeast,
    /// <summary>The hexside on the lower-right of the hex</summary>
    Southeast = 1 << Hexside.Southeast,
    /// <summary>The hexside on the bottom of the hex.</summary>
    South     = 1 << Hexside.South,
    /// <summary>The hexside on the lower-left of the hex.</summary>
    Southwest = 1 << Hexside.Southwest,
    /// <summary>The hexside on the upper-left of the hex.</summary>
    Northwest = 1 << Hexside.Northwest
  }

  /// <summary>TODO</summary>
  public static partial class HexsidesExtensions {
    /// <summary>Tests (without boxing) if all flags are clear.</summary>
    public static bool AreAllClear(this Hexsides @this, Hexsides testBits) {
      if (@this == 0) throw new ArgumentOutOfRangeException("this");//,"Value must not be 0.");
      return (@this & testBits) == 0;
    }
    /// <summary>Tests (without boxing) if all flags are set.</summary>
    public static bool AreAllSet(this Hexsides @this, Hexsides testBits) {
      if (@this == 0) throw new ArgumentOutOfRangeException("this");//"Value must not be 0.");
      return (@this & testBits) == testBits;
    }
    /// <summary>Tests (without boxing) if all flags are clear.</summary>
    public static bool IsAnySet(this Hexsides @this, Hexsides testBits) {
      if (@this == 0) throw new ArgumentOutOfRangeException("this");//"Value must not be 0.");
      return (@this & testBits) != 0;
    }
    /// <summary>Clears the bits clearBits in this.</summary>
    public static Hexsides ClearBits(this Hexsides @this, Hexsides bits) {
      return @this & ~bits;
    }
    /// <summary>Sets the bits setBits in this.</summary>
    public static Hexsides SetBits(this Hexsides @this, Hexsides bits) {
      return @this | bits;
    }
    /// <summary>TODO</summary>
    public static Hexsides ValidBitsMask(this Hexsides @this) {
      const Hexsides HexsidesMask = Hexsides.North | Hexsides.Northeast | Hexsides.Southeast
                                  | Hexsides.South | Hexsides.Southwest | Hexsides.Northwest;
      return @this &= HexsidesMask;
    }
    /// <summary>Performs action for all bits set in this.</summary>
    public static void ForEach(this Hexsides @this, Action<Hexsides> action) {
      if (action == null) throw new ArgumentNullException("action");
      for (UInt32 bit = 1; bit != 0; bit <<= 1) {
        var flag = (Hexsides) bit;
        if (@this.IsAnySet(flag)) action(flag);
      }
    }

    private static readonly int[] LookupTable = 
      Enumerable.Range(0,256).Select(CountBits).ToArray();

    private static int CountBits(int value) {
      int count = 0;
      for (int i=0; i < 8; i++) { count += (value >> i) & 1; }
      return count;
    }

    /// <summary>Returns the count of of set bit-flags in the argument.</summary>
    /// <param name="this"></param>
    public static int BitCount(this Hexsides @this) { return LookupTable[(int)@this]; }
  }
}
