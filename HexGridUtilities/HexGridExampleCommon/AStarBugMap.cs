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
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridExampleCommon {
  using MapGridHex      = Hex<Graphics,GraphicsPath>;

    /// <summary>TODO</summary>
  public sealed class AStarBugMap : MapDisplay<MapGridHex> {
    /// <summary>TODO</summary>
     public AStarBugMap() : base(_sizeHexes, new Size(26,30), InitializeHex) { }

    /// <inheritdoc/>
   [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "2*range")]
    public override int   Heuristic(int range) { return 2 * range; }

    /// <inheritdoc/>
    public override void PaintUnits(Graphics graphics) { ; }

    #region static Board definition
    static IList<string> _board     = MapDefinitions.AStarBugMapDefinition;
    static Size          _sizeHexes = new Size(_board[0].Length, _board.Count);
    #endregion

    private new static MapGridHex InitializeHex(GraphicsPath hexgridPath, HexCoords coords) {
      char value = _board[coords.User.Y][coords.User.X];
      switch(value) {
        default:   
        case '.':  return new ClearTerrainGridHex   (hexgridPath, coords);
        case '2':  return new PikeTerrainGridHex    (hexgridPath, coords);
        case '3':  return new RoadTerrainGridHex    (hexgridPath, coords);
        case 'F':  return new FordTerrainGridHex    (hexgridPath, coords);
        case 'H':  return new HillTerrainGridHex    (hexgridPath, coords);
        case 'M':  return new MountainTerrainGridHex(hexgridPath, coords);
        case 'R':  return new RiverTerrainGridHex   (hexgridPath, coords);
        case 'W':  return new WoodsTerrainGridHex   (hexgridPath, coords);
      }
    }
  }
}
