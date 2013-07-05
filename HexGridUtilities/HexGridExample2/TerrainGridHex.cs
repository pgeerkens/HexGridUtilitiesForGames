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
  internal abstract class TerrainGridHex : MapGridHex {

    protected TerrainGridHex(IBoard<IHex> map, HexCoords coords, Size gridSize) 
      : base(map, coords) { 
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

    protected Size         GridSize      { get; private set; }
    protected GraphicsPath HexgridPath   { get; set; }

    public override void Paint(Graphics g) {;}
    public override int  Elevation     { get { return  0; }  }
    public override int  ElevationASL  { get { return Board.ElevationASL(Elevation); } }
    public override int  HeightTerrain { get { return ElevationASL; } }
    public override int  StepCost(Hexside direction) { return  4; }
  }
  internal sealed class ClearTerrainGridHex    : TerrainGridHex {
    public ClearTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override void Paint(Graphics g) { ; }
  }
  internal sealed class FordTerrainGridHex     : TerrainGridHex {
    public FordTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int StepCost(Hexside direction) { return  5; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      g.FillPath(Brushes.Brown, HexgridPath); 
    }
  }
  internal sealed class RiverTerrainGridHex    : TerrainGridHex {
    public RiverTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int StepCost(Hexside direction) { return -1; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      g.FillPath(Brushes.DarkBlue, HexgridPath); 
    }
  }
  internal sealed class PikeTerrainGridHex     : TerrainGridHex {
    public PikeTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int StepCost(Hexside direction) { return  2; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      using(var brush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
        g.FillPath(brush, HexgridPath); 
    }
  }
  internal sealed class RoadTerrainGridHex     : TerrainGridHex {
    public RoadTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int StepCost(Hexside direction) { return  3; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      using(var brush = new SolidBrush(Color.FromArgb(78,Color.SaddleBrown)))
        g.FillPath(brush, HexgridPath); 
    }
  }
  internal sealed class HillTerrainGridHex     : TerrainGridHex {
    public HillTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int Elevation      { get { return  1; } }
    public override int StepCost(Hexside direction) { return  5; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      g.FillPath(Brushes.Khaki, HexgridPath); 
    }
  }
  internal sealed class MountainTerrainGridHex : TerrainGridHex {
    public MountainTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int Elevation      { get { return  2; } }
    public override int StepCost(Hexside direction) { return  6; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      g.FillPath(Brushes.DarkKhaki, HexgridPath); 
    }
  }
  internal sealed class WoodsTerrainGridHex    : TerrainGridHex {
    public WoodsTerrainGridHex(IBoard<MapGridHex> map, HexCoords coords, Size gridSize) : base(map, coords, gridSize) { }
    public override int HeightTerrain  { get { return ElevationASL + 7; } }
    public override int StepCost(Hexside direction) { return  8; }
    public override void Paint(Graphics g) { 
      if (g==null) throw new ArgumentNullException("g");
      g.FillPath(Brushes.Green, HexgridPath); 
    }
  }
}
