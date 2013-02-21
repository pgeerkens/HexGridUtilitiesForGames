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
  public interface INeighbourHex {
    IGridHex  Hex        { get; }
    Hexside   Direction  { get; }
    int       SequenceNo { get; }
  }
  public struct NeighbourHex : INeighbourHex, IEquatable<NeighbourHex> {
    public IGridHex  Hex        { get; private set; }
    public Hexside   Direction  { get; private set; }
    public int       SequenceNo { get; private set; }
    public NeighbourHex(IGridHex hex, Hexside direction) : this(hex,direction,0) {}
    public NeighbourHex(IGridHex hex, Hexside direction, int seqNo) : this() {
      Hex        = hex;
      Direction  = direction;
      SequenceNo = seqNo;
    }

    public override string ToString() { 
      return string.Format("NeighbourHex: {0} at {1}",Hex.Coords,Direction);
    }

    public static IEnumerable<NeighbourHex> GetNeighbours(IGridHex hex) {
      return hex.Coords.GetNeighbours(~Hexside.None)
                .Select((nn,seq)=>new NeighbourHex(hex.Board[nn.Coords], nn.Direction, seq))
                .Where(n=>n.Hex!=null)
                .Select(nh=>nh);
    }

    #region Value Equality - on Hex field only
    public override bool Equals(object obj)                               {
      return (obj is NeighbourHex)  &&  this == (NeighbourHex)obj; 
    }
    bool IEquatable<NeighbourHex>.Equals(NeighbourHex rhs)                { return this == rhs; }
    public static bool operator != (NeighbourHex @this, NeighbourHex rhs) { return ! (@this == rhs); }
    public static bool operator == (NeighbourHex @this, NeighbourHex rhs) { return @this.Hex == rhs.Hex; }
    public override int GetHashCode()                                     { return Hex.Coords.GetHashCode(); }
    #endregion
  }
}
