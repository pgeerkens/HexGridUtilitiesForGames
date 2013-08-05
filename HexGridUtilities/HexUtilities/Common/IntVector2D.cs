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
using System.Drawing;
using System.Globalization;
using System.Text;

///<summary>Shared technoloiges across the library, and useful gadgets.</summary>
namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Representation of an immutable integer 2D vector.</summary>
  public struct IntVector2D : IEquatable<IntVector2D>, IFormattable {
    /// <summary>TODO</summary>
    public static readonly IntVector2D Empty = new IntVector2D(Point.Empty);

    /// <summary>TODO</summary>
    public int X { get; private set; }
    /// <summary>TODO</summary>
    public int Y { get; private set; }
    /// <summary>TODO</summary>
    public int W { get; private set; }

    /// <summary>TODO</summary>
    public IntVector2D(Point point)         : this(point.X, point.Y, 1) {}
    /// <summary>TODO</summary>
    public IntVector2D(Size size)           : this(size.Width, size.Height, 1) {}
    /// <summary>TODO</summary>
    public IntVector2D(IntVector2D v)       : this(v.X, v.Y, 1) {}
    /// <summary>TODO</summary>
    public IntVector2D(int x, int y)        : this(x, y, 1) {}
    /// <summary>TODO</summary>
    public IntVector2D(int x, int y, int w) : this() {
      X = x;
      Y = y;
      W = w;
    }

    /// <summary>TODO</summary>
    public IntVector2D Normalize() {
      switch (W) {
        case 0:   return Empty; // uninitialized!
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
    /// <summary>TODO</summary>
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
    /// <summary>TODO</summary>
    public static IntVector2D Multiply (int s, IntVector2D v) { return v * s; }
    /// <summary>TODO</summary>
    public static IntVector2D Multiply (IntVector2D v, int s) { return v * s; }
    /// <summary>TODO</summary>
    public static IntVector2D Divide (IntVector2D v, int i)   { return v / i; }
    /// <summary>TODO</summary>
    public static IntVector2D Divide (IntVector2D v, float s) { return v / s; }
    #endregion

    #region Vector operators
    /// <summary>Scalar (Inner, or Dot) Product of two <code>IntVector2D</code> as an Int32.</summary>
    public static int operator * (IntVector2D v1, IntVector2D v2) {
      return v1.X*v2.X + v1.Y*v2.Y;
    }
    /// <summary>Z component of the 'Vector'- or Cross-Product of two <code>IntVector2D</code>s</summary>
    /// <returns>A pseudo-scalar (it reverses sign on exchange of its arguments).</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2225:OperatorOverloadsHaveNamedAlternates")]
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

    /// <summary>TODO</summary>
    public static int InnerProduct (IntVector2D v1, IntVector2D v2) { return v1 * v2; }
    /// <summary>TODO</summary>
    public static int CrossProduct (IntVector2D v1, IntVector2D v2) { return v1 ^ v2; }

    #endregion

    #region Casts
    /// <summary>TODO</summary>
    public static implicit operator IntVector2D (Point p) { return new IntVector2D(p);  }
    /// <summary>TODO</summary>
    public static implicit operator IntVector2D (Size s)  { return new IntVector2D(s);  }
    /// <summary>TODO</summary>
    public static implicit operator Point (IntVector2D v) { return new Point(v.X, v.Y); }
    /// <summary>TODO</summary>
    public static implicit operator Size (IntVector2D v)  { return new Size(v.X, v.Y);  }
    #endregion

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj)              { 
      return (obj is IntVector2D) && this == (IntVector2D)obj; }
    bool IEquatable<IntVector2D>.Equals(IntVector2D rhs) { return this == rhs; }
    /// <summary>TODO</summary>
    public static bool operator != (IntVector2D lhs, IntVector2D rhs) { return ! (lhs == rhs); }
    /// <summary>TODO</summary>
    public static bool operator == (IntVector2D lhs, IntVector2D rhs) {
      return (lhs.X == rhs.X) && (lhs.Y == rhs.Y) && (lhs.W == rhs.W);
    }
    /// <inheritdoc/>
    public override int GetHashCode() { return (X<<16) ^ Y ^ W; }
    #endregion

    /// <summary>Culture-invariant string representation of this instance's value.</summary>
    public override string ToString() { return ToString("G", CultureInfo.InvariantCulture); }

    /// <summary>Converts the value of this instance to its equivalent string representation using the 
    /// specified format and culture-specific format information.</summary>
    /// <param name="format">Type: System.String. 
    /// 
    /// > A standard or custom numeric format string.</param>
    /// <param name="formatProvider">Type: IFormatProvider<para/>
    /// 
    /// > An object that supplies culture-specific formatting information.</param>
    /// <remarks>Format characters:
    /// - 'V' or 'v': Vector formatting - Vector output like (nn,mm);
    /// - 'G' or 'g': General formatting - same as 'V' or 'v';  
    /// - 'I': 2-Dimensional  vector formatting as 11I + 22J;
    /// - 'i': 2-Dimensional  vector formatting as 11i + 22j;
    /// - 'K': 3-Dimensional  vector formatting as 11I + 22J = 33K;
    /// - 'k': 3-Dimensional  vector formatting as 11i + 22j + 33k;
    /// In all cases the leading character of the format string is stripped off and parsed, 
    /// with the remainder passed to the formatter completing the display formatting.
    /// </remarks>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (format==null || format.Length==0 || Char.IsDigit(format[0])) format = "G";
      var formatChar = format[0];
      var sb = new StringBuilder();
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
