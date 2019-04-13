#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IFastEnumerable = IFastEnumerable<ILandmark>;
    using IFastEnumerator = IFastEnumerator<ILandmark>;

     /// <summary>A <b>ReadOnlyCollection</b> of defined <see cref="Landmark"/> locations.</summary>
    public sealed class LandmarkCollection : ILandmarkCollection {
        internal LandmarkCollection(IEnumerable<ILandmark> items)
        => _fastList = items.ToFastList();

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

        /// <inheritdoc/>
        public int IndexOf(ILandmark item) => _fastList.IndexOf(item);

        IFastList<ILandmark> _fastList;
        #endregion
    }
}
