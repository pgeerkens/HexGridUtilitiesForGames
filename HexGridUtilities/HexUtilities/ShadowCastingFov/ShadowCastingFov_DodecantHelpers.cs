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
#undef TraceFoV
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using PG_Napoleonics;
using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities.ShadowCastingFov {
  internal static partial class ShadowCasting {
    /// <summary>Build dodecant matrices from sextant matrices and reflection about theta=30.</summary>
    static ShadowCasting() {
      var matrixRotate  = new IntMatrix2D( 0,-1, 1,1);
      var matrixReflect = new IntMatrix2D(-1, 0, 1,1);
      matrices = new List<IntMatrix2D>(12);
      matrices.Add(IntMatrix2D.Identity);
      matrices.Add(matrixReflect);
      for (int i=2; i<12; i+=2) {
        matrices.Add(matrixRotate  * matrices[i-2]);
        matrices.Add(matrixReflect * matrices[i]);
      }
    }
    #if TraceFoV
    static void TestMatrices() {
        var vector_0_1 = new IntVector2D(0,1);
        var vector_1_2 = new IntVector2D(1,2);
        for(var dodecant=0; dodecant < matrices.Count; dodecant++) {
            TraceFlag.FieldOfView.Trace(true,
              "Dodecant #{0,2} canons:  (0,1) to {1}; (1,2) to {2}", dodecant, 
                  (vector_0_1 * matrices[dodecant]).ToString(), 
                  (vector_1_2 * matrices[dodecant]).ToString());
        }
      }
    #endif

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
    private static List<IntMatrix2D> matrices;
    private static Action<HexCoords> TranslateDodecant(IntMatrix2D matrix, Action<HexCoords> action) {
      return (v) => action(HexCoords.NewCanonCoords(v.Canon*matrix));
    }
    private static Func<HexCoords,T> TranslateDodecant<T>(IntMatrix2D matrix, Func<HexCoords,T> func) {
      return (v) => func(HexCoords.NewCanonCoords(v.Canon*matrix));
    }
  }
}
