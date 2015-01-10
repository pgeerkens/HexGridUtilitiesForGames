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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexgridScrollViewer {


  /// <summary>Sub-class implementation of a <b>WPF</b> Scrollable with integrated <see cref="TransposableHexgrid"/> support.</summary>
  public partial class HexgridScrollViewer : Canvas { //, IHexgridHost {
    /// <summary>Creates a new instance of HexgridScrollable.</summary>
    public HexgridScrollViewer() : base() {
//      InitializeComponent();
    }

    public event EventHandler<RenderingEventArgs> Render;

    protected override void OnRender(DrawingContext dc) {
      Render.Raise(this, new RenderingEventArgs(dc));
    }

    /// <summary>TODO</summary>
    /// <param name="scrollPosition"></param>
    public void SetScrollPosition(Point scrollPosition) {
//      ScrollInfo.SetHorizontalOffset(scrollPosition.X);
//      ScrollInfo.SetVerticalOffset(scrollPosition.Y);
    }

//    #region Events
//    /// <summary>Announces that the mouse is now over a new hex.</summary>
//    public event EventHandler<HexEventArgs> HotspotHexChange;
//    /// <summary>Announces occurrence of a mouse left-click with the <b>Alt</b> key depressed.</summary>
//    public event EventHandler<HexEventArgs> MouseAltClick;
//    /// <summary>Announces occurrence of a mouse left-click with the <b>Ctl</b> key depressed.</summary>
//    public event EventHandler<HexEventArgs> MouseCtlClick;
//    /// <summary>Announces a mouse left-click with no <i>shift</i> keys depressed.</summary>
//    public event EventHandler<HexEventArgs> MouseLeftClick;
//    /// <summary>Announces a mouse right-click. </summary>
//    public event EventHandler<HexEventArgs> MouseRightClick;
//    /// <summary>Announces a change of drawing scale on this HexgridPanel.</summary>
//    public event EventHandler<EventArgs>    ScaleChange;
//    #endregion

//    #region Properties
//    /// <summary>Mimic WinForms ScrollPosition to simpliify conversion.</summary>
//    private HexPoint ScrollPosition { 
//      get { return new HexPoint((int)HorizontalOffset,(int)VerticalOffset); } 
//      set { ScrollInfo.SetHorizontalOffset(value.X); ScrollInfo.SetVerticalOffset(value.Y); } 
//    }
//    /// <summary>Mimic WinForms ClientSize to simpliify conversion.</summary>
//    private HexSize  ClientSize         { get {return new HexSize((int)ViewportWidth,(int)ViewportHeight);} }

////    /// <summary>TODO</summary>
//    public HexgridViewModel        DataContext       { get; set; }
//    /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
//    public         Size            GridSizeF         { get { return DataContext.Model.GridSize.Scale(MapScale).ToSize().ToWpfSize(); } }
//    /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
//    public         HexCoords       HotspotHex        { get { return DataContext.HotspotHex; } }
//    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
//    public static  bool            IsAltKeyDown      { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Alt); } }
//    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
//    public static  bool            IsCtlKeyDown      { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control); } }
//    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
//    public static  bool            IsShiftKeyDown    { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift); } }
//    /// <summary>TODO</summary>
//    public         bool            IsMapDirty        { 
//      get { return _isMapDirty; }
//      set { 
//        _isMapDirty = value; 
//        if(_isMapDirty) { IsUnitsDirty = true; } 
//      }
//    } bool _isMapDirty;
//    /// <summary>TODO</summary>
//    public         bool            IsUnitsDirty      { 
//      get { return _isUnitsDirty; }
//      set { 
//        _isUnitsDirty = value; 
//        if(_isUnitsDirty) { InvalidateVisual(); }
//      }
//    } bool _isUnitsDirty;
//    /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
//    public         bool            IsTransposed      { 
//      get { return DataContext.IsTransposed; }
//      set { DataContext.IsTransposed = value;  SetScrollLimits(DataContext.Model); }
//    }
//    /// <inheritdoc/>
//    public         HexSize         MapSizePixels     { get { return DataContext.Model.MapSizePixels; } } // + MapMargin.Scale(2);} }
//    /// <summary>Current scaling factor for map display.</summary>
//    public         float           MapScale          { get { return DataContext.MapScale; } }
////    /// <summary>MapBoard hosting this panel.</summary>
////    public         IMapDisplay     Model             { get { return DataContext.Model; } }
//    /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
//    public         HexCoords       PanelCenterHex    { 
////      get { return GetHexCoords( Location + Size.Round(ClientSize.Scale(0.50F)) ); }
//      get { return GetHexCoords( this.PointFromScreen(new WpfPoint(ViewportWidth *0.50, ViewportHeight * 0.50)).ToHexPoint()); }
//    }
//    /// <summary>TODO</summary>
//    public WpfInput.ICommand       RefreshCmd        { get; private set; }
//    /// <summary>Index into <code>Scales</code> of current map scale.</summary>
//    public virtual int     ScaleIndex    { 
//      get { return _scaleIndex; }
//      set { var newValue = Math.Max(0, Math.Min(ScaleList.Count-1, value));
//            if( _scaleIndex != newValue) {
//              _scaleIndex = newValue;
////              MapScale    = ScaleList[ScaleIndex];

//              OnScaleChange(EventArgs.Empty); 
//            }
//          } 
//    } int _scaleIndex;

//    /// <summary>Array of supported map scales  as IList {float}.</summary>
//    public ReadOnlyCollection<float>     ScaleList        { get; private set; }
//    #endregion

//    /// <summary>Returns, as a Rectangle, the IUserCoords for the currently visible extent.</summary>
//    public virtual CoordsRectangle VisibleRectangle  {
//      get { return GetClipInHexes( ScrollPosition.Scale(-1.0F/MapScale).ToWpfPoint(), 
//                                         ClientSize.Scale( 1.0F/MapScale).ToWpfSize() );
//      }
//    }

//    public System.Windows.Media.Color BackColor { get; set; }

//    #region Methods
//    /// <summary>TODO</summary>
//    public         void CenterOnHex(HexCoords coords) {
//      ScrollPosition = ScrollPositionToCenterOnHex(coords);
////      IsMapDirty = true;
//      InvalidateScrollInfo();
//    }

//    /// <summary>TODO</summary>
//        CoordsRectangle GetClipInHexes(WpfPoint point, WpfSize size) { return DataContext.Model.GetClipInHexes(point, size); }
//    /// <summary><c>HexCoords</c> for a selected hex.</summary>
//    /// <param name="point">Screen point specifying hex to be identified.</param>
//    /// <returns>Coordinates for a hex specified by a screen point.</returns>
//    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
//    public    HexCoords GetHexCoords(HexPoint point) {
//      return DataContext.Hexgrid.GetHexCoords(point, new HexSize(ScrollPosition));
//    }
//    /// <summary>Force repaint of backing buffer for Map underlay.</summary>
//    public virtual void SetMapDirty() { InvalidateVisual(); }

//    /// <summary>TODO</summary>
//    public         void SetModel(IMapDisplayWpf model) {
//      SetScrollLimits(DataContext.Model);   
//      DataContext.SetModel(model);
//      SetMapDirty();
//    }

//    /// <summary>TODO</summary>
//    public         void SetPanelSize() {
////      if(DesignMode || !IsHandleCreated) return;
////      DebugTracing.Trace(TraceFlags.Sizing," - {0}.SetPanelSize; ClientSize = {1}", DataContext.Model.Name, ClientSize); 
//      SetScroll(PanelCenterHex);
//    }

//    /// <summary>Sets ScrollBars, then centres on <c>newCenterHex</c>.</summary>
//    public virtual void SetScroll(HexCoords newCenterHex) {
////      if(DesignMode || !IsHandleCreated) return;
////      DebugTracing.Trace(TraceFlags.Sizing," - {0}.SetPanelSize; Center Hex = {1}", DataContext.Model.Name, newCenterHex.ToString()); 

//      SetScrollLimits(DataContext.Model);

//      CenterOnHex(newCenterHex);
//    }

//    /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
//    public virtual void SetScrollLimits(IMapDisplayWpf model) {
////      if (model == null  ||  !AutoScroll) return;
//      //var smallChange              = HexSize.Ceiling(model.GridSize.Scale(MapScale));
//      //HorizontalScroll.SmallChange = smallChange.Width;
//      //VerticalScroll.SmallChange   = smallChange.Height;

//      //var largeChange              = HexSize.Round(ClientSize.Scale(0.75F));
//      //HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
//      //VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

//      //var size                     = DataContext.Hexgrid.GetSize(MapSizePixels,MapScale)
//      //                             + Margin.Size;
//      //if (AutoScrollMinSize != size) {
//      //  AutoScrollMinSize          = size;
//      //  HorizontalScroll.Maximum   = Math.Min(1, Math.Max(1, Margin.Horizontal 
//      //                             + size.Width  - ClientSize.Width));
//      //  VerticalScroll.Maximum     = Math.Min(1, Math.Max(1, Margin.Vertical 
//      //                             + size.Height - ClientSize.Height));
//      //  Invalidate();
//      //}
//    }
//    #endregion

//    #region Grid Coordinates
//    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
//    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
//    /// <returns>Pixel coordinates in Client reference frame.</returns>
//    public HexPoint HexCenterPoint(HexCoords coordsNewULHex) {
//      return DataContext.Hexgrid.HexCenterPoint(coordsNewULHex);
//    }
//    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
//    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
//    /// <returns>Pixel coordinates in Client reference frame.</returns>
//    protected HexPoint ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex) {
//      return DataContext.Hexgrid.ScrollPositionToCenterOnHex(coordsNewCenterHex,VisibleRectangle);
//    }
//    #endregion

//    #region Painting
//    /// <inheritdoc/>
////    protected override void OnPaintBackground(PaintEventArgs e) { ; }

//    /// <inheritdoc/>
//    //protected override void OnPaint(PaintEventArgs e) {
//    //  if (e==null) throw new ArgumentNullException("e");

//    //  if(!IsHandleCreated) return;
//    //  var dc      = e.Graphics;
//    //  if (DesignMode) { dc.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

//    //  dc.Clip = new Region(e.ClipRectangle);
//    //  if (IsTransposed) { dc.Transform = TransposeMatrix; }

//    //  var scroll = DataContext.Hexgrid.GetScrollPosition(ScrollPosition);
//    //  dc.TranslateTransform(scroll.X, scroll.Y);
//    //  dc.TranslateTransform(Margin.Left,Margin.Top);
//    //  dc.ScaleTransform(MapScale,MapScale);
//    //  TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, dc.VisibleClipBounds);

//    //  var state = dc.Save();
//    //  RenderMap(dc);

//    //  dc.Restore(state); state = dc.Save();
//    //  RenderUnits(dc);

//    //  dc.Restore(state); state = dc.Save();
//    //  RenderHighlight(dc);

//    //  dc.Restore(state);
//    //}
//    protected override void OnRender(DrawingContext dc) {
//      if (dc==null) throw new ArgumentNullException("dc");

////      dc.Clip = new Region(e.ClipRectangle);
//      if (IsTransposed) { dc.PushTransform(TransposeMatrix); }
//      OnRenderDetail (dc);
//      if (IsTransposed) dc.Pop();
//    }
//    protected virtual void OnRenderDetail(DrawingContext dc) {
//      if (dc==null) throw new ArgumentNullException("dc");

//      var scroll = DataContext.Hexgrid.GetScrollPosition(ScrollPosition);
//      dc.PushTransform(new TranslateTransform(scroll.X, scroll.Y));
//      dc.PushTransform(new TranslateTransform(Margin.Left,Margin.Top));
//      dc.PushTransform(new ScaleTransform(MapScale,MapScale));
////      TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, dc.VisibleClipBounds);

//      RenderMap(dc);
//      RenderUnits(dc);
//      RenderHighlight(dc);

//      dc.Pop();dc.Pop();dc.Pop();
//    }

//    /// <summary>TODO</summary>
//    protected virtual void RenderMap(DrawingContext dc) {
//      if (dc == null) throw new ArgumentNullException("dc");

//      var rectangle = new Rect(new WpfPoint(),new Vector(ActualWidth,ActualHeight));
//      var brush = new SolidColorBrush(this.BackColor);
//      dc.DrawRectangle(brush,null, rectangle);
//      DataContext.Model.PaintMap(dc);
//    }
//    /// <summary>TODO</summary>
//    protected virtual void RenderUnits(DrawingContext dc) {
//      if (dc == null) throw new ArgumentNullException("dc");
//      DataContext.Model.PaintUnits(dc);
//    }
//    /// <summary>TODO</summary>
//    protected virtual void RenderHighlight(DrawingContext dc) {
//      if (dc == null) throw new ArgumentNullException("dc");
//      DataContext.Model.PaintHighlight(dc);
//    }

//    /// <summary>TODO</summary>
//    static protected readonly Transform TransposeMatrix = new MatrixTransform(new Matrix(0F,1F, 1F,0F, 0F,0F));
//    #endregion

//    private ModifierKeys ModifierKeys(object o) {
//      return Keyboard.Modifiers;
//    }

//    #region Mouse event handlers
//    /// <inheritdoc/>
//    protected virtual void OnMouseClick(MouseEventArgs e) {
//      if (e==null) throw new ArgumentNullException("e");
//      //TraceFlags.Mouse.Trace(" - {0}.OnMouseClick - Shift: {1}; Ctl: {2}; Alt: {3}", 
//      //                                Name, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);
//      var coords    = GetHexCoords(Mouse.GetPosition(this).ToHexPoint());
//      // TODO - remap this in ViewModel
//      //var eventArgs = new HexEventArgs(coords, e, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);

//      //     if (e.Button == MouseButtons.Middle)   base.OnMouseClick(eventArgs);
//      //else if (e.Button == MouseButtons.Right)    this.OnMouseRightClick(eventArgs);
//      //else if (IsAltKeyDown  && !IsCtlKeyDown)    this.OnMouseAltClick(eventArgs);
//      //else if (IsCtlKeyDown)                      this.OnMouseCtlClick(eventArgs);
//      //else                                        this.OnMouseLeftClick(eventArgs);
//    }
//    /// <inheritdoc/>
//    protected override void OnMouseMove(MouseEventArgs e) {
//      if (e==null) throw new ArgumentNullException("e");
//      var coords    = GetHexCoords(Mouse.GetPosition(this).ToHexPoint());
//      // TODO - remap this in ViewModel
////      OnHotspotHexChange(new HexEventArgs(GetHexCoords(e.Location - Margin.OffsetSize())));

//      base.OnMouseMove(e);
//    }

//    /// <summary>Raise the MouseAltClick event.</summary>
//    protected virtual void OnMouseAltClick(HexEventArgs e) { MouseAltClick.Raise(this,e); }
//    /// <summary>Raise the MouseCtlClick event.</summary>
//    protected virtual void OnMouseCtlClick(HexEventArgs e) {
//      if (e==null) throw new ArgumentNullException("e");
//      DataContext.Model.GoalHex = e.Coords;
//      MouseCtlClick.Raise(this,e);
////      Refresh();
//    }
//    /// <summary>Raise the MouseLeftClick event.</summary>
//    protected virtual void OnMouseLeftClick(HexEventArgs e) {
//      if (e==null) throw new ArgumentNullException("e");
//      DataContext.Model.StartHex = e.Coords;
//      MouseLeftClick.Raise(this,e);
////      Refresh();
//    }
//    /// <summary>Raise the MouseRightClick event.</summary>
//    protected virtual void OnMouseRightClick(HexEventArgs e) { MouseRightClick.Raise(this,e); }
//   /// <summary>Raise the HotspotHexChange event.</summary>
//    protected virtual void OnHotspotHexChange(HexEventArgs e) {
//      if (e==null) throw new ArgumentNullException("e");
//      DataContext.Model.HotspotHex = e.Coords;
//      HotspotHexChange.Raise(this,e);
////      Refresh();
//    }

//    /// <summary>Raise the ScaleChange event.</summary>
//    protected virtual void OnScaleChange(EventArgs e) {
//      SetMapDirty();
//      OnRenderSizeChanged(new SizeChangedInfo(this,new WpfSize(),true,true));
//      InvalidateVisual();
//      ScaleChange.Raise(this, e);
//    }

//    /// <inheritdoc/>
//    protected override void OnRenderSizeChanged(SizeChangedInfo info) {
//      SetScrollLimits(DataContext.Model);
//      base.OnRenderSizeChanged(info);
//    }
//    #endregion

//    #region MouseWheel & Scroll event handlers
//    ///// <inheritdoc/>
//    //protected virtual void OnMouseWheel(MouseEventArgs e) {
//    //  if (e == null) throw new ArgumentNullException("e");
//    //  TraceFlags.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Name, e.ToString());

//    //  //if (Control.ModifierKeys.HasFlag(Keys.Control)) ScaleIndex += Math.Sign(e.Delta);
//    //  //else if (IsShiftKeyDown)                        base.OnMouseHWheel(e);
//    //  //else                                            base.OnMouseWheel(e);
//    //}

//    /// <summary>TODO</summary>
//    public void ScrollPanelVertical(bool isPage, int sign) {
//      ScrollPanelCommon(false, isPage, sign);
//    }
//    /// <summary>TODO</summary>
//    public void ScrollPanelHorizontal(bool isPage, int sign) {
//      ScrollPanelCommon(true, isPage, sign);
//    }
//    /// <summary>TODO</summary>
//    private void ScrollPanelCommon(bool  isHorizontal, bool isPage, int sign) {
//      if (sign == 0) return;
//      switch (sign) {
//        case -1: if(isPage) if(isHorizontal) PageLeft(); else PageUp(); else if(isHorizontal) LineLeft(); else LineUp(); break;
//        case +1: if(isPage) if(isHorizontal) PageRight(); else PageDown(); else if(isHorizontal) LineRight(); else LineDown(); break;
//        default: break;
//      }
//    }
//    #endregion

//    /// <summary>Array of supported map scales  as IList {float}.</summary>
//    public ReadOnlyCollection<float>     Scales        { get; private set; }
//    public void SetScales (IList<float> scales) { Scales = new ReadOnlyCollection<float>(scales); }
  }

  public class RenderingEventArgs : RoutedEventArgs {
    public RenderingEventArgs(DrawingContext dc) : base() {
      DrawingContext = dc;
    }
    public DrawingContext DrawingContext { get; private set; }
  }
}

namespace PGNapoleonics.HexgridScrollViewer {
  using HexPointF = System.Drawing.PointF;
  using HexSizeF = System.Drawing.SizeF;

  using MapGridHex      = Hex<DrawingContext,StreamGeometry>;

  public class HexgridScrollViewerViewModel : ViewModelBase {
    public HexgridScrollViewerViewModel() : base("HexgridScrollViewer Test (WPF)") {
      View.Render += HexgridScrollViewer_Rendering;
    }

    #region Properties
    public System.Windows.Media.Color BackColor { get; set; }
    /// <summary>Mimic WinForms ScrollPosition to simpliify conversion.</summary>
    private Point ScrollPosition { 
      get { return new Point(0,0); }//View.HorizontalOffset,View.VerticalOffset); } 
      set { View.SetScrollPosition(value); } 
    }
    /// <summary>MapBoard hosting this panel.</summary>
    public MapDisplay<MapGridHex> Model    { 
      get { return _model; }
      set {  if (_model != null) _model.Dispose();  _model = value;  }
    } MapDisplay<MapGridHex> _model = new EmptyBoard();

    /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
    public        HexCoords HotspotHex     { get; private set; }

    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsAltKeyDown   { get {return Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);} }
    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsCtlKeyDown   { get {return Keyboard.Modifiers.HasFlag(ModifierKeys.Control);} }
    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
    protected static  bool  IsShiftKeyDown { get { return Keyboard.Modifiers.HasFlag(ModifierKeys.Shift); } }

    /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
    public     bool         IsTransposed  { 
      get { return Model.IsTransposed; }
      set { Model.IsTransposed = value;  } //SetScrollLimits(Model); }
    }

    /// <inheritdoc/>
    public     Size         MapSizePixels { get {return MapSizePixels;} }

    /// <summary>Current scaling factor for map display.</summary>
    public     float        MapScale      { 
      get { return Model.MapScale; } 
      private set { Model.MapScale = value; } // SetScrollLimits(Model); } 
    }

    /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
    public     HexCoords    PanelCenterHex  { 
      get { return GetHexCoords( View.PointFromScreen(new Point(View.Width*0.50, View.Height*0.50)) ); }
    }

    /// <summary>Index into <code>Scales</code> of current map scale.</summary>
    public virtual int      ScaleIndex    { 
      get { return _scaleIndex; }
      set { var newValue = Math.Max(0, Math.Min(ScaleList.Count-1, value));
            if( _scaleIndex != newValue) {
              _scaleIndex = newValue;
              MapScale    = ScaleList[ScaleIndex];

//              OnScaleChange(EventArgs.Empty); 
            }
          } 
    } int _scaleIndex;

    /// <summary>Array of supported map scales  as IList {float}.</summary>
    public IList<float>     ScaleList        { get; private set; }

    private HexgridScrollViewer View { get; set; }
    #endregion
  
    #region Grid Coordinates
    /// <inheritdoc/>
    protected IHexgrid    Hexgrid         { get {return Model.Hexgrid;} }
    /// <summary><c>HexCoords</c> for a selected hex.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <returns>Coordinates for a hex specified by a screen point.</returns>
    /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
    public HexCoords GetHexCoords(Point point) {
      return Hexgrid.GetHexCoords(new HexPointF((float)point.X,(float)point.Y), 
                                  new HexSizeF((float)ScrollPosition.X,(float)ScrollPosition.Y));
    }
    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public Point HexCenterPoint(HexCoords coordsNewULHex) {
      var point = Hexgrid.HexCenterPoint(coordsNewULHex);
      return new Point(point.X,point.Y);
    }
    #endregion

    #region Painting
    /// <summary>TODO</summary>
    /// <param name="dc"></param>
    protected void HexgridScrollViewer_Rendering(object sender, RenderingEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");

      var dc = e.DrawingContext;
//      dc.Clip = new Region(e.ClipRectangle);
      if (IsTransposed) { dc.PushTransform(TransposeMatrix); }
      OnRenderViewDetail (dc);
      if (IsTransposed) dc.Pop();
      View.InvalidateVisual();
    }
    /// <summary>TODO</summary>
    /// <param name="dc"></param>
    protected virtual void OnRenderViewDetail(DrawingContext dc) {
      if (dc==null) throw new ArgumentNullException("dc");

      Point scroll; // = Hexgrid.GetScrollPosition(ScrollPosition);
      if (IsTransposed) scroll = ScrollPosition; 
      else scroll = new Point(ScrollPosition.Y, ScrollPosition.X);
      dc.PushTransform(new TranslateTransform(scroll.X, scroll.Y));
      dc.PushTransform(new TranslateTransform(View.Margin.Left,View.Margin.Top));
      dc.PushTransform(new ScaleTransform(MapScale,MapScale));
//      TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, dc.VisibleClipBounds);

      RenderMap(dc);
      RenderUnits(dc);
      RenderHighlight(dc);

      dc.Pop();dc.Pop();dc.Pop();
    }
    /// <summary>TODO</summary>
    protected virtual void RenderMap(DrawingContext dc) {
      if (dc == null) throw new ArgumentNullException("dc");

      var rectangle = new Rect(new Point(),new Vector(View.ActualWidth,View.ActualHeight));
      var brush = new SolidColorBrush(BackColor);
      dc.DrawRectangle(brush,null, rectangle);
      Model.PaintMap(dc);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderUnits(DrawingContext dc) {
      if (dc == null) throw new ArgumentNullException("dc");
      Model.PaintUnits(dc);
    }
    /// <summary>TODO</summary>
    protected virtual void RenderHighlight(DrawingContext dc) {
      if (dc == null) throw new ArgumentNullException("dc");
      Model.PaintHighlight(dc);
    }

    static readonly Transform TransposeMatrix = new MatrixTransform(new Matrix(0F,1F, 1F,0F, 0F,0F));
    #endregion
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
