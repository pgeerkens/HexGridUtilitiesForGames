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
using System.Windows;
using System.Windows.Media;

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridScrollViewer {
  using HexSize  = System.Drawing.Size;

  public class HexgridModel : MapDisplay<MapGridHex> {
    /// <summary>TODO</summary>
    public HexgridModel(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex)
      : base(sizeHexes, gridSize, initializeHex) {
    }
  }

  /// <summary>TODO</summary>
  public sealed class EmptyBoard : HexgridModel {
    /// <summary>TODO</summary>
    public EmptyBoard() : base(new HexSize(1,1), new HexSize(26,30), (mb,c) => new EmptyGridHex(mb,c)) {
      FovRadius = 20;
    }
  }

  /// <summary>TODO</summary>
  public sealed class EmptyGridHex : MapGridHex, IHex  {
    /// <summary>TODO</summary>
    public EmptyGridHex(HexBoard<MapGridHex,StreamGeometry> board, HexCoords coords) : base(board, coords) {
      Board = board;
    }

    /// <summary>The Board on which this <see cref="Hex{TDrawingSurface,TPath}"/> is located</summary>
    public new HexBoard<MapGridHex,StreamGeometry> Board { get; private set; }

      ///  <inheritdoc/>
    public override StreamGeometry  HexgridPath   { get {return Board.HexgridPath;} }

    /// <summary>TODO</summary>
    public override int             HeightTerrain { get { return ElevationASL;   } }
    /// <summary>TODO</summary>
    public override int             StepCost(Hexside direction) { return -1; }

      ///  <inheritdoc/>
    public override void            Paint(DrawingContext graphics) { ; }
  }
}
