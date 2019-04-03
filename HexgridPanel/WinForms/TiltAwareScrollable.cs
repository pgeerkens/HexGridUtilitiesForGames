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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexgridPanel.WinForms;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>A Tilt-Aware <see cref="ScrollableControl"/></summary>
    /// ///<remarks>Courtesy of Hans Passant: 
    /// <a>http://stackoverflow.com/questions/3562235/panel-not-getting-focus</a>
    /// </remarks>
    public class TiltAwareScrollableControl : ScrollableControl, IScrollableControl {
        /// <summary>TODO</summary>
        public TiltAwareScrollableControl() : base() {
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
        }

        #region Implementation of "scrolling without focus"
        /// <inheritdoc/>
        protected override bool IsInputKey(Keys keyData) 
            => keyData.IsInputKey() || base.IsInputKey(keyData);
        /// <inheritdoc/>
        protected override void OnMouseDown(MouseEventArgs e) { Focus(); base.OnMouseDown(e); }
        /// <inheritdoc/>
        protected override void OnEnter(EventArgs e)          { Invalidate(); base.OnEnter(e); }
        /// <inheritdoc/>
        protected override void OnLeave(EventArgs e)          { Invalidate(); base.OnLeave(e); }
        /// <inheritdoc/>
        protected override void OnMouseEnter(EventArgs e)     { base.OnMouseEnter(e); Focus(); }
        /// <inheritdoc/>
        protected override void OnMouseLeave(EventArgs e)     { Parent.Focus(); base.OnMouseLeave(e); }
        /// <inheritdoc/>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (Focused  &&  ShowFocusCues) { this.DrawFocusRectangle(e.Graphics, new Point(-2,-2)); }
        }
        #endregion

        /// <summary>Occurs when the mouse tilt-wheel moves while the control has focus.</summary>
        public virtual event EventHandler<MouseEventArgs>  MouseHWheel;

        /// <inheritdoc/>
        public Point UnappliedScroll { get; set; } = new Point();

        /// <inheritdoc/>
        public Point ScrollLargeChange
            => new Point (HorizontalScroll.LargeChange, VerticalScroll.LargeChange);

        /// <summary>Raise a <see cref="MouseHWheel"/> event.</summary>
        /// <param name="e">EventArgs for the event.</param>
        protected virtual void OnMouseHWheel(MouseEventArgs e) {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (!AutoScroll) return;

            this.RollHorizontal(e.Delta);
            MouseHWheel.Raise(this, e);

            if(e is HandledMouseEventArgs eh) eh.Handled = true;
        }

        /// <summary>Extend Windows Message Loop to receive MouseHWheel messages.</summary>
        protected override void WndProc(ref Message m) {
            if (!IsDisposed  &&  m.HWnd == Handle) {
                switch (m.Msg) {
                    case (int)WM.MouseHWheel:  OnMouseHWheel(m.CreateMouseEventArgs());
                                               m.Result = (IntPtr)0;
                                               break;
                    default:                   break;
                }
            }
            base.WndProc(ref m);
        }
    }
}
