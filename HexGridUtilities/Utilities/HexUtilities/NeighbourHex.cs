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
                .Select((nn,seq)=>new NeighbourHex(hex.Board[nn.Coords.User], nn.Direction, seq))
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
