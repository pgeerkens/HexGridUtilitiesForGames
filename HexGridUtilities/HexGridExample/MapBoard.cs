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
using System.Linq;
using System.Text;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.HexGridExample {
  public abstract class MapBoard : IMapBoard {
    public ICoordsUser         CurrentHex   { get; set; }
    public ICoordsUser         HotSpotHex   { get; set; }
    public IPath<ICoordsCanon> Path         { get; set; }
    public Size                SizeHexes    { get {return new Size(Board[0].Length,Board.Length);} }

    public char   this[ICoordsCanon coords] { get {return this[coords.User];} }
    public char   this[ICoordsUser coords]  { get {return Board[coords.Y][coords.X];} }

    public string HexText(ICoordsUser coords)   { return HexText(coords.X, coords.Y); }
    public bool   IsOnBoard(ICoordsUser coords) {
      return 0<=coords.X && coords.X < SizeHexes.Width
          && 0<=coords.Y && coords.Y < SizeHexes.Height;
    }
    public int    StepCost(ICoordsCanon coords, Hexside hexSide) {
      return ( IsOnBoard(coords.User) && this[coords.StepOut(hexSide)]=='.' ? 1 : -1 );
    }

    #region Board definition
    string[]     Board = new string[] {
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
    string HexText(int x, int y)            { return string.Format("{0,2}-{1,2}", x, y); }
  } 
}
