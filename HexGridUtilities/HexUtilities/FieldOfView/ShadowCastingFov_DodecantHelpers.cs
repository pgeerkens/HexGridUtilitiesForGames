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

  public static partial class ShadowCasting {
    private static Action<HexCoords> TranslateDodecant(IntMatrix2D matrix, Action<HexCoords> action) {
      return (coords) => action(HexCoords.NewCanonCoords(coords.Canon * matrix));
    }
    private static Func<HexCoords,T> TranslateDodecant<T>(IntMatrix2D matrix, Func<HexCoords,T> func) {
      return (coords) => func(HexCoords.NewCanonCoords(coords.Canon * matrix));
    }

    private static Func<HexCoords,Hexside,T> TranslateDodecant<T>(IntMatrix2D matrix, HexsideMap map,Func<HexCoords,Hexside,T> func) {
      return (coords,hexside) => func(HexCoords.NewCanonCoords(coords.Canon * matrix), map(hexside));
    }

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
    private static readonly ReadOnlyCollection<IntMatrix2D> _dodecantMatrices = (
      new ReadOnlyCollection<IntMatrix2D>(
        new IntMatrix2D[] {
          new IntMatrix2D( 1, 0,  0, 1),  new IntMatrix2D(-1, 0,  1, 1),
          new IntMatrix2D( 0,-1,  1, 1),  new IntMatrix2D( 0, 1,  1, 0),
          new IntMatrix2D(-1,-1,  1, 0),  new IntMatrix2D( 1, 1,  0,-1),
          new IntMatrix2D(-1, 0,  0,-1),  new IntMatrix2D( 1, 0, -1,-1),
          new IntMatrix2D( 0, 1, -1,-1),  new IntMatrix2D( 0,-1, -1, 0),
          new IntMatrix2D( 1, 1, -1, 0),  new IntMatrix2D(-1,-1,  0, 1) 
        }
    ) );

    private static readonly Func<int,Hexside> Mod6 = 
      intHexside => (
          new Hexside[] {
              Hexside.North, Hexside.Northeast, Hexside.Southeast, Hexside.South, Hexside.Southwest, Hexside.Northwest,
              Hexside.North, Hexside.Northeast, Hexside.Southeast, Hexside.South, Hexside.Southwest, Hexside.Northwest 
          }
        ) [intHexside];

    private static readonly ReadOnlyCollection<Func<Hexside,Hexside>> _dodecantHexsides = (
      new ReadOnlyCollection<Func<Hexside,Hexside>>(
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
        }
    ) );
  }
}
