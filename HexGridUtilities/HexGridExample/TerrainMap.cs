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
  public sealed class TerrainMap : MapDisplay {
    public TerrainMap() : base() { GridHex.MyBoard = this; }

    public override int    StepCost(ICoordsCanon coords, Hexside hexSide) {
      switch (this[coords.StepOut(hexSide)].Value) {
        case '2': return  2;
        case '3': return  3;
        case 'F': return  5;
        case 'R': return -1;
        case 'W': return  8;
        case 'H': return  5;
        case 'M': return  6;
        default:  return  4;
      }
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
            var coords = Coords.NewUserCoords(x,y);
            switch (this[coords].Value) {
              case 'F': g.FillPath(Brushes.Brown,     HexgridPath); break;  // Ford
              case 'R': g.FillPath(Brushes.DarkBlue,  HexgridPath); break;  // River
              case 'W': g.FillPath(Brushes.Green,     HexgridPath); break;  // Woods
              case 'H': g.FillPath(Brushes.Khaki,     HexgridPath); break;  // Hill
              case 'M': g.FillPath(Brushes.DarkKhaki, HexgridPath); break;  // Mountain
              default:  g.FillPath(Brushes.White,     HexgridPath); break;  // Clear
            }
            switch (this[coords].Value) {
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

    public override IGridHex this[ICoordsCanon coords] { get {return this[coords.User];} }
    public override IGridHex this[ICoordsUser  coords] { get {
      return new GridHex(Board[coords.Y][coords.X], coords);
    } }

    public struct GridHex : IGridHex {
      internal static IBoard<IGridHex> MyBoard { get; set; }

      public GridHex(char value, ICoordsUser coords) : this() { Value = value; Coords = coords; }

      public IBoard<IGridHex> Board          { get { return MyBoard; } }
      public ICoordsUser      Coords         { get; private set; }
      public int              Elevation      {
        get {
          switch (Value) {
            default:  return  0;
            case 'H': return  1;
            case 'M': return  2;
          }
        }
      }
      public int              ElevationASL   { get { return Elevation * 10; } }
      public int              HeightObserver { get { return ElevationASL + 1; } }
      public int              HeightTarget   { get { return ElevationASL + 1; } }
      public int              HeightTerrain  { 
        get {
          switch (Value) {
            default:  return  ElevationASL;
            case 'W': return  ElevationASL + 7;
          }
        }
      }
      public char             Value          { get; private set; }
      public IEnumerable<NeighbourHex> GetNeighbours() {
        var @this = this;
        return NeighbourHex.GetNeighbours(@this).Where(n=>@this.Board.IsOnBoard(n.Hex.Coords));
      }
    }
  }
}
