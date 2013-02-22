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
  public interface ICoordsCustom {
    int            X         { get; }
    int            Y         { get; }
    IntVector2D    Vector    { get; set; }
    ICoordsCanon   Canon     { get; }
    ICoordsCustom  Clone();
    string         ToString();
    int            Range(ICoordsCustom coords);
    IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);
  }

  public partial struct Coords {
    int           ICoordsCustom.X          { get { return VectorCustom.X; } }
    int           ICoordsCustom.Y          { get { return VectorCustom.Y; } }
    IntVector2D   ICoordsCustom.Vector     { get { return VectorCustom;   }
                                             set { VectorCustom=value;    } }
    ICoordsCanon  ICoordsCustom.Canon      { get { return this;           } } 
    ICoordsCustom ICoordsCustom.Clone()    { return NewCustomCoords( this.VectorCustom);  }
    string        ICoordsCustom.ToString() { return VectorCustom.ToString(); }

    IEnumerable<NeighbourCoords> ICoordsCustom.GetNeighbours(Hexside hexsides) { 
      return GetNeighbours(hexsides); 
    }
    int ICoordsCustom.Range(ICoordsCustom coords) { return ((ICoordsCanon)this).Range(coords.Canon); }

    public static void SetCustomMatrices(IntMatrix2D userToCustom, IntMatrix2D customToUser) {
      MatrixUserToCustom = userToCustom;
      MatrixCustomToUser = customToUser;
    }
  }
}
