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

namespace PGNapoleonics.HexUtilities.FieldOfView {
  /// <summary>Field-of-View cone for shadow-casting implementation.</summary>
  [DebuggerDisplay("{RiseRun} at {Range}; VTop: {VectorTop}; VBot: {VectorBottom}")]
  public struct FovCone : IEquatable<FovCone> {
    #region Constructors
    /// <summary>Construct a new FovCone instance.</summary>
    internal FovCone(int range, IntVector2D top, IntVector2D bottom, RiseRun riseRun) : this() {
      this.Range        = range;
      this.RiseRun      = riseRun;
      this.VectorTop    = top;
      this.VectorBottom = bottom;
    }
    #endregion

    #region Properties
    /// <summary>The distance from the observer at which this FovCone was generated.</summary>
    public int          Range        { get; private set; }
    /// <summary>The pitch angle at which visibility begisn for this FovCone. </summary>
    public RiseRun      RiseRun      { get; private set; }
    /// <summary>The maximum yaw angle for this FovCone.</summary>
    public IntVector2D  VectorBottom { get; private set; }
    /// <summary>The minimum yaw angle for this FovCone.</summary>
    public IntVector2D  VectorTop    { get; private set; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() {
      return string.Format(CultureInfo.InvariantCulture,
        "Y={0}, TopVector={1}, BottomVector={2}, RiseRun={3}",
                                  Range, VectorTop, VectorBottom, RiseRun);
    }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as FovCone?;
      return other.HasValue  &&  this == other.Value;
    }
    /// <inheritdoc/>
    public override int GetHashCode() {
      return VectorTop.GetHashCode() ^ Range.GetHashCode() 
           ^ RiseRun.GetHashCode()   ^ VectorBottom.GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(FovCone other) { return this == other; }

    /// <summary>Tests value-inequality of two <c>FovCone</c> instances.</summary>
    public static bool operator != (FovCone @this, FovCone rhs) { return ! ( @this == rhs); }

    /// <summary>Tests value-equality of two <c>FovCone</c> instances.</summary>
    public static bool operator == (FovCone @this, FovCone rhs) {
      return @this.Range        == rhs.Range  
         &&  @this.RiseRun      == rhs.RiseRun
         &&  @this.VectorTop    == rhs.VectorTop 
         &&  @this.VectorBottom == rhs.VectorBottom;
    }
    #endregion
  }
}
