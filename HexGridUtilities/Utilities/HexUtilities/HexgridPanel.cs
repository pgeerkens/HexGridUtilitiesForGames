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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PG_Napoleonics.Utilities.HexUtilities;
using PG_Napoleonics.Utilities.WinForms;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface IMapDisplay {
    Size   GridSize      { get; }
    Size   MapSizePixels { get; }
    string Name          { get; }   // only used in DebugTracing calls

    UserCoordsRectangle GetClipCells(PointF point, SizeF size);
    UserCoordsRectangle GetClipCells(RectangleF visibleClipBounds);

    void  PaintHighlight(Graphics g);
    void  PaintMap(Graphics g);
    void  PaintUnits(Graphics g);
  }

  public partial class HexgridPanel : Panel, ISupportInitialize {
    public HexgridPanel() {
      InitializeComponent();
    }
    public HexgridPanel(IContainer container) {
      container.Add(this);

      InitializeComponent();
    }

    #region ISupportInitialize implementation
    public virtual void BeginInit() { 
      MapMargin = new System.Drawing.Size(5,5);
      Scales    = new float[] {0.707F, 1.000F, 1.414F};
    }
    public virtual void EndInit() { 
      this.MakeDoubleBuffered(true);
    }
    #endregion

    public event EventHandler<HexEventArgs> MouseAltClick;
    public event EventHandler<HexEventArgs> MouseCtlClick;
    public event EventHandler<HexEventArgs> MouseLeftClick;
    public event EventHandler<HexEventArgs> MouseRightClick;
    public event EventHandler<HexEventArgs> HotSpotHexChange;
    public event EventHandler<EventArgs>    ScaleChange;

    /// <summary>MapBoard hosting this panel.</summary>
    public IMapDisplay Host          {
      get { return _host; }
      set { _host = value; MapBuffer = null; }
    } IMapDisplay _host;

    public ICoordsUser HotSpotHex    { get; private set; }

    public bool        IsTransposed  { 
      get { return _isTransposed; }
      set { _isTransposed = value; if (IsHandleCreated) SetScroll(); }
    } bool _isTransposed;

    /// <summary>Margin of map in pixels.</summary>
    public Size        MapMargin     { get; private set; }

    /// <summary>Size of map in pixels.</summary>
    public Size        MapSizePixels { get {return Host.MapSizePixels + MapMargin.Scale(2);} }

    /// <summary>Current scaling factor for map display.</summary>
    public float       MapScale      { get { return DesignMode ? 1 : Scales[ScaleIndex]; } }

    /// <summary>Returns <code>ICoordsUser</code> of the hex closest to the center of the current viewport.</summary>
    public ICoordsUser PanelCenterHex { 
      get { return GetHexCoords( Location + Size.Round(ClientSize.Scale(0.50F)) ).User; }
    }

    /// <summary>Index into <code>Scales</code> of current map scale.</summary>
    public virtual int ScaleIndex    { 
      get { return _scaleIndex; }
      set { var newValue = Math.Max(0, Math.Min(Scales.Length-1, value));
            if( _scaleIndex != newValue) {
              _scaleIndex = newValue; 
              OnScaleChange(EventArgs.Empty); 
            }
      } 
    } int _scaleIndex;

    /// <summary>Array of supported map scales (float).</summary>
    public float[]     Scales        { 
      get {return _scales;}
      set {_scales = value; if (_scaleIndex!=0) ScaleIndex = _scaleIndex;}
    } float[] _scales;

    /// <summary>Returns, as a Rectangle, the IUserCoords for the currently visible extent.</summary>
    public UserCoordsRectangle     VisibleRectangle {
      get { return Host.GetClipCells( AutoScrollPosition.Scale(-1.0F/MapScale), 
                                      ClientSize.Scale(1.0F/MapScale) );
      }
    }

    /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
    public virtual void SetScroll() {
      var smallChange              = Size.Ceiling(Host.GridSize.Scale(MapScale));
      HorizontalScroll.SmallChange = smallChange.Width;
      VerticalScroll.SmallChange   = smallChange.Height;

      var largeChange              = Size.Round(ClientSize.Scale(0.75F));
      HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
      VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

      var size                     = TransposeSize(Size.Ceiling(MapSizePixels.Scale(MapScale)));
      if (AutoScrollMinSize != size) {
        AutoScrollMinSize          = size;
        HorizontalScroll.Maximum   = Math.Min(1, Math.Max(1, size.Width  - ClientSize.Width));
        VerticalScroll.Maximum     = Math.Min(1, Math.Max(1, size.Height - ClientSize.Height));
        Invalidate();
      }
    }

    #region Grid Coordinates
    /// <summary>Scaled <code>Size</code> of each hexagon in grid, being the 'full-width' and 'full-height'.</summary>
    SizeF GridSize { get { return Host.GridSize.Scale(MapScale); } }

    /// Mathemagically (left as exercise for the reader) our 'picking' matrices are these, assuming: 
    ///  - origin at upper-left corner of hex (0,0);
    ///  - 'straight' hex-axis vertically down; and
    ///  - 'oblique'  hex-axis up-and-to-right (at 120 degrees from 'straight').
    private Matrix matrixX { 
      get { return new Matrix((3.0F/2.0F)/GridSize.Width,  (3.0F/2.0F)/GridSize.Width,
                                     1.0F/GridSize.Height,       -1.0F/GridSize.Height,  -0.5F,-0.5F); } 
    }
    private Matrix matrixY { 
      get { return new Matrix(       0.0F,                 (3.0F/2.0F)/GridSize.Width,
                                    2.0F/GridSize.Height,         1.0F/GridSize.Height,  -0.5F,-0.5F); } 
    }

    /// <summary>Canonical coordinates for a selected hex.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <returns>Canonical coordinates for a hex specified by a screen point.</returns>
    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
    public ICoordsCanon GetHexCoords(Point point) {
      return GetHexCoords(point, new Size(AutoScrollPosition));
    }
    /// <summary>Canonical coordinates for a selected hex for a given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    /// <returns>Canonical coordinates for a hex specified by a screen point.</returns>
    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
    protected ICoordsCanon GetHexCoords(Point point, Size autoScroll) {
      if( Host == null ) return HexCoords.EmptyCanon;

      autoScroll = TransposeSize(autoScroll);

      // Adjust for origin not as assumed by GetCoordinate().
      var grid    = new Size((int)(GridSize.Width*2F/3F), (int)GridSize.Height);
      var margin  = new Size((int)(MapMargin.Width *MapScale), 
                             (int)(MapMargin.Height*MapScale));
      point      -= autoScroll + margin + grid;

      return HexCoords.NewCanonCoords( GetCoordinate(matrixX, point), 
                                    GetCoordinate(matrixY, point) );
    }

    /// <summary>Calculates a (canonical X or Y) grid-coordinate for a point, from the supplied 'picking' matrix.</summary>
    /// <param name="matrix">The 'picking' matrix</param>
    /// <param name="point">The screen point identifying the hex to be 'picked'.</param>
    /// <returns>A (canonical X or Y) grid coordinate of the 'picked' hex.</returns>
	  private static int GetCoordinate (Matrix matrix, Point point){
      var pts = new Point[] {point};
      matrix.TransformPoints(pts);
		  return (int) Math.Floor( (pts[0].X + pts[0].Y + 2F) / 3F );
	  }

    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex">ICoordsUser for the hex to be centered in viewport.</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    protected Point ScrollPositionToCenterOnHex(ICoordsUser coordsNewCenterHex) {
      return HexCenterPoint(HexCoords.NewUserCoords(
                coordsNewCenterHex.Vector - ( new IntVector2D(VisibleRectangle.Size.Vector) / 2 )
      ));
    }

    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex">User (ie rectangular) hex-coordinates for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public Point HexCenterPoint(ICoordsUser coordsNewULHex) {
      if (coordsNewULHex == null) return new Point();
      var offset = new Size((int)(GridSize.Width*2F/3F), (int)GridSize.Height);
      var margin = Size.Round( MapMargin.Scale(MapScale) );
      return TransposePoint(HexOrigin(GridSize,coordsNewULHex) + margin + offset);
    }
    Point HexOrigin(SizeF gridSize, ICoordsUser coords) {
      return new Point(
        (int)(GridSize.Width  * coords.X),
        (int)(GridSize.Height * coords.Y   + GridSize.Height/2 * (coords.X+1)%2)
      );
    }
    #endregion

    #region Painting
    protected override void OnPaintBackground(PaintEventArgs e) { ; }
    protected override void OnPaint(PaintEventArgs e) {
      base.OnPaint(e);
      if(IsHandleCreated)    PaintPanel(e.Graphics);
    }
    protected virtual void PaintPanel(Graphics g) {
      var scroll = TransposePoint(AutoScrollPosition);
      if (DesignMode) { g.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

      g.Clear(Color.White);
      g.DrawRectangle(Pens.Black, ClientRectangle);

      if (IsTransposed) { g.Transform = new Matrix(0F,1F, 1F,0F, 0F,0F); }
      g.TranslateTransform(scroll.X, scroll.Y);
      g.ScaleTransform(MapScale,MapScale);

      var state = g.Save();
      g.DrawImageUnscaled(MapBuffer, Point.Empty);

      g.Restore(state); state = g.Save();
      g.TranslateTransform(MapMargin.Width, MapMargin.Height);
      Host.PaintUnits(g);

      g.Restore(state); state = g.Save();
      g.TranslateTransform(MapMargin.Width, MapMargin.Height);
      Host.PaintHighlight(g);
    }
    #endregion

    #region Double-Buffering
    Bitmap MapBuffer     { 
      get { return _mapBuffer ?? ( _mapBuffer = PaintBuffer()); } 
      set { if (_mapBuffer!=null) _mapBuffer.Dispose(); _mapBuffer = value; }
    } Bitmap _mapBuffer;
    Bitmap PaintBuffer() {
      var size   = MapSizePixels;
      var buffer = new Bitmap(size.Width,size.Height, PixelFormat.Format32bppPArgb);
      using(var g = Graphics.FromImage(buffer)) {
        g.Clear(Color.White);
        g.TranslateTransform(MapMargin.Width, MapMargin.Height);
        Host.PaintMap(g);
      }
      return buffer;
    }
    #endregion

    protected         bool        IsAltKeyDown   { get {return ModifierKeys.HasFlag(Keys.Alt);} }
    protected         bool        IsCtlKeyDown   { get {return ModifierKeys.HasFlag(Keys.Control);} }
    protected         bool        IsShiftKeyDown { get {return ModifierKeys.HasFlag(Keys.Shift);} }

    Point TransposePoint(Point point) { return IsTransposed ? new Point(point.Y,point.X) : point; }
    Size  TransposeSize(Size  size)   { return IsTransposed ? new Size (size.Height, size.Width) : size; }

    protected override void OnMouseClick(MouseEventArgs e) {
      TraceFlag.Mouse.Trace(" - {0}.OnMouseClick - Shift: {1}; Ctl: {2}; Alt: {3}", 
                                      Name, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);

      var eventArgs = new HexEventArgs( GetHexCoords(TransposePoint(e.Location)).User, e, ModifierKeys);

           if (e.Button == MouseButtons.Middle)   base.OnMouseClick(eventArgs);
      else if (e.Button == MouseButtons.Right)    OnMouseRightClick(eventArgs);
      else if (IsAltKeyDown  && !IsCtlKeyDown)    OnMouseAltClick(eventArgs);
      else if (IsCtlKeyDown)                      OnMouseCtlClick(eventArgs);
      else                                        OnMouseLeftClick(eventArgs);
    }
    protected override void OnMouseMove(MouseEventArgs e) {
      var newHex = GetHexCoords(TransposePoint(e.Location)).User;
      if ( ! newHex.Equals(HotSpotHex))
        OnHotSpotHexChange(new HexEventArgs(newHex));
      HotSpotHex = newHex;

      base.OnMouseMove(e);
    }
    protected override void OnMouseWheel(MouseEventArgs e) {
      TraceFlag.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Host.Name, e.ToString()); 
      if( Control.ModifierKeys.HasFlag(Keys.Control)) {
        ScaleIndex += Math.Sign(e.Delta);  
      } else {
        var orientation = IsShiftKeyDown ? ScrollOrientation.HorizontalScroll
                                         : ScrollOrientation.VerticalScroll;
        WheelPanel(orientation, -e.Delta);
      }
      Invalidate();
    }

    protected virtual void OnMouseAltClick(HexEventArgs e) {
      var handler = MouseAltClick;
      if( handler != null ) handler(this, e);
    }
    protected virtual void OnMouseCtlClick(HexEventArgs e) {
      var handler = MouseCtlClick;
      if( handler != null ) handler(this, e);
    }
    protected virtual void OnMouseLeftClick(HexEventArgs e) {
      var handler = MouseLeftClick;
      if( handler != null ) handler(this, e);
    }
    protected virtual void OnMouseRightClick(HexEventArgs e) {
      var handler = MouseRightClick;
      if( handler != null ) handler(this, e);
    }
    protected virtual void OnHotSpotHexChange(HexEventArgs e) {
      var handler = HotSpotHexChange;
      if( handler != null ) handler(this, e);
    }
    protected virtual void OnScaleChange(EventArgs e) {
      var handler = ScaleChange;
      if( handler != null ) handler(this, e);
    }

    int mouseWheelHorizontalRemainder = 0;
    int mouseWheelVerticalRemainder   = 0;
    void WheelPanel(ScrollOrientation orientation, int delta) {
      if( orientation == ScrollOrientation.VerticalScroll )
        mouseWheelVerticalRemainder   = WheelPanelDetail(orientation, VerticalScroll,  delta,
            (p,amount) => new Point(-p.X, -p.Y + amount), mouseWheelVerticalRemainder);
      else
        mouseWheelHorizontalRemainder = WheelPanelDetail(orientation, HorizontalScroll,delta,
            (p,amount) => new Point(-p.X + amount, -p.Y), mouseWheelHorizontalRemainder);
    }
    int WheelPanelDetail(ScrollOrientation orientation, ScrollProperties scroll, int delta,
      Func<Point,int,Point> newAutoScroll, int remainder) {
      const int mouseWheelDelta = 120;

      var scrollLines = SystemInformation.MouseWheelScrollLines;
      var steps       = Math.Sign(delta) * ( (Math.Abs(delta+remainder) * scrollLines) / mouseWheelDelta );
      remainder       = Math.Sign(delta) * ( (Math.Abs(delta+remainder) * scrollLines) % mouseWheelDelta );

      if (steps != 0) {
        var oldValue = scroll.Value;
        AutoScrollPosition = newAutoScroll(AutoScrollPosition, scroll.SmallChange * steps);
        OnScroll( new ScrollEventArgs(ScrollEventType.ThumbPosition, oldValue, scroll.Value, orientation) );
      }
      return remainder;
    }
    public void ScrollPanel(ScrollEventType type, ScrollOrientation orientation, int sign) {
      if( sign != 0 ) {
        ScrollProperties          scroll;
        Func<Point,int,int,Point> func;
        if( orientation == ScrollOrientation.VerticalScroll ) {
          scroll = VerticalScroll;
          func   = (p,sgn,stp) => new Point(-p.X, -p.Y + stp * sgn);
        } else {
          scroll = HorizontalScroll;
          func   = (p,sgn,stp) => new Point(-p.X + stp * sgn, -p.Y);
        }

        var step = type.HasFlag(ScrollEventType.LargeIncrement) || type.HasFlag(ScrollEventType.LargeDecrement)
                 ? scroll.LargeChange
                 : scroll.SmallChange;
        var oldValue = scroll.Value;
        AutoScrollPosition = func(AutoScrollPosition, sign, step);
        OnScroll( new ScrollEventArgs(type, oldValue, scroll.Value, orientation) );
      }
    }
  }
}
