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
using System.Windows.Forms;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>A Tilt-Aware Focusable Panel</summary>
  /// ///<remarks>Courtesy of Hans Passant: 
  /// <a>http://stackoverflow.com/questions/3562235/panel-not-getting-focus</a>
  /// </remarks>
  public class TiltAwarePanel : Panel {
    /// <summary>TODO</summary>
    public TiltAwarePanel() {
      this.SetStyle(ControlStyles.Selectable, true);
      this.TabStop = true;
    }

    #region SelectablePanel implementation
    /// <inheritdoc/>
    protected override void OnMouseDown(MouseEventArgs e) {
      this.Focus();
      base.OnMouseDown(e);
    }
    /// <inheritdoc/>
    protected override bool IsInputKey(Keys keyData) {
      if (keyData == Keys.Up || keyData == Keys.Down) return true;
      if (keyData == Keys.Left || keyData == Keys.Right) return true;
      return base.IsInputKey(keyData);
    }
    /// <inheritdoc/>
    protected override void OnEnter(EventArgs e) {
      this.Invalidate();
      base.OnEnter(e);
    }
    /// <inheritdoc/>
    protected override void OnLeave(EventArgs e) {
      this.Invalidate();
      base.OnLeave(e);
    }
    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e) {
      if (e == null) throw new ArgumentNullException("e");
      base.OnPaint(e);
      if (this.Focused) {
        var rc = this.ClientRectangle;
        rc.Inflate(-2, -2);
        ControlPaint.DrawFocusRectangle(e.Graphics, rc);
      }
    }
    #endregion

    #region Mouse Tilt Wheel (MouseHWheel) event implementation
    /// <summary>Occurs when the mouse tilt-wheel moves while the control has focus.</summary>
    public event EventHandler<MouseEventArgs> MouseHwheel;

    private int _wheelHPos = 0;   //!< <summary>Unapplied horizontal scroll.</summary>

    /// <summary>Processes Windows messages.</summary>
    protected override void WndProc(ref Message m) {
      if (!IsDisposed && m.HWnd == this.Handle) {
        switch ((WM)m.Msg) {
          case WM.MouseHwheel:  OnMouseHwheel(CreateMouseEventArgs(m));
                                m.Result = (IntPtr)0;
                                break;

          default:              break;
        }
      }
      base.WndProc(ref m);
    }

    /// <summary>Raises the MouseHWheel event.</summary>
    /// <param name="e">A <c>MouseEventArgs</c> that contains the event data. </param>
    protected virtual void OnMouseHwheel(MouseEventArgs e) {
      if (e == null) throw new ArgumentNullException("e");

      _wheelHPos += e.Delta;
      while (_wheelHPos >= MouseWheelStep) {
        ScrollHorizontal(MouseWheelStep);
        _wheelHPos -= MouseWheelStep;
      }
      while (_wheelHPos <= -MouseWheelStep) {
        ScrollHorizontal(-MouseWheelStep);
        _wheelHPos += MouseWheelStep;
      }

      if (MouseHwheel != null) MouseHwheel.Raise(this, e);
    }

    /// <summary>TODO</summary>
    private void ScrollHorizontal(int delta) {
      AutoScrollPosition = new Point(
        -AutoScrollPosition.X + delta,
        -AutoScrollPosition.Y);
    }

    /// <summary>TODO</summary>
    private static int MouseWheelStep {
      get {
        return SystemInformation.MouseWheelScrollDelta
             / SystemInformation.MouseWheelScrollLines;
      }
    }

    private static MouseEventArgs CreateMouseEventArgs(Message m) {
      return new MouseEventArgs (
          (MouseButtons)NativeMethods.LoWord(m.WParam),
          0,
          NativeMethods.LoWord(m.LParam),
          NativeMethods.HiWord(m.LParam),
          (Int16)NativeMethods.HiWord(m.WParam)
        );
    }
    #endregion

    #region Panel Scroll extensions
    /// <summary>TODO</summary>
    public void ScrollVertical(ScrollEventType type, int sign) {
      ScrollPanelCommon(type, sign, VerticalScroll);
    }
    /// <summary>TODO</summary>
    public void ScrollHorizontal(ScrollEventType type, int sign) {
      ScrollPanelCommon(type, sign, HorizontalScroll);
    }
    /// <summary>TODO</summary>
    private void ScrollPanelCommon(ScrollEventType type, int sign, ScrollProperties scroll) {
      if (sign == 0) return;
      Func<Point, int, Point> func = (p, step) => new Point(-p.X, -p.Y + step * sign);
      AutoScrollPosition = func(AutoScrollPosition,
        type.Hasflag(ScrollEventType.LargeDecrement) ? scroll.LargeChange : scroll.SmallChange);
    }

    /// <summary>Service routine to execute a Panel scroll.</summary>
    [Obsolete("Use ScrollPanelVertical or ScrollPanelHorizontal instead.")]
    public void ScrollPanel(ScrollEventType type, ScrollOrientation orientation, int sign) {
      if (orientation == ScrollOrientation.VerticalScroll)
        ScrollVertical(type, sign);
      else
        ScrollHorizontal(type, sign);
    }
    #endregion
  }
}
