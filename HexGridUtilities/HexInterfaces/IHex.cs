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

namespace PGNapoleonics.HexUtilities {
  /// <summary>External interface exposed by individual hexes.</summary>
  public interface IHex {
    /// <summary>The <c>IBoard {IHex}</c> on which this hex is located.</summary>
    IHexBoard<IHex> Board          { get; }

    /// <summary>The <c>HexCoords</c> coordinates for this hex on <c>Board</c>.</summary>
    HexCoords       Coords         { get; }

    /// <summary>Elevation of this hex in "steps" above the minimum elevation of the board.</summary>
    int             Elevation      { get; }

    /// <summary>Elevation "Above Sea Level" in <i>game units</i> of the ground in this hex.</summary>
    /// <remarks>Calculated as BaseElevationASL + Elevation * ElevationStep.</remarks>
    int             ElevationASL   { get; }

    /// <summary>Height ASL in <i>game units</i> of observer's eyes for FOV calculations.</summary>
    int             HeightObserver { get; }

    /// <summary>Height ASL in <i>game units</i> of target above ground level to be spotted.</summary>
    int             HeightTarget   { get; }

    /// <summary>Height ASL in <i>game units</i> of any blocking terrian in this hex.</summary>
    int             HeightTerrain  { get; }

    /// <summary>Returns the neighbouring hex across <c>Hexside</c> <c>hexside</c>.</summary>
    IHex Neighbour(Hexside hexside);

    /// <summary>Cost to extend the path with the hex located across the <c>Hexside</c> at <c>direction</c>.</summary>
    int  StepCost(Hexside direction);

    /// <summary>Cost to exit this hex through the <c>Hexside</c> <c>hexsideExit</c>.</summary>
    int  GetDirectedCostToExit(Hexside hexsideExit);

    /// <summary>Height ASL in <i>game units</i> of any blocking terrain in this hex and the specified Hexside.</summary>
    int  HeightHexside(Hexside hexside);
  }
}
