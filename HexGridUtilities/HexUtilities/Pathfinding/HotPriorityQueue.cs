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
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
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

    int                                                _baseIndex;
    int                                                _preferenceWidth;
    IPriorityQueue<int, TValue>                        _queue;
    IDictionary<int, HotPriorityQueueList<int,TValue>> _lists;

    /// <summary>Constructs a new instance with a preferenceWidth of 0 bits.</summary>
    /// <remarks>PreferenceWidth is the number of low-order bits on the key that are for 
    /// alignment, the remainder being the actual left-shifted distance estimate.</remarks>
    public HotPriorityQueue() : this(0) {}
    /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
    /// <remarks></remarks>
    /// <paramref name="preferenceWidth">the number of low-order bits on 
    /// the key that are for alignment, the remainder being the actual left-shifted distance 
    /// estimate.</paramref>
    public HotPriorityQueue(int preferenceWidth) : this(preferenceWidth, 2048) {}
    /// <summary>returns a new instance with a preferenceWidth of shift bits.</summary>
    /// <remarks></remarks>
    /// <param name="preferenceWidth">the number of low-order bits on 
    /// the key that are for alignment, the remainder being the actual left-shifted distance 
    /// estimate.</param>
    /// <param name="initialSize">Maximum size of the Heap-On-Top; initial Pool size will be 
    /// set at 7/8 of this value. Powers of 2 work best for a value.</param>
    public HotPriorityQueue(int preferenceWidth, int initialSize) { 
      PoolSize         = initialSize >> 3 * 7;
      _baseIndex       = 0;
      _preferenceWidth = preferenceWidth; 
      _queue           = new HotPriorityQueueList<int,TValue>(initialSize).PriorityQueue;
#if UseSortedDictionary
      _lists = new SortedDictionary<int, HotPriorityQueueList<int, TValue>>();
#else
      _lists = new SortedList<int, HotPriorityQueueList<int,TValue>>();
#endif
    }

    /// <summary>Returns whether any elements exist in the heap.</summary>
    bool IPriorityQueue<int,TValue>.Any()    { return this.Any; }

    /// <summary>Returns whether any elements exist in the heap.</summary>
    public bool Any      { get { return _queue.Count > 0  ||  _lists.Count > 0; } }

    /// <summary>Returns the number of elements in the heap.</summary>
    public int  Count    { get { return _queue.Count; } }

    /// <summary>The number of elements which are handled by a straight HeapPriorityQueue.</summary>
    /// <remarks>
    /// When the number of elements exceeds this value, additional lists are created 
    /// to handle the overflow elements of lower priority (higher <c>TKey</c> values.
    /// </remarks>
    public int  PoolSize { get; set; }

    /// <inheritdoc/>
    public void Enqueue(int priority, TValue value) {
      Enqueue(new HexKeyValuePair<int,TValue>(priority,value));
    }
    /// <inheritdoc/>
    public void Enqueue(HexKeyValuePair<int,TValue> item) {
      var index = item.Key >> _preferenceWidth;
      if (index <= _baseIndex) {
        _queue.Enqueue(item);
      } else if (_lists.Count == 0  &&  _queue.Count < PoolSize) {
        _baseIndex = index;
        _queue.Enqueue(item);
      } else {
        HotPriorityQueueList<int,TValue> list;
        if( ! _lists.TryGetValue(index, out list) ) {
          list = new HotPriorityQueueList<int,TValue>();
          _lists.Add(index, list);
        }
        list.Add(item);
      }
    }

    /// <inheritdoc/>
    public bool TryDequeue(out HexKeyValuePair<int,TValue> result) {
      if (_queue.TryDequeue(out result)) return true;
      else if (_lists.Count > 0)         return (_queue = GetNextQueue()).TryDequeue(out result);
      else                               return false;
    }

    /// <inheritdoc/>
    public bool TryPeek(out HexKeyValuePair<int,TValue> result) {
      if (_queue.TryPeek(out result))  return true;
      else if (_lists.Count > 0)       return (_queue = GetNextQueue()).TryPeek(out result);
      else                             return false;
    }

    /// <summary>TODO</summary>
    private IPriorityQueue<int,TValue> GetNextQueue() {
      var list   = _lists.First();
      _lists.Remove(list.Key);
      _baseIndex = list.Key;

      return list.Value.PriorityQueue;
    }
  }
}
