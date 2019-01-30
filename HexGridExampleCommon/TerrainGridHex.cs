#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridExampleCommon {
    /// <summary>Abstract class for <c>MapGridHex</c> as used in the examples.</summary>
    internal abstract class TerrainGridHex : Hex {
        /// <summary>Initializes a new instance of a <see cref="TerrainGridHex"/>.</summary>
        /// <param name="coords">Board location of this hex.</param>
        /// <param name="elevationLevel">Elevation of this hex.</param>
        protected TerrainGridHex(HexCoords coords, int elevationLevel, int heightTerrain,
                char terrainType)
        : base(coords,elevationLevel) {
            HeightTerrain  = heightTerrain;
            TerrainType    = terrainType;
        }

        ///  <inheritdoc/>
        public override char   TerrainType   { get; }

        ///  <inheritdoc/>
        public  override int   HeightTerrain { get; }
    }

    /// <summary>A <see cref="TerrainGridHex"/> representing clear terrain.</summary>
    internal sealed class PassableTerrainGridHex : TerrainGridHex {
        /// <summary>Creates a new instance of a passable <see cref="TerrainGridHex"/>.</summary>
        public PassableTerrainGridHex(HexCoords coords, int elevationLevel, int heightTerrain,
                short stepCost, char terrainType)
        : base(coords,elevationLevel, heightTerrain, terrainType) => _stepCost = stepCost;
   
        ///  <inheritdoc/>
        public    override short? TryStepCost(Hexside direction) => _stepCost; readonly short _stepCost;
    }

    /// <summary>A <see cref="TerrainGridHex"/> representing clear terrain.</summary>
    internal sealed class ImpassableTerrainGridHex : TerrainGridHex {
        /// <summary>Creates a new instance of an impassable <see cref="TerrainGridHex"/>.</summary>
        public ImpassableTerrainGridHex(HexCoords coords, int elevationLevel, int heightTerrain,
                char terrainType)
        : base(coords,elevationLevel, heightTerrain, terrainType) {
        }
   
        ///  <inheritdoc/>
        public    override short? TryStepCost(Hexside direction) => null;
    }
}
