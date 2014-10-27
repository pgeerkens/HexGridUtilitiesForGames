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

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  /// <summary>TODO</summary>
  [DebuggerDisplay("{Coords} at {Hexside}")]
  public struct NeighbourCoords : IEquatable<NeighbourCoords> {
    #region Constructors
    /// <summary>TODO</summary>
    public NeighbourCoords(HexCoords coords, Hexside hexside) : this() {
      Coords = coords; Hexside = hexside;
    }
    #endregion

    #region Properties
    /// <summary>TODO</summary>
    public Hexside   Hexside   { get; private set; }
    /// <summary>TODO</summary>
    public HexCoords Coords    { get; private set; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() { 
      return string.Format(CultureInfo.InvariantCulture,"Neighbour: {0} at {1}", Coords.User,Hexside);
    }

    /// <summary>TODO</summary>
    public static Func<NeighbourCoords,T> Bind<T>(Func<HexCoords,T> f) {
      return n => f(n.Coords);
    }

    #region Value Equality - on Coords field only
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as NeighbourCoords?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int  GetHashCode() { return Coords.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(NeighbourCoords other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (NeighbourCoords lhs, NeighbourCoords rhs) { return ! (lhs == rhs); }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (NeighbourCoords lhs, NeighbourCoords rhs) { 
      return lhs.Coords == rhs.Coords; 
    }
    #endregion
  }
}
