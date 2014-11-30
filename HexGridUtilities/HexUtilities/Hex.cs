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
using System.Linq;

using PGNapoleonics.HexUtilities.Pathfinding;

#pragma warning disable 1587
/// <summary>A library of utilities and controls for buillding strategy simulation games on hexagonal-grid 
/// mapboards.</summary>
/// <remarks>
/// To get started, explore how the two sample maps <see cref="TerrainMap"/> and <see cref="MazeMap"/> override
/// the base class <see cref="MapDisplay<THex>"/>, and how the sample form <see cref="HexgridExampleForm"/>
/// overrides the base control <see cref="HexgridPanel"/>. The Collaboration diagram for MapDisplay<THex>
/// provides a good overview fof the library structure.
/// 
/// Brought to you by <b>PG Software Solutions Inc.</b>, a quality software provider.
/// 
/// Our products are more than <b>Pretty Good Software</b>; ... they're <b>Pretty Great Solutions!</b></remarks>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities {
  using HexSize = System.Drawing.Size;

  /// <summary>Abstract implementation of the interface <see Cref="IHex"/>.</summary>
  [DebuggerDisplay("Coords: {Coords} / ElevASL: {ElevationASL}m")]
  public abstract class Hex<TDrawingSurface,TPath> : IHex, IEquatable<Hex<TDrawingSurface,TPath>>  {
    /// <summary>Construct a new Hex instance at location <paramref name="coords"/>.</summary>
    protected Hex(IHexBoard<IHex> board, HexCoords coords) : this(board,coords,0) { }
    /// <summary>Construct a new Hex instance at location <paramref name="coords"/>.</summary>
    protected Hex(IHexBoard<IHex> board, HexCoords coords, int elevation) {
      Board     = board;
      Coords    = coords; 
      Elevation = elevation;
    }

    /// <inheritdoc/>
    public          IHexBoard<IHex> Board          { get; private set; }

    /// <inheritdoc/>
    public          HexCoords       Coords         { get; private set; }

    /// <inheritdoc/>
    public          int             Elevation      { get; protected set; }

    /// <inheritdoc/>
    public          int             ElevationASL   { get {return Board.ElevationASL(Elevation);} }

    /// <summary>TODO</summary>
    public          HexSize         GridSize       { get { return Board.GridSize; } }

    /// <inheritdoc/>
    public virtual  int             HeightObserver { get { return ElevationASL + 1; } }

    /// <inheritdoc/>
    public virtual  int             HeightTarget   { get { return ElevationASL + 1; } }

    /// <inheritdoc/>
    public abstract int             HeightTerrain  { get; }

    /// <inheritdoc/>
    public abstract int  StepCost(Hexside direction);

    /// <inheritdoc/>
    public virtual  int  GetDirectedCostToExit(Hexside hexsideExit) {
      return Board[Coords.GetNeighbour(hexsideExit)].StepCost(hexsideExit);
    }

    /// <summary>Default implementation, assuming no blocking hexside terrain.</summary>
    public virtual  int  HeightHexside(Hexside hexside) { return HeightTerrain; }

    /// <inheritdoc/>
    public virtual  IHex Neighbour(Hexside hexside) { return Board[Coords.GetNeighbour(hexside)]; }

    /// <summary>TODO</summary>
    public abstract void  Paint(TDrawingSurface graphics);

    /// <summary>TODO</summary>
    public abstract TPath HexgridPath { get; }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) {
      var hex = obj as Hex<TDrawingSurface,TPath>;
      return hex!=null && Coords.Equals(hex.Coords);
    }

    /// <inheritdoc/>
    public override int GetHashCode()       { return Coords.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(Hex<TDrawingSurface,TPath> other)     { return other!=null  &&  this.Coords.Equals(other.Coords); }
    #endregion
  }

  /// <summary>Extension methods for <see Cref="Hex"/>.</summary>
  public static partial class HexExtensions {
    /// <summary>Returns the requested neighbours for this hex.</summary>
    /// <param name="this"></param>
    /// <param name="directions">A HexsideFlags specification of requested neighbours.</param>
    /// <returns></returns>
    public static IEnumerable<NeighbourHex> GetNeighbourHexes(this IHex @this, Hexsides directions) {
      return from n in @this.GetNeighbourHexes()
             where directions.HasFlag(n.HexsideEntry.Direction()) && n.Hex.IsOnboard()
             select n;
    }
    /// <summary>All neighbours of this hex, as an <c>IEnumerable {NeighbourHex}</c></summary>
    public static IEnumerable<NeighbourHex> GetAllNeighbours(this IHex @this) {
      return HexsideExtensions.HexsideList.Select(hexside => 
            new NeighbourHex(@this.Board[@this.Coords.GetNeighbour(hexside)], hexside.Reversed()));
    }
    /// <summary>Perform <c>action</c> for all neighbours of this hex.</summary>
    public static void ForAllNeighbours<THex>(this IHex @this, Func<HexCoords,THex> board, Action<THex,Hexside> action) where THex : IHex {
      if (@this == null) throw new ArgumentNullException("this");
      if (board == null) throw new ArgumentNullException("board");
      if (action == null) throw new ArgumentNullException("action");

      HexsideExtensions.HexsideList.ForEach( hexside =>
        action(board(@this.Coords.GetNeighbour(hexside)), hexside)
      );
    }

    /// <summary>All <i>OnBoard</i> neighbours of this hex, as an <c>IEnumerable {NeighbourHex}</c></summary>
    public static IEnumerable<NeighbourHex> GetNeighbourHexes(this IHex @this) { 
      return @this.GetAllNeighbours().Where(n => n.Hex!=null);
    }

    /// <inheritdoc/>
    public static IEnumerator GetEnumerator(this IHex @this) { 
      return @this.GetNeighbourHexes().GetEnumerator();
    }

    /// <summary>The <i>Manhattan</i> distance from this hex to that at <c>coords</c>.</summary>
    public static int Range(this IHex @this, IHex target) { 
      if (@this==null) throw new ArgumentNullException("this");
      if (target==null) throw new ArgumentNullException("target");

      return @this.Coords.Range(target.Coords); 
    }

    /// <summary>Returns a least-cost path from this hex to the hex <c>goal.</c></summary>
    public static IDirectedPathCollection GetDirectedPath(this IHex @this, IHex goal) {
      if (@this==null) throw new ArgumentNullException("this");
      return @this.Board.GetDirectedPath(@this,goal);
    }

    /// <summary>Returns whether this hex is "On Board".</summary>
    public static bool IsOnboard(this IHex @this) {
      return @this!=null  &&  @this.Board.IsOnboard(@this.Coords);
    }
  }
}
