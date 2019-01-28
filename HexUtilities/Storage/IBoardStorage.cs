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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities.Storage {
  using HexPointF    = System.Drawing.PointF;
  using HexPoint     = System.Drawing.Point;
  using HexSizeF     = System.Drawing.SizeF;
  using HexSize      = System.Drawing.Size;
  using HexRectangle = System.Drawing.Rectangle;

  /// <summary>TODO</summary>
  /// <typeparam name="T">The <c>Type</c> being stored.</typeparam>
  [ContractClass(typeof(IBoardStorageContract<>))]
  public interface IBoardStorage<out T> {
    /// <summary>Returns the <c>THex</c> instance at the specified coordinates.</summary>
    [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    T this[HexCoords coords] { get; }

    /// <summary>Perform <paramref name="action"/> for all neighbours of <paramref name="coords"/>.</summary>
    void ForAllNeighbours(HexCoords coords, Action<T,Hexside> action);

    /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
    HexSize MapSizeHexes            { get; }

    /// <summary>TODO</summary>
    /// <param name="coords"></param>
    /// <param name="hexside"></param>
    /// <returns></returns>
    T Neighbour(HexCoords coords, Hexside hexside);
  }

  [ContractClassFor(typeof(IBoardStorage<>))]
  internal abstract class IBoardStorageContract<T> : IBoardStorage<T> {
    private IBoardStorageContract() { }

    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() { }

    public abstract HexSize MapSizeHexes  { get; }
    
    public T this[HexCoords coords]       { get { 
      return default(T); 
    } }

    public void ForAllNeighbours(HexCoords coords, Action<T, Hexside> action) {
    }
    
    public abstract T Neighbour(HexCoords coords, Hexside hexside);

    public abstract void Dispose();
  }
}
