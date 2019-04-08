#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>Type-safe extension methods for parsing Enums.</summary>
    public static partial class EnumExtensions{
        /// <summary>Typesafe wrapper for <c>Enum.GetValues(typeof(TEnum).</c></summary>
        public static IList<TEnum> EnumGetValues<TEnum>()
        => new List<TEnum>((TEnum[])(Enum.GetValues(typeof(TEnum)))).AsReadOnly();

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design","CA1004:GenericMethodsShouldProvideTypeParameter")]
        [Obsolete("Less useful or convenient than originally thought - just use Enum.GetNames({TEnum}).")]
        public static IList<string> EnumGetNames<TEnum>() where TEnum : struct
        => new List<string>(Enum.GetNames(typeof(TEnum))).AsReadOnly();

        /// <summary>Typesafe wrapper for <c>Enum.ParseEnum()</c> that automatically checks membership.</summary>
        public static TEnum ParseEnum<TEnum>(string value) where TEnum : struct
        => ParseEnum<TEnum>(value,true,false);

        /// <summary>Typesafe wrapper for <c>Enum.ParseEnum()</c> that automatically checks membership.</summary>
        public static TEnum ParseEnum<TEnum>(string value, bool checkConstants, bool ignoreCase)
        where TEnum : struct {
            if (!TryParseEnum<TEnum>(value,ignoreCase,out var enumValue) && checkConstants)
                throw new ArgumentOutOfRangeException("value",value,"Enum type: " + typeof(TEnum).Name);

            return enumValue;
        }

        /// <summary>Typesafe wrapper for <c>Enum.TryParseEnum()</c> that automatically checks membership.</summary>
        public static bool TryParseEnum<TEnum>(string value,bool ignoreCase,out TEnum enumValue)
        where TEnum : struct
        => Enum.TryParse(value,ignoreCase,out enumValue)
        &  Enum.IsDefined(typeof(TEnum),enumValue);

        /// <summary>Typesafe wrapper for <c>Enum.ToObject()</c>.</summary>
        /// <typeparam name="TEnum"></typeparam>
        public static TEnum EnumParse<TEnum>(char c, string lookup) {
            if (lookup==null) throw new ArgumentNullException("lookup");
            var index = lookup.IndexOf(c);
            if (index == -1) throw new ArgumentOutOfRangeException("c",c,"Enum Type: " + typeof(TEnum).Name);

            return (TEnum) Enum.ToObject(typeof(TEnum), index);
        }
    }
}
