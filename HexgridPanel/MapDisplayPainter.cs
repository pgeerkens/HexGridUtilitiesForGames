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

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridPanel {
    using System.Drawing;
    using System.Drawing.Drawing2D;

    using ILandmarks = ILandmarkCollection;
    using Hexes      = BoardStorage<Maybe<IHex>>;

    /// <summary>Extension methods to paint an <see cref="IMapDisplayWinForms{T}"/> from a <see cref="Graphics"/>.</summary>
    public static partial class MapDisplayPainter {
        /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
        public static void PaintMap<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics,
                bool showHexgrid, Hexes boardHexes, ILandmarks landmarks)
        where THex:IHex
        =>  graphics.Contain( g => {
                var clipHexes  = @this.GetClipInHexes(graphics.VisibleClipBounds);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                @this.PaintForEachHex(graphics, clipHexes, coords => {
                    boardHexes[coords].IfHasValueDo(h => {
                        if(h is IHex hex) hex.Paint(graphics, @this.HexgridPath, hex.GetBrush());
                    });
                    if (showHexgrid) graphics.DrawPath(Pens.Black, @this.HexgridPath);
                    if (@this.LandmarkToShow > 0) {
                        var font       = SystemFonts.MenuFont;
                        var textOffset = new Point((@this.GridSize.Scale(0.50F)
                                       - new SizeF(font.Size,font.Size).Scale(0.8F)).ToSize());
                        graphics.DrawString(landmarks.DistanceFrom(coords,@this.LandmarkToShow-1),
                                font, Brushes.Black, textOffset);
                    }
                } );
            } );

        /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintHighlight<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics)
        where THex:IHex {
            graphics.Contain(g => {
                g.Transform = @this.TranslateToHex(@this.StartHex);
                g.DrawPath(Pens.Red, @this.HexgridPath);
            } );

            if (@this.Path != null) {
                graphics.Contain(g => { @this.PaintPath(g, @this.Path); });
            }

            if (@this.ShowRangeLine) {
                graphics.Contain(g => {
                    var target = @this.CentreOfHex(@this.HotspotHex);
                    graphics.DrawLine(Pens.Red, @this.CentreOfHex(@this.StartHex), target);
                    graphics.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
                    graphics.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
                } );
            }
        }

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintShading<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics)
        where THex:IHex
        =>  graphics.Contain(g => {
            var fov = @this?.Fov;
                if (fov != null) {
                    graphics.CompositingMode = CompositingMode.SourceOver;
                    var clipHexes  = @this.GetClipInHexes(graphics.VisibleClipBounds);
                    using(var shadeBrush = new SolidBrush(Color.FromArgb(@this.ShadeBrushAlpha, ShadeColor))) {
                        @this.PaintForEachHex(graphics, clipHexes, coords => {
                            if ( ! fov[coords]) { graphics.FillPath(shadeBrush, @this.HexgridPath); }
                        } );
                    }
                }
            } );

        /// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
        /// for each hex on <paramref name="graphics"/>.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="clipRectangle">The rectangular extent of hexes to be painted as a <see cref="CoordsRectangle"/>.</param>
        /// <param name="paintAction">The paint action to be performed for each hex as a <see cref="Action{HexCoords}"/>.</param>
        public static void PaintForEachHex<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, 
                                    CoordsRectangle clipRectangle, Action<HexCoords> paintAction) 
        where THex:IHex
        =>  @this.ForEachHex(clipRectangle, hex => {
                graphics.Transform = @this.TranslateToHex(hex.Coords);
                paintAction(hex.Coords);
            } );

        /// <summary>Paint the current shortese path.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="maybePath">Type: <see cref="IDirectedPathCollection"/> - 
        /// A directed path (ie linked-list> of hexes to be painted.</param>
        public static void PaintPath<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, 
                                           Maybe<IDirectedPathCollection> maybePath) 
        where THex:IHex {
            if (graphics==null) throw new ArgumentNullException("graphics");

            var path = maybePath.ElseDefault();
            using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
                while (path != null) {
                    var coords = path.PathStep.Coords;
                    graphics.Transform = @this.TranslateToHex(coords);
                    graphics.FillPath(brush, @this.HexgridPath);

                    if (@this.ShowPathArrow) @this.PaintPathArrow(graphics, path);

                    path = path.PathSoFar;
                }
            }
        }

        /// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
        /// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
        static void PaintPathArrow<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, IDirectedPathCollection path)
        where THex:IHex {
            if (graphics==null) throw new ArgumentNullException("graphics");
            if (path==null) throw new ArgumentNullException("path");

            graphics.TranslateTransform(@this.HexCentreOffset.Width, @this.HexCentreOffset.Height);
            if (path.PathSoFar == null) @this.PaintPathDestination(graphics);
            else                        @this.PaintPathArrow(graphics, path.PathStep.HexsideExit);
        }

        /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="hexside">Type: <see cref="Hexside"/> - 
        /// Direction from this hex in which the next step is made.</param>
        /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
        static void PaintPathArrow<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics, Hexside hexside) 
        where THex:IHex {
            if (graphics==null) throw new ArgumentNullException("graphics");

            var unit = @this.GridSize.Height/8.0F;
            graphics.RotateTransform(60 * hexside);
            graphics.DrawLine(Pens.Black, 0,unit*4,       0,  -unit);
            graphics.DrawLine(Pens.Black, 0,unit*4, -unit*3/2, unit*2);
            graphics.DrawLine(Pens.Black, 0,unit*4,  unit*3/2, unit*2);
        }

        /// <summary>Paint the destination indicator for the current shortest path.</summary>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
        static void PaintPathDestination<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics) 
        where THex:IHex {
            if (graphics==null) throw new ArgumentNullException("graphics");

            var unit = @this.GridSize.Height/8.0F;
            graphics.DrawLine(Pens.Black, -unit*2,-unit*2, unit*2, unit*2);
            graphics.DrawLine(Pens.Black, -unit*2, unit*2, unit*2,-unit*2);
        }

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        public static void PaintUnits<THex>(this IMapDisplayWinForms<THex> @this, Graphics graphics)
        where THex:IHex {
            if (@this    == null) throw new ArgumentNullException("this");
            if (graphics == null) throw new ArgumentNullException("graphics");

            /* NO-OP - Not implemented in examples. */
        }

        /// <summary>.</summary>
        /// <param name="hex"></param>
        /// <remarks>
        /// Returns clones to avoid inter-thread contention.
        /// </remarks>
        public static Brush GetBrush(this IHex hex) {
            switch(hex.TerrainType) {
                default:  return UndefinedBrush;
                case '.': return ClearBrush;
                case '2': return PikeBrush;
                case '3': return RoadBrush;
                case 'F': return FordBrush;
                case 'H': return HillBrush;
                case 'M': return MountainBrush;
                case 'R': return RiverBrush;
                case 'W': return WoodsBrush;
            }
        }

        /// <summary>Gets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        private static Color ShadeColor     = Color.Black;
        private static Brush UndefinedBrush = (Brush)Brushes.SlateGray.Clone();
        private static Brush ClearBrush     = (Brush)Brushes.White.Clone();
        private static Brush PikeBrush      = (Brush)Brushes.DarkGray.Clone();
        private static Brush RoadBrush      = (Brush)Brushes.SandyBrown.Clone();
        private static Brush FordBrush      = (Brush)Brushes.Brown.Clone();
        private static Brush HillBrush      = (Brush)Brushes.Khaki.Clone();
        private static Brush MountainBrush  = (Brush)Brushes.DarkKhaki.Clone();
        private static Brush RiverBrush     = (Brush)Brushes.DarkBlue.Clone();
        private static Brush WoodsBrush     = (Brush)Brushes.Green.Clone();

        /// <summary>Performs the specified <see cref="Action{THex}"/> for each hex of <paramref name="this"/> in <paramref name="clipRectangle"/>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="clipRectangle">The rectangular extent of hexes to be painted as a <see cref="CoordsRectangle"/>.</param>
        public static void ForEachHex<THex>(this IMapDisplayWinForms<THex> @this,
                CoordsRectangle clipRectangle, Action<THex> action)
        where THex:IHex
        =>  @this.BoardHexes.ForEachSerial(maybe =>
                maybe.IfHasValueDo(hex => { if (clipRectangle.EncompassesHex(hex.Coords)) action(hex); } )
            );

        /// <summary>TODO</summary>
        public static void Paint(this IHex @this, Graphics graphics, GraphicsPath path, Brush brush) {
            if (graphics==null) throw new ArgumentNullException("graphics");
            lock(brush) graphics.FillPath(brush, path);
        }
    }
}
