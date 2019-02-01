#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
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
        /// <summary>TODO</summary>
        public static CoordsRectangle New(HexCoords location, HexCoords size)
        => new CoordsRectangle(new HexRectangle(location.User, size.User));

        /// <summary>TODO</summary>
        public static CoordsRectangle New(int x, int y, int width, int height)
        => new CoordsRectangle(new HexRectangle(x,y,width,height));

        /// <summary>TODO</summary>
        private CoordsRectangle(HexRectangle rectangle) : this() => Rectangle = rectangle;

        /// <summary>TODO</summary>
        public int       Bottom     => Rectangle.Bottom;
        /// <summary>TODO</summary>
        public int       Height     => Rectangle.Height;
        /// <summary>TODO</summary>
        public bool      IsEmpty    => Rectangle.IsEmpty;
        /// <summary>TODO</summary>
        public int       Left       => Rectangle.Left;
        /// <summary>TODO</summary>
        public HexCoords Location   => HexCoords.NewUserCoords(Rectangle.Location);
        /// <summary>TODO</summary>
        public int       Right      => Rectangle.Right;
        /// <summary>TODO</summary>
        public HexCoords Size       => HexCoords.NewUserCoords(Rectangle.Size);
        /// <summary>TODO</summary>
        public int       Top        => Rectangle.Top;
        /// <summary>TODO</summary>
        public int       Width      => Rectangle.Width;
        /// <summary>TODO</summary>
        public int       X          => Rectangle.X;
        /// <summary>TODO</summary>
        public int       Y          => Rectangle.Y;

        /// <summary>TODO</summary>
        public HexRectangle Rectangle  { get; private set; }
        /// <summary>TODO</summary>
        public HexCoords    UpperLeft  => HexCoords.NewUserCoords(Left,Top);
        /// <summary>TODO</summary>
        public HexCoords    UpperRight => HexCoords.NewUserCoords(Right,Top);
        /// <summary>TODO</summary>
        public HexCoords    LowerLeft  => HexCoords.NewUserCoords(Left,Bottom);
        /// <summary>TODO</summary>
        public HexCoords    LowerRight => HexCoords.NewUserCoords(Right,Bottom);

        /// <inheritdoc/>
        public override string ToString()
        => string.Format(CultureInfo.CurrentCulture,"({0},{1}):({2},{3})",X,Y,Width,Height);

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
