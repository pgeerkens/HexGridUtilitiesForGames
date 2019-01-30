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
using System.Diagnostics;

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

        /// <inheritdoc/>
        public virtual  int       HeightObserver => 1;

        /// <inheritdoc/>
        public virtual  int       HeightTarget   => 1;

        /// <inheritdoc/>
        public abstract int       HeightTerrain  { get; }

        /// <inheritdoc/>
        public abstract char      TerrainType    { get; }

        /// <inheritdoc/>
        public abstract short?    TryStepCost(Hexside hexsideExit);

        /// <summary>Default implementation, assuming no blocking hexside terrain.</summary>
        public virtual  int       HeightHexside(Hexside hexside) => HeightTerrain;

        #region Value Equality implementation
        /// <inheritdoc/>
        public override bool      Equals(object obj) => this.Equals(obj as Hex);

        /// <inheritdoc/>
        public override int       GetHashCode()      => Coords.GetHashCode();

        /// <inheritdoc/>
        public bool               Equals(Hex other)  => other!=null  &&  Coords.Equals(other.Coords);
        #endregion
    }
}
