#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PGNapoleonics.HexUtilities {
    /// <summary>Row-major order integer implementation of an immutable 2D affine transformation matrix.</summary>
    /// <remarks> 
    /// Represents Points as row vectors and planes as column vectors.
    /// This representation is standard for computer graphics, though opposite 
    /// to standard mathematical (and physics) representation, and treats row 
    /// vectors as contravariant and column vectors as covariant.
    /// </remarks>
    [DebuggerDisplay("(({M11},{M12}), ({M21},{M22}), ({M31},{M32}), {M33}))")]
    public struct IntMatrix2D : IEquatable<IntMatrix2D>, IFormattable {
        static IntMatrix2D TransposeMatrix => new IntMatrix2D(0,1, 1,0);

        /// <summary>Returns the transpose of the supplied matrix.</summary>
        public static IntMatrix2D Transpose(IntMatrix2D matrix) => matrix * TransposeMatrix;

        #region Constructors
        /// <summary> Initializes a new <code>IntMatrix2D</code> as the translation defined by the vector vector.</summary>
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
            if (norm == 0) throw new ArgumentNullException(nameof(norm));
            M11 = m11;  M12 = m12;
            M21 = m21;  M22 = m22;
            M31 = dx;   M32 = dy;   M33 = norm;
        }
        #endregion

        #region Properties
        /// <summary>Get the i-scale component.</summary>
        public int M11 { get; }

        /// <summary>Get the X-shear component</summary>
        public int M12 { get; }

        /// <summary>Get the j-shear component</summary>
        public int M21 { get; }

        /// <summary>Get the Y-scale component</summary>
        public int M22 { get; }

        /// <summary>Get the i-translation component</summary>
        public int M31 { get; }

        /// <summary>Get the j-translationcomponent</summary>
        public int M32 { get; }

        /// <summary>Ge the normalization component</summary>
        public int M33 { get; }

        /// <summary>Get the identity @this.</summary>
        public static readonly IntMatrix2D Identity = new IntMatrix2D(1,0,0,1,0,0, 1);

        /// <summary>todo</summary>
        public int Determinant => M11*M22 - M21*M12;
        #endregion

        #region Operators
        /// <summary>(Contravariant) Vector transformation by a matrix.</summary>
        /// <param name="v">IntVector2D to be transformed.</param>
        /// <param name="m">IntMatrix2D to be applied.</param>
        /// <returns>New IntVector2D resulting from application of matrix <paramref name="m"/> to vector <paramref name="v"/>.</returns>
        public static IntVector2D operator * (IntVector2D v, IntMatrix2D m)
        => new IntVector2D (
                v.X * m.M11 + v.Y * m.M21 + m.M31,   v.X * m.M12 + v.Y * m.M22 + m.M32,  v.W * m.M33
           ).Normalize();
        
        /// <summary>(Covariant) Vector transformation by a matrix.</summary>
        /// <param name="m">IntMatrix2D to be applied.</param>
        /// <param name="v">IntVector2D to be transformed.</param>
        /// <returns>New IntVector2D resulting from application of matrix <paramref name="m"/> to vector <paramref name="v"/>.</returns>
        [Obsolete("The standard in PGNapoleonics is to use Contravariant (ie column) vectors.")]
        public static IntVector2D operator * (IntMatrix2D m, IntVector2D v)
        => new IntVector2D (
                v.X * m.M11 + v.Y * m.M12 + m.M31,   v.X * m.M21 + v.Y * m.M22 + m.M32,  v.W * m.M33
           ).Normalize();
        
        /// <summary>Matrix multiplication.</summary>
        /// <param name="m1">Prepended transformation.</param>
        /// <param name="m2">Appended transformation.</param>
        /// <returns></returns>
        public static IntMatrix2D operator * (IntMatrix2D m1, IntMatrix2D m2)
        => new IntMatrix2D (
                m1.M11*m2.M11 + m1.M12*m2.M21,           m1.M11*m2.M12 + m1.M12*m2.M22,
                m1.M21*m2.M11 + m1.M22*m2.M21,           m1.M21*m2.M12 + m1.M22*m2.M22,
                m1.M31*m2.M11 + m1.M32*m2.M21 + m2.M31,  m1.M31*m2.M12 + m1.M32*m2.M22 + m2.M32,  m1.M33 * m2.M33
           );
        
        /// <summary>(Contravariant) Vector transformation by a matrix.</summary>
        public static IntVector2D Multiply(IntVector2D v, IntMatrix2D m) => v * m;
        
        /// <summary>(Covariant) Vector transformation by a matrix.</summary>
        public static IntMatrix2D Multiply(IntMatrix2D m1, IntMatrix2D m2) => m1 * m2;
        #endregion

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is IntMatrix2D other) && this.Equals(other);
       
        /// <inheritdoc/>
        public bool Equals(IntMatrix2D other)
        => other.M33*M11 == M33*other.M11  &&  other.M33*M12 == M33*other.M12
        && other.M33*M21 == M33*other.M21  &&  other.M33*M22 == M33*other.M22
        && other.M33*M31 == M33*other.M31  &&  other.M33*M32 == M33*other.M32;

        /// <inheritdoc/>
        public override int GetHashCode() => ((2*M11/M33)<<13) + (2*M12/M33)
                                           ^ ((2*M21/M33)<<14) + ((2*M22/M33)<<1)
                                           ^ ((2*M31/M33)<<15) + ((2*M32/M33)<<2);

        /// <summary>Tests value-inequality.</summary>
        public static bool operator != (IntMatrix2D lhs, IntMatrix2D rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator == (IntMatrix2D lhs, IntMatrix2D rhs) => lhs.Equals(rhs);
        #endregion

        /// <summary>Returns a string representation of this <see cref="IntMatrix2D"/> in the Invariant Culture.</summary>
        public override string ToString() => ToString("G", CultureInfo.InvariantCulture);

        /// <summary>Returns a string representation of this <see cref="IntMatrix2D"/>.</summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        public string ToString(string format, IFormatProvider formatProvider)
        => string.Format(CultureInfo.CurrentCulture,"(({0},{1}), ({2},{3}), ({4},{5}), {6})",  
            M11.ToString(format,formatProvider), M12.ToString(format,formatProvider),
            M21.ToString(format,formatProvider), M22.ToString(format,formatProvider),
            M31.ToString(format,formatProvider), M32.ToString(format,formatProvider), M33.ToString(format,formatProvider));
    }
}
