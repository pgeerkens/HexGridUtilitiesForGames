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
using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
    /// <summary>TODO</summary>
  public static class CustomCoords {

    /// <summary>Return the coordinate vector of this hex in the Custom frame.</summary>
    public static IntVector2D UserToCustom(this HexCoords @this) {
      return @this.User * MatrixUserToCustom;
    }
    /// <summary>Return the coordinate vector of this hex in the User frame.</summary>
    public static HexCoords CustomToUser(this IntVector2D @this) {
      return HexCoords.NewUserCoords(@this * MatrixUserToCustom);
    }

    /// <summary>Initialize the conversion matrices for the Custom coordinate frame.</summary>
    public static void SetMatrices(IntMatrix2D matrix) { SetMatrices(matrix,matrix); }

    /// <summary>Initialize the conversion matrices for the Custom coordinate frame.</summary>
    public static void SetMatrices(IntMatrix2D userToCustom, IntMatrix2D customToUser) {
      MatrixUserToCustom = userToCustom;
      MatrixCustomToUser = customToUser;
    }

    /// <summary>Gets the conversion @this from Custom to Rectangular (User) coordinates.</summary>
    public static IntMatrix2D MatrixCustomToUser { get; private set; }

    /// <summary>Gets the conversion @this from Rectangular (User) to Custom coordinates.</summary>
    public static IntMatrix2D MatrixUserToCustom { get; private set; }
  }
}
