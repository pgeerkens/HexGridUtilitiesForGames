#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.Generic;
using System.Linq;

namespace PGNapoleonics.HexUtilities.FastList {
    ///// <summary>Contract-enforcing wrapper for FastList{T} constructors.</summary>
    //public static class FastList {
    //    ///// <summary>Contract-enforcing wrapper for FastList{T} constructor returning an IFastList{TItem}.</summary>
    //    ///// <typeparam name="TItem">Type of the class/interface being indexed.</typeparam>
    //    ///// <param name="array">An <c>T[]</c> to be indexed.</param>
    //    //public static IFastList<T> New<T>(T[] array) => new FastList<T>(array);

    //    ///// <summary>Contract-enforcing wrapper for FastList{T} constructor returning an IFastList{TItem}.</summary>
    //    ///// <typeparam name="TItem">Type of the class/interface being indexed.</typeparam>
    //    ///// <param name="array">An <c>T[]</c> to be indexed.</param>
    //    //internal static IFastListX<T> NewX<T>(T[] array) => new FastList<T>(array);
    //}

    /// <summary>Contract-enforcing wrapper for FastList{T} constructors.</summary>
    public static class FastListExtensions {
        /// <summary>Creates an <see cref="IFastList{T}"/> from a T[].</summary>
        /// <typeparam name="T">Type of the element in the supplied array and returned list.</typeparam>
        /// <param name="this">The T[] to be converted.</param>
        public static IFastList<T> ToFastList<T>(this T[] @this) => new FastList<T>(@this ?? new T[0]);

        /// <summary>Creates an <see cref="IFastList{T}"/> from a <see cref="List{T}"/>.</summary>
        /// <typeparam name="T">Type of the element in the supplied array and returned list.</typeparam>
        /// <param name="this">The T[] to be converted.</param>
        public static IFastList<T> ToFastList<T>(this List<T> @this) => (@this ?? new List<T>()).ToArray().ToFastList();

        /// <summary>TODO</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IFastList<T> ToFastList<T>(this IEnumerable<T> @this) => new FastList<T>(@this.ToArray());

        /// <summary>Creates an <see cref="IFastListX{T}"/> from a T[].</summary>
        /// <typeparam name="T">Type of the element in the supplied array and returned list.</typeparam>
        /// <param name="this">The T[] to be converted.</param>
        internal static IFastListX<T> ToFastListX<T>(this T[] @this) => new FastList<T>(@this ?? new T[0]);
    }
}
