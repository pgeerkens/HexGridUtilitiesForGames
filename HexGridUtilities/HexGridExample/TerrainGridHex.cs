#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.HexGridExample {
    public abstract class TerrainGridHex : MapGridHex {

      public TerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }

      public override int Elevation      { get { return  0; }  }
      public override int HeightTerrain  { get { return  ElevationASL; } }
      public override int StepCost       { get { return  4; } }
    }
    public sealed class ClearTerrainGridHex : TerrainGridHex {
      public ClearTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
    }
    public sealed class FordTerrainGridHex : TerrainGridHex {
      public FordTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int StepCost       { get { return  5; } }
    }
    public sealed class RiverTerrainGridHex : TerrainGridHex {
      public RiverTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int StepCost       { get { return -1; } }
    }
    public sealed class RoadTerrainGridHex : TerrainGridHex {
      public RoadTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int StepCost       { get { return  2; } }
    }
    public sealed class TrailTerrainGridHex : TerrainGridHex {
      public TrailTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int StepCost       { get { return  3; } }
    }
    public sealed class HillTerrainGridHex : TerrainGridHex {
      public HillTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int Elevation      { get { return  1; } }
      public override int StepCost       { get { return  5; } }
   }
    public sealed class MountainTerrainGridHex : TerrainGridHex {
      public MountainTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int Elevation      { get { return  2; } }
      public override int StepCost       { get { return  6; } }
    }
    public sealed class WoodsTerrainGridHex : TerrainGridHex {
      public WoodsTerrainGridHex(char value, ICoordsUser coords) : base(value, coords) { }
      public override int HeightTerrain  { get { return ElevationASL + 7; } }
      public override int StepCost       { get { return  8; } }
    }
}
