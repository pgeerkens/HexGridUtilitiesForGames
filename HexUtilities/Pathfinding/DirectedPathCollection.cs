#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using IDirectedPath = IDirectedPathCollection;

    /// <summary>A directed sequence of <see cref="Hex"/>steps comprising a travel path.</summary>
    [DebuggerDisplay("TotalCost={TotalCost} / TotalSteps={TotalSteps}")]
    internal sealed class DirectedPathCollection : IDirectedPath {
        /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
        public DirectedPathCollection(HexCoords start)
        : this(null, new DirectedPathStepHex(start, Hexside.North), 0) { }

        /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
        public DirectedPathCollection(IDirectedPath pathSoFar, DirectedPathStepHex pathStep, int totalCost) {
            PathStep   = pathStep;
            PathSoFar  = pathSoFar;
            TotalCost  = totalCost;
            TotalSteps = pathSoFar==null ? 0 : pathSoFar.TotalSteps+1;
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
        public IDirectedPath AddStep(HexCoords coords, Hexside hexsideExit, int stepCost)
        => AddStep(new DirectedPathStepHex(coords,hexsideExit),stepCost);
        
        /// <inheritdoc/>
        public IDirectedPath AddStep(DirectedPathStepHex neighbour, int stepCost)
        => new DirectedPathCollection(this,neighbour,TotalCost + stepCost);

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
}
