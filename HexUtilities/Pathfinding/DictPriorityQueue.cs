#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Stable (insertion-order preserving for equal-priority elements) PriorityQueue implementation.</summary>
    /// <remarks>Eric Lippert's C# implementation of PriorityQueue for use by the A* algorithm.</remarks>
    /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx">Path Finding Using A* Part Three</a>
    /// <typeparam name="TPriority">Type of the queue-item prioirty.</typeparam>
    /// <typeparam name="TValue">Type of the queue-item value.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification="The suffix has an unambiguous meaning in the application domain.")]
    [DebuggerDisplay("Count={Count}")]
    public sealed class DictionaryPriorityQueue<TPriority,TValue> : IPriorityQueue<TPriority,TValue>
    where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority> {
        IDictionary<TPriority,Queue<TValue>> _dictionary = new SortedDictionary<TPriority,Queue<TValue>>();

        /// <inheritdoc/>
        bool IPriorityQueue<TPriority,TValue>.Any() => this.Any;

        /// <summary>Returns true exactly when the queue is not empty.</summary>
        public bool Any => this.Count > 0;

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <inheritdoc/>
        public void Enqueue(TPriority priority,TValue value) => Enqueue(HexKeyValuePair.New(priority,value));

        /// <inheritdoc/>
        public void Enqueue(HexKeyValuePair<TPriority,TValue> item) {
            if (!_dictionary.TryGetValue(item.Key,out var queue)) {
                queue = new Queue<TValue>();
                _dictionary.Add(item.Key,queue);
            }
            queue.Enqueue(item.Value);    // Only not-null values enqueued; so assumptions below valid.
        }

        /// <inheritdoc/>
        public bool TryDequeue(out HexKeyValuePair<TPriority,TValue> result) {
            if (_dictionary.Count > 0)  {
                var list = _dictionary.First();    
                var v    = list.Value.Dequeue();   
                result   = HexKeyValuePair.New(list.Key, v);
                if( list.Value.Count == 0)  _dictionary.Remove(list.Key);
                return true;
            }
            result = default;
            return false;
        }

        /// <inheritdoc/>
        public bool TryPeek(out HexKeyValuePair<TPriority,TValue> result) {
            if (_dictionary.Count > 0)  {
                var list = _dictionary.First();   
                var v    = list.Value.Peek();      
                result   = HexKeyValuePair.New(list.Key, v);
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>TODO</summary>
        public void Clear() => _dictionary.Clear();

        /// <summary>TODO</summary>
        public bool Contains(TValue value) => Enumerable().Select(i => i.Value).Contains(value);

        /// <summary>TODO</summary>
        public HexKeyValuePair<TPriority,TValue> Dequeue() {
            if (TryDequeue(out var result)) return result;
            throw new InvalidOperationException();
        }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerator<HexKeyValuePair<TPriority,TValue>> GetEnumerator() {
            foreach(var item in Enumerable()) yield return item;
        }

        /// <summary>TODO</summary>
        public HexKeyValuePair<TPriority,TValue> Peek() {
            if (TryPeek(out var result)) return result;
            throw new InvalidOperationException();
        }

        /// <summary>TODO</summary>
        public HexKeyValuePair<TPriority,TValue>[] ToArray() => Enumerable().ToArray();

        IEnumerable<HexKeyValuePair<TPriority,TValue>> Enumerable()
        => from list in _dictionary
           from item in list.Value
           select HexKeyValuePair.New(list.Key,item);

        #region Not implemented yet - Synchronization and Clone
#if false
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public bool IsSynchronized { get { return false; } }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public object SyncRoot     { get { return null; } }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public object Clone() { throw new NotSupportedException(); }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public void TrimExcess() { throw new NotSupportedException(); }
#endif
        #endregion
    }
}
