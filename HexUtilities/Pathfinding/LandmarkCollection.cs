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
//#define UseSortedDictionary
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

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
            landmarkCoords.RequiredNotNull("landmarks");
            Contract.Ensures(Contract.Result<ILandmarkCollection>() != null);

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
        [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void ObjectInvariant() => Contract.Invariant(_fastList != null);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private LandmarkCollection(ILandmark[] array) =>
            _fastList = (array ?? new ILandmark[0]).ToFastList();

        private LandmarkCollection(ParallelQuery<ILandmark> query) {
            var list = new List<ILandmark>();

            query.ForAll( landmark => { lock (_syncLock) list.Add(landmark); } );

            _fastList = list.ToArray().ToFastList();
        }

        private readonly object _syncLock = new object();

        /// <inheritdoc/>
        public int    IndexOf(ILandmark item) {
            Contract.Ensures(0 <= Contract.Result<int>()  &&  Contract.Result<int>() < _fastList.Count);
            return _fastList.IndexOf(item);
        }

        /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
        /// <param name="coords">Type HexCoords - Hex for which to return Landmark distanace.</param>
        /// <param name="landmarkToShow">Type int - Index of the Landmark from which to display distances.</param>
        public string DistanceFrom(HexCoords coords, int landmarkToShow) { 
            if (landmarkToShow < 0  ||  Count <= landmarkToShow) return "";

            return string.Format(CultureInfo.CurrentCulture,"{0,3}", this[landmarkToShow].DistanceFrom(coords));
        }

        /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
        /// <param name="coords">Type HexCoords - Hex for which to return Landmark distanace.</param>
        /// <param name="landmarkToShow">Type int - Index of the Landmark from which to display distances.</param>
        public string DistanceTo(HexCoords coords, int landmarkToShow) { 
            if (landmarkToShow < 0  ||  Count <= landmarkToShow) return "";

            return string.Format(CultureInfo.CurrentCulture,"{0,3}", this[landmarkToShow].DistanceTo(coords));
        }

        #region IFastList implemenation
        /// <inheritdoc/>
        public IEnumerator<ILandmark>                         GetEnumerator(){
            Contract.Ensures(Contract.Result<IEnumerator<ILandmark>>() != null);
            return ((IEnumerable<ILandmark>)this).GetEnumerator();;
        }
        IEnumerator                               IEnumerable.GetEnumerator() =>
            ((IEnumerable)_fastList).GetEnumerator();
        IEnumerator<ILandmark>         IEnumerable<ILandmark>.GetEnumerator() =>
            ((IEnumerable<ILandmark>)_fastList).GetEnumerator();
        IFastEnumerator<ILandmark> IFastEnumerable<ILandmark>.GetEnumerator() =>
            ((IFastEnumerable<ILandmark>)_fastList).GetEnumerator();

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
        public void Dispose() => Dispose(true);
        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;
        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        private void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    if (_fastList is IDisposable disposable) disposable.Dispose();

                    _fastList = null;
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
