#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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
