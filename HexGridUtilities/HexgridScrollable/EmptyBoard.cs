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
using System.Drawing;

using PGNapoleonics.HexUtilities;


namespace PGNapoleonics.HexgridPanel {

  /// <summary>TODO</summary>
  public sealed class EmptyBoard : MapDisplay<MapGridHex> {
    /// <summary>TODO</summary>
    public EmptyBoard() : base(new Size(1,1), new Size(26,30), (mb,c) => new EmptyGridHex(mb,c)) {
      FovRadius = 20;
    }
  }

  /// <summary>TODO</summary>
  public sealed class EmptyGridHex : MapGridHex {
    /// <summary>TODO</summary>
    public EmptyGridHex(HexBoard<MapGridHex> board, HexCoords coords) : base(board, coords) {}

    /// <summary>TODO</summary>
    public override int ElevationASL  { get { return 10 * Elevation; } }
    /// <summary>TODO</summary>
    public override int HeightTerrain { get { return ElevationASL;   } }
    /// <summary>TODO</summary>
    public override int StepCost(Hexside direction) { return -1; }
  }
}
