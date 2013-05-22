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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface IHexGridHost {
    Size    ClientSize      { get; }

    /// <summary>Scaled <code>Size</code> of each hexagon in grid, being the 'full-width' and 'full-height'.</summary>
    SizeF   GridSizeF       { get; }

    /// <summary>Margin of map in pixels.</summary>
    Size    MapMargin       { get; }

    /// <summary>Current scaling factor for map display.</summary>
    float   MapScale        { get; }

    /// <summary></summary>
    Size    MapSizePixels   { get; }

    /// <summary></summary>
    Point   ScrollPosition  { get; }

    /// <summary>IUserCoords for the currently visible extent (location & size), as a Rectangle.</summary>
    UserCoordsRectangle     VisibleRectangle { get; }
  }

  /// <summary>C# implementation of the hex-picking algorithm noted  below.</summary>
  /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
  public class HexGrid {
    public HexGrid(IHexGridHost host) { Host = host; }

    public virtual Point ScrollPosition { get { return Host.ScrollPosition; } }
    public virtual Size  Size           { get { return Size.Ceiling(Host.MapSizePixels.Scale(Host.MapScale)); } }

    /// <summary><c>ICoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    public virtual ICoords GetHexCoords(Point point, Size autoScroll) {
      if( Host == null ) return HexCoords.EmptyCanon;

      // Adjust for origin not as assumed by GetCoordinate().
      var grid    = new Size((int)(Host.GridSizeF.Width*2F/3F), (int)Host.GridSizeF.Height);
      var margin  = new Size((int)(Host.MapMargin.Width  * Host.MapScale), 
                             (int)(Host.MapMargin.Height * Host.MapScale));
      point      -= autoScroll + margin + grid;

      return HexCoords.NewCanonCoords( GetCoordinate(matrixX, point), 
                                       GetCoordinate(matrixY, point) );
    }

    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>ICoords</c> for the hex to be centered in viewport.</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public virtual Point ScrollPositionToCenterOnHex(ICoords coordsNewCenterHex) {
      return HexCenterPoint(HexCoords.NewUserCoords(
              coordsNewCenterHex.User - ( new IntVector2D(Host.VisibleRectangle.Size.User) / 2 )
      ));
    }

    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>ICoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    public virtual Point HexCenterPoint(ICoords coordsNewULHex) {
      if (coordsNewULHex == null) return new Point();
      var offset = new Size((int)(Host.GridSizeF.Width*2F/3F), (int)Host.GridSizeF.Height);
      var margin = Size.Round( Host.MapMargin.Scale(Host.MapScale) );
      return HexOrigin(Host.GridSizeF,coordsNewULHex) + margin + offset;
    }

    /// <summary>Scrolling control hosting this HexGrid.</summary>
    protected IHexGridHost Host { get; private set; }

    /// Mathemagically (left as exercise for the reader) our 'picking' matrices are these, assuming: 
    ///  - origin at upper-left corner of hex (0,0);
    ///  - 'straight' hex-axis vertically down; and
    ///  - 'oblique'  hex-axis up-and-to-right (at 120 degrees from 'straight').
    Matrix matrixX { 
      get { return new Matrix(
          (3.0F/2.0F)/Host.GridSizeF.Width,  (3.0F/2.0F)/Host.GridSizeF.Width,
                 1.0F/Host.GridSizeF.Height,       -1.0F/Host.GridSizeF.Height,  -0.5F,-0.5F); } 
    }
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

    Point HexOrigin(SizeF gridSize, ICoords coords) {
      return new Point(
        (int)(Host.GridSizeF.Width  * coords.User.X),
        (int)(Host.GridSizeF.Height * coords.User.Y   + Host.GridSizeF.Height/2 * (coords.User.X+1)%2)
      );
    }
  }

  public class TransposedHexGrid : HexGrid {
    public TransposedHexGrid(IHexGridHost host) :base(host) {}

    ///<inheritdoc/>
    public override Point ScrollPosition { get { return TransposePoint(base.ScrollPosition); } }

    ///<inheritdoc/>
    public override Size  Size           { get { return TransposeSize(base.Size); } }

    ///<inheritdoc/>
    public override ICoords GetHexCoords(Point point, Size autoScroll) {
      return base.GetHexCoords(TransposePoint(point), TransposeSize(autoScroll));
    }
    ///<inheritdoc/>
    public override Point HexCenterPoint(ICoords coordsNewULHex) {
      return TransposePoint(base.HexCenterPoint(coordsNewULHex));
    }

    Point TransposePoint(Point point) { return new Point(point.Y,point.X); }
    Size  TransposeSize(Size  size)   { return new Size (size.Height, size.Width); }
  }
}
