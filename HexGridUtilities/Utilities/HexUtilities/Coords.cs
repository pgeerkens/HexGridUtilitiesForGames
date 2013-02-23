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
  public enum CoordsType { Canon, User, Custom }

  public partial struct Coords : ICoordsCanon, ICoordsUser, ICoordsCustom, IEquatable<Coords>, IEqualityComparer<Coords> {
    public static Func<ICoordsUser,ICoordsUser>[] Directions = 
      new Func<ICoordsUser,ICoordsUser>[] {
        (c)=>c.Canon.StepNorthWest.User,
        (c)=>c.Canon.StepNorth.User,
        (c)=>c.Canon.StepNorthEast.User,
        (c)=>c.Canon.StepSouthWest.User,
        (c)=>c.Canon.StepSouth.User,
        (c)=>c.Canon.StepSouthEast.User
      };

    static readonly ICoordsCanon _EmptyCanon = Coords.NewCanonCoords(0,0);
    static readonly ICoordsUser  _EmptyUser  = Coords.NewUserCoords(0,0);
    public static ICoordsCanon EmptyCanon { get { return _EmptyCanon; } }
    public static ICoordsUser  EmptyUser  { get { return _EmptyUser; } }

    /// <inheritDoc/>
    /// <remarks>Prefers Custom over User over Canon in choosing which values to present.</remarks>
    public override string ToString() {
      return ! isCustomNull ? string.Format("Custom: {0}", vectorCustom)
           : ! isUserNull   ? string.Format("User: {0}",  vectorUser)
                            : string.Format("Canon: {0}",vectorCanon);
    }

    #region Value Equality
    bool IEquatable<Coords>.Equals(Coords rhs) { return this == rhs; }
    public override bool Equals(object rhs) { return (rhs is Coords) && this == (Coords)rhs; }
    public static bool operator == (Coords lhs, Coords rhs) { return lhs.VectorCanon == rhs.VectorCanon; }
    public static bool operator != (Coords lhs, Coords rhs) { return ! (lhs == rhs); }
    public override int GetHashCode() { return VectorUser.GetHashCode(); }

    bool IEqualityComparer<Coords>.Equals(Coords lhs, Coords rhs) { return lhs == rhs; }
    int  IEqualityComparer<Coords>.GetHashCode(Coords coords) { return coords.GetHashCode(); }
    #endregion

    #region Constructors
    public static ICoordsCanon  NewCanonCoords (IntVector2D vector){ return new Coords(CoordsType.Canon, vector); }
    public static ICoordsUser   NewUserCoords  (IntVector2D vector){ return new Coords(CoordsType.User,vector); }
    public static ICoordsCustom NewCustomCoords(IntVector2D vector){ return new Coords(CoordsType.Custom,vector); }
    public static ICoordsCanon  NewCanonCoords (int x, int y) { return new Coords(CoordsType.Canon, x,y); }
    public static ICoordsUser   NewUserCoords  (int x, int y) { return new Coords(CoordsType.User,x,y); }
    public static ICoordsUser   NewCustomCoords(int x, int y) { return new Coords(CoordsType.Custom,x,y); }

    private Coords(CoordsType coordsType, int x, int y) : this(coordsType, new IntVector2D(x,y)) {}
    private Coords(CoordsType coordsType, IntVector2D vector) : this() {
      switch(coordsType) {
        default:
        case CoordsType.Canon:   vectorCanon  = vector;  isUserNull   = isCustomNull = true;  break;
        case CoordsType.User:    vectorUser   = vector;  isCustomNull = isCanonNull  = true;  break;
        case CoordsType.Custom:  vectorCustom = vector;  isCanonNull  = isUserNull   = true;  break;
      }
    }
    #endregion

    #region Conversions
    static readonly IntMatrix2D MatrixUserToCanon = new IntMatrix2D(2,1, 0,2, 0,0, 2);
    static IntMatrix2D MatrixCustomToUser         = new IntMatrix2D(2,0, 0,2, 0,0, 2);     
    IntVector2D VectorCanon {
      get { return !isCanonNull ? vectorCanon : VectorUser * MatrixUserToCanon; }
      set { vectorCanon = value;  isUserNull = isCustomNull = true; }
    } IntVector2D vectorCanon;
    bool isCanonNull;

    static readonly IntMatrix2D MatrixCanonToUser  = new IntMatrix2D(2,-1, 0,2, 0,1, 2);    
    IntVector2D VectorUser {
      get { return !isUserNull  ? vectorUser 
                 : !isCanonNull ? VectorCanon  * MatrixCanonToUser
                                : VectorCustom * MatrixCustomToUser; }
      set { vectorUser  = value;  isCustomNull = isCanonNull = true; }
    } IntVector2D vectorUser;
    bool isUserNull;

    static IntMatrix2D MatrixUserToCustom  = new IntMatrix2D(2,0, 0,2, 0,0, 2);
    IntVector2D VectorCustom {
      get { return !isCustomNull ? vectorCustom : VectorUser * MatrixUserToCustom; }
      set { vectorCustom  = value;  isCanonNull = isUserNull = true; }
    } IntVector2D vectorCustom;
    bool isCustomNull;
    #endregion

    #region private internals
    private IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides) {
      ICoordsCanon coords = this;
      if(hexsides.HasFlag(Hexside.North))     yield return new NeighbourCoords(Hexside.North,     coords.StepNorth);
      if(hexsides.HasFlag(Hexside.NorthEast)) yield return new NeighbourCoords(Hexside.NorthEast, coords.StepNorthEast);
      if(hexsides.HasFlag(Hexside.SouthEast)) yield return new NeighbourCoords(Hexside.SouthEast, coords.StepSouthEast);
      if(hexsides.HasFlag(Hexside.South))     yield return new NeighbourCoords(Hexside.South,     coords.StepSouth);
      if(hexsides.HasFlag(Hexside.SouthWest)) yield return new NeighbourCoords(Hexside.SouthWest, coords.StepSouthWest);
      if(hexsides.HasFlag(Hexside.NorthWest)) yield return new NeighbourCoords(Hexside.NorthWest, coords.StepNorthWest);
    }

    private int Range(ICoordsCanon coords) {
      var deltaX = coords.X - VectorCanon.X;
      var deltaY = coords.Y - VectorCanon.Y;
      return (Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY)) / 2;
    }

    private ICoordsCanon StepOut(IntVector2D vector) { 
      return NewCanonCoords(VectorCanon + vector); 
    }
    #endregion
  } 
}
