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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FastList {
    /// <summary>Adapted implementation of Joe Duffy's Simple (Fast) List enumerator.</summary>
    /// <remarks>
    /// This entire namespace is adapted from 
    /// <a href="http://joeduffyblog.com/2008/09/21/the-cost-of-enumerating-in-net/">
    /// The Cost of Enumeration in DotNet</a> by Joe Duffy.
    /// 
    /// The idea behind IFastEnumerable{TItem} (and specifically IFastEnumerator{TItem}) is to
    /// return the current element during the call to MoveNext itself.  This cuts the number of
    /// interface method calls necessary to enumerate a list in half.  The impact to performance
    /// isn’t huge, but it was enough to cut our overhead from about 3X to 2.3X.  Every little
    /// bit counts.
    /// </remarks>
    /// <typeparam name="TItem">The Type of the Item to be stored and iterated over.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification="The suffix has an unambiguous meaning in the application domain.")]
    [DebuggerDisplay("Count={Count}")]
    public abstract partial class AbstractFastList<TItem> : IFastList<TItem>, IFastListX<TItem> {
        /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
        protected AbstractFastList(TItem[] array) => _array = array;

        /// <inheritdoc/>>
        public IEnumerator<TItem> GetEnumerator()
        => Extensions.InitializeDisposable( () => new ClassicEnumerable<TItem>(_array) );

        /// <inheritdoc/>>
        IEnumerator  IEnumerable.GetEnumerator() => GetEnumerator();
    
        IFastEnumerator<TItem> IFastEnumerable<TItem>.GetEnumerator() => new FastEnumerable<TItem>(_array);

        /// <summary>IForEachable{TItem} implementation.</summary>
        public   void  ForEach(Action<TItem> action) {
            TItem[] array = _array;
            for (int i = 0; i < array.Length; i++)    action(array[i]);
        }

        /// <inheritsdoc/>
        void IForEachable<TItem>.ForEach(Action<TItem> action) => ForEach(action);

        /// <summary>IForEachable2{TItem} implementation</summary>
        public   void  ForEach(FastIteratorFunctor<TItem> functor) {
            TItem[] array = _array;
            for (int i = 0; i < array.Length; i++)    functor.Invoke(array[i]);
        }

        /// <inheritsdoc/>
        void IForEachable2<TItem>.ForEach(FastIteratorFunctor<TItem> functor) => ForEach(functor);

        /// <inheritdoc/>
        public   int   Count               => _array.Length;

        /// <inheritdoc/>
        public   TItem this[int index]     => _array[index];

        /// <inheritdoc/>
        public   int   IndexOf(TItem item) => Array.IndexOf(_array, item, 0, _array.Length);

        /// <summary>Use carefully - must not interfere with iterators.</summary>
        void  IFastListX<TItem>.SetItem(int index, TItem value) => _array[index] = value;

        /// <summary>TODO</summary>
        private readonly TItem[] _array;

        /// <summary>Implements IEnumerable{TItem} in the <i>standard</i> way:</summary>
        /// <typeparam name="TItem2">Type of the objects being enumerated.</typeparam>
        [DebuggerDisplay("Count={Count}")]
        private sealed class ClassicEnumerable<TItem2> : IEnumerator<TItem2> {
            /// <summary>Construct a new instance from array <c>a</c>.</summary>
            /// <param name="array">The array of type <c>TItem</c> to make enumerable.</param>
            internal ClassicEnumerable(TItem2[] array) => _array = array;

            private readonly TItem2[] _array;       //!< Array being enumerated..
            private          int      _index = -1;  //!< Index of the currently-enumerated element.

            /// <inheritdoc/>
            public TItem2      Current    => _array[_index];

            /// <inheritdoc/>
            object IEnumerator.Current    => Current;

            /// <inheritdoc/>
            public bool        MoveNext() => ++_index < _array.Length;

            /// <inheritdoc/>
            public void        Reset()    => _index = -1;

            /// <inheritdoc/>
            /// <remarks> No unmanaged resources owned - so simply suppress finalization. </remarks>
            public void Dispose() => GC.SuppressFinalize(this);
        }
    }
}
