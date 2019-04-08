#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Factory class for <see cref="HotPriorityQueue{TValue}"/></summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
            Justification="The suffix 'PriorityQueue' has an unambiguous meaning in the application domain.")]
    public static class HotPriorityQueue {
        private static IDictionary<int, IHotPriorityQueueList<int, TValue>> _factory<TValue>()
        => new SortedList<int, IHotPriorityQueueList<int, TValue>>();

        /// <summary>Constructs a new instance with a preferenceWidth of 0 bits and initialSize of 32 elements.</summary>
        /// <remarks>PreferenceWidth is the number of low-order bits on the key that are for 
        /// alignment, the remainder being the actual left-shifted distance estimate.</remarks>
        public static IPriorityQueue<int, TValue> New<TValue>()
        => New<TValue>(0, 32);

        /// <summary>returns a new instance with a preferenceWidth of 0 bits and initialSize as specified.</summary>
        /// <remarks></remarks>
        /// <param name="initialSize">Maximum size of the Heap-On-Top; initial Pool size will be set
        /// at 7/8 of this value. Powers of 2 work best for a value; the value must be at least 32.</param>
        public static IPriorityQueue<int, TValue> New<TValue>(int initialSize)
        => New<TValue>(0, initialSize);

        /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        /// <remarks></remarks>
        /// <param name="preferenceWidth">the number of low-order bits on 
        /// the key that are for alignment, the remainder being the actual left-shifted distance 
        /// estimate.</param>
        /// <param name="initialSize">Maximum size of the Heap-On-Top; initial Pool size will be 
        /// set at 7/8 of this value. Powers of 2 work best for a value.</param>
        public static IPriorityQueue<int, TValue> New<TValue>(int preferenceWidth, int initialSize)
        => New(preferenceWidth, initialSize, _factory<TValue>);

        /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        /// <param name="preferenceWidth">the number of low-order bits on 
        /// the key that are for alignment, the remainder being the actual left-shifted distance 
        /// estimate.</param>
        /// <param name="initialSize">Maximum size of the Heap-On-Top; initial Pool size will be set
        /// at 7/8 of this value. Powers of 2 work best for a value; the value must be at least 32.</param>
        /// <param name="factory"></param>
        /// <remarks>
        /// Out-of-the-box implementations of IDictionary{int, IHotPriorityQueueList{int,TValue}} include:
        ///   - () => new SortedList{int, IHotPriorityQueueList{int,TValue}}()
        ///   - () => new SortedDictionary{int, IHotPriorityQueueList{int,TValue}}()
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IPriorityQueue<int, TValue> New<TValue>(int preferenceWidth, int initialSize,
            Func<IDictionary<int, IHotPriorityQueueList<int, TValue>>> factory)
        => new HotPriorityQueue<TValue>(preferenceWidth, initialSize, factory);
    }

    /// <summary>Heap-On-Top (HOT) Priority Queue implementation with a key of type <c>int</c>.</summary>
    /// <typeparam name="TValue">Type of the queue-item value.</typeparam>
    /// <remarks>
    /// 
    /// </remarks>
    /// <a href="http://en.wikipedia.org/wiki/Heapsort">Wikepedia - Heapsort</a>/>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
            Justification="The suffix 'PriorityQueue' has an unambiguous meaning in the application domain.")]
    [DebuggerDisplay("Count={Count}")]
    public sealed class HotPriorityQueue<TValue> : IPriorityQueue<int,TValue> {
        /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        /// <remarks></remarks>
        /// <param name="preferenceWidth">the number of low-order bits on 
        /// the key that are for alignment, the remainder being the actual left-shifted distance 
        /// estimate.</param>
        /// <param name="initialSize">Maximum size of the Heap-On-Top; initial Pool size will be 
        /// set at 7/8 of this value. Powers of 2 work best for a value.</param>
        /// <param name="factory"></param>
        public HotPriorityQueue(int preferenceWidth, int initialSize,
            Func<IDictionary<int, IHotPriorityQueueList<int, TValue>>> factory
        ) {
            PoolSize        = initialSize * 7 / 8;
            PreferenceWidth = preferenceWidth; 
            Lists           = factory();
            _baseIndex      = 0;
            _queue          = new MinListHeap<int,TValue>(initialSize);
        }

        /// <summary>Returns whether any elements exist in the heap.</summary>
        public bool Any => _queue.Count > 0  ||  Lists.Count > 0;

        /// <summary>Returns the number of elements in the heap.</summary>
        public int Count => _queue.Count;

        /// <summary>The number of elements which are handled by a straight HeapPriorityQueue.</summary>
        /// <remarks>
        /// When the number of elements exceeds this value, additional lists are created 
        /// to handle the overflow elements of lower priority (higher <c>TKey</c> values.
        /// </remarks>
        public int  PoolSize { get; }

        IDictionary<int,IHotPriorityQueueList<int,TValue>> Lists { get; }

        int                        PreferenceWidth { get; }

        int                        _baseIndex;
        IPriorityQueue<int,TValue> _queue;

        /// <summary>Returns whether any elements exist in the heap.</summary>
        bool IPriorityQueue<int,TValue>.Any() => Any;

        /// <inheritdoc/>
        public void Enqueue(int priority, TValue value) => Enqueue(HexKeyValuePair.New(priority,value));

        /// <inheritdoc/>
        public void Enqueue(HexKeyValuePair<int,TValue> item) {
            var index = item.Key >> PreferenceWidth;
            if (index <= _baseIndex) {
                _queue.Enqueue(item);
            } else if (Lists.Count == 0  &&  _queue.Count < PoolSize) {
                _baseIndex = index;
                _queue.Enqueue(item);
            } else {
                if(!Lists.TryGetValue(index, out var list)) {
                    list = new HotPriorityQueueList<int, TValue>();
                    Lists.Add(index, list);
                }
                list.Add(item);
            }
        }

        /// <inheritdoc/>
        public bool TryDequeue(out HexKeyValuePair<int,TValue> result) {
            if (_queue.TryDequeue(out result)) return true;
            else if (Lists.Count > 0)          return (_queue = GetNextQueue()).TryDequeue(out result);
            else                               return false;
        }

        /// <inheritdoc/>
        public bool TryPeek(out HexKeyValuePair<int,TValue> result) {
            if (_queue.TryPeek(out result)) return true;
            else if (Lists.Count > 0)       return (_queue = GetNextQueue()).TryPeek(out result);
            else                            return false;
        }

        /// <summary>TODO</summary>
        private IPriorityQueue<int,TValue> GetNextQueue() {
            var list   = Lists.First();
            Lists.Remove(list.Key);
            _baseIndex = list.Key;
            return new MinListHeap<int,TValue>(list.Value);
        }
    }
}
