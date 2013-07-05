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

namespace PGNapoleonics.HexGridExample2 {
  internal sealed class TerrainMap : MapDisplay {
    public TerrainMap() : base(_sizeHexes, (map,coords) => InitializeHex(map,coords)) {}

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2233:OperationsShouldNotOverflow", MessageId = "10*elevationLevel")]
    public override int   ElevationASL(int elevationLevel) { return 10 * elevationLevel; }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2233:OperationsShouldNotOverflow", MessageId = "2*range")]
    public override int   Heuristic(int range) { return 2 * range; }

    #region Painting
    public override void PaintMap(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      var clipCells = GetClipCells(g.VisibleClipBounds);
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
            this[HexCoords.NewUserCoords(x,y)].Paint(g);
            g.DrawPath(Pens.Black, HexgridPath);

            g.TranslateTransform(0,GridSize.Height);
          }
          g.EndContainer(container);
        }
      }
    }
    public override void PaintUnits(Graphics g) { ; }
    #endregion

    #region static Board definition
    static List<string> _board = new List<string>() {
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
    static Size _sizeHexes = new Size(_board[0].Length, _board.Count);
    #endregion

    private static MapGridHex InitializeHex(IBoard<MapGridHex> board, HexCoords coords) {
      char value = _board[coords.User.Y][coords.User.X];
      switch(value) {
        default:   
        case '.':  return new ClearTerrainGridHex   (board, coords, board.GridSize);
        case '2':  return new PikeTerrainGridHex    (board, coords, board.GridSize);
        case '3':  return new RoadTerrainGridHex    (board, coords, board.GridSize);
        case 'F':  return new FordTerrainGridHex    (board, coords, board.GridSize);
        case 'H':  return new HillTerrainGridHex    (board, coords, board.GridSize);
        case 'M':  return new MountainTerrainGridHex(board, coords, board.GridSize);
        case 'R':  return new RiverTerrainGridHex   (board, coords, board.GridSize);
        case 'W':  return new WoodsTerrainGridHex   (board, coords, board.GridSize);
      }
    }
  }
}
