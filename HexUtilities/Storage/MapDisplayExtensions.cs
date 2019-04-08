#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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

namespace PGNapoleonics.HexUtilities.Storage {
    using HexPoint = System.Drawing.Point;
    using HexSize  = System.Drawing.Size;
    using Matrix   = System.Drawing.Drawing2D.Matrix;

    /// <summary>Non-painting extension metohds for <see cref="IMapDisplayWinForms{T}"/></summary>
    public static partial class MapDisplayExtensions {
        /// <summary>Returns the translation transform of @this for the upper-left corner of the specified hex.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this"></param>
        /// <param name="coords">Type: HexCoords - Coordinates of the hex to be painted next.</param>
        public static Matrix TranslateToHex<THex>(this IMapDisplayWinForms<THex> @this, HexCoords coords)
        where THex:IHex {
            var offset  = @this.UpperLeftOfHex<THex>(coords);
            return new Matrix(1, 0, 0, 1, offset.X, offset.Y);
        }

        /// <summary>Returns pixel coordinates of upper-left corner of specified hex.</summary>
        /// <param name="this"></param>
        /// <param name="coords"></param>
        /// <returns>A Point structure containing pixel coordinates for the (upper-left corner of the) specified hex.</returns>
        public static HexPoint UpperLeftOfHex<THex>(this IMapDisplayWinForms<THex> @this, HexCoords coords)
        where THex:IHex
        => new HexPoint(
            coords.User.X * @this.GridSize.Width,
            coords.User.Y * @this.GridSize.Height + (coords.User.X + 1) % 2 * @this.GridSize.Height / 2
        );

        /// <summary>Returns pixel coordinates of centre of specified hex.</summary>
        /// <param name="this"></param>
        /// <param name="coords"></param>
        /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
        public static HexPoint CentreOfHex<THex>(this IMapDisplayWinForms<THex> @this, HexCoords coords)
        where THex:IHex
        => @this.UpperLeftOfHex(coords) + @this.HexCentreOffset;

        /// <summary>Rectangular extent in pixels of the defined mapboard.</summary>
        /// <param name="this">The current <see cref="IMapDisplay{THex}"/>.</param>
        public static HexSize MapSizePixels<THex>(this IMapDisplay<THex> @this)
        where THex:IHex
        => @this.MapSizeHexes * @this.GridSizePixels;
    }
}
