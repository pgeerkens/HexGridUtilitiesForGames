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
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  /// <summary>Representation of an immutable integer 2D vector.</summary>
  public struct IntVector2D : IEquatable<IntVector2D> {
    public static readonly IntVector2D Empty = new IntVector2D(Point.Empty);

    public int X { get; private set; }
    public int Y { get; private set; }
    public int W { get; private set; }

    public IntVector2D(Point p)             : this(p.X, p.Y, 1) {}
    public IntVector2D(Size s)              : this(s.Width, s.Height, 1) {}
    public IntVector2D(IntVector2D v)       : this(v.X, v.Y, 1) {}
    public IntVector2D(int x, int y)        : this(x, y, 1) {}
    public IntVector2D(int x, int y, int w) : this() {
      X = x;
      Y = y;
      W = w;
    }

    public IntVector2D Normalize() {
      switch (W) {
        case 1:   return this;
        case 2:   return new IntVector2D(X >> 1, Y >> 1);
        case 4:   return new IntVector2D(X >> 2, Y >> 2);
        case 3:                                
        default:  var x = (X >= 0) ? X : X - W;
                  var y = (Y >= 0) ? Y : Y - W;
                  return new IntVector2D(x/W, y/W);
      }
    }

    #region Scalar operators
    /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
    public static IntVector2D operator * (int s, IntVector2D v) { return v * s; }
    public static IntVector2D operator * (IntVector2D v, int s) {
      return new IntVector2D(v.X*s, v.Y*s);
    }
    /// <summary>Scalar Division into a new IntegerVector2D.</summary>
    public static IntVector2D operator / (IntVector2D v, int i) {
      return new IntVector2D(v.X/i, v.Y/i);
    }
    /// <summary>Scalar Division into a new IntegerVector2D.</summary>
    public static IntVector2D operator / (IntVector2D v, float s) {
      return new IntVector2D((int)Math.Floor(v.X/(float)s), (int)Math.Floor(v.Y/(float)s));
    }
    #endregion

    #region Vector operators
    /// <summary>Scalar (Inner, or Dot) Product of two <code>IntVector2D</code> as an Int32.</summary>
   public static int operator * (IntVector2D v1, IntVector2D v2) {
      return v1.X*v2.X + v1.Y*v2.Y;
    }
    /// <summary>Z component of the 'Vector'- or Cross-Product of two <code>IntVector2D</code>s</summary>
    /// <returns>A pseudo-scalar (it reverses sign on exchange of its arguments).</returns>
    public static int operator ^ (IntVector2D v1, IntVector2D v2) {
      return v1.X*v2.Y - v1.Y*v2.X;
    }
    /// <summary>Vector Addition of two <code>IntVector2D</code> as a new <code>IntVector2D</code>.</summary>
   public static IntVector2D operator + (IntVector2D v1, IntVector2D v2) {
      return new IntVector2D(v1.X+v2.X, v1.Y+v2.Y);
    }
    /// <summary>Vector Subtraction of two <code>IntVector2D</code> as a new <code>IntVector2D</code></summary>
    public static IntVector2D operator - (IntVector2D v1, IntVector2D v2) {
      return new IntVector2D(v1.X-v2.X, v1.Y-v2.Y);
    }
    #endregion

    #region Coords-vector operators
    public static ICoordsCanon operator + (ICoordsCanon coords, IntVector2D vector) {
      return HexCoords.NewCanonCoords(coords.Vector.X + vector.X, coords.Vector.Y + vector.Y);
    }
    public static ICoordsUser operator + (ICoordsUser coords, IntVector2D vector) {
      return HexCoords.NewCanonCoords(coords.Vector.X + vector.X, coords.Vector.Y + vector.Y).User;
    }
    #endregion

    #region Casts
    public static implicit operator IntVector2D (Point p) { return new IntVector2D(p);  }
    public static implicit operator IntVector2D (Size s)  { return new IntVector2D(s);  }
    public static implicit operator Point (IntVector2D v) { return new Point(v.X, v.Y); }
    public static implicit operator Size (IntVector2D v)  { return new Size(v.X, v.Y);  }
    #endregion

    #region Value Equality
    public override bool Equals(object obj)              { 
      return (obj is IntVector2D) && this == (IntVector2D)obj; }
    bool IEquatable<IntVector2D>.Equals(IntVector2D rhs) { return this == rhs; }
    public static bool operator != (IntVector2D lhs, IntVector2D rhs) { return ! (lhs == rhs); }
    public static bool operator == (IntVector2D lhs, IntVector2D rhs) {
      return (lhs.X == rhs.X) && (lhs.Y == rhs.Y) && (lhs.W == rhs.W);
    }
    public override int GetHashCode() { return (X<<16) ^ Y ^ W; }
    #endregion

    public override string ToString() { return string.Format("({0,3},{1,3})",X,Y); }
  }
}
