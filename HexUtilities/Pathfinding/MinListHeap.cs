#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>List implementation of a binary MinHeap PriorityQueue.</summary>    
    internal sealed class MinListHeap<TKey,TValue> : IPriorityQueue<TKey,TValue>
        where TKey : struct, IEquatable<TKey>, IComparable<TKey> {
        /// <summary>Construct a new heap with the specified capacity.</summary>
        public MinListHeap(int capacity) => _items = new List<HexKeyValuePair<TKey,TValue>>(capacity);

        /// <summary>Construct a new heap from <paramref name="list"/>.</summary>
        public MinListHeap(IEnumerable<HexKeyValuePair<TKey,TValue>> list) {
            _items = list?.ToList()??throw new ArgumentNullException("list");
            for(var start = (_items.Count-1) / 2; start >=0; start--) MinHeapifyDown(start);
        }

        /// <summary>Returns true exactly when the heap has at least one element.</summary>
        public bool Any => _items.Count > 0;

        /// <inheritdoc/>
        public int Count => _items.Count;

        /// <inheritdoc/>
        bool IPriorityQueue<TKey,TValue>.Any() => Any;

        /// <inheritdoc/>
        public void Clear() => _items.Clear();

        /// <inheritdoc/>
        public void Enqueue(TKey key,TValue value) => Enqueue(HexKeyValuePair.New(key,value));

        /// <inheritdoc/>
        public void Enqueue(HexKeyValuePair<TKey,TValue> item) {            
            _items.Add(item);
            var child  = _items.Count-1;
            var parent = (child-1) / 2;

            while (child > 0  &&  _items[parent] > _items[child]) {
                var heap = _items[parent];  _items[parent] = _items[child];  _items[child] = heap;
                child  = parent;
                parent = (child-1) / 2;
            }
        }

        /// <inheritdoc/>
        public bool TryDequeue(out HexKeyValuePair<TKey,TValue> result) {
            if (_items.Count == 0) {
                result = default;
                return false;
            }

            result = _items[0];

            // Remove the first item if neighbour will only be 0 or 1 items left after doing so.  
            if (_items.Count <= 2) 
                _items.RemoveAt(0);
            else { 
                // Remove the first item and move the last item to the front.
                _items[0] = _items[_items.Count - 1];
                _items.RemoveAt(_items.Count - 1);

                MinHeapifyDown(0);
            }
            return true;
        }

        /// <inheritdoc/>
        public bool TryPeek(out HexKeyValuePair<TKey,TValue> result) {
            if (_items.Count == 0) {
                result = default; 
                return false;
            } 

            result = _items[0];
            return true;
        }

        private List<HexKeyValuePair<TKey,TValue>> _items;  //!< backing store

        /// <summary>Min-Heapify by sifting-down from last parent in heap.</summary>
        private void MinHeapifyDown(int currentIndex) {
            int leftChildIndex;
            while ( (leftChildIndex = 2*currentIndex + 1) < _items.Count) {
                // identify smallest of parent and both children
                var smallestIndex   = _items[leftChildIndex] < _items[currentIndex] ? leftChildIndex 
                                                                                    : currentIndex;
                var rightChildIndex = leftChildIndex + 1;
                if (rightChildIndex < _items.Count && _items[rightChildIndex] < _items[smallestIndex])
                  smallestIndex = rightChildIndex;

                // if nothing to swap, ... then the tree is a heap
                if (currentIndex == smallestIndex)  break;
          
                // swap smallest value up
                var tempValue         = _items[currentIndex];
                _items[currentIndex]  = _items[smallestIndex];
                _items[smallestIndex] = tempValue;

                // follow swapped value down and repeat until the tree is a heap
                currentIndex = smallestIndex;
            }
        }
    }
}
