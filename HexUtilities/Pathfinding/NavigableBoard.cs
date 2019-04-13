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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Storage;

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
        public int? Heuristic(IHex here, IHex target) =>
            MapSizeHexes.IsOnboard(target.Coords)  &&  MapSizeHexes.IsOnboard(here.Coords)
                ? here.Range(target) * _minimumCost
                : default;

        /// <summary>TODO</summary>
        public int TryExitCost(HexCoords hexCoords, Hexside hexside) =>
            (from x in _exitCosts[hexCoords] select x[hexside]).ElseDefault() ?? -1;

        /// <summary>TODO</summary>
        public int TryEntryCost(HexCoords hexCoords, Hexside hexside) =>
            (from x in _entryCosts[hexCoords] select x[hexside]).ElseDefault() ?? -1;

        readonly StepCosts _entryCosts;
        readonly StepCosts _exitCosts;
        readonly int       _minimumCost;
    }
}
