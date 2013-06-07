#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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
using System.Text;

using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities {
  /// <summary>Enumeration of the six hexagonal directions.</summary>
  public enum Hexside {
    North,    NorthEast,    SouthEast,    South,    SouthWest,    NorthWest
  }

  /// <summary>Flags for combinations of the six hexagonal directions.</summary>
  [Flags] public enum HexsideFlags {
    None      = 0x00,
    North     = 1 << Hexside.North,
    NorthEast = 1 << Hexside.NorthEast,
    SouthEast = 1 << Hexside.SouthEast,
    South     = 1 << Hexside.South,
    SouthWest = 1 << Hexside.SouthWest,
    NorthWest = 1 << Hexside.NorthWest,
  }

  /// <summary>Common <i>extension methods</i> for <c>Hexside</c> and <c>HexSideFlags</c>.</summary>
  public static partial class HexExtensions {

    /// <summary><c>Static List&lt;Hexside></c> for enumerations.</summary>
    public static readonly List<Hexside> HexsideList 
      = Utils.EnumGetValues<Hexside>().ToList();
      
    /// <summary>Static List&lt;HexSideFlags> for enumerations.</summary>
    public static readonly List<HexsideFlags> HexsideFlagsList =
      HexsideList.Select(h=>Utils.ParseEnum<HexsideFlags>(h.ToString())).ToList();

    /// <summary>The <c>Hexside</c> corresponding to this <c>HexsideFlag</c>, or -1 if it doesn't exist.</summary>
    public static Hexside IndexOf(this HexsideFlags @this) {
      return (Hexside)HexsideFlagsList.IndexOf(@this);
    }

    /// <summary>The <c>HexsideFlag</c> corresponding to this <c>HexSide</c>.</summary>
    public static HexsideFlags Direction(this Hexside @this) {
      return HexsideFlagsList[(int)@this];
    }

    /// <summary>Returns the reversed, or opposite, <c>Hexside</c> to the supplied value.</summary>
    /// <param name="this"></param>
    public static Hexside Reversed(this Hexside @this) {
      var reversed = @this+3;
      return (reversed <= Hexside.NorthWest) ? reversed : (reversed - 6);
    }
  }
}
