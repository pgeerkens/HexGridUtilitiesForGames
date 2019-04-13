#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;
using System.Linq;

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridExampleCommon {
    /// <summary>TODO</summary>
    public sealed class EmptyGridHex : Hex {
        /// <summary>TODO</summary>
        public EmptyGridHex(HexCoords coords) : base(coords,0) => TerrainType = default; 

        ///  <inheritdoc/>
        public override char   TerrainType   { get; }

        ///  <inheritdoc/>
        public override int    HeightTerrain => 0;

        ///  <inheritdoc/>
        public override bool   IsPassable => false;

        ///  <inheritdoc/>
        public override int    EntryCost(Hexside hexsideExit) => -1;

        ///  <inheritdoc/>
        public override int    ExitCost(Hexside hexsideExit) => -1;

        ///  <inheritdoc/>
        private         int?   StepCost(Hexside hexsideExit) => default;
 
        internal void SetCosts<THex>(IBoard<EmptyGridHex> board)
        => _costs = new List<IList<int>>() {
             ( from hexside in Hexside.HexsideList select StepCost(hexside) ?? -1).ToList(),
             ( from hexside in Hexside.HexsideList
               select ( from hex in board[Coords.GetNeighbour(hexside)] select hex.StepCost(hexside)
                      ).ElseDefault() ?? -1
             ).ToList()
        };

        IList<IList<int>> _costs;
   }
}
