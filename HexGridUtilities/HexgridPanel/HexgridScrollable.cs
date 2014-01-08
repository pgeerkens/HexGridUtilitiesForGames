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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

using WpfInput = System.Windows.Input;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="Hexgrid"/> support.</summary>
  [DockingAttribute(DockingBehavior.AutoDock)]
  public abstract partial class HexgridScrollable : ScrollableControl, IHexgridHost, IMessageFilter, ISupportInitialize {
    /// <summary>Creates a new instance of HexgridPanel.</summary>
    protected HexgridScrollable() {
      InitializeComponent();
    }

    #region ISupportInitialize implementation
    /// <summary>Signals the object that initialization is starting.</summary>
    public virtual void BeginInit() { 
      RefreshCmd  = new RelayCommand(o => { if (o != null) { SetMapDirty(); }  Refresh(); } );
      DataContext = new HexgridViewModel(this);
    }
    /// <summary>Signals the object that initialization is complete.</summary>
    public virtual void EndInit() { 
			Application.AddMessageFilter(this);

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

            SetScrollLimits(DataContext.Model);
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

    /// <summary>TODO</summary>
    static         bool IsLargeStep(ScrollEventType type) { 
      return type == ScrollEventType.LargeIncrement || type == ScrollEventType.LargeDecrement; 
    }

    /// <summary>Service routine to execute a Panel scroll.</summary>
    public         void ScrollPanel(ScrollEventType type, ScrollOrientation orientation, int sign) {
      var scroll = orientation==ScrollOrientation.HorizontalScroll 
                 ? (ScrollProperties)HorizontalScroll : VerticalScroll;
      var delta  = sign * (IsLargeStep(type) ? scroll.LargeChange
                                             : scroll.SmallChange);
      MouseScroll(orientation == ScrollOrientation.HorizontalScroll, -delta);
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
      if (model == null) return;
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
      using(var brush = new SolidBrush(this.BackColor)) g.FillRectangle(brush, g.VisibleClipBounds);
      DataContext.Model.PaintMap(g);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderUnits(Graphics g) {
      DataContext.Model.PaintUnits(g);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderHighlight(Graphics g) {
      DataContext.Model.PaintHighlight(g);
    }

    /// <summary>TODO</summary>
    static protected readonly Matrix TransposeMatrix = new Matrix(0F,1F, 1F,0F, 0F,0F);
    #endregion

    /// <summary>TODO</summary>
    protected override void OnMarginChanged(EventArgs e) {
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
      OnHotspotHexChange(new HexEventArgs(
                          GetHexCoords(e.Location - new Size(Margin.Left,Margin.Top))));

      base.OnMouseMove(e);
    }
    /// <summary>TODO</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual  void OnMouseHWheel(object sender, MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseHWheel: {1}", DataContext.Model.Name, e.ToString());
        AutoScrollPosition = WheelPanel(HorizontalScroll,-e.Delta, ref scrollRemainderHorizontal,
              (delta) => new Point(-AutoScrollPosition.X + delta, -AutoScrollPosition.Y));
        OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack,
              IsShiftKeyDown ? AutoScrollPosition.X : AutoScrollPosition.Y));

    }
    /// <inheritdoc/>
    protected override void OnMouseWheel(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", DataContext.Model.Name, e.ToString()); 
      if( Control.ModifierKeys.HasFlag(Keys.Control)) 
        ScaleIndex += Math.Sign(e.Delta);  
      else
        MouseScroll(IsShiftKeyDown, e.Delta);
    }
    void MouseScroll(bool isHorizontal, int delta) {
      var oldScrollPosition = AutoScrollPosition;

      AutoScrollPosition = isHorizontal
        ? WheelPanel(HorizontalScroll, -delta, ref scrollRemainderHorizontal,
            (amount) => new Point(-AutoScrollPosition.X + amount, -AutoScrollPosition.Y))
        : WheelPanel(VerticalScroll,   -delta, ref scrollRemainderVertical,
            (amount) => new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y + amount));

      OnScroll(GetScrollEventArgs(isHorizontal,oldScrollPosition,AutoScrollPosition));
    }
    int scrollRemainderHorizontal = 0;  //!< <summary>Unapplied horizontal scroll.</summary>
    int scrollRemainderVertical   = 0;  //!< <summary>Unapplied vertical scroll.</summary>
    /// <summary>Return new AutoScrollPosition for applied muse-wheel scroll.</summary>
    static Point WheelPanel(ScrollProperties scroll, int delta, ref int remainder,
      Func<int,Point> newAutoScroll)
    {
      if (Math.Sign(delta) != Math.Sign(remainder)) remainder = 0;
      var steps = (delta+remainder) 
                / (SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines);
      remainder = (delta+remainder) 
                % (SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines);
      return newAutoScroll(scroll.SmallChange * steps);
    }
    static ScrollEventArgs GetScrollEventArgs(bool isHorizontal, Point oldScroll, Point newScroll) {
      return new ScrollEventArgs(
        ScrollEventType.ThumbTrack,
        isHorizontal ? -oldScroll.X : -oldScroll.Y,
        isHorizontal ? -newScroll.X : -newScroll.Y,
        isHorizontal ? ScrollOrientation.HorizontalScroll : ScrollOrientation.VerticalScroll
      );
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
    /// <summary>Raise the MouseLeftClic event.</summary>
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
    #endregion

    /// <summary>Raise the ScaleChange event.</summary>
    protected virtual  void OnScaleChange(EventArgs e) {
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
 
    #region IMessageFilter implementation
    /// <summary>Redirect WM_MouseWheel messages to window under mouse.</summary>
		/// <remarks>Redirect WM_MouseWheel messages to window under mouse (rather than 
    /// that with focus) with adjusted delta.
    /// <a href="http://www.flounder.com/virtual_screen_coordinates.htm">Virtual Screen Coordinates</a>
    /// Dont forget to add this to constructor:
    /// 			Application.AddMessageFilter(this);
    /// </remarks>
		/// <param name="m">The Windows Message to filter and/or process.</param>
		/// <returns>Success (true) or failure (false) to OS.</returns>
		[System.Security.Permissions.PermissionSetAttribute(
			System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		public bool PreFilterMessage(ref Message m) {
			var hWnd  = NativeMethods.WindowFromPoint( WindowsMouseInput.GetPointLParam(m.LParam) );
			var ctl	  = Control.FromHandle(hWnd);
      if (hWnd != IntPtr.Zero  &&  ctl != null) {
        switch((WM)m.Msg) {
          default:  break;
          case WM.MOUSEHWHEEL:
          case WM.MOUSEWHEEL:
            DebugTracing.Trace(TraceFlags.ScrollEvents, true," - {0}.WM.{1}: ", Name, ((WM)m.Msg)); 
            return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
        }
      }
      return false;
		}
    #endregion
  }
}
