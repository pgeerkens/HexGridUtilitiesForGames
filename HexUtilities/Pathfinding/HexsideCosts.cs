#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>TODO</summary>
    public class HexsideCosts : ReadOnlyCollection<int?> {
        /// <summary>TODO</summary>
        public static HexsideCosts ExitCosts<THex>(BoardStorage<Maybe<THex>> boardHexes,HexCoords hexCoords)
        where THex : IHex
        => new HexsideCosts(hexside => DirectedCost(boardHexes,hexCoords,hexside));

        /// <summary>TODO</summary>
        public static HexsideCosts EntryCosts<THex>(BoardStorage<Maybe<THex>> boardHexes,HexCoords hexCoords)
        where THex : IHex
        => new HexsideCosts(hexside => DirectedCost(boardHexes,hexCoords.GetNeighbour(hexside),hexside.Reversed));

        /// <summary>TODO</summary>
        public static int? DirectedCost<THex>(BoardStorage<Maybe<THex>> boardHexes,HexCoords hexCoords,Hexside hexside)
        where THex : IHex
        => boardHexes[hexCoords].Bind<int?>(hex => hex.EntryCost(hexside)).ToNullable();

        private HexsideCosts(Func<Hexside,int?> directedCostToExit) : base(Generator(directedCostToExit)) { }

        static List<int?> Generator(Func<Hexside,int?> directedCostToExit)
        => ( from hexsideExit in Hexside.HexsideList
             select directedCostToExit(hexsideExit)
           ).ToList();
    }
}
