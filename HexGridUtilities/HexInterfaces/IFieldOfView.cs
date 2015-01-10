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
using System.Drawing;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  /// <summary>Enumeration of line-of-sight modes</summary>
  public enum FovTargetMode {
    /// <summary>Target height and observer height both set to the same constant value 
    /// (ShadowCasting.DefaultHeight) above ground eleevation</summary>
    EqualHeights,
    /// <summary>Use actual observer height and ground level as target height.</summary>
    TargetHeightEqualZero,
    /// <summary>Use actual observer and target height.</summary>
    TargetHeightEqualActual
  }

  /// <summary>Interface required to make use of ShadowCasting Field-of-View calculation.</summary>
  public interface IFovBoard<out THex> where THex : IHex {
    /// <summary>Distance in hexes out to which Field-of-View is to be calculated.</summary>
    int  FovRadius               { get; set; }

    /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
    Size MapSizeHexes            { get; }

    /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
    [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    THex this[HexCoords  coords] { get; }

    /// <summary>TODO</summary>
    int  ElevationGroundASL(HexCoords coords);
    /// <summary>TODO</summary>
    int  ElevationHexsideASL(HexCoords coords, Hexside hexside);
    /// <summary>TODO</summary>
    int  ElevationObserverASL(HexCoords coords);
    /// <summary>TODO</summary>
    int  ElevationTargetASL(HexCoords coords);
    /// <summary>TODO</summary>
    int  ElevationTerrainASL(HexCoords coords);

    /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
    bool IsOnboard(HexCoords coords);

    /// <summary>Returns whether the hex at location <c>coords</c> is passable.</summary>
    /// <param name="coords"></param>
    bool IsPassable(HexCoords coords);
  }

  /// <summary>Structure returned by the Field-of-View factory.</summary>
  public interface IFov {
    /// <summary>True if the hex at location <c>coords</c>c> is visible in this field-of-view.</summary>
   [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    bool this[HexCoords coords] { get; }
  }
}
