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
using System.Drawing;
using System.Windows.Media;

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridScrollViewer {
  /// <summary>Deprecated; use interface IHex or abstract class MapGridHex instead.</summary>
//  [Obsolete("Use interface IHex or abstract class MapGridHex instead.")]
  public interface IMapGridHex : IHex {
  }

  /// <summary>TODO</summary>
  public abstract class MapGridHex : Hex<IMapGridHex>, IMapGridHex {
    /// <summary>TODO</summary>
    protected MapGridHex(HexBoardWpf<MapGridHex> board, HexCoords coords) : base(coords) { 
      BoardX = board;
    }

    /// <inheritdoc/>
    public override IBoard<IHex>                Board     { get { return BoardX; } }

    /// <inheritdoc/>
    public virtual HexBoardWpf<MapGridHex> BoardX    {  get; private set; }

    /// <summary>TODO</summary>
    protected  Size                             GridSize  { get { return Board.GridSize; } }

    /// <inheritdoc/>
    public override int  DirectedStepCost(Hexside hexsideExit) {
      return Board[Coords.GetNeighbour(hexsideExit)].StepCost(hexsideExit);
    }

    /// <inheritdoc/>
    public override IHex Neighbour(Hexside hexside) { return Board[Coords.GetNeighbour(hexside)]; }

    /// <inheritdoc/>
    public virtual  void Paint(DrawingContext g) {;}
  }
}
