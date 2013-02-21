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
  internal struct FovCone : IEquatable<FovCone> {
    public IntVector2D  VectorTop    { get; private set; }
    public IntVector2D  VectorBottom { get; private set; }
    public int          Range        { get; private set; }
    public RiseRun      RiseRun      { get; private set; }

    public FovCone(int range, IntVector2D top, IntVector2D bottom, RiseRun riseRun) : this() {
      this.Range        = range;
      this.VectorTop    = top;
      this.VectorBottom = bottom;
      this.RiseRun      = riseRun;
    }
    public override string ToString() {
      return string.Format("Y={0}, TopVector={1}, BottomVector={2}, RiseRun={3}",
                                  Range, VectorTop, VectorBottom, RiseRun);
    }

    #region Value Equality
    bool IEquatable<FovCone>.Equals(FovCone obj) { return this == obj; }
    public override bool Equals(object obj) { 
      return (obj is FovCone) ? this == (FovCone)obj : false;
    }
    public static bool operator != (FovCone @this, FovCone obj) { return !( @this == obj); }
    public static bool operator == (FovCone @this, FovCone obj) {
      return @this.Range == obj.Range  
          &&  @this.RiseRun   == obj.RiseRun
          &&  @this.VectorTop == obj.VectorTop 
          &&  @this.VectorBottom == obj.VectorBottom;
    }
    public override int GetHashCode() {
      return VectorTop.GetHashCode() ^ Range.GetHashCode() 
            ^ RiseRun.GetHashCode()   ^ VectorBottom.GetHashCode();
    }
    #endregion
  }
}
