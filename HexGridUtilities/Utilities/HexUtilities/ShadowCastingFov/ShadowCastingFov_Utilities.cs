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
    static IntVector2D LogAndEnqueue(Action<FovCone> enqueue, int range, IntVector2D top, 
            IntVector2D bottom, RiseRun riseRun, int code
    ) {
      if( top.GT(bottom)) {
        var cone = new FovCone(range+1, top, bottom, riseRun);
        #if TraceFOV
          DebugTracing.Trace(TraceFlag.FieldOfView,false, "  EQ: ({0}) code: {1}",cone,code);
        #endif
        enqueue(cone);
        return bottom;
      } else {
        return top;
      }
    }

    private static int GetRange(ICoordsCanon coords) { return Coords.EmptyCanon.Range(coords); }

    static int XFromVector(int y, IntVector2D v, bool isTop) {
      if (isTop) {
        return (-2 * v.Y + v.X * (3 * y + 1) + (3 * v.Y) - 1) / (3 * v.Y);
      } else { 
        return (+2 * v.Y + v.X * (3 * y - 1)) / (3 * v.Y);
      }
    }
    /// <summary>IntVector2D for top corner of cell Canon(x,y).</summary>
    /// <remarks>
    /// In first dodecant; The top corner for hex (x,y) is determined 
    /// (from close visual inspection) as:
    ///       (x,y) + 1/3 * (2,1)
    /// which reduces to:
    ///       (x + 2/3, y + 1/3) == 1/3 * (3x + 2, 3y + 1)
    /// </remarks>
    static IntMatrix2D matrixHexTop = new IntMatrix2D(3,0,  0,3, 2,1);
    static IntVector2D VectorHexTop(ICoordsCanon hex) { return hex.Vector * matrixHexTop; }
    /// <summary>IntVector2D for bottom corner of cell Canon(x,y).</summary>
    /// <remarks>
    /// In first dodecant; The bottom corner for hex (x,y) is determined 
    /// (from close visual inspection) as:
    ///       (x,y) + 1/3 * (-2,-1)
    /// which reduces to:
    ///       (x - 2/3, y - 1/3) == 1/3 * (3x - 2, 3y - 1)
    /// </remarks>
    static IntMatrix2D matrixHexBottom = new IntMatrix2D(3,0,  0,3, -2,-1);
    static IntVector2D VectorHexBottom(ICoordsCanon hex)  { return hex.Vector * matrixHexBottom;  }

    // These are here (instead of IntVector2D.cs) because they are "upside-down" for regular use.
    static IntVector2D VectorMax(IntVector2D lhs, IntVector2D rhs) {
      return lhs.GT(rhs) ? lhs : rhs; 
    }
    static IntVector2D VectorMin(IntVector2D lhs, IntVector2D rhs) {
      return lhs.LE(rhs) ? lhs : rhs;
    }
    private static bool GT(this IntVector2D lhs, IntVector2D rhs) {
      return lhs.X*rhs.Y > lhs.Y*rhs.X; 
    }
    private static bool LE(this IntVector2D lhs, IntVector2D rhs) { return ! lhs.GT(rhs); }
  }
}
