#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Diagnostics;

using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>A board location storing shortest-path distances to every board hex.</summary>
    /// <remarks>
    /// A board location that stores shortest-path distances for every board hex, for use in 
    /// computing an accurate A* heuristic. A simple Dijkstra implementation is used to generate
    /// the distances upon creation.
    /// </remarks>
    [DebuggerDisplay("Coords={Coords}")]
    public sealed partial class Landmark : ILandmark {
        internal Landmark(HexCoords coords, IBoardStorage<int> distanceTo, IBoardStorage<int> distanceFrom) {
            Coords       = coords;
            BackingStore = new IBoardStorage<int>[] { distanceTo, distanceFrom };
        }

        /// <inheritdoc/>
        public  HexCoords Coords { get; }

        /// <summary>TODO</summary>
        private IBoardStorage<int>[] BackingStore { get; }

        /// <inheritdoc/>
        public int DistanceTo(HexCoords coords)  => BackingStore[(int)Direction.ToHex][coords];

        /// <inheritdoc/>
        public int DistanceFrom(HexCoords coords) => BackingStore[(int)Direction.FromHex][coords];
    }
}
