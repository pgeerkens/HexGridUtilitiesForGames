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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.ShadowCasting;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>TODO</summary>
  /// <typeparam name="THex"></typeparam>
  public abstract class MapDisplay<THex> : HexBoard<THex>, IMapDisplay
    where THex : MapGridHex {

    #region Constructors
    /// <summary>Creates a new instance of th eMapDisplay class.</summary>
    protected MapDisplay(Size sizeHexes, Func<HexBoard<THex>, HexCoords, THex> initializeHex) 
      : this(sizeHexes, new Size(27,30), initializeHex) {}

    /// <summary>Creates a new instance of th eMapDisplay class.</summary>
    protected MapDisplay(Size sizeHexes, Size gridSize, Func<HexBoard<THex>, HexCoords, THex> initializeHex) 
    : base(sizeHexes, gridSize, (map) => 
          new BoardStorage<THex>.FlatBoardStorage(sizeHexes, coords => initializeHex(map,coords))
    ) {
      InitializeProperties();
    }

    /// <summary>Creates a new instance of th eMapDisplay class.</summary>
    protected MapDisplay(Size sizeHexes, Func<HexBoard<THex>, HexCoords, THex> initializeHex, 
                       ReadOnlyCollection<HexCoords> landmarkCoords) 
    : this(sizeHexes, new Size(27,30), initializeHex, landmarkCoords) {}

    /// <summary>Creates a new instance of th eMapDisplay class.</summary>
    protected MapDisplay(Size sizeHexes, Size gridSize, Func<HexBoard<THex>, HexCoords, THex> initializeHex, 
                       ReadOnlyCollection<HexCoords> landmarkCoords) 
    : base(sizeHexes, gridSize, (map) => 
          new BoardStorage<THex>.FlatBoardStorage(sizeHexes, coords => initializeHex(map,coords)),
          landmarkCoords
    ) {
      InitializeProperties();
    }

    void InitializeProperties() {
      GoalHex         = 
      HotspotHex      = 
      StartHex        = HexCoords.EmptyUser;
      ShadeBrushAlpha = 78;
      ShadeBrushColor = Color.Black;
      ShowHexgrid     = true;
      ShowPathArrow   = true;
      ShowRangeLine   = false;
    }
    #endregion

    #region Properties
    /// <summary>Gets or sets the Field-of-View for the current <see cref="HotspotHex"/>, as an <see cref="IFov"/> object.</summary>
    public virtual  IFov          Fov             {
//      get { return _fov ?? (_fov = this.GetFieldOfView(HotspotHex)); }
      get { return _fov ?? (_fov = this.GetFieldOfView(ShowRangeLine ? StartHex : HotspotHex)); }
      protected set { _fov = value; }
    } IFov _fov;
    /// <summary>Gets or sets the <see cref="HexCoords"/> of the goal hex for path-fnding.</summary>
    public virtual  HexCoords     GoalHex         { 
      get { return _goalHex; }
      set { _goalHex=value; _path = null; } 
    } HexCoords _goalHex = HexCoords.EmptyUser;
    /// <summary>Gets or sets the <see cref="HexCoords"/> of the hex currently under the mouse.</summary>
    public virtual  HexCoords     HotspotHex      { 
      get { return _hotSpotHex; }
      set { _hotSpotHex = value; Fov = null; }
    } HexCoords _hotSpotHex = HexCoords.EmptyUser;

    /// <summary>Gets or sets the index (-1 for none) of the path-finding <see cref="Landmark"/> to show.</summary>
    public          int           LandmarkToShow  { get; set; }
    /// <summary>Gets or sets the thickness in pixels (at 100% scale) of the margin to be painted around the map proper.</summary>
    public          Size          MapMargin       { get; set; }
    /// <inheritdoc/>>
    public          string        Name            { get {return "MapDisplay";} }
    /// <summary>Gets the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
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
    public          bool          ShowPathArrow   { get; set; }
    /// <summary>Gets or sets whether to display the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
    public          bool          ShowRangeLine   { get; set; }
    /// <summary>Gets or sets the <see cref="HexCoords"/> of the start hex for path-finding.</summary>
    public virtual  HexCoords     StartHex        { 
      get { return _startHex; } // ?? (_startHex = HexCoords.EmptyUser); } 
      set { if (IsOnboard(value)) _startHex = value; _path = null; } 
    } HexCoords _startHex = HexCoords.EmptyUser;
    #endregion

    /// <inheritdoc/>>
    [Obsolete("Use GetClipInHexes(PointF,SizeF) instead.")]
    public CoordsRectangle GetClipCells(PointF point, SizeF size) {
      return GetClipInHexes( new RectangleF(point,size), MapSizeHexes );
    }
    /// <inheritdoc/>>
    [Obsolete("Use GetClipInHexes(RectangleF) instead.")]
    public CoordsRectangle GetClipCells(RectangleF visibleClipBounds) {
      return GetClipInHexes(visibleClipBounds, MapSizeHexes);
    }

    /// <inheritdoc/>>
    public CoordsRectangle GetClipInHexes(PointF point, SizeF size) {
      return GetClipInHexes( new RectangleF(point,size), MapSizeHexes );
    }

    /// <inheritdoc/>>
    public CoordsRectangle GetClipInHexes(RectangleF visibleClipBounds) {
      return GetClipInHexes(visibleClipBounds, MapSizeHexes);
    }

    /// <inheritdoc/>>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    public    virtual  void PaintHighlight(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      var container = g.BeginContainer();
      TranslateGraphicsToHex(g, StartHex);
      g.DrawPath(Pens.Red, HexgridPath);

      g.EndContainer(container); container = g.BeginContainer();
      PaintPath(g,Path);

      if (ShowRangeLine) {
        g.EndContainer(container); container = g.BeginContainer();
        var target = CentreOfHex(HotspotHex);
        g.DrawLine(Pens.Red, CentreOfHex(StartHex), target);
        g.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
        g.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
      }

      g.EndContainer(container); container = g.BeginContainer();
      var clipHexes  = GetClipInHexes(g.VisibleClipBounds);

      using(var shadeBrush = new SolidBrush(Color.FromArgb(ShadeBrushAlpha, ShadeBrushColor))) {
        PaintForEachHex(g, clipHexes, coords => {
          if (ShowFov && Fov!=null && ! Fov[coords]) { g.FillPath(shadeBrush, HexgridPath);  }

          if (LandmarkToShow != -1 ) {
            var font       = SystemFonts.MenuFont;
            var brush      = Brushes.Black;
            var textOffset = new Point(GridSize.Scale(0.50F).ToSize() 
                           - new SizeF(font.Size,font.Size).Scale(0.8F).ToSize());
            g.DrawString(LandmarkText(coords,LandmarkToShow), font, brush, textOffset);
          }
        } );
      }
    }

    /// <inheritdoc/>>
    public    virtual  void PaintMap(Graphics g) { PaintMap(g, (h) =>h.Paint(g)); }

    /// <summary>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="paintAction"></param>
    void PaintMap(Graphics g, Action<THex> paintAction) { 
      if (g==null) throw new ArgumentNullException("g");
      if (paintAction==null) throw new ArgumentNullException("paintAction");
      var clipHexes = GetClipInHexes(g.VisibleClipBounds);
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      using(var font   = new Font("ArialNarrow", 8))
      using(var format = new StringFormat()) {
        format.Alignment = format.LineAlignment = StringAlignment.Center;

        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        PaintForEachHex(g, clipHexes, coords => {
          paintAction(this[coords]);
          if (ShowHexgrid) g.DrawPath(Pens.Black, HexgridPath);
        } );
      }
    }

    /// <summary>Paint the current shortese path.</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="path"></param>
    protected virtual  void PaintPath(Graphics g, IDirectedPath path) {
      if (g==null) throw new ArgumentNullException("g");

      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        while (path != null) {
          var coords = path.PathStep.Hex.Coords;
          TranslateGraphicsToHex(g, coords);
          g.FillPath(brush, HexgridPath);

          if (ShowPathArrow) PaintPathArrow(g, path);
          path = path.PathSoFar;
        }
      }
    }

    /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="path"></param>
    protected virtual  void PaintPathArrow(Graphics g, IDirectedPath path) {
      if (g==null) throw new ArgumentNullException("g");
      if (path==null) throw new ArgumentNullException("path");

      g.TranslateTransform(GridSize.Width * 2/3, GridSize.Height/2);
      var unit = GridSize.Height/8.0F;
      if (path.PathSoFar == null) {
        g.DrawLine(Pens.Black, -unit*2,-unit*2, unit*2, unit*2);
        g.DrawLine(Pens.Black, -unit*2, unit*2, unit*2,-unit*2);
      } else {
        g.RotateTransform(60 * (int)path.PathStep.HexsideEntry);
        g.DrawLine(Pens.Black, 0,unit*4,       0,-unit);
        g.DrawLine(Pens.Black, 0,unit*4, -unit*3/2, unit*2);
        g.DrawLine(Pens.Black, 0,unit*4,  unit*3/2, unit*2);
      }
    }

    /// <inheritdoc/>>
    public    abstract void PaintUnits(Graphics g);

    /// <summary>TODO</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="clipHexes"></param>
    /// <param name="paintAction"></param>
    void PaintForEachHex(Graphics g, CoordsRectangle clipHexes, Action<HexCoords> paintAction) {
      BoardHexes.ForEach(hex => {
        if (clipHexes.Left <= hex.Coords.User.X  &&  hex.Coords.User.X <= clipHexes.Right
        &&  clipHexes.Top  <= hex.Coords.User.Y  &&  hex.Coords.User.Y <= clipHexes.Bottom) {
          TranslateGraphicsToHex(g, hex.Coords);
          paintAction(hex.Coords);
        }
      } );
      return;
    }

    /// <summary>TODO</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="coords"></param>
    void TranslateGraphicsToHex(Graphics g, HexCoords coords) {
      var offset = UpperLeftOfHex(coords);
      g.Transform = new System.Drawing.Drawing2D.Matrix(1,0,0,1, offset.X, offset.Y);
    }

    /// <summary>Returns pixel coordinates of upper-left corner of specified hex.</summary>
    /// <param name="coords"></param>
    /// <returns>A Point structure containing pixel coordinates for the (upper-left corner of the) specified hex.</returns>
    protected Point UpperLeftOfHex(HexCoords coords) {
      return new Point(
        MapMargin.Width  + coords.User.X * GridSize.Width,
        MapMargin.Height + coords.User.Y * GridSize.Height + (coords.User.X+1)%2 * GridSize.Height/2
      );
    }

    /// <summary>Returns pixel coordinates of centre of specified hex.</summary>
    /// <param name="coords"></param>
    /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
    protected Point CentreOfHex(HexCoords coords) {
      return UpperLeftOfHex(coords) + new Size(GridSize.Width * 2 / 3, GridSize.Height / 2);
    }

    /// <summary>TODO</summary>
    protected virtual string LandmarkText(HexCoords coords, int landmarkToShow) { 
      var index = (0 <= landmarkToShow && landmarkToShow < Landmarks.Count)
        ? Landmarks[landmarkToShow].HexDistance(coords) 
        : -1;
      return index == -1 ? "" : string.Format("{0,3}", index);
    }
  }
}
