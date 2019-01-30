#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using PGNapoleonics.HexUtilities;

using Graphics     = System.Drawing.Graphics;
using HexSize      = System.Drawing.Size;
using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;

namespace PGNapoleonics.HexgridPanel {
    using MapGridHex = Hex<Graphics,GraphicsPath>;

    /// <summary>TODO</summary>
    public sealed class EmptyBoard : MapDisplayBlocked<MapGridHex> {
        public static EmptyBoard TheOne { get; } = new EmptyBoard();

        /// <summary>TODO</summary>
        private EmptyBoard()
        : base(new HexSize(1,1), new HexSize(26,30), c => new EmptyGridHex(c)) => FovRadius = 20;
        /// <inheritdoc/>
        public override int      ElevationBase     => 0;
        /// <inheritdoc/>
        public override int      ElevationStep     => 10;

        /// <summary>Wrapper for MapDisplayPainter.PaintHighlight.</summary>
        public override void PaintHighlight(Graphics graphics) {}

        /// <summary>Wrapper for MapDisplayPainter.PaintMap.</summary>
        public override void PaintMap(Graphics graphics)
        => this.PaintMap<MapGridHex>(graphics, ShowHexgrid, this.Hexes(), Landmarks);

        /// <summary>Wrapper for MapDisplayPainter.PaintShading.</summary>
        public override void PaintShading(Graphics graphics) {}

        /// <summary>Wrapper for MapDisplayPainter.PaintUnits.</summary>
        public override void PaintUnits(Graphics graphics) {}

        /// <inheritdoc/>
        public    override short?   Heuristic(HexCoords source, HexCoords target) => source.Range(target);
    }

    /// <summary>TODO</summary>
    public sealed class EmptyGridHex : MapGridHex, IHex {
        /// <summary>TODO</summary>
        public EmptyGridHex(HexCoords coords) : base(coords,0) {}

        /// <summary>TODO</summary>
        public override int         HeightTerrain => 0;
        /// <summary>TODO</summary>
        public override short?      TryStepCost(Hexside hexsideExit) => default(short?);
        ///  <inheritdoc/>
        public override void        Paint(Graphics graphics, GraphicsPath graphicsPath) { ; }
    }
}
