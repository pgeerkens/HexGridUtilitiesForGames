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
using System.Collections.Generic;
using System.Drawing;

using PGNapoleonics.HexUtilities;

/// <summary>Example of <see cref="HexUtilities"/> usage with <see cref="HexUtilities.HexgridPanel"/> to implement
/// a terrain map.</summary>
namespace PGNapoleonics.HexGridExample2.TerrainExample {
  internal sealed class TerrainMap : MapDisplay<MapGridHex> {
    public TerrainMap() : base(_sizeHexes, (map,coords) => InitializeHex(map,coords)) {}

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2233:OperationsShouldNotOverflow", MessageId = "10*elevationLevel")]
    public override int   ElevationASL(int elevationLevel) { return 10 * elevationLevel; }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2233:OperationsShouldNotOverflow", MessageId = "2*range")]
    public override int   Heuristic(int range) { return 2 * range; }

    /// <inheritdoc/>
    public override void PaintUnits(Graphics g) { ; }

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

    private static MapGridHex InitializeHex(HexBoard<MapGridHex> board, HexCoords coords) {
      char value = _board[coords.User.Y][coords.User.X];
      switch(value) {
        default:   
        case '.':  return new ClearTerrainGridHex   (board, coords);
        case '2':  return new PikeTerrainGridHex    (board, coords);
        case '3':  return new RoadTerrainGridHex    (board, coords);
        case 'F':  return new FordTerrainGridHex    (board, coords);
        case 'H':  return new HillTerrainGridHex    (board, coords);
        case 'M':  return new MountainTerrainGridHex(board, coords);
        case 'R':  return new RiverTerrainGridHex   (board, coords);
        case 'W':  return new WoodsTerrainGridHex   (board, coords);
      }
    }
  }
}
