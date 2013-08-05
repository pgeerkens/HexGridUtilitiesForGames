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
  public interface IHexgridHost {
    /// <summary>TODO</summary>
    Size            ClientSize       { get; }

    /// <summary>Scaled <code>Size</code> of each hexagon in grid, being the 'full-width' and 'full-height'.</summary>
    SizeF           GridSizeF        { get; }

    /// <summary>Margin of map in pixels.</summary>
    Size            MapMargin        { get; }

    /// <summary>Current scaling factor for map display.</summary>
    float           MapScale         { get; }

    /// <summary>Rectangular extent in pixels of the defined mapboard.</summary>
    Size            MapSizePixels    { get; }

    /// <summary>Returns the current scroll position, as per the <b>WinForms</b> behaviour of <i>AutoScroll</i>.</summary>
    Point           ScrollPosition   { get; }

    /// <summary>IUserCoords for the currently visible extent (location &amp; size), as a Rectangle.</summary>
    CoordsRectangle VisibleRectangle { get; }
  }

  /// <summary>TODO</summary>
  public interface IHexgrid {
    /// <summary></summary>
    Point ScrollPosition { get; }
    /// <summary></summary>
    Size  Size           { get; }

    /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    HexCoords GetHexCoords(Point point, Size autoScroll);

    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    Point   ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex);

    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    Point   HexCenterPoint(HexCoords coordsNewULHex);
  }

  /// <summary>C# implementation of the hex-picking algorithm noted below.</summary>
  /// <remarks>Mathemagically (left as exercise for the reader) our 'picking' matrices are these, assuming: 
  ///  - origin at upper-left corner of hex (0,0);
  ///  - 'straight' hex-axis vertically down; and
  ///  - 'oblique'  hex-axis up-and-to-right (at 120 degrees from 'straight').</remarks>
  /// <a href="file://Documentation/HexGridAlgorithm.mht">Hex-grid Algorithms</a>
  public class Hexgrid : IHexgrid {
    /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
    public Hexgrid(IHexgridHost host) { Host = host; }

    /// <inheritdoc/>
    public virtual Point ScrollPosition { get { return Host.ScrollPosition; } }

    /// <inheritdoc/>
    public virtual Size  Size           { get { return Size.Ceiling(Host.MapSizePixels.Scale(Host.MapScale)); } }

    /// <inheritdoc/>
    public virtual HexCoords GetHexCoords(Point point, Size autoScroll) {
      if( Host == null ) return HexCoords.EmptyCanon;

      // Adjust for origin not as assumed by GetCoordinate().
      var grid    = new Size((int)(Host.GridSizeF.Width*2F/3F), (int)Host.GridSizeF.Height);
      var margin  = new Size((int)(Host.MapMargin.Width  * Host.MapScale), 
                             (int)(Host.MapMargin.Height * Host.MapScale));
      point      -= autoScroll + margin + grid;

      return HexCoords.NewCanonCoords( GetCoordinate(matrixX, point), 
                                       GetCoordinate(matrixY, point) );
    }

    /// <inheritdoc/>
    public virtual Point   ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex) {
      return HexCenterPoint(HexCoords.NewUserCoords(
              coordsNewCenterHex.User - ( new IntVector2D(Host.VisibleRectangle.Size.User) / 2 )
      ));
    }

    /// <inheritdoc/>
    public virtual Point   HexCenterPoint(HexCoords coordsNewULHex) {
      if (coordsNewULHex == null) return new Point();
      var offset = new Size((int)(Host.GridSizeF.Width*2F/3F), (int)Host.GridSizeF.Height);
      var margin = Size.Round( Host.MapMargin.Scale(Host.MapScale) );
      return HexOrigin(coordsNewULHex) + margin + offset;
    }

    /// <summary>Scrolling control hosting this HexGrid.</summary>
    protected IHexgridHost Host { get; private set; }

    /// <summary>Matrix2D for 'picking' the <B>X</B> hex coordinate</summary>
    Matrix matrixX { 
      get { return new Matrix(
          (3.0F/2.0F)/Host.GridSizeF.Width,  (3.0F/2.0F)/Host.GridSizeF.Width,
                 1.0F/Host.GridSizeF.Height,       -1.0F/Host.GridSizeF.Height,  -0.5F,-0.5F); } 
    }
    /// <summary>Matrix2D for 'picking' the <B>Y</B> hex coordinate</summary>
    Matrix matrixY { 
      get { return new Matrix(
                0.0F,                        (3.0F/2.0F)/Host.GridSizeF.Width,
                2.0F/Host.GridSizeF.Height,         1.0F/Host.GridSizeF.Height,  -0.5F,-0.5F); } 
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

    /// <summary>Returns the pixel coordinates of the center of the specified hex.</summary>
    /// <param name="coords"><see cref="HexCoords"/> specification for which pixel center is desired.</param>
    /// <returns>Pixel coordinates of the center of the specified hex.</returns>
    Point HexOrigin(HexCoords coords) {
      return new Point(
        (int)(Host.GridSizeF.Width  * coords.User.X),
        (int)(Host.GridSizeF.Height * coords.User.Y   + Host.GridSizeF.Height/2 * (coords.User.X+1)%2)
      );
    }
  }

  /// <summary>A transposed hexgrid with pointy-top/flat-sides hexes; the rectangular x-axis running 
  /// verticallly down; and the rectnaagular y-axis running out to the right from teh upper-left corner 
  /// (which remains coordinate (0,0) for both User (rectangular) and Canon (obtuse) coordinate frames.</summary>
  public class TransposedHexgrid : Hexgrid {
    /// <summary>Returns a <c>TransposedHexgrid</c> instance from the supplied <see cref="IHexgridHost"/>.</summary>
    public TransposedHexgrid(IHexgridHost host) : base(host) {}

    ///<inheritdoc/>
    public override Point ScrollPosition { get { return TransposePoint(base.ScrollPosition); } }

    ///<inheritdoc/>
    public override Size  Size           { get { return TransposeSize(base.Size); } }

    ///<inheritdoc/>
    public override HexCoords GetHexCoords(Point point, Size autoScroll) {
      return base.GetHexCoords(TransposePoint(point), TransposeSize(autoScroll));
    }
    ///<inheritdoc/>
    public override Point HexCenterPoint(HexCoords coordsNewULHex) {
      return TransposePoint(base.HexCenterPoint(coordsNewULHex));
    }

    static Point TransposePoint(Point point) { return new Point(point.Y,point.X); }
    static Size  TransposeSize(Size  size)   { return new Size (size.Height, size.Width); }
  }
}
