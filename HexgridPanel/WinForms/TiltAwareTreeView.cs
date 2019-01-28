#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Drawing;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel.WinForms {
  /// <summary>TODO</summary>
  public partial class TiltAwareTreeView : TreeView {
    private const int VerticalScrollLargeChange   = 120;
    private const int HorizontalScrollLargeChange = 120;

    /// <summary>TODO</summary>
    public TiltAwareTreeView() {
      InitializeComponent();
    }

    #region SelectablePanel implementation
    /// <inheritdoc/>
    protected override void OnMouseEnter(EventArgs e) {
      base.OnMouseEnter(e);
      Focus();
    }
    /// <inheritdoc/>
    protected override void OnMouseLeave(EventArgs e) {
      Parent.Focus();
      base.OnMouseEnter(e);
    }
    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e) {
      base.OnPaint(e);
      if (this.Focused  &&  this.ShowFocusCues) {
        var rc = this.ClientRectangle;
        rc.Inflate(-2, -2);
        ControlPaint.DrawFocusRectangle(e.Graphics, rc);
      }
    }
    #endregion

    #region Mouse Tilt Wheel (MouseHwheel) event implementation
    /// <summary>Occurs when the mouse tilt-wheel moves while the control has focus.</summary>
    public virtual event EventHandler<MouseEventArgs>  MouseHwheel;

    private int _unappliedHorizontalScroll = 0;
    private int _unappliedVerticalScroll   = 0;

    /// <summary>Extend Windows Message Loop to receive MouseHwheel messages.</summary>
    protected override void WndProc(ref Message m) {
      if (!IsDisposed  &&  m.HWnd == this.Handle) {
        switch ((WM)m.Msg) {
          case WM.MouseHwheel:  OnMouseHwheel(CreateMouseEventArgs(m));
                                m.Result = (IntPtr)0;
                                break;
          default:              break;
        }
      }
      base.WndProc(ref m);
    }

    /// <summary>TODO</summary>
    /// <param name="e"></param>
    protected virtual void OnMouseHwheel(MouseEventArgs e) {
      if (e == null) throw new ArgumentNullException("e");
      if (!Scrollable) return;

      RollHorizontal(e.Delta);
      MouseHwheel.Raise(this, e);

      var eh = e as HandledMouseEventArgs;
      if (eh != null) eh.Handled = true;
    }

    /// <summary>TODO</summary>
    protected virtual int MouseWheelStep {
      get { return SystemInformation.MouseWheelScrollDelta
                 / SystemInformation.MouseWheelScrollLines;
      }
    }

    /// <summary>TODO</summary>
    private static MouseEventArgs CreateMouseEventArgs(Message m) {
      return new MouseEventArgs(
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
    public void PageUp()    { RollVertical(-1 * VerticalScrollLargeChange); }
    /// <summary>TODO</summary>
    public void PageDown()  { RollVertical(+1 * VerticalScrollLargeChange); }
    /// <summary>TODO</summary>
    public void PageLeft()  { RollHorizontal(-1 * HorizontalScrollLargeChange); }
    /// <summary>TODO</summary>
    public void PageRight() { RollHorizontal(+1 * HorizontalScrollLargeChange); }
    /// <summary>TODO</summary>
    public void LineUp()    { RollVertical(-1 * MouseWheelStep); }
    /// <summary>TODO</summary>
    public void LineDown()  { RollVertical(+1 * MouseWheelStep); }
    /// <summary>TODO</summary>
    public void LineLeft()  { RollHorizontal(-1 * MouseWheelStep); }
    /// <summary>TODO</summary>
    public void LineRight() { RollHorizontal(+1 * MouseWheelStep); }

    private void RollHorizontal(int delta) {
      _unappliedHorizontalScroll += delta;
      while (_unappliedHorizontalScroll >= MouseWheelStep) {
        HScrollByOffset( + MouseWheelStep);
        _unappliedHorizontalScroll -= MouseWheelStep;
      }
      while (_unappliedHorizontalScroll <= -MouseWheelStep) {
        HScrollByOffset( - MouseWheelStep);
        _unappliedHorizontalScroll += MouseWheelStep;
      }
    }

    private void RollVertical(int delta) {
      _unappliedVerticalScroll += delta;
      while (_unappliedVerticalScroll >= MouseWheelStep) {
        VScrollByOffset( + MouseWheelStep);
        _unappliedVerticalScroll -= MouseWheelStep;
      }
      while (_unappliedVerticalScroll <= -MouseWheelStep) {
        VScrollByOffset( - MouseWheelStep);
        _unappliedVerticalScroll += MouseWheelStep;
      }
    }

    /// <summary>TODO</summary>
    private void HScrollByOffset(int delta) {
      AutoScrollOffset = new Point (-AutoScrollOffset.X + delta, -AutoScrollOffset.Y);
    }

    /// <summary>TODO</summary>
    private void VScrollByOffset(int delta) {
      AutoScrollOffset = new Point (-AutoScrollOffset.X, -AutoScrollOffset.Y + delta);
    }

    List<Action> ScrollActions { get { return new List<Action> {
                                        () => PageUp(),   () => PageDown(),
                                        () => PageLeft(), () => PageRight(),
                                        () => LineUp(),   () => LineDown(),
                                        () => LineLeft(), () => LineRight()  };
      }
    }

    /// <summary>Service routine to execute a Panel scroll.</summary>
    [Obsolete("Use ScrollPanelVertical or ScrollPanelHorizontal instead.")]
    public void ScrollPanel(ScrollEventType type, ScrollOrientation orientation, int sign) {
      ScrollActions [
            ( (type.HasFlag(ScrollEventType.SmallDecrement))      ? 4 : 0 )
          + ( (orientation == ScrollOrientation.HorizontalScroll) ? 2 : 0 )
          + ( (sign == +1)                                        ? 1 : 0 ) ] ();
    }
    #endregion
  }
}
