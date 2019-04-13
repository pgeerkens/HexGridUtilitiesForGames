#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities {
    /// <summary>Structure returned by the Field-of-View factory.</summary>
    public interface IShadingMask {
        /// <summary>True if the hex at location <c>coords</c>c> is visible in this field-of-view.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        bool this[HexCoords coords] { get; }
    }
}
