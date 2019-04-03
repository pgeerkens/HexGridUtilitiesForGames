#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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

using PGNapoleonics.HexUtilities.Common;

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