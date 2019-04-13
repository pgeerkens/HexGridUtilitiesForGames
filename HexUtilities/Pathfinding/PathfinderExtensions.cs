#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>.</summary>
    public static class PathfinderExtensions {
        /// <summary>Returns the result of stacking <paramref name="mergePath"/> onto <paramref name="targetPath"/></summary>
        public static Maybe<IDirectedPath> MergePaths<THex>(this IDirectedPath targetPath, IDirectedPath mergePath)
        where THex: class,IHex {
            if (mergePath != null) {
                while (mergePath.PathSoFar != null) {
                    var hexside = mergePath.PathStep.HexsideExit;
                    var cost    = mergePath.TotalCost - (mergePath = mergePath.PathSoFar).TotalCost;
                    targetPath  = targetPath.AddStep(mergePath.PathStep.Hex, hexside, cost);
                }
            }
            return targetPath.ToMaybe();
        }
    }
}
