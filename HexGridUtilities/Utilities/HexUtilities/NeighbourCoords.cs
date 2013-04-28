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

namespace PG_Napoleonics.Utilities.HexUtilities {
  [Flags] public enum Hexside {
    None      = 0x00,
    North     = 0x01,
    NorthEast = 0x02,
    SouthEast = 0x04,
    South     = 0x08,
    SouthWest = 0x10,
    NorthWest = 0x20
  }

  public struct NeighbourCoords : IEquatable<NeighbourCoords> {
    public Hexside      Direction { get; private set; }
    public ICoordsCanon Coords    { get; private set; }

    public NeighbourCoords(Hexside direction, ICoordsCanon coords) : this() {
      Direction = direction; Coords = coords;
    }
    public override string ToString() { 
      return string.Format("Neighbour: {0} at {1}", Coords.User,Direction);
    }

    #region Value Equality - on Coords field only
    public override bool Equals(object obj) { 
      return (obj is NeighbourCoords) && this == (NeighbourCoords)obj;
    }
    bool IEquatable<NeighbourCoords>.Equals(NeighbourCoords obj)              { return this == obj; }
    public static bool operator != (NeighbourCoords lhs, NeighbourCoords rhs) { return ! (lhs == rhs); }
    public static bool operator == (NeighbourCoords lhs, NeighbourCoords rhs) {
      return lhs.Direction == rhs.Direction  &&  lhs.Coords == rhs.Coords;
    }
    public override int GetHashCode() { return Coords.User.GetHashCode(); }
    #endregion
  }
}
