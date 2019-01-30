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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridExampleCommon {
    using MapGridHex   = Hex<Graphics,GraphicsPath>;

    /// <summary>Example of <see cref="HexUtilities"/> usage with <see cref="HexgridPanel"/> to implement
    /// a maze map.</summary>
    public sealed class MazeMap : MapDisplayBlocked<MapGridHex> {
        /// <summary>TODO</summary>
        public MazeMap() : base(_sizeHexes, new Size(26,30), InitializeHex) {}

        /// <inheritdoc/>
        public override short?  Heuristic(HexCoords source, HexCoords target) => source.Range(target);

        /// <summary>TODO</summary>
        protected override int  MinimumStepCost => 1;

        /// <inheritdoc/>
        public override int  ElevationBase   =>  0;
        /// <inheritdoc/>
        public override int  ElevationStep   => 10;

        /// <summary>Wrapper for MapDisplayPainter.PaintHighlight.</summary>
        public override void PaintHighlight(Graphics graphics)
        => this.PaintHighlight<MapGridHex>(graphics, ShowRangeLine);
        /// <summary>Wrapper for MapDisplayPainter.PaintMap.</summary>
        public override void PaintMap(Graphics graphics)
        => this.PaintMap<MapGridHex>(graphics,ShowHexgrid, this.Hexes(), Landmarks);
        /// <summary>Wrapper for MapDisplayPainter.PaintShading.</summary>
        public override void PaintShading(Graphics graphics)
        => this.PaintShading<MapGridHex>(graphics, Fov, ShadeBrushAlpha, ShadeBrushColor);
        /// <summary>Wrapper for MapDisplayPainter.PaintUnits.</summary>
        public override void PaintUnits(Graphics graphics) {}

        #region static Board definition
        static IReadOnlyList<string> _board     = MapDefinitions.MazeMapDefinition;
        static Size                  _sizeHexes = new Size(_board[0].Length, _board.Count);
        #endregion

        private new static MapGridHex InitializeHex(HexCoords coords) {
            switch (_board[coords.User.Y][coords.User.X]) {
                case '.': return new PassableTerrainGridHex   (coords,0, 0,1,Brushes.White);     // Path
                default:  return new ImpassableTerrainGridHex (coords,1,10,  Brushes.DarkGray);  // Wall
            }
        }
    }
}
