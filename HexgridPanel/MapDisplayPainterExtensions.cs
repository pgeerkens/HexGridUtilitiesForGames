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
        //=>  graphics.Contain( g => {
        //        var boardHexes    = @this.BoardHexes;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        @this.PaintForEachHex(graphics, coords => {
        //            boardHexes[coords].IfHasValueDo(h => {
        //                if(h is IHex hex) hex.Paint(graphics, @this.HexgridPath, hex.GetHexBrush());
        //            });
        //            if (showHexgrid) graphics.DrawPath(Pens.Black, @this.HexgridPath);
        //        } );
        //    } );

        /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="hexText">.</param>
        /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
        public static void PaintLabels<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics,
                Func<HexCoords,string> hexText)
        where THex:IHex
        => @this.Painter().PaintLabels(graphics,hexText);
        //=>  graphics.Contain( g => {
        //        @this.PaintForEachHex(graphics, coords => {
        //            var font       = SystemFonts.MenuFont;
        //            var textOffset = new Point((@this.GridSize.Scale(0.50F)
        //                           - new SizeF(font.Size,font.Size).Scale(0.8F)).ToSize());
        //            graphics.DrawString(hexText(coords), font, TextBrush, textOffset);
        //        } );
        //    } );

        /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintHighlight<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics)
        where THex:IHex
        => @this.Painter().PaintHighlight(graphics);
        //{
        //    graphics.Contain(g => {
        //        g.Transform = @this.TranslateToHex(@this.StartHex);
        //        g.DrawPath(Pens.Red, @this.HexgridPath);
        //    } );

        //    if (@this.Path != null) {
        //        graphics.Contain(g => { @this.PaintPath(g); });
        //    }

        //    if (@this.ShowRangeLine) {
        //        graphics.Contain(g => {
        //            var target = @this.CentreOfHex(@this.HotspotHex);
        //            graphics.DrawLine(Pens.Red, @this.CentreOfHex(@this.StartHex), target);
        //            graphics.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
        //            graphics.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
        //        } );
        //    }
        //}

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="isNotShaded">The <see cref="IShadingMask"/> object for the canvas being painted.</param>
        public static void PaintShading<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, IShadingMask isNotShaded)
        where THex: IHex
        => @this.Painter().PaintShading(graphics,isNotShaded);
        //=>  graphics.Contain(g => {
        //        if (isNotShaded == null) return;
        //        graphics.CompositingMode = CompositingMode.SourceOver;
        //        using (var shadeBrush = new SolidBrush(Color.FromArgb(@this.ShadeBrushAlpha,ShadeColor))) {
        //            @this.PaintForEachHex(graphics,coords => {
        //                if (!isNotShaded[coords]) { graphics.FillPath(shadeBrush,@this.HexgridPath); }
        //            });
        //        }
        //});

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintUnits<THex>(this IMapDisplayWinForms<THex> @this,Graphics graphics)
        where THex : IHex
        => @this.Painter().PaintUnits(graphics);
        //{
        //    if (@this    == null) throw new ArgumentNullException("this");
        //    if (graphics == null) throw new ArgumentNullException("graphics");

        //    /* NO-OP - Not implemented in examples. */
        //}

        ///// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
        ///// for each hex on <paramref name="graphics"/>.</summary>
        ///// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        ///// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        ///// <param name="clipRectangle">The rectangular extent of hexes to be painted as a <see cref="CoordsRectangle"/>.</param>
        ///// <param name="paintAction">The paint action to be performed for each hex as a <see cref="Action{HexCoords}"/>.</param>
        //private static void PaintForEachHex<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, 
        //                            Action<HexCoords> paintAction) 
        //where THex:IHex {
        //    var clipRectangle = @this.GetClipInHexes(graphics.VisibleClipBounds);
        //    @this.ForEachHex(clipRectangle, hex => {
        //        graphics.Transform = @this.TranslateToHex(hex.Coords);
        //        paintAction(hex.Coords);
        //    } );
        //}

        ///// <summary>Paint the current shortese path.</summary>
        ///// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        ///// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        ///// <param name="maybePath">Type: <see cref="IDirectedPathCollection"/> - 
        ///// A directed path (ie linked-list> of hexes to be painted.</param>
        //private static void PaintPath<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics) 
        //where THex:IHex {
        //    if (graphics==null) throw new ArgumentNullException("graphics");

        //    var path = @this.Path.ElseDefault();
        //    using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        //        while (path != null) {
        //            var coords = path.PathStep.Coords;
        //            graphics.Transform = @this.TranslateToHex(coords);
        //            graphics.FillPath(brush, @this.HexgridPath);

        //            if (@this.ShowPathArrow) @this.PaintPathDetail(graphics, path);

        //            path = path.PathSoFar;
        //        }
        //    }
        //}

        ///// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
        ///// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        ///// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        ///// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
        ///// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
        //static void PaintPathDetail<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, IDirectedPathCollection path)
        //where THex:IHex {
        //    graphics.TranslateTransform(@this.HexCentreOffset.Width, @this.HexCentreOffset.Height);
        //    if (path.PathSoFar == null) @this.PaintPathDestination(graphics);
        //    else                        @this.PaintPathArrow(graphics, path.PathStep.HexsideExit);
        //}

        ///// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
        ///// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        ///// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        ///// <param name="hexside">Type: <see cref="Hexside"/> - 
        ///// Direction from this hex in which the next step is made.</param>
        ///// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
        //static void PaintPathArrow<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, Hexside hexside) 
        //where THex:IHex {
        //    var unit = @this.GridSize.Height/8.0F;
        //    graphics.RotateTransform(60 * hexside);
        //    graphics.DrawLine(Pens.Black, 0,unit*4,       0,  -unit);
        //    graphics.DrawLine(Pens.Black, 0,unit*4, -unit*3/2, unit*2);
        //    graphics.DrawLine(Pens.Black, 0,unit*4,  unit*3/2, unit*2);
        //}

        ///// <summary>Paint the destination indicator for the current shortest path.</summary>
        ///// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        ///// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        ///// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
        //static void PaintPathDestination<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics) 
        //where THex:IHex {
        //    var unit = @this.GridSize.Height/8.0F;
        //    graphics.DrawLine(Pens.Black, -unit*2,-unit*2, unit*2, unit*2);
        //    graphics.DrawLine(Pens.Black, -unit*2, unit*2, unit*2,-unit*2);
        //}

        ///// <summary>.</summary>
        ///// <param name="hex"></param>
        ///// <remarks>
        ///// Returns clones to avoid inter-thread contention.
        ///// </remarks>
        //private static Brush GetHexBrush(this IHex hex) {
        //    switch(hex.TerrainType) {
        //        default:  return UndefinedBrush;
        //        case '.': return ClearBrush;
        //        case '2': return PikeBrush;
        //        case '3': return RoadBrush;
        //        case 'F': return FordBrush;
        //        case 'H': return HillBrush;
        //        case 'M': return MountainBrush;
        //        case 'R': return RiverBrush;
        //        case 'W': return WoodsBrush;
        //    }
        //}

        ///// <summary>Gets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        //private static Color ShadeColor     = Color.Black;
        //private static Brush TextBrush      = (Brush)Brushes.Black.Clone();

        //private static Brush UndefinedBrush = (Brush)Brushes.SlateGray.Clone();
        //private static Brush ClearBrush     = (Brush)Brushes.White.Clone();
        //private static Brush PikeBrush      = (Brush)Brushes.DarkGray.Clone();
        //private static Brush RoadBrush      = (Brush)Brushes.SandyBrown.Clone();
        //private static Brush FordBrush      = (Brush)Brushes.Brown.Clone();
        //private static Brush HillBrush      = (Brush)Brushes.Khaki.Clone();
        //private static Brush MountainBrush  = (Brush)Brushes.DarkKhaki.Clone();
        //private static Brush RiverBrush     = (Brush)Brushes.DarkBlue.Clone();
        //private static Brush WoodsBrush     = (Brush)Brushes.Green.Clone();

        ///// <summary>Performs the specified <see cref="Action{THex}"/> for each hex of <paramref name="this"/> in <paramref name="clipRectangle"/>.</summary>
        ///// <typeparam name="THex"></typeparam>
        ///// <param name="this">The map to be painted, as a <see cref="IMapDisplayWinForms{THex}"/>.</param>
        ///// <param name="clipRectangle">The rectangular extent of hexes to be painted as a <see cref="CoordsRectangle"/>.</param>
        ///// <param name="action">The <see cref="Action{THex}"/> to be performed with each hex.</param>
        //private static void ForEachHex<THex>(this IMapDisplayWinForms<THex> @this, CoordsRectangle clipRectangle,
        //        Action<THex> action)
        //where THex:IHex
        //=>  @this.BoardHexes.ForEachSerial(maybe =>
        //        maybe.IfHasValueDo(hex => { if (clipRectangle.EncompassesHex(hex.Coords)) action(hex); } )
        //    );

        ///// <summary>TODO</summary>
        ///// <param name="this">The <see cref="IHex"/> to be painted</param>
        ///// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        ///// <param name="path">The closed <see cref="GraphicsPath"/> outlining the hex to be painted.</param>
        ///// <param name="brush">The <see cref="Brush"/> to be used in filling this hex.</param>
        //private static void Paint(this IHex @this, Graphics graphics, GraphicsPath path, Brush brush) {
        //    lock(brush) graphics.FillPath(brush, path);
        //}
    }
}
