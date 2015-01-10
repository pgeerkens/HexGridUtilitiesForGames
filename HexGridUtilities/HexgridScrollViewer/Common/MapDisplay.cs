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
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.FieldOfView;

#pragma warning disable 1587
/// <summary>WinForms-specific utilities, including implementation of the subclasses HexgridPanel
/// and MapDisplay<THex>, utilizing the System.Windows.Forms technology.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridScrollViewer {
  using HexSize    = System.Drawing.Size;
  using HexPoint   = System.Drawing.Point;
  using HexRectF   = System.Drawing.RectangleF;
  using MapGridHex = Hex<DrawingContext,StreamGeometry>;

  using Int32ValueEventArgs = ValueChangedEventArgs<Int32>;

  /// <summary>Abstract class representing the basic game board.</summary>
  /// <typeparam name="THex">Type of the hex for which a game board is desired.</typeparam>
  public abstract class MapDisplay<THex> : HexBoard<THex,StreamGeometry>, IHexBoard<THex>, IMapDisplayWpf
    where THex : MapGridHex {

    /// <summary>TODO</summary>
    public delegate THex InitializeHex(StreamGeometry hexgridPath, HexCoords coords);

    /// <summary>TODO</summary>
    private static StreamGeometry GetGraphicsPath(HexSize gridSize) {
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.EvenOdd;

      using (var context = geometry.Open()) {
        context.BeginFigure(new Point(gridSize.Width*1/3,                0),false,true);
        context.LineTo     (new Point(gridSize.Width*3/3,                0),true,false);
        context.LineTo     (new Point(gridSize.Width*4/3,gridSize.Height/2),true,false);
        context.LineTo     (new Point(gridSize.Width*3/3,gridSize.Height  ),true,false);
        context.LineTo     (new Point(gridSize.Width*1/3,gridSize.Height  ),true,false);
        context.LineTo     (new Point(                 0,gridSize.Height/2),true,false);
      }
      geometry.Freeze();

      return geometry;
    }

    #region Constructors
    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    protected MapDisplay(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex) 
    : this(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes)) {}

    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    protected MapDisplay(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex, IFastList<HexCoords> landmarkCoords) 
    : base(sizeHexes, gridSize, landmarkCoords, GetGraphicsPath
      #if FlatBoardStorage
        ,() => new FlatBoardStorage<THex>(sizeHexes, coords => initializeHex(GetGraphicsPath(gridSize),coords))
      #else
        ,() => new BlockedBoardStorage32x32<THex>(sizeHexes, coords => initializeHex(GetGraphicsPath(gridSize),coords))
      #endif
    ) {
      InitializeProperties();
    }

    void InitializeProperties() {
      GoalHex         = 
      HotspotHex      = 
      StartHex        = HexCoords.EmptyUser;
      ShadeBrushAlpha = 78;
      ShadeBrushColor = Colors.Black;
      ShowFov         = true;
      ShowHexgrid     = true;
      ShowPath        = true;
      ShowPathArrow   = true;
    }
    #endregion

    #region Properties
    /// <summary>Gets or sets the Field-of-View for the current <see cref="HotspotHex"/>, as an <see cref="IFov"/> object.</summary>
    public virtual  IFov          Fov             {
      get { return _fov ?? (_fov = this.GetFieldOfView(ShowRangeLine ? StartHex : HotspotHex)); }
      protected set { _fov = value; }
    } IFov _fov;
    /// <inheritdoc/>
    public override int           FovRadius       { set { RangeCutoff = base.FovRadius = value; Fov = null; } }
    /// <inheritdoc/>
    public virtual  HexCoords     GoalHex         { 
      get { return _goalHex; }
      set { _goalHex=value; _path = null; } 
    } HexCoords _goalHex = HexCoords.EmptyUser;
    /// <inheritdoc/>
    public virtual  HexCoords     HotspotHex      { 
      get { return _hotSpotHex; }
      set { if (IsOnboard(value)) _hotSpotHex = value; if (!ShowRangeLine) _fov = null; }
    } HexCoords _hotSpotHex = HexCoords.EmptyUser;
    /// <inheritdoc/>
    public          int           LandmarkToShow  { get; set; }
    /// <inheritdoc/>
    public          string        Name            { get {return "MapDisplay";} }
    /// <inheritdoc/>
    public          IDirectedPathCollection Path            { 
      get { return _path ?? (_path = this.GetDirectedPath(this[StartHex], this[GoalHex])); } 
    } IDirectedPathCollection _path;
    /// <summary>Gets or sets the alpha component for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
    public          byte          ShadeBrushAlpha { get; set; }
    /// <summary>Gets or sets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
    public          Color         ShadeBrushColor { get; set; }
    /// <summary>Gets or sets whether to display the FIeld-of-View for <see cref="HotspotHex"/>.</summary>
    public          bool          ShowFov         { get; set; }
    /// <summary>Gets or sets whether to display the hexgrid.</summary>
    public          bool          ShowHexgrid     { get; set; }
    /// <summary>Gets or sets whether to display the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
    public          bool          ShowPath        { get; set; }
    /// <summary>Gets or sets whether to display direction indicators for the current path.</summary>
    public          bool          ShowPathArrow   { get; set; }
    /// <summary>Gets or sets whether to display the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
    public          bool          ShowRangeLine   { 
      get { return _showRangeLine; } 
      set { _showRangeLine = value; if (_showRangeLine) Fov = null; }
    } bool _showRangeLine = false;
    /// <inheritdoc/>
    public virtual  HexCoords     StartHex        { 
      get { return _startHex; }
      set { if (IsOnboard(value)) _startHex = value; _path = null; if (ShowRangeLine) _fov = null; } 
    } HexCoords _startHex = HexCoords.EmptyUser;
    #endregion

    #region Painting
    /// <inheritdoc/>
    public CoordsRectangle GetClipInHexes(Point point, Size size) {
      return GetClipInHexes( new HexRectF(point.ToHexPoint(),size.ToHexSize()), MapSizeHexes );
    }

    /// <inheritdoc/>
    public CoordsRectangle GetClipInHexes(HexRectF visibleClipBounds) {
      return GetClipInHexes(visibleClipBounds, MapSizeHexes);
    }

    /// <inheritdoc/>
    public    virtual  void PaintHighlight(DrawingContext graphics) { 
      if (graphics==null) throw new ArgumentNullException("dc");
      var brushBlack = Brushes.Black;

      graphics.PushTransform(TranslateToHex(StartHex));
      graphics.DrawGeometry(Brushes.Transparent,new Pen(brushBlack,1),HexgridPath);
      graphics.Pop();

      //dc.DrawPath(Pens.Red, HexgridPath);

      //if (ShowPath) {
      //  dc.EndContainer(container); container = dc.BeginContainer();
      //  PaintPath(dc,Path);
      //}

      //if (ShowRangeLine) {
      //  dc.EndContainer(container); container = dc.BeginContainer();
      //  var target = CentreOfHex(HotspotHex);
      //  dc.DrawLine(Pens.Red, CentreOfHex(StartHex), target);
      //  dc.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
      //  dc.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
      //}

      //if (ShowFov) {
      //  dc.EndContainer(container); container = dc.BeginContainer();
      //  var clipHexes  = GetClipInHexes(dc.VisibleClipBounds);
      //  using(var shadeBrush = new SolidBrush(Color.FromArgb(ShadeBrushAlpha, ShadeBrushColor))) {
      //    PaintForEachHex(dc, clipHexes, coords => {
      //      if (Fov!=null && ! Fov[coords]) { dc.FillPath(shadeBrush, HexgridPath);  }
      //    } );
      //  }
      //}
    }

    /// <inheritdoc/>
    public    virtual  void PaintMap(DrawingContext graphics) { 
      if (graphics==null) throw new ArgumentNullException("graphics");

//      dc.InterpolationMode = InterpolationMode.HighQualityBicubic;
      var clipHexes = GetClipInHexes(HexPoint.Empty.ToWpfPoint(),MapSizeHexes.ToWpfSize());

      var fontSize   = SystemFonts.MenuFontSize * 0.8F;
      var brush      = Brushes.Black;
      var textOffset = new HexPoint(GridSize.Scale(0.50F).ToSize()).ToWpfPoint()
                     - new Vector(fontSize,fontSize);
      PaintForEachHex(graphics, clipHexes, coords => {
        this[coords].Paint(graphics);
        if (ShowHexgrid) graphics.DrawGeometry(Brushes.Transparent,new Pen(Brushes.Black,1), HexgridPath);
        if (LandmarkToShow > 0) {
          graphics.DrawText(new FormattedText(
              LandmarkDistance(coords,LandmarkToShow-1),
              CultureInfo.GetCultureInfo("en-US"),
              FlowDirection.LeftToRight,
              new Typeface("Verdana"),
              fontSize,
              Brushes.Black),
            textOffset);
        }
      } );
      graphics.Pop();
    }

    /// <summary>Paint the current shortese path.</summary>
    /// <param name="dc">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPath"/> - 
    /// A directed path (ie linked-list> of hexes to be painted.</param>
    protected virtual  void PaintPath(DrawingContext graphics, IDirectedPathCollection path) {
      if (graphics==null) throw new ArgumentNullException("graphics");

      var pen   = new Pen(Brushes.Black,1.0F);
      var brush = new SolidColorBrush(Color.FromArgb(78, Colors.PaleGoldenrod.R,Colors.PaleGoldenrod.G,Colors.PaleGoldenrod.B));
      while (path != null) {
        var coords = path.PathStep.Hex.Coords;
        graphics.PushTransform(TranslateToHex(StartHex));
        graphics.DrawGeometry(brush,pen,HexgridPath);

        if (ShowPathArrow) PaintPathArrow(graphics, path);

        path = path.PathSoFar;
      }
    }

    /// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
    /// <param name="dc">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPath"/> - 
    /// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
    protected virtual  void PaintPathArrow(DrawingContext graphics, IDirectedPathCollection path) {
      if (graphics==null) throw new ArgumentNullException("graphics");
      if (path==null) throw new ArgumentNullException("path");

      graphics.PushTransform(new TranslateTransform(CentreOfHexOffset.Width, CentreOfHexOffset.Height));
      if (path.PathSoFar == null)    PaintPathDestination(graphics);
      else                           PaintPathArrow(graphics, path.PathStep.HexsideEntry);
      graphics.Pop();
    }

    /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
    /// <param name="dc">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="hexside">Type: <see cref="Hexside"/> - 
    /// Direction from this hex in which the next step is made.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    protected virtual  void PaintPathArrow(DrawingContext graphics, Hexside hexside) {
      if (graphics==null) throw new ArgumentNullException("graphics");

      var penBlack = new Pen(Brushes.Black,1.0F);
      var unit = GridSize.Height/8.0F;
      graphics.PushTransform(new RotateTransform(60 * (int)hexside));
      graphics.DrawLine(penBlack, new Point(0,unit*4), new Point(      0,  -unit));
      graphics.DrawLine(penBlack, new Point(0,unit*4), new Point(-unit*3/2, unit*2));
      graphics.DrawLine(penBlack, new Point(0,unit*4), new Point( unit*3/2, unit*2));
      graphics.Pop();
    }

    /// <summary>Paint the destination indicator for the current shortest path.</summary>
    /// <param name="dc">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    protected virtual  void PaintPathDestination(DrawingContext graphics) {
      if (graphics==null) throw new ArgumentNullException("graphics");

      var penBlack = new Pen(Brushes.Black,1.0F);
      var unit = GridSize.Height/8.0F;
      graphics.DrawLine(penBlack, new Point(-unit*2,-unit*2), new Point(unit*2, unit*2));
      graphics.DrawLine(penBlack, new Point(-unit*2, unit*2), new Point(unit*2,-unit*2));
    }

    /// <inheritdoc/>
    public    virtual  void PaintUnits(DrawingContext graphics) {}

    /// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
    /// for each hex on <paramref name="dc"/>.</summary>
    /// <param name="dc">Graphics object for the canvas being painted.</param>
    /// <param name="clipHexes">Type: CoordRectangle - 
    /// The rectangular extent of hexes to be painted.</param>
    /// <param name="paintAction">Type: Action {HexCoords} - 
    /// The paint action to be performed for each hex.</param>
    void PaintForEachHex(DrawingContext graphics, CoordsRectangle clipHexes, Action<HexCoords> paintAction) {
      ForEachHex(hex => {
        if (clipHexes.Left <= hex.Coords.User.X  &&  hex.Coords.User.X <= clipHexes.Right
        &&  clipHexes.Top  <= hex.Coords.User.Y  &&  hex.Coords.User.Y <= clipHexes.Bottom) {
          graphics.PushTransform(TranslateToHex(StartHex));
          paintAction(hex.Coords);
        }
      } );
      return;
    }

    /// <summary>Returns the translation transform-matrix for the upper-left corner of the specified hex.</summary>
    /// <param name="coords">Type: HexCoords - 
    /// Coordinates of the hex to be painted next.</param>
    //void TranslateGraphicsToHex(DrawingContext dc, HexCoords coords) {
    //  var offset  = UpperLeftOfHex(coords);
    //  dc.Transform = new Matrix(1, 0, 0, 1, offset.X, offset.Y);
    //}
    Transform TranslateToHex(HexCoords coords) {
      var offset  = UpperLeftOfHex(coords);
      return new MatrixTransform(new Matrix(1, 0, 0, 1, offset.X, offset.Y));
    }

    /// <summary>Returns pixel coordinates of upper-left corner of specified hex.</summary>
    /// <param name="coords"></param>
    /// <returns>A Point structure containing pixel coordinates for the (upper-left corner of the) specified hex.</returns>
    protected HexPoint UpperLeftOfHex(HexCoords coords) {
      return new HexPoint(
        coords.User.X * GridSize.Width,
        coords.User.Y * GridSize.Height + (coords.User.X+1)%2 * GridSize.Height/2
      );
    }

    /// <summary>Returns pixel coordinates of centre of specified hex.</summary>
    /// <param name="coords"></param>
    /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
    protected HexPoint CentreOfHex(HexCoords coords) {
      return UpperLeftOfHex(coords) + CentreOfHexOffset;
    }
    #endregion

    /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
    /// <param name="coords">Type HexCoords - 
    /// Hex for which to return Landmark distanace.</param>
    /// <param name="landmarkToShow">Type int - 
    /// Index of the Landmark from which to display distances.</param>
    protected virtual string LandmarkDistance(HexCoords coords, int landmarkToShow) { 
      if (landmarkToShow < 0  ||  Landmarks.Count <= landmarkToShow) return "";

      return string.Format("{0,3}", Landmarks[landmarkToShow].DistanceFrom(coords));
    }

    /// <summary>TODO</summary>
    private void Host_FovRadiusChanged(object sender, Int32ValueEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      FovRadius = RangeCutoff = e.Value;
    }
  }

  /// <summary>TODO</summary>
  /// <typeparam name="T"></typeparam>
  public class ValueChangedEventArgs<T> : EventArgs {
    /// <summary>TODO</summary>
    public ValueChangedEventArgs(T value) : base() { Value = value; }
    /// <summary>TODO</summary>
    public T Value { get; private set; }
  }
}
