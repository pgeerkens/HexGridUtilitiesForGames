#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;
using System.Globalization;

namespace PGNapoleonics.HexUtilities {
    using HexPoint = System.Drawing.Point;
    using HexSize  = System.Drawing.Size;

    /// <summary>Representation of an immutable integer 2D vector.</summary>
    [DebuggerDisplay("({X},{Y},{W})")]
    public struct IntVector2D : IEquatable<IntVector2D>, IFormattable {
        /// <summary>Returns the origin vector.</summary>
        public static IntVector2D Empty => new IntVector2D(0,0);

        #region Constructors
        /// <summary>Construct a new instance at offset (x,y) from the origin with norm of 1.</summary>
        /// <param name="x">The horizontal coordinate of the offset for this displacement.</param>
        /// <param name="y">The vertical coordinate of the offset for this displacement.</param>
        public IntVector2D(int x, int y) : this(x,y,1) { }

        /// <summary>Construct a new instance at offset (x,y) from the origin with norm <paramref name="norm"/>.</summary>
        /// <param name="x">The horizontal coordinate of the offset for this displacement.</param>
        /// <param name="y">The vertical coordinate of the offset for this displacement.</param>
        /// <param name="norm">The 'norm' of this augmented affine vector.</param>
        internal IntVector2D(int x, int y, int norm) : this() {
            X = x;
            Y = y;
            W = norm;
        }
        #endregion

        #region Properties
        /// <summary>The horizontal component of the vector..</summary>
        public int X { get; }
        /// <summary>The vertical component of the vector..</summary>
        public int Y { get; }
        /// <summary>Get the w-component (ie scale factor or norm) of the vector.</summary>
        public int W { get; }
        #endregion

        /// <summary>Returns a new instance with coordinates normalized using integer arithmetic.</summary>
        public IntVector2D Normalize() {
            switch (W) {
                case 1:   return this;
                case 2:   return new IntVector2D(X / 2, Y / 2);
                case 4:   return new IntVector2D(X / 4, Y / 4);
                case 8:   return new IntVector2D(X / 8, Y / 8);

                    // Wow! All this because integer division wasn't defined to be a field operator.
                default:  return new IntVector2D(Math.Sign(X)*Math.Sign(W)*Math.Abs(X)/Math.Abs(W),
                                                 Math.Sign(Y)*Math.Sign(W)*Math.Abs(Y)/Math.Abs(W));
            }
        }

        #region Scalar operators
        /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
        public static IntVector2D operator *(int s,IntVector2D v) => v * s;

        /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
        public static IntVector2D operator *(IntVector2D v,int s) => new IntVector2D(v.X*s,v.Y*s);

        /// <summary>Scalar Division into a new IntegerVector2D.</summary>
        public static IntVector2D operator /(IntVector2D v,int s) => DivideInner(v,s);

        /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
        public static IntVector2D Multiply(int s,IntVector2D v) => v * s;

        /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
        public static IntVector2D Multiply(IntVector2D v,int s) => v * s;

        /// <summary>Scalar Division into a new IntegerVector2D.</summary>
        public static IntVector2D Divide(IntVector2D v,int s) => DivideInner(v,s);

        private static IntVector2D DivideInner(IntVector2D v,int s) => new IntVector2D(v.X/s,v.Y/s);
        #endregion

        #region Vector operators
        /// <summary>Scalar (Inner, or Dot) Product of two <code>IntVector2D</code> as an Int32.</summary>
        public static int operator * (IntVector2D v1, IntVector2D v2) => v1.X*v2.X + v1.Y*v2.Y;

        /// <summary>Z component of the 'Vector'- or Cross-Product of two <code>IntVector2D</code>s</summary>
        /// <returns>A pseudo-scalar (it reverses sign on exchange of its arguments).</returns>
        public static int operator ^(IntVector2D v1,IntVector2D v2) => v1.X*v2.Y - v1.Y*v2.X;

        /// <summary>Vector Addition of two <code>IntVector2D</code> as a new <code>IntVector2D</code>.</summary>
        public static IntVector2D operator +(IntVector2D v1,IntVector2D v2) => new IntVector2D(v1.X+v2.X,v1.Y+v2.Y);

        /// <summary>Vector Subtraction of two <code>IntVector2D</code> as a new <code>IntVector2D</code></summary>
        public static IntVector2D operator -(IntVector2D v1,IntVector2D v2) => new IntVector2D(v1.X-v2.X,v1.Y-v2.Y);

        /// <summary>Vector Addition of two <code>IntVector2D</code> as a new <code>IntVector2D</code>.</summary>
        public static IntVector2D Add(IntVector2D v1,IntVector2D v2) => v1 + v2;

        /// <summary>Vector Subtraction of two <code>IntVector2D</code> as a new <code>IntVector2D</code></summary>
        public static IntVector2D Subtract(IntVector2D v1,IntVector2D v2) => v1 - v2;

        /// <summary>Returns the vector cross-product of v1 and v2.</summary>
        public static int CrossProduct(IntVector2D v1,IntVector2D v2) => v1 ^ v2;

        /// <summary>Returns the inner- / scalar / dot-product of v1 and v2.</summary>
        public static int InnerProduct(IntVector2D v1,IntVector2D v2) => v1 * v2;

        /// <summary>Obsolete - use InnerProduct operator instead.</summary>
        [Obsolete("Deprecated (as really confusing) - use InnerProduct instead.",true)]
        public static int Xor (IntVector2D v1, IntVector2D v2) => InnerProduct(v1, v2);
        #endregion

        #region Casts
        /// <summary>Returns a new instance initialized from point.</summary>
        public static implicit operator IntVector2D(HexPoint point)
        => new IntVector2D(point.X,point.Y);

        /// <summary>Returns a new instance initialized from size.</summary>
        public static implicit operator IntVector2D (HexSize size)
        => new IntVector2D(size.Width, size.Height);

        /// <summary>Returns a new Point instance initialized from vector.</summary>
        public static implicit operator HexPoint (IntVector2D vector)
        => new HexPoint(vector.X, vector.Y);

        /// <summary>Returns a new Size instance initialized from vector.</summary>
        public static implicit operator HexSize (IntVector2D vector)
        => new HexSize(vector.X, vector.Y);
        #endregion

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is IntVector2D other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(IntVector2D other) 
        => (other.W*X == W*other.X)  &&  (other.W*Y == W*other.Y);

        /// <inheritdoc/>
        /// <remarks>
        /// Maps the components into 32 bits as "00xx-xxxx xxxx-xxxx xyyy-yyyy yyyy-yyyy"
        /// for expected ranges of X and Y.
        /// Doubling X and Y ensures that adjacent hexes get different hashcodes.
        /// </remarks>
        public override int GetHashCode() => ( (2*X/W) <<15 ) + (2*Y/W) ;

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(IntVector2D lhs, IntVector2D rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(IntVector2D lhs, IntVector2D rhs) => lhs.Equals(rhs);
        #endregion

        /// <summary>Culture-invariant string representation of this instance's value.</summary>
        public override string ToString() => ToString("G", CultureInfo.InvariantCulture);

        /// <summary>Converts the value of this instance to its equivalent string representation using the 
        /// specified format and culture-specific format information.</summary>
        /// <param name="format">Type: System.String. 
        /// 
        /// > A standard or custom numeric format string.</param>
        /// <param name="formatProvider">Type: IFormatProvider - 
        /// 
        /// > An object that supplies culture-specific formatting information.</param>
        /// <remarks>Format characters:
        /// - 'V' or 'v': Vector formatting - Vector output like (nn,mm);
        /// - 'G' or 'g': General formatting - same as 'V' or 'vector';  
        /// - 'I' or 'i': 2-Dimensional  vector formatting as 11I + 22J;
        /// - 'W' or 'w': 3-Dimensional  vector formatting as 11I + 22J = 33K;
        /// In all cases the leading character of the format string is stripped off and parsed, 
        /// with the remainder passed to the formatter completing the display formatting.
        /// 
        /// For the 2-D and 3-D vector formatting (ie 'I', 'i', 'W', or 'w'), the case of the 
        /// unit vectors follows that of the supplied formatting command character.
        /// </remarks>
        public string ToString(string format, IFormatProvider formatProvider) {
            if (format==null || format.Length==0 || char.IsDigit(format[0])) format = "G";
            var formatChar = format[0];
            format = "D" + format.Substring(1);
            string layout;
            switch(formatChar) {
                default:    throw new FormatException();
                case 'G':
                case 'g': 
                case 'V':
                case 'v':   layout = "({0}, {1})"; break;
                case 'I':   layout = "{0}I + {1}J"; break;
                case 'i':   layout = "{0}i + {1}j"; break;
                case 'W':   layout = "{0}I + {1}J + {2}K"; break;
                case 'w':   layout = "{0}i + {1}j + {2}k"; break;
              }
          return string.Format(CultureInfo.CurrentCulture,layout, X.ToString(format,formatProvider),
                                                                  Y.ToString(format,formatProvider),
                                                                  W.ToString(format,formatProvider));
        }
    }
}
