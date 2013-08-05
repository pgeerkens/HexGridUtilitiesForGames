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
using System.Globalization;
using System.Reflection;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>Type-safe extension methods for parsing Enums.</summary>
  public static partial class Utilities{
    #region Enum Parsing utilities
    /// <summary>Typesafe wrapper for <c>Enum.GetValues(typeof(TEnum).</c></summary>
    public static IEnumerable<TEnum> EnumGetValues<TEnum>() {
      return (IEnumerable<TEnum>)(Enum.GetValues(typeof(TEnum)));
    }

    /// <summary>Typesafe wrapper for <c>Enum.ParseEnum()</c> that automatically checks 
    /// constants for membership in the <c>enum</c>.</summary>
    public static T ParseEnum<T>(string value) where T:struct {return ParseEnum<T>(value,true); }

    /// <summary>Typesafe wrapper for <c>Enum.ParseEnum()</c> that automatically checks 
    /// constants for membership in the <c>enum</c>.</summary>
    public static T ParseEnum<T>(string value, bool checkConstants) where T:struct {
      T enumValue;
      if (!TryParseEnum<T>(value, out enumValue) && checkConstants) 
            throw new ArgumentOutOfRangeException("value",value,"Enum type: " + typeof(T).Name);

      return enumValue;
    }

    /// <summary>Typesafe wrapper for <c>Enum.TryParseEnum()</c> that automatically checks 
    /// constants for membership in the <c>enum</c>.</summary>
    public static bool TryParseEnum<T>(string value, out T enumValue) where T:struct {
      return Enum.TryParse<T>(value, out enumValue)  
         &&  Enum.IsDefined(typeof(T),enumValue);
    }

    /// <summary>Typesafe wrapper for <c>Enum.ToObject()</c>.</summary>
    /// <typeparam name="T"></typeparam>
    public static T EnumParse<T>(char c, string lookup) {
      if (lookup==null) throw new ArgumentNullException("lookup");
      var index = lookup.IndexOf(c);
      if (index == -1) throw new ArgumentOutOfRangeException("c",c,"Enum Type: " + typeof(T).Name);

      return (T) Enum.ToObject(typeof(T), index);
    }
    #endregion
  }
}
#region Deprecated code
namespace PGNapoleonics.HexUtilities.Common {
using System.IO;
  public static partial class Utilities{
    #region InvalidDataException Throwers
    /// <summary>Deprecated</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1062:Validate arguments of public methods", MessageId = "0")]
    [Obsolete("InvalidDataException is an IO Exception. Subclass and throw a more appropriate error instead.")]
    public static void ThrowInvalidDataException(MemberInfo type, object data) {
      throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
          "{0}: Invalid: '{1}'", type.Name, data));
    }
    ///  <summary>Deprecated:</summary>
    [Obsolete("InvalidDataException is an IO Exception. Subclass and throw a more appropriate error instead.")]
    public static void ThrowInvalidDataException(string parseType, int lineNo, 
      object section, string error, Exception ex, object data) {
      throw new InvalidDataException(
          string.Format(CultureInfo.InvariantCulture,
            "{0}: {3}\n  for section {2} on line # {1}:\n   {4}",  
              parseType, lineNo, section, error, data), ex);
    }
    #endregion
  }
}
#endregion