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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PG_Napoleonics.Utilities {
  public static class Utils{
    #region Enum Parsing utilities
    public static IEnumerable<TEnum> EnumGetValues<TEnum>() {
      return (IEnumerable<TEnum>)(Enum.GetValues(typeof(TEnum)));
    }

    public static T ParseEnum<T>(string value, bool checkConstants = true) {
      T enumValue;
      if (!TryParseEnum<T>(value, out enumValue) && checkConstants) 
                  ThrowInvalidDataException(typeof(T), enumValue);
      return enumValue;
    }
    public static bool TryParseEnum<T>(string value, out T enumValue) {
      enumValue = (T)Enum.Parse(typeof(T),value);
      return  (Enum.IsDefined(typeof(T),enumValue));
    }
    public static T EnumParse<T>(char c, string lookup) {
      var index = lookup.IndexOf(c);
      if (index == -1) ThrowInvalidDataException(typeof(T), c);
      return (T) Enum.ToObject(typeof(T), index);
    }
    #endregion

    #region ErrorThrowers
    public static void ThrowInvalidDataException(Type type, object data) {
      throw new InvalidDataException(string.Format("{0}: Invalid: '{1}'", type.Name, data));
    }
    public static void ThrowInvalidDataException(string parseType, int lineNo, object section, 
      string error, Exception ex, object data) {
      throw new InvalidDataException(
              string.Format("{0}: {3}\n  for section {2} on line # {1}:\n   {4}",  
                  parseType, lineNo, section, error, data), ex);
    }
    #endregion
  }
}
