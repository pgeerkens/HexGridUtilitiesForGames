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

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities.Common;

#pragma warning disable 1587
/// <summary>Display-technology-independent utilities for implementation of hex-grids..</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities {
  using HexPoint  = System.Drawing.Point;
  using HexPointF = System.Drawing.PointF;
  using HexSize   = System.Drawing.Size;
  using HexSizeF  = System.Drawing.SizeF;
  using HexMatrix = System.Drawing.Drawing2D.Matrix;

  /// <summary>Interface defining the functionality required of a form or control hosting an instannce of <see cref="Hexgrid"/>.</summary>
  [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
  public interface IHexgridHost {  }

  /// <summary>TODO</summary>
  public interface IHexgrid {
    /// <summary>TODO</summary>
    HexPoint GetScrollPosition(HexPoint scrollPosition);
    /// <summary>TODO</summary>
    HexSize  GetSize(HexSize mapSizePixels, float mapScale);

    /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    HexCoords GetHexCoords(HexPoint point, HexSize autoScroll);

    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <param name="visibleRectangle"></param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    HexPoint   ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex, CoordsRectangle visibleRectangle);

    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    HexPoint   HexCenterPoint(HexCoords coordsNewULHex);

    /// <summary>TODO</summary>
    HexSizeF GridSizeF  { get; }
    /// <summary>Offset of grid origin, from control's client-area origin.</summary>
    HexSize  Margin     { get; set; }
}

  /// <summary>C# implementation of the hex-picking algorithm noted below.</summary>
  /// <remarks>Mathemagically (left as exercise for the reader) our 'picking' matrices are these, assuming: 
  ///  - origin at upper-left corner of hex (0,0);
  ///  - 'straight' hex-axis vertically down; and
  ///  - 'oblique'  hex-axis up-and-to-right (at 120 degrees from 'straight').</remarks>
  /// <a href="file://Documentation/HexGridAlgorithm.mht">Hex-grid Algorithms</a>
  public class Hexgrid : IHexgrid {
    /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
    public Hexgrid(HexSizeF gridSizeF) : this(gridSizeF, HexSize.Empty) {}
    /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
    public Hexgrid(HexSizeF gridSizeF, HexSize margin) { 
      GridSizeF = gridSizeF;
      Margin    = margin;
    }

    #region Properties
    /// <inheritdoc/>
    public HexSizeF  GridSizeF { get; private set; }

    /// <inheritdoc/>
    public HexSize   Margin    { get; set; }

    /// <summary>Matrix2D for 'picking' the <B>X</B> hex coordinate</summary>
    HexMatrix matrixX { 
      get { return new HexMatrix(
          (3.0F/2.0F)/GridSizeF.Width,  (3.0F/2.0F)/GridSizeF.Width,
                 1.0F/GridSizeF.Height,       -1.0F/GridSizeF.Height,  -0.5F,-0.5F); } 
    }
    /// <summary>Matrix2D for 'picking' the <B>Y</B> hex coordinate</summary>
    HexMatrix matrixY { 
      get { return new HexMatrix(
                                 0.0F,  (3.0F/2.0F)/GridSizeF.Width,
                2.0F/GridSizeF.Height,         1.0F/GridSizeF.Height,  -0.5F,-0.5F); } 
    }
    #endregion

    /// <summary>Scroll position on the (possibly transposed) HexGrid.</summary>
    public virtual HexPoint GetScrollPosition(HexPoint scrollPosition) { return scrollPosition; }

    /// <inheritdoc/>
    public virtual HexSize  GetSize(HexSize mapSizePixels, float mapScale) {
      return HexSize.Ceiling(mapSizePixels.Scale(mapScale)); 
    }

    /// <inheritdoc/>
    public virtual HexCoords GetHexCoords(HexPoint point, HexSize autoScroll) {
      // Adjust for origin not as assumed by GetCoordinate().
      var grid    = new HexSize((int)(GridSizeF.Width*2F/3F), (int)GridSizeF.Height);
      point      -= autoScroll + grid - Margin;

      return HexCoords.NewCanonCoords( GetCoordinate(matrixX, point), 
                                       GetCoordinate(matrixY, point) );
    }
    /// <summary>TODO</summary>
    public virtual HexCoords GetHexCoords(HexPointF point, HexSizeF autoScroll) {
      // Adjust for origin not as assumed by GetCoordinate().
      var grid  = new HexSizeF(GridSizeF.Width*2F/3F, GridSizeF.Height);
      point    -= autoScroll + grid - new HexSizeF(Margin.Width,Margin.Height);

      return HexCoords.NewCanonCoords( GetCoordinate(matrixX, point), 
                                       GetCoordinate(matrixY, point) );
    }

    /// <inheritdoc/>
    public virtual HexPoint ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex, CoordsRectangle visibleRectangle) {
      return HexCenterPoint(HexCoords.NewUserCoords(
              coordsNewCenterHex.User - ( new IntVector2D(visibleRectangle.Size.User) / 2 )
      ));
    }

    /// <summary>Calculates a (canonical X or Y) grid-coordinate for a point, from the supplied 'picking' matrix.</summary>
    /// <param name="matrix">The 'picking-matrix' matrix</param>
    /// <param name="point">The screen point identifying the hex to be 'picked'.</param>
    /// <returns>A (canonical X or Y) grid coordinate of the 'picked' hex.</returns>
	  static int GetCoordinate (HexMatrix matrix, HexPoint point){
      var points = new HexPoint[] {point};
      matrix.TransformPoints(points);
		  return (int) Math.Floor( (points[0].X + points[0].Y + 2F) / 3F );
	  }
    /// <summary>Calculates a (canonical X or Y) grid-coordinate for a point, from the supplied 'picking' matrix.</summary>
    /// <param name="matrix">The 'picking-matrix' matrix</param>
    /// <param name="point">The screen point identifying the hex to be 'picked'.</param>
    /// <returns>A (canonical X or Y) grid coordinate of the 'picked' hex.</returns>
	  static int GetCoordinate (HexMatrix matrix, HexPointF point){
      var points = new HexPointF[] {point};
      matrix.TransformPoints(points);
		  return (int) Math.Floor( (points[0].X + points[0].Y + 2F) / 3F );
	  }

    /// <inheritdoc/>
    public virtual HexPoint   HexCenterPoint(HexCoords coordsNewULHex) {
      var offset = new HexSize((int)(GridSizeF.Width*2F/3F), (int)GridSizeF.Height);
      return HexOrigin(coordsNewULHex) + offset;
    }

    /// <summary>Returns the pixel coordinates of the center of the specified hex.</summary>
    /// <param name="coords"><see cref="HexCoords"/> specification for which pixel center is desired.</param>
    /// <returns>Pixel coordinates of the center of the specified hex.</returns>
    HexPoint HexOrigin(HexCoords coords) {
      return new HexPoint(
        (int)(GridSizeF.Width  * coords.User.X),
        (int)(GridSizeF.Height * coords.User.Y   + GridSizeF.Height/2 * (coords.User.X+1)%2)
      );
    }
  }

  /// <summary>A transposed hexgrid with pointy-top/flat-sides hexes; the rectangular i-axis running 
  /// verticallly down; and the rectnaagular j-axis running out to the right from teh upper-left corner 
  /// (which remains coordinate (0,0) for both User (rectangular) and Canon (obtuse) coordinate frames.</summary>
  public class TransposedHexgrid : Hexgrid {
    /// <summary>Returns a <c>TransposedHexgrid</c> instance from the supplied <see cref="IHexgridHost"/>.</summary>
    public TransposedHexgrid(HexSizeF gridSizeF) : this(gridSizeF, HexSize.Empty) {}
    /// <summary>Returns a <c>TransposedHexgrid</c> instance from the supplied <see cref="IHexgridHost"/>.</summary>
    public TransposedHexgrid(HexSizeF gridSizeF, HexSize margin) : base(gridSizeF, margin) {}

    /// <inheritdoc/>
    public override HexPoint GetScrollPosition(HexPoint scrollPosition) { 
      return TransposePoint(scrollPosition); 
    }

    /// <inheritdoc/>
    public override HexSize  GetSize(HexSize mapSizePixels, float mapScale) { 
      return TransposeSize(base.GetSize(mapSizePixels, mapScale)); 
    }

    /// <inheritdoc/>
    public override HexCoords GetHexCoords(HexPoint point, HexSize autoScroll) {
      return base.GetHexCoords(TransposePoint(point), TransposeSize(autoScroll));
    }
    /// <inheritdoc/>
    public override HexPoint HexCenterPoint(HexCoords coordsNewULHex) {
      return TransposePoint(base.HexCenterPoint(coordsNewULHex));
    }

    static HexPoint TransposePoint(HexPoint point) { return new HexPoint(point.Y, point.X); }
    static HexSize  TransposeSize(HexSize  size)   { return new HexSize (size.Height, size.Width); }
  }
}
