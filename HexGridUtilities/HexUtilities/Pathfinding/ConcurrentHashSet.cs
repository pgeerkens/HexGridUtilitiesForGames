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
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Provides a thread-safe hash set data structure.</summary>
  /// <typeparam name="TKey">Specifies the type of elements in the hash set.</typeparam>
  /// <typeparam name="TValue">Type of the element value in the hash set.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [DebuggerDisplay("Count={Count}")]
  public class ConcurrentHashSet<TKey, TValue> : ISet<TKey> where TKey : IEquatable<TKey>
  {
    private readonly HashSet<TKey> _hashSet  = new  HashSet<TKey>();
    private readonly object        _syncLock = new object();

    /// <summary>Initializes a new instance of the <c>ConcurrentHashSet</c> class.</summary>
    public ConcurrentHashSet() {}

    /// <summary>Initializes a new instance of the <c>ConcurrentHashSet</c> class that 
    /// contains elements copied from the specified collection.</summary>
    /// <param name="collection">The collection whose elements are copied to the new 
    /// <c>ConcurrentHashSet</c>.</param>
    public ConcurrentHashSet(IEnumerable<TKey> collection)
    {
      if (collection == null) throw new ArgumentNullException("collection");
      foreach (var item in collection) _hashSet.Add(item);
    }

    /// <inheritdoc/>
    public int                     Count      { get { lock (_syncLock) return _hashSet.Count; } }

    /// <inheritdoc/>
    public IEqualityComparer<TKey> Comparer   { get { lock (_syncLock) return _hashSet.Comparer; } }

    /// <inheritdoc/>
    public bool                    IsReadOnly { get { lock (_syncLock) return false; } }

    /// <inheritdoc/>
    bool ISet<TKey>.Add(TKey item) { lock (_syncLock) return _hashSet.Add(item); }
    /// <inheritdoc/>
    public void Add(TKey item) { lock (_syncLock) _hashSet.Add(item); }

    /// <inheritdoc/>
    public void Clear() { lock(_syncLock) _hashSet.Clear(); }

    /// <inheritdoc/>
    public bool Contains(TKey item) { lock (_syncLock) return _hashSet.Contains(item); }

    /// <inheritdoc/>
    public void CopyTo(TKey[] array)
    {
      lock (_syncLock) _hashSet.CopyTo(array);
    }
    /// <inheritdoc/>
    public void CopyTo(TKey[] array, int index, int count)
    {
      lock (_syncLock) _hashSet.CopyTo(array, index, count);
    }
    /// <inheritdoc/>
    public void CopyTo(TKey[] array, int arrayIndex)
    {
      lock (_syncLock) _hashSet.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public IEqualityComparer<HashSet<TKey>> CreateSetComparer() { 
      return HashSet<TKey>.CreateSetComparer(); 
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TKey> other) {
      lock (_syncLock) _hashSet.ExceptWith(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    /// <inheritdoc/>
    public IEnumerator<TKey> GetEnumerator()
    {
        var arr = new TKey[_hashSet.Count];
        CopyTo(arr);
        return ((IEnumerable<TKey>)arr).GetEnumerator();
    }

    /// <inheritdoc/>
    public void GetObjectData(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext  context) {
        lock (_syncLock) _hashSet.GetObjectData(info, context);
    }

    /// <inheritdoc/>
    public void IntersectWith (IEnumerable<TKey> other) {
      lock (_syncLock) _hashSet.IntersectWith(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf (IEnumerable<TKey> other) {
      lock (_syncLock) return _hashSet.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf (IEnumerable<TKey> other) {
      lock (_syncLock) return _hashSet.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf (IEnumerable<TKey> other) {
      lock (_syncLock) return _hashSet.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf (IEnumerable<TKey> other) {
      lock (_syncLock) return _hashSet.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public virtual void OnDeserialization(Object sender) {
      lock (_syncLock) _hashSet.OnDeserialization(sender);
    }

    /// <inheritdoc/>
    public bool Overlaps (IEnumerable<TKey> other) {
      lock (_syncLock) return _hashSet.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Remove (TKey item) {
      lock (_syncLock) return _hashSet.Remove(item);
    }

    /// <inheritdoc/>
    public int RemoveWhere (Predicate<TKey> match) {
      lock (_syncLock) return _hashSet.RemoveWhere(match);
    }

    /// <inheritdoc/>
    public bool SetEquals (IEnumerable<TKey> other) {
      lock (_syncLock) return _hashSet.SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith (IEnumerable<TKey> other) {
      lock (_syncLock) _hashSet.SymmetricExceptWith(other);
    }

    /// <inheritdoc/>
    public void TrimExcess() {
      lock (_syncLock) _hashSet.TrimExcess();
    }

    /// <inheritdoc/>
    public void UnionWith (IEnumerable<TKey> other) {
      lock (_syncLock) _hashSet.UnionWith(other);
    }
  }
}