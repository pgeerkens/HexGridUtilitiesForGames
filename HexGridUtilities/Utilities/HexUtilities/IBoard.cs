#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface IBoard<TGridHex> where TGridHex : class, IGridHex {
    bool IsOnBoard(ICoordsUser coords);

    /// <summary>The rectangular extent of the board's hexagonal grid.</summary>
    Size     SizeHexes                 { get; }
    TGridHex this[ICoordsUser coords]  { get; }
    TGridHex this[ICoordsCanon coords] { get; }
  }

}
