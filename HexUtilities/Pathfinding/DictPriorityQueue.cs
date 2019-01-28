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
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Stable (insertion-order preserving for equal-priority elements) PriorityQueue implementation.</summary>
  /// <remarks>Eric Lippert's C# implementation of PriorityQueue for use by the A* algorithm.</remarks>
  /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx">Path Finding Using A* Part Three</a>
  /// <typeparam name="TPriority">Type of the queue-item prioirty.</typeparam>
  /// <typeparam name="TValue">Type of the queue-item value.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  [DebuggerDisplay("Count={Count}")]
  internal sealed class DictionaryPriorityQueue<TPriority,TValue> 
    : IPriorityQueue<TPriority,TValue>
    where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority>
//    where TValue : class
  {
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    }

    IDictionary<TPriority,Queue<TValue>> _dictionary = new SortedDictionary<TPriority,Queue<TValue>>();

    /// <inheritdoc/>
    bool IPriorityQueue<TPriority,TValue>.Any() { return this.Any; }

    /// <summary>Returns true exactly when the queue is not empty.</summary>
    public bool Any   { get { return this.Count > 0; } }

    /// <inheritdoc/>
    public int  Count { get { return _dictionary.Count; } }

    /// <inheritdoc/>
    public void Enqueue(TPriority priority, TValue value) {
      Enqueue(HexKeyValuePair.New(priority,value)); 
    }
    /// <inheritdoc/>
    public void Enqueue(HexKeyValuePair<TPriority,TValue> item) {
      Queue<TValue> queue;
      if( ! _dictionary.TryGetValue(item.Key, out queue) ) {
        queue = new Queue<TValue>();
        _dictionary.Add(item.Key, queue);
      }
      queue.Enqueue(item.Value);    // Only not-null values enqueued; so assumptions below valid.
    }

    /// <inheritdoc/>
    public bool TryDequeue(out HexKeyValuePair<TPriority,TValue> result) {
      if (_dictionary.Count > 0)  {
        var list = _dictionary.First();    
        var v    = list.Value.Dequeue();   
        result   = HexKeyValuePair.New(list.Key,v);
        if( list.Value.Count == 0)  _dictionary.Remove(list.Key);
        return true;
      }
      result = default(HexKeyValuePair<TPriority,TValue>);
      return false;
    }

    /// <inheritdoc/>
    public bool TryPeek(out HexKeyValuePair<TPriority,TValue> result) {
      if (_dictionary.Count > 0)  {
        var list = _dictionary.First();   
        var v    = list.Value.Peek();      
        result   = HexKeyValuePair.New(list.Key,v);
        return true;
      }
      result = default(HexKeyValuePair<TPriority,TValue>);
      return false;
    }

    /// <summary>TODO</summary>
    public void Clear() { _dictionary.Clear(); }

    /// <summary>TODO</summary>
    public bool Contains(TValue value) { 
      return Enumerable().Select(i => i.Value).Contains(value);
    }

    /// <summary>TODO</summary>
    public HexKeyValuePair<TPriority,TValue> Dequeue() { 
      HexKeyValuePair<TPriority,TValue> result;
      if (this.TryDequeue(out result)) return result;
      throw new InvalidOperationException();
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public IEnumerator<HexKeyValuePair<TPriority,TValue>> GetEnumerator() {
      foreach(var item in Enumerable()) 
        yield return item;
    }

    /// <summary>TODO</summary>
    public HexKeyValuePair<TPriority,TValue> Peek() { 
      HexKeyValuePair<TPriority,TValue> result;
      if (this.TryPeek(out result)) return result;
      throw new InvalidOperationException();
    }

    /// <summary>TODO</summary>
    public HexKeyValuePair<TPriority,TValue>[] ToArray() {
      return Enumerable().ToArray(); }

    IEnumerable<HexKeyValuePair<TPriority,TValue>> Enumerable() {
      return from list in _dictionary
             from item in list.Value
             select HexKeyValuePair.New(list.Key, item);
    }

    #region Not implemented yet - Synchronization and Clone
#if false
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public bool IsSynchronized { get { return false; } }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public object SyncRoot     { get { return null; } }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public object Clone() { throw new NotSupportedException(); }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public void TrimExcess() { throw new NotSupportedException(); }
#endif
    #endregion
  }
}
