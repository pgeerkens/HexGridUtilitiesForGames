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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PG_Napoleonics.HexUtilities.Common;
using PG_Napoleonics.HexUtilities.PathFinding;

namespace PG_Napoleonics.HexUtilities {
  public interface IHex : IEnumerable<NeighbourHex>{
    /// <summary>The <c>IBoard<T></c> on which this hex is located.</summary>
    IBoard<IHex>        Board          { get; }

    /// <summary>The <c>ICoords</c> coordinates for this hex on <c>Board</c>.</summary>
    ICoords             Coords         { get; }

    int                 ElevationASL   { get; }
    int                 HeightObserver { get; }
    int                 HeightTarget   { get; }
    int                 HeightTerrain  { get; }
    IList<PathShortcut> Shortcuts      { get; }

    /// <summary>The <i>Manhattan</i> distance from this hex to that at <c>coords</c>.</summary>
    int              Range(ICoords target);

    /// <summary>The cost to enter this hex heading in the direction <c>hexside</c>.</summary>
    int              StepCost(Hexside direction);

    /// <summary>The cost to exit this hex in the direction <c>hexside</c>.</summary>
    int              StepCostFwd(Hexside hexsideExit);

    IEnumerable<NeighbourHex> GetNeighbourHexes();
  }

  public abstract class Hex : IHex, IEquatable<Hex>, IEqualityComparer<Hex> {
    public Hex(IBoard<IHex> board, ICoords coords) { 
      Board    = board;
      Coords   = coords; 
      Shortcuts = new List<PathShortcut>(0);
    }

    ///  <inheritdoc/>
    public virtual  IBoard<IHex> Board          { get; private set; }

    ///  <inheritdoc/>
    public virtual  ICoords      Coords         { get; private set; }

    ///  <inheritdoc/>
    public virtual  int          ElevationASL   { get; protected set; }

    ///  <inheritdoc/>
    public virtual  int          HeightObserver { get { return ElevationASL + 1; } }

    ///  <inheritdoc/>
    public virtual  int          HeightTarget   { get { return ElevationASL + 1; } }

    ///  <inheritdoc/>
    public abstract int          HeightTerrain  { get; }

    ///  <inheritdoc/>
    public IList<PathShortcut>   Shortcuts      { get; private set; }

    ///  <inheritdoc/>
    public          int          Range(ICoords target) { return Coords.Range(target); }

    ///  <inheritdoc/>
    public abstract int          StepCost(Hexside direction);

    ///  <inheritdoc/>
    public virtual  int          StepCostFwd(Hexside hexsideExit) {
      return Board[Coords.StepOut(hexsideExit)].StepCost(hexsideExit);
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetNeighbourHexes().GetEnumerator(); }

    public IEnumerable<NeighbourHex>   GetNeighbourHexes() { return this; }
    IEnumerator<NeighbourHex> IEnumerable<NeighbourHex>.GetEnumerator() {
      foreach (var index in HexExtensions.HexsideList) {
        var hex = Board[Coords.StepOut(index)];
        if (hex != null) yield return new NeighbourHex(hex,index);
      }
    }

    #region Value Equality
    public override bool Equals(object obj)              {
      return obj is Hex  &&  this.Coords.Equals(((Hex)obj).Coords); 
    }
    public override int GetHashCode()                    { return Coords.GetHashCode(); }

    bool IEquatable<Hex>.Equals(Hex rhs)                 { return this.Equals(rhs); }
    bool IEqualityComparer<Hex>.Equals(Hex lhs, Hex rhs) { return lhs.Equals(rhs); }
    int  IEqualityComparer<Hex>.GetHashCode(Hex @this)   { return @this.GetHashCode(); }
    #endregion
  }
}
