using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.FastLists {
    public abstract partial class AbstractFastList<TItem> {
        /// <summary>Implements IEnumerable{TItem} in the <i><b>standard</b></i> way:</summary>
        /// <typeparam name="TItem2">Type of the objects being enumerated.</typeparam>
        /// <remarks>Adapted from 
        /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
        /// The Cost of Enumeration in DotNet</a>
        /// </remarks>
        [DebuggerDisplay("Count={Count}")]
        private sealed partial class ClassicEnumerable<TItem2> : IEnumerator<TItem2> {
            private readonly TItem2[] _array;       //!< Array being enumerated..
            private          int      _index = -1;  //!< Index of the currently-enumerated element.

            /// <inheritdoc/>
            public TItem2 Current { get { return _array[_index]; } }
            /// <inheritdoc/>
            object IEnumerator.Current { get { return Current; } }
            /// <inheritdoc/>
            public bool MoveNext() { return ++_index < _array.Length; }
            /// <inheritdoc/>
            public void Reset() { _index = -1; }

            #region IDisposable implementation
            /// <inheritdoc/>
            public void Dispose() { GC.SuppressFinalize(this); }
            #endregion
        }
    }
}
