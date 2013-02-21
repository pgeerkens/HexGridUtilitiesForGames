#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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

    public int X { get { return Vector[0]; } set { Vector[0] = value; } }
    public int Y { get { return Vector[1]; } set { Vector[1] = value; } }

    public IntVector2D(Point p)       : this(p.X,p.Y) {}
    public IntVector2D(Size s)        : this(s.Width, s.Height) {}
    public IntVector2D(IntVector2D v) : this(v.X, v.Y) {}
    public IntVector2D(int x, int y) {
      Vector = new int[3];
      Vector[0] = x;
      Vector[1] = y;
      Vector[2] = 1;
    }

    #region Scalar operators
    /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
    public static IntVector2D operator * (int s, IntVector2D v) { return v * s; }
    public static IntVector2D operator * (IntVector2D v, int s) {
      return new IntVector2D(v.X*s, v.Y*s);
    }
    /// <summary>Scalar Division into a new IntegerVector2D.</summary>
    public static IntVector2D operator / (IntVector2D v, int s) {
      return new IntVector2D((int)Math.Floor(v.X/(float)s), (int)Math.Floor(v.Y/(float)s));
    }

    #endregion

    #region Vector operators
    /// <summary>Scalar Product of two IntegerVector2D as an Int32.</summary>
   public static int operator * (IntVector2D v1, IntVector2D v2) {
      return v1.X*v2.X + v1.Y*v2.Y;
    }
    /// <summary>Vector Addition of two IntegerVector2D as a new IntegerVector2D.</summary>
   public static IntVector2D operator + (IntVector2D v1, IntVector2D v2) {
      return new IntVector2D(v1.X+v2.X, v1.Y+v2.Y);
    }
    /// <summary>Vector Subtraction of two IntegerVector2D as a new IntegerVector2D</summary>
    public static IntVector2D operator - (IntVector2D v1, IntVector2D v2) {
      return new IntVector2D(v1.X-v2.X, v1.Y-v2.Y);
    }
    #endregion

    #region Coords-vector operators
    public static ICoordsCanon operator + (ICoordsCanon coords, IntVector2D vector) {
      return Coords.NewCanonCoords(coords.Vector.X + vector.X, coords.Vector.Y + vector.Y);
    }
    public static ICoordsUser operator + (ICoordsUser coords, IntVector2D vector) {
      return Coords.NewCanonCoords(coords.Vector.X + vector.X, coords.Vector.Y + vector.Y).User;
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
      return (lhs.X == rhs.X) && (lhs.Y == rhs.Y);
    }
    public override int GetHashCode() { return X ^ Y; }
    #endregion

    public override string ToString() { return string.Format("({0,3},{1,3})",X,Y); }

    private int[] Vector;
  }
}
