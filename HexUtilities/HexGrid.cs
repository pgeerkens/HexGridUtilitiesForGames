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
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities {
    using HexPoint  = System.Drawing.Point;
    using HexPoints = IList<System.Drawing.Point>;
    using HexSize   = System.Drawing.Size;

    /// <summary>C# implementation of the hex-picking algorithm noted below.</summary>
    /// <remarks>Mathemagically (left as exercise for the reader) our 'picking' matrices are these, assuming: 
    ///  - origin at upper-left corner of hex (0,0);
    ///  - 'straight' hex-axis vertically down; and
    ///  - 'oblique'  hex-axis up-and-to-right (at 120 degrees from 'straight').</remarks>
    /// <a href="file://Documentation/HexGridAlgorithm.mht">Hex-grid Algorithms</a>
    public class Hexgrid : IHexgrid {
        /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
        public Hexgrid(bool isTransposed, HexSize gridSize, float scale)
        : this(isTransposed, gridSize, scale, HexSize.Empty) { }

        /// <summary>Return a new instance of <c>Hexgrid</c>.</summary>
        public Hexgrid(bool isTransposed, HexSize gridSize, float scale, HexSize margin) {
            GridSize     = gridSize;
            IsTransposed = isTransposed;
            Margin       = margin;
            Scale        = scale;

            HexCorners = new List<HexPoint>() {
                new HexPoint(GridSize.Width*1/3,              0  ), 
                new HexPoint(GridSize.Width*3/3,              0  ),
                new HexPoint(GridSize.Width*4/3,GridSize.Height/2),
                new HexPoint(GridSize.Width*3/3,GridSize.Height  ),
                new HexPoint(GridSize.Width*1/3,GridSize.Height  ),
                new HexPoint(             0,    GridSize.Height/2),
                new HexPoint(GridSize.Width*1/3,              0  )
            }.AsReadOnly();
        }

        /// <inheritdoc/>
        public HexSize   GridSize     { get; }
        /// <inheritdoc/>
        public HexPoints HexCorners   { get; }
        /// <inheritdoc/>
        public bool      IsTransposed { get; }
        /// <inheritdoc/>
        public HexSize   Margin       { get; set; }
        /// <inheritdoc/>
        public float     Scale        { get; }
    }
}
