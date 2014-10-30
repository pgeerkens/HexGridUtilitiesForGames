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
using System.Collections.Generic;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  /// <summary>A queue of FovCOne objects to be processed</summary>
  /// <remarks> This implementation tracks the last added item as Pending, and merges all 
  /// subsequently added items with the same Range and RiseRun values prior to the Pending 
  /// item being Dequeued'</remarks>
  [DebuggerDisplay("Count={Count}")]
  internal class FovConeQueue : Queue<FovCone> {
    internal FovConeQueue() : this(0) {}
    internal FovConeQueue(int capacity) : base(capacity) { Pending = null; }

    FovCone?        Pending;

    /// <summary>Returns the number of elemnts in the queue.</summary>
    public new int Count { get { return base.Count + (Pending.HasValue ? 1 : 0); } }

    /// <inheritdoc/>
    public new FovCone Dequeue() {
      if ( base.Count > 0 ) { return base.Dequeue(); }
      if (Pending.HasValue) { var cache = Pending.Value; Pending = null; return cache; }
      throw new InvalidOperationException("Queue empty.");
    }

    /// <summary>Adds a new item to the queue.</summary>
    /// <remarks>If cone has the same range and RiseRun as Pending, then Pending is extended by 
    /// merging the two.
    /// 
    /// Otherwise Pending is added to the base queue and cone becomes the new pending item.</remarks>
    public new void Enqueue(FovCone cone) {
      if ( ! Pending.HasValue) {
        Pending = cone;
      } else if (Pending.Value.Range == cone.Range && Pending.Value.RiseRun == cone.RiseRun) {
        Pending = new FovCone(cone.Range, Pending.Value.VectorTop, cone.VectorBottom, cone.RiseRun);
      } else {
        base.Enqueue(Pending.Value);
        Pending = cone;
      }
    }

    #region Save - perhaps is referenced by client
    ///// <inheritdoc/>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //public new void Clear() { base.Clear(); Pending = null; }

    ///// <inheritdoc/>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //public new bool Contains(FovCone cone) { 
    //  return base.Contains(cone) || (Pending.HasValue && Pending.Value==cone); 
    //}

    ///// <inheritdoc/>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //public new void CopyTo(FovCone[] array, int index) { 
    //  base.CopyTo(array,index); 
    //  if (Pending.HasValue) array[index + base.Count] = Pending.Value;
    //}

    ///// <inheritdoc/>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //public new IEnumerator<FovCone> GetEnumerator() { 
    //  return ToList().GetEnumerator();
    //}

    ///// <summary>Returns and removes the top item in the queue.</summary>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //public new FovCone Peek() {
    //  if ( base.Count > 0 ) { return base.Peek(); }
    //  if (Pending.HasValue) { return Pending.Value; }
    //  throw new InvalidOperationException("Queue empty.");
    //}

    ///// <inheritdoc/>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //public new FovCone[] ToArray() { return ToList().ToArray(); }

    ///// <summary></summary>
    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //List<FovCone> ToList() {
    //  var list = new List<FovCone>(Count);  
    //  foreach(var item in this) list.Add(item);
    //  if (Pending.HasValue) list.Add(Pending.Value);
    //  return list;
    //}
    #endregion
  }
}
