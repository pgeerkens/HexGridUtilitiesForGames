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
  public class HexCoords : ICoords,IEquatable<HexCoords>,IEqualityComparer<HexCoords> {
    #region static members
    public static ICoords EmptyCanon { get { return _EmptyCanon; } }
    public static ICoords EmptyUser  { get { return _EmptyUser; } }

    public static ICoords NewCanonCoords (IntVector2D vector){ return new HexCoords(true, vector); }
    public static ICoords NewUserCoords  (IntVector2D vector){ return new HexCoords(false,vector); }
    public static ICoords NewCanonCoords (int x, int y) { return new HexCoords(true, x,y); }
    public static ICoords NewUserCoords  (int x, int y) { return new HexCoords(false,x,y); }

    static readonly IntMatrix2D MatrixUserToCanon = new IntMatrix2D(2, 1,  0,2,  0,0,  2);
    static readonly IntMatrix2D MatrixCanonToUser = new IntMatrix2D(2,-1,  0,2,  0,1,  2);

    static readonly ICoords _EmptyCanon  = HexCoords.NewCanonCoords(0,0);
    static readonly ICoords _EmptyUser   = HexCoords.NewUserCoords(0,0);

    static readonly IntVector2D vectorNW = new IntVector2D(-1,-1);
    static readonly IntVector2D vectorN  = new IntVector2D( 0,-1);
    static readonly IntVector2D vectorNE = new IntVector2D( 1, 0);
    static readonly IntVector2D vectorSE = new IntVector2D( 1, 1);
    static readonly IntVector2D vectorS  = new IntVector2D( 0, 1);
    static readonly IntVector2D vectorSW = new IntVector2D(-1, 0);

    static readonly IntVector2D[] HexsideVectors = new IntVector2D[] {
      vectorN,  vectorNE, vectorSE, vectorS,  vectorSW, vectorNW
    };
    #endregion

    #region Constructors
    protected HexCoords(bool isCanon, int x, int y) : this(isCanon, new IntVector2D(x,y)) {}
    protected HexCoords(bool isCanon, IntVector2D vector) {
      if (isCanon) { _Canon = vector;  _User = null;   }
      else         { _Canon = null;    _User = vector; }
    }
    #endregion

    #region ICoords implementation
    /// <inheritDoc/>
    IntVector2D ICoords.User   { get { return User;   } }
    /// <inheritDoc/>
    IntVector2D ICoords.Canon  { get { return Canon;  } }

    /// <inheritDoc/>
    int     ICoords.Range(ICoords coords)    { return Range(coords); }

    /// <inheritDoc/>
    ICoords ICoords.StepOut(Hexside hexside) {
      return NewCanonCoords(Canon + HexsideVectors[(int)hexside]); 
    }

    /// <inheritDoc/>
    string  ICoords.ToString()               { return ToString(); }

    /// <inheritDoc/>
    IEnumerable<NeighbourCoords> ICoords.GetNeighbours(HexsideFlags hexsides) { 
      return ((ICoords)this).GetNeighbours().Where(n=>hexsides.HasFlag(n.Direction));
    }
    IEnumerable<NeighbourCoords> ICoords.GetNeighbours() { 
      foreach (var hexside in HexExtensions.HexsideList)
        yield return new NeighbourCoords(((ICoords)this).StepOut(hexside), hexside);
    }

    //bool IEquatable<ICoords>.Equals(ICoords rhs) { return this.Equals(rhs); }
    //bool IEqualityComparer<ICoords>.Equals(ICoords lhs,ICoords rhs) { 
    //  return lhs != null  &&  lhs.Equals(rhs); 
    //}
    //int  IEqualityComparer<ICoords>.GetHashCode(ICoords @this) { 
    //  return ((HexCoords)@this).GetHashCode(); 
    //}
    #endregion

    #region Conversions
    protected IntVector2D Canon {
      get { return ( _Canon.HasValue ? _Canon 
                                     : _Canon = User * MatrixUserToCanon
                   ).Value; }
    } protected Nullable<IntVector2D> _Canon;
    protected IntVector2D User  {
      get { return ( _User.HasValue  ? _User
                                     : _User = Canon * MatrixCanonToUser
                   ).Value; }
    } protected Nullable<IntVector2D> _User;
    #endregion

    /// <summary>Manhattan distance of supplied coordinate from this one.</summary>
    protected       int    Range(ICoords coords) { 
      var deltaX = coords.Canon.X - Canon.X;
      var deltaY = coords.Canon.Y - Canon.Y;
      return (Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY)) / 2;
    }

    /// <inheritDoc/>
    public override string ToString() { return string.Format("User: {0}", User); }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      return obj is HexCoords  &&  Equals((HexCoords)obj);
    }
    /// <inheritdoc/>
    public override int GetHashCode() { return User.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(HexCoords rhs) { 
      return User.Equals(rhs.User);
    }
    /// <inheritdoc/>
    public bool Equals(HexCoords lhs, HexCoords rhs) { 
      return lhs.Equals(rhs); 
    }
    /// <inheritdoc/>
    public int  GetHashCode(HexCoords @this) { 
      return @this.GetHashCode(); 
    }

    public static bool operator != (HexCoords lhs, HexCoords rhs) { return ! (lhs==rhs); }
    public static bool operator == (HexCoords lhs, HexCoords rhs) {
      return lhs != null  &&  lhs.Equals(rhs);
    }
    #endregion
  } 
}
