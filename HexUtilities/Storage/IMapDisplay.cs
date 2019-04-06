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
    using HexSize = System.Drawing.Size;
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>(Technology-independent portion of) interface contract required of a map board to be displayed by the Hexgrid control.</summary>
    public interface IMapDisplay<THex> where THex:IHex {
        /// <summary>TODO</summary>
        BoardStorage<Maybe<THex>> BoardHexes { get; }

        /// <summary>TODO</summary>
        int       FovRadius        { get; set; }

        /// <summary>Gets or sets the <see cref="HexCoords"/> of the goal hex for path-fnding.</summary>
        HexCoords GoalHex          { get; set; }

        /// <summary>Gets the extent in pixels of the grid upon whch hexes are to be laid out.</summary>
        /// <remarks>>Width is 3/4 of the point-to-point width of each hex, and Height is the full height.
        /// Hexes should be defined assumed flat-topped and pointy-sided, and the entire board transposed 
        /// if necessary.</remarks>
        HexSize   GridSize         { get; }

        /// <summary>Gets or sets the <see cref="HexCoords"/> of the hex currently under the mouse.</summary>
        HexCoords HotspotHex       { get; set; }

        /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
        bool      IsTransposed     { get; set; }

        /// <summary>Gets or sets the index (-1 for none) of the path-finding <see cref="ILandmark"/> to show.</summary>
        int       LandmarkToShow   { get; set; }

        /// <summary>Current scaling factor for map display.</summary>
        float     MapScale         { get; set; }

        /// <summary>TODO</summary>
        IntMatrix2D GridSizePixels { get; }

        /// <summary>The dimensions of the board as a <see cref="HexSize"/></summary>
        HexSize     MapSizeHexes   { get; }

        /// <summary>Gets the display name for this HexgridPanel host.</summary>
        string    Name             { get; }

        /// <summary>Gets the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
        Maybe<IDirectedPath> Path  { get; }

        /// <summary>Gets or sets the <see cref="HexCoords"/> of the start hex for path-finding.</summary>
        HexCoords StartHex         { get; set; }
    }
}
