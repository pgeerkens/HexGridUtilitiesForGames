#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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
using System.Diagnostics;
using System.Globalization;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Row-major order representation of an immutable integer matrix.</summary>
  /// <remarks> Represents Points as row vectors and planes as column vectors.
  /// This representation is standard for computer graphics, though opposite 
  /// to standard mathematical (and physics) representation, and treats 
  /// row vectors as contravariant and column vectors as covariant.</remarks>
  [DebuggerDisplay("(({M11},{M12}), ({M21},{M22}), ({M31},{M32}), {M33}))")]
  public struct IntMatrix2D : IEquatable<IntMatrix2D>, IFormattable {
    static IntMatrix2D TransposeMatrix = new IntMatrix2D(0,1, 1,0);
    /// <summary>TODO</summary>
    public static IntMatrix2D Transpose(IntMatrix2D matrix) {
      return matrix * TransposeMatrix;
    }

    #region Properties
    /// <summary>Get the x-scale component.</summary>
    public int M11 { get; private set; }
    /// <summary>Get the X-shear component</summary>
    public int M12 { get; private set; }
    /// <summary>Get the y-shear component</summary>
    public int M21 { get; private set; }
    /// <summary>Get the Y-scale component</summary>
    public int M22 { get; private set; }
    /// <summary>Get the x-translation component</summary>
    public int M31 { get; private set; }
    /// <summary>Get the y-translationcomponent</summary>
    public int M32 { get; private set; }
    /// <summary>Ge the normalization component</summary>
    public int M33 { get; private set; }
    /// <summary>Get the identity matrix.</summary>
    public static readonly IntMatrix2D Identity = new IntMatrix2D(1,0,0,1,0,0, 1);
    #endregion

    #region Constructors
    /// <summary> Initializes a new <code>IntMatrix2D</code> as the translation defined by the vector v.</summary>
    /// <param name="vector">the translation vector</param>
    public IntMatrix2D(IntVector2D vector)  : this(1,0, 0,1, vector.X,vector.Y, 1) {}
    /// <summary> Initializes a new <code>IntMatrix2D</code> as the translation (dx,dy).</summary>
    /// <param name="dx">X-translate component</param>
    /// <param name="dy">Y-translate component</param>
    public IntMatrix2D(int dx, int dy)  : this(1,0, 0,1, dx,dy,1) {}
    /// <summary> Initialies a new <code>IntMatrix2D</code> with a rotation.</summary>
    /// <param name="m11">X-scale component.</param>
    /// <param name="m12">Y-shear component</param>
    /// <param name="m21">X-shear component</param>
    /// <param name="m22">Y-scale component</param>
    [SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
    public IntMatrix2D(int m11, int m12, int m21, int m22) : this(m11,m12, m21,m22, 0,0, 1) {}
    /// <summary>Copy Constructor for a new <code>IntMatrix2D</code>.</summary>
    /// <param name="m">Source IntegerMatrix</param>
    public IntMatrix2D(IntMatrix2D m) : this(m.M11,m.M21, m.M12,m.M22, m.M31,m.M32, m.M33) { }
    /// <summary>Initializes a new fully-specificed <code>IntMatrix2D</code> .</summary>
    /// <param name="m11">X-scale component.</param>
    /// <param name="m12">Y-shear component</param>
    /// <param name="m21">X-shear component</param>
    /// <param name="m22">Y-scale component</param>
    /// <param name="dx">X-translate component</param>
    /// <param name="dy">Y-translate component</param>
    public IntMatrix2D(int m11, int m12, int m21, int m22, int dx, int dy) : this(m11,m12,m21,m22,dx,dy,1) {}
    /// <summary>Initializes a new fully-specificed non-normed <code>IntMatrix2D</code>.</summary>
    /// <param name="m11">X-scale component.</param>
    /// <param name="m12">Y-shear component</param>
    /// <param name="m21">X-shear component</param>
    /// <param name="m22">Y-scale component</param>
    /// <param name="dx">X-translate component</param>
    /// <param name="dy">Y-translate component</param>
    /// <param name="norm">Normalization component</param>
    public IntMatrix2D(int m11, int m12, int m21, int m22, int dx, int dy, int norm) : this() {
//      if (norm <= 0) throw new ArgumentOutOfRangeException("norm", norm, "Parameter norm must be > 0.");
      if (norm <= 0) norm = 1;
      M11 = m11;  M12 = m12;
      M21 = m21;  M22 = m22;
      M31 = dx;   M32 = dy;   M33 = norm;
    }
    #endregion

    #region operators
    /// <summary>(Contravariant) Vector application.</summary>
    /// <param name="v">IntVector2D to be transformed.</param>
    /// <param name="m">IntMatrix2D to be applied.</param>
    /// <returns>New IntVector2D resulting from application of this matrix to vector v.</returns>
    public static IntVector2D operator * (IntVector2D v, IntMatrix2D m) {
      return new IntVector2D (
        v.X * m.M11 + v.Y * m.M21 + m.M31,   v.X * m.M12 + v.Y * m.M22 + m.M32,  v.W * m.M33
      ).Normalize();
    }
    /// <summary>Matrix multiplication.</summary>
    /// <param name="m1">Prepended transformation.</param>
    /// <param name="m2">Appended transformation.</param>
    /// <returns></returns>
    public static IntMatrix2D operator * (IntMatrix2D m1, IntMatrix2D m2) {
      return new IntMatrix2D (
        m1.M11*m2.M11 + m1.M12*m2.M21,           m1.M11*m2.M12 + m1.M12*m2.M22,
        m1.M21*m2.M11 + m1.M22*m2.M21,           m1.M21*m2.M12 + m1.M22*m2.M22,
        m1.M31*m2.M11 + m1.M32*m2.M21 + m2.M31,  m1.M31*m2.M12 + m1.M32*m2.M22 + m2.M32,  m1.M33 * m2.M33
      );
    }
    /// <summary>Returns the result of applying the (row) vector <c>v</c> to the matrix <c>m</c>.</summary>
    public static IntVector2D Multiply(IntVector2D v, IntMatrix2D m) { return v * m; }
    /// <summary>Returns the <c>IntMatrix2D</c> representing the transformation <c>m1</c> composed with <c>m2</c>.</summary>
    public static IntMatrix2D Multiply(IntMatrix2D m1, IntMatrix2D m2) { return m1 * m2; }
    #endregion

    /// <summary>Vector Rotation (only).</summary>
    /// <param name="v">IntVector2D to be rotated.</param>
    /// <returns>New IntVector2D resulting from rotaion (only) of vector v by this matrix 
    /// (ignoring any translation components of this matrix).</returns>
    public IntVector2D Rotate(IntVector2D v) {
      return new IntVector2D (v.X * M11 + v.Y * M12, v.X * M21 + v.Y * M22, v.W * M33).Normalize();
    }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      return (obj is IntMatrix2D) && this.Equals((IntMatrix2D)obj); 
    }
    /// <inheritdoc/>
    public bool          Equals(IntMatrix2D other)              { return this == other; }
    /// <inheritdoc/>
    public static bool operator != (IntMatrix2D lhs, IntMatrix2D rhs) { return ! (lhs == rhs); }
    /// <inheritdoc/>
    public static bool operator == (IntMatrix2D lhs, IntMatrix2D rhs) {
      return lhs.M11== rhs.M11 && lhs.M12 == rhs.M12
          && lhs.M21== rhs.M21 && lhs.M22 == rhs.M22
          && lhs.M31== rhs.M31 && lhs.M32 == rhs.M32 && lhs.M33 == rhs.M33;
    }
    /// <inheritdoc/>
    public override int GetHashCode() { return M11 ^ M12 ^ M21 ^ M22 ^ M31 ^ M32 ^ M33; }
    #endregion

    /// <inheritdoc/>
    public override string ToString() {
      return ToString("G", CultureInfo.InvariantCulture);
    }
    /// <summary>Returns a string representation of the matrix.</summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public string ToString(string format, IFormatProvider formatProvider) {
      return string.Format("(({0},{1}), ({2},{3}), ({4},{5}), {6})",  
        M11.ToString(format,formatProvider), M12.ToString(format,formatProvider),
        M21.ToString(format,formatProvider), M22.ToString(format,formatProvider),
        M31.ToString(format,formatProvider), M32.ToString(format,formatProvider), M33.ToString(format,formatProvider));
    }
  }
}
