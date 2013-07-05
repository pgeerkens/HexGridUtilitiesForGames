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
  internal abstract class MazeGridHex : MapGridHex {
    protected MazeGridHex(IBoard<MapGridHex> board, HexCoords coords, Size gridSize) 
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

    public override int    ElevationASL  { get { return Elevation * 10; } }
    public override void   Paint(Graphics g) {;}

    protected Size         GridSize      { get; private set; }
    protected GraphicsPath HexgridPath   { get; set; }
  }

  internal sealed class PathMazeGridHex : MazeGridHex {
    public PathMazeGridHex(IBoard<MapGridHex> board, HexCoords coords, Size gridSize) 
      : base(board, coords, gridSize) {}
    public override int  Elevation      { get { return 0; } }
    public override int  HeightTerrain  { get { return ElevationASL + 0; } }
    public override int  StepCost(Hexside direction) { return  1; }
  }

  internal sealed class WallMazeGridHex : MazeGridHex {
    public WallMazeGridHex(IBoard<MapGridHex> board, HexCoords coords, Size gridSize) 
      : base(board, coords, gridSize) {}
    public override int  Elevation      { get { return 1; } }
    public override int  HeightTerrain  { get { return ElevationASL + 10; } }
    public override int  StepCost(Hexside direction) { return -1; }

    public override void Paint(Graphics g) {
      if (g==null) throw new ArgumentNullException("g");
      using(var brush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
        g.FillPath(brush, HexgridPath);
    }
  }
}
