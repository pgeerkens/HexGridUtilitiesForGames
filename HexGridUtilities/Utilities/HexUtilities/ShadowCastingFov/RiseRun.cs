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
#undef TraceFoV
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using PG_Napoleonics.Utilities;

namespace PG_Napoleonics.Utilities.HexUtilities.ShadowCastingFov {
  #pragma warning disable 0660,0661 // operator == or != w/o overriding object.equals(object o)
  internal struct RiseRun : IEquatable<RiseRun>, IComparable<RiseRun> {
    static RiseRun Max(RiseRun lhs, RiseRun rhs) { return lhs >  rhs  ? lhs : rhs; }
    static RiseRun Min(RiseRun lhs, RiseRun rhs) { return lhs <= rhs  ? lhs : rhs; }

    public int Rise;    // in units of elevation: meters.
    public int Run;     // in units of distance:  hexes.
    public RiseRun(int rise, int run) : this() {
      this.Rise = rise;
      this.Run  = run;
    }

    #region Operators and Interface implementations: IEquatable<RiseRun>, IComparable<RiseRun>
    public static bool operator <  (RiseRun lhs, RiseRun rhs) {
      return (lhs.Rise * rhs.Run) < (lhs.Run * rhs.Rise);
    }
    public static bool operator <= (RiseRun lhs, RiseRun rhs) { return ! (lhs > rhs); }
    public static bool operator >  (RiseRun lhs, RiseRun rhs) {
      return (lhs.Rise * rhs.Run) > (lhs.Run * rhs.Rise);
    }
    public static bool operator >= (RiseRun lhs, RiseRun rhs) { return ! (lhs < rhs); }
    public static bool operator == (RiseRun lhs, RiseRun rhs) { return lhs.Equals(rhs); }
    public static bool operator != (RiseRun lhs, RiseRun rhs) { return ! (lhs == rhs); }
    public bool Equals(RiseRun rhs) { return (this.Rise * rhs.Run) == (this.Run * rhs.Rise); }
    public int CompareTo(RiseRun rhs) { 
      return (this == rhs) ?  0
           : (this  < rhs) ? -1 
                           : +1;
    }
    public override int GetHashCode() { return Rise ^ Run; }
    #endregion
    public override string ToString() { return string.Format("Rise={0}; Run={1}", Rise, Run); }
  }
  #pragma warning restore 0660,0661
}
