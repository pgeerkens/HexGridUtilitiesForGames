#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities {
    using HexSize = System.Drawing.Size;

    /// <summary>The basic interface defining a mapboard with hexes.</summary>
    public interface IBoard<THex> where THex: IHex {
        /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
        HexSize     MapSizeHexes           { get; }

        /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        Maybe<THex> this[HexCoords coords] { get; }

        /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
        bool IsOnboard(HexCoords coords);

        /// <summary>Perform <paramref name="action"/> for all neighbours of this hex.</summary>
        void ForAllNeighbours(HexCoords coords, Action<THex,Hexside> action);
    }
}
