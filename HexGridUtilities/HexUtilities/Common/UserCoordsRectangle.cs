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
using System.Drawing;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Stores a rectangular board region as a a location and extent of <see cref="HexCoords"/>.</summary>
  [DebuggerDisplay("({Location}):({Size})")]
  public struct CoordsRectangle : IEquatable<CoordsRectangle> {
    #region Constructors
    /// <summary>TODO</summary>
    public CoordsRectangle(HexCoords location, HexCoords size)  : this(new Rectangle(location.User, size.User)) {}
    /// <summary>TODO</summary>
    internal CoordsRectangle(int x, int y, int width, int height) : this(new Rectangle(x,y,width,height)) {}
    /// <summary>TODO</summary>
    private CoordsRectangle(Rectangle rectangle) : this() { Rectangle = rectangle; }
    #endregion

    #region Properties
    /// <summary>TODO</summary>
    public int       Bottom     { get { return Rectangle.Bottom; } }
    /// <summary>TODO</summary>
    public int       Height     { get { return Rectangle.Height; } }
    /// <summary>TODO</summary>
    public bool      IsEmpty    { get { return Rectangle.IsEmpty; } }
    /// <summary>TODO</summary>
    public int       Left       { get { return Rectangle.Left; } }
    /// <summary>TODO</summary>
    public HexCoords Location   { get { return HexCoords.NewUserCoords(Rectangle.Location); } }
    /// <summary>TODO</summary>
    public int       Right      { get { return Rectangle.Right; } }
    /// <summary>TODO</summary>
    public HexCoords Size       { get { return HexCoords.NewUserCoords(Rectangle.Size); } }
    /// <summary>TODO</summary>
    public int       Top        { get { return Rectangle.Top; } }
    /// <summary>TODO</summary>
    public int       Width      { get { return Rectangle.Width; } }
    /// <summary>TODO</summary>
    public int       X          { get { return Rectangle.X; } }
    /// <summary>TODO</summary>
    public int       Y          { get { return Rectangle.Y; } }

    /// <summary>TODO</summary>
    public Rectangle Rectangle  { get; private set; }
    /// <summary>TODO</summary>
    public HexCoords UpperLeft  { get { return HexCoords.NewUserCoords(Left,Top); } }
    /// <summary>TODO</summary>
    public HexCoords UpperRight { get { return HexCoords.NewUserCoords(Right,Top); } }
    /// <summary>TODO</summary>
    public HexCoords LowerLeft  { get { return HexCoords.NewUserCoords(Left,Bottom); } }
    /// <summary>TODO</summary>
    public HexCoords LowerRight { get { return HexCoords.NewUserCoords(Right,Bottom); } }
    #endregion

    /// <inheritdoc/>
    public override string ToString() {
      return string.Format("({0},{1}):({2},{3})",X,Y,Width,Height);
    }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as CoordsRectangle?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Rectangle.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(CoordsRectangle other) { return this == other; }

    /// <summary>Tests value inequality of two CoordsRectangle instances.</summary>
    public static bool operator != (CoordsRectangle lhs, CoordsRectangle rhs) { return ! (lhs == rhs); }

    /// <summary>Tests value equality of two CoordsRectangle instances.</summary>
    public static bool operator == (CoordsRectangle lhs, CoordsRectangle rhs) { 
      return lhs.Rectangle == rhs.Rectangle; 
    }
    #endregion
  }
}
