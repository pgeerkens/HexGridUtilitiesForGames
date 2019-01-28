#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities {
    /// <summary>TODO</summary>
    public class HexsideCosts : ReadOnlyCollection<short?> {
        /// <summary>TODO</summary>
        public static HexsideCosts ExitCosts<THex>(BoardStorage<Maybe<THex>> boardHexes, HexCoords hexCoords)
        where THex : IHex {
            return new HexsideCosts(hexside => DirectedCost(boardHexes, hexCoords, hexside) );
        }
        /// <summary>TODO</summary>
        public static HexsideCosts EntryCosts<THex>(BoardStorage<Maybe<THex>> boardHexes, HexCoords hexCoords)
        where THex : IHex {
            return new HexsideCosts(hexside => DirectedCost(boardHexes, hexCoords.GetNeighbour(hexside), hexside.Reversed) );
        }

        /// <summary>TODO</summary>
        public static short? DirectedCost<THex>(BoardStorage<Maybe<THex>> boardHexes, HexCoords hexCoords, Hexside hexside)
        where THex : IHex {
            return boardHexes[hexCoords].Bind(hex => hex.TryStepCost(hexside).ToMaybe()).ToNullable();
        }

        private HexsideCosts(Func<Hexside,short?> directedCostToExit) : base(Generator(directedCostToExit)) { }

        static List<short?> Generator(Func<Hexside,short?> directedCostToExit) {

            return ( from hexsideExit in Hexside.HexsideList
                     select directedCostToExit(hexsideExit)
                   ).ToList();
        }
    }
}
