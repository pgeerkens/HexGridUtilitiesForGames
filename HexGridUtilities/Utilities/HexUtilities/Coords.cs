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
  public enum CoordsType { Canon, User, Custom }

  public abstract partial class Coords : ICoordsCanon, ICoordsUser, ICoordsCustom,
    IEquatable<Coords>, IEqualityComparer<Coords> {
    #region Constructors
    protected Coords(CoordsType coordsType, IntVector2D vector) {
      switch(coordsType) {
        default:
        case CoordsType.Canon:   _vectorCanon  = vector;  isUserNull   = isCustomNull = true;  break;
        case CoordsType.User:    _vectorUser   = vector;  isCustomNull = isCanonNull  = true;  break;
        case CoordsType.Custom:  _vectorCustom = vector;  isCanonNull  = isUserNull   = true;  break;
      }
    }
    #endregion

    /// <inheritDoc/>
    /// <remarks>Prefers Custom over User over Canon in choosing which values to present.</remarks>
    public override string ToString() {
      return ! isCustomNull ? string.Format("Custom: {0}", VectorCustom)
                            : string.Format("User: {0}",   VectorUser);
    }

    #region Value Equality
    bool IEquatable<Coords>.Equals(Coords rhs) { return this == rhs; }
    public override bool Equals(object rhs) { return (rhs is Coords) && this == (Coords)rhs; }
    public static bool operator == (Coords lhs, Coords rhs) { return lhs.VectorCanon.Equals(rhs.VectorCanon); }
    public static bool operator != (Coords lhs, Coords rhs) { return ! (lhs == rhs); }
    public override int GetHashCode() { return VectorUser.GetHashCode(); }

    bool IEqualityComparer<Coords>.Equals(Coords lhs, Coords rhs) { return lhs == rhs; }
    int  IEqualityComparer<Coords>.GetHashCode(Coords coords) { return coords.GetHashCode(); }
    #endregion

    #region Conversions
    protected static IntMatrix2D MatrixUserToCanon;
    protected IntVector2D VectorCanon {
      get { return !isCanonNull ? _vectorCanon : VectorUser * MatrixUserToCanon; }
      set { _vectorCanon = value;  isUserNull = isCustomNull = true; }
    } IntVector2D _vectorCanon;
    bool isCanonNull;

    protected static IntMatrix2D MatrixCanonToUser;
    protected IntVector2D VectorUser {
      get { return !isUserNull  ? _vectorUser 
                 : !isCanonNull ? VectorCanon  * MatrixCanonToUser
                                : VectorCustom * MatrixCustomToUser; }
      set { _vectorUser  = value;  isCustomNull = isCanonNull = true; }
    } IntVector2D _vectorUser;
    bool isUserNull;

    protected static IntMatrix2D MatrixCustomToUser;
    protected static IntMatrix2D MatrixUserToCustom;
    protected IntVector2D VectorCustom {
      get { return !isCustomNull ? _vectorCustom : VectorUser * MatrixUserToCustom; }
      set { _vectorCustom  = value;  isCanonNull = isUserNull = true; }
    } IntVector2D _vectorCustom;
    bool isCustomNull;
    #endregion

    protected abstract IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);
    protected abstract int Range(ICoordsCanon coords);
    protected abstract ICoordsCanon StepOut(IntVector2D vector);
  }
}
