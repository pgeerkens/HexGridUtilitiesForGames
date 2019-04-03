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
using System.Drawing;
using System.Drawing.Drawing2D;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridPanel {
    //public class GraphicsMapPainter {
    //    public GraphicsMapPainter() {

    //    }

    //    public void PaintMap<THex>(Graphics graphics, HexgridViewModel dataContext, MapPanel panel)
    //    where THex:IHex {
    //        if (panel.IsTransposed) { graphics.Transform = TransposeMatrix; }

    //        var scroll = dataContext.Grid.GetScrollPosition(panel.AutoScrollPosition);
    //        graphics.TranslateTransform(scroll.X + panel.Margin.Left,  scroll.Y + panel.Margin.Top);
    //        graphics.ScaleTransform(panel.MapScale,panel.MapScale);
    //        Tracing.PaintDetail.Trace($"{panel.Name}.PaintBuffer - VisibleClipBounds: ({graphics.VisibleClipBounds})");

    //        using(var brush = new SolidBrush(panel.BackColor)) {
    //            graphics.FillRectangle(brush, graphics.VisibleClipBounds);
    //        }
    //        graphics.Paint(Point.Empty, 1.0F, g => {
    //            var model = dataContext.Model;
    //            //model.PaintMap(g, true, c => from h in model[c] select h as IHex, model.Landmarks);
    //            model.PaintMap(g, true, model.BoardHexes, model.Landmarks);
    //        });
    //    }

    //    /// <summary>TODO</summary>
    //    static protected Matrix TransposeMatrix => new Matrix(0F,1F, 1F,0F, 0F,0F);
    //}

    //public class GraphicsMapPainter<THex> : IMapPainter<THex> where THex:IHex {
    //    //public void PaintHighlight<Graphics>(Graphics surface, MapDisplay<THex> map)
    //    //=> map.PaintHighlight(surface, map.ShowRangeLine);

    //    /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
    //    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    //    /// <param name="graphics">Graphics object for the canvas being painted.</param>
    //    public void PaintHighlight<Graphics>(Graphics graphics,  IMapDisplay<THex> @this, bool showRangeLine)
    //    {
    //        graphics.Contain(g => {
    //            g.Transform = @this.TranslateToHex(@this.StartHex);
    //            g.DrawPath(Pens.Red, @this.HexgridPath);
    //        });

    //        if (@this.Path != null) {
    //            graphics.Contain(g => { @this.PaintPath(g, @this.Path); });
    //        }

    //        if (showRangeLine) {
    //            graphics.Contain(g => {
    //                var target = @this.CentreOfHex(@this.HotspotHex);
    //                graphics.DrawLine(Pens.Red, @this.CentreOfHex(@this.StartHex), target);
    //                graphics.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
    //                graphics.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
    //            });
    //        }
    //    }

    //    public void PaintMap<Graphics>(Graphics surface, IMapDisplay<THex> map)
    //    => map.PaintMap(surface, map.ShowHexgrid, map.BoardHexes, map.Landmarks);

    //    public void PaintShading<Graphics>(Graphics surface, IMapDisplay<THex> map)
    //    => map.PaintShading(surface, map.Fov, ShadeBrushAlpha, ShadeBrushColor);
    //}
}
