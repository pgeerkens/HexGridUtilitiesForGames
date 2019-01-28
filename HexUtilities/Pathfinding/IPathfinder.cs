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
using System.Diagnostics;
using System.Linq;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  using IDirectedPath = IDirectedPathCollection;

  /// <summary>TODO</summary>
  [ContractClass(typeof(IPathfinderContract))]
  public interface IPathfinder {
    /// <summary>The <see cref="ISet{HexCoords}"/> of all hexes expanded in finding the shortest-path.</summary>
    ISet<HexCoords> ClosedSet   { get; }
    /// <summary>The target hex for this shortest-path search.</summary>
    HexCoords       Source      { get; }
    /// <summary>The source hex for this shortest-path search.</summary>
    HexCoords       Target      { get; }
    /// <summary>The source hex for this shortest-path search.</summary>
    IDirectedPath   PathForward { get; }
    /// <summary>The source hex for this shortest-path search.</summary>
    IDirectedPath   PathReverse { get; }
  }

  /// <summary>Contract class for IPathfinder{IHex}.</summary>
  [ContractClassFor(typeof(IPathfinder))]
  internal abstract class IPathfinderContract : IPathfinder {
    private IPathfinderContract() {}

    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    //  Contract.Invariant(ClosedSet != null);
    }

    public abstract ISet<HexCoords> ClosedSet   { get; }
    public abstract HexCoords       Source      { get; }
    public abstract HexCoords       Target      { get; }
    public abstract IDirectedPath   PathForward { get; }
    public abstract IDirectedPath   PathReverse { get; }
  }
}
