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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities {
  public interface IPriorityQueue<TPriority,TValue> 
    where TPriority : struct, IComparable<TPriority>, IEquatable<TPriority>
  {
    bool IsEmpty { get; }

    void                           Enqueue(TPriority priority, TValue value);
    TValue                         Dequeue();
    KeyValuePair<TPriority,TValue> Peek();
  }

  /// <summary>Stable (insertion-order preserving for equal-priority elements) PriorityQueue implementation.</summary>
  /// <remarks>Eric Lippert's C# implementation of PrioirtyQueue for use by the A* algorithm.</remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx</cref>
  /// <typeparam name="TPriority">Type of the queue-item prioirty.</typeparam>
  /// <typeparam name="TValue">Type of the queue-item value.</typeparam>
  public sealed class DictPriorityQueue<TPriority,TValue> : IPriorityQueue<TPriority,TValue>
    where TPriority : struct, IComparable<TPriority>, IEquatable<TPriority> 
  {
    IDictionary<TPriority,Queue<TValue>> list = new SortedDictionary<TPriority,Queue<TValue>>();

    public bool IsEmpty { get { return !list.Any(); } }

    public void Enqueue(TPriority priority, TValue value) {
      Queue<TValue> queue;
      if( ! list.TryGetValue(priority, out queue) ) {
        queue = new Queue<TValue>();
        list.Add(priority, queue);
      }
      queue.Enqueue(value);
    }

    public TValue Dequeue() {
      var pair = list.First();
      var v    = pair.Value.Dequeue();
      if( pair.Value.Count == 0)  list.Remove(pair.Key);

      return v;
    }

    public KeyValuePair<TPriority,TValue> Peek() { 
      var peek = list.First();
      return new KeyValuePair<TPriority,TValue>(peek.Key, peek.Value.Peek()); 
    }
  }
}
