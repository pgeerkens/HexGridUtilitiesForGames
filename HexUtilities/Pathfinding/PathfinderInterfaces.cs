#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Collections;
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using HexSize       = System.Drawing.Size;
    using IDirectedPath = IDirectedPathCollection;
    using ILandmarks    = ILandmarkCollection;
 
    /// <summary>TODO</summary>
    public interface IPathfinder {
        /// <summary>Returns an <c>IDirectedPath</c> for the optimal path from 
        /// <paramref name="source"/> to <paramref name="target"/>.</summary>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        /// <returns>A <c>IDirectedPathCollection</c>  for the shortest path found, or null if no path was found.</returns>
        Maybe<IDirectedPath> GetPath(HexCoords source, HexCoords target);
    }

    /// <summary>TODO</summary>
    public interface ILandmark : IDisposable {
        /// <summary>Board coordinates for the landmark location.</summary>
        HexCoords Coords { get; }

        /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
        short? DistanceFrom(HexCoords coords);
        /// <summary>Returns the shortest-path directed-distance to the specified hex from the landmark.</summary>
        short? DistanceTo  (HexCoords coords);
    }

    /// <summary>TODO</summary>
    public interface IDirectedLandmark : IDisposable {
        ///// <summary>Board coordinates for the landmark location.</summary>
        //HexCoords Coords    { get; }

        /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
        short? Distance(HexCoords coords);
    }

    /// <summary>An <see cref="IList"/> of defined <see cref="ILandmark"/> locations.</summary>
    public interface ILandmarkCollection : IFastList<ILandmark>, IDisposable {
        /// <summary>Calculated distance from the specified landmark to the specified hex</summary>
        /// <param name="coords"></param>
        /// <param name="landmarkToShow"></param>
        /// <returns></returns>
        string DistanceFrom(HexCoords coords, int landmarkToShow);
        /// <summary>Calculated distance to the specified landmark from the specified hex</summary>
        /// <param name="coords"></param>
        /// <param name="landmarkToShow"></param>
        /// <returns></returns>
        string DistanceTo(HexCoords coords, int landmarkToShow);
    }

    /// <summary>Interface required to make use of A* Path Finding utility.</summary>
    public interface INavigableBoard {
        /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
        HexSize MapSizeHexes           { get; }

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>
        /// Returning the supplied range multiplied by the cheapest movement cost for a single hex
        /// is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic in order 
        /// for the algorithm to perform properly and reliably return an optimum path.
        /// </remarks>
        short?  Heuristic(HexCoords source, HexCoords target);

        /// <summary>TODO</summary>
        short? TryExitCost(HexCoords hexCoords, Hexside hexside);
        /// <summary>TODO</summary>
        short? TryEntryCost(HexCoords hexCoords, Hexside hexside);
    }

    /// <summary>Interface required to make use of A* Path Finding utility with Landmark heuristic.</summary>
    public interface ILandmarkBoard : INavigableBoard {
        /// <summary>TODO</summary>
        ILandmarks Landmarks { get; }
    }
  
    /// <summary>Interface of common data structures exposed to <see cref="AltPathfinder"/>s.</summary>
    internal interface IPathHalves {
        /// <summary>Retrieves the cost of the shortest path found so far.</summary>
        int             BestSoFar { get; }

        /// <inheritdoc/>
        ILandmarkBoard  Board     { get; }
        /// <inheritdoc/>
        HexCoords       Start     { get; }
        /// <inheritdoc/>
        HexCoords       Goal      { get; }

        /// <summary>The <see cref="ISet{HexCoords}"/> of all hexes expanded in finding the shortest-path.</summary>
        ISet<HexCoords> ClosedSet { get; }

        /// <summary>Updates the record of the shortest path found so far.</summary>
        /// <param name="pathFwd">The half-path obtained by searching backward from the target (so stacked forwards).</param>
        /// <param name="pathRev">The half-path obtained by searching forward from the source (so stacked backwards).</param>
        void            SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd);
    }
}
