#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridExampleCommon {
    using HexSize = System.Drawing.Size;
    using IMapDef = IReadOnlyList<string>;

    /// <summary>Example of <see cref="HexUtilities"/> usage to implement a terrain map.</summary>
    public sealed class TerrainMap : MapDisplayBlocked<TerrainGridHex> {
        public async static Task<TerrainMap> NewAsync() {
            var map = new TerrainMap();
            map.ForEachHex<TerrainGridHex,TerrainMap>(hex => hex.IfHasValueDo(h=> h.SetCosts<TerrainGridHex>(c => map[c])));
            await map.ResetLandmarksAsync();
            return map;
        }

        public static TerrainMap New() {
            var map = new TerrainMap();
            map.ForEachHex<TerrainGridHex,TerrainMap>(hex => hex.IfHasValueDo(h=> h.SetCosts<TerrainGridHex>(c => map[c])));
            map.ResetLandmarks();
            return map;
        }

        /// <summary>TODO</summary>
        private TerrainMap() : base(_sizeHexes, new HexSize(26,30), InitializeHex) {}

        /// <inheritdoc/>
        public override int  ElevationBase =>  0;

        /// <inheritdoc/>
        public override int  ElevationStep => 10;

        /// <inheritdoc/>
        public override int? Heuristic(HexCoords source, HexCoords target) => 2 * source.Range(target);

        /// <inheritdoc/>
        public override int? Heuristic(IHex source, IHex target) => Heuristic(source.Coords, target.Coords);

        /// <inheritdoc/>
        public override int? Heuristic(int range) => range;

        static IMapDef       _board     = MapDefinitions.TerrainMapDefinition;
        static HexSize       _sizeHexes = new HexSize(_board[0].Length, _board.Count);

        public new static TerrainGridHex InitializeHex(HexCoords coords) {
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
