#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>Extension methods to support the identification of and translation between Dodecants.</summary>
    internal class Dodecant {
        /// <summary>The dodecant (30 degree arcs) transformations.</summary>
        /// <remarks>
        /// These transformations 
        /// 
        ///           Sextant map
        ///                    X-axis
        ///         \     |     /
        ///           \ 3 | 2 /
        ///             \ | / 
        ///          4    +    1     
        ///             / | \
        ///           / 5 | 0 \  
        ///         /     |     \
        ///             Y-axis
        /// </remarks>
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

        public Dodecant(Dodecant dodecant, IntMatrix2D matrixOrigin) {
            HexsideMap = dodecant.HexsideMap;
            Matrix     = dodecant.Matrix * matrixOrigin;
        }

        private Dodecant(int hexsideBase, IntMatrix2D matrix) {
            var sign   = Math.Sign(matrix.Determinant);
            HexsideMap = hexside => _hexsideMap[hexsideBase + sign * hexside];
            Matrix     = matrix;
        }

        public  Func<Hexside, Hexside> HexsideMap { get; }
        public  IntMatrix2D            Matrix     { get; }

        #region Mod6 - replace an integer modulo operation with a lookup into a table twice as long 
        /// <summary>Replaces an modulo-6 operation with a lookup into a table twice as long.</summary>
        private static readonly IList<Hexside> _hexsideMap = new Hexside[] {
            Hexside.North, Hexside.Northeast, Hexside.Southeast, 
            Hexside.South, Hexside.Southwest, Hexside.Northwest,

            Hexside.North, Hexside.Northeast, Hexside.Southeast, 
            Hexside.South, Hexside.Southwest, Hexside.Northwest 
        };
        #endregion

        #region TranslateDodecant
        public Action<HexCoords> TranslateDodecant(Action<HexCoords> action)
        => (coords) => action(HexCoords.NewCanonCoords(coords.Canon * Matrix));

        public Func<HexCoords,T> TranslateDodecant<T>(Func<HexCoords,T> func)
        => (coords) => func(HexCoords.NewCanonCoords(coords.Canon * Matrix));

        public Func<HexCoords,Hexside,T> TranslateDodecant<T>(Func<HexCoords,Hexside,T> func)
        => (coords,hexside) => func(HexCoords.NewCanonCoords(coords.Canon * Matrix), HexsideMap(hexside));
        #endregion
    }
}
