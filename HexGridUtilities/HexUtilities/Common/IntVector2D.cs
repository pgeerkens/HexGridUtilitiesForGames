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
using System.Drawing;
using System.Globalization;

#pragma warning disable 1587
/// <summary>Shared technoloiges across the library, and useful gadgets.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Representation of an immutable integer 2D vector.</summary>
  [DebuggerDisplay("({X},{Y},{W})")]
  public struct IntVector2D : IEquatable<IntVector2D>, IFormattable {
    /// <summary>Returns the origin vector.</summary>
    public static readonly IntVector2D Empty = new IntVector2D(Point.Empty);

    #region Properties
    /// <summary>Get the x-component.</summary>
    public int X { get; private set; }
    /// <summary>Get the y-component.</summary>
    public int Y { get; private set; }
    /// <summary>Get the w-component (ie scale factor).</summary>
    public int W { get; private set; }
    #endregion

    #region Constructors
    /// <summary>Construct a new instance from <paramref name="point"/>.</summary>
    public IntVector2D(Point point)              : this(point.X, point.Y, 1) {}
    /// <summary>Construct a new instance from <paramref name="size"/>.</summary>
    public IntVector2D(Size size)                : this(size.Width, size.Height, 1) {}
    /// <summary>Construct a new instance from <paramref name="intVector2D"/>.</summary>
    public IntVector2D(IntVector2D intVector2D)  : this(intVector2D.X, intVector2D.Y, 1) {}
    /// <summary>Construct a new instance from <paramref name="x"/> and .</summary>
    public IntVector2D(int x, int y)             : this(x, y, 1) {}
    /// <summary>Construct a new instance from x, y, and w.</summary>
    internal IntVector2D(int x, int y, int norm) : this() {
//      if (norm <= 0) throw new ArgumentOutOfRangeException("norm", norm, "Parameter 'norm' must be > 0.");
      if (norm == 0) norm = 1;
      X = x;
      Y = y;
      W = norm;
    }
    #endregion

    /// <summary>Returns a new instance with coordinates normalized using integer arithmetic.</summary>
    public IntVector2D Normalize() {
      switch (W) {
        case 0:   throw new InvalidOperationException("IntVector2D is uninitialized.");
        case 1:   return this;
        case 2:   return new IntVector2D(X >> 1, Y >> 1);
        case 4:   return new IntVector2D(X >> 2, Y >> 2);
        case 8:   return new IntVector2D(X >> 3, Y >> 3);

        case 3:                                
        default:  var x = X; var i = 0; while(x<0) {i--; x+=W;} // TODO - Is there a better way?
                  var y = Y; var j = 0; while(y<0) {j--; y+=W;}
                  return new IntVector2D(i, j);
      }
    }

    #region Scalar operators
    /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
    public static IntVector2D operator * (int s, IntVector2D v) { return v * s; }
    /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
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
    /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
    public static IntVector2D Multiply (int s, IntVector2D v) { return v * s; }
    /// <summary>Scalar Multiplication into a new IntegerVector2D.</summary>
    public static IntVector2D Multiply (IntVector2D v, int s) { return v * s; }
    /// <summary>Scalar Division into a new IntegerVector2D.</summary>
    public static IntVector2D Divide (IntVector2D v, int i)   { return v / i; }
    /// <summary>Scalar Division into a new IntegerVector2D.</summary>
    public static IntVector2D Divide (IntVector2D v, float s) { return v / s; }
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
    /// <summary>Vector Addition of two <code>IntVector2D</code> as a new <code>IntVector2D</code>.</summary>
    public static IntVector2D Add  (IntVector2D v1, IntVector2D v2) { return v1 + v2; }
    /// <summary>Vector Subtraction of two <code>IntVector2D</code> as a new <code>IntVector2D</code></summary>
    public static IntVector2D Subtract (IntVector2D v1, IntVector2D v2) { return v1 - v2; }

    /// <summary>Returns the vector corss-product of v1 and v2.</summary>
    public static int CrossProduct (IntVector2D v1, IntVector2D v2) { return v1 ^ v2; }
    /// <summary>Returns the inner- / scalar / dot-product of v1 and v2.</summary>
    public static int InnerProduct (IntVector2D v1, IntVector2D v2) { return v1 * v2; }
    /// <summary>Obsolete - use InnerProduct operaotr instead.</summary>
    [Obsolete("Deprecated (as really confusing) - use InnerProduct instead.")]
    public static int Xor (IntVector2D v1, IntVector2D v2) { return InnerProduct(v1, v2); }

    #endregion

    #region Casts
    /// <summary>Returns a new instance initialized from point.</summary>
    public static implicit operator IntVector2D (Point point) { return new IntVector2D(point);  }
    /// <summary>Returns a new instance initialized from size.</summary>
    public static implicit operator IntVector2D (Size size)   { return new IntVector2D(size);  }
    /// <summary>Returns a new Point instance initialized from vector.</summary>
    public static implicit operator Point (IntVector2D vector) { return new Point(vector.X, vector.Y); }
    /// <summary>Returns a new Size instance initialized from vector.</summary>
    public static implicit operator Size (IntVector2D vector)  { return new Size(vector.X, vector.Y);  }
    #endregion

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as IntVector2D?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return X << 16  ^  Y  ^  W; }

    /// <inheritdoc/>
    public bool Equals(IntVector2D other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (IntVector2D lhs, IntVector2D rhs) { return ! (lhs == rhs); }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (IntVector2D lhs, IntVector2D rhs) {
      return (lhs.X == rhs.X)  &&  (lhs.Y == rhs.Y)  &&  (lhs.W == rhs.W);
    }
    #endregion

    /// <summary>Culture-invariant string representation of this instance's value.</summary>
    public override string ToString() { return ToString("G", CultureInfo.InvariantCulture); }

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
    /// - 'G' or 'g': General formatting - same as 'V' or 'v';  
    /// - 'I' or 'i': 2-Dimensional  vector formatting as 11I + 22J;
    /// - 'W' or 'w': 3-Dimensional  vector formatting as 11I + 22J = 33K;
    /// In all cases the leading character of the format string is stripped off and parsed, 
    /// with the remainder passed to the formatter completing the display formatting.
    /// 
    /// For the 2-D and 3-D vector formatting (ie 'I', 'i', 'W', or 'w'), the case of the 
    /// unit vectors follows that of the supplied formatting command character.
    /// </remarks>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (format==null || format.Length==0 || Char.IsDigit(format[0])) format = "G";
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
      return string.Format(layout, X.ToString(format,formatProvider),
                                   Y.ToString(format,formatProvider),
                                   W.ToString(format,formatProvider));
    }
  }
}
