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

namespace PGNapoleonics.HexgridPanel {
  /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="Hexgrid"/> support.</summary>
  [DockingAttribute(DockingBehavior.AutoDock)]
  public partial class HexgridScrollable : ScrollableControl, ISupportInitialize, IHexgridHost, IMessageFilter {
    /// <summary>Creates a new instance of HexgridPanel.</summary>
    public HexgridScrollable() {
      InitializeComponent();
    }
    /// <summary>Creates a new instance of HexgridPanel.</summary>
    public HexgridScrollable(IContainer container) {
      if (container==null) throw new ArgumentNullException("container");
      container.Add(this);

      InitializeComponent();
    }

    #region ISupportInitialize implementation
    /// <summary>Signals the object that initialization is starting.</summary>
    public virtual void BeginInit() { 
      MapMargin = new System.Drawing.Size(5,5);
      Scales    = new List<float>() {1.000F}.AsReadOnly();
    }
    /// <summary>Signals the object that initialization is complete.</summary>
    public virtual void EndInit() { 
      HotspotHex = HexCoords.EmptyUser;
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

    BufferedGraphicsContext _bufferedGraphicsContext = new BufferedGraphicsContext();

    #region Properties
    /// <summary>MapBoard hosting this panel.</summary>
    public IMapDisplay Host            {
      get { return _host; }
      set { _host = value; SetMapDirty(); }
    } IMapDisplay _host;

    /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
    public HexCoords   HotspotHex      { get; private set; }

    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsAltKeyDown   { get {return ModifierKeys.HasFlag(Keys.Alt);} }
    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsCtlKeyDown   { get {return ModifierKeys.HasFlag(Keys.Control);} }
    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsShiftKeyDown { get {return ModifierKeys.HasFlag(Keys.Shift);} }

    /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
    public bool        IsTransposed    { 
      get { return _isTransposed; }
      set { //if (_isTransposed == value) return;
            _isTransposed = value;  
            Hexgrid       = IsTransposed ? new TransposedHexgrid(this) 
                                         : new Hexgrid(this);
//            ResizeBuffer();
            if (IsHandleCreated) SetScrollLimits();   
          }
    } bool _isTransposed;

    /// <summary>Margin of map in pixels.</summary>
    public Size        MapMargin       { get; private set; }

    /// <inheritdoc/>
    public Size        MapSizePixels   { get {return Host.MapSizePixels + MapMargin.Scale(2);} }

    /// <summary>Current scaling factor for map display.</summary>
    public float       MapScale        { get { return Scales[ScaleIndex]; } }

    /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
    public HexCoords   PanelCenterHex  { 
      get { return GetHexCoords( Location + Size.Round(ClientSize.Scale(0.50F)) ); }
    }

    /// <summary>Index into <code>Scales</code> of current map scale.</summary>
    public virtual int ScaleIndex      { 
      get { return _scaleIndex; }
      set { var newValue = Math.Max(0, Math.Min(Scales.Count-1, value));
            if( _scaleIndex != newValue) {
              _scaleIndex = newValue; 
              OnScaleChange(EventArgs.Empty); 
            }
      } 
    } int _scaleIndex;

    /// <summary>Array of supported map scales  as IList&lt;float&gt;.</summary>
    public ReadOnlyCollection<float> Scales        { get; set; }
    #endregion

    /// <summary>Set property Scales (array of supported map scales as IList&lt;float&gt;.</summary>
    [Obsolete("Use Property Setter 'Scales' instead.")]
    public void SetScaleList(ReadOnlyCollection<float> scales) { Scales = scales; }

    /// <summary>Force repaint of backing buffer for Map underlay.</summary>
    //public void SetMapDirty() { if (MapBuffer!=null) MapBuffer.Dispose(); MapBuffer = null; }
    public void SetMapDirty() { 
      if (MapBuffer!=null) PaintBuffer(DisplayRectangle); 
      Invalidate(ClientRectangle); 
    }

    /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
    public virtual void SetScrollLimits() {
      var smallChange              = Size.Ceiling(Host.GridSize.Scale(MapScale));
      HorizontalScroll.SmallChange = smallChange.Width;
      VerticalScroll.SmallChange   = smallChange.Height;

      var largeChange              = Size.Round(ClientSize.Scale(0.75F));
      HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
      VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

      var size                     = Hexgrid.Size;
      if (AutoScrollMinSize != size) {
        AutoScrollMinSize          = size;
        HorizontalScroll.Maximum   = Math.Min(1, Math.Max(1, size.Width  - ClientSize.Width));
        VerticalScroll.Maximum     = Math.Min(1, Math.Max(1, size.Height - ClientSize.Height));
        Invalidate();
      }
    }

    #region Grid Coordinates
    ///<inheritdoc/>
    protected Hexgrid    Hexgrid        { get; private set; }
    Size    IHexgridHost.ClientSize     { get { return ClientSize; } }
    /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
    public SizeF   GridSizeF      { get { return Host.GridSize.Scale(MapScale); } }
    /// <summary>Gets the current Panel AutoScrollPosition.</summary>
    public Point   ScrollPosition { get { return AutoScrollPosition; } }

    CoordsRectangle GetClipCells(PointF point, SizeF size) {
      return Host.GetClipCells(point, size);
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
      return Hexgrid.ScrollPositionToCenterOnHex(coordsNewCenterHex);
    }
    #endregion

    #region Painting
    /// <summary>Internal handler for the OnPaintBackground event.</summary>
    protected override void OnPaintBackground(PaintEventArgs e) { ; }

    /// <summary>Internal handler for the OnPaint event.</summary>
    protected override void OnPaint(PaintEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");

//      base.OnPaint(e);
      if(IsHandleCreated) { 
        PaintPanel(e); 
      }
    }
    /// <summary>TODO</summary>
    /// <param name="e"></param>
    /// <deprecated>Use OnPaint(PaintEventArgs) instead.</deprecated>/>
    [Obsolete("Use OnPaint(PaintEventArgs) instead.")]
    protected virtual void PaintPanel(PaintEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");

      var g      = e.Graphics;
      if (DesignMode) { g.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

      g.Clip = new Region(e.ClipRectangle);
      if (IsTransposed) { g.Transform = TransposeMatrix; }

      var scroll = Hexgrid.ScrollPosition;  // scroll position on the transposed HexGrid
      g.TranslateTransform(scroll.X, scroll.Y);
      g.ScaleTransform(MapScale,MapScale);
      TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, g.VisibleClipBounds);

      var state = g.Save();
      Render(g, AutoScrollPosition);

      g.Restore(state); state = g.Save();
      Host.PaintUnits(g);

      g.Restore(state); state = g.Save();
      Host.PaintHighlight(g);

      g.Restore(state);
    }
    static readonly Matrix TransposeMatrix = new Matrix(0F,1F, 1F,0F, 0F,0F);
    #endregion

    /// <summary>TODO</summary>
    /// <param name="target"></param>
    /// <param name="scrollPosition"></param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    public void Render(Graphics target, Point scrollPosition) {
      if (target != null) {
        IntPtr targetDC = target.GetHdc();
 
        try { RenderInternal(new HandleRef(target, targetDC), MapBuffer, scrollPosition); } 
        finally { target.ReleaseHdcInternal(targetDC);  }
      }
    }
    /// <summary>TODO</summary>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
    private void RenderInternal(HandleRef refTargetDC, BufferedGraphics buffer, Point scrollPosition) {
      const int  rop = GdiRasterOps.SrcCopy; // 0xcc0020; // RasterOp.SOURCE.GetRop();

      var sourceDC    = buffer.Graphics.GetHdc(); 
      var virtualSize = DisplayRectangle.Size;
      try { 
        NativeMethods.BitBlt(refTargetDC, scrollPosition.X,  scrollPosition.Y, 
                                          virtualSize.Width, virtualSize.Height, 
                            new HandleRef(buffer.Graphics, sourceDC), 0, 0, rop);
      } 
      finally { buffer.Graphics.ReleaseHdcInternal(sourceDC); }
    } 

    /// <inheritdoc/>
    protected override void OnResize(EventArgs e) {
      ResizeBuffer();
      base.OnResize(e);
    }

    #region Double-Buffering
    /// <summary>Gets or sets the backing buffer for the map underlay. Setting to null  forces a repaint.</summary>
    protected BufferedGraphics MapBuffer { get; set; }

    /// <summary>Service routine to paint the backing store bitmap for the map underlay.</summary>
    protected virtual void PaintBuffer(Rectangle clipBounds) {
      if (Host==null  ||  MapBuffer==null) return;

      var g = MapBuffer.Graphics;
      if (g != null) {
        var state = g.Save();
        g.Clip = new Region(clipBounds);
        if (IsTransposed) { g.Transform = TransposeMatrix; }

        g.ScaleTransform(MapScale,MapScale);
        TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, g.VisibleClipBounds);

        g.Clear(this.BackColor);
        Host.PaintMap(g);
        g.Restore(state);
      }
    }

    void ResizeBuffer() {
      var rectangle = new Rectangle(Point.Empty, DisplayRectangle.Size);
      if (DisplayRectangle.Size != _bufferedGraphicsContext.MaximumBuffer) {
        _bufferedGraphicsContext.MaximumBuffer = DisplayRectangle.Size;
        if (MapBuffer != null) MapBuffer.Dispose();
        MapBuffer = _bufferedGraphicsContext.Allocate(
          this.CreateGraphics(), rectangle);
      }
      PaintBuffer(rectangle);
    }
    #endregion

    #region Mouse & Scroll events
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
    /// <inheritdoc/>
    protected override void OnMouseWheel(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Host.Name, e.ToString()); 
      var oldValue    = IsShiftKeyDown ? AutoScrollPosition.X
                                       : AutoScrollPosition.Y;
      base.OnMouseWheel(e);
      if( Control.ModifierKeys.HasFlag(Keys.Control)) {
        ScaleIndex += Math.Sign(e.Delta);  
        Invalidate();
      } else {
        const ScrollEventType scrollType = ScrollEventType.ThumbPosition;
        var orientation = IsShiftKeyDown ? ScrollOrientation.HorizontalScroll
                                         : ScrollOrientation.VerticalScroll;
        var newValue    = IsShiftKeyDown ? AutoScrollPosition.X
                                         : AutoScrollPosition.Y;
        OnScroll(new ScrollEventArgs(scrollType, oldValue, newValue, orientation));
      }
    }

    /// <summary>Internal handler for the MouseAltClick event.</summary>
    protected virtual void OnMouseAltClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var handler = MouseAltClick;
      if( handler != null ) handler(this, e);
    }
    /// <summary>Internal handler for the MouseCtlClick event.</summary>
    protected virtual void OnMouseCtlClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var handler = MouseCtlClick;
      if( handler != null ) handler(this, e);
    }
    /// <summary>Internal handler for the MouseLeftClic event.</summary>
    protected virtual void OnMouseLeftClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var handler = MouseLeftClick;
      if( handler != null ) handler(this, e);
    }
    /// <summary>Internal handler for the MouseRightClick event.</summary>
    protected virtual void OnMouseRightClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var handler = MouseRightClick;
      if( handler != null ) handler(this, e);
    }

    /// <summary>Internal handler for the HotspotHexChange event.</summary>
    protected virtual void OnHotspotHexChange(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var handler = HotspotHexChange;
      if( handler != null ) handler(this, e);
    }
    #endregion

    /// <summary>Internal handler for the SclaeChange event.</summary>
    protected virtual void OnScaleChange(EventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var handler = ScaleChange;
      if( handler != null ) handler(this, e);
//      SetScrollLimits();
      ResizeBuffer();
    }

    /// <summary>Service routine to execute a Panel scroll from keyboard.</summary>
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
        AutoScrollPosition = func(AutoScrollPosition, sign, step);
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
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			var hWnd  = NativeMethods.WindowFromPoint( WindowsMouseInput.GetPointLParam(m.LParam) );
			var ctl	  = Control.FromHandle(hWnd);
      //if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
      if (hWnd != IntPtr.Zero  &&  ctl != null) {
        switch((WM)m.Msg) {
          default:  break;
          case WM.MOUSEWHEEL:
          case WM.MOUSEHWHEEL:
            DebugTracing.Trace(TraceFlags.ScrollEvents, true," - {0}.WM.{1}: ", Name, ((WM)m.Msg)); 
            //if (! IsCtlKeyDown  &&  IsShiftKeyDown)
            //  m.Msg = (int)WM.MOUSEHWHEEL;

            return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
        }
      }
      return false;
		}
    #endregion

    /// <summary>TODO</summary>
    protected override void WndProc(ref System.Windows.Forms.Message m)  {
        base.WndProc(ref m);

        const int WM_MOUSEHWHEEL = 0x020E;
        if (m.Msg == WM_MOUSEHWHEEL)
        {
            m.Result = new IntPtr((((int)m.WParam >> 16) & 0xFFFF) / 120);
        }
    }
  }

  #pragma warning disable 1570,1591
  /// <summary>TODO</summary>
  public enum ScrollBarCommand {
      SB_LINEUP           = 0,  ///< TODO
      SB_LINELEFT         = 0,  ///< TODO
      SB_LINEDOWN         = 1,  ///< TODO
      SB_LINERIGHT        = 1,  ///< TODO
      SB_PAGEUP           = 2,  ///< TODO
      SB_PAGELEFT         = 2,  ///< TODO
      SB_PAGEDOWN         = 3,  ///< TODO
      SB_PAGERIGHT        = 3,  ///< TODO
      SB_THUMBPOSITION    = 4,  ///< TODO
      SB_THUMBTRACK       = 5,  ///< TODO
      SB_TOP              = 6,  ///< TODO
      SB_LEFT             = 6,  ///< TODO
      SB_BOTTOM           = 7,  ///< TODO
      SB_RIGHT            = 7,  ///< TODO
      SB_ENDSCROLL        = 8   ///< TODO
  }
  #pragma warning restore 1570,1591
  internal static partial class GdiRasterOps {
    public const int SrcCopy                 = 0x00CC0020; /* dest = source                   */ 
    public const int SrcPaint                = 0x00EE0086; /* dest = source OR dest           */
    public const int SrcAnd                  = 0x008800C6; /* dest = source AND dest          */
    public const int SrcInvert               = 0x00660046; /* dest = source XOR dest          */
    public const int SrcErase                = 0x00440328; /* dest = source AND (NOT dest )   */ 
    public const int NotSrcCopy              = 0x00330008; /* dest = (NOT source)             */
    public const int NotSrcErase             = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */ 
    public const int MergeCopy               = 0x00C000CA; /* dest = (source AND pattern)     */ 
    public const int MergePaint              = 0x00BB0226; /* dest = (NOT source) OR dest     */
    public const int PatCopy                 = 0x00F00021; /* dest = pattern                  */ 
    public const int PatPaint                = 0x00FB0A09; /* dest = DPSnoo                   */
    public const int PatInvert               = 0x005A0049; /* dest = pattern XOR dest         */
    public const int DstInvert               = 0x00550009; /* dest = (NOT dest)               */
    public const int Blackness               = 0x00000042; /* dest = BLACK                    */ 
    public const int Whiteness               = 0x00FF0062; /* dest = WHITE                    */
//    public const int CaptureBlt              = 0x40000000; /* Include layered windows */ 
  }
}
