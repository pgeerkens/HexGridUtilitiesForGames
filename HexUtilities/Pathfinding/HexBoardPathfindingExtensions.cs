#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Extensions for the HexBoard class.</summary>
    public static partial class HexBoardPathfindingExtensions {
        /// <summary>Asynchronously returns a least-cost path from the hex <c>source</c> to the hex <c>target.</c></summary>
        ///<remarks>Invokes <see cref="GetDirectedPath"/> asynchronously.</remarks>
        public static async Task<Maybe<IDirectedPathCollection>> GetDirectedPathAsync(this HexCoords source, 
            HexCoords target, Func<HexCoords, HexCoords, Pathfinder> pathfinder
        ) {

            return await Task.Run(() => source.GetDirectedPath(target, pathfinder));
        }
        /// <summary>Returns a least-cost path from the hex <c>source</c> to the hex <c>target.</c></summary>
        public static Maybe<IDirectedPathCollection> GetDirectedPath(this HexCoords source, 
            HexCoords target, Func<HexCoords, HexCoords, Pathfinder> pathfinderFunc
        ) {

            var pathfinder = pathfinderFunc(source,target);
            return pathfinder.PathForward.ToMaybe();
        }

        /// <summary>TODO</summary>
        /// <param name="this"></param>
        public static Func<HexCoords,HexCoords,Pathfinder> GetStandardPathfinder(this INavigableBoard @this) =>
            (source,target) => _getStandardPathfinder(@this,source,target);

        /// <summary>TODO</summary>
        /// <param name="this"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static Pathfinder _getStandardPathfinder(this INavigableBoard @this, HexCoords source, HexCoords target) {
            return StandardPathfinder.New(@this, source, target);
        }

        /// <summary>TODO</summary>
        /// <param name="this"></param>
        public static Func<HexCoords,HexCoords,Pathfinder> GetLandmarkPathfinder<TBoard>(this TBoard @this)
        where TBoard : class, INavigableBoard, ILandmarkBoard =>
            (source,target) => _getLandmarkPathfinder(@this,source,target);

        /// <summary>TODO</summary>
        /// <param name="this"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static Pathfinder _getLandmarkPathfinder<TBoard>(TBoard @this, HexCoords source, HexCoords target)
        where TBoard : class, INavigableBoard, ILandmarkBoard {
            return LandmarkPathfinder.New(@this, @this.Landmarks, source, target);
        }
    }
}
