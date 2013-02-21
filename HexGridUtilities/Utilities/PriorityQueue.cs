#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities {
  /// <summary>Eric Lippert's C# implementation for use by the A* algorithm.</summary>
  /// <remarks></remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx</cref>
  /// <typeparam name="TPriority">Type of the queue-item prioirty.</typeparam>
  /// <typeparam name="TValue">Type of the queue-item value.</typeparam>
  public class PriorityQueue<TPriority,TValue> 
    where TPriority:IComparable<TPriority>, IEquatable<TPriority> 
  {
    private SortedDictionary<TPriority,Queue<TValue>> list 
      = new SortedDictionary<TPriority,Queue<TValue>>();

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

    public bool IsEmpty { get { return !list.Any(); } }
  }
}
