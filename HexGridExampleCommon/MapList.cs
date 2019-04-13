#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridExampleCommon {
    using Map = Map<TerrainGridHex>;

    public static class MapList {
        public static IReadOnlyList<Map> Maps { get; } = new ReadOnlyCollection<Map>(
            new Map[] {
                new Map("Terrain Map", () => TerrainMap.New()),
                new Map("Maze Map",    () => MazeMap.New()),
                new Map("A* Bug Map",  () => AStarBugMap.New())
            } );
    }
}
