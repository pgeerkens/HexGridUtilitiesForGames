#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Drawing;

namespace PGNapoleonics.HexgridPanel.WinForms {
    /// <summary>Methods to get/set AUtoScrollPosition for a <see cref="TreeView"/> control</summary>
    /// <remarks>
    ///  Courtesy of Stefan Koell for this solution:
    ///    <a>https://stackoverflow.com/questions/332788/maintain-scroll-position-of-treeview]]</a>
    /// </remarks>
    internal static class NativeMethodsTreeView {
        #region TreeView
        public static Point GetAutoScrollPosition(this IntPtr HWnd)
            => new Point(GetScrollPos(HWnd, SB_HORZ),  GetScrollPos(HWnd, SB_VERT) );

        public static void SetAutoScrollPosition(this IntPtr HWnd, Point position) {
            SetScrollPos(HWnd, SB_HORZ, position.X, true);
            SetScrollPos(HWnd, SB_VERT, position.Y, true); 
        }

        [DllImport("user32.dll",  CharSet = CharSet.Unicode)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll",  CharSet = CharSet.Unicode)]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
        #endregion
    }
}
