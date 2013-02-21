#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
#undef TraceFoV
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using PG_Napoleonics.Utilities;

namespace PG_Napoleonics.Utilities.HexUtilities.ShadowCastingFov {
  public static partial class ShadowCasting {
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
            DebugTracing.Trace(TraceFlag.FieldOfView, true,
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
    private static Action<ICoordsCanon> TranslateDodecant(IntMatrix2D matrix, Action<ICoordsCanon> action) {
      return (v) => action(Coords.NewCanonCoords(v.Vector*matrix));
    }
    private static Func<ICoordsCanon,T> TranslateDodecant<T>(IntMatrix2D matrix, Func<ICoordsCanon,T> func) {
      return (v) => func(Coords.NewCanonCoords(v.Vector*matrix));
    }
  }
}
