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
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexUtilities.Storage {
    using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
    using HexPointF    = System.Drawing.PointF;
    using HexSize      = System.Drawing.Size;
    using HexSizeF     = System.Drawing.SizeF;
    using ILandmarks   = ILandmarkCollection;
    using RectangleF   = System.Drawing.RectangleF;

    /// <summary>(Technology-dependent portion of) interface contract required of a map board to be displayed by the Hexgrid control.</summary>
    public interface IMapDisplayWinForms<THex> : IMapDisplay<THex> where THex:IHex {
        /// <summary>Gets or sets the Field-of-View for the current HotspotHex, as an <see cref="IShadingMask"/> object.</summary>
        IShadingMask Fov                    { get; }
        /// <summary>.</summary>/>
        GraphicsPath HexgridPath            { get; }
        /// <summary>Offset of hex centre from upper-left corner, as a <see cref="HexSize"/> struct.</summary>
        HexSize      HexCentreOffset        { get; }
        /// <summary>.</summary>
        ILandmarks   Landmarks              { get;}
        /// <summary>Gets or sets the alpha component for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        byte         ShadeBrushAlpha        { get; }
        /// <summary>Gets or sets whether to display direction indicators for the current path.</summary>
        bool         ShowPathArrow          { get; }
        /// <summary>Gets or sets whether to display a range line from the selected hex to the hover hex.</summary>
        bool         ShowRangeLine          { get; }

        /// <summary>Returns the <see cref="Maybe{IHex}"/> at location <c>coords</c>.</summary>
        Maybe<THex>  this[HexCoords coords] { get; }

        /// <summary>Gets the CoordsRectangle description of the clipping region.</summary>
        /// <param name="point">Upper-left corner in pixels of the clipping region.</param>
        /// <param name="size">Width and height of the clipping region in pixels.</param>
        CoordsRectangle GetClipInHexes(HexPointF point, HexSizeF size);

        /// <summary>Gets the CoordsRectangle description of the clipping region.</summary>
        /// <param name="visibleClipBounds">Rectangular extent in pixels of the clipping region.</param>
        CoordsRectangle GetClipInHexes(RectangleF visibleClipBounds);
    }
}
