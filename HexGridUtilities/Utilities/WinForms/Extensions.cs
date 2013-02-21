#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms {
  public static partial class Extensions {
    /// <summary>Reflect to set Double-Buffering on Control.</summary>
    /// <param name="control">Control to operate on.</param>
    /// <param name="setting">New value for parameter.</param>
    public static void MakeDoubleBuffered(this Control control, bool setting)
    {
      control.GetType()
             .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
             .SetValue(control, setting, null);
    }
    /// <summary>Use WS_EX_COMPOSITED to make a flicker-free form control.</summary>
		/// <see cref="http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/aaed00ce-4bc9-424e-8c05-c30213171c2c"/>
    public static CreateParams SetCompositedStyle(this Control control, CreateParams cp) {
      cp.ExStyle |= (int)WindowStylesEx.WS_EX_COMPOSITED;
      return cp;
    }
    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="control"></param>
    /// <param name="code"></param>
    public static void UIThread(this Control @this, Action action) {
      if (@this.InvokeRequired) {
        @this.BeginInvoke(action);
      } else {
        action.Invoke();
      }
    }
    public static void UIThread(this Control @this, Action<object[]> action, params object[] args) {
      if (@this.InvokeRequired) {
        @this.BeginInvoke(action,args);
      } else {
        action.Invoke(args);
      }
    }
    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="control"></param>
    /// <param name="code"></param>
    public static void UIThread(this Form @this, Action action) {
      if (@this.InvokeRequired) {
        @this.BeginInvoke(action);
      } else {
        action.Invoke();
      }
    }
    public static void UIThread(this Form @this, Action<object[]> action, params object[] args) {
      if (@this.InvokeRequired) {
        @this.BeginInvoke(action,args);
      } else {
        action.Invoke(args);
      }
    }
  }
}
