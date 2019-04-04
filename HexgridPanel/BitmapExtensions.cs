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

namespace PGNapoleonics.HexgridPanel {

    /// <summary>TODO</summary>/>
    public static partial class BitmapExtensions {
        /// <summary>Renders the supplied <see cref="Image"/> <paramref name="source"/> to the specified
        /// <see cref="Image"/> <paramref name="target"/>, translated.</summary>
        /// <param name="target">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="source">Source <see cref="Image"/> to be rendered.</param>
        /// <param name="point"><see cref="Point"/> at which to render the <paramref name="source"/>.</param>
        public static void Render(this Image target, Image source, Point point)
        =>  Render(target, source, point, 1.0F);

        /// <summary>Renders the supplied <see cref="Image"/> <paramref name="source"/> to the specified
        /// <see cref="Image"/> <paramref name="target"/>, scaled and translated.</summary>
        /// <param name="target">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="source">Source <see cref="Image"/> to be rendered.</param>
        /// <param name="point"><see cref="Point"/> at which to render the <paramref name="source"/>.</param>
        /// <param name="scale">Scale at which the source should be drawn</param>
        public static void Render(this Image target, Image source, Point point, float scale) {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");
            Tracing.Paint.Trace($"Render source to {target.Tag}:");

            using (var graphics = Graphics.FromImage(target)) graphics.Render(source, point, scale);
        }

        /// <summary>Renders the supplied <see cref="Image"/> <paramref name="source"/> to the specified
        /// <see cref="Graphics"/> <paramref name="graphics"/>, scaled and translated.</summary>
        /// <param name="graphics">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="source">Source <see cref="Image"/> to be rendered.</param>
        /// <param name="point"><see cref="Point"/> at which to render the <paramref name="source"/>.</param>
        /// <param name="scale">Scale at which the source should be drawn</param>
        public static void Render(this Graphics graphics, Image source, Point point, float scale) {
            if (source == null) throw new ArgumentNullException("source");
            if (graphics == null) throw new ArgumentNullException("graphics");

            graphics.Clear(Color.Black);
            graphics.PageUnit = GraphicsUnit.Pixel;
            graphics.TranslateTransform(point.X, point.Y);
            graphics.ScaleTransform(scale, scale);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(source, Point.Empty);
        }

        /// <summary>Renders, scaled and translated, first the supplied <see cref="Image"/> <paramref name="source"/> to the specified
        /// <see cref="Graphics"/> <paramref name="target"/> and then the specified <paramref name="action"/>.</summary>
        /// <param name="target">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="source">Source <see cref="Image"/> to be rendered.</param>
        /// <param name="point"><see cref="Point"/> at which to render the <paramref name="source"/>.</param>
        /// <param name="scale">Scale at which the source should be drawn</param>
        /// <param name="action">The drawing action to be overlain to target.</param>
        public static void Render(this Image target, Image source, Point point, float scale,
                                Action<Graphics> action) {
            if (target == null) throw new ArgumentNullException("target");
            if (action == null) throw new ArgumentNullException("action");
            Tracing.Paint.Trace($"Render cache to {target.Tag}:");

            using (var graphics = Graphics.FromImage(target)) {
                if (source != null) { graphics.DrawImageUnscaled(source, Point.Empty); }
                graphics.PageUnit = GraphicsUnit.Pixel;
                graphics.TranslateTransform(point.X, point.Y);
                graphics.ScaleTransform(scale,scale);

                action(graphics);
            }
        }

        /// <summary>Paints, scaled and translated, the supplied <see cref="Graphics"/> <paramref name="action"/> to the
        /// specified <see cref="Image"/> <paramref name="target"/>.</summary>
        /// <param name="target">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="point"><see cref="Point"/> at which to render <paramref name="action"/>.</param>
        /// <param name="scale">Scale at which the source should be drawn</param>
        /// <param name="action">The drawing action to be performed.</param>
        public static void Paint(this Image target, Point point, float scale, Action<Graphics> action)
        =>  Paint(target, point, scale, action, Color.Black);

        /// <summary>Paints, scaled and translated, the supplied <see cref="Graphics"/> <paramref name="action"/> to the
        /// specified <see cref="Image"/> <paramref name="target"/>.</summary>
        /// <param name="target">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="point"><see cref="Point"/> at which to render <paramref name="action"/>.</param>
        /// <param name="scale">Scale at which the source should be drawn</param>
        /// <param name="action">The drawing action to be performed.</param>
        /// <param name="background"><seealso cref="Color"/> with which to paint the background.</param>
        public static void Paint(this Image target, Point point, float scale, Action<Graphics> action,
                                Color background) {
            if (target == null) throw new ArgumentNullException("target");
            Tracing.Paint.Trace($"Paint Buffer-{target.Tag}:");

            using (var graphics = Graphics.FromImage(target)) {
                graphics.Clear(background);
                graphics.Paint(point, scale, action);
            }
        }

        /// <summary>Service routine to paint the backing store bitmap for the map underlay.</summary>
        /// <param name="paintAction">The painting <see cref="Action{Graphics}"/> to be performed. </param>
        /// <param name="getBitmap">A <see cref="Func{T}"/> that prouces the <typeparamref name="T"/> drawing target.</param>
        /// <param name="clipBounds"></param>
        public static T ToBitmap<T>(this Action<Graphics> paintAction, Func<T> getBitmap,
                                      Rectangle clipBounds) where T:Image {
            T bitmap = null, temp = null;
            try {
                temp = getBitmap();
                using(var graphics = Graphics.FromImage(temp)) {
                    graphics.Clip = new Region(clipBounds);
                    graphics.Contain(paintAction);
                }

                bitmap = temp;
                temp   = null;
            } finally { if(temp != null) temp.Dispose(); }

            return bitmap;
        }

        /// <summary>Returns a new empty allocated bitmap of the specified size.</summary>
        /// <param name="size">The {Size} of the bitmap to be allocated.</param>
        public static Bitmap AllocateBitmap(this Size size) {
            Bitmap temp = null, buffer = null;
            try {
                temp   = new Bitmap(Math.Max(1,size.Width), Math.Max(1,size.Height));
                buffer = temp;
                temp   = null;
            } finally { if (temp != null) temp.Dispose(); }
            return buffer;
        }
    }
}
