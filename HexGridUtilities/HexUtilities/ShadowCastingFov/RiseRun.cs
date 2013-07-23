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
using System.Globalization;

namespace PGNapoleonics.HexUtilities.ShadowCasting {
  /// <summary>TODO</summary>
  public struct RiseRun : IEquatable<RiseRun>, IComparable<RiseRun> {

    /// <summary>Creates a new instance of the RiseRUn struct.</summary>
    /// <param name="rise"></param>
    /// <param name="run"></param>
    internal RiseRun(int rise, int run) : this() {
      this.Rise = rise;
      this.Run  = run;
    }
    /// <summary>Delta-height in units of elevation: meters.</summary>
    public int Rise { get; private set; }
    /// <summary>Delta-width in units of distance:  hexes.</summary>
    public int Run  { get; private set; }

    #region Operators and Interface implementations: IEquatable<RiseRun>, IComparable<RiseRun>
    /// <summary>TODO</summary>
    public static bool operator <  (RiseRun lhs, RiseRun rhs) {
      return (lhs.Rise * rhs.Run) < (lhs.Run * rhs.Rise);
    }
    /// <summary>TODO</summary>
    public static bool operator <= (RiseRun lhs, RiseRun rhs) { return ! (lhs > rhs); }
    /// <summary>TODO</summary>
    public static bool operator >  (RiseRun lhs, RiseRun rhs) {
      return (lhs.Rise * rhs.Run) > (lhs.Run * rhs.Rise);
    }
    /// <summary>TODO</summary>
    public static bool operator >= (RiseRun lhs, RiseRun rhs) { return ! (lhs < rhs); }
    /// <summary>TODO</summary>
    public static bool operator == (RiseRun lhs, RiseRun rhs) { return lhs.Equals(rhs); }
    /// <summary>TODO</summary>
    public static bool operator != (RiseRun lhs, RiseRun rhs) { return ! (lhs == rhs); }
    /// <summary>TODO</summary>
    public bool Equals(RiseRun other) { return (this.Rise * other.Run) == (this.Run * other.Rise); }
    /// <summary>TODO</summary>
    public int CompareTo(RiseRun other) { 
      return (this == other) ?  0
           : (this  < other) ? -1 
                             : +1;
    }
    /// <summary>TODO</summary>
    public override bool Equals(object obj) { return this == (RiseRun)obj; }
    /// <summary>TODO</summary>
    public override int GetHashCode() { return Rise ^ Run; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() { return string.Format(CultureInfo.InvariantCulture,"Rise={0}; Run={1}", Rise, Run); }
  }
}
