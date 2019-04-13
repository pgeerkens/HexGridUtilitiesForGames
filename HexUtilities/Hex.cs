#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;

#pragma warning disable 1587
/// <summary>Display-technology-independent utilities for implementation of hex-grids..</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities {
    /// <summary>Abstract implementation of the interface <see Cref="IHex"/>.</summary>
    [DebuggerDisplay("Coords: {Coords} / ElevLevel: {ElevationLevel}")]
    public abstract class Hex : IHex, IEquatable<Hex>  {
        /// <summary>Construct a new Hex instance at location <paramref name="coords"/>.</summary>
        protected Hex(HexCoords coords) : this(coords,0) { }
        /// <summary>Construct a new Hex instance at location <paramref name="coords"/>.</summary>
        protected Hex(HexCoords coords, int elevationLevel) {
            Coords         = coords; 
            ElevationLevel = elevationLevel;
        }

        /// <inheritdoc/>
        public          HexCoords Coords         { get; }

        /// <inheritdoc/>
        public          int       ElevationLevel { get; }

        /// <summary>Default implementation, assuming no blocking hexside terrain.</summary>
        public virtual  int       HeightHexside(Hexside hexside) => HeightTerrain;

        /// <inheritdoc/>
        public virtual  int       HeightObserver => 1;

        /// <inheritdoc/>
        public virtual  int       HeightTarget   => 1;

        /// <inheritdoc/>
        public abstract int       HeightTerrain  { get; }

        /// <inheritdoc/>
        public abstract bool      IsPassable     { get; }

        /// <inheritdoc/>
        public abstract char      TerrainType    { get; }

        /// <inheritdoc/>
        public abstract int       EntryCost(Hexside hexsideExit);

        /// <inheritdoc/>
        public abstract int       ExitCost(Hexside hexsideExit);

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is Hex other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(Hex other) => Coords == other.Coords;

        /// <inheritdoc/>
        public override int GetHashCode() => Coords.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(Hex lhs, Hex rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(Hex lhs, Hex rhs) => lhs.Equals(rhs);
        #endregion
    }
}
