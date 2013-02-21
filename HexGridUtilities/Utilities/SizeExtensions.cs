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
  public static class SizeExtensions {
    #region Size scaling functions
    public static Size Scale(this Size @this, int scale) { 
      return @this.Scale(scale,scale);
    }
    public static Size Scale(this Size @this, int scaleX, int scaleY) {
      return new Size(@this.Width * scaleX, @this.Height * scaleY);
    }
    public static SizeF Scale(this Size @this, float scale) {
      return @this.Scale(scale,scale);
    }
    public static SizeF Scale(this Size @this, float scaleX, float scaleY) {
      return new SizeF(@this).Scale(scaleX,scaleY);
    }
    public static SizeF Scale(this SizeF @this, float scale) { 
      return @this.Scale(scale,scale);
    }
    public static SizeF Scale(this SizeF @this, float scaleX, float scaleY) {
      return new SizeF(@this.Width * scaleX, @this.Height * scaleY);
    }
    #endregion
  }
}
