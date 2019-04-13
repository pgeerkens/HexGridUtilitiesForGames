#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IFastEnumerable = IFastEnumerable<ILandmark>;
    using IFastEnumerator = IFastEnumerator<ILandmark>;

    /// <summary>A <b>ReadOnlyCollection</b> of defined <see cref="ILandmark"/> locations.</summary>
    public sealed class LandmarkCollection : ILandmarkCollection {
        /// <summary>Returns a new <see cref="ILandmarkCollection"/>populated parallelly using a <see cref="HotPriorityQueue"/>.</summary>
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
                        select board.HotPriorityQueueLandmark(coords);

            return new LandmarkCollection(query);
        }

        /// <summary>Returns a new <see cref="ILandmarkCollection"/> populated parallelly using a <see cref="DictionaryPriorityQueue{TPriority,TValue}"/>.</summary>
        /// <param name="board">The board on which the collection of landmarks is to be instantiated.</param>
        /// <param name="landmarkCoords">Board coordinates of the desired landmarks</param>
        public static ILandmarkCollection NewDictionary(
            INavigableBoard board, 
            IFastList<HexCoords> landmarkCoords
        ) {
            if (landmarkCoords==null) throw new ArgumentNullException("landmarkCoords");

            int degreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1);
            var query = from coords in landmarkCoords.AsParallel()
                                                     .WithDegreeOfParallelism(degreeOfParallelism)
                                                     .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                        select board.DictionaryPriorityQueueLandmark(coords);

            return new LandmarkCollection(query);
        }

        /// <summary>Returns a new  <see cref="ILandmarkCollection"/> populated serially using a <see cref="HotPriorityQueue"/>.</summary>
        /// <param name="board">The board on which the collection of landmarks is to be instantiated.</param>
        /// <param name="landmarkCoords">Board coordinates of the desired landmarks</param>
        public static ILandmarkCollection New2(
            INavigableBoard board, 
            IFastList<HexCoords> landmarkCoords
        ) {
            if (landmarkCoords==null) throw new ArgumentNullException("landmarkCoords");

            var query = ( from coords in landmarkCoords
                          select board.HotPriorityQueueLandmark(coords)
                        );

            return new LandmarkCollection(query);
        }

        private LandmarkCollection(IEnumerable<ILandmark> list) => _fastList = list.ToFastList();

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
        public IEnumerator<ILandmark>   GetEnumerator() => ((IEnumerable<ILandmark>)_fastList).GetEnumerator();
        IEnumerator         IEnumerable.GetEnumerator() => GetEnumerator();
        IFastEnumerator IFastEnumerable.GetEnumerator() => ((IFastEnumerable)_fastList).GetEnumerator();

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
    }
}
