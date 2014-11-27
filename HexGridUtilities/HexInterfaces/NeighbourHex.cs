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
using System.Diagnostics;
using System.Globalization;

namespace PGNapoleonics.HexUtilities {
  /// <summary>TODO</summary>
  [DebuggerDisplay("NeighbourHex: {Hex.Coords} enters from {HexsideEntry}")]
  public struct NeighbourHex : IEquatable<NeighbourHex> {
    #region Constructors
    /// <summary>TODO</summary>
    public NeighbourHex(IHex hex) : this(hex, Hexside.North) {}
    /// <summary>TODO</summary>
    public NeighbourHex(IHex hex, Hexside hexsideExit) : this() {
      Hex          = hex;
      HexsideExit  = hexsideExit;
    }
    #endregion

    #region Properties
    /// <summary>The Hex</summary>
    public IHex    Hex          { get; private set; }

    /// <summary>The hexside of the neighbour through which the agent exits from the neighbour.</summary>
    public Hexside HexsideEntry { get {return HexsideExit.Reversed();} }

    /// <summary>The hexside of this hex through which the agent enters from the neighbour.</summary>
    public Hexside HexsideExit  { get; private set; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() { 
      return string.Format(CultureInfo.InvariantCulture,
        "NeighbourHex: {0} enters from {1}", Hex.Coords, HexsideEntry);
    }

    #region Value Equality - on Hex field only
    /// <inheritdoc/>
    public override bool Equals(object obj)                  {
      var other = obj as NeighbourHex?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Hex.Coords.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(NeighbourHex other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (NeighbourHex lhs, NeighbourHex rhs) { return ! (lhs == rhs); }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (NeighbourHex lhs, NeighbourHex rhs) {
      return lhs.Hex.Coords == rhs.Hex.Coords;
    }
    #endregion
  }
}
