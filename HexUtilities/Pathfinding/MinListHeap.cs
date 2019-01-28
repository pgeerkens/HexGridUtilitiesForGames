#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>List implementation of a binary MinHeap PriorityQueue.</summary>    
  internal sealed class MinListHeap<TKey,TValue> : IPriorityQueue<TKey,TValue>
    where TKey : struct, IEquatable<TKey>, IComparable<TKey>
//    where TValue : class
  {
    #region Constructors
    /// <summary>Construct a new heap with the specified capacity.</summary>
    public MinListHeap(int capacity) { 
    //  Contract.Requires(capacity >= 0);
      _items = new List<HexKeyValuePair<TKey,TValue>>(capacity); 
    }

    /// <summary>Construct a new heap from <paramref name="list"/>.</summary>
    public MinListHeap(IEnumerable<HexKeyValuePair<TKey,TValue>> list) {
      list.RequiredNotNull("list");

      _items = list.ToList();
      for(var start = (_items.Count-1) / 2; start >=0; start--) MinHeapifyDown(start);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    //  Contract.Invariant(_items != null);
    }
    #endregion

    /// <summary>Returns true exactly when the heap has at least one element.</summary>
    public bool Any     { get { return _items.Count > 0; } }

    /// <inheritdoc/>
    public int  Count   { get { return _items.Count; } }

    /// <inheritdoc/>
    bool IPriorityQueue<TKey,TValue>.Any()   { return Any; }

    /// <inheritdoc/>
    public void Clear() { _items.Clear(); }

    /// <inheritdoc/>
    public void Enqueue(TKey key, TValue value) {
      Enqueue(HexKeyValuePair.New(key,value));
    }

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
    private void MinHeapifyDown(int currentIndex) {
    //  Contract.Requires(currentIndex >= 0);

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
