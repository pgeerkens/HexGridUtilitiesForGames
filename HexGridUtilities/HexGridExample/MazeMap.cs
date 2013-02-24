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
  public sealed class MazeMap : MapDisplay {
    public MazeMap() : base() { GridHex.MyBoard = this; }

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
            var coords = Coords.NewUserCoords(x,y);
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

    public override IGridHex this[ICoordsCanon coords] { get {return this[coords.User];} }
    public override IGridHex this[ICoordsUser  coords] { get {
      return new GridHex(Board[coords.Y][coords.X], coords);
    } }

    public struct GridHex : IGridHex {
      internal static IBoard<IGridHex> MyBoard { get; set; }

      public GridHex(char value, ICoordsUser coords) : this() { Value = value; Coords = coords; }

      public IBoard<IGridHex> Board          { get { return MyBoard; } }
      public ICoordsUser      Coords         { get; private set; }
      public int              Elevation      { get { return Value == '.' ? 0 : 1; } }
      public int              ElevationASL   { get { return Elevation * 10;   } }
      public int              HeightObserver { get { return ElevationASL + 1; } }
      public int              HeightTarget   { get { return ElevationASL + 1; } }
      public int              HeightTerrain  { get { return ElevationASL + (Value == '.' ? 0 : 10); } }
      public char             Value          { get; private set; }
      public IEnumerable<NeighbourHex> GetNeighbours() {
        var @this = this;
        return NeighbourHex.GetNeighbours(@this).Where(n=>@this.Board.IsOnBoard(n.Hex.Coords));
      }
    }
  }
}
