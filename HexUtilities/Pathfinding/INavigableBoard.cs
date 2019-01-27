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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using HexSize = System.Drawing.Size;

    /// <summary>Interface required to make use of A* Path Finding utility.</summary>
    [ContractClass(typeof(INavigableBoardContract))]
    public interface INavigableBoard {
        /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
        HexSize MapSizeHexes           { get; }

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>
        /// Returning the supplied range multiplied by the cheapest movement cost for a single hex
        /// is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic in order 
        /// for the algorithm to perform properly and reliably return an optimum path.
        /// </remarks>
        [Pure]Maybe<short>  Heuristic(HexCoords source, HexCoords target);

        /// <summary>TODO</summary>
        Maybe<short> TryExitCost(HexCoords hexCoords, Hexside hexside);
        /// <summary>TODO</summary>
        Maybe<short> TryEntryCost(HexCoords hexCoords, Hexside hexside);
    }

    [ContractClassFor(typeof(INavigableBoard))]
    internal abstract class INavigableBoardContract : INavigableBoard {
        private INavigableBoardContract() { }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void ObjectInvariant() {
            Contract.Invariant(MapSizeHexes.Width  > 0);
            Contract.Invariant(MapSizeHexes.Height > 0);
        }

        public abstract HexSize       MapSizeHexes           { get; }

        [Pure]public           Maybe<short>  Heuristic(HexCoords source, HexCoords target) {
            Contract.Ensures(Contract.Result<int>() >= 0);
            // Contract.Ensures((range > 0).Implies(() => Contract.Result<int>() > 0));
            return default(int);
        }
        [Pure]public   Maybe<short> TryEntryCost(HexCoords hexCoords, Hexside hexside) {
            Contract.Ensures(Contract.Result<Maybe<short>>().ValueContract(v => v>0));
            return default(Maybe<short>);
        }
        [Pure]public   Maybe<short> TryExitCost(HexCoords hexCoords, Hexside hexside) {
            Contract.Ensures(Contract.Result<Maybe<short>>().ValueContract(v => v>0));
            return default(Maybe<short>);
        }
    }
}
namespace PGNapoleonics.HexUtilities.Pathfinding {
    using HexSize   = System.Drawing.Size;
    using StepCosts = IBoardStorage<Maybe<HexsideCosts>>;

    /// <summary>TODO</summary>
    public class NavigableBoard : INavigableBoard {
        /// <summary>TODO</summary>
        public NavigableBoard(HexSize mapSizeHexes, StepCosts entryCosts, StepCosts exitCosts)
        : this(mapSizeHexes, entryCosts, exitCosts, 1) { }
        /// <summary>TODO</summary>
        public NavigableBoard(HexSize mapSizeHexes, StepCosts entryCosts, StepCosts exitCosts, int minimumCost) {
            MapSizeHexes = mapSizeHexes;
            _entryCosts   = entryCosts;
            _exitCosts    = exitCosts;
            _minimumCost  = minimumCost;
        }

        /// <summary>TODO</summary>
        public HexSize MapSizeHexes { get; }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow",
                MessageId = "2*range", Justification="No map is big enough to overflow,")]
        public Maybe<short> Heuristic(HexCoords here, HexCoords target) =>
            MapSizeHexes.IsOnboard(target)  &&  MapSizeHexes.IsOnboard(here)
                ? (short)(here.Range(target) * _minimumCost)
                : Maybe<short>.NoValue();

        /// <summary>TODO</summary>
        public Maybe<short> TryExitCost(HexCoords hexCoords, Hexside hexside) =>
            (from x in _exitCosts[hexCoords] from c in x[hexside].ToMaybe() select c).ToMaybe();

        /// <summary>TODO</summary>
        public Maybe<short> TryEntryCost(HexCoords hexCoords, Hexside hexside) =>
            (from x in _entryCosts[hexCoords] from c in x[hexside].ToMaybe() select c).ToMaybe();

        readonly StepCosts _entryCosts;
        readonly StepCosts _exitCosts;
        readonly int       _minimumCost;
    }
}
