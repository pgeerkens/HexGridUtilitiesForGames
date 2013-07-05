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
using System.Threading.Tasks;

using PGNapoleonics;
using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexGridExample2 {
  internal sealed class MazeMap : MapDisplay {
    public MazeMap() : base(_sizeHexes, (map,coords) => InitializeHex(map,coords)) {}

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2233:OperationsShouldNotOverflow", MessageId = "10*elevationLevel")]
    public override int   ElevationASL(int elevationLevel) { return 10 * elevationLevel; }
    /// <inheritdoc/>
    public override int   Heuristic(int range) { return range; }
    /// <inheritdoc/>
    public override bool  IsPassable(HexCoords coords) { 
      return IsOnboard(coords)  &&  this[coords].Elevation == 0; 
    }

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
    static Size _sizeHexes = new Size(_board[0].Length, _board.Count);
    #endregion

    private static MapGridHex InitializeHex(IBoard<MapGridHex> board, HexCoords coords) {
      switch (_board[coords.User.Y][coords.User.X]) {
        case '.': return new PathMazeGridHex(board, coords, board.GridSize);
        default:  return new WallMazeGridHex(board, coords, board.GridSize);
      }
    }
  }
}
