#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
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
using System.Collections.Generic;
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>This class manages the to unidirectional searches, and merges the resulting optimal path.</summary>
    /// <remarks>
    /// C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).
    /// 
    /// Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.
    /// <see cref="StandardPathfinder"/>
    /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
    /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
    /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
    /// </remarks>
    internal class PathHalves : IPathHalves {
        public PathHalves(ILandmarkBoard board, HexCoords start, HexCoords goal) {
            Board       = board;
            Start       = start;
            Goal        = goal;
            ClosedSet   = new HashSet<HexCoords>();
            BestSoFar   = int.MaxValue;
        }

        /// <inheritdoc/>
        public  int             BestSoFar { get; set; }
        /// <inheritdoc/>
        public  ILandmarkBoard  Board     { get; }
        /// <inheritdoc/>
        public  ISet<HexCoords> ClosedSet { get; private set; }   
        /// <inheritdoc/>
        public  HexCoords       Start     { get; }
        /// <inheritdoc/>
        public  HexCoords       Goal      { get; }

        /// <summary>The half-path obtained by searching backward from the target (so stacked forwards).</summary>
        private IDirectedPath   _pathFwd;
        /// <summary>The half-path obtained by searching forward from the source (so stacked backwards).</summary>
        private IDirectedPath   _pathRev;

        /// <inheritdoc/>
        public void SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd) {
            if( pathFwd.TotalCost + pathRev.TotalCost < BestSoFar) {
                _pathRev  = pathRev; 
                _pathFwd  = pathFwd; 
                BestSoFar = _pathRev.TotalCost + _pathFwd.TotalCost;

                PathfinderExtensions.TraceFindPathDetailBestSoFar(pathFwd, pathRev, BestSoFar);
            }
        }

        /// <inheritdoc/>
        public Maybe<IDirectedPath> GetPath(AltPathfinder pathfinderFwd, AltPathfinder pathfinderRev) {
            pathfinderFwd.Partner = pathfinderRev;
            pathfinderRev.Partner = pathfinderFwd;
            var pathfinder        = pathfinderFwd;

            // Until done: alternate searching from each direction and calling the other direction
            while (! pathfinder.IsFinished()) { pathfinder = pathfinder.Partner; }

            PathfinderExtensions.TraceFindPathDone(ClosedSet.Count);

            return _pathFwd.MergePaths(_pathRev);
        }
    }
}

