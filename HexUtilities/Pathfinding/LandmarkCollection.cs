#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
//#define UseSortedDictionary
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>A <b>ReadOnlyCollection</b> of defined <see cref="Landmark"/> locations.</summary>
    public sealed class LandmarkCollection : IFastList<ILandmark>, ILandmarkCollection {
        /// <summary>Creates a populated <see cref="Collection{T}"/> of <see cref="Landmark"/>
        /// instances.</summary>
        /// <param name="board">The board on which the collection of landmarks is to be instantiated.</param>
        /// <param name="landmarkCoords">Board coordinates of the desired landmarks</param>
        public static ILandmarkCollection New(
            INavigableBoard board, 
            IFastList<HexCoords> landmarkCoords
        ) {
          if (landmarkCoords==null) throw new ArgumentNullException("landmarkCoords");

            int degreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1);
            var query = from coords in landmarkCoords.AsParallel()
                                                     .WithDegreeOfParallelism(degreeOfParallelism)
                                                     .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                        #if UseSortedDictionary
                        select Landmark.DictionaryPriorityQueueLandmark(coords, board);
                        #else
                        select Landmark.HotPriorityQueueLandmark(coords, board);
                        #endif

            return Extensions.InitializeDisposable(() => new LandmarkCollection(query) );
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private LandmarkCollection(IList<ILandmark> list) => _fastList = list.ToFastList();

        private LandmarkCollection(ParallelQuery<ILandmark> query) {
            var list = new List<ILandmark>();

            query.ForAll( landmark => { lock (_syncLock) list.Add(landmark); } );

            _fastList = list.ToArray().ToFastList();
        }

        private readonly object _syncLock = new object();

        /// <inheritdoc/>
        public int IndexOf(ILandmark item) => _fastList.IndexOf(item);

        /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
        /// <param name="coords">Type HexCoords - Hex for which to return Landmark distanace.</param>
        /// <param name="landmarkToShow">Type int - Index of the Landmark from which to display distances.</param>
        public string DistanceFrom(HexCoords coords, int landmarkToShow) { 
            if (landmarkToShow < 0  ||  Count <= landmarkToShow) return "";

            return $"{this[landmarkToShow].DistanceFrom(coords),3}";
        }

        /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
        /// <param name="coords">Type HexCoords - Hex for which to return Landmark distanace.</param>
        /// <param name="landmarkToShow">Type int - Index of the Landmark from which to display distances.</param>
        public string DistanceTo(HexCoords coords, int landmarkToShow) { 
            if (landmarkToShow < 0  ||  Count <= landmarkToShow) return "";

            return $"{this[landmarkToShow].DistanceTo(coords),3}";
        }

        #region IFastList implemenation
        /// <inheritdoc/>
        public IEnumerator<ILandmark>                         GetEnumerator()
        => ((IEnumerable<ILandmark>)this).GetEnumerator();
        IEnumerator                               IEnumerable.GetEnumerator()
        => ((IEnumerable)_fastList).GetEnumerator();
        IEnumerator<ILandmark>         IEnumerable<ILandmark>.GetEnumerator()
        => ((IEnumerable<ILandmark>)_fastList).GetEnumerator();
        IFastEnumerator<ILandmark> IFastEnumerable<ILandmark>.GetEnumerator()
        => ((IFastEnumerable<ILandmark>)_fastList).GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<ILandmark> action) => _fastList.ForEach(action);

        /// <inheritdoc/>
        public void ForEach(FastIteratorFunctor<ILandmark> functor) => _fastList.ForEach(functor);

        /// <inheritdoc/>
        public int       Count           => _fastList.Count;
        /// <inheritdoc/>
        public ILandmark this[int index] => _fastList[index];

        IFastList<ILandmark> _fastList;
        #endregion

        #region IDisposable implementation w/o finalizer; as no unmanageed resources used by sealed class.
        /// <summary>Clean up any resources being used.</summary>
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        private void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    if (_fastList is IDisposable disposable) disposable.Dispose();
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
