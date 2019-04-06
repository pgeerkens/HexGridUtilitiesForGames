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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PGNapoleonics.HexUtilities {
    /// <summary>TODO</summary>
    public class CustomCoords : IFormatProvider, ICustomFormatter {
        /// <summary>Return the coordinate vector of this hex in the Custom frame.</summary>
        public IntVector2D UserToCustom(HexCoords coords)
        => coords.User * MatrixUserToCustom;
        
        /// <summary>Return the coordinate vector of this hex in the User frame.</summary>
        public HexCoords CustomToUser(IntVector2D coords)
        => HexCoords.NewUserCoords(coords * MatrixUserToCustom);

        /// <summary>Initialize the conversion matrices for the Custom coordinate frame.</summary>
        public CustomCoords(IntMatrix2D matrix) : this(matrix,matrix) { }

        /// <summary>Initialize the conversion matrices for the Custom coordinate frame.</summary>
        public CustomCoords(IntMatrix2D userToCustom, IntMatrix2D customToUser) {
            MatrixUserToCustom = userToCustom;
            MatrixCustomToUser = customToUser;
        }

        /// <summary>Gets the conversion @this from Custom to Rectangular (User) coordinates.</summary>
        public IntMatrix2D MatrixCustomToUser { get; }

        /// <summary>Gets the conversion @this from Rectangular (User) to Custom coordinates.</summary>
        public IntMatrix2D MatrixUserToCustom { get; }

        /// <summary>TODO</summary>
        public ICustomFormatter GetFormat(Type formatType)
        => formatType == typeof(HexCoords) ? this : CultureInfo.CurrentUICulture.GetFormat(formatType) as ICustomFormatter;

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#",
                                    Justification="Agrees with interface specification.")]
        public string Format(string format, HexCoords coords, IFormatProvider formatProvider) {
            if (format==null || format.Length==0) format = "U";
            switch(format[0]) {
                case 'U': return UserToCustom(coords).ToString(format.Substring(1), formatProvider);
                case 'u': return "Custom: " + 
                                 UserToCustom(coords).ToString(format.Substring(1), formatProvider);

                default:  return coords.ToString(format, formatProvider);
            }
        }

        object IFormatProvider.GetFormat(Type formatType) => GetFormat(formatType);
 
        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        => Format(format, arg as HexCoords?, formatProvider);

        string Format(string format, HexCoords? coords, IFormatProvider formatProvider)
        => coords.HasValue ? Format(format, coords.Value, formatProvider)
                           : HandleOtherFormats(format, coords);

        private static string HandleOtherFormats(string format, object obj)
        => (obj is IFormattable f) ? f.ToString(format, CultureInfo.CurrentCulture)
                                   : obj?.ToString() ?? string.Empty;
    }
}
