#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>Extension methods for enums that avoid boxing.</summary>
    public static class EnumHelper {
        /// <summary>Determines whether one or more bit fields are set in the current instance; without boxing.</summary>
        /// <remarks>Use <see cref="Enum.HasFlag"/> where CLS-Compliance is required.</remarks>
        [SuppressMessage("Microsoft.Naming","CA1704:IdentifiersShouldBeSpelledCorrectly",MessageId = "Hasflag")]
        [CLSCompliant(false)]
        public static bool TestBits<TEnum>(this TEnum bitField,TEnum bitsToTest)
        where TEnum : struct, IConvertible
        => EnumHelper<TEnum>.HasflagDelegate(bitField,bitsToTest);

        #region Utility methods for (all possible) base-types of an enumeration;
        internal static bool HasFlag( sbyte item,  sbyte bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag(  byte item,   byte bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag( short item,  short bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag(ushort item, ushort bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag(   int item,    int bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag(  uint item,   uint bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag(  long item,   long bitFields) => (item & bitFields) != 0;
        internal static bool HasFlag( ulong item,  ulong bitFields) => (item & bitFields) != 0;
        #endregion
    }

    /// <summary>Support class for <see cref="EnumHelper"/>.</summary>
    /// <typeparam name="TEnum"></typeparam>
    internal static class EnumHelper<TEnum> where TEnum : struct, IConvertible {
        /// <summary>Type-safe (and boxing-free) delegate supporting <see cref="EnumHelper.TestBits"/>.</summary>
        public static readonly Func<TEnum,TEnum,bool> HasflagDelegate = GetHasflagDelegate();

        private static string HasFlag => "HasFlag";

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

        private static bool IsFlagsEnum(IList<CustomAttributeData> attributes)
        => attributes.Select(a => IsFlagsAttribute(a)).FirstOrDefault(b => b);

        private static bool IsFlagsAttribute(CustomAttributeData attribute)
        => attribute.AttributeType.FullName == "System.FlagsAttribute";
    }
}
