#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface IGridHex {
    char             DefLetter      { get; }
    ICoordsUser      Coords         { get; }
    int              Elevation      { get; }
    int              ElevationASL   { get; }
    int              HeightTerrain  { get; }
    IBoard<IGridHex> Board          { get; }
    IEnumerable<NeighbourHex> GetNeighbours();
  }
}
