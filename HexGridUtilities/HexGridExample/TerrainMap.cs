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
  public sealed class TerrainMap : MapDisplay {
    public TerrainMap() : base() { TerrainGridHex.MyBoard = this; }

    public override int    Heuristic(int range) { return 2 * range; }
    public override int    StepCost(ICoordsCanon coords, Hexside hexSide) {
      return this[coords].StepCost;
    }

    #region Painting
    public override void PaintMap(Graphics g) { 
      var clipCells = GetClipCells(g.VisibleClipBounds);
      var state     = g.Save();
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      g.TranslateTransform(MapMargin.Width + clipCells.Right*GridSize.Width, MapMargin.Height);

      using(var pikeBrush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
      using(var roadBrush = new SolidBrush(Color.FromArgb(78,Color.SaddleBrown)))
      using(var font   = new Font("ArialNarrow", 8))
      using(var format = new StringFormat()) {
        format.Alignment = format.LineAlignment = StringAlignment.Center;
        for (int x=clipCells.Right; x-->clipCells.Left; ) {
          g.TranslateTransform(-GridSize.Width, 0);
          var container = g.BeginContainer();
          g.TranslateTransform(0,  clipCells.Top*GridSize.Height + (x+1)%2 * (GridSize.Height)/2);
          for (int y=clipCells.Top; y<clipCells.Bottom; y++) {
            var coords = HexCoords.NewUserCoords(x,y);
            switch (this[coords].Value) {
              case 'F': g.FillPath(Brushes.Brown,     HexgridPath); break;  // Ford
              case 'R': g.FillPath(Brushes.DarkBlue,  HexgridPath); break;  // River
              case 'W': g.FillPath(Brushes.Green,     HexgridPath); break;  // Woods
              case 'H': g.FillPath(Brushes.Khaki,     HexgridPath); break;  // Hill
              case 'M': g.FillPath(Brushes.DarkKhaki, HexgridPath); break;  // Mountain
              case '2': g.FillPath(pikeBrush, HexgridPath); break;
              case '3': g.FillPath(roadBrush, HexgridPath); break;
              default:  break;
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
      "...................3.......22...........R..............",
      "...................3.........222222.....R..............",
      "...................3..............2.....R..............",
      "...................33..............22222F.2.2..........",
      "............RR......3...................R2.2.2222......",
      "...........RRRR.....3.....HHHH..........R........22....",
      "..........RRRRRRR...3....HHMMHH.........R..........22..",
      "..........RRRRRRR....3....HHHH..........R............22",
      "...........RRRR......3..................R..............",
      "............RR.......3..........RRRRRRRR...............",
      ".....................3.........R............WW.........",
      ".....................3.........R...........WWWWW.......",
      ".....................3.........R............WWW........",
      ".....................3.........R.......................",
      ".....................3.........RRRRRRRR................",
      "..........WWWWWWW....3.................R...............",
      "........WWWWWWWWW....33................R...............",
      ".......WWWWWWWWW.....3.33..............R...............",
      "..........WWWWWWW....3...32222222222222F22222..........",
      "...........WWWWWWW...3...2.............R.....222222....",
      "............WWWWW....32222.............R...........22..",
      "....................22....HHHH........RR.............22",
      "................2222.....HHMMHH.....RR.......WW........",
      "............2222..........HHMHH...RR.......WWWWW.......",
      "........2222...............HHH..RR.........WWWW........",
      "......22......................RR............WW.........",
      "..2222......................RR.........................",
      "22..................RRRRRRRR...........................",
      "..................RR...................................",
      ".................RRR..................................."
    };
    #endregion

    public override IMapGridHex this[ICoordsCanon coords] { get {return this[coords.User];} }
    public override IMapGridHex this[ICoordsUser  coords] { get {
      return GetGridHex(Board[coords.Y][coords.X], coords);
    } }

    public static TerrainGridHex GetGridHex(char value, ICoordsUser coords) {
      switch(value) {
      default:  return new ClearTerrainGridHex(value, coords);
      case '2': return new RoadTerrainGridHex(value, coords);
      case '3': return new TrailTerrainGridHex(value, coords);
      case 'R': return new RiverTerrainGridHex(value, coords);
      case 'W': return new WoodsTerrainGridHex(value, coords);
      case 'F': return new FordTerrainGridHex(value, coords);
      case 'H': return new HillTerrainGridHex(value, coords);
      case 'M': return new MountainTerrainGridHex(value, coords);
      }
    }
  }
}
