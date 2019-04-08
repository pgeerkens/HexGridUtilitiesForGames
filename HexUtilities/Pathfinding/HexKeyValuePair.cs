#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Builder for <see cref="HexKeyValuePair{TKey,TValue}"/>.</summary>
    public static class HexKeyValuePair {
        /// <summary>Constructs a new <see cref="HexKeyValuePair{TKey,TValue}"/> instance, with type inference.</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static HexKeyValuePair<TKey,TValue> New<TKey,TValue>(TKey key, TValue value)
        where TKey : struct, IEquatable<TKey>,IComparable<TKey>
        => new HexKeyValuePair<TKey,TValue>(key,value);
    }

    /// <summary>An immutable struct representing an associated Key and Value pair with equality
    /// and  comparabilitye of instances defined by the supplied TKey type.</summary>
    public struct HexKeyValuePair<TKey,TValue> 
    : IEquatable<HexKeyValuePair<TKey,TValue>>, 
      IComparable<HexKeyValuePair<TKey,TValue>>
    where TKey : struct, IEquatable<TKey>,IComparable<TKey> {
        /// <summary>Constructs a new HexKeyValuePair instance.</summary>
        internal HexKeyValuePair(TKey key, TValue value) : this() {
            Key   = key;
            Value = value;
        }

        /// <summary>TODO</summary>
        public TKey   Key     { get; }

        /// <summary>TODO</summary>
        public TValue Value   { get; }

        #region IComparable implementation
        /// <summary>Tests whether lhs &lt; rhs.</summary>
        public static bool operator <(HexKeyValuePair<TKey,TValue> lhs,HexKeyValuePair<TKey,TValue> rhs)
        => lhs.CompareTo(rhs) < 0;

        /// <summary>Tests whether lhs &lt;= rhs.</summary>
        public static bool operator <= (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs)
        => lhs.CompareTo(rhs) <= 0;

        /// <summary>Tests whether lhs &gt;= rhs.</summary>
        public static bool operator >=(HexKeyValuePair<TKey,TValue> lhs,HexKeyValuePair<TKey,TValue> rhs)
        => lhs.CompareTo(rhs) >= 0;

        /// <summary>Tests whether lhs &gt; rhs.</summary>
        public static bool operator >(HexKeyValuePair<TKey,TValue> lhs,HexKeyValuePair<TKey,TValue> rhs)
        => lhs.CompareTo(rhs) > 0;

        /// <inheritdoc/>
        public int CompareTo(HexKeyValuePair<TKey,TValue> other) => Key.CompareTo(other.Key);
        #endregion

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj)
        => (obj is HexKeyValuePair<TKey,TValue> other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(HexKeyValuePair<TKey,TValue> other) => CompareTo(other) == 0;

        /// <inheritdoc/>
        public override int GetHashCode() => Key.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator != (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs)
        => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator == (HexKeyValuePair<TKey,TValue> lhs, HexKeyValuePair<TKey,TValue> rhs)
        => lhs.Equals(rhs);
        #endregion
    }
}
