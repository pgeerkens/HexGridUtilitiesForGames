#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.Pathfinding {

    /// <summary>.</summary>
    public static class LandmarkExtensions {
        /// <summary>TODO</summary>
        public static ILandmark DictionaryPriorityQueueLandmark(this INavigableBoard board, HexCoords coords) {
            var backingStore = BackingStore(board, coords, PriorityQueueFactory.NewDictionaryQueue<int,HexCoords>);
            return new Landmark(coords, backingStore);
        }

        /// <summary>TODO</summary>
        public static ILandmark HotPriorityQueueLandmark(this INavigableBoard board, HexCoords coords) {
            var backingStore = BackingStore(board, coords, PriorityQueueFactory.NewHotPriorityQueue<HexCoords>);
            return new Landmark(coords, backingStore);
        }

        /// <param name="coords">The <see cref="HexCoords"/> for this landmark.</param>
        /// <param name="board">IBoard{IHex} on which the landmark is to be created.</param>
        /// <param name="queueFactory">Function that returns a new <see cref="IPriorityQueue{TPriority,TValue}"</param>
        private static IList<DirectedLandmark> BackingStore(this INavigableBoard board, HexCoords coords,
            Func<IPriorityQueue<int,HexCoords>> queueFactory)
        => new List<DirectedLandmark> {
                DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryEntryCost),
                DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryExitCost)
        }.AsReadOnly();

        /// <summary>A board location storing shortest-path distances to every board hex.</summary>
        /// <remarks>
        /// A board location that stores shortest-path distances for every board hex, for use in 
        /// computing an accurate A* heuristic. A simple Dijkstra implementation is used to generate the 
        /// distances upon creation.
        /// </remarks>
        [DebuggerDisplay("Coords={Coords}")]
        private sealed class Landmark : ILandmark {
            /// <summary>Populates and returns a new landmark at the specified board coordinates.</summary>
            /// <param name="coords">The <see cref="HexCoords"/> for this landmark.</param>
            /// <param name="backingStore">TODO</param>
            public Landmark(HexCoords coords, IList<DirectedLandmark> backingStore) {
                Coords        = coords;
                _backingStore = backingStore;
            }

            /// <summary>Board coordinates for the landmark location.</summary>
            public  HexCoords Coords      { get; }

            /// <inheritdoc/>
            public  int DistanceTo  (HexCoords coords) =>
                _backingStore[(int)Direction.ToHex].Distance(coords);

            /// <inheritdoc/>
            public  int DistanceFrom(HexCoords coords) =>
                _backingStore[(int)Direction.FromHex].Distance(coords);

            private IList<DirectedLandmark> _backingStore { get; }
        }
    }
}
