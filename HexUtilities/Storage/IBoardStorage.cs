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
using System;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexSize  = System.Drawing.Size;

    /// <summary>TODO</summary>
    /// <typeparam name="T">The Type being stored.</typeparam>
    public interface IBoardStorage<out T> {
        /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
        HexSize MapSizeHexes     { get; }

        /// <summary>Returns the <c>THex</c> instance at the specified coordinates.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        T this[HexCoords coords] { get; }
        /// <summary>Returns the <c>THex</c> instance at the specified user coordinates.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        T this[IntVector2D userCoords] { get; }

        /// <summary>Perform <paramref name="action"/> for all neighbours of <paramref name="coords"/>.</summary>
        void ForAllNeighbours(HexCoords coords, Action<T,Hexside> action);

        /// <summary>Perform the specified <c>action</c> serially on all hexes.</summary>
        void ForEach(Action<T> action);

        ///// <summary>Returns whether the hex with <see cref="HexCoords"/> <c>coords</c> is 
        ///// within the extent of the board.</summary>
        //bool IsOnboard(HexCoords coords);

        /// <summary>TODO</summary>
        /// <param name="coords"></param>
        /// <param name="hexside"></param>
        /// <returns></returns>
        T Neighbour(HexCoords coords, Hexside hexside);
    }
}
