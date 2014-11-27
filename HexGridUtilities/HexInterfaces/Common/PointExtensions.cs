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
using System.Drawing;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>TODO</summary>
  public static class PointExtensions {
    /// <summary>TODO</summary>
    public static Point Scale(this Point @this, int value) { 
      return @this.Scale(value,value);
    }
    /// <summary>TODO</summary>
    public static Point Scale(this Point @this, int valueX, int valueY) {
      return new Point(@this.X * valueX, @this.Y * valueY);
    }


    /// <summary>TODO</summary>
    public static PointF Scale(this Point @this, float value) {
      return @this.Scale(value,value);
    }
    /// <summary>TODO</summary>
    public static PointF Scale(this Point @this, float valueX, float valueY) {
      return new PointF(@this.X,@this.Y).Scale(valueX,valueY);
    }
    /// <summary>TODO</summary>
    public static PointF Scale(this PointF @this, float value) { 
      return @this.Scale(value,value);
    }
    /// <summary>TODO</summary>
    public static PointF Scale(this PointF @this, float valueX, float valueY) {
      return new PointF(@this.X * valueX, @this.Y * valueY);
    }
  }
}

