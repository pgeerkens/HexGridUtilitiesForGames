#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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
    ICoordsCanon    Clone();
    string          ToString();
    int             Range(ICoordsCanon coords);
    IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);

    ICoordsCanon StepNorthWest { get; }
    ICoordsCanon StepNorth     { get; }
    ICoordsCanon StepNorthEast { get; }
    ICoordsCanon StepSouthEast { get; }
    ICoordsCanon StepSouth     { get; }
    ICoordsCanon StepSouthWest { get; }

    ICoordsCanon StepOut(IntVector2D vector);
    ICoordsCanon StepOut(Hexside hexside);
  }

  public partial struct Coords {
    int             ICoordsCanon.X          { get { return VectorCanon.X; } }
    int             ICoordsCanon.Y          { get { return VectorCanon.Y; } }
    IntVector2D     ICoordsCanon.Vector     { get { return VectorCanon;   }
                                              set { VectorCanon=value;    } }
    ICoordsUser     ICoordsCanon.User       { get { return this; } }
    ICoordsCustom   ICoordsCanon.Custom     { get { return this; } }
    ICoordsCanon    ICoordsCanon.Clone()    { return NewCanonCoords(this.VectorCanon); }
    string          ICoordsCanon.ToString() { return VectorCanon.ToString(); }

    IEnumerable<NeighbourCoords> ICoordsCanon.GetNeighbours(Hexside hexsides) { 
      return GetNeighbours(hexsides); 
    }
    int ICoordsCanon.Range(ICoordsCanon coords) { return this.Range(coords); }

    ICoordsCanon ICoordsCanon.StepNorthWest { get { return StepOut(new IntVector2D(-1,-1)); } }
    ICoordsCanon ICoordsCanon.StepNorth     { get { return StepOut(new IntVector2D( 0,-1)); } }
    ICoordsCanon ICoordsCanon.StepNorthEast { get { return StepOut(new IntVector2D( 1, 0)); } }
    ICoordsCanon ICoordsCanon.StepSouthEast { get { return StepOut(new IntVector2D( 1, 1)); } }
    ICoordsCanon ICoordsCanon.StepSouth     { get { return StepOut(new IntVector2D( 0, 1)); } }
    ICoordsCanon ICoordsCanon.StepSouthWest { get { return StepOut(new IntVector2D(-1, 0)); } }

    ICoordsCanon ICoordsCanon.StepOut(IntVector2D vector) { return StepOut(vector); }
    ICoordsCanon ICoordsCanon.StepOut(Hexside hexside) { 
      switch(hexside) {
        case Hexside.North:       return ((ICoordsCanon)this).StepNorth;
        case Hexside.NorthEast:   return ((ICoordsCanon)this).StepNorthEast;
        case Hexside.SouthEast:   return ((ICoordsCanon)this).StepSouthEast;
        case Hexside.South:       return ((ICoordsCanon)this).StepSouth;
        case Hexside.SouthWest:   return ((ICoordsCanon)this).StepSouthWest;
        case Hexside.NorthWest:   return ((ICoordsCanon)this).StepNorthWest;
        default:                  throw new ArgumentOutOfRangeException();
      }
    }
  }
}
