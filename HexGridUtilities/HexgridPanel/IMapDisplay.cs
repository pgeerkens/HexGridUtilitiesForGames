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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>Interface contract required of a map board to be displayed by the HexgridPanel.</summary>
  public interface IMapDisplay {
    /// <summary>Gets the extens in pixels of the grid upon whch hexes are to be laid out.</summary>
    /// <remarks>>Width is 3/4 of the point-to-point width of each hex, and Height is the full height.
    /// Hexes should be defined assumed flat-topped and pointy-sided, and the entire board transposed 
    /// if necessary.</remarks>
    Size   GridSize      { get; }
    /// <summary>Rectangular extent in pixels of the defined mapboard.</summary>
    Size   MapSizePixels { get; }
    /// <summary>Gets the display name for this HexgridPanel host.</summary>
    string Name          { get; }

    /// <summary>Gets the CoordsRectangle description of the clipping region.</summary>
    /// <param name="point">Upper-left corner in pixels of the clipping region.</param>
    /// <param name="size">Width and height of the clipping region in pixels.</param>
    CoordsRectangle GetClipCells(PointF point, SizeF size);
    /// <summary>Gets the CoordsRectangle description of the clipping region.</summary>
    /// <param name="visibleClipBounds">Rectangular extent in pixels of the clipping region.</param>
    CoordsRectangle GetClipCells(RectangleF visibleClipBounds);

    /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    void  PaintHighlight(Graphics g);

    /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
    void  PaintMap(Graphics g);
    /// <summary>Paint the intermediate layer of the display, graphics that changes infrequently between refreshes.</summary>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    void  PaintUnits(Graphics g);
  }
}
