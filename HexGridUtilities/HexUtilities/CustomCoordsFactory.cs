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

using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities {
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
        : base(false, factory.User(vector)) {
        Factory = factory;
      }

      private CustomCoordsFactory Factory { get; set; }

      public IntVector2D Custom { get { return Factory.Custom(base.VectorUser); } }

      public override string ToString() { return Custom.ToString(); }
    }
  }
}
