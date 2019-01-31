#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities;

#pragma warning disable 1587
/// <summary>TODO</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridExampleCommon {
    using HexSize    = System.Drawing.Size;
    using MapGridHex = IHex;
    using MapDef     = IReadOnlyList<string>;

    /// <summary>Example of <see cref="HexUtilities"/> usage to implement a terrain map.</summary>
    public sealed class TerrainMap : MapDisplayBlocked<MapGridHex> {
        /// <summary>TODO</summary>
        public TerrainMap() : base(_sizeHexes, new HexSize(26,30), InitializeHex) {}

        /// <inheritdoc/>
        public override int    ElevationBase =>  0;
        /// <inheritdoc/>
        public override int    ElevationStep => 10;

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "2*range", Justification="No map is big enough to overflow,")]
        public override short? Heuristic(HexCoords source, HexCoords target) => (short)(2 * source.Range(target));

        static MapDef          _board     = MapDefinitions.TerrainMapDefinition;
        static HexSize         _sizeHexes = new HexSize(_board[0].Length, _board.Count);

        public new static MapGridHex InitializeHex(HexCoords coords) {
            char value = _board[coords.User.Y][coords.User.X];
            switch(value) {
                case '.': return TerrainGridHex.NewPassable(coords, 0,0,value, 4); // Clear
                case '2': return TerrainGridHex.NewPassable(coords, 0,0,value, 2); // Pike
                case '3': return TerrainGridHex.NewPassable(coords, 0,0,value, 3); // Road
                case 'F': return TerrainGridHex.NewPassable(coords, 0,0,value, 5); // Ford
                case 'H': return TerrainGridHex.NewPassable(coords, 1,0,value, 5); // Hill
                case 'M': return TerrainGridHex.NewPassable(coords, 2,0,value, 6); // Mountain
                case 'W': return TerrainGridHex.NewPassable(coords, 0,7,value, 8); // Woods
                default:
                case 'R': return TerrainGridHex.NewImpassable(coords, 0,0,value);    // River
            }
        }
    }
}
