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

using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities {
  public class HexCoords : ICoords,
    IEquatable<HexCoords>, IEqualityComparer<HexCoords> {
    #region static members
    static HexCoords() {
      HexsideList        = Utils.EnumGetValues<Hexside>().Where(h=>h!=Hexside.None).ToList();

      MatrixUserToCanon  = new IntMatrix2D(2, 1,  0,2,  0,0,  2);
      MatrixCanonToUser  = new IntMatrix2D(2,-1,  0,2,  0,1,  2);
    }

    static readonly List<Hexside> HexsideList;
    static readonly ICoords _EmptyCanon = HexCoords.NewCanonCoords(0,0);
    static readonly ICoords _EmptyUser  = HexCoords.NewUserCoords(0,0);
    public static   ICoords EmptyCanon { get { return _EmptyCanon; } }
    public static   ICoords EmptyUser  { get { return _EmptyUser; } }

    public static ICoords NewCanonCoords (IntVector2D vector){ return new HexCoords(true, vector); }
    public static ICoords NewUserCoords  (IntVector2D vector){ return new HexCoords(false,vector); }
    public static ICoords NewCanonCoords (int x, int y) { return new HexCoords(true, x,y); }
    public static ICoords NewUserCoords  (int x, int y) { return new HexCoords(false,x,y); }
    #endregion

    #region Constructors
    protected HexCoords(bool isCanon, int x, int y) : this(isCanon, new IntVector2D(x,y)) {}
    protected HexCoords(bool isCanon, IntVector2D vector) {
      if (isCanon) { _vectorCanon = vector;  _vectorUser = null;   }
      else         { _vectorCanon = null;    _vectorUser = vector; }
    }
    #endregion

    #region ICoords implementation
    IntVector2D ICoords.User   { get { return VectorUser;   } }
    IntVector2D ICoords.Canon  { get { return VectorCanon;  } }

    int     ICoords.Range(ICoords coords)    { return Range(coords); }
    ICoords ICoords.StepOut(Hexside hexside) {
      //switch(hexside) {
      //  case Hexside.NorthWest: return StepOut(NewCanonCoords(-1,-1));
      //  case Hexside.North:     return StepOut(NewCanonCoords( 0,-1));
      //  case Hexside.NorthEast: return StepOut(NewCanonCoords( 1, 0));
      //  case Hexside.SouthEast: return StepOut(NewCanonCoords( 1, 1));
      //  case Hexside.South:     return StepOut(NewCanonCoords( 0, 1));
      //  case Hexside.SouthWest: return StepOut(NewCanonCoords(-1, 0));
      //  default:                throw new ArgumentOutOfRangeException();
      //}
      switch(hexside) {
        case Hexside.NorthWest: return NewCanonCoords(VectorCanon + vectorNW);
        case Hexside.North:     return NewCanonCoords(VectorCanon + vectorN );
        case Hexside.NorthEast: return NewCanonCoords(VectorCanon + vectorNE);
        case Hexside.SouthEast: return NewCanonCoords(VectorCanon + vectorSE);
        case Hexside.South:     return NewCanonCoords(VectorCanon + vectorS );
        case Hexside.SouthWest: return NewCanonCoords(VectorCanon + vectorSW);
        default:                throw new ArgumentOutOfRangeException();
      }
    }

    internal static readonly IntVector2D vectorNW = new IntVector2D(-1,-1);
    internal static readonly IntVector2D vectorN  = new IntVector2D( 0,-1);
    internal static readonly IntVector2D vectorNE = new IntVector2D( 1, 0);
    internal static readonly IntVector2D vectorSE = new IntVector2D( 1, 1);
    internal static readonly IntVector2D vectorS  = new IntVector2D( 0, 1);
    internal static readonly IntVector2D vectorSW = new IntVector2D(-1, 0);

    string  ICoords.ToString()               { return ToString(); }

    IEnumerable<NeighbourCoords> ICoords.GetNeighbours(Hexside hexsides) { 
      return GetNeighbours(hexsides);
    }
    #endregion

    #region Conversions
    protected static IntMatrix2D MatrixUserToCanon;
    protected IntVector2D VectorCanon {
      get { return ( _vectorCanon.HasValue ? _vectorCanon 
                                           : _vectorCanon = VectorUser * MatrixUserToCanon
                   ).Value; }
    } protected Nullable<IntVector2D> _vectorCanon;

    protected static IntMatrix2D MatrixCanonToUser;
    protected IntVector2D VectorUser {
      get { return ( _vectorUser.HasValue  ? _vectorUser
                                           : _vectorUser = VectorCanon * MatrixCanonToUser
                   ).Value; }
    } protected Nullable<IntVector2D> _vectorUser;
    #endregion

    #region Value Equality
    bool IEquatable<HexCoords>.Equals(HexCoords rhs) { return this == rhs; }
    public override bool Equals(object rhs) { return (rhs is HexCoords) && this == (HexCoords)rhs; }
    public static bool operator == (HexCoords lhs, HexCoords rhs) { return lhs.VectorCanon.Equals(rhs.VectorCanon); }
    public static bool operator != (HexCoords lhs, HexCoords rhs) { return ! (lhs == rhs); }
    public override int GetHashCode() { return VectorUser.GetHashCode(); }

    bool IEqualityComparer<HexCoords>.Equals(HexCoords lhs, HexCoords rhs) { return lhs == rhs; }
    int  IEqualityComparer<HexCoords>.GetHashCode(HexCoords coords) { return coords.GetHashCode(); }
    #endregion

    protected IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides) {
      ICoords coords = this;
      foreach (var hexside in HexsideList.Where(h=>hexsides.HasFlag(h)))
        yield return new NeighbourCoords(hexside, coords.StepOut(hexside));
    }

    protected       int     Range(ICoords coords) { return Range(coords.Canon); }
    private         int     Range(IntVector2D vector) {
      var deltaX = vector.X - VectorCanon.X;
      var deltaY = vector.Y - VectorCanon.Y;
      return (Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY)) / 2;
    }
    //protected       ICoords StepOut(ICoords coords) { 
    //  return NewCanonCoords(VectorCanon + coords.Canon); 
    //}
    //protected       ICoords StepOut(int x, int y) {
    //  return NewCanonCoords(VectorCanon + new IntVector2D(x,y));
    //}

    /// <inheritDoc/>
    public override string  ToString() {
      return string.Format("User: {0}", VectorUser);
    }
  } 
}
