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

namespace PGNapoleonics.HexUtilities {
    using HexSize = System.Drawing.Size;

    /// <summary>TODO</summary>
    public static partial class Extensions {
        /// <summary><b>True</b>b> <i>modulo</i> operation returning <paramref name="dividend"/> mod <paramref name="divisor"/>.</summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int Modulo(this int dividend, int divisor) {
            while(dividend <       0) dividend += divisor;
            while(dividend > divisor) dividend -= divisor;
            return dividend;
        }

        /// <summary>Returns whether the specified hex coordinates are "on" a board of the given dimensions.</summary>
        /// <param name="mapSizeHexes">The hex dimensions of the board.</param>
        /// <param name="hexCoords">The hex coordinates of the hex of interest.</param>
        public static bool IsOnboard(this HexSize mapSizeHexes, HexCoords hexCoords) =>
            mapSizeHexes.IsOnboard(hexCoords.User);

        /// <summary>Returns whether the specified hex coordinates are "on" a board of the given dimensions.</summary>
        /// <param name="mapSizeHexes">The hex dimensions of the board.</param>
        /// <param name="userCoords">The User coordinates of the hex of interest.</param>
        public static bool IsOnboard(this HexSize mapSizeHexes, IntVector2D userCoords) =>
            mapSizeHexes.IsOnboard(userCoords.X,userCoords.Y);

        /// <summary>Returns whether the specified hex coordinates are "on" a board of the given dimensions.</summary>
        /// <param name="mapSizeHexes">The hex dimensions of the board.</param>
        /// <param name="x">The horizontal hex index of the hex of interest.</param>
        /// <param name="y">The vertical hex index of the hex of interest.</param>
        public static bool IsOnboard(this HexSize mapSizeHexes, int x, int y) =>
            x.InRange(0, mapSizeHexes.Width)  &&  y.InRange(0, mapSizeHexes.Height);

        /// <summary>Create and initialize a non-local <see cref="IDisposable"/> instance safely.</summary>
        /// <typeparam name="T">The <see cref="IDisposable"/> type being initialized.</typeparam>
        /// <param name="initializer">The <i>functor</i> creating and initializing the instance.</param>
        /// <returns>Returns a safely initialized <see cref="IDisposable"/> instance</returns>
        /// <remarks>
        /// The returned <see cref="IDisposable"/> instance has been constructed and initialized in a manner
        /// that does not raise the warning: <b>CA2000:</b> <i>Dispose objects before losing scope</i>.
        /// </remarks>
        public static T InitializeDisposable<T>(this Func<T> initializer) where T : class, IDisposable {
            T item     = null;
            T tempItem = null;
            try {
                tempItem = initializer();
                item     = tempItem;
                tempItem = null;
              } finally { if(tempItem!=null) tempItem.Dispose(); }
              return item;
        }

        /// <summary>Returns true exactly if lower &lt;= value &lt; lower+height</summary>
        /// <param name="value">Vlaue being tested.</param>
        /// <param name="lower">Inclusive lower bound for the range.</param>
        /// <param name="height">Height of the range.</param>
        public static bool InRange(this int value, int lower, int height) =>
            lower <= value && value < lower+height;

        /// <summary>The <i>Manhattan</i> distance from this hex to that at <c>coords</c>.</summary>
        public static int Range(this IHex @this, IHex target)
        => @this.Coords.Range(target.Coords);
    }
}

