#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.Storage {
    /// <summary>TODO</summary>
    public delegate MapDisplay<THex> MapExtractor<THex>() where THex:class,IHex;

    /// <summary>TODO</summary>
    public class Map<THex> : IEquatable<Map<THex>> where THex: class,IHex {
        /// <summary>TODO</summary>
        public Map(string mapName, MapExtractor<THex> mapSource) {
            MapName   = mapName;
            MapSource = mapSource;
        }

        /// <summary>TODO</summary>
        public  string             MapName   { get; }
        /// <summary>TODO</summary>
        public  MapDisplay<THex>   MapBoard  => MapSource(); 
        
        private MapExtractor<THex> MapSource { get; }

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is Map<THex> other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(Map<THex> other) => MapName == other.MapName;

        /// <inheritdoc/>
        public override int GetHashCode() => MapName.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(Map<THex> lhs, Map<THex> rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(Map<THex> lhs, Map<THex> rhs) => lhs.Equals(rhs);
        #endregion
    }
}
