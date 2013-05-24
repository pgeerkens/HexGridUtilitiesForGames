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
using PG_Napoleonics.HexUtilities;

namespace PG_Napoleonics.HexGridExample2 {
  public sealed class MazeMap : MapDisplay {
    public MazeMap() : base() {}

    public override bool  IsPassable(ICoords coords) { 
      return IsOnBoard(coords)  &&  GetMapGridHex(coords).Elevation == 0; 
    }

    /// <inheritdoc/>
    public override int    Heuristic(int range) { return range; }

    #region Painting
    public override void PaintMap(Graphics g) { 
      var clipCells = GetClipCells(g.VisibleClipBounds);
      var state     = g.Save();
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      g.TranslateTransform(MapMargin.Width + clipCells.Right*GridSize.Width, MapMargin.Height);

      using(var font   = new Font("ArialNarrow", 8))
      using(var format = new StringFormat()) {
        format.Alignment = format.LineAlignment = StringAlignment.Center;
        for (int x=clipCells.Right; x-->clipCells.Left; ) {
          g.TranslateTransform(-GridSize.Width, 0);
          var container = g.BeginContainer();
          g.TranslateTransform(0,  clipCells.Top*GridSize.Height + (x+1)%2 * (GridSize.Height)/2);
          for (int y=clipCells.Top; y<clipCells.Bottom; y++) {
            GetMapGridHex(HexCoords.NewUserCoords(x,y)).Paint(g);
            g.DrawPath(Pens.Black, HexgridPath);

            g.TranslateTransform(0,GridSize.Height);
          }
          g.EndContainer(container);
        }
      }
    }
    public override void PaintUnits(Graphics g) { ; }
    #endregion

    #region Board definition
    protected override string[]   Board { get { return _board; } }
    string[] _board = new string[] {
      ".............|.........|.......|.........|.............",
      ".............|.........|.......|.........|.............",
      "....xxxxxxxxx|....|....|...|...|...|.....|.............",
      ".............|....|....|...|...|...|.....|.............",
      "xxxxxxxxx....|....|....|...|...|...|.....|.............",
      ".............|....|....|...|...|...|.....|.............",
      ".............|....|....|...|...|...|...................",
      ".....xxxxxxxx|....|........|.......|.......xxxxxxxx....",
      "..................|........|.......|.......|......|....",
      "xxxxxxxxx....xxxxxxxxxxxxxxxxxxxxxxxx......|......|....",
      "................|........|...|.............|...|..|....",
      "xxxxxxxxxxxx....|...|....|...|.............|...|...|...",
      "................|...|....|...|.............|...|...|...",
      "..xxxxxxxxxxxxxxx...|....|...|.............|...|...|...",
      "....................|....|.................|...|...|...",
      "xxxxxxxxxxxxxxxxx..xx....|.................|...|.......",
      ".........................|...xxxxxxxxxxxxxxx...|xxxxxxx",
      "xxxxxx...................|...|.................|.......",
      ".........xxxxxxxxxxxxxxxxx...|.................|.......",
      ".............................|...xxxxxxxxxxxxxx|.......",
      "xxxxxxxxxx......xxxxxxxxxxxxx|...|.....................",
      ".............................|...|.....................",
      ".............................|...|...xxxxxxxxxxxxx.....",
      "....xxxxxxxxxxxxxxxxxxxxxxxxx|...|...|.................",
      ".............................|...|...|..........xxxxxxx",
      "xxxxxxxxxxxxxxxxxxxxxxxxx....|...|...|.....|....|......",
      ".............................|...|...|.....|....|......",
      "...xxxxxxxxxxxxxxxxxxxxxxxxxx|...|...|.....|....|......",
      ".............................|.......|.....|...........",
      ".............................|.......|.....|..........."
    };
    #endregion

    protected override IHex GetGridHex(ICoords coords) { return GetMapGridHex(coords); }
    private IMapGridHex GetMapGridHex(ICoords coords) {
      switch (Board[coords.User.Y][coords.User.X]) {
        case '.':   return new PathMazeGridHex(this,coords,GridSize);
        default:    return new WallMazeGridHex(this,coords,GridSize);
      }
    }

    public abstract class MazeGridHex : MapGridHex {
      public MazeGridHex(MapDisplay board, ICoords coords, Size gridSize) 
        : base(board, coords) {
        GridSize  = gridSize;

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
      }

      public override int  ElevationASL   { get { return Elevation * 10; } protected set { throw new NotSupportedException(); } }
      public override void Paint(Graphics g) {;}

      protected Size         GridSize      { get; private set; }
      protected GraphicsPath HexgridPath   { get; set; }
    }

    public sealed class PathMazeGridHex : MazeGridHex {
      public PathMazeGridHex(MapDisplay board, ICoords coords, Size gridSize) 
        : base(board, coords, gridSize) {}
      public override int  Elevation      { get { return 0; } }
      public override int  HeightTerrain  { get { return ElevationASL + 0; } }
      public override int  StepCost(Hexside direction) { return  1; }
    }

    public sealed class WallMazeGridHex : MazeGridHex {
      public WallMazeGridHex(MapDisplay board, ICoords coords, Size gridSize) 
        : base(board, coords, gridSize) {}
      public override int  Elevation      { get { return 1; } }
      public override int  HeightTerrain  { get { return ElevationASL + 10; } }
      public override int  StepCost(Hexside direction) { return -1; }

      public override void Paint(Graphics g) {
        using(var brush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
          g.FillPath(brush, HexgridPath);
      }
    }
  }
}
