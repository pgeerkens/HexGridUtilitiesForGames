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
using System.Linq;
using System.Threading.Tasks;

using PG_Napoleonics;
using PG_Napoleonics.HexUtilities;
using PG_Napoleonics.HexUtilities.Common;
using PG_Napoleonics.HexUtilities.ShadowCastingFov;

namespace PG_Napoleonics.HexUtilities.ShadowCastingFov {
  /// <summary>Structure returned by the Field-of-View factory.</summary>
  public interface IFov {
    /// <summary>True if the hex at location <c>coords> is visible in this field-of-view.</summary>
    bool this[HexCoords coords] { get; }
  }

  /// <summary>Implementation of IFov using a 2-D backing array.</summary>
  internal class ArrayFieldOfView : IFov {
    public ArrayFieldOfView(IFovBoard board) {
      Board      = board;
      FovBacking = new bool[board.SizeHexes.Width, board.SizeHexes.Height];
    }

    public bool this[HexCoords coords] { 
      get { 
        return Board.IsOnBoard(coords) && FovBacking[coords.User.X, coords.User.Y];
      } 
      internal set { 
        if (Board.IsOnBoard(coords)) { FovBacking[coords.User.X,coords.User.Y] = value; } 
      }
    } bool[,] FovBacking;

    IFovBoard           Board;
  }
}
