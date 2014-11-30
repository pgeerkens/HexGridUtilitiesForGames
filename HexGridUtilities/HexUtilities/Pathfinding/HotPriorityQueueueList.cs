#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Heap-On-Top (HOT) Priority Queue implementation.</summary>
  /// <typeparam name="TKey">Struct type for the keys used to prioritize values..</typeparam>
  /// <typeparam name="TValue">Type of the queue elements.</typeparam>
  /// <remarks>
  /// 
  /// </remarks>
  /// <a href="http://en.wikipedia.org/wiki/Heapsort">Wikepedia - Heapsort</a>/>
  [DebuggerDisplay("Count={Count}")]
  internal sealed class HotPriorityQueueList<TKey, TValue> 
    : ICollection<HexKeyValuePair<TKey,TValue>>
    where TKey : struct, IEquatable<TKey>, IComparable<TKey>
  {
    /// <inheritdoc/>
    public HotPriorityQueueList() : this(1024) {}

    /// <inheritdoc/>
    public HotPriorityQueueList(int capacity) {
      _list = new List<HexKeyValuePair<TKey,TValue>>(capacity);
    }

    /// <inheritdoc/>
    public int  Count      { get {return _list.Count;} }

    /// <inheritdoc/>
    public bool IsReadOnly { get {return false;} }

    /// <inheritdoc/>
    public void Add(HexKeyValuePair<TKey,TValue> item) { _list.Add(item); }

    /// <inheritdoc/>
    public IPriorityQueue<TKey,TValue> PriorityQueue { 
      get { return new MinListHeap(ref _list); }
    }

    /// <inheritdoc/>
    public IEnumerator<HexKeyValuePair<TKey,TValue>> GetEnumerator() {
      return _list.GetEnumerator();
    }

    /// <inheritdoc/>
    public void Clear() { _list.Clear(); }

    /// <inheritdoc/>
    public bool Contains(HexKeyValuePair<TKey,TValue> item) { 
      return _list.Contains(item); 
    }

    /// <inheritdoc/>
    public void CopyTo(HexKeyValuePair<TKey,TValue>[] array, int arrayIndex) {
      _list.CopyTo(array,arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(HexKeyValuePair<TKey,TValue> item) {
      throw new InvalidOperationException("Remove"); 
    }

    IEnumerator IEnumerable.GetEnumerator() { return _list.GetEnumerator(); }

    List<HexKeyValuePair<TKey,TValue>> _list;  // < backing store

    /// <summary>List implementation of a binary MinHeap PriorityQueue.</summary>    
    private sealed class MinListHeap : IPriorityQueue<TKey,TValue> {
      #region Constructors
      /// <summary>Construct a new heap with default capacity of 16.</summary>
      public MinListHeap() : this(16) { }

      /// <summary>Construct a new heap with the specified capacity.</summary>
      public MinListHeap(int capacity) { 
        _items = new List<HexKeyValuePair<TKey,TValue>>(capacity); 
      }

      public MinListHeap(ref List<HexKeyValuePair<TKey,TValue>> list) {
        if (list == null) throw new ArgumentNullException("list");

        _items = list;
        for(var start = (_items.Count-1) / 2; start >=0; start--) MinHeapifyDown(start);
        list = null;
      }
      #endregion

      /// <summary>Returns true exactly when the heap has at least one element.</summary>
      public bool Any     { get {  return _items.Count > 0; } }

      /// <inheritdoc/>
      public int  Count   { get { return _items.Count; } }

      /// <inheritdoc/>
      bool IPriorityQueue<TKey,TValue>.Any()   { return Any; }

      /// <inheritdoc/>
      public void Clear() { _items.Clear(); }

      /// <inheritdoc/>
      public void Enqueue(TKey key, TValue value) {
        Enqueue(new HexKeyValuePair<TKey,TValue>(key,value));
      }

      /// <inheritdoc/>
      public void Enqueue(HexKeyValuePair<TKey,TValue> item) {            
        _items.Add(item);
        var child  = Count-1;
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
          result = default(HexKeyValuePair<TKey,TValue>);
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
          result = default(HexKeyValuePair<TKey,TValue>); 
          return false;
        } 

        result = _items[0];
        return true;
      }

      private List<HexKeyValuePair<TKey,TValue>> _items;  //!< backing store

      /// <summary>Min-Heapify by sifting-down from last parent in heap.</summary>
      private void MinHeapifyDown(int current) {

        int leftChild;
        while ( (leftChild = 2*current + 1) < _items.Count) {

          // identify smallest of parent and both children
          var smallest   = _items[leftChild] < _items[current] ? leftChild 
                                                               : current;
          var rightChild = leftChild + 1;
          if (rightChild < _items.Count && _items[rightChild] < _items[smallest])
            smallest = rightChild;

          // if nothing to swap, ... then the tree is a heap
          if (current == smallest)  break;
          
          // swap smallest value up
          var temp         = _items[current];
          _items[current]  = _items[smallest];
          _items[smallest] = temp;

          // follow swapped value down and repeat until the tree is a heap
          current = smallest;
        }
      }
    }
  }
}
