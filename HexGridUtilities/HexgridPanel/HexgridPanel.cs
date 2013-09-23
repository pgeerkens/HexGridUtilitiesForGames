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
using System.Drawing.Imaging;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="Hexgrid"/> support.</summary>
  [DockingAttribute(DockingBehavior.AutoDock)]
  [Obsolete("Use PGNapoleonics.HexgridPanel.HexgridScrollable instead.")]
  public partial class HexgridPanel : Panel, IHexgridHost, IMessageFilter {
    #region Constructors
    /// <summary>Creates a new instance of HexgridPanel.</summary>
    public HexgridPanel() {
      BeginInit();
      InitializeComponent();
      EndInit();
    }
    /// <summary>Creates a new instance of HexgridPanel.</summary>
    public HexgridPanel(IContainer container) {
      if (container==null) throw new ArgumentNullException("container");
      container.Add(this);

      BeginInit();
      InitializeComponent();
      EndInit();
    }
    #endregion

    #region ISupportInitialize implementation
    /// <summary>Signals the object that initialization is starting.</summary>
    public virtual void BeginInit() { 
      Scales        = new List<float>() {1.000F}.AsReadOnly();
      SetModel(new EmptyBoard());
      HotspotHex    = HexCoords.EmptyUser;
    }
    /// <summary>Signals the object that initialization is complete.</summary>
    public virtual void EndInit() { 
			Application.AddMessageFilter(this);
      this.MakeDoubleBuffered(true);
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

    /// <summary>TODO</summary>
    public void SetModel(MapDisplay<MapGridHex> model) {
      Model = model;   
      SetScrollLimits(Model);   
      SetMapDirty();
    }

    #region Properties
    /// <summary>MapBoard hosting this panel.</summary>
    public MapDisplay<MapGridHex> Model           { get; private set; }

    /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
    public        HexCoords HotspotHex     { get; private set; }

    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsAltKeyDown   { get {return ModifierKeys.HasFlag(Keys.Alt);} }
    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsCtlKeyDown   { get {return ModifierKeys.HasFlag(Keys.Control);} }
    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsShiftKeyDown { get {return ModifierKeys.HasFlag(Keys.Shift);} }

    /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
    [Browsable(true)]
    public bool        IsTransposed  { 
      get { return Model.IsTransposed; }
      set { Model.IsTransposed = value;  SetScrollLimits(Model); }
    }

    /// <inheritdoc/>
    public Size        MapSizePixels { get {return Model.MapSizePixels;} } // + MapMargin.Scale(2);} }

    /// <summary>Current scaling factor for map display.</summary>
    public float       MapScale      { 
      get { return Model.MapScale; } 
      private set { Model.MapScale = value;  SetScrollLimits(Model); } 
    }

    /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
    public HexCoords PanelCenterHex  { 
      get { return GetHexCoords( Location + Size.Round(ClientSize.Scale(0.50F)) ); }
    }

    /// <summary>Index into <code>Scales</code> of current map scale.</summary>
    public virtual int ScaleIndex    { 
      get { return _scaleIndex; }
      set { var newValue = Math.Max(0, Math.Min(Scales.Count-1, value));
            if( _scaleIndex != newValue) {
              _scaleIndex = newValue;
              MapScale    = Scales[ScaleIndex];

              OnScaleChange(EventArgs.Empty); 
            }
          } 
    } int _scaleIndex;

    /// <summary>Array of supported map scales  as IList&lt;float&gt;.</summary>
    public ReadOnlyCollection<float>     Scales        { get; set; }
    #endregion

    /// <summary>Force repaint of backing buffer for Map underlay.</summary>
    public void SetMapDirty() {  MapBuffer = null; }

    /// <summary>Set property Scales (array of supported map scales as IList&lt;float&gt;.</summary>
    [Obsolete("Use Property Setter 'Scales' instead.")]
    public void SetScaleList(ReadOnlyCollection<float> scales) { Scales = scales; }

    /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
    public virtual void SetScrollLimits(IMapDisplay model) {
      if (model == null) return;
      var smallChange              = Size.Ceiling(model.GridSize.Scale(MapScale));
      HorizontalScroll.SmallChange = smallChange.Width;
      VerticalScroll.SmallChange   = smallChange.Height;

      var largeChange              = Size.Round(ClientSize.Scale(0.75F));
      HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
      VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

      var size                     = Hexgrid.GetSize(MapSizePixels,MapScale);
      if (AutoScrollMinSize != size) {
        AutoScrollMinSize          = size;
        HorizontalScroll.Maximum   = Math.Min(1, Math.Max(1, Padding.Left + Padding.Right 
                                   + size.Width  - ClientSize.Width));
        VerticalScroll.Maximum     = Math.Min(1, Math.Max(1, Padding.Top + Padding.Bottom 
                                   + size.Height - ClientSize.Height));
        Invalidate();
      }
    }

    #region Grid Coordinates
    ///<inheritdoc/>
    protected Hexgrid    Hexgrid        { get {return Model.Hexgrid;} }
    /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
//    public SizeF   GridSizeF      { get; private set; } // { return Model.GridSize.Scale(MapScale); } }
    /// <summary>Gets the current Panel AutoScrollPosition.</summary>
    public Point   ScrollPosition { get { return AutoScrollPosition; } }

    CoordsRectangle GetClipCells(PointF point, SizeF size) {
      return Model.GetClipCells(point, size);
    }

    /// <summary>Returns, as a Rectangle, the IUserCoords for the currently visible extent.</summary>
    public virtual CoordsRectangle     VisibleRectangle {
      get { return GetClipCells( AutoScrollPosition.Scale(-1.0F/MapScale), 
                                      ClientSize.Scale(1.0F/MapScale) );
      }
    }

    /// <summary><c>HexCoords</c> for a selected hex.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <returns>Coordinates for a hex specified by a screen point.</returns>
    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
    public HexCoords GetHexCoords(Point point) {
      return Hexgrid.GetHexCoords(point, new Size(AutoScrollPosition));
    }
    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public Point HexCenterPoint(HexCoords coordsNewULHex) {
      return Hexgrid.HexCenterPoint(coordsNewULHex);
    }
    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    protected Point ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex) {
      return Hexgrid.ScrollPositionToCenterOnHex(coordsNewCenterHex,VisibleRectangle);
    }
    #endregion

    #region Painting
    /// <inheritdoc/>
    protected override void OnPaintBackground(PaintEventArgs e) { ; }
    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      if(IsHandleCreated) {
        var g = e.Graphics;
        var scroll = Hexgrid.GetScrollPosition(AutoScrollPosition);
        if (DesignMode) { g.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

        g.Clear(Color.Black);
        g.DrawRectangle(Pens.Black, ClientRectangle);

        if (IsTransposed) { g.Transform = TransposeMatrix; }
        g.TranslateTransform(scroll.X, scroll.Y);
        g.ScaleTransform(MapScale,MapScale);

        var state = g.Save();
        g.DrawImageUnscaled(MapBuffer, Point.Empty);

        g.Restore(state); state = g.Save();
        Model.PaintUnits(g);

        g.Restore(state); state = g.Save();
        Model.PaintHighlight(g);
      }
    }
    static readonly Matrix TransposeMatrix = new Matrix(0F,1F, 1F,0F, 0F,0F);
    #endregion

    #region Double-Buffering
    /// <summary>Gets or sets the backing buffer for the map underlay. Setting to null  forces a repaint.</summary>
    Bitmap MapBuffer     { 
      get { return _mapBuffer ?? ( _mapBuffer = PaintBuffer()); } 
      set { if (_mapBuffer!=null) _mapBuffer.Dispose(); _mapBuffer = value; }
    } Bitmap _mapBuffer;

    /// <summary>Service routine to paint the backing store bitmap for the map underlay.</summary>
    Bitmap PaintBuffer() {
      var size      = MapSizePixels;

      Bitmap buffer     = null;
      Bitmap tempBuffer = null;
      try {
        tempBuffer = new Bitmap(size.Width,size.Height, PixelFormat.Format32bppPArgb);
        using(var g = Graphics.FromImage(tempBuffer)) {
          g.Clear(Color.White);
          Model.PaintMap(g);
        }
        buffer     = tempBuffer;
        tempBuffer = null;
      } finally { if (tempBuffer!=null) tempBuffer.Dispose(); }
      return buffer;
    }
    #endregion

    #region Mouse & Scroll event handlers
    /// <inheritdoc/>
    protected override void OnMouseClick(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.Mouse.Trace(" - {0}.OnMouseClick - Shift: {1}; Ctl: {2}; Alt: {3}", 
                                      Name, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);
      var eventArgs = new HexEventArgs( GetHexCoords(e.Location), e, ModifierKeys);

           if (e.Button == MouseButtons.Middle)   base.OnMouseClick(eventArgs);
      else if (e.Button == MouseButtons.Right)    OnMouseRightClick(eventArgs);
      else if (IsAltKeyDown  && !IsCtlKeyDown)    OnMouseAltClick(eventArgs);
      else if (IsCtlKeyDown)                      OnMouseCtlClick(eventArgs);
      else                                        OnMouseLeftClick(eventArgs);
    }
    /// <inheritdoc/>
    protected override void OnMouseMove(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var newHex = GetHexCoords(e.Location);
      if ( newHex != HotspotHex)
        OnHotspotHexChange(new HexEventArgs(newHex));
      HotspotHex = newHex;

      base.OnMouseMove(e);
    }
    /// <summary>TODO</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual  void OnMouseHWheel(object sender, MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseHWheel: {1}", Model.Name, e.ToString());
        AutoScrollPosition = WheelPanel(HorizontalScroll,-e.Delta, ref scrollRemainderHorizontal,
              (delta) => new Point(-AutoScrollPosition.X + delta, -AutoScrollPosition.Y));
        OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack,
              IsShiftKeyDown ? AutoScrollPosition.X : AutoScrollPosition.Y));

    }
    /// <inheritdoc/>
    protected override void OnMouseWheel(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Model.Name, e.ToString());

      if( Control.ModifierKeys.HasFlag(Keys.Control)) {
        ScaleIndex += Math.Sign(e.Delta);  
      } else {
        var oldScrollPosition = AutoScrollPosition;
        AutoScrollPosition = IsShiftKeyDown
          ? WheelPanel(HorizontalScroll,-e.Delta, ref scrollRemainderHorizontal,
              (delta) => new Point(-AutoScrollPosition.X + delta, -AutoScrollPosition.Y))
          : WheelPanel(VerticalScroll,  -e.Delta, ref scrollRemainderVertical,
              (delta) => new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y + delta));
        OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack,
              IsShiftKeyDown ? -oldScrollPosition.X  : -oldScrollPosition.Y,
              IsShiftKeyDown ? -AutoScrollPosition.X : -AutoScrollPosition.Y,
              IsShiftKeyDown ? ScrollOrientation.HorizontalScroll : ScrollOrientation.VerticalScroll));
      }
    }
    int scrollRemainderHorizontal = 0;  //!< <summary>Unapplied horizontal scroll.</summary>
    int scrollRemainderVertical   = 0;  //!< <summary>Unapplied vertical scroll.</summary>
    /// <summary>Return new AutoScrollPosition for applied muse-wheel scroll.</summary>
    static Point WheelPanel(ScrollProperties scroll, int delta, ref int remainder,
      Func<int,Point> newAutoScroll)
    {
      if (Math.Sign(delta) != Math.Sign(remainder)) remainder = 0;
//      var  x = SystemInformation
      var steps = (delta+remainder) 
                / (SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines);
      remainder = (delta+remainder) 
                % (SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines);
      return newAutoScroll(scroll.SmallChange * steps);
    }

    /// <summary>Raise the MouseAltClick event.</summary>
    protected virtual void OnMouseAltClick(HexEventArgs e) { MouseAltClick.Raise(this,e);}
    /// <summary>Raise the MouseCtlClick event.</summary>
    protected virtual void OnMouseCtlClick(HexEventArgs e) { MouseCtlClick.Raise(this,e); }
    /// <summary>Raise the MouseLeftClic event.</summary>
    protected virtual void OnMouseLeftClick(HexEventArgs e) { MouseLeftClick.Raise(this,e); }
    /// <summary>Raise the MouseRightClick event.</summary>
    protected virtual void OnMouseRightClick(HexEventArgs e) { MouseRightClick.Raise(this,e); }
   /// <summary>Raise the HotspotHexChange event.</summary>
    protected virtual void OnHotspotHexChange(HexEventArgs e) { HotspotHexChange.Raise(this,e); }
    /// <summary>Raise the ScaleChange event.</summary>
    protected virtual void OnScaleChange(EventArgs e) { 
      ScaleChange.Raise(this,e); 
      Invalidate();
    }
    #endregion

    /// <summary>Service routine to execute a Panel scroll.</summary>
    public void ScrollPanel(ScrollEventType type, ScrollOrientation orientation, int sign) {
      if( sign != 0 ) {
        ScrollProperties  scroll  = VerticalScroll;
        Func<Point,Point> func;
        int               step    = 0;
        if( orientation == ScrollOrientation.VerticalScroll ) {
          scroll = VerticalScroll;
          func   = p => new Point(-p.X, -p.Y + step * sign);
        } else {
          scroll = HorizontalScroll;
          func   = p => new Point(-p.X + step * sign, -p.Y);
        }

        step = type.HasFlag(ScrollEventType.LargeIncrement) || type.HasFlag(ScrollEventType.LargeDecrement)
             ? scroll.LargeChange
             : scroll.SmallChange;
        AutoScrollPosition = func(AutoScrollPosition);
      }
    }
 
    #region IMessageFilter implementation
    /// <summary>Redirect WM_MouseWheel messages to window under mouse.</summary>
		/// <remarks>Redirect WM_MouseWheel messages to window under mouse (rather than 
    /// that with focus) with adjusted delta.
    /// <a href="http://www.flounder.com/virtual_screen_coordinates.htm">Virtual Screen Coordinates</a>
    /// Dont forget to add this to constructor:
    /// 			Application.AddMessageFilter(this);
    ///</remarks>
		/// <param name="m">The Windows Message to filter and/or process.</param>
		/// <returns>Success (true) or failure (false) to OS.</returns>
		[System.Security.Permissions.PermissionSetAttribute(
			System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		public bool PreFilterMessage(ref Message m) {
			var hWnd  = NativeMethods.WindowFromPoint( WindowsMouseInput.GetPointLParam(m.LParam) );
			var ctl	  = Control.FromHandle(hWnd);
      if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
//      if (hWnd != IntPtr.Zero   &&  ctl != null) {
        switch((WM)m.Msg) {
          default:  break;
          case WM.MOUSEHWHEEL:
            DebugTracing.Trace(TraceFlags.ScrollEvents, true," - {0}.WM.{1}: ", Name, ((WM)m.Msg)); 
            return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
          case WM.MOUSEWHEEL:
            DebugTracing.Trace(TraceFlags.ScrollEvents, true," - {0}.WM.{1}: ", Name, ((WM)m.Msg)); 
            return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
        }
      }
      return false;
		}
    #endregion
  }

  /// <summary>TODO</summary>
  public sealed class EmptyBoard : MapDisplay<MapGridHex> {
    /// <summary>TODO</summary>
    public EmptyBoard() : base(new Size(1,1), new Size(26,30), (mb,c) => new EmptyGridHex(mb,c)) {
      FovRadius = 20;
    }
  }

  /// <summary>TODO</summary>
  public sealed class EmptyGridHex : MapGridHex {
    /// <summary>TODO</summary>
    public EmptyGridHex(HexBoard<MapGridHex> board, HexCoords coords) : base(board, coords) {}

    /// <summary>TODO</summary>
    public override int ElevationASL  { get { return 10 * Elevation; } }
    /// <summary>TODO</summary>
    public override int HeightTerrain { get { return ElevationASL;   } }
    /// <summary>TODO</summary>
    public override int StepCost(Hexside direction) { return -1; }
  }
}
