#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Function that returns a new IPriorityQueue&lt;int,IHex>.</summary>
    public delegate IPriorityQueue<int,IHex> QueueFactory();

#if false
    /// <summary>TODO</summary>
    /// <param name="here"></param>
    /// <param name="hexside"></param>
    /// <param name="there"></param>
    /// <returns></returns>
    public delegate Maybe<short> TryDirectedCost(IHex here, Hexside hexside, IHex there);
#else
    /// <summary>TODO</summary>
    /// <param name="here"></param>
    /// <param name="hexside"></param>
    /// <returns></returns>
    public delegate short? TryDirectedCost(HexCoords here, Hexside hexside);
#endif

    /// <summary>TODO</summary>
    public enum Direction {
        ///<summary>TODO</summary>
        ToHex   = 0,
        ///<summary>TODO</summary>
        FromHex = 1
    }

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
        private static IList<DirectedLandmark> BackingStore(HexCoords coords, INavigableBoard board, Func<IPriorityQueue<int,HexCoords>> queueFactory) {
            return new List<DirectedLandmark> {
                DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryEntryCost),
                DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryExitCost)
            }.AsReadOnly();
        }

        /// <summary>Populates and returns a new landmark at the specified board coordinates.</summary>
        /// <param name="coords">The <see cref="HexCoords"/> for this landmark.</param>
        /// <param name="backingStore">TODO</param>
        private Landmark(HexCoords coords, IList<DirectedLandmark> backingStore) {
            Coords        = coords;
            _backingStore = backingStore;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void ObjectInvariant() {
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
                    if (_backingStore[0] != null) { _backingStore[0].Dispose(); _backingStore[0] = null; }
                    if (_backingStore[1] != null) { _backingStore[1].Dispose(); _backingStore[1] = null; }
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
