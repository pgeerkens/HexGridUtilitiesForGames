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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

using WpfInput = System.Windows.Input;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>TODO</summary>
  public class TiltAwareScrollableControl : ScrollableControl {
    /// <summary>TODO</summary>
    public TiltAwareScrollableControl() {
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
    public event EventHandler<MouseEventArgs> MouseHWheel;

    private int _wheelHPos = 0;   //!< <summary>Unapplied horizontal scroll.</summary>

    /// <summary>TODO</summary>
    protected override void WndProc(ref Message m) {
      if (!IsDisposed  &&  m.HWnd == this.Handle) {
        switch ((WM)m.Msg) {
          case WM.MOUSEHWHEEL: OnMouseHWheel(CreateMouseEventArgs(m));
            m.Result = (IntPtr)0;
            break;
          default: break;
        }
      }
      base.WndProc(ref m);
    }

    /// <summary>TODO</summary>
    /// <param name="e"></param>
    protected virtual void OnMouseHWheel(MouseEventArgs e) {
      if (e == null) throw new ArgumentNullException("e");
      if (!AutoScroll) return;

      _wheelHPos += e.Delta;
      while (_wheelHPos > MouseWheelStep) {
        ScrollHorizontal(MouseWheelStep);
        _wheelHPos -= MouseWheelStep;
      }
      while (_wheelHPos < -MouseWheelStep) {
        ScrollHorizontal(-MouseWheelStep);
        _wheelHPos += MouseWheelStep;
      }

      if (MouseHWheel != null) MouseHWheel.Raise(this, e);
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

    /// <summary>TODO</summary>
    private static MouseEventArgs CreateMouseEventArgs(Message m) {
      return new MouseEventArgs(
          (MouseButtons)NativeMethods.LOWORD(m.WParam),
          0,
          NativeMethods.LOWORD(m.LParam),
          NativeMethods.HIWORD(m.LParam),
          (Int16)NativeMethods.HIWORD(m.WParam)
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
        type.HasFlag(ScrollEventType.LargeDecrement) ? scroll.LargeChange : scroll.SmallChange);
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

  /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="Hexgrid"/> support.</summary>
  [DockingAttribute(DockingBehavior.AutoDock)]
  public partial class HexgridScrollable : TiltAwareScrollableControl, IHexgridHost, ISupportInitialize {
    /// <summary>Creates a new instance of HexgridScrollable.</summary>
    protected HexgridScrollable() {
      InitializeComponent();
    }
    /// <summary>Creates a new instance of HexgridScrollable.</summary>
    public HexgridScrollable(IContainer container) {
      if (container==null) throw new ArgumentNullException("container");
      container.Add(this);

      InitializeComponent();
    }

    #region ISupportInitialize implementation
    /// <summary>Signals the object that initialization is starting.</summary>
    public virtual void BeginInit() { 
      RefreshCmd  = new RelayCommand(o => { if (o != null) { SetMapDirty(); }  Refresh(); } );
      DataContext = new HexgridViewModel(this);
      SetScales (new float[] {1.00F});
    }
    /// <summary>Signals the object that initialization is complete.</summary>
    public virtual void EndInit() { 
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.Opaque, true);
    }
    #endregion

    #region Events
    /// <summary>Announces that the mouse is now over a new hex.</summary>
    public event EventHandler<HexEventArgs> HotspotHexChange;
    /// <summary>Announces occurrence of a mouse left-click with the <b>Alt</b> key depressed.</summary>
    public event EventHandler<HexEventArgs> MouseAltClick;
    /// <summary>Announces occurrence of a mouse left-click with the <b>Ctl</b> key depressed.</summary>
    public event EventHandler<HexEventArgs> MouseCtlClick;
    /// <summary>Announces a mouse left-click with no <i>shift</i> keys depressed.</summary>
    public event EventHandler<HexEventArgs> MouseLeftClick;
    /// <summary>Announces a mouse right-click. </summary>
    public event EventHandler<HexEventArgs> MouseRightClick;
    /// <summary>Announces a change of drawing scale on this HexgridPanel.</summary>
    public event EventHandler<EventArgs>    ScaleChange;
    #endregion

    #region Properties
    /// <summary>TODO</summary>
    public HexgridViewModel        DataContext       { get; set; }
    /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
    public         SizeF           GridSizeF         { get { return DataContext.Model.GridSize.Scale(MapScale); } }
    /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
    public         HexCoords       HotspotHex        { get { return DataContext.HotspotHex; } }
    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
    public static  bool            IsAltKeyDown      { get { return ModifierKeys.HasFlag(Keys.Alt); } }
    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
    public static  bool            IsCtlKeyDown      { get { return ModifierKeys.HasFlag(Keys.Control); } }
    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
    public static  bool            IsShiftKeyDown    { get { return ModifierKeys.HasFlag(Keys.Shift); } }
    /// <summary>TODO</summary>
    public         bool            IsMapDirty        { 
      get { return _isMapDirty; }
      set { 
        _isMapDirty = value; 
        if(_isMapDirty) { IsUnitsDirty = true; } 
      }
    } bool _isMapDirty;
    /// <summary>TODO</summary>
    public         bool            IsUnitsDirty      { 
      get { return _isUnitsDirty; }
      set { 
        _isUnitsDirty = value; 
        if(_isUnitsDirty) { Invalidate(); }
      }
    } bool _isUnitsDirty;
    /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
    public         bool            IsTransposed      { 
      get { return DataContext.IsTransposed; }
      set { DataContext.IsTransposed = value;  SetScrollLimits(DataContext.Model); }
    }
    /// <inheritdoc/>
    public         Size            MapSizePixels     { get { return DataContext.Model.MapSizePixels; } } // + MapMargin.Scale(2);} }
    /// <summary>Current scaling factor for map display.</summary>
    public         float           MapScale          { get { return DataContext.MapScale; } }
//    /// <summary>MapBoard hosting this panel.</summary>
//    public         IMapDisplay     Model             { get { return DataContext.Model; } }
    /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
    public         HexCoords       PanelCenterHex    { 
      get { return GetHexCoords( Location + Size.Round(ClientSize.Scale(0.50F)) ); }
    }
    /// <summary>TODO</summary>
    public WpfInput.ICommand       RefreshCmd        { get; private set; }
    /// <summary>Index into <code>Scales</code> of current map scale.</summary>
    public virtual int             ScaleIndex        { 
      get { return DataContext.ScaleIndex; }
      set { var newValue = Math.Max(0, Math.Min(DataContext.Scales.Count-1, value));
            var CenterHex           = PanelCenterHex;
            DataContext.ScaleIndex  = newValue; 

//            SetScrollLimits(DataContext.Model);
            SetScroll(CenterHex);
            OnScaleChange(EventArgs.Empty); 
          } 
    }
    /// <summary>Returns, as a Rectangle, the IUserCoords for the currently visible extent.</summary>
    public virtual CoordsRectangle VisibleRectangle  {
      get { return GetClipCells( AutoScrollPosition.Scale(-1.0F/MapScale), 
                                         ClientSize.Scale( 1.0F/MapScale) );
      }
    }
    #endregion

    #region Methods
    /// <summary>TODO</summary>
    public         void CenterOnHex(HexCoords coords) {
      AutoScrollPosition = ScrollPositionToCenterOnHex(coords);
      IsMapDirty = true;
      Invalidate();
    }

    /// <summary>TODO</summary>
        CoordsRectangle GetClipCells(PointF point, SizeF size) { return DataContext.Model.GetClipCells(point, size); }
    /// <summary><c>HexCoords</c> for a selected hex.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <returns>Coordinates for a hex specified by a screen point.</returns>
    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
    public    HexCoords GetHexCoords(Point point) {
      return DataContext.Hexgrid.GetHexCoords(point, new Size(AutoScrollPosition));
    }

    /// <summary>Force repaint of backing buffer for Map underlay.</summary>
    public virtual void SetMapDirty() { Invalidate(ClientRectangle); }

    /// <summary>TODO</summary>
    public         void SetModel(IMapDisplay model) {
      SetScrollLimits(DataContext.Model);   
      DataContext.SetModel(model);
      SetMapDirty();
    }

    /// <summary>TODO</summary>
    public         void SetPanelSize() {
      if(DesignMode || !IsHandleCreated) return;
      DebugTracing.Trace(TraceFlags.Sizing," - {0}.SetPanelSize; ClientSize = {1}", DataContext.Model.Name, ClientSize); 
      SetScroll(PanelCenterHex);
    }

    /// <summary>Sets ScrollBars, then centres on <c>newCenterHex</c>.</summary>
    public virtual void SetScroll(HexCoords newCenterHex) {
      if(DesignMode || !IsHandleCreated) return;
      DebugTracing.Trace(TraceFlags.Sizing," - {0}.SetPanelSize; Center Hex = {1}", DataContext.Model.Name, newCenterHex.ToString()); 

      SetScrollLimits(DataContext.Model);

      CenterOnHex(newCenterHex);
    }

    /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
    public virtual void SetScrollLimits(IMapDisplay model) {
      if (model == null  ||  !AutoScroll) return;
      var smallChange              = Size.Ceiling(model.GridSize.Scale(MapScale));
      HorizontalScroll.SmallChange = smallChange.Width;
      VerticalScroll.SmallChange   = smallChange.Height;

      var largeChange              = Size.Round(ClientSize.Scale(0.75F));
      HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
      VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

      var size                     = DataContext.Hexgrid.GetSize(MapSizePixels,MapScale)
                                   + Margin.Size;
      if (AutoScrollMinSize != size) {
        AutoScrollMinSize          = size;
        HorizontalScroll.Maximum   = Math.Min(1, Math.Max(1, Margin.Horizontal 
                                   + size.Width  - ClientSize.Width));
        VerticalScroll.Maximum     = Math.Min(1, Math.Max(1, Margin.Vertical 
                                   + size.Height - ClientSize.Height));
        Invalidate();
      }
    }
    #endregion

    #region Grid Coordinates
    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public Point HexCenterPoint(HexCoords coordsNewULHex) {
      return DataContext.Hexgrid.HexCenterPoint(coordsNewULHex);
    }
    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    protected Point ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex) {
      return DataContext.Hexgrid.ScrollPositionToCenterOnHex(coordsNewCenterHex,VisibleRectangle);
    }
    #endregion

    #region Painting
    /// <inheritdoc/>
    protected override void OnPaintBackground(PaintEventArgs e) { ; }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");

      if(IsHandleCreated) { 
        var g      = e.Graphics;
        if (DesignMode) { g.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

        g.Clip = new Region(e.ClipRectangle);
        if (IsTransposed) { g.Transform = TransposeMatrix; }

        var scroll = DataContext.Hexgrid.GetScrollPosition(AutoScrollPosition);
        g.TranslateTransform(scroll.X, scroll.Y);
        g.TranslateTransform(Margin.Left,Margin.Top);
        g.ScaleTransform(MapScale,MapScale);
        TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, g.VisibleClipBounds);

        var state = g.Save();
        RenderMap(g);

        g.Restore(state); state = g.Save();
        RenderUnits(g);

        g.Restore(state); state = g.Save();
        RenderHighlight(g);

        g.Restore(state);
      }
    }
    /// <summary>TODO</summary>
    protected virtual void RenderMap(Graphics g) {
      if (g == null) throw new ArgumentNullException("g");
      using(var brush = new SolidBrush(this.BackColor)) g.FillRectangle(brush, g.VisibleClipBounds);
      DataContext.Model.PaintMap(g);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderUnits(Graphics g) {
      if (g == null) throw new ArgumentNullException("g");
      DataContext.Model.PaintUnits(g);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderHighlight(Graphics g) {
      if (g == null) throw new ArgumentNullException("g");
      DataContext.Model.PaintHighlight(g);
    }

    /// <summary>TODO</summary>
    static protected readonly Matrix TransposeMatrix = new Matrix(0F,1F, 1F,0F, 0F,0F);
    #endregion

    /// <summary>TODO</summary>
    protected override void OnMarginChanged(EventArgs e) {
      if (e == null) throw new ArgumentNullException("e");
      base.OnMarginChanged(e);
      DataContext.Margin = Margin;
    }

    #region Mouse event handlers
    /// <inheritdoc/>
    protected override void OnMouseClick(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.Mouse.Trace(" - {0}.OnMouseClick - Shift: {1}; Ctl: {2}; Alt: {3}", 
                                      Name, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);

      var coords    = GetHexCoords(e.Location);
      var eventArgs = new HexEventArgs(coords, e, ModifierKeys);

           if (e.Button == MouseButtons.Middle)   base.OnMouseClick(eventArgs);
      else if (e.Button == MouseButtons.Right)    this.OnMouseRightClick(eventArgs);
      else if (IsAltKeyDown  && !IsCtlKeyDown)    this.OnMouseAltClick(eventArgs);
      else if (IsCtlKeyDown)                      this.OnMouseCtlClick(eventArgs);
      else                                        this.OnMouseLeftClick(eventArgs);
    }
    /// <inheritdoc/>
    protected override void OnMouseMove(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      OnHotspotHexChange(new HexEventArgs(GetHexCoords(e.Location - Margin.OffsetSize())));

      base.OnMouseMove(e);
    }

    /// <summary>Raise the MouseAltClick event.</summary>
    protected virtual void OnMouseAltClick(HexEventArgs e) { MouseAltClick.Raise(this,e); }
    /// <summary>Raise the MouseCtlClick event.</summary>
    protected virtual void OnMouseCtlClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      DataContext.Model.GoalHex = e.Coords;
      MouseCtlClick.Raise(this,e);
      Refresh();
    }
    /// <summary>Raise the MouseLeftClick event.</summary>
    protected virtual void OnMouseLeftClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      DataContext.Model.StartHex = e.Coords;
      MouseLeftClick.Raise(this,e);
      Refresh();
    }
    /// <summary>Raise the MouseRightClick event.</summary>
    protected virtual void OnMouseRightClick(HexEventArgs e) { MouseRightClick.Raise(this,e); }
   /// <summary>Raise the HotspotHexChange event.</summary>
    protected virtual void OnHotspotHexChange(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      DataContext.Model.HotspotHex = e.Coords;
      HotspotHexChange.Raise(this,e);
      Refresh();
    }

    /// <summary>Raise the ScaleChange event.</summary>
    protected virtual void OnScaleChange(EventArgs e) {
      SetMapDirty();
      OnResize(e);
      Invalidate();
      ScaleChange.Raise(this, e);
    }

    /// <inheritdoc/>
    protected override void OnResize(EventArgs e) {
      SetScrollLimits(DataContext.Model);
      base.OnResize(e);
    }
    #endregion

    #region MouseWheel & Scroll event handlers
    /// <inheritdoc/>
    protected override void OnMouseWheel(MouseEventArgs e) {
      if (e == null) throw new ArgumentNullException("e");
      TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Name, e.ToString());

      if (Control.ModifierKeys.HasFlag(Keys.Control)) ScaleIndex += Math.Sign(e.Delta);
      else if (IsShiftKeyDown)                        base.OnMouseHWheel(e);
      else                                            base.OnMouseWheel(e);
    }

    /// <summary>TODO</summary>
    public void ScrollPanelVertical(ScrollEventType type, int sign) {
      ScrollPanelCommon(type, sign, VerticalScroll);
    }
    /// <summary>TODO</summary>
    public void ScrollPanelHorizontal(ScrollEventType type, int sign) {
      ScrollPanelCommon(type, sign, HorizontalScroll);
    }
    /// <summary>TODO</summary>
    private void ScrollPanelCommon(ScrollEventType type, int sign, ScrollProperties scroll) {
      if (sign == 0) return;
      Func<Point, int, Point> func = (p, step) => new Point(-p.X, -p.Y + step * sign);
      AutoScrollPosition = func(AutoScrollPosition,
        type.HasFlag(ScrollEventType.LargeDecrement) ? scroll.LargeChange : scroll.SmallChange);
    }
    #endregion

    /// <summary>Array of supported map scales  as IList&lt;float&gt;.</summary>
    public ReadOnlyCollection<float>     Scales        { get; private set; }
    public void SetScales (IList<float> scales) { Scales = new ReadOnlyCollection<float>(scales); }
  }
}
