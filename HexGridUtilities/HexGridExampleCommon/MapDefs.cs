#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexgridExampleCommon {
  /// <summary>TODO</summary>
  public static class MapDefinitions {

    #region Map Definitions
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly IList<string> TerrainMapDefinition = new List<string>() {
      "...................3.......22...........R..............",
      "...................3.........222222.....R..............",
      "...................3..............2.....R..............",
      "...................33..............22222F.2.2..........",
      "..........WWRR......3...................R2.2.2222......",
      ".........WWRRRR.....3.....HHHH..........R........22....",
      ".........WRRRRRRR...3....HHMMHH.........R..........22..",
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
    }.AsReadOnly();

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly IList<string> MazeMapDefinition = new List<string>() {
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
    }.AsReadOnly();

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly IList<string> AStarBugMapDefinition = new List<string>() {
      ".2333333333R..........2",
      ".2........R33333333.22.",
      ".2.......R........22...",
      ".2......RR......22...RR",
      "R2RRRRRR.......2.......",
      ".2.....R.......2....RR.",
      ".2.....R.......2.....RR.",
      ".2.....R.......2......R."
    }.AsReadOnly();
    #endregion

  }
}
