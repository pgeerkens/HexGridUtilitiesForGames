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
  [Flags] public enum Hexside {
    None      = 0x00,
    North     = 0x01,
    NorthEast = 0x02,
    SouthEast = 0x04,
    South     = 0x08,
    SouthWest = 0x10,
    NorthWest = 0x20
  }
  public enum HexsideIndex {
    North,    NorthEast,    SouthEast,    South,    SouthWest,    NorthWest
  }
  public static partial class HexExtensions {
    public static readonly List<HexsideIndex> HexsideIndexList 
      = Utils.EnumGetValues<HexsideIndex>().ToList();
      
    public static readonly List<Hexside> HexsideList = new List<Hexside>() { 
      Hexside.North,  Hexside.NorthEast,  Hexside.SouthEast, 
      Hexside.South,  Hexside.SouthWest,  Hexside.NorthWest 
    };

    /// <summary>Reverses a given Hexside direction.</summary>
    public static Hexside Reversed(this Hexside hexside) {
      if (hexside.Equals(Hexside.None)) return Hexside.None;
      var indexReversed = 3 + (int)hexside.IndexOf();
      if (indexReversed > 5) indexReversed -= 6;
      return HexsideList[indexReversed];
    }

    public static HexsideIndex IndexOf(this Hexside @this) {
      return (HexsideIndex)HexsideList.IndexOf(@this);
    }

    public static Hexside Direction(this HexsideIndex @this) {
      return HexsideList[(int)@this];
    }
    public static HexsideIndex Reversed(this HexsideIndex @this) {
      var reversed = @this+3;
      return (reversed <= HexsideIndex.NorthWest) ? reversed : (reversed - 6);
    }
  }

  public struct NeighbourCoords : IEquatable<NeighbourCoords> {
    public Hexside Direction { get; private set; }
    public ICoords Coords    { get; private set; }

    public NeighbourCoords(Hexside direction, ICoords coords) : this() {
      Direction = direction; Coords = coords;
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
