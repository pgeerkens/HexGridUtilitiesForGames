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

namespace PGNapoleonics.HexUtilities.FieldOfView {
  /// <summary>TODO</summary>
  [DebuggerDisplay("RiseRun: ({Rise} over {Run})")]
  public struct RiseRun : IEquatable<RiseRun>, IComparable<RiseRun> {
    #region Constructors
    /// <summary>Creates a new instance of the RiseRUn struct.</summary>
    /// <param name="rise"></param>
    /// <param name="run"></param>
    internal RiseRun(int rise, int run) : this() {
      this.Rise = rise;
      this.Run  = run;
    }
    #endregion

    #region Properties
    /// <summary>Delta-height in units of elevation: meters.</summary>
    public int Rise { get; private set; }
    /// <summary>Delta-width in units of distance:  hexes.</summary>
    public int Run  { get; private set; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() { return string.Format(CultureInfo.InvariantCulture,"Rise={0}; Run={1}", Rise, Run); }

    #region Value equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as RiseRun?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Rise / Run; }

    /// <inheritdoc/>
    public bool Equals(RiseRun other) { return this == other; }

    #region Operators and  IComparable<RiseRun> implementations: 
    /// <summary>Test Value-Inequality.</summary>
    public static bool operator != (RiseRun lhs, RiseRun rhs) { return lhs.CompareTo(rhs) != 0; }
    /// <summary>Test Value-Equality.</summary>
    public static bool operator == (RiseRun lhs, RiseRun rhs) { return lhs.CompareTo(rhs) == 0; }

    /// <summary>Less Than operator</summary>
    public static bool operator <  (RiseRun lhs, RiseRun rhs) { return lhs.CompareTo(rhs) <  0; }
    /// <summary>Less Than or Equals operator</summary>
    public static bool operator <= (RiseRun lhs, RiseRun rhs) { return lhs.CompareTo(rhs) <= 0; }
    /// <summary>Greater Thanoperator</summary>
    public static bool operator >  (RiseRun lhs, RiseRun rhs) { return lhs.CompareTo(rhs) >  0; }
    /// <summary>Greater Than or Equals operator</summary>
    public static bool operator >= (RiseRun lhs, RiseRun rhs) { return lhs.CompareTo(rhs) >= 0; }
    /// <summary>Less-Than comparaator.</summary>
    public int CompareTo(RiseRun other) { 
      return (this.Rise * other.Run).CompareTo(other.Rise * this.Run);
    }
    #endregion
    #endregion
  }
}
