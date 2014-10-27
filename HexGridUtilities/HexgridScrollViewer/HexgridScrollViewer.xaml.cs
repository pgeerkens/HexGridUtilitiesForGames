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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexgridScrollViewer {
  using WpfInput  = System.Windows.Input;

  using HexPoint  = System.Drawing.Point;
  using HexSize   = System.Drawing.Size;
  using WpfPoint  = System.Windows.Point;
  using WpfSize   = System.Windows.Size;

  /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="Hexgrid"/> support.</summary>
  public partial class HexgridScrollViewer : TiltAwareScrollViewer, IHexgridHost {
    /// <summary>Creates a new instance of HexgridScrollable.</summary>
    protected HexgridScrollViewer() : base() {
      RefreshCmd  = new RelayCommand(o => { if (o != null) { SetMapDirty(); }  UpdateLayout(); } );
      DataContext = new HexgridViewModel(this);
      SetScales (new float[] {1.00F});

//      InitializeComponent();
    }

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
    /// <summary>Mimic WinForms AutoScrollPosition to simpliify conversion.</summary>
    private HexPoint AutoScrollPosition { 
      get { return new HexPoint((int)HorizontalOffset,(int)VerticalOffset); } 
      set { ScrollInfo.SetHorizontalOffset(value.X); ScrollInfo.SetVerticalOffset(value.Y); } 
    }
    /// <summary>Mimic WinForms ClientSize to simpliify conversion.</summary>
    private HexSize  ClientSize         { get {return new HexSize((int)ViewportWidth,(int)ViewportHeight);} }

    /// <summary>TODO</summary>
    public HexgridViewModel        DataContext       { get; set; }
    /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
    public         Size            GridSizeF         { get { return DataContext.Model.GridSize.Scale(MapScale).ToSize().ToWpfSize(); } }
    /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
    public         HexCoords       HotspotHex        { get { return DataContext.HotspotHex; } }
    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
    public static  bool            IsAltKeyDown      { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Alt); } }
    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
    public static  bool            IsCtlKeyDown      { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control); } }
    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
    public static  bool            IsShiftKeyDown    { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift); } }
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
        if(_isUnitsDirty) { InvalidateVisual(); }
      }
    } bool _isUnitsDirty;
    /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
    public         bool            IsTransposed      { 
      get { return DataContext.IsTransposed; }
      set { DataContext.IsTransposed = value;  SetScrollLimits(DataContext.Model); }
    }
    /// <inheritdoc/>
    public         HexSize         MapSizePixels     { get { return DataContext.Model.MapSizePixels; } } // + MapMargin.Scale(2);} }
    /// <summary>Current scaling factor for map display.</summary>
    public         float           MapScale          { get { return DataContext.MapScale; } }
//    /// <summary>MapBoard hosting this panel.</summary>
//    public         IMapDisplay     Model             { get { return DataContext.Model; } }
    /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
    public         HexCoords       PanelCenterHex    { 
//      get { return GetHexCoords( Location + Size.Round(ClientSize.Scale(0.50F)) ); }
      get { return GetHexCoords( this.PointFromScreen(new WpfPoint(ViewportWidth *0.50, ViewportHeight * 0.50)).ToHexPoint()); }
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
      get { return GetClipCells( AutoScrollPosition.Scale(-1.0F/MapScale).ToWpfPoint(), 
                                         ClientSize.Scale( 1.0F/MapScale).ToWpfSize() );
      }
    }

    public System.Windows.Media.Color BackColor { get; set; }
    #endregion

    #region Methods
    /// <summary>TODO</summary>
    public         void CenterOnHex(HexCoords coords) {
      AutoScrollPosition = ScrollPositionToCenterOnHex(coords);
//      IsMapDirty = true;
      InvalidateScrollInfo();
    }

    /// <summary>TODO</summary>
        CoordsRectangle GetClipCells(WpfPoint point, WpfSize size) { return DataContext.Model.GetClipCells(point, size); }
    /// <summary><c>HexCoords</c> for a selected hex.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <returns>Coordinates for a hex specified by a screen point.</returns>
    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
    public    HexCoords GetHexCoords(HexPoint point) {
      return DataContext.Hexgrid.GetHexCoords(point, new HexSize(AutoScrollPosition));
    }
    /// <summary>Force repaint of backing buffer for Map underlay.</summary>
    public virtual void SetMapDirty() { InvalidateVisual(); }

    /// <summary>TODO</summary>
    public         void SetModel(IMapDisplayWpf model) {
      SetScrollLimits(DataContext.Model);   
      DataContext.SetModel(model);
      SetMapDirty();
    }

    /// <summary>TODO</summary>
    public         void SetPanelSize() {
//      if(DesignMode || !IsHandleCreated) return;
      DebugTracing.Trace(TraceFlags.Sizing," - {0}.SetPanelSize; ClientSize = {1}", DataContext.Model.Name, ClientSize); 
      SetScroll(PanelCenterHex);
    }

    /// <summary>Sets ScrollBars, then centres on <c>newCenterHex</c>.</summary>
    public virtual void SetScroll(HexCoords newCenterHex) {
//      if(DesignMode || !IsHandleCreated) return;
      DebugTracing.Trace(TraceFlags.Sizing," - {0}.SetPanelSize; Center Hex = {1}", DataContext.Model.Name, newCenterHex.ToString()); 

      SetScrollLimits(DataContext.Model);

      CenterOnHex(newCenterHex);
    }

    /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
    public virtual void SetScrollLimits(IMapDisplayWpf model) {
//      if (model == null  ||  !AutoScroll) return;
      //var smallChange              = HexSize.Ceiling(model.GridSize.Scale(MapScale));
      //HorizontalScroll.SmallChange = smallChange.Width;
      //VerticalScroll.SmallChange   = smallChange.Height;

      //var largeChange              = HexSize.Round(ClientSize.Scale(0.75F));
      //HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
      //VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

      //var size                     = DataContext.Hexgrid.GetSize(MapSizePixels,MapScale)
      //                             + Margin.Size;
      //if (AutoScrollMinSize != size) {
      //  AutoScrollMinSize          = size;
      //  HorizontalScroll.Maximum   = Math.Min(1, Math.Max(1, Margin.Horizontal 
      //                             + size.Width  - ClientSize.Width));
      //  VerticalScroll.Maximum     = Math.Min(1, Math.Max(1, Margin.Vertical 
      //                             + size.Height - ClientSize.Height));
      //  Invalidate();
      //}
    }
    #endregion

    #region Grid Coordinates
    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public HexPoint HexCenterPoint(HexCoords coordsNewULHex) {
      return DataContext.Hexgrid.HexCenterPoint(coordsNewULHex);
    }
    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    protected HexPoint ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex) {
      return DataContext.Hexgrid.ScrollPositionToCenterOnHex(coordsNewCenterHex,VisibleRectangle);
    }
    #endregion

    #region Painting
    /// <inheritdoc/>
//    protected override void OnPaintBackground(PaintEventArgs e) { ; }

    /// <inheritdoc/>
    //protected override void OnPaint(PaintEventArgs e) {
    //  if (e==null) throw new ArgumentNullException("e");

    //  if(!IsHandleCreated) return;
    //  var g      = e.Graphics;
    //  if (DesignMode) { g.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

    //  g.Clip = new Region(e.ClipRectangle);
    //  if (IsTransposed) { g.Transform = TransposeMatrix; }

    //  var scroll = DataContext.Hexgrid.GetScrollPosition(AutoScrollPosition);
    //  g.TranslateTransform(scroll.X, scroll.Y);
    //  g.TranslateTransform(Margin.Left,Margin.Top);
    //  g.ScaleTransform(MapScale,MapScale);
    //  TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, g.VisibleClipBounds);

    //  var state = g.Save();
    //  RenderMap(g);

    //  g.Restore(state); state = g.Save();
    //  RenderUnits(g);

    //  g.Restore(state); state = g.Save();
    //  RenderHighlight(g);

    //  g.Restore(state);
    //}
    protected override void OnRender(DrawingContext g) {
      if (g==null) throw new ArgumentNullException("g");

//      g.Clip = new Region(e.ClipRectangle);
      if (IsTransposed) { g.PushTransform(TransposeMatrix); }
      OnRenderDetail (g);
      if (IsTransposed) g.Pop();
    }
    protected virtual void OnRenderDetail(DrawingContext g) {
      if (g==null) throw new ArgumentNullException("g");

      var scroll = DataContext.Hexgrid.GetScrollPosition(AutoScrollPosition);
      g.PushTransform(new TranslateTransform(scroll.X, scroll.Y));
      g.PushTransform(new TranslateTransform(Margin.Left,Margin.Top));
      g.PushTransform(new ScaleTransform(MapScale,MapScale));
//      TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, g.VisibleClipBounds);

      RenderMap(g);
      RenderUnits(g);
      RenderHighlight(g);

      g.Pop();g.Pop();g.Pop();
    }

    /// <summary>TODO</summary>
    protected virtual void RenderMap(DrawingContext g) {
      if (g == null) throw new ArgumentNullException("g");

      var rectangle = new Rect(new WpfPoint(),new Vector(ActualWidth,ActualHeight));
      var brush = new SolidColorBrush(this.BackColor);
      g.DrawRectangle(brush,null, rectangle);
      DataContext.Model.PaintMap(g);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderUnits(DrawingContext g) {
      if (g == null) throw new ArgumentNullException("g");
      DataContext.Model.PaintUnits(g);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderHighlight(DrawingContext g) {
      if (g == null) throw new ArgumentNullException("g");
      DataContext.Model.PaintHighlight(g);
    }

    /// <summary>TODO</summary>
    static protected readonly Transform TransposeMatrix = new MatrixTransform(new Matrix(0F,1F, 1F,0F, 0F,0F));
    #endregion

    private ModifierKeys ModifierKeys(object o) {
      return Keyboard.Modifiers;
    }

    #region Mouse event handlers
    /// <inheritdoc/>
    protected virtual void OnMouseClick(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      TraceFlags.Mouse.Trace(" - {0}.OnMouseClick - Shift: {1}; Ctl: {2}; Alt: {3}", 
                                      Name, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);
      var coords    = GetHexCoords(Mouse.GetPosition(this).ToHexPoint());
      // TODO - remap this in ViewModel
      //var eventArgs = new HexEventArgs(coords, e, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);

      //     if (e.Button == MouseButtons.Middle)   base.OnMouseClick(eventArgs);
      //else if (e.Button == MouseButtons.Right)    this.OnMouseRightClick(eventArgs);
      //else if (IsAltKeyDown  && !IsCtlKeyDown)    this.OnMouseAltClick(eventArgs);
      //else if (IsCtlKeyDown)                      this.OnMouseCtlClick(eventArgs);
      //else                                        this.OnMouseLeftClick(eventArgs);
    }
    /// <inheritdoc/>
    protected override void OnMouseMove(MouseEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      var coords    = GetHexCoords(Mouse.GetPosition(this).ToHexPoint());
      // TODO - remap this in ViewModel
//      OnHotspotHexChange(new HexEventArgs(GetHexCoords(e.Location - Margin.OffsetSize())));

      base.OnMouseMove(e);
    }

    /// <summary>Raise the MouseAltClick event.</summary>
    protected virtual void OnMouseAltClick(HexEventArgs e) { MouseAltClick.Raise(this,e); }
    /// <summary>Raise the MouseCtlClick event.</summary>
    protected virtual void OnMouseCtlClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      DataContext.Model.GoalHex = e.Coords;
      MouseCtlClick.Raise(this,e);
//      Refresh();
    }
    /// <summary>Raise the MouseLeftClick event.</summary>
    protected virtual void OnMouseLeftClick(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      DataContext.Model.StartHex = e.Coords;
      MouseLeftClick.Raise(this,e);
//      Refresh();
    }
    /// <summary>Raise the MouseRightClick event.</summary>
    protected virtual void OnMouseRightClick(HexEventArgs e) { MouseRightClick.Raise(this,e); }
   /// <summary>Raise the HotspotHexChange event.</summary>
    protected virtual void OnHotspotHexChange(HexEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      DataContext.Model.HotspotHex = e.Coords;
      HotspotHexChange.Raise(this,e);
//      Refresh();
    }

    /// <summary>Raise the ScaleChange event.</summary>
    protected virtual void OnScaleChange(EventArgs e) {
      SetMapDirty();
      OnRenderSizeChanged(new SizeChangedInfo(this,new WpfSize(),true,true));
      InvalidateVisual();
      ScaleChange.Raise(this, e);
    }

    /// <inheritdoc/>
    protected override void OnRenderSizeChanged(SizeChangedInfo info) {
      SetScrollLimits(DataContext.Model);
      base.OnRenderSizeChanged(info);
    }
    #endregion

    #region MouseWheel & Scroll event handlers
    ///// <inheritdoc/>
    //protected virtual void OnMouseWheel(MouseEventArgs e) {
    //  if (e == null) throw new ArgumentNullException("e");
    //  TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Name, e.ToString());

    //  //if (Control.ModifierKeys.HasFlag(Keys.Control)) ScaleIndex += Math.Sign(e.Delta);
    //  //else if (IsShiftKeyDown)                        base.OnMouseHWheel(e);
    //  //else                                            base.OnMouseWheel(e);
    //}

    /// <summary>TODO</summary>
    public void ScrollPanelVertical(bool isPage, int sign) {
      ScrollPanelCommon(false, isPage, sign);
    }
    /// <summary>TODO</summary>
    public void ScrollPanelHorizontal(bool isPage, int sign) {
      ScrollPanelCommon(true, isPage, sign);
    }
    /// <summary>TODO</summary>
    private void ScrollPanelCommon(bool  isHorizontal, bool isPage, int sign) {
      if (sign == 0) return;
      switch (sign) {
        case -1: if(isPage) if(isHorizontal) PageLeft(); else PageUp(); else if(isHorizontal) LineLeft(); else LineUp(); break;
        case +1: if(isPage) if(isHorizontal) PageRight(); else PageDown(); else if(isHorizontal) LineRight(); else LineDown(); break;
        default: break;
      }
    }
    #endregion

    /// <summary>Array of supported map scales  as IList&lt;float&gt;.</summary>
    public ReadOnlyCollection<float>     Scales        { get; private set; }
    public void SetScales (IList<float> scales) { Scales = new ReadOnlyCollection<float>(scales); }
  }
}

namespace PGNapoleonics.HexgridScrollViewer {
using System.Windows.Forms;
  public partial class TiltAwareScrollViewer : ScrollViewer {
    /// <summary>TODO</summary>
    private static int MouseWheelStep {
      get {
        return SystemInformation.MouseWheelScrollDelta
             / SystemInformation.MouseWheelScrollLines;
      }
    }
  }
}
