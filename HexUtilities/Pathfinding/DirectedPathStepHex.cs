#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System;
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>A <see cref="DirectedPathCollection"/>Step with a board location and travel direction.</summary>
    [DebuggerDisplay("NeighbourHex: {Hex.Coords} enters from {HexsideEntry}")]
    public struct DirectedPathStepHex : IEquatable<DirectedPathStepHex> {
        /// <summary>Creates a new <see cref="DirectedPathStepHex"/> instance at <paramref name="coords"/> exiting through <paramref name="hexsideExit"/>.</summary>
        public DirectedPathStepHex(HexCoords coords, Hexside hexsideExit) : this() {
            Coords      = coords;
            HexsideExit = hexsideExit;
        }

        /// <summary>The hex of the <see cref="DirectedPathCollection"/>Step.</summary>
        public HexCoords Coords { get; }

        /// <summary>The hexside of the neighbour through which the agent enters from this hex.</summary>
        public Hexside HexsideEntry => HexsideExit.Reversed;

        /// <summary>The hexside of this hex through which the agent exits to the neighbour.</summary>
        public Hexside HexsideExit { get; }

        #region Value Equality - on Hex field only
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is DirectedPathStepHex other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(DirectedPathStepHex other) => Coords == other.Coords;

        /// <inheritdoc/>
        public override int GetHashCode() => Coords.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(DirectedPathStepHex lhs, DirectedPathStepHex rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(DirectedPathStepHex lhs, DirectedPathStepHex rhs) =>  lhs.Equals(rhs);
        #endregion

        /// <inheritdoc/>
        public override string ToString() => $"NeighbourHex: {Coords} enters from {HexsideEntry}";
  }
}
