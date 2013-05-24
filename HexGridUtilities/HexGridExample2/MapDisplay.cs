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

using PG_Napoleonics;
using PG_Napoleonics.HexgridPanel;
using PG_Napoleonics.HexUtilities;
using PG_Napoleonics.HexUtilities.Common;
using PG_Napoleonics.HexUtilities.PathFinding;
using PG_Napoleonics.HexUtilities.ShadowCastingFov;

namespace PG_Napoleonics.HexGridExample2 {
  public abstract class MapDisplay : HexBoard, IMapDisplay, IBoard<IHex>, INavigableBoard {
 
    public MapDisplay() : base(Size.Empty) {
      FovRadius = 40;
      GridSize = new Size(27,30);
      StartHex = GoalHex = HotSpotHex = HexCoords.EmptyUser;

      HexgridPath = new GraphicsPath();
      HexgridPath.AddLines(new Point[] {
        new Point(GridSize.Width*1/3,                0), 
        new Point(GridSize.Width*3/3,                0),
        new Point(GridSize.Width*4/3,GridSize.Height/2),
        new Point(GridSize.Width*3/3,GridSize.Height  ),
        new Point(GridSize.Width*1/3,GridSize.Height  ),
        new Point(                 0,GridSize.Height/2),
        new Point(GridSize.Width*1/3,                0)
      } );
      MapSizeMatrix = new IntMatrix2D(GridSize.Width,                 0, 
                                                   0,    GridSize.Height, 
                                      GridSize.Width/3,  GridSize.Height/2);
    }

    public virtual  IFov     FOV            {
      get { return _fov ?? (_fov = this.GetFov(HotSpotHex)); }
      protected set { _fov = value; }
    } IFov _fov;
    public virtual  ICoords  GoalHex        { 
      get { return _goalHex??(_goalHex=HexCoords.EmptyUser); } 
      set { _goalHex=value; _path = null; } 
    } ICoords _goalHex;
    public virtual  ICoords  HotSpotHex     { 
      get { return _hotSpotHex; }
      set { _hotSpotHex = value; FOV = null; }
    } ICoords _hotSpotHex;
    public          IPath    Path           { 
      get {return _path ?? (_path = this.GetPath(StartHex,GoalHex));} 
    } IPath _path;
    public virtual  ICoords  StartHex       { 
      get { return _startHex ?? (_startHex = HexCoords.EmptyUser); } 
      set { if (IsOnBoard(value)) _startHex = value; _path = null; } 
    } ICoords _startHex;

    public Size            GridSize      { get; private set; }
    public Size            MapMargin     { get; set; }
    public Size            MapSizePixels { get {return SizeHexes * MapSizeMatrix;} }
    public string          Name          { get { return "MapDisplay"; }}
    protected GraphicsPath HexgridPath   { get; set; }
    protected IntMatrix2D  MapSizeMatrix { get; set; }
    public bool            ShowFov       { get; set; }
    public override Size   SizeHexes     { get {return new Size(Board[0].Length, Board.Length);} }

    protected abstract string[] Board    { get; }

    public UserCoordsRectangle GetClipCells(PointF point, SizeF size) {
      return GetClipHexes( new RectangleF(point,size), SizeHexes );
    }
    public UserCoordsRectangle GetClipCells(RectangleF visibleClipBounds) {
      return GetClipHexes(visibleClipBounds, SizeHexes);
    }
    public virtual  void PaintHighlight(Graphics g) { 
      var state = g.Save();
      g.TranslateTransform(
        MapMargin.Width  + StartHex.User.X * GridSize.Width,
        MapMargin.Height + StartHex.User.Y * GridSize.Height + (StartHex.User.X+1)%2 * GridSize.Height/2
      );
      g.DrawPath(Pens.Red, HexgridPath);

      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        var path = Path;
        while (path != null) {
          g.Restore(state); state = g.Save();
          var step = path.LastStep;

          g.TranslateTransform(
            MapMargin.Width  + step.User.X * GridSize.Width,
            MapMargin.Height + step.User.Y * GridSize.Height + (step.User.X+1)%2 * GridSize.Height/2
          );
          g.FillPath(brush, HexgridPath);
          path = path.PreviousSteps;
        }
      }

      g.Restore(state); state = g.Save();
      var clipCells = GetClipCells(g.VisibleClipBounds);
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      g.TranslateTransform(MapMargin.Width + clipCells.Right*GridSize.Width, MapMargin.Height);

      using(var shadeBrush = new SolidBrush(Color.FromArgb(78,Color.Black))) {
        for (int x=clipCells.Right; x-->clipCells.Left; ) {
          g.TranslateTransform(-GridSize.Width, 0);
          var container = g.BeginContainer();
          g.TranslateTransform(0,  clipCells.Top*GridSize.Height + (x+1)%2 * (GridSize.Height)/2);
          for (int y=clipCells.Top; y<clipCells.Bottom; y++) {
            var coords = HexCoords.NewUserCoords(x,y);
            if (ShowFov && FOV!=null && ! FOV[coords]) { g.FillPath(shadeBrush, HexgridPath);  }

            g.TranslateTransform(0,GridSize.Height);
          }
          g.EndContainer(container);
        }
      }
    }
    public abstract void PaintMap(Graphics g);
    public abstract void PaintUnits(Graphics g);

    UserCoordsRectangle  GetClipHexes(RectangleF visibleClipBounds, Size boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new UserCoordsRectangle (left, top, right-left, bottom-top);
    }
    
    public string        HexText(ICoords coords) { return HexText(coords.User.X, coords.User.Y); }
           string        HexText(int x, int y)   { return string.Format("{0,2}-{1,2}", x, y); }
  }
}
