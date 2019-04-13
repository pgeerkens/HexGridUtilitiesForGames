#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace PGNapoleonics.HexUtilities {
    /// <summary>External interface exposed by individual hexes.</summary>
    public interface IHex {
        /// <summary>The <c>HexCoords</c> coordinates for this hex on <c>Board</c>.</summary>
        HexCoords Coords         { get; }

        /// <summary>Elevation of this hex in "steps" above the minimum elevation of the board.</summary>
        int       ElevationLevel { get; }

        /// <summary>Height ASL in <i>game units</i> of observer's eyes for FOV calculations.</summary>
        int       HeightObserver { get; }

        /// <summary>Height ASL in <i>game units</i> of target above ground level to be spotted.</summary>
        int       HeightTarget   { get; }

        /// <summary>Height ASL in <i>game units</i> of any blocking terrian in this hex.</summary>
        int       HeightTerrain  { get; }
 
        /// <summary>Char code for the tYpe of the terrain in this hex.</summary>
        char      TerrainType    { get; }

        /// <summary>Returns true exactly when thhis hex is passable.</summary>
        bool      IsPassable     { get; }

        /// <summary>Cost to extend the path with the hex located across the <c>Hexside</c> at <c>direction</c>.</summary>
        int       EntryCost(Hexside hexsideExit);

        /// <summary>Cost to extend the path with the hex located across the <c>Hexside</c> at <c>direction</c>.</summary>
        int       ExitCost(Hexside hexsideExit);

        /// <summary>Returns the cost (> 0) to extend the path across <paramref name="hexsideExit"/>; or null.</summary>
        int?      StepCost(Hexside hexsideExit);

        /// <summary>Height ASL in <i>game units</i> of any blocking terrain in this hex and the specified Hexside.</summary>
        int       HeightHexside(Hexside hexside);
    }
}
