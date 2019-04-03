#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Globalization;

namespace PGNapoleonics.HexUtilities.Common {
    using HexRectangle = System.Drawing.Rectangle;

    /// <summary>Stores a rectangular board region as a a location and extent of <see cref="HexCoords"/>.</summary>
    [DebuggerDisplay("({Location}):({Size})")]
    public struct CoordsRectangle : IEquatable<CoordsRectangle> {
        /// <summary>Returns a new <see cref="CoordsRectangle"/> from the specified location and dimensions.</summary>
        /// <param name="location">The upper left corner as a <see cref="HexCoords"/> object.</param>
        /// <param name="size">The dimensions as a <see cref="HexCoords"/> object.</param>
        public static CoordsRectangle New(HexCoords location, HexCoords size)
        => new CoordsRectangle(new HexRectangle(location.User, size.User));

        /// <summary>Returns a new <see cref="CoordsRectangle"/> from the specified location and dimensions.</summary>
        /// <param name="x">X-coordinate of the upper left corner.</param>
        /// <param name="y">Y-coordinate of the upper left corner.</param>
        /// <param name="width">X extent of the size.</param>
        /// <param name="height">Y extent of the size.</param>
        public static CoordsRectangle New(int x, int y, int width, int height)
        => new CoordsRectangle(new HexRectangle(x,y,width,height));

        /// <summary>Initializes a new <see cref="CoordsRectangle"/>.</summary>
        private CoordsRectangle(HexRectangle rectangle) : this() => Rectangle = rectangle;

        /// <summary>Returns true exactly when the test hex is inside this rectangle.</summary>
        /// <param name="hexCoords">Location as a <see cref="HexCoords"/> of the hex to be tested.</param>
        public bool EncompassesHex(HexCoords hexCoords)
        =>  Rectangle.Left <= hexCoords.User.X  &&  hexCoords.User.X <= Rectangle.Right
        &&  Rectangle.Top  <= hexCoords.User.Y  &&  hexCoords.User.Y <= Rectangle.Bottom;

        /// <summary>Gets the <see cref="HexCoords"/> of the upper-left corner for this CoordsRectangle</summary>
        public HexCoords Location => HexCoords.NewUserCoords(Rectangle.Location);

        /// <summary>Gets the <see cref="HexCoords"/> of the dimensions for this CoordsRectangle</summary>
        public HexCoords Size => HexCoords.NewUserCoords(Rectangle.Size);

        /// <summary>Gets the underlying hex-coordinates as a <see cref="HexRectangle"/>.</summary>
        private HexRectangle Rectangle  { get; }

        /// <summary>Gets the underlying hex-coordinates as a <see cref="HexRectangle"/>.</summary>
        public static implicit operator HexRectangle(CoordsRectangle rectangle) => rectangle.Rectangle;

        /// <summary>Gets the underlying hex-coordinates as a <see cref="CoordsRectangle"/>.</summary>
        public static implicit operator CoordsRectangle(HexRectangle rectangle) => new CoordsRectangle(rectangle);

        /// <inheritdoc/>
        public override string ToString()
        => string.Format(CultureInfo.CurrentCulture,"({0},{1}):({2},{3})",
                Rectangle.X,Rectangle.Y,Rectangle.Width,Rectangle.Height);

        #region Value Equality
        /// <inheritdoc/>
        public override bool Equals(object obj) { 
            var other = obj as CoordsRectangle?;
            return other.HasValue  &&  this == other.Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Rectangle.GetHashCode();

        /// <inheritdoc/>
        public bool Equals(CoordsRectangle other)=> this == other;

        /// <summary>Tests value inequality of two CoordsRectangle instances.</summary>
        public static bool operator != (CoordsRectangle lhs, CoordsRectangle rhs)
        => ! (lhs == rhs);

        /// <summary>Tests value equality of two CoordsRectangle instances.</summary>
        public static bool operator == (CoordsRectangle lhs, CoordsRectangle rhs)
        => lhs.Rectangle == rhs.Rectangle; 
        #endregion
    }
}
