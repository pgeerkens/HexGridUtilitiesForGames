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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  /// <summary>Enumeration of the six hexagonal directions.</summary>
  public enum Hexside {
    /// <summary>The hexside on the top of the hex.</summary>
    North,    
    /// <summary>The hexside on the upper-right of the hex.</summary>
    Northeast,    
    /// <summary>The hexside on the lower-right of the hex</summary>
    Southeast,    
    /// <summary>The hexside on the bottom of the hex.</summary>
    South,    
    /// <summary>The hexside on the lower-left of the hex.</summary>
    Southwest,   
    /// <summary>The hexside on the upper-left of the hex.</summary>
    Northwest
  }


  /// <summary>Common <i>extension methods</i> for <c>Hexside</c> and <c>HexSideFlags</c>.</summary>
  public static partial class HexsideExtensions {
    /// <summary><c>Static List {Hexside}</c> for enumerations.</summary>
    public static readonly ReadOnlyCollection<Hexside> HexsideList 
      = EnumExtensions.EnumGetValues<Hexside>().ToList().AsReadOnly();
      
    internal static readonly ReadOnlyCollection<Hexsides> HexsideFlags
      = HexsideList.Select(h=>EnumExtensions.ParseEnum<Hexsides>(h.ToString()))
                   .ToList().AsReadOnly();

    /// <summary>Static List {HexSideFlags} for enumerations.</summary>
    public static readonly IEnumerable<Hexsides> HexsideCollection = HexsideFlags;

    /// <summary>The <c>Hexside</c> corresponding to this <c>HexsideFlag</c>, or -1 if it doesn't exist.</summary>
    public static Hexside IndexOf(this Hexsides @this) {
      return (Hexside)HexsideFlags.IndexOf(@this);
    }

    /// <summary>The <c>HexsideFlag</c> corresponding to this <c>HexSide</c>.</summary>
    public static Hexsides Direction(this Hexside @this) { return HexsideFlags[(int)@this]; }

    /// <summary>Returns the reversed, or opposite, <c>Hexside</c> to the supplied value.</summary>
    /// <param name="this"></param>
    public static Hexside Reversed(this Hexside @this) {
      return (@this <= Hexside.Southeast) ? (@this + 3) : (@this - 3);
    }
  }
}
