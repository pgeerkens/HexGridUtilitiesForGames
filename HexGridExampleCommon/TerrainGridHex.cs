#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;
using System.Linq;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridExampleCommon {
    /// <summary>Implementation of abstract class <c>MapGridHex</c> as used in the examples.</summary>
    public class TerrainGridHex : Hex {
        /// <summary>Returns a new passable <see cref="TerrainGridHex"/>, as specified.</summary>
        /// <param name="coords">Board location of this hex.</param>
        /// <param name="elevationLevel">Elevation of this hex.</param>
        /// <param name="heightTerrain"></param>
        /// <param name="terrainType"></param>
        /// <param name="stepCost"></param>
        public static TerrainGridHex NewPassable(HexCoords coords, int elevationLevel,
                                            int heightTerrain, char terrainType, int stepCost)
        => new TerrainGridHex(coords, elevationLevel, heightTerrain, terrainType, stepCost);

        /// <summary>Returns a new impassable <see cref="TerrainGridHex"/>.</summary>
        /// <param name="coords">Board location of this hex.</param>
        /// <param name="elevationLevel">Elevation of this hex.</param>
        /// <param name="heightTerrain"></param>
        /// <param name="terrainType"></param>
        public static TerrainGridHex NewImpassable(HexCoords coords, int elevationLevel,
                                            int heightTerrain, char terrainType)
        => new TerrainGridHex(coords, elevationLevel, heightTerrain, terrainType, null);

        /// <summary>Initializes a new instance of a <see cref="TerrainGridHex"/>.</summary>
        /// <param name="coords">Board location of this hex.</param>
        /// <param name="elevationLevel">Elevation of this hex.</param>
        /// <param name="heightTerrain">Elevation of the terrain in this hex.</param>
        /// <param name="terrainType">Type of the terrain in this hex.</param>
        /// <param name="stepCost">Cost to enter this hex.</param>
        private TerrainGridHex(HexCoords coords, int elevationLevel, int heightTerrain,
                        char terrainType, int? stepCost)
        : base(coords,elevationLevel) {
            HeightTerrain = heightTerrain;
            TerrainType   = terrainType;
            _stepCost     = stepCost;
        }

        ///  <inheritdoc/>
        public  override int   HeightTerrain { get; }

        ///  <inheritdoc/>
        public override bool   IsPassable => _stepCost != null;

        ///  <inheritdoc/>
        public override char   TerrainType   { get; }
   
        ///  <inheritdoc/>
        public override int    EntryCost(Hexside hexside) => _costs[(int)Direction.ToHex][hexside];

        ///  <inheritdoc/>
        public override int    ExitCost(Hexside hexside) => _costs[(int)Direction.FromHex][hexside];

        ///  <inheritdoc/>
        private         int?   StepCost(Hexside direction) => _stepCost; 
        internal int? _stepCost { get; }

        internal void SetCosts<THex>(GetHex board)
        => _costs = new List<IList<int>>() {
             ( from hexside in Hexside.HexsideList select StepCost(hexside) ?? -1).ToList(),
             ( from hexside in Hexside.HexsideList
               select ( from hex in board(Coords.GetNeighbour(hexside)) select hex.StepCost(hexside)
                      ).ElseDefault() ?? -1
             ).ToList()
        };

        IList<IList<int>> _costs;

        internal delegate Maybe<TerrainGridHex> GetHex(HexCoords coords);
    }
}
