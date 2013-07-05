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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  /// <summary>Coordinate structure for hexagonal grids that abstracts the distinction 
  /// between rectangular (User) and canonical (Canon) bases (basis vectors, or reference 
  /// frame).</summary>
  /// <remarks>
  /// An obtuse reference frmae, with basis vectors at 120 degrees, eases most grid 
  /// calculations and movement operations; a rectangular reference frmae is easier for 
  /// most user interactions, and optimal for board storage. This structure hides the
  /// distinction betwene them, and automatically converting from one to the other on 
  /// demand (and caching the result).
  /// </remarks>
  [DebuggerDisplay("User: {User}")]
  public struct HexCoords : IEquatable<HexCoords>, IEqualityComparer<HexCoords> {
    #region static members
    public static HexCoords EmptyCanon { get { return _EmptyCanon; } }
    public static HexCoords EmptyUser  { get { return _EmptyUser; } }

    public static HexCoords NewCanonCoords (IntVector2D vector){ return new HexCoords(true, vector); }
    public static HexCoords NewUserCoords  (IntVector2D vector){ return new HexCoords(false,vector); }
    public static HexCoords NewCanonCoords (int x, int y) { return new HexCoords(true, x,y); }
    public static HexCoords NewUserCoords  (int x, int y) { return new HexCoords(false,x,y); }

    static readonly IntMatrix2D MatrixUserToCanon = new IntMatrix2D(2, 1,  0,2,  0,0,  2);
    static readonly IntMatrix2D MatrixCanonToUser = new IntMatrix2D(2,-1,  0,2,  0,1,  2);

    static readonly HexCoords _EmptyCanon  = HexCoords.NewCanonCoords(0,0);
    static readonly HexCoords _EmptyUser   = HexCoords.NewUserCoords(0,0);

    static readonly IntVector2D vectorN  = new IntVector2D( 0,-1);
    static readonly IntVector2D vectorNE = new IntVector2D( 1, 0);
    static readonly IntVector2D vectorSE = new IntVector2D( 1, 1);
    static readonly IntVector2D vectorS  = new IntVector2D( 0, 1);
    static readonly IntVector2D vectorSW = new IntVector2D(-1, 0);
    static readonly IntVector2D vectorNW = new IntVector2D(-1,-1);

    static readonly IntVector2D[] HexsideVectors = new IntVector2D[] {
      vectorN,  vectorNE, vectorSE, vectorS,  vectorSW, vectorNW
    };
    #endregion

    #region Constructors
    private HexCoords(bool isCanon, int x, int y) : this(isCanon, new IntVector2D(x,y)) {}
    private HexCoords(bool isCanon, IntVector2D vector) : this() {
      if (isCanon) { Canon = vector; userHasValue  = false; }
      else         { User  = vector; canonHasValue = false; }
    }
    #endregion

    #region Properties
    public  IntVector2D Canon {
      get { return canonHasValue ? _Canon : ( Canon = _User * MatrixUserToCanon); }
      set { _Canon = value; canonHasValue = true; userHasValue = false; }
    } private IntVector2D _Canon;
    bool canonHasValue;

    public  IntVector2D User  {
      get { return userHasValue ? _User : ( User = _Canon * MatrixCanonToUser); }
      set { _User = value;  userHasValue = true; canonHasValue = false; }
    } private IntVector2D _User;
    bool userHasValue;
    #endregion

    #region Methods
    /// <summary>Returns an <c>HexCoords</c> for the hex in direction <c>hexside</c> from this one.</summary>
    public HexCoords GetNeighbour(Hexside hexside) {
      return NewCanonCoords(Canon + HexsideVectors[(int)hexside]); 
    }

    ///<summary>Returns all neighbouring hexes as IEnumerable.</summary>
    public IEnumerable<NeighbourCoords> GetNeighbours() { 
      for (var hexside=0; hexside<HexsideVectors.Length; hexside++)
        yield return new NeighbourCoords(NewCanonCoords(Canon + HexsideVectors[hexside]),
                                        (Hexside)hexside); 
      //foreach (var hexside in HexExtensions.HexsideList)
      //  yield return new NeighbourCoords(GetNeighbour(hexside), hexside);
    }

    ///<summary>Returns set of hexes at direction(s) specified by <c>hexsides</c>, as IEnumerable.</summary>
    public IEnumerable<NeighbourCoords> GetNeighbours(HexsideFlags hexsides) { 
      return GetNeighbours().Where(n=>hexsides.HasFlag(n.Hexside));
    }

    /// <summary>Modified <i>Manhattan</i> distance of supplied coordinate from this one.</summary>
    public int       Range(HexCoords coords) { 
      var deltaX = coords.Canon.X - Canon.X;
      var deltaY = coords.Canon.Y - Canon.Y;
      return ( Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY) ) / 2;
    }

    /// <inheritDoc/>
    public override string ToString() { return string.Format(CultureInfo.InvariantCulture,
      "User: {0}", User); }
    #endregion

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      return obj is HexCoords  &&  User == ((HexCoords)obj).User;
    }
    /// <inheritdoc/>
    public override int GetHashCode() { return User.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(HexCoords other) { return User == other.User; }

    public static bool operator != (HexCoords lhs, HexCoords rhs) { return ! (lhs==rhs); }
    public static bool operator == (HexCoords lhs, HexCoords rhs) { return lhs.User == rhs.User; }

    /// <inheritdoc/>
    bool IEqualityComparer<HexCoords>.Equals(HexCoords lhs, HexCoords rhs) { return lhs.User == rhs.User; }

    /// <inheritdoc/>
    int  IEqualityComparer<HexCoords>.GetHashCode(HexCoords coords) { return coords.GetHashCode(); }
    #endregion
  } 
}
