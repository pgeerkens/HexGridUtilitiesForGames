#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;
using System.Globalization;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>Field-of-View cone for shadow-casting implementation.</summary>
    [DebuggerDisplay("{RiseRun} at {Range}; VTop: {VectorTop}; VBot: {VectorBottom}")]
    public struct FovCone : IEquatable<FovCone> {
        /// <summary>Construct a new FovCone instance.</summary>
        internal FovCone(int range, IntVector2D top, IntVector2D bottom, RiseRun riseRun) : this() {
            Range        = range;
            RiseRun      = riseRun;
            VectorTop    = top;
            VectorBottom = bottom;
        }

        /// <summary>The distance from the observer at which this FovCone was generated.</summary>
        public int          Range        { get; }

        /// <summary>The pitch angle at which visibility begisn for this FovCone. </summary>
        public RiseRun      RiseRun      { get; }

        /// <summary>The maximum yaw angle for this FovCone.</summary>
        public IntVector2D  VectorBottom { get; }

        /// <summary>The minimum yaw angle for this FovCone.</summary>
        public IntVector2D  VectorTop    { get; }

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is FovCone other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(FovCone other)
        => Range        == other.Range      && RiseRun      == other.RiseRun
        && VectorTop    == other.VectorTop  && VectorBottom == other.VectorBottom;

        /// <inheritdoc/>
        public override int GetHashCode()
        =>  VectorTop.GetHashCode() ^ Range.GetHashCode()
          ^ RiseRun.GetHashCode()   ^ VectorBottom.GetHashCode();

        /// <summary>Tests value-inequality of two <c>FovCone</c> instances.</summary>
        public static bool operator !=(FovCone @this,FovCone rhs) => ! @this.Equals(rhs);

        /// <summary>Tests value-equality of two <c>FovCone</c> instances.</summary>
        public static bool operator ==(FovCone @this, FovCone rhs) => @this.Equals(rhs);
        #endregion

        /// <inheritdoc/>
        public override string ToString() => string.Format(CultureInfo.InvariantCulture,
            $"Y={Range}, TopVector={VectorTop}, BottomVector={VectorBottom}, RiseRun={RiseRun}");
    }
}
