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
      if (Queue.Count>0)    { return Queue.Dequeue(); }
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
