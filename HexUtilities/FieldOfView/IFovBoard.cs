#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.Contracts;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  using HexPointF    = System.Drawing.PointF;
  using HexPoint     = System.Drawing.Point;
  using HexSizeF     = System.Drawing.SizeF;
  using HexSize      = System.Drawing.Size;
  using HexRectangle = System.Drawing.Rectangle;

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

  /// <summary>Structure returned by the Field-of-View factory.</summary>
  public interface IFov {
    /// <summary>True if the hex at location <c>coords</c>c> is visible in this field-of-view.</summary>
    [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    bool this[HexCoords coords] { get; }
  }

  /// <summary>Interface required to make use of ShadowCasting Field-of-View calculation.</summary>
  [ContractClass(typeof(IFovBoardContract))]
  public interface IFovBoard {
    /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
    HexSize       MapSizeHexes            { get; }

    /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
    [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    Maybe<IHex>   this[HexCoords  coords] { get; }

    /// <summary>Elevation of the ground Above Sea Level</summary>
    [Pure]int  ElevationGroundASL(HexCoords coords);
    /// <summary>Elevation of the hexside Above Sea Level</summary>
    [Pure]int  ElevationHexsideASL(HexCoords coords, Hexside hexside);
    /// <summary>Elevation of the observer Above Sea Level</summary>
    [Pure]int  ElevationObserverASL(HexCoords coords);
    /// <summary>Elevation of the target Above Sea Level</summary>
    [Pure]int  ElevationTargetASL(HexCoords coords);
    /// <summary>Elevation of the terrain Above Sea Level</summary>
    [Pure]int  ElevationTerrainASL(HexCoords coords);

    /// <summary>Returns whether the hex at location <paramref name="coords"/> is passable.</summary>
    /// <param name="coords">The coordinates of the hex being tested.</param>
    [Pure]bool IsOverseeable(HexCoords coords);
  }

  [ContractClassFor(typeof(IFovBoard))]
  internal abstract class IFovBoardContract : IFovBoard {
    private IFovBoardContract() { }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
      Contract.Invariant(MapSizeHexes.Width  > 0);
      Contract.Invariant(MapSizeHexes.Height > 0);

      // Redundant - but added to help static checker
      Contract.Invariant(MapSizeHexes.Width  >= 0);
      Contract.Invariant(MapSizeHexes.Height >= 0);
    }

    public HexSize MapSizeHexes        { get { return default(HexSize); } }

    public abstract Maybe<IHex> this[HexCoords coords] { get; }

    public abstract int  ElevationGroundASL(HexCoords coords);
    public abstract int  ElevationHexsideASL(HexCoords coords, Hexside hexside);
    public abstract int  ElevationObserverASL(HexCoords coords);
    public abstract int  ElevationTargetASL(HexCoords coords);
    public abstract int  ElevationTerrainASL(HexCoords coords);

    public abstract bool IsOverseeable(HexCoords coords);
  }
}
