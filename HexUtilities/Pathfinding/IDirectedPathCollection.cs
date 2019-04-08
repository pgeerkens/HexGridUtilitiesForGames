#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;
    using PathStepHex   = DirectedPathStepHex;

    /// <summary>Structure returned by the A* Path Finding utility.</summary>
    public interface IDirectedPathCollection : IEnumerable<IDirectedPath> { 
        /// <summary>The <see cref="Hexside"/> through which an agent must move in taking the first step of this path.</summary>
        Hexside       HexsideExit { get; }

        /// <summary>The coordinates of the first step on this path.</summary>
        HexCoords     StepCoords  { get; }

        /// <summary>The total movement cost for this path.</summary>
        int           TotalCost   { get; }

        /// <summary>The total number of movement steps for this path.</summary>
        int           TotalSteps  { get; }

        /// <summary>The first step on this path.</summary>
        PathStepHex   PathStep    { get; }

        /// <summary>The remaining steps of this path, as an <see cref="IDirectedPathCollection"/> instance.</summary>
        IDirectedPath PathSoFar   { get; }

        /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
        /// <param name="neighbour"></param>
        /// <param name="stepCost"></param>
        /// <returns></returns>
        IDirectedPath AddStep(PathStepHex neighbour, int stepCost);

        /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
        /// <param name="coords"></param>
        /// <param name="hexsideEntry"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        IDirectedPath AddStep(HexCoords coords, Hexside hexsideEntry, int cost);

        /// <summary>Returns a descriptive text suitable for the Status Bar.</summary>
        string StatusText { get; }
    }
}
