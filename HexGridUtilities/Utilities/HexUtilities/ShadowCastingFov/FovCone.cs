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
      return (obj is FovCone) && this == (FovCone)obj;
    }
    public static bool operator != (FovCone @this, FovCone obj) { return ! ( @this == obj); }
    public static bool operator == (FovCone @this, FovCone obj) {
      return @this.Range        == obj.Range  
         &&  @this.RiseRun      == obj.RiseRun
         &&  @this.VectorTop    == obj.VectorTop 
         &&  @this.VectorBottom == obj.VectorBottom;
    }
    public override int GetHashCode() {
      return VectorTop.GetHashCode() ^ Range.GetHashCode() 
           ^ RiseRun.GetHashCode()   ^ VectorBottom.GetHashCode();
    }
    #endregion
  }
}
