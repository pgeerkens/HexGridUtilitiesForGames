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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PGNapoleonics.WinForms {
  /// <summary>TODO</summary>
  public static partial class WinFormsExtensions {
    /// <summary>Reflect to set Double-Buffering on Control.</summary>
    /// <param name="control">Control to operate on.</param>
    /// <param name="setting">New value for parameter.</param>
    public static void MakeDoubleBuffered(this Control control, bool setting)
    {
      if (control==null) throw new ArgumentNullException("control");
      control.GetType()
             .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
             .SetValue(control, setting, null);
    }

    /// <summary>Use COMPOSITED to make a flicker-free form control.</summary>
		/// See "http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/aaed00ce-4bc9-424e-8c05-c30213171c2c"
    public static CreateParams SetCompositedStyle(this Control control, CreateParams parameters) {
      if (control==null) throw new ArgumentNullException("control");
      if (parameters==null) throw new ArgumentNullException("parameters");
      parameters.ExStyle |= (int)WindowStylesEx.COMPOSITED;
      return parameters;
    }
  }

  /// <summary>TODO</summary>/>
  public static partial class PaddingExtensions {
    /// <summary>Point for the Upper-Left corner of the Padding rectangle.</summary>/>
    public static Point Offset(this Padding @this) { return new Point(@this.Left, @this.Top); }
    /// <summary>Point for the Upper-Left corner of the Padding rectangle.</summary>/>
    public static Size OffsetSize(this Padding @this) { return new Size(@this.Left, @this.Top); }
  }
}
