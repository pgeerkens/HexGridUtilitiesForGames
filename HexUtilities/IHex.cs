#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  /// <summary>External interface exposed by individual hexes.</summary>
  [ContractClass(typeof(IHexContract))]
  public interface IHex {
    /// <summary>The <c>HexCoords</c> coordinates for this hex on <c>Board</c>.</summary>
    [Pure]HexCoords Coords         { get; }

    /// <summary>Elevation of this hex in "steps" above the minimum elevation of the board.</summary>
    [Pure]int       ElevationLevel { get; }

    /// <summary>Height ASL in <i>game units</i> of observer's eyes for FOV calculations.</summary>
    [Pure]int       HeightObserver { get; }

    /// <summary>Height ASL in <i>game units</i> of target above ground level to be spotted.</summary>
    [Pure]int       HeightTarget   { get; }

    /// <summary>Height ASL in <i>game units</i> of any blocking terrian in this hex.</summary>
    [Pure]int       HeightTerrain  { get; }

    /// <summary>Returns the cost (> 0) to extend the path across <paramref name="hexsideExit"/>; or null.</summary>
    [Pure]short?    TryStepCost(Hexside hexsideExit);

    /// <summary>Height ASL in <i>game units</i> of any blocking terrain in this hex and the specified Hexside.</summary>
    [Pure]int       HeightHexside(Hexside hexside);
  }

    /// <summary>Internal contract class for <see cref="IHex"/></summary>
    [ContractClassFor(typeof(IHex))]
    internal abstract class IHexContract : IHex {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IHexContract() { }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void ObjectInvariant() {
          Contract.Invariant(Coords.User.X >= 0);
          Contract.Invariant(Coords.User.Y >= 0);
          Contract.Invariant(ElevationLevel >= 0);

          Contract.Invariant(Enumerable.Range(0,int.MaxValue).All(i => Coords.User.Y/i >= 0) );
        }

        [Pure]
        public short? TryStepCost(Hexside hexsideExit) {
          Contract.Ensures(Contract.Result<Maybe<short>>().ValueContract(s => s > 0));
          return default(short?);
        }

        public abstract HexCoords Coords         { get; }
        public abstract int       ElevationLevel { get; }
        public abstract int       HeightObserver { get; }
        public abstract int       HeightTarget   { get; }
        public abstract int       HeightTerrain  { get; }
        public abstract int       HeightHexside(Hexside hexside);
    }
}
