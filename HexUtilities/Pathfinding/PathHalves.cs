#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>This class manages the to unidirectional searches, and merges the resulting optimal path.</summary>
    /// <remarks>
    /// Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.
    /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
    /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
    /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
    /// </remarks>
    internal sealed class PathHalves<THex>: IPathHalves<THex>
    where THex: class,IHex {
        /// <summary>.</summary>
        /// <param name="board">Board on which this path search is taking place.</param>
        /// <param name="source">Start hex for this half of the bidirectional path search.</param>
        /// <param name="target">Goal hex for this this half of the bidirectional path search.</param>
        public PathHalves(ILandmarkBoard<THex> board, THex source, THex target) {
            Board     = board;
            Source    = source;
            Target    = target;
            ClosedSet = new HashSet<HexCoords>();
            BestSoFar = int.MaxValue;
        }

        /// <inheritdoc/>
        public int                  BestSoFar { get; private set; }
        /// <inheritdoc/>
        public ILandmarkBoard<THex> Board     { get; }
        /// <inheritdoc/>
        public ISet<HexCoords>      ClosedSet { get; }
        /// <inheritdoc/>
        public THex                 Source    { get; }
        /// <inheritdoc/>
        public THex                 Target    { get; }

        /// <summary>Retrieve the found path in walking order: first step at top of stack to target at bottom.</summary>
        /// <see cref="IDirectedPath"/>
        public Maybe<IDirectedPath> PathFwd => _pathRev.MergePaths<THex>(_pathFwd);

        /// <summary>Updates the record of the shortest path found so far.</summary>
        /// <param name="pathFwd">The half-path obtained by searching backward from the target (so stacked forwards).</param>
        /// <param name="pathRev">The half-path obtained by searching forward from the source (so stacked backwards).</param>
        public void    SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd) {
            if (pathFwd==null  ||  pathRev==null) return;

            if( pathFwd.TotalCost + pathRev.TotalCost < BestSoFar) {
                _pathRev  = pathRev; 
                _pathFwd  = pathFwd; 
                BestSoFar = _pathRev.TotalCost + _pathFwd.TotalCost;

                pathFwd.TraceFindPathDetailBestSoFar(pathRev, BestSoFar);
            }
        }

        /// <summary>The half-path obtained by searching backward from the target (so stacked forwards).</summary>
        private IDirectedPath _pathFwd = null;
        /// <summary>The half-path obtained by searching forward from the source (so stacked backwards).</summary>
        private IDirectedPath _pathRev = null;
    }
}

