#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.HexGridExample {
  public interface IMapGridHex : IGridHex {
    char             Value          { get; }
  }

  public abstract class MapDisplay : MapBoard, IMapDisplay, IBoard<IMapGridHex> {
    public MapDisplay() : base() {
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
      MapSizeMatrix = new IntMatrix2D(GridSize.Width,                0, 
                                                   0,  GridSize.Height, 
                                      GridSize.Width/3,GridSize.Height/2);

    }

    public Size            GridSize      { get; private set; }
    public Size            MapMargin     { get; set; }
    public Size            MapSizePixels { get {return SizeHexes * MapSizeMatrix;} }
    public string          Name          { get { return "MapDisplay"; }}
    protected GraphicsPath HexgridPath   { get; set; }
    protected IntMatrix2D  MapSizeMatrix { get; set; }

    public UserCoordsRectangle GetClipCells(PointF point, SizeF size) {
      return GetClipHexes( new RectangleF(point,size), SizeHexes );
    }
    public UserCoordsRectangle GetClipCells(RectangleF visibleClipBounds) {
      return GetClipHexes(visibleClipBounds, SizeHexes);
    }
    public virtual void PaintHighlight(Graphics g) { 
      var state = g.Save();
      g.TranslateTransform(
        MapMargin.Width  + StartHex.X * GridSize.Width,
        MapMargin.Height + StartHex.Y * GridSize.Height + (StartHex.X+1)%2 * GridSize.Height/2
      );
      g.DrawPath(Pens.Red, HexgridPath);

      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        var path = Path;
        while (path != null) {
          g.Restore(state); state = g.Save();
          var step = path.LastStep.User;

          g.TranslateTransform(
            MapMargin.Width  + step.X * GridSize.Width,
            MapMargin.Height + step.Y * GridSize.Height + (step.X+1)%2 * GridSize.Height/2
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

    UserCoordsRectangle GetClipHexes(RectangleF visibleClipBounds, Size boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new UserCoordsRectangle (left, top, right-left, bottom-top);
    }
  }
}
