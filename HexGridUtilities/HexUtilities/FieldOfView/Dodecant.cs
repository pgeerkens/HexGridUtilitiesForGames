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

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  using HexsideMap = Func<Hexside,Hexside>;

  /// <summary>Extension methods to support the identification of and translation between Dodecants.</summary>
  internal class Dodecant {
    #region Mod6 - replace an integer modulo operation with a lookup into a table twice as long 
    private static readonly IList<Hexside> _hexsideMap = new Hexside[] {
      Hexside.North, Hexside.Northeast, Hexside.Southeast, 
      Hexside.South, Hexside.Southwest, Hexside.Northwest,

      Hexside.North, Hexside.Northeast, Hexside.Southeast, 
      Hexside.South, Hexside.Southwest, Hexside.Northwest 
    };
    #endregion

    //           Sextant map
    //                    X-axis
    //         \     |     /
    //           \ 3 | 2 /
    //             \ | / 
    //          4    +    1     
    //             / | \
    //           / 5 | 0 \  
    //         /     |     \
    //             Y-axis
    public static IFastList<Dodecant> Dodecants = new FastList<Dodecant>( new Dodecant[] {
      new Dodecant( 0, new IntMatrix2D( 1, 0,  0, 1)), //  CW  from Hexside.North
      new Dodecant( 1, new IntMatrix2D( 0,-1,  1, 1)), //  CW  from Hexside.Northwest
      new Dodecant( 2, new IntMatrix2D(-1,-1,  1, 0)), //  CW  from Hexside.Southwest
      new Dodecant( 3, new IntMatrix2D(-1, 0,  0,-1)), //  CW  from Hexside.South
      new Dodecant( 4, new IntMatrix2D( 0, 1, -1,-1)), //  CW  from Hexside.Southeast
      new Dodecant( 5, new IntMatrix2D( 1, 1, -1, 0)), //  CW  from Hexside.Northeast

      new Dodecant( 6, new IntMatrix2D(-1,-1,  0, 1)), // CCW  from Hexside.North
      new Dodecant( 7, new IntMatrix2D( 0,-1, -1, 0)), // CCW  from Hexside.Northeast
      new Dodecant( 8, new IntMatrix2D( 1, 0, -1,-1)), // CCW  from Hexside.Southeast
      new Dodecant( 9, new IntMatrix2D( 1, 1,  0,-1)), // CCW  from Hexside.South
      new Dodecant(10, new IntMatrix2D( 0, 1,  1, 0)), // CCW  from Hexside.Southwest
      new Dodecant(11, new IntMatrix2D(-1, 0,  1, 1))  // CCW  from Hexside.Northwest
    } );

    private Dodecant(int hexsideBase, IntMatrix2D matrix) {
       var sign  = Math.Sign(matrix.Determinant);
      HexsideMap = hexside => _hexsideMap[hexsideBase + sign * (int)hexside];
      Matrix     = matrix;
    }

    public Dodecant(Dodecant dodecant, IntMatrix2D matrixOrigin) {
      HexsideMap = dodecant.HexsideMap;
      Matrix     = dodecant.Matrix * matrixOrigin;
    }

    public  Func<Hexside, Hexside> HexsideMap { get; private set; }
    public  IntMatrix2D            Matrix     { get; private set; }

    #region TranslateDodecant
    public Action<HexCoords> TranslateDodecant(Action<HexCoords> action) {
      return (coords) => action(HexCoords.NewCanonCoords(coords.Canon * this.Matrix));
    }
    public Func<HexCoords,T> TranslateDodecant<T>(Func<HexCoords,T> func) {
      return (coords) => func(HexCoords.NewCanonCoords(coords.Canon * this.Matrix));
    }
    public Func<HexCoords,Hexside,T> TranslateDodecant<T>(Func<HexCoords,Hexside,T> func) {
      return (coords,hexside) => func(HexCoords.NewCanonCoords(coords.Canon * this.Matrix), HexsideMap(hexside));
    }
    #endregion
  }
}
