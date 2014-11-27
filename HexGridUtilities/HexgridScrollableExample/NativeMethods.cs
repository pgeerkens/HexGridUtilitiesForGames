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
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1587
/// <summary>Extensions to the System.Windows.Forms technologies used by namespace PGNapoleonics.HexgridScrollable.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.WinForms {
  /// <summary>Extern declarations from the Win32 API.</summary>
  internal static partial class NativeMethods {
    /// <summary>P/Invoke declaration for user32.dll.WindowFromPoint</summary>
    /// <remarks><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633558(vector=vs.85).aspx"></a></remarks>
    /// <param name="pt">(Sign-extended) screen coordinates as a Point structure.</param>
    /// <returns>Window handle (hWnd).</returns>
   [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", 
      MessageId = "0", Justification="Research suggests the Code Analysis message is incorrect.")] 
    [DllImport("user32.dll")]
    internal static extern IntPtr WindowFromPoint(System.Drawing.Point pt);

    /// <summary>P/Invoke declaration for user32.dll.SendMessage</summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="msg">Windows message</param>
    /// <param name="wParam">WParam</param>
    /// <param name="lParam">LParam</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
  }
}
