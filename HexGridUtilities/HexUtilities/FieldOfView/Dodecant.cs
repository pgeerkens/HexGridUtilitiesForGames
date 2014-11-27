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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  using HexsideMap = Func<Hexside,Hexside>;

  /// <summary>Extension methods to support the identification of and translation between Dodecants.</summary>
  internal static class Dodecant {
    #region TranslateDodecant
    internal static Action<HexCoords> TranslateDodecant(this IntMatrix2D @this, Action<HexCoords> action) {
      return (coords) => action(HexCoords.NewCanonCoords(coords.Canon * @this));
    }
    internal static Func<HexCoords,T> TranslateDodecant<T>(this IntMatrix2D @this, Func<HexCoords,T> func) {
      return (coords) => func(HexCoords.NewCanonCoords(coords.Canon * @this));
    }
    internal static Func<HexCoords,Hexside,T> TranslateDodecant<T>(this IntMatrix2D @this, HexsideMap map,Func<HexCoords,Hexside,T> func) {
      return (coords,hexside) => func(HexCoords.NewCanonCoords(coords.Canon * @this), map(hexside));
    }
    #endregion

    #region Matrices
    //         Sextant map
    //                    X-axis
    //         \     |     /
    //           \ 3 | 2 /
    //             \ | / 
    //          4    +    1     
    //             / | \
    //           / 5 | 0 \  
    //         /     |     \
    //             Y-axis
    internal static IList<IntMatrix2D> Matrices {
      get { return _matrices; }
    } static readonly IList<IntMatrix2D> _matrices = (
        new List<IntMatrix2D>() {
          new IntMatrix2D( 1, 0,  0, 1),  new IntMatrix2D(-1, 0,  1, 1),
          new IntMatrix2D( 0,-1,  1, 1),  new IntMatrix2D( 0, 1,  1, 0),
          new IntMatrix2D(-1,-1,  1, 0),  new IntMatrix2D( 1, 1,  0,-1),
          new IntMatrix2D(-1, 0,  0,-1),  new IntMatrix2D( 1, 0, -1,-1),
          new IntMatrix2D( 0, 1, -1,-1),  new IntMatrix2D( 0,-1, -1, 0),
          new IntMatrix2D( 1, 1, -1, 0),  new IntMatrix2D(-1,-1,  0, 1) 
        }.AsReadOnly());
    #endregion

    #region Hexsides
    internal static IList<Func<Hexside,Hexside>> Hexsides { get { return _hexsides; } } 
    private static readonly IList<Func<Hexside,Hexside>> _hexsides = (
        new List<Func<Hexside,Hexside>>() {
          hexside => Mod6( 0 + (int)hexside), //  CW  from Hexside.North
          hexside => Mod6(11 - (int)hexside), // CCW  from Hexside.Northwest

          hexside => Mod6( 1 + (int)hexside), //  CW  from Hexside.Northwest
          hexside => Mod6(10 - (int)hexside), // CCW  from Hexside.Southwest

          hexside => Mod6( 2 + (int)hexside), //  CW  from Hexside.Southwest
          hexside => Mod6( 9 - (int)hexside), // CCW  from Hexside.South

          hexside => Mod6( 3 + (int)hexside), //  CW  from Hexside.South
          hexside => Mod6( 8 - (int)hexside), // CCW  from Hexside.Southeast

          hexside => Mod6( 4 + (int)hexside), //  CW  from Hexside.Southeast
          hexside => Mod6( 7 - (int)hexside), // CCW  from Hexside.Northeast

          hexside => Mod6( 5 + (int)hexside), //  CW  from Hexside.Northeast
          hexside => Mod6( 6 - (int)hexside), // CCW  from Hexside.North
        }.AsReadOnly()
    );
    #endregion

    #region Mod6 - replace an integer modulo operation with a lookup innto t table twice as long 
    private static Func<int,Hexside> Mod6 = (intHexside) => _hexsideMap [intHexside];

    private static readonly IList<Hexside> _hexsideMap = new Hexside[] {
      Hexside.North, Hexside.Northeast, Hexside.Southeast, Hexside.South, Hexside.Southwest, Hexside.Northwest,
      Hexside.North, Hexside.Northeast, Hexside.Southeast, Hexside.South, Hexside.Southwest, Hexside.Northwest 
    };
    #endregion
  }
}
