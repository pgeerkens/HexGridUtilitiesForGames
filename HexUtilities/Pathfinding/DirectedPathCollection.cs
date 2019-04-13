#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>A directed sequence of <see cref="Hex"/>steps comprising a travel path.</summary>
    [DebuggerDisplay("TotalCost={TotalCost} / TotalSteps={TotalSteps}")]
    internal sealed class DirectedPathCollection : IDirectedPath {
        /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
        public DirectedPathCollection(IHex start)
        : this(null, new DirectedPathStepHex(start, Hexside.North), 0) { }

        /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
        public DirectedPathCollection(IDirectedPath pathSoFar, DirectedPathStepHex pathStep, int stepCost) {
            PathStep   = pathStep;
            PathSoFar  = pathSoFar;
            TotalCost  = (pathSoFar?.TotalCost ?? 0) + stepCost;
            TotalSteps = (pathSoFar?.TotalSteps??-1) + 1;
        }

        /// <inheritdoc/>
        public Hexside              HexsideExit => PathStep.HexsideExit;
        /// <inheritdoc/>
        public IDirectedPath        PathSoFar   { get; }
        /// <inheritdoc/>
        public DirectedPathStepHex  PathStep    { get; }
        /// <inheritdoc/>
        public string               StatusText  => $"Path Length: {TotalCost}/{TotalSteps}";
        /// <inheritdoc/>
        public HexCoords            StepCoords  => PathStep.Coords;
        /// <inheritdoc/>
        public int                  TotalCost   { get; }
        /// <inheritdoc/>
        public int                  TotalSteps  { get; }

        /// <inheritdoc/>
        public override string ToString()
        => PathSoFar == null
            ? $"Hex: {PathStep.Coords} arrives with TotalCost={TotalCost}"
            : $"Hex: {PathStep.Coords} exits {PathStep.HexsideEntry} with TotalCost={TotalCost}";

        /// <summary>Returns the ordered sequence of sub-paths comprising this DirectedPath.</summary>
        public IEnumerator<IDirectedPath> GetEnumerator() {
            yield return this;
            for (var p = (IDirectedPath)this; p.PathSoFar != null; p = p.PathSoFar) {
                yield return p.PathSoFar;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>.</summary>
    internal static partial class PathExtensions {
        /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
        /// <param name="this"></param>
        /// <param name="hex"></param>
        /// <param name="hexsideExit"></param>
        /// <param name="stepCost"></param>
        public static IDirectedPath AddStep(this IDirectedPath @this, IHex hex, Hexside hexsideExit, int stepCost)
        => @this.AddStep(new DirectedPathStepHex(hex,hexsideExit),stepCost);

        /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
        /// <param name="this"></param>
        /// <param name="stepHex"></param>
        /// <param name="stepCost"></param>
        public static IDirectedPath AddStep(this IDirectedPath @this, DirectedPathStepHex stepHex, int stepCost)
        => new DirectedPathCollection(@this,stepHex,stepCost);
    }

    /// <summary>.</summary>
    internal class Path<THex>: IPath<THex> where THex: IHex {
        /// <summary>.</summary>
        public Path(Maybe<IDirectedPath> path, THex source, THex target, ISet<HexCoords> closedSet, ISet<HexCoords> openSet) {
            DirectedPath = path;
            Source       = source;
            Target       = target;
            ClosedSet    = closedSet;
            OpenSet      = openSet;
        }

        /// <inheritdoc/>
        public Maybe<IDirectedPath> DirectedPath { get; }

        /// <inheritdoc/>
        public THex Source { get; }
        /// <inheritdoc/>
        public THex Target { get; }

        /// <inheritdoc/>
        public ISet<HexCoords> OpenSet   { get; }
        /// <inheritdoc/>
        public ISet<HexCoords> ClosedSet { get; }
    }

    /// <summary>.</summary>
    public interface IPath<THex>where THex: IHex {
        /// <summary>.</summary>
        Maybe<IDirectedPath> DirectedPath { get; }

        /// <summary>.</summary>
        THex Source { get; }
        /// <summary>.</summary>
        THex Target { get; }

        /// <summary>.</summary>
        ISet<HexCoords> OpenSet   { get; }
        /// <summary>.</summary>
        ISet<HexCoords> ClosedSet { get; }
    }
}
