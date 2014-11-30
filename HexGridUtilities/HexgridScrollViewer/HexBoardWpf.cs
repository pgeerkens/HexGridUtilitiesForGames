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
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexgridScrollViewer {
  using HexSize = System.Drawing.Size;

  public abstract class HexBoardWpf<THex> : HexBoard<THex,StreamGeometry> where THex : class, IHex {
    /// <summary>TODO</summary>
    private static StreamGeometry GetGraphicsPath(HexSize gridSize) {
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.EvenOdd;

      using (var context = geometry.Open()) {
        context.BeginFigure(new Point(gridSize.Width*1/3,                0),false,true);
        context.LineTo     (new Point(gridSize.Width*3/3,                0),true,false);
        context.LineTo     (new Point(gridSize.Width*4/3,gridSize.Height/2),true,false);
        context.LineTo     (new Point(gridSize.Width*3/3,gridSize.Height  ),true,false);
        context.LineTo     (new Point(gridSize.Width*1/3,gridSize.Height  ),true,false);
        context.LineTo     (new Point(                 0,gridSize.Height/2),true,false);
      }
      geometry.Freeze();

      return geometry;
    }

    #region Constructors
    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex}"/> with default 
    /// landmarks for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="initializeBoard">Delegate that creates the <see cref="BoardStorage{T}"/> backing
    /// store for this instance.</param>
    protected HexBoardWpf(HexSize sizeHexes, HexSize gridSize) 
    : this(sizeHexes, gridSize, DefaultLandmarks(sizeHexes)) { }

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
    protected HexBoardWpf(HexSize sizeHexes, HexSize gridSize, IFastList<HexCoords> landmarkCoords)
    : base(sizeHexes, gridSize,landmarkCoords,GetGraphicsPath) { }
    #endregion
  }
}
