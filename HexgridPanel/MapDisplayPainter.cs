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

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>.</summary>
    /// <typeparam name="THex"></typeparam>
    public class MapDisplayPainter<THex>: AbstractModelDisplayPainter<THex> where THex:IHex {
        /// <summary></summary>
        /// <param name="model">The map to be painted, as a <see cref="IMapDisplayWinForms{THex}"/>.</param>
        public MapDisplayPainter(IMapDisplayWinForms<THex> model) : base(model) { }

        /// <summary>Returns a <see cref="Brush"/> suitable for painting the specified <see cref="THex"/>.</summary>
        /// <param name="hex">The <see cref="THex"/> being queried.</param>
        /// <remarks>
        /// Returns clones to avoid inter-thread contention.
        /// </remarks>
        protected override Brush GetHexBrush(THex hex) {
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

        private readonly Brush UndefinedBrush = (Brush)Brushes.SlateGray.Clone();
        private readonly Brush ClearBrush     = (Brush)Brushes.White.Clone();
        private readonly Brush PikeBrush      = (Brush)Brushes.DarkGray.Clone();
        private readonly Brush RoadBrush      = (Brush)Brushes.SandyBrown.Clone();
        private readonly Brush FordBrush      = (Brush)Brushes.Brown.Clone();
        private readonly Brush HillBrush      = (Brush)Brushes.Khaki.Clone();
        private readonly Brush MountainBrush  = (Brush)Brushes.DarkKhaki.Clone();
        private readonly Brush RiverBrush     = (Brush)Brushes.DarkBlue.Clone();
        private readonly Brush WoodsBrush     = (Brush)Brushes.Green.Clone();

        /// <summary>Gets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        protected override Color ShadeColor { get; } = Color.Black;
        /// <summary>Gets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        protected override Brush TextBrush  { get; } = (Brush)Brushes.Black.Clone();
 
        /// <summary>Gets the <see cref="Font"/> to be used by <see cref="PaintLabels(Graphics, Func{HexCoords, string})"/.></summary>
        protected override Font  LabelFont  { get; } = SystemFonts.MenuFont;
   }
}
