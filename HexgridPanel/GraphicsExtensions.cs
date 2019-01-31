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
using System.Drawing;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>TODO</summary>
    public static partial class GraphicsExtensions {
        /// <summary>TODO</summary>
        public static void Contain(this Graphics graphics, Action<Graphics> drawingCommands) {
            if (graphics==null) throw new ArgumentNullException("graphics");

            var container = graphics.BeginContainer();
            drawingCommands?.Invoke(graphics);
            graphics.EndContainer(container); 
        }

        /// <summary>Paints, scaled and translated, the supplied <paramref name="action"/> to the
        /// specified <paramref name="graphics"/>.</summary>
        /// <param name="graphics">Target <see cref="Graphics"/> to be rendered to.</param>
        /// <param name="point"><see cref="Point"/> at which to render <paramref name="action"/>.</param>
        /// <param name="scale">Scale at which the source should be drawn.</param>
        /// <param name="action">The drawing action to be performed.</param>
        public static void Paint(this Graphics graphics, Point point, float scale, Action<Graphics> action) {
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (action   == null) throw new ArgumentNullException("action");
    
            graphics.PageUnit = GraphicsUnit.Pixel;
            graphics.TranslateTransform(point.X, point.Y);
            graphics.ScaleTransform(scale,scale);

            action(graphics);
        }
   }
}
