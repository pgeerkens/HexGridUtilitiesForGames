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
  public enum Hexside {
    North,    NorthEast,    SouthEast,    South,    SouthWest,    NorthWest
  }
  [Flags] public enum HexsideFlags {
    None      = 0x00,
    North     = 1 << Hexside.North,
    NorthEast = 1 << Hexside.NorthEast,
    SouthEast = 1 << Hexside.SouthEast,
    South     = 1 << Hexside.South,
    SouthWest = 1 << Hexside.SouthWest,
    NorthWest = 1 << Hexside.NorthWest,
  }
  public static partial class HexExtensions {
    public static readonly List<Hexside> HexsideList 
      = Utils.EnumGetValues<Hexside>().ToList();
      
    public static readonly List<HexsideFlags> HexsideFlagsList =
      HexsideList.Select(h=>Utils.ParseEnum<HexsideFlags>(h.ToString())).ToList();

    /// <summary>Reverses a given Hexside direction.</summary>
    public static HexsideFlags Reversed(this HexsideFlags hexside) {
      if (hexside.Equals(HexsideFlags.None)) return HexsideFlags.None;
      var indexReversed = 3 + (int)hexside.IndexOf();
      if (indexReversed > 5) indexReversed -= 6;
      return HexsideFlagsList[indexReversed];
    }

    public static Hexside IndexOf(this HexsideFlags @this) {
      return (Hexside)HexsideFlagsList.IndexOf(@this);
    }

    public static HexsideFlags Direction(this Hexside @this) {
      return HexsideFlagsList[(int)@this];
    }
    public static Hexside Reversed(this Hexside @this) {
      var reversed = @this+3;
      return (reversed <= Hexside.NorthWest) ? reversed : (reversed - 6);
    }
  }

  public struct NeighbourCoords : IEquatable<NeighbourCoords> {
    public HexsideFlags      Direction { get { return Index.Direction();} }
    public Hexside Index     { get; private set; }
    public ICoords      Coords    { get; private set; }

    public NeighbourCoords(ICoords coords, HexsideFlags direction) 
      : this(coords, direction.IndexOf()) {}
    public NeighbourCoords(ICoords coords, Hexside index) : this() {
      Coords = coords; Index = index;
    }
    public override string ToString() { 
      return string.Format("Neighbour: {0} at {1}", Coords.User,Direction);
    }

    #region Value Equality - on Coords field only
    public override bool Equals(object obj) { 
      return obj is NeighbourCoords  &&  this.Coords.Equals(((NeighbourCoords)obj).Coords);
    }
    public override int GetHashCode() { return Coords.GetHashCode(); }

    bool IEquatable<NeighbourCoords>.Equals(NeighbourCoords obj) { return this.Equals(obj); }

    public static bool operator != (NeighbourCoords lhs, NeighbourCoords rhs) { return ! (lhs == rhs); }
    public static bool operator == (NeighbourCoords lhs, NeighbourCoords rhs) { return lhs.Equals(rhs); }
    #endregion
  }
}
