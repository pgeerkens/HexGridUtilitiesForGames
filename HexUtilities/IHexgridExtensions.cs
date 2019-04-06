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

namespace PGNapoleonics.HexUtilities {
    using HexPoint = System.Drawing.Point;
    using HexSize  = System.Drawing.Size;

    /// <summary>.</summary>
    public static partial class IHexgridExtensions {
        /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
        /// <param name="this"></param>
        /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
        /// <param name="visibleRectangle"></param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        public static HexPoint ScrollPositionToCenterOnHex(this IHexgrid @this,
                HexCoords coordsNewCenterHex, CoordsRectangle visibleRectangle)
        => @this.HexCenterPoint(HexCoords.NewUserCoords(coordsNewCenterHex.User - (visibleRectangle.Size.User / 2)) );

        /// <summary>TODO</summary>
        /// <param name="this"></param>
        /// <param name="mapSizePixels"></param>
        /// <param name="mapScale"></param>
        public static HexSize GetSize(this IHexgrid @this, HexSize mapSizePixels, float mapScale)
        => HexSize.Ceiling(mapSizePixels.Scale(mapScale)); 
    }
}
