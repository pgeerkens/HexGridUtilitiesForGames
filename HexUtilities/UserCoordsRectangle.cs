#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;
using System.Globalization;

namespace PGNapoleonics.HexUtilities {
    using HexRectangle = System.Drawing.Rectangle;

    /// <summary>Stores a rectangular board region as a a location and extent of <see cref="HexCoords"/>.</summary>
    [DebuggerDisplay("({Location}):({Size})")]
    public struct CoordsRectangle : IEquatable<CoordsRectangle> {
        /// <summary>Returns a new <see cref="CoordsRectangle"/> from the specified location and dimensions.</summary>
        /// <param name="location">The upper left corner as a <see cref="HexCoords"/> object.</param>
        /// <param name="size">The dimensions as a <see cref="HexCoords"/> object.</param>
        public CoordsRectangle(HexCoords location, HexCoords size)
        : this(new HexRectangle(location.User, size.User)) { }

        /// <summary>Returns a new <see cref="CoordsRectangle"/> from the specified location and dimensions.</summary>
        /// <param name="x">X-coordinate of the upper left corner.</param>
        /// <param name="y">Y-coordinate of the upper left corner.</param>
        /// <param name="width">X extent of the size.</param>
        /// <param name="height">Y extent of the size.</param>
        public CoordsRectangle(int x, int y, int width, int height)
        : this(new HexRectangle(x,y,width,height)) { }

        /// <summary>Initializes a new <see cref="CoordsRectangle"/>.</summary>
        private CoordsRectangle(HexRectangle rectangle) : this() => Rectangle = rectangle;

        /// <summary>Gets the <see cref="HexCoords"/> of the upper-left corner for this CoordsRectangle</summary>
        public HexCoords Location => HexCoords.NewUserCoords(Rectangle.Location);

        /// <summary>Gets the <see cref="HexCoords"/> of the dimensions for this CoordsRectangle</summary>
        public HexCoords Size => HexCoords.NewUserCoords(Rectangle.Size);

        /// <summary>Gets the underlying hex-coordinates as a <see cref="HexRectangle"/>.</summary>
        internal HexRectangle Rectangle { get; }

        /// <summary>Returns true exactly when the test hex is inside this rectangle.</summary>
        /// <param name="hexCoords">Location as a <see cref="HexCoords"/> of the hex to be tested.</param>
        public bool EncompassesHex(HexCoords hexCoords)
        =>  Rectangle.Left <= hexCoords.User.X  &&  hexCoords.User.X <= Rectangle.Right
        &&  Rectangle.Top  <= hexCoords.User.Y  &&  hexCoords.User.Y <= Rectangle.Bottom;

        /// <summary>Gets the underlying hex-coordinates as a <see cref="HexRectangle"/>.</summary>
        public static implicit operator HexRectangle(CoordsRectangle rectangle) => rectangle.Rectangle;

        /// <summary>Gets the underlying hex-coordinates as a <see cref="CoordsRectangle"/>.</summary>
        public static implicit operator CoordsRectangle(HexRectangle rectangle) => new CoordsRectangle(rectangle);

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is CoordsRectangle other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(CoordsRectangle other) => Rectangle == other.Rectangle;

        /// <inheritdoc/>
        public override int GetHashCode() => Rectangle.GetHashCode();

        /// <summary>Tests value inequality of two CoordsRectangle instances.</summary>
        public static bool operator !=(CoordsRectangle lhs, CoordsRectangle rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value equality of two CoordsRectangle instances.</summary>
        public static bool operator ==(CoordsRectangle lhs, CoordsRectangle rhs) => lhs.Equals(rhs);
        #endregion

        /// <inheritdoc/>
        public override string ToString() => string.Format(CultureInfo.CurrentCulture,
                $"({Rectangle.X},{Rectangle.Y}):({Rectangle.Width},{Rectangle.Height})");
    }

    public static partial class Extensions {
        /// <summary>TODO</summary>
        public static int Bottom(this CoordsRectangle @this) => @this.Rectangle.Bottom;
        /// <summary>TODO</summary>
        public static int Height(this CoordsRectangle @this) => @this.Rectangle.Height;
        /// <summary>TODO</summary>
        public static int Left(this CoordsRectangle @this)   => @this.Rectangle.Left;
        /// <summary>TODO</summary>
        public static int Right(this CoordsRectangle @this)  => @this.Rectangle.Right;
        /// <summary>TODO</summary>
        public static int Top(this CoordsRectangle @this)    => @this.Rectangle.Top;
        /// <summary>TODO</summary>
        public static int Width(this CoordsRectangle @this)  => @this.Rectangle.Width;
        /// <summary>TODO</summary>
        public static int X(this CoordsRectangle @this)      => @this.Rectangle.X;
        /// <summary>TODO</summary>
        public static int Y(this CoordsRectangle @this)      => @this.Rectangle.Y;

        /// <summary>TODO</summary>
        public static HexCoords UpperLeft(this CoordsRectangle @this)  => HexCoords.NewUserCoords(@this.Left(),@this.Top());
        /// <summary>TODO</summary>
        public static HexCoords UpperRight(this CoordsRectangle @this) => HexCoords.NewUserCoords(@this.Right(),@this.Top());
        /// <summary>TODO</summary>
        public static HexCoords LowerLeft(this CoordsRectangle @this)  => HexCoords.NewUserCoords(@this.Left(),@this.Bottom());
        /// <summary>TODO</summary>
        public static HexCoords LowerRight(this CoordsRectangle @this) => HexCoords.NewUserCoords(@this.Right(),@this.Bottom());
    }
}
