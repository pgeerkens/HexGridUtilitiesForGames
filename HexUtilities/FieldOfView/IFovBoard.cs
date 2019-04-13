#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    using HexSize = System.Drawing.Size;

    /// <summary>Enumeration of line-of-sight modes</summary>
    public enum FovTargetMode {
        /// <summary>Target height and observer height both set to the same constant value 
        /// (ShadowCasting.DefaultHeight) above ground elevation</summary>
        EqualHeights,
        /// <summary>Use actual observer height and ground level as target height.</summary>
        TargetHeightEqualZero,
        /// <summary>Use actual observer and target height.</summary>
        TargetHeightEqualActual
    }

    /// <summary>Interface required to make use of ShadowCasting Field-of-View calculation.</summary>
    public interface IFovBoard {
        /// <summary>Height in units of elevation level 0 (zero).</summary>
        int     ElevationBase { get; }

        /// <summary>Height increase in units of each elevation level.</summary>
        int     ElevationStep { get; }

        /// <summary>Height in metres.</summary>
        int     HeightOfMan   { get; }

        /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
        HexSize MapSizeHexes  { get; }

        /// <summary>Returns the <see cref="IHex"/> at location <paramref name="coords"/>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        Maybe<IHex> this[HexCoords coords] { get; }
    }
}
