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
    [SuppressMessage("Microsoft.Naming","CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
            Justification = "The suffix 'PriorityQueue' has an unambiguous meaning in the application domain.")]
    public static class HotPriorityQueue {
        private static IDictionary<int,IHotPriorityQueueList<int,TValue>> _factory<TValue>()
        => new SortedList<int,IHotPriorityQueueList<int,TValue>>();

        /// <summary>Constructs a new instance with a preferenceWidth of 0 bits and initialSize of 32 elements.</summary>
        /// <remarks>PreferenceWidth is the number of low-order bits on the key that are for 
        /// alignment, the remainder being the actual left-shifted distance estimate.</remarks>
        public static IPriorityQueue<int,TValue> New<TValue>()
        => New<TValue>(0,32);

        /// <summary>returns a new instance with a preferenceWidth of 0 bits and initialSize as specified.</summary>
        /// <remarks></remarks>
        /// <param name="initialSize">Maximum size of the _heap-On-Top; initial Pool size will be set
        /// at 7/8 of this value. Powers of 2 work best for a value; the value must be at least 32.</param>
        public static IPriorityQueue<int,TValue> New<TValue>(int initialSize)
        => New<TValue>(0,initialSize);

        /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        /// <remarks></remarks>
        /// <param name="preferenceWidth">the number of low-order bits on 
        /// the key that are for alignment, the remainder being the actual left-shifted distance 
        /// estimate.</param>
        /// <param name="initialSize">Maximum size of the _heap-On-Top; initial Pool size will be 
        /// set at 7/8 of this value. Powers of 2 work best for a value.</param>
        public static IPriorityQueue<int,TValue> New<TValue>(int preferenceWidth,int initialSize)
        => New(preferenceWidth,initialSize,_factory<TValue>);

        /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        /// <param name="preferenceWidth">the number of low-order bits on 
        /// the key that are for alignment, the remainder being the actual left-shifted distance 
        /// estimate.</param>
        /// <param name="initialSize">Maximum size of the _heap-On-Top; initial Pool size will be set
        /// at 7/8 of this value. Powers of 2 work best for a value; the value must be at least 32.</param>
        /// <param name="factory"></param>
        /// <remarks>
        /// Out-of-the-box implementations of IDictionary{int, IHotPriorityQueueList{int,TValue}} include:
        ///   - () => new SortedList{int, IHotPriorityQueueList{int,TValue}}()
        ///   - () => new SortedDictionary{int, IHotPriorityQueueList{int,TValue}}()
        /// </remarks>
        [SuppressMessage("Microsoft.Design","CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IPriorityQueue<int,TValue> New<TValue>(int preferenceWidth,int initialSize,
            Func<IDictionary<int,IHotPriorityQueueList<int,TValue>>> factory)
        => new HotPriorityQueue<TValue>(preferenceWidth,initialSize,factory);
    }

    /// <summary>_heap-On-Top (HOT) Priority _heap implementation with a key of type <c>int</c>.</summary>
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
        /// <param name="initialSize">Maximum size of the _heap-On-Top; initial Pool size will be 
        /// set at 7/8 of this value. Powers of 2 work best for a value.</param>
        /// <param name="factory"></param>
        public HotPriorityQueue(int preferenceWidth, int initialSize,
            Func<IDictionary<int, IHotPriorityQueueList<int, TValue>>> factory
        ) {
            PoolSize         = initialSize * 7 / 8;
            _baseIndex       = 0;
            PreferenceWidth = preferenceWidth; 
            _heap           = new MinListHeap<int,TValue>(initialSize);
            _lists           = factory();
        }

        ///// <summary>Constructs a new instance with a preferenceWidth of 0 bits.</summary>
        ///// <remarks>PreferenceWidth is the number of low-order bits on the key that are for 
        ///// alignment, the remainder being the actual left-shifted distance estimate.</remarks>
        //public HotPriorityQueue() : this(0) {}
        ///// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        ///// <remarks></remarks>
        ///// <paramref name="preferenceWidth">the number of low-order bits on 
        ///// the key that are for alignment, the remainder being the actual left-shifted distance 
        ///// estimate.</paramref>
        //public HotPriorityQueue(int preferenceWidth) : this(preferenceWidth, 2048) {}
        ///// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
        ///// <remarks></remarks>
        ///// <param name="preferenceWidth">the number of low-order bits on 
        ///// the key that are for alignment, the remainder being the actual left-shifted distance 
        ///// estimate.</param>
        ///// <param name="initialSize">Maximum size of the _heap-On-Top; initial Pool size will be 
        ///// set at 7/8 of this value. Powers of 2 work best for a value.</param>
        //public HotPriorityQueue(int preferenceWidth, int initialSize) { 
        //    PoolSize         = initialSize >> 3 * 7;
        //    _baseIndex       = 0;
        //    PreferenceWidth = preferenceWidth; 
        //    _heap           = new HotPriorityQueueList<int,TValue>(initialSize).PriorityQueue;
        //#if UseSortedDictionary
        //    _lists = new SortedDictionary<int, HotPriorityQueueList<int, TValue>>();
        //#else
        //    _lists = new SortedList<int, HotPriorityQueueList<int,TValue>>();
        //#endif
        //}

        /// <summary>Returns whether any elements exist in the heap.</summary>
        public bool Any => _heap.Count > 0  ||  _lists.Count > 0;

        /// <summary>Returns the number of elements in the (on-top) heap.</summary>
        public int Count => _heap.Count;

        /// <summary>The number of elements which are handled by a straight HeapPriorityQueue.</summary>
        /// <remarks>
        /// When the number of elements exceeds this value, additional lists are created 
        /// to handle the overflow elements of lower priority (higher <c>TKey</c> values.
        /// </remarks>
        public int  PoolSize { get; }

        /// <summary>.</summary>
        public int  PreferenceWidth { get; }

        /// <inheritdoc/>
        bool IPriorityQueue<int,TValue>.Any() => Any;

        /// <inheritdoc/>
        public void Enqueue(int priority,TValue value)
        => Enqueue(new HexKeyValuePair<int,TValue>(priority,value));

        /// <inheritdoc/>
        public void Enqueue(HexKeyValuePair<int,TValue> item) {
            var index = item.Key >> PreferenceWidth;
            if (index <= _baseIndex) {
                _heap.Enqueue(item);
            } else if (_lists.Count == 0  &&  _heap.Count < PoolSize) {
                _baseIndex = index;
                _heap.Enqueue(item);
            } else {
                //HotPriorityQueueList<int,TValue> list;
                if( ! _lists.TryGetValue(index, out var list) ) {
                  list = new HotPriorityQueueList<int,TValue>();
                  _lists.Add(index, list);
                }
                list.Add(item);
            }
        }

        /// <inheritdoc/>
        public bool TryDequeue(out HexKeyValuePair<int,TValue> result) {
            if (_heap.TryDequeue(out result)) return true;
            else if (_lists.Count > 0)         return (_heap = GetNextQueue()).TryDequeue(out result);
            else                               return false;
        }

        /// <inheritdoc/>
        public bool TryPeek(out HexKeyValuePair<int,TValue> result) {
            if (_heap.TryPeek(out result)) return true;
            else if (_lists.Count > 0)      return (_heap = GetNextQueue()).TryPeek(out result);
            else                            return false;
        }

        /// <summary>TODO</summary>
        private IPriorityQueue<int,TValue> GetNextQueue() {
            var list   = _lists.First();
            _lists.Remove(list.Key);
            _baseIndex = list.Key;

            return new MinListHeap<int,TValue>(list.Value);
        }

        int _baseIndex;
        IPriorityQueue<int,TValue> _heap { get; set; }
        IDictionary<int,IHotPriorityQueueList<int,TValue>> _lists { get; }
    }
}
