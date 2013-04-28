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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace PG_Napoleonics.Utilities {
  /// <summary>
  /// MinHeap from ZeraldotNet (http://zeraldotnet.codeplex.com/)
  /// Modified by Roy Triesscheijn (http://royalexander.wordpress.com)    
  /// -Moved method variables to class variables
  /// -Added English Exceptions and comments (instead of Chinese)    
  /// Adapted by PIeter Geerkens:
  /// </summary>    
  public sealed class MinListHeap<T> : IHeap<T> where T : struct, IComparable<T> {  
    /// <summary>Construct a new heap with default capacity of 16.</summary>
    public MinListHeap() : this(16) { }
    /// <summary>Construct a new heap with the specified capacity.</summary>
    public MinListHeap(int capacity) {
      list = new List<T>(capacity);
    }

    /// <inheritdoc/>
    public int Count        { get { return list.Count; } }

    /// <inheritdoc/>
    public bool IsEmpty     { get { return list.Count==0; } }

    /// <inheritdoc/>
    public void BuildHead() {
      for (var position = (Count-1) >> 1;  position >= 0;  position--) { MinHeapify(position); }
    }

    /// <inheritdoc/>
    public void Clear()     { list.Clear(); }

    /// <inheritdoc/>
    public void Add(T item) {            
      list.Add(item);
      int position        = Count-1;
      int parent_position = (position-1) >> 1;

      while (position > 0  &&  list[parent_position].CompareTo(list[position]) > 0) {
        ListSwap(position, parent_position);
        position          = parent_position;
        parent_position   = (position-1) >> 1;
      }
    }

    /// <inheritdoc/>
    public T ExtractFirst() {
      var item = list[0];            
      list[0]  = list.Last();
      list.RemoveAt(Count-1);
      MinHeapify(0);
      return item;
    }

    /// <inheritdoc/>
    public T Peek()         { return list[0]; }

    #region private internals
    List<T> list;

    void MinHeapify(int position) {
      do {
        var left        = (position << 1) + 1;
        var minPosition = (left < Count  &&  list[left].CompareTo(list[position]) < 0)
                        ? left
                        : position;

        var right       = left + 1;
        if (right < Count  &&  list[right].CompareTo(list[minPosition]) < 0) {
          minPosition   = right;
        }

        if (minPosition == position) return;

        ListSwap(position, minPosition);
        position        = minPosition;
      } while (true);
    }

    void ListSwap(int lhs, int rhs) { var heap = list[lhs];   list[lhs] = list[rhs];   list[rhs] = heap; }
    #endregion
  }
}
