#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Function that returns a new IPriorityQueue&lt;int,IHex>.</summary>
    public delegate IPriorityQueue<int,IHex> QueueFactory();

    /// <summary>TODO</summary>
    /// <param name="here"></param>
    /// <param name="hexside"></param>
    /// <returns></returns>
    public delegate short? TryDirectedCost(HexCoords here, Hexside hexside);

    /// <summary>A board location storing shortest-path distances to every board hex.</summary>
    /// <remarks>
    /// A board location that stores shortest-path distances for every board hex, for use in 
    /// computing an accurate A* heuristic. A simple Dijkstra implementation is used to generate the 
    /// distances upon creation.
    /// No Finalizer is implemented as the class possesses no unmanaged resources.
    /// </remarks>
    [DebuggerDisplay("Coords={Coords}")]
    public sealed partial class Landmark : ILandmark, IDisposable {
        /// <summary>TODO</summary>
        public static ILandmark DictionaryPriorityQueueLandmark(HexCoords coords, INavigableBoard board) {
            var backingStore = BackingStore(coords, board, ()=>PriorityQueueFactory.NewDictionaryQueue<int,HexCoords>());
            return Extensions.InitializeDisposable( () => new Landmark(coords, backingStore) );
        }

        /// <summary>TODO</summary>
        public static ILandmark HotPriorityQueueLandmark(HexCoords coords, INavigableBoard board) {
            var backingStore = BackingStore(coords, board, ()=>PriorityQueueFactory.NewHotPriorityQueue<HexCoords>(1024));
            return Extensions.InitializeDisposable( () => new Landmark(coords, backingStore ) );
        }

        /// <param name="coords">The <see cref="HexCoords"/> for this landmark.</param>
        /// <param name="board">IBoard{IHex} on which the landmark is to be created.</param>
        /// <param name="queueFactory">TODO</param>
        private static IList<DirectedLandmark> BackingStore(HexCoords coords, INavigableBoard board,
            Func<IPriorityQueue<int,HexCoords>> queueFactory)
        => new List<DirectedLandmark> {
                DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryEntryCost),
                DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryExitCost)
            }.AsReadOnly();

        /// <summary>Populates and returns a new landmark at the specified board coordinates.</summary>
        /// <param name="coords">The <see cref="HexCoords"/> for this landmark.</param>
        /// <param name="backingStore">TODO</param>
        private Landmark(HexCoords coords, IList<DirectedLandmark> backingStore) {
            Coords        = coords;
            _backingStore = backingStore;
        }

        /// <summary>Board coordinates for the landmark location.</summary>
        public  HexCoords Coords      { get; }

        /// <inheritdoc/>
        public  short? DistanceTo  (HexCoords coords) =>
            _backingStore[(int)Direction.ToHex].Distance(coords);

        /// <inheritdoc/>
        public  short? DistanceFrom(HexCoords coords) =>
            _backingStore[(int)Direction.FromHex].Distance(coords);

        private IList<DirectedLandmark> _backingStore { get; }

        #region IDisposable implementation w/o finalizer; as no unmanageed resources used by sealed class.
        /// <summary>Clean up any resources being used.</summary>
        public void Dispose() => Dispose(true);
        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;
        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        private void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    if(_backingStore != null) {
                        _backingStore[0]?.Dispose();
                        _backingStore[1]?.Dispose();
                    }
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
