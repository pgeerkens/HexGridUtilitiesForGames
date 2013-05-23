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
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public partial class HexCoords : Coords {
    static HexCoords() {
      HexsideList        = Utils.EnumGetValues<Hexside>().Where(h=>h!=Hexside.None).ToList();

      MatrixUserToCanon  = new IntMatrix2D(2, 1,  0,2,  0,0,  2);
      MatrixCanonToUser  = new IntMatrix2D(2,-1,  0,2,  0,1,  2);
    }

    static readonly List<Hexside> HexsideList;
    static readonly ICoords _EmptyCanon = HexCoords.NewCanonCoords(0,0);
    static readonly ICoords _EmptyUser  = HexCoords.NewUserCoords(0,0);
    public static   ICoords EmptyCanon { get { return _EmptyCanon; } }
    public static   ICoords EmptyUser  { get { return _EmptyUser; } }

    #region Constructors
    public static ICoords NewCanonCoords (IntVector2D vector){ return new HexCoords(CoordsType.Canon, vector); }
    public static ICoords NewUserCoords  (IntVector2D vector){ return new HexCoords(CoordsType.User,vector); }
    public static ICoords NewCanonCoords (int x, int y) { return new HexCoords(CoordsType.Canon, x,y); }
    public static ICoords NewUserCoords  (int x, int y) { return new HexCoords(CoordsType.User,x,y); }

    protected HexCoords(CoordsType coordsType, IntVector2D vector) : base(coordsType, vector) {}
    protected HexCoords(CoordsType coordsType, int x, int y) : base(coordsType, new IntVector2D(x,y)) {}
    #endregion

    #region protected overrides
    protected override IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides) {
      ICoords coords = this;
      foreach (var hexside in HexsideList.Where(h=>hexsides.HasFlag(h)))
        yield return new NeighbourCoords(hexside, coords.StepOut(hexside));
    }

    protected override int Range(ICoords coords) {
      return Range(coords.Canon);
    }
    private int Range(IntVector2D vector) {
      var deltaX = vector.X - VectorCanon.X;
      var deltaY = vector.Y - VectorCanon.Y;
      return (Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaX-deltaY)) / 2;
    }

    protected override ICoords StepOut(ICoords coords) { 
      return NewCanonCoords(VectorCanon + coords.Canon); 
    }
    #endregion
  } 
  public class CustomCoordsFactory {
    public CustomCoordsFactory(IntMatrix2D matrix) : this(matrix,matrix) {}
    public CustomCoordsFactory(IntMatrix2D userToCustom, IntMatrix2D customToUser) {
      MatrixUserToCustom = userToCustom;
      MatrixCustomToUser = customToUser;
    }

    protected IntMatrix2D MatrixCustomToUser { get; private set; }
    protected IntMatrix2D MatrixUserToCustom { get; private set; }

    public ICoords Coords(int x, int y) { return Coords(new IntVector2D(x,y)); }
    public ICoords Coords(IntVector2D vector) {
      return HexCoords.NewUserCoords(vector * MatrixCustomToUser);
    }
    public IntVector2D Custom(ICoords coords) {
      return coords.User * MatrixUserToCustom;
    }

    public IntVector2D User(IntVector2D custom) { return custom * MatrixCustomToUser; }
    public IntVector2D Custom(IntVector2D user) { return user * MatrixUserToCustom; }

    public class CustomCoords : HexCoords {
      private CustomCoords (CustomCoordsFactory factory, IntVector2D vector) 
        : base(CoordsType.User, factory.User(vector)) {
        Factory = factory;
      }

      private CustomCoordsFactory Factory { get; set; }

      public IntVector2D Custom { get { return Factory.Custom(base.VectorUser); } }

      public override string ToString() { return Custom.ToString(); }
    }
  }
}
