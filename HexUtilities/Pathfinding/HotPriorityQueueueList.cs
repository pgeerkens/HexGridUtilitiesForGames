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

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Heap-On-Top (HOT) Priority Queue implementation.</summary>
  /// <typeparam name="TKey">Struct type for the keys used to prioritize values..</typeparam>
  /// <typeparam name="TValue">Type of the queue elements.</typeparam>
  /// <remarks>
  /// 
  /// </remarks>
  /// <a href="http://en.wikipedia.org/wiki/Heapsort">Wikepedia - Heapsort</a>/>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix 'List' has an unambiguous meaning in the application domain.")]
  [DebuggerDisplay("Count={Count}")]
  public sealed class HotPriorityQueueList<TKey,TValue> : IHotPriorityQueueList<TKey, TValue>
  where TKey : struct, IEquatable<TKey>, IComparable<TKey>
//  where TValue : class
  {
    /// <summary>Create a new instance with a capacity of 1024 elements.</summary>
    public HotPriorityQueueList() : this(1024) {}

    /// <summary>Create a new instance with the specified capacity.</summary>
    public HotPriorityQueueList(int capacity) {
    //  Contract.Requires(capacity > 0);
      _list = new List<HexKeyValuePair<TKey,TValue>>(capacity);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    //  Contract.Invariant(_list != null);
    }

    /// <inheritdoc/>
    public void Add(HexKeyValuePair<TKey,TValue> item) { _list.Add(item); }

    /// <inheritdoc/>
    public IEnumerator<HexKeyValuePair<TKey,TValue>> GetEnumerator() {
      return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() { return _list.GetEnumerator(); }

    IList<HexKeyValuePair<TKey,TValue>> _list;  // < backing store
  }
}
