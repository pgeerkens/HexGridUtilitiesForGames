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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.Utilities.HexUtilities {
  /// <summary>Stores a rectangular board region as four User Coordinate integers.</summary>
  public struct UserCoordsRectangle : IEquatable<UserCoordsRectangle>, IEqualityComparer<UserCoordsRectangle> {
    public UserCoordsRectangle(ICoordsUser location, ICoordsUser size) 
      : this(new Rectangle(location.Vector, size.Vector)) {}
    public UserCoordsRectangle(int x, int y, int width, int height) 
      : this(new Rectangle(x,y,width,height)) {}
    public UserCoordsRectangle(Rectangle rectangle) : this() {
      Rectangle = rectangle;
    }

    public int         Bottom   { get { return Rectangle.Bottom; } }
    public int         Height   { get { return Rectangle.Height; } }
    public bool        IsEmpty  { get { return Rectangle.IsEmpty; } }
    public int         Left     { get { return Rectangle.Left; } }
    public ICoordsUser Location { get { return HexCoords.NewUserCoords(Rectangle.Location); } }
    public int         Right    { get { return Rectangle.Right; } }
    public ICoordsUser Size     { get { return HexCoords.NewUserCoords(Rectangle.Size); } }
    public int         Top      { get { return Rectangle.Top; } }
    public int         Width    { get { return Rectangle.Width; } }
    public int         X        { get { return Rectangle.X; } }
    public int         Y        { get { return Rectangle.Y; } }

    public Rectangle   Rectangle  { get; private set; }
    public ICoordsUser UpperLeft  { get { return HexCoords.NewUserCoords(Left,Top); } }
    public ICoordsUser UpperRight { get { return HexCoords.NewUserCoords(Right,Top); } }
    public ICoordsUser LowerLeft  { get { return HexCoords.NewUserCoords(Left,Bottom); } }
    public ICoordsUser LowerRight { get { return HexCoords.NewUserCoords(Right,Bottom); } }

    #region Value Equality
    bool IEquatable<UserCoordsRectangle>.Equals(UserCoordsRectangle rhs) { return this == rhs; }
    public override bool Equals(object rhs) { return (rhs is UserCoordsRectangle) && this == (UserCoordsRectangle)rhs; }
    public static bool operator == (UserCoordsRectangle lhs, UserCoordsRectangle rhs) { 
      return lhs.Rectangle == rhs.Rectangle; 
    }
    public static bool operator != (UserCoordsRectangle lhs, UserCoordsRectangle rhs) { return ! (lhs == rhs); }
    public override int GetHashCode() { return Rectangle.GetHashCode(); }

    bool IEqualityComparer<UserCoordsRectangle>.Equals(UserCoordsRectangle lhs, UserCoordsRectangle rhs) { return lhs == rhs; }
    int  IEqualityComparer<UserCoordsRectangle>.GetHashCode(UserCoordsRectangle coords) { return Rectangle.GetHashCode(); }
    #endregion
  }
}
