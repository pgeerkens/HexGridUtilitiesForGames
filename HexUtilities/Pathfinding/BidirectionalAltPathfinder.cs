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
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).</summary>
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
    public sealed class BidirectionalAltPathfinder : IPathfinder{

        /// <summary>Calculates an <see cref="IDirectedPath"/> for the optimal path from coordinates.</summary>
        /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
        /// <remarks>
        /// <para>Note that the Heuristic provided by <paramref name="board"/> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly.</para>
        /// <seealso cref="StandardPathfinder"/>
        /// </remarks>
        public BidirectionalAltPathfinder(ILandmarkBoard board) => Board = board;
   
        private ILandmarkBoard Board { get; }

        /// <inheritdoc/>
        public Maybe<IDirectedPath> GetPath(HexCoords source, HexCoords target) {
            if (Board?.Landmarks == null) {
                return null; 
            } else {
                PathfinderExtensions.TraceFindPathDetailInit(source, target);

                var pathHalves    = new PathHalves(Board, source, target);
                var pathfinderFwd = new AltPathfinder(pathHalves, true);
                var pathfinderRev = new AltPathfinder(pathHalves, false);

                return pathHalves.GetPath(pathfinderFwd, pathfinderRev);
            }
        }
    }
}

