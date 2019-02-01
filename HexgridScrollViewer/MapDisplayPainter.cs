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
using System.Windows.Media;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridScrollViewer {
    using ILandmarks = ILandmarkCollection;
    using Hexes      = BoardStorage<Maybe<IHex>>;

    public static partial class MapDisplayPainter {
        /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
        /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
        /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
        /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
        public static void PaintMap<THex>(this MapDisplay<THex> @this, DrawingContext dc,
                bool showHexgrid, Hexes boardHexes, ILandmarks landmarks)
        where THex:IHex {
            ;
        }

        /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
        /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
        /// <param name="graphics">Graphics object for the canvas being painted.</param>
        public static void PaintHighlight<THex>(this MapDisplay<THex> @this, DrawingContext dc,
                bool showRangeLine)
        where THex:IHex {
            ;
        }

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
        /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
        public static void PaintUnits<THex>(this MapDisplay<THex> @this, DrawingContext dc)
        where THex:IHex {
            ;
        }
    }
}
