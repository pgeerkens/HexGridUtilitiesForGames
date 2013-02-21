#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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
    public Hexside     Direction  { get; private set; }
    public ICoordsUser Coords     { get; private set; }

    public NeighbourCoords(Hexside direction, ICoordsUser coords) : this() {
      Direction = direction; Coords = coords;
    }
    public NeighbourCoords(Hexside direction, ICoordsCanon coords) : this(direction, coords.User) {}
    public override string ToString() { 
      return string.Format("Neighbour: {0} at {1}", Coords,Direction);
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
    public override int GetHashCode() { return Coords.GetHashCode(); }
    #endregion
  }
}
