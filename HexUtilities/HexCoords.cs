#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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

namespace PGNapoleonics.HexUtilities {
    using HexPoint = System.Drawing.Point;
    using HexSize  = System.Drawing.Size;

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
    public struct HexCoords : IEquatable<HexCoords>, IFormattable  {
        #region private static fields
        static readonly IntMatrix2D _matrixUserToCanon = new IntMatrix2D(2, 1,  0,2,  0,0,  2);
        static readonly IntMatrix2D _matrixCanonToUser = new IntMatrix2D(2,-1,  0,2,  0,1,  2);

        static readonly IList<IntVector2D> _hexsideVectorsCanon = new List<IntVector2D>() {
            new IntVector2D( 0,-1),   // HexSide.North
            new IntVector2D( 1, 0),   // HexSide.NorthEast
            new IntVector2D( 1, 1),   // HexSide.SouthEast
            new IntVector2D( 0, 1),   // HexSide.South
            new IntVector2D(-1, 0),   // HexSide.SouthWest
            new IntVector2D(-1,-1)    // HexSide.NorthWest
        }.AsReadOnly();

        static readonly IList<IList<IntVector2D>> _hexsideVectorsUser = new List<IList<IntVector2D>>() {
            new List<IntVector2D>() {
                new IntVector2D( 0,-1),    // even x HexSide.North
                new IntVector2D( 1, 0),    // even x HexSide.NorthEast
                new IntVector2D( 1, 1),    // even x HexSide.SouthEast
                new IntVector2D( 0, 1),    // even x HexSide.South
                new IntVector2D(-1, 1),    // even x HexSide.SouthWest
                new IntVector2D(-1, 0)     // even x HexSide.NorthWest
            }.AsReadOnly(),
            new List<IntVector2D>() {
                new IntVector2D( 0,-1),    // odd x HexSide.North
                new IntVector2D( 1,-1),    // odd x HexSide.NorthEast
                new IntVector2D( 1, 0),    // odd x HexSide.SouthEast
                new IntVector2D( 0, 1),    // odd x HexSide.South
                new IntVector2D(-1, 0),    // odd x HexSide.South West
                new IntVector2D(-1,-1)     // odd x HexSide.NorthWest
            }.AsReadOnly()
        }.AsReadOnly();
        #endregion

        #region Public static members
        /// <summary>Create a new instance located at the specified i and j offsets as interpreted in the Canon(ical) frame.</summary>
        public static HexCoords NewCanonCoords (int x, int y)
        => NewCanonCoords(new IntVector2D(x,y));

        /// <summary>Create a new instance located at the specified i and j offsets as interpreted in the ectangular (User) frame.</summary>
        public static HexCoords NewUserCoords  (int x, int y)
        => NewUserCoords(new IntVector2D(x,y));

        /// <summary>Create a new instance located at the specified vector offset as interpreted in the Canon(ical) frame.</summary>
        public static HexCoords NewCanonCoords (IntVector2D vector)
        => new HexCoords(vector, vector * _matrixCanonToUser);

        /// <summary>Create a new instance located at the specified vector offset as interpreted in the Rectangular (User) frame.</summary>
        public static HexCoords NewUserCoords  (IntVector2D vector)
        => new HexCoords(vector * _matrixUserToCanon, vector);

        /// <summary>Origin of the Canon(ical) coordinate frame.</summary>
        public static HexCoords EmptyCanon { get; }  = NewCanonCoords(0,0);
        /// <summary>Origin of the Rectangular (User) coordinate frame.</summary>
        public static HexCoords EmptyUser  { get; }  = NewUserCoords(0,0);

        /// <summary>Returns the drawing origin (upper-left) for the hex with specified user components.</summary>
        public static HexPoint HexOrigin(HexSize gridSize, int i, int j) =>
            new HexPoint(
                checked(i * gridSize.Width),
                checked(j * gridSize.Height + (i+1)%2 * (gridSize.Height)/2));
        #endregion

        #region Constructors
        
        private HexCoords(IntVector2D canon, IntVector2D user) :this() {
            Canon = canon;
            User  = user;
        }
        #endregion

        #region Properties
        /// <summary>Returns an <c>IntVector2D</c> representing the Canonical (obtuse) coordinates of this hex.</summary>
        public  IntVector2D Canon { get; }

        /// <summary>Returns an <c>IntVector2D</c> representing the User (rectangular) coordinates of this hex.</summary>
        public  IntVector2D User  { get; }

        /// <summary>Modified <i>Manhattan</i> distance of supplied coordinate from the origin.</summary>
        public  int         RangeFromOrigin => EmptyCanon.Range(this);
        #endregion

        #region Methods
        /// <summary>Returns an <c>HexCoords</c> for the hex in direction <c>hexside</c> from this one.</summary>
        public HexCoords GetNeighbour(Hexside hexside) {
            var i = User.X & 1; 
            return new HexCoords(Canon + _hexsideVectorsCanon  [hexside]
                                ,User  + _hexsideVectorsUser[i][hexside] );
        }

        /// <summary>Returns the drawing origin (upper-left) for the specified hex.</summary>
        public HexPoint HexOrigin(HexSize gridSize) => HexOrigin(gridSize, User.X, User.Y); 

        /// <summary>Modified <i>Manhattan</i> distance of supplied coordinate from this one.</summary>
        public int       Range(HexCoords coords) {
            var deltaX = coords.Canon.X - Canon.X;
            var deltaY = coords.Canon.Y - Canon.Y;
            return ( Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY) ) / 2;
        }

        /// <summary>Culture-invariant string representation of this instance's value.</summary>
        public override string ToString() => ToString("g", CultureInfo.InvariantCulture);

        /// <summary>Converts the value of this instance to its equivalent string representation using the 
        /// specified format and culture-specific format information.</summary>
        /// <param name="format">Type: System.String. 
        /// 
        /// > A standard or custom numeric format string.</param>
        /// <param name="formatProvider">Type: IFormatProvider - 
        /// 
        /// > An object that supplies culture-specific formatting information.</param>
        /// <remarks>
        /// Format characters:
        /// - 'C' or 'c': Canonical formatting - Int2Vector output of the Canonical coordinates for this hex;
        /// - 'G' or 'g': General formatting - Int2Vector output of the User coordinates for this hex;
        /// - 'R' or 'r': Range formatting - Scalar output of the Range of this hex from canonical (0,0);
        /// In all cases the leading character of the format string is stripped off and parsed, 
        /// with the remainder passed to the formatter completing the display formatting.
        /// 
        /// If an instance of <see cref="CustomCoords"/> is passed as the <see cref="IFormatProvider"/> then
        /// two additional formats are supported:
        /// - 'U' or 'u': Custom formatting - Int2Vector output of the Custom coordinates for this hex.
        /// 
        /// Likewise, any additional formats supported by <paramref name="formatProvider"/> can be processed.
        /// 
        /// The lower-case format comands prefix a descriptive string on the output (ie one of "Canon: ",
        /// "User: ", "Custom: ", or "Range: " respectivelly), while the upper-case commands do not.
        /// </remarks>
        public string ToString(string format, IFormatProvider formatProvider) {

            if (format==null || format.Length==0) format = "G";
            if (formatProvider == null) formatProvider = CultureInfo.CurrentUICulture;
            switch(format[0]) {
                case 'C': 
                case 'G': 
                case 'R': return Format(format[0], format.Substring(1), formatProvider);
                case 'c': return "Canon: " + Format(format[0], format.Substring(1), formatProvider);
                case 'g': return "User: "  + Format(format[0], format.Substring(1), formatProvider);
                case 'r': return "Range: " + Format(format[0], format.Substring(1), formatProvider);

                default:  var formatter = formatProvider.GetFormat(typeof(HexCoords)) as ICustomFormatter
                                        ?? formatProvider.GetFormat(typeof(object)) as ICustomFormatter;
                          if (formatter == null) return string.Empty;
                          return formatter.Format(format, this, formatProvider) ?? string.Empty;
            }
        }

        private string Format(char formatChar, string formatRest, IFormatProvider formatProvider){
            switch (formatChar) {
                case 'c':
                case 'C': return Canon.ToString(formatRest, formatProvider);
                case 'g':
                case 'G': return User.ToString(formatRest, formatProvider);
                case 'r':
                case 'R': return Range(HexCoords.EmptyCanon).ToString("G" + formatRest, formatProvider);
                default:  return string.Empty;
            }
        }
        #endregion

        #region Operators
        /// <summary>Vector sum; <see cref="Add"/>.</summary>
        public static HexCoords operator + (HexCoords lhs, HexCoords rhs) =>
            NewCanonCoords(lhs.Canon + rhs.Canon);
        /// <summary>Vector difference; <see cref="Subtract"/>.</summary>
        public static HexCoords operator - (HexCoords lhs, HexCoords rhs) =>
            NewCanonCoords(lhs.Canon - rhs.Canon);
        /// <summary>(Canonical) vector sum lhs + rhs.</summary>
        /// <param name="lhs">The first term of the sum.</param>
        /// <param name="rhs">The second term of the sum.</param>
        /// <returns>A new HexCoords struct containing the vector sum lhs + rhs calculated 
        /// in the Canonical frame of reference.</returns>
        public static HexCoords Add(HexCoords lhs, HexCoords rhs)      => lhs + rhs;
        /// <summary>(Canonical) vector difference lhs - rhs.</summary>
        /// <param name="lhs">The first term of the difference.</param>
        /// <param name="rhs">The second term of the difference.</param>
        /// <returns>A new HexCoords struct containing the vector difference lhs - rhs calculated 
        /// in the Canonical frame of reference.</returns>
        public static HexCoords Subtract(HexCoords lhs, HexCoords rhs) => lhs - rhs;

        /// <summary>Returns an <c>IntVector2D</c> representing the Canonical (obtuse) coordinates of <c>this</c>.</summary>
        public static implicit operator IntVector2D(HexCoords @this)   => @this.Canon;
        #endregion

        #region Value Equality
        /// <inheritdoc/>
        public override bool Equals(object obj) { 
            var other = obj as HexCoords?;
            return other.HasValue  &&  this == other.Value;
        }
        /// <inheritdoc/>
        public override int GetHashCode() => User.GetHashCode();

        /// <inheritdoc/>
        public bool Equals(HexCoords other) => this == other;
        /// <summary>Tests value-inequality.</summary>
        public static bool operator != (HexCoords lhs, HexCoords rhs) => ! (lhs == rhs);
        /// <summary>Tests value-equality.</summary>
        public static bool operator == (HexCoords lhs, HexCoords rhs) => lhs.User == rhs.User;
        #endregion
    }
}
