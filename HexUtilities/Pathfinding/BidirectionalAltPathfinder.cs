#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using PGNapoleonics.HexUtilities.Common;

#pragma warning disable 1587
/// <summary>A fast efficient serial implementation of <b>Bidirectional ALT</b> (<b>A*</b> with <b>L</b>andmarks
/// and <b>T</b>riangle-inequality heuristic) <b>path-finding</b> on a <see cref="Hexgrid"/> map.</summary>
#pragma warning restore 1587
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

