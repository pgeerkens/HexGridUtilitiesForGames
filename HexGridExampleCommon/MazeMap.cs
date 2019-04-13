#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;
using System.Threading.Tasks;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridExampleCommon {
    using HexSize = System.Drawing.Size;
    using IMapDef = IReadOnlyList<string>;

    /// <summary>Example of <see cref="HexUtilities"/> usage to implement a maze map.</summary>
    public sealed class MazeMap : MapDisplayBlocked<TerrainGridHex> {
        public async static Task<MazeMap> NewAsync() {
            var map = new MazeMap();
            map.ForEachHex<TerrainGridHex,MazeMap>(hex => hex.IfHasValueDo(h=> h.SetCosts<TerrainGridHex>(c => map[c])));
            await map.ResetLandmarksAsync();
            return map;
        }

        public static MazeMap New() {
            var map = new MazeMap();
            map.ForEachHex<TerrainGridHex,MazeMap>(hex => hex.IfHasValueDo(h=> h.SetCosts<TerrainGridHex>(c => map[c])));
            map.ResetLandmarks();
            return map;
        }

        /// <summary>TODO</summary>
        private MazeMap() : base(_sizeHexes, new HexSize(26,30), InitializeHex) {}

        /// <inheritdoc/>
        public override int  ElevationBase   =>  0;

        /// <inheritdoc/>
        public override int  ElevationStep   => 10;

        /// <inheritdoc/>
        public override int? Heuristic(HexCoords source, HexCoords target)
        => MinimumStepCost * source.Range(target);

        /// <inheritdoc/>
        public override int? Heuristic(IHex source, IHex target) => Heuristic(source.Coords, target.Coords);

        /// <inheritdoc/>
        public override int? Heuristic(int range) => range;

        /// <summary>TODO</summary>
        protected override int MinimumStepCost => 1;

        static IMapDef         _board     = MapDefinitions.MazeMapDefinition;
        static HexSize         _sizeHexes = new HexSize(_board[0].Length, _board.Count);

        private new static TerrainGridHex InitializeHex(HexCoords coords) {
            var value = _board[coords.User.Y][coords.User.X];
            switch (value) {
                case '.': return TerrainGridHex.NewPassable   (coords,0, 0, value, 1); // Path
                default:  return TerrainGridHex.NewImpassable (coords,1,10, value);    // Wall
            }
        }
    }
}
