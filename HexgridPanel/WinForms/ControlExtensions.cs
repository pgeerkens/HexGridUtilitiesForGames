#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Windows.Forms;

namespace PGNapoleonics.HexgridPanel.WinForms {
    /// <summary>Extension methods for System.Windows.Forms.Control.</summary>
    public static partial class ControlExtensions {
        /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
        /// <param name="this"></param>
        /// <param name="action"></param>
        public static void UIThread(this Control @this, Action action) {
            if (@this ==null) throw new ArgumentNullException("this");
            if (action==null) throw new ArgumentNullException("action");

            if (@this.InvokeRequired)   @this.BeginInvoke(action);
            else                        action.Invoke();
        }

        /// <summary>Executes Action asynchronously on the UI thread, without blocking the calling thread.</summary>
        /// <param name="this"></param>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static void UIThread(this Control @this, Action<object[]> action, params object[] args) {
            if (@this ==null) throw new ArgumentNullException("this");
            if (action==null) throw new ArgumentNullException("action");

            if (@this.InvokeRequired)   @this.BeginInvoke(action,args);
            else                        action.Invoke(args);
        }

        public static void DrawFocusRectangle(this Control @this, Graphics g, Point inflation)
        => ControlPaint.DrawFocusRectangle(g, @this.ClientRectInflated(inflation));

        private static Rectangle ClientRectInflated(this Control @this, Point p) {
            var a = @this.ClientRectangle;    a.Inflate(p.X, p.Y);    return a;
        }        

        /// <inheritdoc/>
        public static bool IsInputKey(this Keys keyData)
        => keyData == Keys.Up   || keyData == Keys.Down
        || keyData == Keys.Left || keyData == Keys.Right;
    }
}
