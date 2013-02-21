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
  public interface ICoordsUser {
    int             X         { get; }
    int             Y         { get; }
    IntVector2D     Vector    { get; set; }
    ICoordsCanon    Canon     { get; }
    ICoordsUser     Clone();
    string          ToString();
    int             Range(ICoordsUser coords);
    IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);
  }

  public partial struct Coords {
    int           ICoordsUser.X          { get { return VectorUser.X; } }
    int           ICoordsUser.Y          { get { return VectorUser.Y; } }
    IntVector2D   ICoordsUser.Vector     { get { return VectorUser;   }
                                           set { VectorUser=value;    } }
    ICoordsCanon  ICoordsUser.Canon      { get { return this; } } 
    ICoordsUser   ICoordsUser.Clone()    { return NewUserCoords( this.VectorUser);  }
    string        ICoordsUser.ToString() { return VectorUser.ToString(); }

    IEnumerable<NeighbourCoords> ICoordsUser.GetNeighbours(Hexside hexsides) { 
      return GetNeighbours(hexsides); 
    }
    int ICoordsUser.Range(ICoordsUser coords) { return ((ICoordsCanon)this).Range(coords.Canon); }
  }
}
