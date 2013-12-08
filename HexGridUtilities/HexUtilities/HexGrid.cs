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
using System.Drawing.Drawing2D;

using PGNapoleonics.HexUtilities.Common;

/// <summary>Display-technology-independent utilities for implementation of hex-grids..</summary>
namespace PGNapoleonics.HexUtilities {
  /// <summary>Interface defining the functionality required of a form or control hosting an instannce of <see cref="Hexgrid"/>.</summary>
  public interface IHexgridHost {  }

  /// <summary>TODO</summary>
  public interface IHexgrid {
    /// <summary>TODO</summary>
    Point GetScrollPosition(Point scrollPosition);
    /// <summary>TODO</summary>
    Size  GetSize(Size mapSizePixels, float mapScale);

    /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    HexCoords GetHexCoords(Point point, Size autoScroll);

    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <param name="visibleRectangle"></param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    Point   ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex, CoordsRectangle visibleRectangle);

    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    Point   HexCenterPoint(HexCoords coordsNewULHex);

    /// <summary>TODO</summary>
    SizeF GridSizeF  { get; }
    /// <summary>Offset of grid origin, from control's client-area origin.</summary>
    Size  Margin     { get; set; }
}

  /// <summary>C# implementation of the hex-picking algorithm noted below.</summary>
  /// <remarks>Mathemagically (left as exercise for the reader) our 'picking' matrices are these, assuming: 
  ///  - origin at upper-left corner of hex (0,0);
  ///  - 'straight' hex-axis vertically down; and
  ///  - 'oblique'  hex-axis up-and-to-right (at 120 degrees from 'straight').</remarks>
  /// <a href="file://Documentation/HexGridAlgorithm.mht">Hex-grid Algorithms</a>
  public class Hexgrid : IHexgrid {
    /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
    public Hexgrid(SizeF gridSizeF) : this(gridSizeF, Size.Empty) {}
    /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
    public Hexgrid(SizeF gridSizeF, Size margin) { 
      GridSizeF = gridSizeF;
      Margin    = margin;
    }

    #region Properties
    /// <inheritdoc/>
    public SizeF  GridSizeF { get; private set; }

    /// <inheritdoc/>
    public Size   Margin    { get; set; }

    /// <summary>Matrix2D for 'picking' the <B>X</B> hex coordinate</summary>
    Matrix matrixX { 
      get { return new Matrix(
          (3.0F/2.0F)/GridSizeF.Width,  (3.0F/2.0F)/GridSizeF.Width,
                 1.0F/GridSizeF.Height,       -1.0F/GridSizeF.Height,  -0.5F,-0.5F); } 
    }
    /// <summary>Matrix2D for 'picking' the <B>Y</B> hex coordinate</summary>
    Matrix matrixY { 
      get { return new Matrix(
                0.0F,                   (3.0F/2.0F)/GridSizeF.Width,
                2.0F/GridSizeF.Height,         1.0F/GridSizeF.Height,  -0.5F,-0.5F); } 
    }
    #endregion

    /// <summary>Scroll position on the (possibly transposed) HexGrid.</summary>
    public virtual Point GetScrollPosition(Point scrollPosition) { return scrollPosition; }

    /// <inheritdoc/>
    public virtual Size  GetSize(Size mapSizePixels, float mapScale) {
      return Size.Ceiling(mapSizePixels.Scale(mapScale)); 
    }

    /// <inheritdoc/>
    public virtual HexCoords GetHexCoords(Point point, Size autoScroll) {
//      if( Host == null ) return HexCoords.EmptyCanon;

      // Adjust for origin not as assumed by GetCoordinate().
      var grid    = new Size((int)(GridSizeF.Width*2F/3F), (int)GridSizeF.Height);
      point      -= autoScroll + grid - Margin;

      return HexCoords.NewCanonCoords( GetCoordinate(matrixX, point), 
                                       GetCoordinate(matrixY, point) );
    }

    /// <inheritdoc/>
    public virtual Point   ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex, CoordsRectangle visibleRectangle) {
      return HexCenterPoint(HexCoords.NewUserCoords(
              coordsNewCenterHex.User - ( new IntVector2D(visibleRectangle.Size.User) / 2 )
      ));
    }

    /// <summary>Calculates a (canonical X or Y) grid-coordinate for a point, from the supplied 'picking' matrix.</summary>
    /// <param name="matrix">The 'picking' matrix</param>
    /// <param name="point">The screen point identifying the hex to be 'picked'.</param>
    /// <returns>A (canonical X or Y) grid coordinate of the 'picked' hex.</returns>
	  static int GetCoordinate (Matrix matrix, Point point){
      var pts = new Point[] {point};
      matrix.TransformPoints(pts);
		  return (int) Math.Floor( (pts[0].X + pts[0].Y + 2F) / 3F );
	  }

    /// <inheritdoc/>
    public virtual Point   HexCenterPoint(HexCoords coordsNewULHex) {
      if (coordsNewULHex == null) return new Point();
      var offset = new Size((int)(GridSizeF.Width*2F/3F), (int)GridSizeF.Height);
      return HexOrigin(coordsNewULHex) + offset;
    }

    /// <summary>Returns the pixel coordinates of the center of the specified hex.</summary>
    /// <param name="coords"><see cref="HexCoords"/> specification for which pixel center is desired.</param>
    /// <returns>Pixel coordinates of the center of the specified hex.</returns>
    Point HexOrigin(HexCoords coords) {
      return new Point(
        (int)(GridSizeF.Width  * coords.User.X),
        (int)(GridSizeF.Height * coords.User.Y   + GridSizeF.Height/2 * (coords.User.X+1)%2)
      );
    }
  }

  /// <summary>A transposed hexgrid with pointy-top/flat-sides hexes; the rectangular x-axis running 
  /// verticallly down; and the rectnaagular y-axis running out to the right from teh upper-left corner 
  /// (which remains coordinate (0,0) for both User (rectangular) and Canon (obtuse) coordinate frames.</summary>
  public class TransposedHexgrid : Hexgrid {
    /// <summary>Returns a <c>TransposedHexgrid</c> instance from the supplied <see cref="IHexgridHost"/>.</summary>
    public TransposedHexgrid(SizeF gridSizeF) : this(gridSizeF, Size.Empty) {}
    /// <summary>Returns a <c>TransposedHexgrid</c> instance from the supplied <see cref="IHexgridHost"/>.</summary>
    public TransposedHexgrid(SizeF gridSizeF, Size margin) : base(gridSizeF, margin) {}

    ///<inheritdoc/>
    public override Point GetScrollPosition(Point scrollPosition) { 
      return TransposePoint(scrollPosition); 
    }

    ///<inheritdoc/>
    public override Size  GetSize(Size mapSizePixels, float mapScale) { 
      return TransposeSize(base.GetSize(mapSizePixels, mapScale)); 
    }

    ///<inheritdoc/>
    public override HexCoords GetHexCoords(Point point, Size autoScroll) {
      return base.GetHexCoords(TransposePoint(point), TransposeSize(autoScroll));
    }
    ///<inheritdoc/>
    public override Point HexCenterPoint(HexCoords coordsNewULHex) {
      return TransposePoint(base.HexCenterPoint(coordsNewULHex));
    }

    static Point TransposePoint(Point point) { return new Point(point.Y, point.X); }
    static Size  TransposeSize(Size  size)   { return new Size (size.Height, size.Width); }
  }
}
