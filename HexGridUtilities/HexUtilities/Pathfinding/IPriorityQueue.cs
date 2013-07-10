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

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>TODO</summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
  public interface IPriorityQueue<TPriority,TValue> 
    where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority>, IComparable
  {
    /// <summary>Returns whether any elements exist in the heap.</summary>
    bool Any();

    /// <summary>The number of items in the queue.</summary>
    int  Count { get; }

    /// <summary>Creates and adds an entry with the specified <c>priority</c> and <c>value</c> to the queue.></summary>
    /// <param name="priority">The <c>TPriority</c> value for the new entry.</param>
    /// <param name="value">The <c>TValue</c> value for the new entry.</param>
    void Enqueue(TPriority priority, TValue value);

    /// <summary>Adds the specified <c>&lt;HexKeyValuePair></c> to the queue.</summary>
    /// <param name="item">The <c>HexKeyValuePair&lt;Tpriority,TValue></c> entry to be added to the queue. </param>
    void Enqueue(HexKeyValuePair<TPriority,TValue> item);

    /// <summary>Returns whether the top queue entry has been successfully stored in <c>result</c> 
    /// and removed from the queue.</summary>
    bool TryDequeue(out HexKeyValuePair<TPriority,TValue> result);

    /// <summary>Returns whether the top queue entry has been successfully stored in <c>result</c>.</summary>
    bool TryPeek(out HexKeyValuePair<TPriority,TValue> result);
  }

  /// <summary>TODO</summary>
  public struct HexKeyValuePair<TKey,TValue> : IComparable<HexKeyValuePair<TKey,TValue>>
    where TKey : struct, IComparable<TKey>, IComparable
  {
    /// <summary>TODO</summary>
    public TKey   Key     { get; private set; }
    /// <summary>TODO</summary>
    public TValue Value   { get; private set; }

    /// <summary>TODO</summary>
    public HexKeyValuePair(TKey key, TValue value) : this() {
      Key   = key;
      Value = value;
    }
    /// <summary>TODO</summary>
    public int CompareTo(HexKeyValuePair<TKey,TValue> other) { 
      return Key.CompareTo(other.Key); 
    }
    /// <summary>TODO</summary>
    public override bool Equals(object obj) {
      return obj is HexKeyValuePair<TKey,TValue> 
        &&  Key.Equals(((HexKeyValuePair<TKey,TValue>)obj).Key);
    }
    /// <summary>TODO</summary>
    public override int GetHashCode() { return Key.GetHashCode(); }

    /// <summary>TODO</summary>
    public static bool operator == (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.Key.Equals(rhs.Key);
    }
    /// <summary>TODO</summary>
    public static bool operator != (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return ! (lhs == rhs);
    }
    /// <summary>TODO</summary>
    public static bool operator <  (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.Key.CompareTo(rhs.Key) < 0;;
    }
    /// <summary>TODO</summary>
    public static bool operator <= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.Key.CompareTo(rhs.Key) <= 0;;
    }
    /// <summary>TODO</summary>
    public static bool operator >= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.Key.CompareTo(rhs.Key) >= 0;
    }
    /// <summary>TODO</summary>
    public static bool operator >  (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.Key.CompareTo(rhs.Key) > 0;
    }
  }
}
