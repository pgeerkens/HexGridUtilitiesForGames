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

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  public static partial class ShadowCasting {
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
    private static List<IntMatrix2D> _dodecantMatrices = BuildDodecantMatrices();
    private static Action<HexCoords> TranslateDodecant(IntMatrix2D matrix, Action<HexCoords> action) {
      return (v) => action(HexCoords.NewCanonCoords(v.Canon*matrix));
    }
    private static Func<HexCoords,T> TranslateDodecant<T>(IntMatrix2D matrix, Func<HexCoords,T> func) {
      return (v) => func(HexCoords.NewCanonCoords(v.Canon*matrix));
    }

    /// <summary>Build dodecant matrices from sextant matrices and reflection about theta=30.</summary>
    static List<IntMatrix2D> BuildDodecantMatrices() {
      //var matrixRotate  = new IntMatrix2D( 0,-1, 1,1);
      //var matrixReflect = new IntMatrix2D(-1, 0, 1,1);
      //var matrices = new List<IntMatrix2D>(12);
      //matrices.Add(IntMatrix2D.Identity);
      //matrices.Add(matrixReflect);
      //for (int i=2; i<12; i+=2) {
      //  matrices.Add(matrixRotate  * matrices[i-2]);
      //  matrices.Add(matrixReflect * matrices[i]);
      //}

      var matrices = new List<IntMatrix2D>(12) {
        new IntMatrix2D( 1, 0,  0, 1),  new IntMatrix2D(-1, 0,  1, 1),
        new IntMatrix2D( 0,-1,  1, 1),  new IntMatrix2D( 0, 1,  1, 0),
        new IntMatrix2D(-1,-1,  1, 0),  new IntMatrix2D( 1, 1,  0,-1),
        new IntMatrix2D(-1, 0,  0,-1),  new IntMatrix2D( 1, 0, -1,-1),
        new IntMatrix2D( 0, 1, -1,-1),  new IntMatrix2D( 0,-1, -1, 0),
        new IntMatrix2D( 1, 1, -1, 0),  new IntMatrix2D(-1,-1,  0, 1)
      };

      return matrices;
    }
  }
}
