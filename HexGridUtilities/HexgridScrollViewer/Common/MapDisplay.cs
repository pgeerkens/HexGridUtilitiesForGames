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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.FieldOfView;

#pragma warning disable 1587
/// <summary>WinForms-specific utilities, including implementation of the subclasses HexgridPanel
/// and MapDisplay<THex>, utilizing the System.Windows.Forms technology.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridScrollViewer {
  using HexSize  = System.Drawing.Size;
  using HexPoint = System.Drawing.Point;
  using HexRectF = System.Drawing.RectangleF;

  using Int32ValueEventArgs = ValueChangedEventArgs<Int32>;

  /// <summary>Abstract class representing the basic game board.</summary>
  /// <typeparam name="THex">Type of the hex for which a game board is desired.</typeparam>
  public abstract class MapDisplay<THex> : HexBoardWpf<THex>, IBoard<THex>, IMapDisplayWpf
    where THex : MapGridHex {

    #region Constructors
    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    protected MapDisplay(HexSize sizeHexes, HexSize gridSize, Func<HexBoardWpf<THex>, HexCoords, THex> initializeHex) 
    : this(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes)) {}

    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    protected MapDisplay(HexSize sizeHexes, HexSize gridSize, Func<HexBoardWpf<THex>, HexCoords, THex> initializeHex, 
                         ReadOnlyCollection<HexCoords> landmarkCoords) 
    : base(sizeHexes, gridSize, 
          (map) => new BoardStorage<THex>.FlatBoardStorage(sizeHexes, coords => initializeHex(map,coords)),
          landmarkCoords
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
//      get { return _fov ?? (_fov = this.GetFieldOfView(HotspotHex)); }
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
    public          IDirectedPath Path            { 
      get { return _path ?? (_path = this.GetDirectedPath(this[StartHex], this[GoalHex])); } 
    } IDirectedPath _path;
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

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2233:OperationsShouldNotOverflow", MessageId = "10*elevationLevel")]
    public override int   ElevationASL(int elevationLevel) { return 10 * elevationLevel; }

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
    public    virtual  void PaintHighlight(DrawingContext g) { 
      if (g==null) throw new ArgumentNullException("g");
      var brushBlack = Brushes.Black;

      g.PushTransform(TranslateToHex(StartHex));
      g.DrawGeometry(Brushes.Transparent,new Pen(brushBlack,1),HexgridPath);
      g.Pop();

      //g.DrawPath(Pens.Red, HexgridPath);

      //if (ShowPath) {
      //  g.EndContainer(container); container = g.BeginContainer();
      //  PaintPath(g,Path);
      //}

      //if (ShowRangeLine) {
      //  g.EndContainer(container); container = g.BeginContainer();
      //  var target = CentreOfHex(HotspotHex);
      //  g.DrawLine(Pens.Red, CentreOfHex(StartHex), target);
      //  g.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
      //  g.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
      //}

      //if (ShowFov) {
      //  g.EndContainer(container); container = g.BeginContainer();
      //  var clipHexes  = GetClipInHexes(g.VisibleClipBounds);
      //  using(var shadeBrush = new SolidBrush(Color.FromArgb(ShadeBrushAlpha, ShadeBrushColor))) {
      //    PaintForEachHex(g, clipHexes, coords => {
      //      if (Fov!=null && ! Fov[coords]) { g.FillPath(shadeBrush, HexgridPath);  }
      //    } );
      //  }
      //}
    }

    /// <inheritdoc/>
    public    virtual  void PaintMap(DrawingContext g) { 
      if (g==null) throw new ArgumentNullException("g");

//      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      var clipHexes = GetClipInHexes(HexPoint.Empty.ToWpfPoint(),MapSizeHexes.ToWpfSize());

      var fontSize   = SystemFonts.MenuFontSize * 0.8F;
      var brush      = Brushes.Black;
      var textOffset = new HexPoint(GridSize.Scale(0.50F).ToSize()).ToWpfPoint()
                     - new Vector(fontSize,fontSize);
      PaintForEachHex(g, clipHexes, coords => {
        this[coords].Paint(g);
        if (ShowHexgrid) g.DrawGeometry(Brushes.Transparent,new Pen(Brushes.Black,1), HexgridPath);
        if (LandmarkToShow > 0) {
          g.DrawText(new FormattedText(
              LandmarkDistance(coords,LandmarkToShow-1),
              CultureInfo.GetCultureInfo("en-US"),
              FlowDirection.LeftToRight,
              new Typeface("Verdana"),
              fontSize,
              Brushes.Black),
            textOffset);
        }
      } );
      g.Pop();
    }

    /// <summary>Paint the current shortese path.</summary>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPath"/> - 
    /// A directed path (ie linked-list> of hexes to be painted.</param>
    protected virtual  void PaintPath(DrawingContext g, IDirectedPath path) {
      if (g==null) throw new ArgumentNullException("g");

      var pen   = new Pen(Brushes.Black,1.0F);
      var brush = new SolidColorBrush(Color.FromArgb(78, Colors.PaleGoldenrod.R,Colors.PaleGoldenrod.G,Colors.PaleGoldenrod.B));
      while (path != null) {
        var coords = path.PathStep.Hex.Coords;
        g.PushTransform(TranslateToHex(StartHex));
        g.DrawGeometry(brush,pen,HexgridPath);

        if (ShowPathArrow) PaintPathArrow(g, path);

        path = path.PathSoFar;
      }
    }

    /// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPath"/> - 
    /// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
    protected virtual  void PaintPathArrow(DrawingContext g, IDirectedPath path) {
      if (g==null) throw new ArgumentNullException("g");
      if (path==null) throw new ArgumentNullException("path");

      g.PushTransform(new TranslateTransform(CentreOfHexOffset.Width, CentreOfHexOffset.Height));
      if (path.PathSoFar == null)    PaintPathDestination(g);
      else                           PaintPathArrow(g, path.PathStep.HexsideEntry);
      g.Pop();
    }

    /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="hexside">Type: <see cref="Hexside"/> - 
    /// Direction from this hex in which the next step is made.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    protected virtual  void PaintPathArrow(DrawingContext g, Hexside hexside) {
      if (g==null) throw new ArgumentNullException("g");

      var penBlack = new Pen(Brushes.Black,1.0F);
      var unit = GridSize.Height/8.0F;
      g.PushTransform(new RotateTransform(60 * (int)hexside));
      g.DrawLine(penBlack, new Point(0,unit*4), new Point(      0,  -unit));
      g.DrawLine(penBlack, new Point(0,unit*4), new Point(-unit*3/2, unit*2));
      g.DrawLine(penBlack, new Point(0,unit*4), new Point( unit*3/2, unit*2));
      g.Pop();
    }

    /// <summary>Paint the destination indicator for the current shortest path.</summary>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    protected virtual  void PaintPathDestination(DrawingContext g) {
      if (g==null) throw new ArgumentNullException("g");

      var penBlack = new Pen(Brushes.Black,1.0F);
      var unit = GridSize.Height/8.0F;
      g.DrawLine(penBlack, new Point(-unit*2,-unit*2), new Point(unit*2, unit*2));
      g.DrawLine(penBlack, new Point(-unit*2, unit*2), new Point(unit*2,-unit*2));
    }

    /// <inheritdoc/>
    public    virtual  void PaintUnits(DrawingContext g) {}

    /// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
    /// for each hex on <paramref name="g"/>.</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="clipHexes">Type: CoordRectangle - 
    /// The rectangular extent of hexes to be painted.</param>
    /// <param name="paintAction">Type: Action&lt;HexCoords&gt; - 
    /// The paint action to be performed for each hex.</param>
    void PaintForEachHex(DrawingContext g, CoordsRectangle clipHexes, Action<HexCoords> paintAction) {
      BoardHexes.ForEach(hex => {
        if (clipHexes.Left <= hex.Coords.User.X  &&  hex.Coords.User.X <= clipHexes.Right
        &&  clipHexes.Top  <= hex.Coords.User.Y  &&  hex.Coords.User.Y <= clipHexes.Bottom) {
          g.PushTransform(TranslateToHex(StartHex));
          paintAction(hex.Coords);
        }
      } );
      return;
    }

    /// <summary>Returns the translation transform-matrix for the upper-left corner of the specified hex.</summary>
    /// <param name="coords">Type: HexCoords - 
    /// Coordinates of the hex to be painted next.</param>
    //void TranslateGraphicsToHex(DrawingContext g, HexCoords coords) {
    //  var offset  = UpperLeftOfHex(coords);
    //  g.Transform = new Matrix(1, 0, 0, 1, offset.X, offset.Y);
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

      return string.Format("{0,3}", Landmarks[landmarkToShow].HexDistance(coords));
    }

    /// <summary>TODO</summary>
    private void Host_FovRadiusChanged(object sender, Int32ValueEventArgs e) {
      if (e==null) throw new ArgumentNullException("e");
      FovRadius = RangeCutoff = e.Value;
    }

    #region deprecated
    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    [Obsolete("Use MapDisplay(Size,Size,Func<HexBoard<THex>, HexCoords, THex>) instead; client should set hex size.")]
    protected MapDisplay(HexSize sizeHexes, Func<HexBoard<THex>, HexCoords, THex> initializeHex) 
      : this(sizeHexes, new HexSize(27,30), initializeHex) {}

    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    [Obsolete("Use MapDisplay(Size,Size,Func<HexBoard<THex>, HexCoords, THex>) instead; client should set hex size.")]
    protected MapDisplay(HexSize sizeHexes, Func<HexBoard<THex>, HexCoords, THex> initializeHex, 
                       ReadOnlyCollection<HexCoords> landmarkCoords) 
    : this(sizeHexes, new HexSize(27,30), initializeHex, landmarkCoords) {}

    /// <inheritdoc/>
    [Obsolete("Use GetClipInHexes(PointF,SizeF) instead.")]
    public CoordsRectangle GetClipCells(Point point, Size size) {
      return GetClipInHexes( new HexRectF(point.ToHexPoint(),size.ToHexSize()), MapSizeHexes );
    }
    /// <inheritdoc/>
    [Obsolete("Use GetClipInHexes(RectangleF) instead.")]
    public CoordsRectangle GetClipCells(HexRectF visibleClipBounds) {
      return GetClipInHexes(visibleClipBounds, MapSizeHexes);
    }
    #endregion
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
