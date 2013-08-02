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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

using PGNapoleonics;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.PathFinding;

namespace PGNapoleonics.HexGridExample2 {
  internal abstract class MapDisplay : HexBoard<MapGridHex>, IMapDisplay, IBoard<IHex>, INavigableBoard {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected MapDisplay(Size sizeHexes, Func<IBoard<MapGridHex>, HexCoords, MapGridHex> initializeHex) 
      : base(sizeHexes, new Size(27,30), (map) => 
          new BoardStorage<MapGridHex>.FlatBoardStorage(sizeHexes, coords => initializeHex(map,coords))
    ) {
      StartHex   = GoalHex = HotspotHex = HexCoords.EmptyUser;
    }

    public virtual  IFov      Fov            {
      get { return _fov ?? (_fov = this.GetFov(HotspotHex)); }
      protected set { _fov = value; }
    } IFov _fov;
    public virtual  HexCoords GoalHex        { 
      get { return _goalHex; }
      set { _goalHex=value; _path = null; } 
    } HexCoords _goalHex = HexCoords.EmptyUser;
    public virtual  HexCoords HotspotHex     { 
      get { return _hotSpotHex; }
      set { _hotSpotHex = value; Fov = null; }
    } HexCoords _hotSpotHex = HexCoords.EmptyUser;
    public          IDirectedPath Path       { 
      get { return _path ?? ( IsPassable(StartHex) && IsPassable(GoalHex) 
                          ? (_path = this.GetDirectedPath(this[StartHex], this[GoalHex])) : null); } 
    } IDirectedPath _path;
    public virtual  HexCoords StartHex       { 
      get { return _startHex; } // ?? (_startHex = HexCoords.EmptyUser); } 
      set { if (IsOnboard(value)) _startHex = value; _path = null; } 
    } HexCoords _startHex = HexCoords.EmptyUser;

    public          int       LandmarkToShow { get; set; }
    public          Size      MapMargin      { get; set; }
    public          string    Name           { get {return "MapDisplay";} }
    public          bool      ShowFov        { get; set; }

    IHex IFovBoard<IHex>.this[HexCoords coords] { get { return BoardHexes[coords]; } }

    public CoordsRectangle GetClipCells(PointF point, SizeF size) {
      return GetClipHexes( new RectangleF(point,size), MapSizeHexes );
    }
    public CoordsRectangle GetClipCells(RectangleF visibleClipBounds) {
      return GetClipHexes(visibleClipBounds, MapSizeHexes);
    }
    public virtual  void PaintHighlight(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      var state = g.Save();
      g.TranslateTransform(
        MapMargin.Width  + StartHex.User.X * GridSize.Width,
        MapMargin.Height + StartHex.User.Y * GridSize.Height + (StartHex.User.X+1)%2 * GridSize.Height/2
      );
      g.DrawPath(Pens.Red, HexgridPath);

#if PathFwd
      g.Restore(state); state = g.Save();
      PaintPath(g,Path);
#else
      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        var path = Path;
        while (path != null) {
          g.Restore(state); state = g.Save();

          var coords = path.StepCoords;
          g.TranslateTransform(
            MapMargin.Width  + coords.User.X * GridSize.Width,
            MapMargin.Height + coords.User.Y * GridSize.Height + (coords.User.X+1)%2 * GridSize.Height/2
          );
          g.FillPath(brush, HexgridPath);

          path = path.PathSoFar;
        }
      }
#endif

      g.Restore(state); state = g.Save();
      var clipCells = GetClipCells(g.VisibleClipBounds);
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      g.TranslateTransform(MapMargin.Width + clipCells.Right*GridSize.Width, MapMargin.Height);

      var textOffset = new Point(GridSize.Width/2 - 6, GridSize.Height/2 - 6);
      var font       = SystemFonts.MenuFont;
      using(var shadeBrush = new SolidBrush(Color.FromArgb(78,Color.Black))) {
        for (int x=clipCells.Right; x-->clipCells.Left; ) {
          g.TranslateTransform(-GridSize.Width, 0);
          var container = g.BeginContainer();
          g.TranslateTransform(0,  clipCells.Top*GridSize.Height + (x+1)%2 * (GridSize.Height)/2);
          for (int y=clipCells.Top; y<clipCells.Bottom; y++) {
            var coords = HexCoords.NewUserCoords(x,y);
            if (ShowFov && Fov!=null && ! Fov[coords]) { g.FillPath(shadeBrush, HexgridPath);  }

            g.DrawString(HexText(x,y,LandmarkToShow), font, Brushes.Black, textOffset);

            g.TranslateTransform(0,GridSize.Height);
          }
          g.EndContainer(container);
        }
      }
    }

#if PathFwd
    void PaintPath(Graphics g, IDirectedPath Path) {
      var state = g.Save();
      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        var path = Path;
        while (path != null) {
          g.Restore(state); state = g.Save();
          var coords = path.PathStep.Hex.Coords;
          g.TranslateTransform(
            MapMargin.Width  + coords.User.X * GridSize.Width,
            MapMargin.Height + coords.User.Y * GridSize.Height + (coords.User.X+1)%2 * GridSize.Height/2
          );
          g.FillPath(brush, HexgridPath);

          PaintPathArrow(g, path);
          path = path.PathSoFar;
        }
      }
    }

    void PaintPathArrow(Graphics g, IDirectedPath path) {
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
#endif

    public abstract void PaintMap(Graphics g);
    public abstract void PaintUnits(Graphics g);

    CoordsRectangle  GetClipHexes(RectangleF visibleClipBounds, Size boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new CoordsRectangle (left, top, right-left, bottom-top);
    }
    
    public string HexText(HexCoords coords, int landmarkToShow) { 
      var value = (0 <= landmarkToShow && landmarkToShow < Landmarks.Count)
        ? Landmarks[landmarkToShow].HexDistance(coords) : -1;
      return value==-1 ? "" : string.Format("{0,3}", value);
    }
    public string HexText(int x, int y, int landmarkToShow)     { 
      return HexText(HexCoords.NewUserCoords(x,y),landmarkToShow); 
    }
  }
}
