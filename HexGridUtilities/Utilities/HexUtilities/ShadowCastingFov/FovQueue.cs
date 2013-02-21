#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using PG_Napoleonics.Utilities;

namespace PG_Napoleonics.Utilities.HexUtilities.ShadowCastingFov {
  internal class FovQueue {
    public FovQueue() : this(0) {}
    public FovQueue(int capacity) {
      Queue            = new Queue<FovCone>(capacity);
      IsCacheOccuppied = false;
    }

    Queue<FovCone> Queue;
    bool           IsCacheOccuppied;
    FovCone        Cache;

    public int Count { get { return Queue.Count + (IsCacheOccuppied ? 1 : 0); } }

    public FovCone Dequeue() {
      if (Queue.Count>0)     { return Queue.Dequeue(); }
      if (IsCacheOccuppied) { IsCacheOccuppied = false; return Cache; }
      throw new InvalidOperationException("Queue empty.");
    }

    public void Enqueue(FovCone cone) {
      if (!IsCacheOccuppied) {
        Cache            = cone;
        IsCacheOccuppied = true;
      } else if (Cache.Range == cone.Range && Cache.RiseRun == cone.RiseRun) {
        Cache = new FovCone(Cache.Range, Cache.VectorTop, cone.VectorBottom, cone.RiseRun);
      } else {
        Queue.Enqueue(Cache);
        Cache = cone;
      }
    }
  }
}
