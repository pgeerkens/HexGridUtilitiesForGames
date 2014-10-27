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

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>an immutable struct representing an associtaed Key and Value pair with equality
  /// and  comparabilitye of instances defined by the supplied TKey type.</summary>
  public struct HexKeyValuePair<TKey,TValue> 
    : IEquatable<HexKeyValuePair<TKey,TValue>>, 
      IComparable<HexKeyValuePair<TKey,TValue>>
    where TKey : struct, IEquatable<TKey>, IComparable<TKey> 
  {
    /// <summary>Constructs a new HexKeyValuePair instance.</summary>
    internal HexKeyValuePair(TKey key, TValue value) : this() {
      Key   = key;
      Value = value;
    }

    #region Properties
    /// <summary>TODO</summary>
    public TKey   Key     { get; private set; }
    /// <summary>TODO</summary>
    public TValue Value   { get; private set; }
    #endregion

    #region Value equality
    /// <inheritdoc/>
    public override bool Equals(object obj) {
      var other = obj as HexKeyValuePair<TKey,TValue>?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Key.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(HexKeyValuePair<TKey,TValue> other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) != 0;
    }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) == 0;
    }

    #region IComparable implementation
    /// <summary>Tests whether lhs &lt; rhs.</summary>
    public static bool operator <  (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) < 0;;
    }
    /// <summary>Tests whether lhs &lt;= rhs.</summary>
    public static bool operator <= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) <= 0;;
    }
    /// <summary>Tests whether lhs &gt;= rhs.</summary>
    public static bool operator >= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) >= 0;
    }
    /// <summary>Tests whether lhs &gt; rhs.</summary>
    public static bool operator >  (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs) {
      return lhs.CompareTo(rhs) > 0;
    }
    /// <inheritdoc/>
    public int CompareTo(HexKeyValuePair<TKey,TValue> other) { return this.Key.CompareTo(other.Key); }
    #endregion
    #endregion
  }
}
