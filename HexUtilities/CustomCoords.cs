#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities {
  /// <summary>TODO</summary>
  public class CustomCoords : IFormatProvider, ICustomFormatter {
    /// <summary>Return the coordinate vector of this hex in the Custom frame.</summary>
    public IntVector2D UserToCustom(HexCoords coords) {
      return coords.User * MatrixUserToCustom;
    }
    /// <summary>Return the coordinate vector of this hex in the User frame.</summary>
    public HexCoords CustomToUser(IntVector2D coords) {
      return HexCoords.NewUserCoords(coords * MatrixUserToCustom);
    }

    /// <summary>Initialize the conversion matrices for the Custom coordinate frame.</summary>
    public CustomCoords(IntMatrix2D matrix) : this(matrix,matrix) { }

    /// <summary>Initialize the conversion matrices for the Custom coordinate frame.</summary>
    public CustomCoords(IntMatrix2D userToCustom, IntMatrix2D customToUser) {
      _matrixUserToCustom = userToCustom;
      _matrixCustomToUser = customToUser;
    }

    /// <summary>Gets the conversion @this from Custom to Rectangular (User) coordinates.</summary>
    public IntMatrix2D MatrixCustomToUser { get { return _matrixCustomToUser; } }
    private readonly IntMatrix2D _matrixCustomToUser;

    /// <summary>Gets the conversion @this from Rectangular (User) to Custom coordinates.</summary>
    public IntMatrix2D MatrixUserToCustom { get { return _matrixUserToCustom; } }
    private readonly IntMatrix2D _matrixUserToCustom;

    /// <summary>TODO</summary>
    public ICustomFormatter GetFormat(Type formatType) {
      return formatType == typeof(HexCoords) ? this : CultureInfo.CurrentUICulture.GetFormat(formatType) as ICustomFormatter;
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#",
      Justification="Agrees with interface specification.")]
    public string Format(string format, HexCoords coords, IFormatProvider formatProvider) {
      Contract.Ensures(Contract.Result<System.String>() != null);
      if (format==null || format.Length==0) format = "U";
      switch(format[0]) {
        case 'U': return UserToCustom(coords).ToString(format.Substring(1), formatProvider);
        case 'u': return "Custom: " + 
                         UserToCustom(coords).ToString(format.Substring(1), formatProvider);

        default:  return coords.ToString(format, formatProvider);
      }
    }

    Object IFormatProvider.GetFormat(Type formatType) { return GetFormat(formatType); }
 
    string ICustomFormatter.Format(string format, Object arg, IFormatProvider formatProvider) {
      var coords = arg as HexCoords?;
      if (coords.HasValue)
        return Format(format, coords.Value, formatProvider);
      else
          return HandleOtherFormats(format, arg);
    }
    private static string HandleOtherFormats(string format, object argument) {
      var formattable = argument as IFormattable;
      return (formattable != null)  ?  formattable.ToString(format, CultureInfo.CurrentCulture)
           :    (argument != null)  ?  argument.ToString()
                                    :  String.Empty;
    }
  }
}
