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
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface ICoordsCanon {
    int             X         { get; }
    int             Y         { get; }
    IntVector2D     Vector    { get; set; }
    ICoordsCustom   Custom    { get; }
    ICoordsUser     User      { get; }
    string          ToString();
    int             Range(ICoordsCanon coords);
    IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);

    ICoordsCanon StepOut(IntVector2D vector);
    ICoordsCanon StepOut(Hexside hexside);
  }

  public partial class Coords {
    int             ICoordsCanon.X          { get { return VectorCanon.X; } }
    int             ICoordsCanon.Y          { get { return VectorCanon.Y; } }
    IntVector2D     ICoordsCanon.Vector     { get { return VectorCanon;   }
                                              set { VectorCanon=value;    } }
    ICoordsUser     ICoordsCanon.User       { get { return this; } }
    ICoordsCustom   ICoordsCanon.Custom     { get { return this; } }
    string          ICoordsCanon.ToString() { return VectorCanon.ToString(); }

    IEnumerable<NeighbourCoords> ICoordsCanon.GetNeighbours(Hexside hexsides) { 
      return GetNeighbours(hexsides); 
    }
    int ICoordsCanon.Range(ICoordsCanon coords) { return Range(coords); }

    ICoordsCanon ICoordsCanon.StepOut(IntVector2D vector) { return StepOut(vector); }
    ICoordsCanon ICoordsCanon.StepOut(Hexside hexside) { 
      switch(hexside) {
        case Hexside.NorthWest:   return StepOut(new IntVector2D(-1,-1));
        case Hexside.North:       return StepOut(new IntVector2D( 0,-1));
        case Hexside.NorthEast:   return StepOut(new IntVector2D( 1, 0));
        case Hexside.SouthEast:   return StepOut(new IntVector2D( 1, 1));
        case Hexside.South:       return StepOut(new IntVector2D( 0, 1));
        case Hexside.SouthWest:   return StepOut(new IntVector2D(-1, 0));
        default:                  throw new ArgumentOutOfRangeException();
      }
    }
  }
}
