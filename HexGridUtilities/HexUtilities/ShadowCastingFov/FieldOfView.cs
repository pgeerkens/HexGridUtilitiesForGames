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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  /// <summary>Structure returned by the Field-of-View factory.</summary>
  public interface IFov {
    /// <summary>True if the hex at location <c>coords> is visible in this field-of-view.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    bool this[HexCoords coords] { get; }
  }

  /// <summary>Implementation of IFov using a backing array of BitArray.</summary>
  internal class ArrayFieldOfView : IFov {
    private readonly object _syncLock = new object();

    public ArrayFieldOfView(IFovBoard<IHex> board) {
      _isOnboard  = h => board.IsOnboard(h);
      _fovBacking = new BitArray[board.MapSizeHexes.Width]; 
      for (var i=0; i< board.MapSizeHexes.Width; i++)
        _fovBacking[i] = new BitArray(board.MapSizeHexes.Height);
    }

    public bool this[HexCoords coords] { 
      get { 
        return _isOnboard(coords) && _fovBacking[coords.User.X][coords.User.Y];
      } 
      internal set { 
        lock(_syncLock) {
          if (_isOnboard(coords)) { _fovBacking[coords.User.X][coords.User.Y] = value; } 
        }
      }
    } BitArray[] _fovBacking;

    Func<HexCoords,bool> _isOnboard;
  }
}
