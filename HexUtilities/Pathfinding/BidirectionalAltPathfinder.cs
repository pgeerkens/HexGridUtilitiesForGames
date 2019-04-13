#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace PGNapoleonics.HexUtilities.Pathfinding {

    /// <summary>C# (serial) implementation of NBA* path-finding algorithm by Pijls &amp; Post (Adapted).</summary>
    /// <remarks>
    /// Adapted to hex-grids, and using a suggestion by Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz.
    /// 
    /// A fast efficient serial implementation of <b>Bidirectional ALT</b> (<b>A*</b> with <b>L</b>andmarks
    /// and <b>T</b>riangle-inequality heuristic) <b>path-finding</b> on a <see cref="Hexgrid"/> map.
    /// 
    /// <see cref="AltPathfinder{THex}"/>
    /// <see cref="PathHalves{THex}"/>
    /// </remarks>
    /// See also: <a href="http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf">Computing Point-to-Point Shortest Paths from Extenal Memory - Andrew V. Goldberg &amp; Renato F. Werneck</a>
    /// See also: <a href="http://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf">PNBA*: A Parallel Bidirectional Heuristic Search Algorithm - Luis Henrique Oliveira Rios &amp; Luiz Chaimowicz</a>
    /// See also: <a href="http://repub.eur.nl/res/pub/16100/ei2009-10.pdf">Yet Another Bidirectional Algorithm for Shortest Paths - Wim Pijls &amp; Henk Post </a>
    /// See also: <a href="http://www.cs.trincoll.edu/~ram/cpsc352/notes/astar.html">A* Algorithm Notes</a>
    public static class BidirectionalAltPathfinder {
 
        /// <summary>Returns an <c>IPath</c> for the optimal path from coordinates <paramref name="source"/> to <paramref name="target"/>.</summary>
        /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        /// <returns>A <see cref="IPath{THex}"/> for the shortest path found, or null if no path was found.</returns>
        /// <remarks>
        /// <para>Note that the Heuristic provided by <paramref name="board"/> must be <b>consistent</b> 
        /// in order for the algorithm to perform properly.</para>
        /// </remarks>
        public static IPath<THex> GetPathBiDiAlt<THex>(this ILandmarkBoard<THex> board, THex source, THex target)
        where THex: class,IHex {
            if (board?.Landmarks == null  ||  source == null  ||  target == null) {
                return new Path<THex>(null,source,target,null,null);
            } else {
                source.TraceFindPathDetailInit(target);

                var pathHalves    = board.NewPathHalves(source,target);
                var pathfinderFwd = pathHalves.NewAltPathfinder(false);
                var pathfinderRev = pathHalves.NewAltPathfinder(true);

                // Alternate searching from each direction and calling the other direction
                pathfinderFwd.Partner = pathfinderRev;
                var pathfinder        = pathfinderRev.Partner
                                      = pathfinderFwd;

                while (! pathfinder.IsFinished())  pathfinder = pathfinder.Partner; 

                pathHalves.ClosedSet.Count.TraceFindPathDone();
                return new Path<THex>(pathHalves.PathFwd,source,target,pathHalves.ClosedSet,null);
            }
        }

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="board"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static IPathHalves<THex> NewPathHalves<THex>(this ILandmarkBoard<THex> board, THex source, THex target)
        where THex: class,IHex
        => new PathHalves<THex>(board, source, target);

        /// <summary>.</summary>
        /// <typeparam name="THex"></typeparam>
        /// <param name="this"></param>
        /// <param name="isForward"></param>
        /// <returns></returns>
        internal static IAltPathfinder<THex> NewAltPathfinder<THex>(this IPathHalves<THex> @this, bool isForward)
        where THex: class,IHex
        => new AltPathfinder<THex>(@this, isForward);
    }
}
