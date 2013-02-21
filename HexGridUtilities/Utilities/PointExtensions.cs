#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities {
  public static class PointExtensions {
    #region Point scaling functions
    public static Point Scale(this Point @this, int scale) { 
      return @this.Scale(scale,scale);
    }
    public static Point Scale(this Point @this, int scaleX, int scaleY) {
      return new Point(@this.X * scaleX, @this.Y * scaleY);
    }
    public static PointF Scale(this Point @this, float scale) {
      return @this.Scale(scale,scale);
    }
    public static PointF Scale(this Point @this, float scaleX, float scaleY) {
      return new PointF(@this.X,@this.Y).Scale(scaleX,scaleY);
    }
    public static PointF Scale(this PointF @this, float scale) { 
      return @this.Scale(scale,scale);
    }
    public static PointF Scale(this PointF @this, float scaleX, float scaleY) {
      return new PointF(@this.X * scaleX, @this.Y * scaleY);
    }
    #endregion
  }
}
