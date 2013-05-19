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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms {
  public static partial class Extensions {
    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
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
    /// <param name="this"></param>
    /// <param name="action"></param>
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
