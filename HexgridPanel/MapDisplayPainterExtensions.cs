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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>Extension methods to paint an <see cref="IMapDisplayWinForms{T}"/> from a <see cref="Graphics"/>.</summary>
    public static partial class MapDisplayPainterExtensions {
        private static IMapDisplayPainter Painter<THex>(this IMapDisplayWinForms<THex> @this) where THex:IHex
        => new MapDisplayPainter<THex>(@this);

        /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="showHexgrid">.</param>
        /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
        public static void PaintMap<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics,
                bool showHexgrid)
        where THex:IHex
        => @this.Painter().PaintMap(graphics,showHexgrid);

        /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="hexText">.</param>
        /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
        public static void PaintLabels<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics,
                Func<HexCoords,string> hexText)
        where THex:IHex
        => @this.Painter().PaintLabels(graphics,hexText);

        /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintHighlight<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics)
        where THex:IHex
        => @this.Painter().PaintHighlight(graphics);

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="isNotShaded">The <see cref="IShadingMask"/> object for the canvas being painted.</param>
        public static void PaintShading<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, IShadingMask isNotShaded)
        where THex: IHex
        => @this.Painter().PaintShading(graphics,isNotShaded);

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintUnits<THex>(this IMapDisplayWinForms<THex> @this,Graphics graphics)
        where THex : IHex
        => @this.Painter().PaintUnits(graphics);
    }
}
