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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Extension methods for enums that avoid boxing.</summary>
  public static class EnumHelper {
    /// <summary>Determines whether one or more bit fields are set in the current instance; without boxing.</summary>
    /// <remarks>Use <see cref="System.Enum.HasFlag"/> where CLS-Compliance is required.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hasflag")]
    [CLSCompliant(false)]
    public static bool TestBits<TEnum>(this TEnum bitField, TEnum bitsToTest) where TEnum : struct, IConvertible {
      return EnumHelper<TEnum>.HasflagDelegate(bitField, bitsToTest);
    }

    #region Utility methods for (all possible) base-types of an enumeration;
    internal static bool HasFlag( SByte item,  SByte bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag(  Byte item,   Byte bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag( Int16 item,  Int16 bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag(UInt16 item, UInt16 bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag( Int32 item,  Int32 bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag(UInt32 item, UInt32 bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag( Int64 item,  Int64 bitFields) { return (item & bitFields) != 0; }
    internal static bool HasFlag(UInt64 item, UInt64 bitFields) { return (item & bitFields) != 0; }
    #endregion
  }

  /// <summary>Support class for <see cref="EnumHelper"/>.</summary>
  /// <typeparam name="TEnum"></typeparam>
  internal static class EnumHelper<TEnum> where TEnum : struct, IConvertible {
    static readonly string       HasFlag      = "HasFlag";

    /// <summary>Type-safe (and boxing-free) delegate supporting <see cref="EnumHelper.TestBits"/>.</summary>
    public static readonly Func<TEnum,TEnum,bool> HasflagDelegate = GetHasflagDelegate();

    /// <summary>Creates and returns a type-safe Hasflag delegate.</summary>
    private static Func<TEnum,TEnum,bool> GetHasflagDelegate() {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

      var baseType  = EnumBaseType(typeof(TEnum));
      var baseTypes = new Type[] { baseType, baseType };
      var method    = typeof(EnumHelper).GetMethod(HasFlag, bindingFlags, null, baseTypes, null);
      if (method == null) throw new MissingMethodException(typeof(TEnum).Name,HasFlag);

      return (Func<TEnum,TEnum,bool>)method.CreateDelegate(typeof(Func<TEnum, TEnum, bool>));
    }
    private static Type EnumBaseType(Type enumType) {
      if ( ! enumType.IsEnum )        throw new MissingMethodException(typeof(TEnum).Name,HasFlag);
      var attributes = enumType.GetCustomAttributesData() ?? new List<CustomAttributeData>();
      if ( ! IsFlagsEnum(attributes)) throw new MissingMethodException(typeof(TEnum).Name,HasFlag);
      return Enum.GetUnderlyingType(enumType);
    }
    private static bool IsFlagsEnum(IList<CustomAttributeData> attributes) {
      return (from attribute in attributes select IsFlagsAttribute(attribute)
             ).FirstOrDefault(); //b => b);
    //  return attributes.Select(a => IsFlagsAttribute(a)).FirstOrDefault(b => b);
    }
    private static bool IsFlagsAttribute(CustomAttributeData attribute) {
      return attribute.AttributeType.FullName == "System.FlagsAttribute";
    }
  }
}
