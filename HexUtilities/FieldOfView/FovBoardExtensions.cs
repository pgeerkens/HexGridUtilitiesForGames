#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>.</summary>
    public static partial class FovBoardExtensions {
        /// <summary>Elevation of the ground Above Sea Level</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="hex">The hex being tested.</param>
        public static int ElevationASL(this IFovBoard @this, IHex hex)
        => @this.ElevationBase + hex.ElevationLevel * @this.ElevationStep;

        /// <summary>Elevation of the ground Above Sea Level</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="coords">The coordinates of the hex being tested.</param>
        public static int ElevationGroundASL(this IFovBoard @this, HexCoords coords)
        => @this[coords].Match(hex => @this.ElevationASL(hex), MaxValue32);

        /// <summary>Elevation of the hexside Above Sea Level</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="coords">The coordinates of the hex being tested.</param>
        /// <param name="hexside"></param>
        public static int ElevationHexsideASL(this IFovBoard @this, HexCoords coords, Hexside hexside)
        => @this[coords].Match(hex => @this.ElevationASL(hex) + hex.HeightHexside(hexside), MaxValue32);

        /// <summary>Elevation of the target Above Sea Level</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="coords">The coordinates of the hex being tested.</param>
        public static int ElevationTargetASL(this IFovBoard @this, HexCoords coords)
        => @this[coords].Match(hex => @this.ElevationASL(hex) + @this.HeightOfMan, MaxValue32);

        /// <summary>Elevation of the observer Above Sea Level</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="coords">The coordinates of the hex being tested.</param>
        public static int ElevationObserverASL(this IFovBoard @this, HexCoords coords)
        => @this[coords].Match(hex => @this.ElevationASL(hex) + @this.HeightOfMan, MaxValue32);

        /// <summary>Elevation of the terrain Above Sea Level</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="coords">The coordinates of the hex being tested.</param>
        public static int ElevationTerrainASL(this IFovBoard @this, HexCoords coords)
        => @this[coords].Match(hex => @this.ElevationASL(hex) + hex.HeightTerrain, MaxValue32);

        /// <summary>Returns whether the hex at location <paramref name="coords"/> is passable.</summary>
        /// <param name="this">The {IFovBoard} to be analyzed.</param>
        /// <param name="coords">The coordinates of the hex being tested.</param>
        public static bool IsOverseeable(this IFovBoard @this, HexCoords coords)
        => @this.MapSizeHexes.IsOnboard(coords);

        private static readonly Func<int> MaxValue32 = () => int.MaxValue;
    }
}