#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities {
    using HexPoints = IList<System.Drawing.Point>;
    using HexSize   = System.Drawing.Size;

    /// <summary>TODO</summary>
    public interface IHexgrid {
        /// <summary>TODO</summary>
        HexSize   GridSize      { get; }
        /// <summary>TODO</summary>
        HexPoints HexCorners    { get; }
        /// <summary>TODO</summary>
        bool      IsTransposed  { get; }
        /// <summary>Offset of grid origin, from control's client-area origin.</summary>
        HexSize   Margin        { get; set; }
        /// <summary>TODO</summary>
        float     Scale         { get; }
    }
}
