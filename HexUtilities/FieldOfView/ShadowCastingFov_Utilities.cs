#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    public static partial class ShadowCasting {
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "code")]
        static IntVector2D LogAndEnqueue(Action<FovCone> enqueue, int range, IntVector2D top, 
                IntVector2D bottom, RiseRun riseRun, int code
        ) {
            if( top.GT(bottom)) {
                var cone = new FovCone(range+1, top, bottom, riseRun);
                FieldOfViewTrace(false, "  EQ: ({0}) code: {1}", cone, code);
                enqueue(cone);
                return bottom;
            } else {
                return top;
            }
        }

        static int XFromVector(int y,IntVector2D v) => (-2 * v.Y + v.X * (3 * y + 1) + (3 * v.Y) - 1) / (3 * v.Y);

        /// <summary>Helper IntMatrix2D for <c>VectorHexTop</c>.</summary>
        static IntMatrix2D matrixHexTop    = new IntMatrix2D(3,0,  0,3,  2, 1);

        /// <summary>Helper IntMatrix2D for <c>VectorHexBottom</c>.</summary>
        static IntMatrix2D matrixHexBottom = new IntMatrix2D(3,0,  0,3, -2,-1);

        /// <summary>IntVector2D for top corner of cell Canon(i,j).</summary>
        /// <remarks>
        /// In first dodecant; The top corner for hex (i,j) is determined 
        /// (from close visual inspection) as:
        ///       (i,j) + 1/3 * (2,1)
        /// which reduces to:
        ///       (i + 2/3, j + 1/3) == 1/3 * (3x + 2, 3y + 1)
        /// </remarks>
        static IntVector2D VectorHexTop(HexCoords hex) => hex.Canon * matrixHexTop;

        /// <summary>IntVector2D for bottom corner of cell Canon(i,j).</summary>
        /// <remarks>
        /// In first dodecant; The bottom corner for hex (i,j) is determined 
        /// (from close visual inspection) as:
        ///       (i,j) + 1/3 * (-2,-1)
        /// which reduces to:
        ///       (i - 2/3, j - 1/3) == 1/3 * (3x - 2, 3y - 1)
        /// </remarks>
        static IntVector2D VectorHexBottom(HexCoords hex) => hex.Canon * matrixHexBottom;

        #region These are here (instead of IntVector2D.cs) because they are "upside-down" for regular use.
        static IntVector2D VectorMax(IntVector2D lhs,IntVector2D rhs) => lhs.GT(rhs) ? lhs : rhs;

        static bool GT(this IntVector2D lhs,IntVector2D rhs) => lhs.X*rhs.Y > lhs.Y*rhs.X;

        static bool LE(this IntVector2D lhs,IntVector2D rhs) => !lhs.GT(rhs);
        #endregion
    }
}
