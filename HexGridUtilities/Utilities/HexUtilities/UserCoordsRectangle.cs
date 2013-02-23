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
    public ICoordsUser Location { get { return Coords.NewUserCoords(Rectangle.Location); } }
    public int         Right    { get { return Rectangle.Right; } }
    public ICoordsUser Size     { get { return Coords.NewUserCoords(Rectangle.Size); } }
    public int         Top      { get { return Rectangle.Top; } }
    public int         Width    { get { return Rectangle.Width; } }
    public int         X        { get { return Rectangle.X; } }
    public int         Y        { get { return Rectangle.Y; } }

    public Rectangle   Rectangle  { get; private set; }
    public ICoordsUser UpperLeft  { get { return Coords.NewUserCoords(Left,Top); } }
    public ICoordsUser UpperRight { get { return Coords.NewUserCoords(Right,Top); } }
    public ICoordsUser LowerLeft  { get { return Coords.NewUserCoords(Left,Bottom); } }
    public ICoordsUser LowerRight { get { return Coords.NewUserCoords(Right,Bottom); } }

    #region Value Equality
    bool IEquatable<UserCoordsRectangle>.Equals(UserCoordsRectangle rhs) { return this == rhs; }
    public override bool Equals(object rhs) { return (rhs is Coords) && this == (UserCoordsRectangle)rhs; }
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
