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
    /// <summary></summary>
    /// <typeparam name="THex"></typeparam>
    public abstract class AbstractModelDisplayPainter<THex>: IMapDisplayPainter where THex:IHex {
        /// <summary></summary>
        /// <param name="model">The map to be painted, as a <see cref="IMapDisplayWinForms{THex}"/>.</param>
        public AbstractModelDisplayPainter(IMapDisplayWinForms<THex> model)
        =>  Model = model??throw new ArgumentNullException(nameof(model));

        protected IMapDisplayWinForms<THex> Model { get; }

        /// <inheritdoc/>
        public virtual void PaintMap(Graphics graphics, bool showHexgrid)
        =>  graphics?.Contain( g => {
                var boardHexes = Model.BoardHexes;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                PaintForEachHex(graphics, coords => {
                    boardHexes[coords].IfHasValueDo(h => {
                        if(h is THex hex) Paint(hex, graphics, Model.HexgridPath, GetHexBrush(hex));
                    });
                    if (showHexgrid) graphics.DrawPath(Pens.Black, Model.HexgridPath);
                } );
            } );

        /// <inheritdoc/>
        public virtual void PaintLabels(Graphics graphics, Func<HexCoords,string> hexText)
        =>  graphics?.Contain( g => {
                var font       = LabelFont;
                var textOffset = new Point((Model.GridSize.Scale(0.50F)
                               - new SizeF(font.Size,font.Size).Scale(0.8F)).ToSize());
                PaintForEachHex(graphics, coords => {
                    graphics.DrawString(hexText(coords), font, TextBrush, textOffset);
                } );
            } );

        /// <inheritdoc/>
        public virtual void PaintHighlight(Graphics graphics) {
            graphics?.Contain(g => {
                g.Transform = Model.TranslateToHex(Model.StartHex);
                g.DrawPath(Pens.Red, Model.HexgridPath);
            } );

            if (Model.Path != null) {
                graphics.Contain(g => { PaintPath(g); });
            }

            if (Model.ShowRangeLine) {
                graphics.Contain(g => {
                    var target = Model.CentreOfHex(Model.HotspotHex);
                    graphics.DrawLine(Pens.Red, Model.CentreOfHex(Model.StartHex), target);
                    graphics.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
                    graphics.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
                } );
            }
        }

        /// <inheritdoc/>
        public virtual void PaintShading(Graphics graphics, IShadingMask isNotShaded)
        =>  graphics?.Contain(g => {
                if (isNotShaded == null) return;
                graphics.CompositingMode = CompositingMode.SourceOver;
                using (var shadeBrush = new SolidBrush(Color.FromArgb(Model.ShadeBrushAlpha,ShadeColor))) {
                    PaintForEachHex(graphics,coords => {
                        if (!isNotShaded[coords]) { graphics.FillPath(shadeBrush,Model.HexgridPath); }
                    });
                }
            } );

        /// <inheritdoc/>
        public virtual void PaintUnits(Graphics graphics) {
            if (graphics == null) throw new ArgumentNullException("graphics");

            /* NO-OP - Not implemented in examples. */
        }

        /// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
        /// for each hex on <paramref name="graphics"/>.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="clipRectangle">The rectangular extent of hexes to be painted as a <see cref="CoordsRectangle"/>.</param>
        /// <param name="paintAction">The paint action to be performed for each hex as a <see cref="Action{HexCoords}"/>.</param>
        public virtual void PaintForEachHex(Graphics graphics, Action<HexCoords> paintAction) {
            var clipRectangle = Model.GetClipInHexes(graphics.VisibleClipBounds);
            ForEachHex(clipRectangle, hex => {
                graphics.Transform = Model.TranslateToHex(hex.Coords);
                paintAction(hex.Coords);
            } );
        }

        /// <summary>Paint the current shortese path.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="maybePath">Type: <see cref="IDirectedPathCollection"/> - 
        /// A directed path (ie linked-list> of hexes to be painted.</param>
        public virtual void PaintPath(Graphics graphics) {
            if (graphics==null) throw new ArgumentNullException("graphics");

            var path = Model.Path.ElseDefault();
            using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
                while (path != null) {
                    var coords = path.PathStep.Coords;
                    graphics.Transform = Model.TranslateToHex(coords);
                    graphics.FillPath(brush, Model.HexgridPath);

                    if (Model.ShowPathArrow) PaintPathDetail(graphics, path);

                    path = path.PathSoFar;
                }
            }
        }

        /// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
        /// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
        protected virtual void PaintPathDetail(Graphics graphics, IDirectedPathCollection path) {
            graphics.TranslateTransform(Model.HexCentreOffset.Width, Model.HexCentreOffset.Height);
            if (path.PathSoFar == null) PaintPathDestination(graphics);
            else                        PaintPathArrow(graphics, path.PathStep.HexsideExit);
        }

        /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="hexside">Type: <see cref="Hexside"/> - 
        /// Direction from this hex in which the next step is made.</param>
        /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
        protected void PaintPathArrow(Graphics graphics, Hexside hexside) {
            var unit = Model.GridSize.Height/8.0F;
            graphics.RotateTransform(60 * hexside);
            graphics.DrawLine(Pens.Black, 0,unit*4,       0,  -unit);
            graphics.DrawLine(Pens.Black, 0,unit*4, -unit*3/2, unit*2);
            graphics.DrawLine(Pens.Black, 0,unit*4,  unit*3/2, unit*2);
        }

        /// <summary>Paint the destination indicator for the current shortest path.</summary>
        /// <param name="this">The map to be painted, as a <see cref="MapDisplay{THex}"/>.</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
        protected void PaintPathDestination(Graphics graphics) {
            var unit = Model.GridSize.Height/8.0F;
            graphics.DrawLine(Pens.Black, -unit*2,-unit*2, unit*2, unit*2);
            graphics.DrawLine(Pens.Black, -unit*2, unit*2, unit*2,-unit*2);
        }

        /// <summary>Performs the specified <see cref="Action{THex}"/> for each hex of <paramref name="this"/> in <paramref name="clipRectangle"/>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this">The map to be painted, as a <see cref="IMapDisplayWinForms{THex}"/>.</param>
        /// <param name="clipRectangle">The rectangular extent of hexes to be painted as a <see cref="CoordsRectangle"/>.</param>
        /// <param name="action">The <see cref="Action{THex}"/> to be performed with each hex.</param>
        protected void ForEachHex(CoordsRectangle clipRectangle, Action<THex> action)
        =>  Model.BoardHexes.ForEachSerial(maybe =>
                maybe.IfHasValueDo(hex => { if (clipRectangle.EncompassesHex(hex.Coords)) action(hex); } )
            );

        /// <summary>TODO</summary>
        /// <param name="this">The <see cref="IHex"/> to be painted</param>
        /// <param name="graphics">The <see cref="Graphics"/> object for the canvas being painted.</param>
        /// <param name="path">The closed <see cref="GraphicsPath"/> outlining the hex to be painted.</param>
        /// <param name="brush">The <see cref="Brush"/> to be used in filling this hex.</param>
        protected void Paint(IHex @this, Graphics graphics, GraphicsPath path, Brush brush)
        =>  graphics.FillPath(brush, path);

        /// <summary>.</summary>
        /// <param name="hex"></param>
        /// <remarks>
        /// Returns clones to avoid inter-thread contention.
        /// </remarks>
        protected abstract Brush GetHexBrush(THex hex);

        /// <summary>Gets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        protected abstract Color ShadeColor { get; }

        /// <summary>Gets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        protected abstract Brush TextBrush  { get; }

        /// <summary>Gets the <see cref="Font"/> to be used by <see cref="PaintLabels(Graphics, Func{HexCoords, string})"/.></summary>
        protected abstract Font  LabelFont  { get; }
    }
}
