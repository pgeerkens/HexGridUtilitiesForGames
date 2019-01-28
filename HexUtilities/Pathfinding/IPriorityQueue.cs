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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>TODO</summary>
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  [ContractClass(typeof(IPriorityQueueContract<,>))]
  public interface IPriorityQueue<TPriority,TValue> 
    where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority>
//    where TValue : class
  {
    /// <summary>Returns whether any elements exist in the heap.</summary>
    [Pure]bool Any();

    /// <summary>The number of items in the queue.</summary>
    int  Count { get; }

    /// <summary>Creates and adds an entry with the specified <c>priority</c> and <c>value</c> to the queue.></summary>
    /// <param name="priority">The <c>TPriority</c> value for the new entry.</param>
    /// <param name="value">The <c>TValue</c> value for the new entry.</param>
    void Enqueue(TPriority priority, TValue value);

    /// <summary>Adds the specified <c>HexKeyValuePair</c> to the queue.</summary>
    /// <param name="item">The <c>HexKeyValuePair {Tpriority,TValue}</c> entry to be added to the queue. </param>
    void Enqueue(HexKeyValuePair<TPriority,TValue> item);

    /// <summary>Returns whether the top queue entry has been successfully stored in <c>result</c> 
    /// and removed from the queue.</summary>
    bool TryDequeue(out HexKeyValuePair<TPriority,TValue> result);

    /// <summary>Returns whether the top queue entry has been successfully stored in <c>result</c>.</summary>
    [Pure]
    bool TryPeek(out HexKeyValuePair<TPriority,TValue> result);
  }

  /// <summary>ContractClass for <see cref="IPriorityQueue{TPriority,TValue}"/>.</summary>
  /// <typeparam name="TPriority"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  [ContractClassFor(typeof(IPriorityQueue<,>))]
  internal abstract class IPriorityQueueContract<TPriority,TValue> : IPriorityQueue<TPriority,TValue> 
    where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority>
//    where TValue : class
  {
    private IPriorityQueueContract() : base() { }

    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    //  Contract.Invariant(Count >= 0 );
    //  Contract.Invariant(Any() == (Count != 0) );
    }

    [Pure]abstract public bool Any();
    abstract public int  Count { get; }

    public void Enqueue(TPriority priority, TValue value) {
    //  Contract.Requires(value != null);
    }
    abstract public void Enqueue(HexKeyValuePair<TPriority, TValue> item);

    [SuppressMessage("Microsoft.Contracts","CC1069", Justification="Can't see any non-Pure methods here.")]
    public bool TryDequeue(out HexKeyValuePair<TPriority, TValue> result) {
    //  Contract.Ensures(!Contract.Result<bool>()
    //      ||  (Contract.ValueAtReturn<HexKeyValuePair<TPriority, TValue>>(out result) != null) );

      result = default(HexKeyValuePair<TPriority, TValue>);
      return default(bool);
    }

    [Pure]
    [SuppressMessage("Microsoft.Contracts","CC1069", Justification="Can't see any non-Pure methods here.")]
    public bool TryPeek(out HexKeyValuePair<TPriority, TValue> result) {
    //  Contract.Ensures(!Contract.Result<bool>()
    //      ||  (Contract.ValueAtReturn<HexKeyValuePair<TPriority, TValue>>(out result) != null) );

      result = default(HexKeyValuePair<TPriority, TValue>);
      return default(bool);
    }
  }
}
