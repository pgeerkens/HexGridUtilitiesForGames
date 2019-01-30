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
using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;

using HexSize       = System.Drawing.Size;
using Graphics      = System.Drawing.Graphics;
using Brushes       = System.Drawing.Brushes;
using GraphicsPath  = System.Drawing.Drawing2D.GraphicsPath;

#pragma warning disable 1587
/// <summary>TODO</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridExampleCommon {
    using MapGridHex    = Hex<Graphics,GraphicsPath>;

    /// <summary>Example of <see cref="HexUtilities"/> usage with <see cref="HexgridPanel"/> to implement
    /// a terrain map.</summary>
    public sealed class TerrainMap : MapDisplayBlocked<MapGridHex> {
        /// <summary>TODO</summary>
        public TerrainMap() : base(_sizeHexes, new HexSize(26,30), InitializeHex) {}

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow",
                MessageId = "2*range", Justification="No map is big enough to overflow,")]
        public    override short?   Heuristic(HexCoords source, HexCoords target) => (short)(2 * source.Range(target));

        /// <inheritdoc/>
        public override int      ElevationBase     { get {return  0;} }
        /// <inheritdoc/>
        public override int      ElevationStep     { get {return 10;} }

        /// <summary>Wrapper for MapDisplayPainter.PaintHighlight.</summary>
        public override void PaintHighlight(Graphics graphics)
        => this.PaintHighlight<MapGridHex>(graphics, ShowRangeLine);
        /// <summary>Wrapper for MapDisplayPainter.PaintMap.</summary>
        public override void PaintMap(Graphics graphics)
        => this.PaintMap<MapGridHex>(graphics, ShowHexgrid, this.Hexes(), Landmarks);

        /// <summary>Wrapper for MapDisplayPainter.PaintShading.</summary>
        public override void PaintShading(Graphics graphics)
        => this.PaintShading<MapGridHex>(graphics, Fov, ShadeBrushAlpha, ShadeBrushColor);

        /// <summary>Wrapper for MapDisplayPainter.PaintUnits.</summary>
        public override void PaintUnits(Graphics graphics) {}

        #region static Board definition
        static IReadOnlyList<string> _board     = MapDefinitions.TerrainMapDefinition;
        static HexSize               _sizeHexes = new HexSize(_board[0].Length, _board.Count);
        #endregion

        public new static MapGridHex InitializeHex(HexCoords coords) {
            char value = _board[coords.User.Y][coords.User.X];
            switch(value) {
            default:
            case '.': return new PassableTerrainGridHex   (coords, 0,0, 4,Brushes.White);      // Clear
            case '2': return new PassableTerrainGridHex   (coords, 0,0, 2,Brushes.DarkGray);   // Pike
            case '3': return new PassableTerrainGridHex   (coords, 0,0, 3,Brushes.SandyBrown); // Road
            case 'F': return new PassableTerrainGridHex   (coords, 0,0, 5,Brushes.Brown);      // Ford
            case 'H': return new PassableTerrainGridHex   (coords, 1,0, 5,Brushes.Khaki);      // Hill
            case 'M': return new PassableTerrainGridHex   (coords, 2,0, 6,Brushes.DarkKhaki);  // Mountain
            case 'R': return new ImpassableTerrainGridHex (coords, 0,0,   Brushes.DarkBlue);   // River
            case 'W': return new PassableTerrainGridHex   (coords, 0,7, 8,Brushes.Green);      // Woods
            }
        }
    }
}
