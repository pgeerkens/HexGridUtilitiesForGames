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
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  using HexPoint  = System.Drawing.Point;
  using HexPointF = System.Drawing.PointF;
  using HexPoints = IList<System.Drawing.Point>;
  using HexSize   = System.Drawing.Size;
  using HexSizeF  = System.Drawing.SizeF;

  /// <summary>TODO</summary>
  public interface IHexgrid {
    /// <summary>TODO</summary>
    HexPoint  GetScrollPosition(HexPoint scrollPosition);
    /// <summary>TODO</summary>
    HexSize   GetSize(HexSize mapSizePixels, float mapScale);

    /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    HexCoords GetHexCoords(HexPoint point, HexSize autoScroll);

    /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
    /// <param name="point">Screen point specifying hex to be identified.</param>
    /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
    HexCoords GetHexCoords(HexPointF point, HexSizeF autoScroll);

    /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
    /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
    /// <param name="visibleRectangle"></param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    HexPoint  ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex, CoordsRectangle visibleRectangle);

    /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
    /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
    /// <returns>Pixel coordinates in Client reference frame.</returns>
    HexPoint  HexCenterPoint(HexCoords coordsNewULHex);

    /// <summary>TODO</summary>
    HexSize   GridSize      { get; }
    /// <summary>TODO</summary>
    HexSizeF  GridSizeF     { get; }
    /// <summary>TODO</summary>
    HexPoints HexCorners    { get; }
    /// <summary>TODO</summary>
    bool      IsTransposed  { get; }
    /// <summary>Offset of grid origin, from control's client-area origin.</summary>
    HexSize   Margin        { get; set; }
    /// <summary>TODO</summary>
    float     Scale         { get; }
}
}
