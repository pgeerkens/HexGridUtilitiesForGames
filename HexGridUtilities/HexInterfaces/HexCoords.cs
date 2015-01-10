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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities.Common;

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
    #region Public static members
    /// <summary>Create a new instance located at the specified i and j offsets as interpreted in the Canon(ical) frame.</summary>
    public static HexCoords NewCanonCoords (int x, int y) { return NewCanonCoords(new IntVector2D(x,y)); }
    /// <summary>Create a new instance located at the specified i and j offsets as interpreted in the ectangular (User) frame.</summary>
    public static HexCoords NewUserCoords  (int x, int y) { return NewUserCoords(new IntVector2D(x,y)); }
    /// <summary>Create a new instance located at the specified vector offset as interpreted in the Canon(ical) frame.</summary>
    public static HexCoords NewCanonCoords (IntVector2D vector){ return new HexCoords(vector, vector * MatrixCanonToUser); }
    /// <summary>Create a new instance located at the specified vector offset as interpreted in the Rectangular (User) frame.</summary>
    public static HexCoords NewUserCoords  (IntVector2D vector){ return new HexCoords(vector * MatrixUserToCanon, vector); }

    /// <summary>Origin of the Canon(ical) coordinate frame.</summary>
    public static HexCoords EmptyCanon { get { return _EmptyCanon; } }
    /// <summary>Origin of the Rectangular (User) coordinate frame.</summary>
    public static HexCoords EmptyUser  { get { return _EmptyUser; } }

    /// <summary>Returns the drawing origin (upper-left) for the hex with specified user components.</summary>
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "i+1",
    Justification="No board remotely near large enough to cause overflow here.")]
    public static HexPoint HexOrigin(HexSize gridSize, int i, int j) {
     return new HexPoint(
       i * gridSize.Width,
       j * gridSize.Height + (i+1)%2 * (gridSize.Height)/2);
    }
    #endregion

    #region Private static methods
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
    static readonly IntVector2D[] HexsideVectorsCanon = new IntVector2D[] {
      vectorN,  vectorNE, vectorSE, vectorS,  vectorSW, vectorNW
    };

    static readonly IntVector2D userEvenN  = new IntVector2D( 0,-1);
    static readonly IntVector2D userEvenNE = new IntVector2D( 1, 0);
    static readonly IntVector2D userEvenSE = new IntVector2D( 1, 1);
    static readonly IntVector2D userEvenS  = new IntVector2D( 0, 1);
    static readonly IntVector2D userEvenSW = new IntVector2D(-1,+1);
    static readonly IntVector2D userEvenNW = new IntVector2D(-1, 0);
    static readonly IntVector2D[] HexsideVectorsUserEven = new IntVector2D[] {
      userEvenN,  userEvenNE, userEvenSE, userEvenS,  userEvenSW, userEvenNW
    };

    static readonly IntVector2D userOddN  = new IntVector2D( 0,-1);
    static readonly IntVector2D userOddNE = new IntVector2D( 1,-1);
    static readonly IntVector2D userOddSE = new IntVector2D( 1, 0);
    static readonly IntVector2D userOddS  = new IntVector2D( 0, 1);
    static readonly IntVector2D userOddSW = new IntVector2D(-1, 0);
    static readonly IntVector2D userOddNW = new IntVector2D(-1,-1);
    static readonly IntVector2D[] HexsideVectorsUserOdd = new IntVector2D[] {
      userOddN,  userOddNE, userOddSE, userOddS,  userOddSW, userOddNW
    };
    #endregion

    #region Constructors
    private HexCoords(IntVector2D canon, IntVector2D user) :this() {
      _canon = canon;
      _user  = user;
    }
    #endregion

    #region Properties
    /// <summary>Returns an <c>IntVector2D</c> representing the Canonical (obtuse) coordinates of this hex.</summary>
    public  IntVector2D Canon { get {return _canon;} } readonly IntVector2D _canon;

    /// <summary>Returns an <c>IntVector2D</c> representing the User (rectangular) coordinates of this hex.</summary>
    public  IntVector2D User  { get {return _user;} } readonly IntVector2D _user;

    /// <summary>Modified <i>Manhattan</i> distance of supplied coordinate from the origin.</summary>
    public  int         RangeFromOrigin { get { return EmptyCanon.Range(this);} }
    #endregion

    #region Methods
    /// <summary>Returns an <c>HexCoords</c> for the hex in direction <c>hexside</c> from this one.</summary>
    public HexCoords GetNeighbour(Hexside hexside) {
      if ((User.X % 2) == 0) 
        return new HexCoords(Canon + HexsideVectorsCanon[(int)hexside]
                            ,User  + HexsideVectorsUserEven[(int)hexside]);
      else 
        return new HexCoords(Canon + HexsideVectorsCanon[(int)hexside]
                            ,User  + HexsideVectorsUserOdd[(int)hexside]);
    }

    /// <summary>Returns all neighbouring hexes as IEnumerable.</summary>
    public IEnumerable<NeighbourCoords> GetNeighbours() { 
      for (var hexside=0; hexside<HexsideVectorsCanon.Length; hexside++)
        yield return new NeighbourCoords(NewCanonCoords(Canon + HexsideVectorsCanon[hexside]),
                                        (Hexside)hexside); 
    }

    /// <summary>Returns set of hexes at direction(s) specified by <c>hexsides</c>, as IEnumerable.</summary>
    public IEnumerable<NeighbourCoords> GetNeighbours(Hexsides hexsides) { 
      return GetNeighbours().Where(n=>hexsides.HasFlag(n.Hexside));
    }

    /// <summary>Returns the drawing origin (upper-left) for the specified hex.</summary>
    public HexPoint HexOrigin(HexSize gridSize) { 
      return HexCoords.HexOrigin(gridSize, User.X, User.Y); 
    }

    /// <summary>Modified <i>Manhattan</i> distance of supplied coordinate from this one.</summary>
    public int       Range(HexCoords coords) { 
      var deltaX = coords.Canon.X - Canon.X;
      var deltaY = coords.Canon.Y - Canon.Y;
      return ( Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY) ) / 2;
    }

    /// <summary>Culture-invariant string representation of this instance's value.</summary>
    public override string ToString() { return ToString("g", CultureInfo.InvariantCulture); }

    /// <summary>Converts the value of this instance to its equivalent string representation using the 
    /// specified format and culture-specific format information.</summary>
    /// <param name="format">Type: System.String. 
    /// 
    /// > A standard or custom numeric format string.</param>
    /// <param name="formatProvider">Type: IFormatProvider - 
    /// 
    /// > An object that supplies culture-specific formatting information.</param>
    /// <remarks>Format characters:
    /// - 'C' or 'c': Canonical formatting - Int2Vector output of the Canonical coordinates for this hex;
    /// - 'G' or 'g': General formatting - same as 'R';  
    /// - 'R' or 'r': Range formatting - Scalar output of the Range of this hex from canonical (0,0);
    /// - 'U' or 'u': Custom formatting - Int2Vector output of the Custom coordinates for this hex;
    /// In all cases the leading character of the format string is stripped off and parsed, 
    /// with the remainder passed to the formatter completing the display formatting.
    /// 
    /// The lower-case format comands prefix a descriptive string on the output (ie one of "Canon: ",
    /// "User: ", "Custom: ", or "Range: " respectivelly), while the upper-case commands do not.
    /// </remarks>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (format==null || format.Length==0) format = "G";
      var formatChar = format[0];
      format = format.Substring(1);
      switch(formatChar) {
        default:    throw new FormatException();
        case 'C':   return this.Canon.ToString(format, formatProvider);
        case 'G':   return this.User.ToString(format, formatProvider);
        case 'U':   return this.UserToCustom().ToString(format, formatProvider);
        case 'R':   if (Char.IsDigit(format[0])) format =  "G" + format;
                    return this.Range(HexCoords.EmptyCanon).ToString(format, formatProvider);

        case 'c':   return "Canon: "  + this.Canon.ToString(format, formatProvider);
        case 'g':   return "User: "   + this.User.ToString(format, formatProvider);
        case 'u':   return "Custom: " + this.UserToCustom().ToString(format, formatProvider);
        case 'r':   if (Char.IsDigit(format[0])) format =  "G" + format;
                    return "Range: "  + this.Range(HexCoords.EmptyCanon).ToString(format, formatProvider);
      }
    }
    #endregion

    #region Operators
    /// <summary>Vector sum; <see cref="Add"/>.</summary>
    public static HexCoords operator + (HexCoords lhs, HexCoords rhs) {
      return HexCoords.NewCanonCoords(lhs.Canon + rhs.Canon);
    }
    /// <summary>Vector difference; <see cref="Subtract"/>.</summary>
    public static HexCoords operator - (HexCoords lhs, HexCoords rhs) {
      return HexCoords.NewCanonCoords(lhs.Canon - rhs.Canon);
    }
    /// <summary>(Canonical) vector sum of lhs plus rhs.</summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>A new HexCoords struct containing the vector sum of lhs and rhs calculated 
    /// in the Canonical frame of reference.</returns>
    public static HexCoords Add(HexCoords lhs, HexCoords rhs) { return lhs + rhs; }
    /// <summary>(Canonical) vector difference of lhs plus rhs.</summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>A new HexCoords struct containing the vector sum of lhs and rhs calculated 
    /// in the Canonical frame of reference.</returns>
    public static HexCoords Subtract(HexCoords lhs, HexCoords rhs) { return lhs - rhs; }

    /// <summary>TODO</summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static implicit operator IntVector2D(HexCoords @this) { return @this.Canon; }
    #endregion

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as HexCoords?;
      return other.HasValue  &&  this == other.Value;
    }
    /// <inheritdoc/>
    public override int GetHashCode() { return User.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(HexCoords other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (HexCoords lhs, HexCoords rhs) { return ! (lhs == rhs); }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (HexCoords lhs, HexCoords rhs) { return lhs.User == rhs.User; }
    #endregion
  }
}
