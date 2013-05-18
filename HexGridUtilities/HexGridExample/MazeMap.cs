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

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.HexGridExample {
  public sealed class MazeMap : MapDisplay {
    public MazeMap() : base() { MazeGridHex.MyBoard = this; }

    public override bool  IsPassable(ICoordsUser coords) { 
      return IsOnBoard(coords)  &&  this[coords].Elevation == 0; 
    }

    public override int    Heuristic(int range) { return range; }
    public override int    StepCost(ICoordsCanon coords, Hexside hexSide) {
      return ( IsOnBoard(coords.User) && this[coords.StepOut(hexSide)].Value=='.' ? 1 : -1 );
    }

    #region Painting
    public override void PaintMap(Graphics g) { 
      var clipCells = GetClipCells(g.VisibleClipBounds);
      var state     = g.Save();
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      g.TranslateTransform(MapMargin.Width + clipCells.Right*GridSize.Width, MapMargin.Height);

      using(var fillBrush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
      using(var font   = new Font("ArialNarrow", 8))
      using(var format = new StringFormat()) {
        format.Alignment = format.LineAlignment = StringAlignment.Center;
        for (int x=clipCells.Right; x-->clipCells.Left; ) {
          g.TranslateTransform(-GridSize.Width, 0);
          var container = g.BeginContainer();
          g.TranslateTransform(0,  clipCells.Top*GridSize.Height + (x+1)%2 * (GridSize.Height)/2);
          for (int y=clipCells.Top; y<clipCells.Bottom; y++) {
            var coords = HexCoords.NewUserCoords(x,y);
            if (this[coords].Value!='.') {
              g.FillPath(fillBrush, HexgridPath);
            }
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

    public override IMapGridHex this[ICoordsCanon coords] { get {return this[coords.User];} }
    public override IMapGridHex this[ICoordsUser  coords] { get {
      return new MazeGridHex(Board[coords.Y][coords.X], coords);
    } }

    public sealed class MazeGridHex : MapGridHex {
      public MazeGridHex(char value, ICoordsUser coords) : base(value, coords) { }

      public override int  Elevation      { get { return Value == '.' ? 0 : 1; } }
      public override int  HeightTerrain  { get { return ElevationASL + (Value == '.' ? 0 : 10); } }
      public override int  StepCost       { get { return 1; } }
    }
  }
}
