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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>Type-safe extension methods for parsing Enums.</summary>
    public static partial class EnumExtensions{
      #region Enum Parsing utilities
      /// <summary>Typesafe wrapper for <c>Enum.GetValues(typeof(TEnum).</c></summary>
      public static IList<TEnum> EnumGetValues<TEnum>() {
        Contract.Ensures(Contract.Result<IList<TEnum>>() != null) ;
        return new List<TEnum>((TEnum[])(Enum.GetValues(typeof(TEnum)))).AsReadOnly();
      }

      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      [Obsolete("Less useful or convenient than originally thought - just use Enum.GetNames({TEnum}).")]
      public static IList<string> EnumGetNames<TEnum>() where TEnum : struct {
        Contract.Ensures(Contract.Result<IList<string>>() != null) ;
        return new List<string>((string[])(Enum.GetNames(typeof(TEnum)))).AsReadOnly();
      }

      /// <summary>Typesafe wrapper for <c>Enum.ParseEnum()</c> that automatically checks membership.</summary>
      public static TEnum ParseEnum<TEnum>(string value) where TEnum : struct {
        return ParseEnum<TEnum>(value,true,false);
      }

      /// <summary>Typesafe wrapper for <c>Enum.ParseEnum()</c> that automatically checks membership.</summary>
      public static TEnum ParseEnum<TEnum>(string value, bool checkConstants, bool ignoreCase) where TEnum : struct {
        TEnum enumValue;
        if (!TryParseEnum<TEnum>(value, ignoreCase, out enumValue) && checkConstants) 
              throw new ArgumentOutOfRangeException("value",value,"Enum type: " + typeof(TEnum).Name);

        return enumValue;
      }

      /// <summary>Typesafe wrapper for <c>Enum.TryParseEnum()</c> that automatically checks membership.</summary>
      public static bool TryParseEnum<TEnum>(string value, out TEnum enumValue) where TEnum : struct {
        return TryParseEnum(value, false, out enumValue);
      }
      /// <summary>Typesafe wrapper for <c>Enum.TryParseEnum()</c> that automatically checks membership.</summary>
      public static bool TryParseEnum<TEnum>(string value, bool ignoreCase, out TEnum enumValue) where TEnum : struct {
        return Enum.TryParse<TEnum>(value, ignoreCase, out enumValue)  
           &&  Enum.IsDefined(typeof(TEnum),enumValue);
      }

      /// <summary>Typesafe wrapper for <c>Enum.ToObject()</c>.</summary>
      /// <typeparam name="TEnum"></typeparam>
      public static TEnum EnumParse<TEnum>(char c, string lookup) {
        lookup.RequiredNotNull("lookup");
        var index = lookup.IndexOf(c);
        if (index == -1) throw new ArgumentOutOfRangeException("c",c,"Enum Type: " + typeof(TEnum).Name);

        return (TEnum) Enum.ToObject(typeof(TEnum), index);
      }
      #endregion
    }
}
