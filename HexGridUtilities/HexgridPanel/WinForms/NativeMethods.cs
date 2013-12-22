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
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

#pragma warning disable 1587
/// <summary>Extensions to the System.Windows.Forms technologies used by namespace PGNapoleonics.HexgridPanel.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.WinForms {
  /// <summary>Extern declarations from the Win32 API.</summary>
  internal static partial class NativeMethods {
    /// <summary>P/Invoke declaration for user32.dll.WindowFromPoint</summary>
		/// <remarks><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633558(v=vs.85).aspx"></a></remarks>
		/// <param name="point">(Sign-extended) screen coordinates as a Point structure.</param>
		/// <returns>Window handle (hWnd).</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", 
      "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0",
      Justification="Research suggests the Code Analysis message is incorrect.")
    ,System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", 
       "CA1811:AvoidUncalledPrivateCode")]
    [DllImport("user32.dll")]
    public static extern IntPtr WindowFromPoint(System.Drawing.Point point);

    /// <summary>P/Invoke declaration for user32.dll.SendMessage</summary>
		/// <param name="hWnd">Window handle</param>
		/// <param name="msg">Windows message</param>
		/// <param name="wParam">WParam</param>
		/// <param name="lParam">LParam</param>
		/// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA1801:ReviewUnusedParameters", MessageId = "hWnd")
    , System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA1801:ReviewUnusedParameters", MessageId = "lParam")
    ,System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA1801:ReviewUnusedParameters", MessageId = "msg")
    ,System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA1801:ReviewUnusedParameters", MessageId = "wParam")]
    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    /// <summary>TODO</summary>
    [DllImport("Gdi32.dll", SetLastError=true, ExactSpelling=true, CharSet=CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)] 
    public static extern bool BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, 
                                     HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);
  }

    /// <summary>Extension methods for System.Windows.Forms.Control.</summary>
  public static partial class ControlExtensions {
    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
    public static void UIThread(this Control @this, Action action) {
      if (@this==null) throw new ArgumentNullException("this");
      if (action==null) throw new ArgumentNullException("action");

      if (@this.InvokeRequired)   @this.BeginInvoke(action);
      else                        action.Invoke();
    }

    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
    /// <param name="args"></param>
    public static void UIThread(this Control @this, Action<object[]> action, params object[] args) {
      if (@this==null) throw new ArgumentNullException("this");
      if (action==null) throw new ArgumentNullException("action");

      if (@this.InvokeRequired)   @this.BeginInvoke(action,args);
       else                       action.Invoke(args);
      
    }
  }

  /// <summary>Extension methods for System.Windows.Forms.Form.</summary>
  public static partial class FormExtensions {
    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static void UIThread(this Form @this, Action action) {
      if (@this==null) throw new ArgumentNullException("this");
      if (action==null) throw new ArgumentNullException("action");

      if (@this.InvokeRequired)   @this.BeginInvoke(action);
      else                        action.Invoke();
    }

    /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
    /// <param name="args"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static void UIThread(this Form @this, Action<object[]> action, params object[] args) {
      if (@this==null) throw new ArgumentNullException("this");
      if (action==null) throw new ArgumentNullException("action");

      if (@this.InvokeRequired)   @this.BeginInvoke(action,args);
       else                       action.Invoke(args);
    }
  }
}
