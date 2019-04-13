#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>_heap-On-Top (HOT) Priority _heap implementation.</summary>
    /// <typeparam name="TKey">Struct type for the keys used to prioritize values..</typeparam>
    /// <typeparam name="TValue">Type of the queue elements.</typeparam>
    /// <remarks>
    /// See: <a href="http://en.wikipedia.org/wiki/Heapsort">Wikepedia - Heapsort</a>/>
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification="The suffix 'List' has an unambiguous meaning in the application domain.")]
    [DebuggerDisplay("Count={Count}")]
    internal sealed class HotPriorityQueueList<TKey, TValue>: IHotPriorityQueueList<TKey, TValue>
    where TKey : struct, IEquatable<TKey>, IComparable<TKey> {
       /// <summary>Create a new instance with a capacity of 1024.</summary>
        public HotPriorityQueueList() : this(1024) {}

       /// <summary>Create a new instance with the specified capacity.</summary>
       /// <param name="capacity"></param>
        public HotPriorityQueueList(int capacity)
        => List = new List<HexKeyValuePair<TKey,TValue>>(capacity);

        /// <inheritdoc/>
        public int Count => List.Count;

        List<HexKeyValuePair<TKey,TValue>> List { get; }  // < backing store

        /// <inheritdoc/>
        public void Add(HexKeyValuePair<TKey,TValue> item) => List.Add(item);

        /// <inheritdoc/>
        public IEnumerator<HexKeyValuePair<TKey,TValue>> GetEnumerator() => List.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    }
}
