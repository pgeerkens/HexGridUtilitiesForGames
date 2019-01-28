#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>A <see cref="DirectedPathCollection"/>Step with a board location and travel direction.</summary>
  [DebuggerDisplay("NeighbourHex: {Hex.Coords} enters from {HexsideEntry}")]
  public struct DirectedPathStepHex : IEquatable<DirectedPathStepHex> {
    #region Constructors
    /// <summary>Creates a new <see cref="DirectedPathStepHex"/> instance at <paramref name="coords"/> exiting through <paramref name="hexsideExit"/>.</summary>
    public DirectedPathStepHex(HexCoords coords, Hexside hexsideExit) : this() {
     // Hex          = hex;
      _coords      = coords;
      _hexsideExit = hexsideExit;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    }
    #endregion

    #region Properties
    /// <summary>The hex of the <see cref="DirectedPathCollection"/>Step.</summary>
    public HexCoords  Coords       { get {return _coords; } } readonly HexCoords _coords;

    ///// <summary>The hex of the <see cref="DirectedPathCollection"/>Step.</summary>
    //public IHex       Hex          { get; private set; }

    /// <summary>The hexside of the neighbour through which the agent enters from this hex.</summary>
    public Hexside    HexsideEntry { get {
      return HexsideExit.Reversed;
    } }

    /// <summary>The hexside of this hex through which the agent exits to the neighbour.</summary>
    public Hexside    HexsideExit  { get { return _hexsideExit;} } readonly Hexside _hexsideExit;
    #endregion

    /// <inheritdoc/>
    public override string ToString() { 
      return string.Format(CultureInfo.InvariantCulture,
        "NeighbourHex: {0} enters from {1}", Coords, HexsideEntry);
    }

    #region Value Equality - on Hex field only
    /// <inheritdoc/>
    public override bool Equals(object obj) {
      var other = obj as DirectedPathStepHex?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Coords.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(DirectedPathStepHex other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (DirectedPathStepHex lhs, DirectedPathStepHex rhs) {
      return ! (lhs == rhs);
    }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (DirectedPathStepHex lhs, DirectedPathStepHex rhs) {
      return lhs.Coords == rhs.Coords;
    }
    #endregion
  }
}
