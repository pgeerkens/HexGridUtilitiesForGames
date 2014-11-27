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
using System;
using System.Drawing.Drawing2D;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexgridPanel {
  using HexPoint    = System.Drawing.Point;
  using HexSize     = System.Drawing.Size;

  /// <summary>TODO</summary>
  public delegate BoardStorage<THex> InitializeBoard<THex>(HexBoardWinForms<THex> board) where THex : class, IHex;

  /// <summary>TODO</summary>
  public abstract class HexBoardWinForms<THex> : HexBoard<THex> where THex : class, IHex {
    #region Constructors
    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex}"/> with default 
    /// landmarks for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="initializeBoard">Delegate that creates the <see cref="BoardStorage{T}"/> backing
    /// store for this instance.</param>
    protected HexBoardWinForms(HexSize sizeHexes, HexSize gridSize, 
                               InitializeBoard<THex> initializeBoard) 
    : this(sizeHexes, gridSize, initializeBoard, DefaultLandmarks(sizeHexes)) {}

    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex}"/> with the specified set of 
    /// landmarks for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="initializeBoard">Delegate that creates the <see cref="BoardStorage{T}"/> backing
    /// store for this instance.</param>
    /// <param name="landmarkCoords">Collection of <see cref="HexCoords"/> specifying the landmark 
    /// locations to be used for pathfinding.</param>
    protected HexBoardWinForms(HexSize sizeHexes, HexSize gridSize, 
                               InitializeBoard<THex> initializeBoard, 
                               IFastList<HexCoords> landmarkCoords)
    : base(sizeHexes, gridSize, landmarkCoords) {
      if (initializeBoard==null) throw new ArgumentNullException("initializeBoard");

      BoardHexes = initializeBoard(this); 

      ResetGrid();
      HexgridPath       = GetGraphicsPath(GridSize);
    }
    #endregion

      ///  <inheritdoc/>
    public GraphicsPath                 HexgridPath       { get; private set; }

    /// <summary>TODO</summary>
    private static GraphicsPath GetGraphicsPath(HexSize gridSize) {
      GraphicsPath path     = null;
      GraphicsPath tempPath = null;
      try {
        tempPath  = new GraphicsPath();
        tempPath.AddLines(new HexPoint[] {
          new HexPoint(gridSize.Width*1/3,              0  ), 
          new HexPoint(gridSize.Width*3/3,              0  ),
          new HexPoint(gridSize.Width*4/3,gridSize.Height/2),
          new HexPoint(gridSize.Width*3/3,gridSize.Height  ),
          new HexPoint(gridSize.Width*1/3,gridSize.Height  ),
          new HexPoint(             0,    gridSize.Height/2),
          new HexPoint(gridSize.Width*1/3,              0  )
        } );
        path     = tempPath;
        tempPath = null;
      } finally { if(tempPath!=null) tempPath.Dispose(); }
      return path;
    }
  }
}
