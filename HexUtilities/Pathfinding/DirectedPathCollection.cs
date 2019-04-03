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
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.Pathfinding {

    /// <summary>A directed sequence of <see cref="Hex"/>steps comprising a travel path.</summary>
    [DebuggerDisplay("TotalCost={TotalCost} / TotalSteps={TotalSteps}")]
    internal class DirectedPathCollection : IDirectedPathCollection {
        /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
        internal DirectedPathCollection(HexCoords start)
        : this(null, new DirectedPathStepHex(start, Hexside.North), 0) { }

        /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
        internal DirectedPathCollection(IDirectedPathCollection pathSoFar, DirectedPathStepHex pathStep, int totalCost) {
            PathStep    = pathStep;
            PathSoFar   = pathSoFar;
            TotalCost   = totalCost;
            TotalSteps  = pathSoFar==null ? 0 : pathSoFar.TotalSteps+1;
        }

        #region Properties
        /// <inheritdoc/>
        public Hexside                  HexsideExit => PathStep.HexsideExit;
        /// <inheritdoc/>
        public IDirectedPathCollection  PathSoFar   { get; }
        /// <inheritdoc/>
        public DirectedPathStepHex      PathStep    { get; }
        /// <inheritdoc/>
        public string                   StatusText  => $"Path Length: {TotalCost}/{TotalSteps}";
        /// <inheritdoc/>
        public HexCoords                StepCoords  => PathStep.Coords;
        /// <inheritdoc/>
        public int                      TotalCost   { get; }
        /// <inheritdoc/>
        public int                      TotalSteps  { get; }
        #endregion

        /// <inheritdoc/>
        public IDirectedPathCollection AddStep(HexCoords coords, Hexside hexsideExit, int stepCost)
        => AddStep(new DirectedPathStepHex(coords,hexsideExit), stepCost);
        
        /// <inheritdoc/>
        public IDirectedPathCollection AddStep(DirectedPathStepHex neighbour, int stepCost)
        => new DirectedPathCollection(this, neighbour, TotalCost + stepCost);

        /// <inheritdoc/>
        public override string ToString()
        => PathSoFar == null
            ? $"Hex: {PathStep.Coords} arrives with TotalCost={TotalCost}"
            : $"Hex: {PathStep.Coords} exits {PathStep.HexsideEntry} with TotalCost={TotalCost}";

        /// <summary>Returns the ordered sequence of sub-paths comprising this DirectedPath.</summary>
        public IEnumerator<IDirectedPathCollection> GetEnumerator() {
            yield return this;
            for (var p = (IDirectedPathCollection)this; p.PathSoFar != null; p = p.PathSoFar) 
                yield return p.PathSoFar;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal sealed class FinalPath : DirectedPathCollection {
        internal FinalPath(HexCoords starth) : base(starth) {

        }
    }
}
