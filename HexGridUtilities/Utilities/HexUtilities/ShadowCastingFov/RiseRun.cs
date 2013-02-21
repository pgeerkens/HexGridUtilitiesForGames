#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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
