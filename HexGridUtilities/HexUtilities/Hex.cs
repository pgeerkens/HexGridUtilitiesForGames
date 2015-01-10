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
  [DebuggerDisplay("Coords: {Coords} / ElevLevel: {ElevationLevel}")]
  public abstract class Hex<TDrawingSurface,TPath> : IHex, IEquatable<Hex<TDrawingSurface,TPath>>  {
    /// <summary>Construct a new Hex instance at location <paramref name="coords"/>.</summary>
    protected Hex(HexCoords coords) : this(coords,0) { }
    /// <summary>Construct a new Hex instance at location <paramref name="coords"/>.</summary>
    protected Hex(HexCoords coords, int elevationLevel) {
      Coords         = coords; 
      ElevationLevel = elevationLevel;
    }

    /// <inheritdoc/>
    public          HexCoords Coords         { get; private set; }

    /// <inheritdoc/>
    public          int       ElevationLevel { get; private set; }

    /// <inheritdoc/>
    public virtual  int       HeightObserver { get { return 1; } }

    /// <inheritdoc/>
    public virtual  int       HeightTarget   { get { return 1; } }

    /// <inheritdoc/>
    public abstract int       HeightTerrain  { get; }

    /// <inheritdoc/>
    public abstract int       StepCost(Hexside hexsideExit);

    /// <summary>Default implementation, assuming no blocking hexside terrain.</summary>
    public virtual  int       HeightHexside(Hexside hexside) { return HeightTerrain; }

    /// <summary>TODO</summary>
    public abstract void      Paint(TDrawingSurface graphics);

    #region Value Equality
    /// <inheritdoc/>
    public override bool  Equals(object obj) { return this.Equals(obj as Hex<TDrawingSurface,TPath>); }

    /// <inheritdoc/>
    public override int   GetHashCode()      { return Coords.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(Hex<TDrawingSurface,TPath> other) { return other!=null  &&  Coords.Equals(other.Coords); }
    #endregion
  }

  /// <summary>Extension methods for <see Cref="Hex"/>.</summary>
  public static partial class HexExtensions {
    /// <summary>Returns the requested neighbours for this hex.</summary>
    /// <param name="this">TODO</param>
    /// <param name="here">TODO</param>
    /// <returns></returns>
    public static IEnumerable<NeighbourHex> GetNeighbourHexes(
      this INavigableBoard<IHex> @this, IHex here
    ) {
      if (@this  == null) throw new ArgumentNullException("this");
      return @this.GetAllNeighbours(here).Where(n => n.Hex!=null);
    }
    /// <summary>All neighbours of this hex, as an <see cref="IEnumerable {NeighbourHex}"/></summary>
    public static IEnumerable<NeighbourHex> GetAllNeighbours(
      this INavigableBoard<IHex> @this,  IHex here
    ) {
      if (@this  == null) throw new ArgumentNullException("this");
      return HexsideExtensions.HexsideList.Select(hexside => 
            new NeighbourHex(@this[here.Coords.GetNeighbour(hexside)], hexside.Reversed()));
    }

    /// <summary>The <i>Manhattan</i> distance from this hex to that at <c>coords</c>.</summary>
    public static int Range(this IHex @this, IHex target) { 
      if (@this==null) throw new ArgumentNullException("this");
      if (target==null) throw new ArgumentNullException("target");

      return @this.Coords.Range(target.Coords); 
    }
  }
}
