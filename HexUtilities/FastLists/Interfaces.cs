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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FastList {
    /// <summary>Optimized ForEach.</summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IFastEnumerable<TItem> {
        /// <summary>Returns the items of a list in order.</summary>
        IFastEnumerator<TItem> GetEnumerator();
    }

    /// <summary>Returns the items of a list in order.</summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IFastEnumerator<TItem>{
        /// <summary>Return the next item in the enumeration.</summary>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool MoveNext(ref TItem item);
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification="The suffix has an unambiguous meaning in the application domain.")]
    public interface IFastList<TItem> : IEnumerable<TItem>,
        IFastEnumerable<TItem>, IForEachable<TItem>, IForEachable2<TItem> {
        /// <summary>Gets the item at location <paramref name="index"/>.</summary>
        TItem this[int index] { get; }

        /// <summary>Returns the current size of the list.</summary>
        int   Count { get; }

        /// <summary>Returns the index of the specified item in the internal array storage.</summary>
        /// <param name="item"></param>
        int   IndexOf(TItem item);
    }

    /// <summary>Delegated ForEach - <c>action</c> describes the work to be performed on each iteration.</summary>
    /// <typeparam name="TItem">The type of object being iterated.</typeparam>
    public interface IForEachable<TItem>{
        /// <summary>Perform the supplied <paramref name="action"/> for every item in the enumeration.</summary>
        void ForEach(Action<TItem> action);
    }

    /// <summary>Functored ForEach - <c>functor</c> describes the work to be performed on each iteration.</summary>
    /// <typeparam name="TItem">The type of object being iterated.</typeparam>
    public interface IForEachable2<TItem> {
        /// <summary>Perform the action specified by <paramref name="functor"/> for every item in the enumeration.</summary>
        void ForEach(FastIteratorFunctor<TItem> functor);
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification="The suffix has an unambiguous meaning in the application domain.")]
    internal interface IFastListX<T> : IFastList<T>  {
        void SetItem(int index, T value);
    }
}
