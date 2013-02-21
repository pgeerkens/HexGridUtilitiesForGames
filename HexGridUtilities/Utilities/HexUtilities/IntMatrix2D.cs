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
  /// <summary>Row-major order representation of an immutable integer matrix.</summary>
  /// <remarks> Represents Points as row vectors and planes as column vectors.
  /// This representation is standard for computer graphics, though opposite 
  /// to standard mathematical (and physics) representation, and treats 
  /// row vectors as contravariant and column vectors as covariant.</remarks>
  public struct IntMatrix2D : IEquatable<IntMatrix2D> {
    public int M11 { get { return Matrix[0,0]; } }
    public int M12 { get { return Matrix[0,1]; } }
    public int M21 { get { return Matrix[1,0]; } }
    public int M22 { get { return Matrix[1,1]; } }
    public int M31 { get { return Matrix[2,0]; } }
    public int M32 { get { return Matrix[2,1]; } }
    public static IntMatrix2D Identity { get { return _identity; } }
    static IntMatrix2D _identity = new IntMatrix2D(1,0,0,1,0,0);

    #region Constructors
    /// <summary> Initializes a new IntegerMatrix as the translation defined by the vector v.</summary>
    /// <param name="v">the translation vector</param>
    public IntMatrix2D(IntVector2D v)  : this(1,0, 0,1, v.X,v.Y) {}
    /// <summary> Initializes a new IntegerMatrix as the translation (dx,dy).</summary>
    /// <param name="dx">X-translate component</param>
    /// <param name="dy">Y-translate component</param>
    public IntMatrix2D(int dx, int dy)  : this(1,0, 0,1, dx,dy) {}
    /// <summary> Initialies a new IntegerMatrix with a rotation.</summary>
    /// <param name="m00">X-scale component.</param>
    /// <param name="m10">Y-shear component</param>
    /// <param name="m01">X-shear component</param>
    /// <param name="m11">Y-scale component</param>
    public IntMatrix2D(int m11, int m12, int m21, int m22) : this(m11,m12, m21,m22, 0,0) {}
    /// <summary>Copy Constructor for a new IntegerMatrix.</summary>
    /// <param name="m">Source IntegerMatrix</param>
    public IntMatrix2D(IntMatrix2D m) : this(m.M11,m.M21, m.M12,m.M22, m.M31,m.M32) { }
    /// <summary>Initializes a new fully-specificed IntegerMatrix .</summary>
    /// <param name="m11">X-scale component.</param>
    /// <param name="m12">Y-shear component</param>
    /// <param name="m21">X-shear component</param>
    /// <param name="m22">Y-scale component</param>
    /// <param name="dx">X-translate component</param>
    /// <param name="dy">Y-translate component</param>
    public IntMatrix2D(int m11, int m12, int m21, int m22, int dx, int dy) : this() {
      Matrix = new int[3,2];
      Matrix[0,0] = m11;  Matrix[0,1] = m12; // Matrix[2,0] = 0;
      Matrix[1,0] = m21;  Matrix[1,1] = m22; // Matrix[2,1] = 0;
      Matrix[2,0] = dx;   Matrix[2,1] = dy;  // Matrix[2,2] = 1;
    }
    #endregion

    #region operators
    /// <summary>(Contravariant) Vector application.</summary>
    /// <param name="v">IntVector2D to be transformed.</param>
    /// <param name="m">IntMatrix2D to be applied.</param>
    /// <returns>New IntVector2D resulting from application of this matrix to vector v.</returns>
    public static IntVector2D operator * (IntVector2D v, IntMatrix2D m) {
      return new IntVector2D (
        v.X * m.M11 + v.Y * m.M21 + m.M31,      v.X * m.M12 + v.Y * m.M22 + m.M32
      );
    }
    /// <summary>Matrix multiplication.</summary>
    /// <param name="m1">Prepended transformation.</param>
    /// <param name="m2">Appended transformation.</param>
    /// <returns></returns>
    public static IntMatrix2D operator * (IntMatrix2D m1, IntMatrix2D m2) {
      return new IntMatrix2D (
        m1.M11*m2.M11 + m1.M12*m2.M21,          m1.M11*m2.M12 + m1.M12*m2.M22,
        m1.M21*m2.M11 + m1.M22*m2.M21,          m1.M21*m2.M12 + m1.M22*m2.M22,
        m1.M31*m2.M11 + m1.M32*m2.M21 + m2.M31, m1.M31*m2.M12 + m1.M32*m2.M22 + m2.M32
      );
    }
    #endregion

    /// <summary>Vector Rotation (only).</summary>
    /// <param name="v">IntVector2D to be rotated.</param>
    /// <returns>New IntVector2D resulting from rotaion (only) of vector v by this matrix 
    /// (ignoring any translation components of this matrix).</returns>
    public IntVector2D Rotate(IntVector2D v) {
      return new IntVector2D (v.X * M11 + v.Y * M12, v.X * M21 + v.Y * M22);
    }

    #region Value Equality
    public override bool Equals(object obj) { 
      return (obj is IntMatrix2D) && this.Equals((IntMatrix2D)obj); 
    }
    bool IEquatable<IntMatrix2D>.Equals(IntMatrix2D rhs)              { return this == rhs; }
    public static bool operator != (IntMatrix2D lhs, IntMatrix2D rhs) { return ! (lhs == rhs); }
    public static bool operator == (IntMatrix2D lhs, IntMatrix2D rhs) {
      return lhs.M11== rhs.M11 && lhs.M12 == rhs.M12
          && lhs.M21== rhs.M21 && lhs.M22 == rhs.M22
          && lhs.M31== rhs.M31 && lhs.M32 == rhs.M32;
    }
    public override int GetHashCode() { return M11 ^ M12 ^ M21 ^ M22 ^ M31 ^ M32; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() {
      return string.Format("(({0},{1]),({2},{3}),({4],{5}))",M11,M12,M21,M22,M31,M32);
    }

    private int [,] Matrix  { get; set; }
  }
}
