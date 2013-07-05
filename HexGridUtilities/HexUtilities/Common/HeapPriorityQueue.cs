#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Linq;
using System.Text;

using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities.Common2 {
  /// <summary>Heap-On-Top (HOT) Priority Queue implementation.</summary>
  /// <typeparam name="TKey">Struct type for the keys used to prioritize values..</typeparam>
  /// <typeparam name="TValue">Type of the queue elements.</typeparam>
  /// <remarks>
  /// 
  /// </remarks>
  /// <seealso cref="http://en.wikipedia.org/wiki/Heapsort"/>
  [DebuggerDisplay("Count={Count}")]
  public sealed class HotPriorityQueueList<TKey, TValue> 
    : ICollection<HexKeyValuePair<TKey,TValue>>
    where TKey : struct, IEquatable<TKey>, IComparable<TKey>, IComparable
  {
    private List<HexKeyValuePair<TKey,TValue>> _list = new List<HexKeyValuePair<TKey,TValue>>(512);

    public void Add(HexKeyValuePair<TKey,TValue> item) { _list.Add(item); }

    public IPriorityQueue<TKey,TValue> GetPriorityQueue() {
      var queue = new HeapPriorityQueue(_list);
      _list     = null;
      return queue;
    }

    public IEnumerator<HexKeyValuePair<TKey,TValue>> GetEnumerator() { return _list.GetEnumerator(); }

    int  ICollection<HexKeyValuePair<TKey,TValue>>.Count      { get {return _list.Count;} }
    bool ICollection<HexKeyValuePair<TKey,TValue>>.IsReadOnly { get {return false;} }

    void ICollection<HexKeyValuePair<TKey,TValue>>.Clear() { throw new InvalidOperationException("Clear"); }
    bool ICollection<HexKeyValuePair<TKey,TValue>>.Contains(HexKeyValuePair<TKey,TValue> item) { 
      return _list.Contains(item); 
    }
    void ICollection<HexKeyValuePair<TKey,TValue>>.CopyTo(HexKeyValuePair<TKey,TValue>[] array, int index) {
      _list.CopyTo(array,index);
    }
    IEnumerator IEnumerable.GetEnumerator() { return _list.GetEnumerator(); }
    bool ICollection<HexKeyValuePair<TKey,TValue>>.Remove(HexKeyValuePair<TKey,TValue> item) {
      throw new InvalidOperationException("Remove"); 
    }

    /// <summary>Fast (but unstable) implementation of PriorityQueue.</summary>
    /// <typeparam name="TPriority"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    private sealed class HeapPriorityQueue : IPriorityQueue<TKey,TValue> {
      readonly MinListHeap _minHeap;

      /// <summary>Initializes a new instance of the HeapPriorityQueue class.</summary>
      public HeapPriorityQueue() { _minHeap = new MinListHeap(); }

      /// <summary>Initializes a new HeapPriorityQueue class with elements copied from the specified collection.</summary>
      /// <param name="collection">The collection whose elements are copied to the new HeapPriorityQueue.</param>
      public HeapPriorityQueue(IEnumerable<HexKeyValuePair<TKey, TValue>> collection) : base() {
        if (collection == null) throw new ArgumentNullException("collection");
        foreach (var item in collection) _minHeap.Add(item);
      }
      /// <summary>Initializes a new HeapPriorityQueue, reusing the storage of the specified List.</summary>
      /// <param name="collection">The List to be contents of the new HeapPriorityQueue.</param>
      public HeapPriorityQueue(List<HexKeyValuePair<TKey, TValue>> list) : base() {
        if (list == null) throw new ArgumentNullException("collection");
        _minHeap = new MinListHeap(list);
      }

      public bool Any() { return _minHeap.Any(); }

      public void Enqueue(TKey priority, TValue value) { 
        Enqueue(new HexKeyValuePair<TKey,TValue>(priority,value)); 
      }
      public void Enqueue(HexKeyValuePair<TKey,TValue> item) {
        _minHeap.Add(new HexKeyValuePair<TKey,TValue>(item.Key, item.Value));
      }

      public bool TryDequeue(out HexKeyValuePair<TKey,TValue> result) {
        if (_minHeap.Any())  {
          result = _minHeap.ExtractFirst();
          return true;
        }
        result = new HexKeyValuePair<TKey,TValue>();
        return false;
      }

      public bool TryPeek(out HexKeyValuePair<TKey,TValue> result) {
        if (_minHeap.Any())  {
          result = _minHeap.Peek();
          return true;
        }
        result = new HexKeyValuePair<TKey,TValue>();
        return false;
      }

      /// <summary>
      /// MinHeap from ZeraldotNet (http://zeraldotnet.codeplex.com/)
      /// Modified by Roy Triesscheijn (http://royalexander.wordpress.com)    
      /// -Moved method variables to class variables
      /// -Added English Exceptions and comments (instead of Chinese)    
      /// Adapted by PIeter Geerkens:
      /// </summary>    
      private sealed class MinListHeap {  
        /// <summary>Construct a new heap with default capacity of 16.</summary>
        public MinListHeap() : this(16) { }

        /// <summary>Construct a new heap with the specified capacity.</summary>
        public MinListHeap(int capacity) { 
          _items = new List<HexKeyValuePair<TKey,TValue>>(capacity); 
        }

        /// <summary>Initializes a new heap from the specified list, resuing the storage and 
        /// heapifying <i>in-place</i>.</summary>
        public MinListHeap(List<HexKeyValuePair<TKey,TValue>> list) {
          _items = list;

          // Min-Heapify by sifting-down from last parent in heap.
          var start = (list.Count - 1) / 2;
          while (start >= 0) { 
            var root = start;
            var end  = list.Count - 1;

            while (root*2 + 1 <= end) { 
              var child = root*2 + 1;       // (root*2 + 1 points to the left child)
              var swap  = root;             // (keeps track of child to swap with)

              // (check if root is smaller than left child)
              if (list[swap].CompareTo(list[child]) > 0)
                swap = child;

              // (check if right child exists, and if it's bigger than what we're currently swapping with)
              if (child+1 <= end && list[swap].CompareTo(list[child+1]) > 0 )
                swap = child + 1;

              // (check if we need to swap at all)
              if (swap == root) break;

              // swap and repeat to continue sifting down the child now
              var temp = list[root]; list[root] = list[swap]; list[swap] = temp;
              root = swap;
            }
              
            start = start - 1;
          }
        }

        /// <inheritdoc/>
        public int Count    { get { return _items.Count; } }

        /// <inheritdoc/>
        public bool Any()   { return _items.Any(); }

        /// <inheritdoc/>
        public void Clear() { _items.Clear(); }

        /// <inheritdoc/>
        public void Add(HexKeyValuePair<TKey,TValue> item) {            
          _items.Add(item);
          var child  = Count-1;
          var parent = (child-1) >> 1;

          while (child > 0  &&  _items[parent].CompareTo(_items[child]) > 0) {
            var heap = _items[parent];   _items[parent] = _items[child];   _items[child] = heap;  // swap
            child  = parent;
            parent = (child-1) >> 1;
          }
        }

        /// <inheritdoc/>
        public HexKeyValuePair<TKey,TValue> ExtractFirst() {
          if (_items.Count == 0) throw new InvalidOperationException("The heap is empty.");

          // save first item for return later.
          HexKeyValuePair<TKey, TValue> toReturn = _items[0];

          // Remove the first item if there will only be 0 or 1 items left after doing so.  
          if (_items.Count <= 2) 
            _items.RemoveAt(0);
          else { 
            // Remove the first item and move the last item to the front.
            _items[0] = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);

            MinHeapifyDownn(0);
          }

          // Return the item from the heap
          return toReturn;
        }

        void MinHeapifyDownn(int current) {
          while (true) {

            // identify smallest of parent and borh children
            int leftChild = 2 * current + 1, smallest;
            if (leftChild < _items.Count)    smallest = GetSmallest(leftChild,current);
            else break;

            int rightChild = leftChild + 1;
            if (rightChild < _items.Count)   smallest = GetSmallest(rightChild,smallest);

            // if nothing to swap, we're done
            if (current == smallest)  break;

            // swap smallest value up
            var temp         = _items[current];
            _items[current]  = _items[smallest];
            _items[smallest] = temp;

            // follow swapped value down and repeat until the tree is a heap
            current = smallest;
          }
        }

        int GetSmallest(int child, int smallest) {
          return _items[child].Key.CompareTo(_items[smallest].Key) < 0
                ? child
                : smallest;
        }

        /// <inheritdoc/>
        public HexKeyValuePair<TKey,TValue> Peek()         { 
          // Returns the first item
          if (_items.Count == 0) throw new InvalidOperationException("The heap is empty.");
          return _items[0]; 
        }

        List<HexKeyValuePair<TKey,TValue>> _items;

        /// <summary>Min-Heapify by sifting-down from last parent in heap.</summary>
        void MinHeapify() {
          var start = (_items.Count - 1) / 2;
          while (start >= 0) { 
            var root = start;
            var end  = _items.Count - 1;

            while (root*2 + 1 <= end) { 
              var child = root*2 + 1;       // (root*2 + 1 points to the left child)
              var swap  = root;             // (keeps track of child to swap with)

              // (check if root is smaller than left child)
              if (_items[swap].CompareTo(_items[child]) > 0)
                swap = child;

              // (check if right child exists, and if it's bigger than what we're currently swapping with)
              if (child+1 <= end && _items[swap].CompareTo(_items[child+1]) > 0 )
                swap = child + 1;

              // (check if we need to swap at all)
              if (swap == root) break;

              // swap and repeat to continue sifting down the child now
              var temp = _items[root]; _items[root] = _items[swap]; _items[swap] = temp;
              root = swap;
            }
              
            start = start - 1;
          }
        }
      }
    }
  }
}
