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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.FieldOfView;

#pragma warning disable 1587
/// <summary>WinForms-specific utilities, including implementation of the subclasses HexgridPanel
/// and MapDisplay<THex>, utilizing the System.Windows.Forms technology.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridPanel {
  using Int32ValueEventArgs = ValueChangedEventArgs<Int32>;
  using IDirectedPath       = IDirectedPathCollection;
  using MapGridHex          = Hex<Graphics,GraphicsPath>;

  /// <summary>Abstract class representing the basic game board.</summary>
  /// <typeparam name="THex">Type of the hex for which a game board is desired.</typeparam>
  public abstract class MapDisplay<THex> : HexBoard<THex,GraphicsPath>, IMapDisplayWinForms
    where THex : MapGridHex {

    /// <summary>TODO</summary>
    protected delegate THex InitializeHex(GraphicsPath hexgridPath, HexCoords coords);

    /// <summary>TODO</summary>
    private static GraphicsPath GetGraphicsPath(Size gridSize) {
      GraphicsPath path     = null;
      GraphicsPath tempPath = null;
      try {
        tempPath  = new GraphicsPath();
        tempPath.AddLines(new Point[] {
          new Point(gridSize.Width*1/3,              0  ), 
          new Point(gridSize.Width*3/3,              0  ),
          new Point(gridSize.Width*4/3,gridSize.Height/2),
          new Point(gridSize.Width*3/3,gridSize.Height  ),
          new Point(gridSize.Width*1/3,gridSize.Height  ),
          new Point(             0,    gridSize.Height/2),
          new Point(gridSize.Width*1/3,              0  )
        } );
        path     = tempPath;
        tempPath = null;
      } finally { if(tempPath!=null) tempPath.Dispose(); }
      return path;
    }

    #region Constructors
    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    protected MapDisplay(Size sizeHexes, Size gridSize, InitializeHex initializeHex) 
    : this(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes)) {}

    /// <summary>Creates a new instance of the MapDisplay class.</summary>
    protected MapDisplay(Size sizeHexes, Size gridSize, InitializeHex initializeHex, IFastList<HexCoords> landmarkCoords) 
    : base(sizeHexes, gridSize, landmarkCoords, GetGraphicsPath
      #if FlatBoardStorage
        ,() => new FlatBoardStorage<THex>(sizeHexes, coords => initializeHex(board,coords))
      #else
        ,() => new BlockedBoardStorage32x32<THex>(sizeHexes, coords => initializeHex(GetGraphicsPath(gridSize),coords))
      #endif
    ) {
      InitializeProperties();
      var grid = TransposableHexgrid.GetNewGrid(false,gridSize,1.0F);
    }

    void InitializeProperties() {
      GoalHex         = 
      HotspotHex      = 
      StartHex        = HexCoords.EmptyUser;
      ShadeBrushAlpha = 78;
      ShadeBrushColor = Color.Black;
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

    #region Painting
    /// <inheritdoc/>
    public CoordsRectangle GetClipInHexes(PointF point, SizeF size) {
      return GetClipInHexes( new RectangleF(point,size), MapSizeHexes );
    }

    /// <inheritdoc/>
    public CoordsRectangle GetClipInHexes(RectangleF visibleClipBounds) {
      return GetClipInHexes(visibleClipBounds, MapSizeHexes);
    }

    /// <summary>Wrapper for MapDisplayPainter.PaintHighlight.</summary>
    public    virtual  void PaintHighlight(Graphics graphics) { MapDisplayPainter.PaintHighlight(this,graphics); }

    /// <summary>Wrapper for MapDisplayPainter.PaintMap.</summary>
    public    virtual  void PaintMap(Graphics graphics) { MapDisplayPainter.PaintMap(this,graphics); }

    /// <summary>Wrapper for MapDisplayPainter.PaintShading.</summary>
    public    virtual  void PaintShading(Graphics graphics) { MapDisplayPainter.PaintShading(this,graphics); }

    /// <summary>Wrapper for MapDisplayPainter.PaintUnits.</summary>
    public    virtual  void PaintUnits(Graphics graphics) { MapDisplayPainter.PaintUnits(this,graphics); }

    /// <summary>Returns the translation transform-@this for the upper-left corner of the specified hex.</summary>
    /// <param name="coords">Type: HexCoords - 
    /// Coordinates of the hex to be painted next.</param>
    public Matrix TranslateToHex(HexCoords coords) {
      var offset  = UpperLeftOfHex(coords);
      return new Matrix(1, 0, 0, 1, offset.X, offset.Y);
    }

    /// <summary>Returns pixel coordinates of upper-left corner of specified hex.</summary>
    /// <param name="coords"></param>
    /// <returns>A Point structure containing pixel coordinates for the (upper-left corner of the) specified hex.</returns>
    public Point UpperLeftOfHex(HexCoords coords) {
      return new Point(
        coords.User.X * GridSize.Width,
        coords.User.Y * GridSize.Height + (coords.User.X+1)%2 * GridSize.Height/2
      );
    }

    /// <summary>Returns pixel coordinates of centre of specified hex.</summary>
    /// <param name="coords"></param>
    /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
    public Point CentreOfHex(HexCoords coords) {
      return UpperLeftOfHex(coords) + CentreOfHexOffset;
    }
    #endregion

    /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
    /// <param name="coords">Type HexCoords - 
    /// Hex for which to return Landmark distanace.</param>
    /// <param name="landmarkToShow">Type int - 
    /// Index of the Landmark from which to display distances.</param>
    public virtual string LandmarkDistance(HexCoords coords, int landmarkToShow) { 
      if (landmarkToShow < 0  ||  Landmarks.Count <= landmarkToShow) return "";

      return string.Format(CultureInfo.CurrentCulture,"{0,3}", Landmarks[landmarkToShow].DistanceFrom(coords));
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
