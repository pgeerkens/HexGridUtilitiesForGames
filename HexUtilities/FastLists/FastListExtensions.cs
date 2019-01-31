#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>Contract-enforcing wrapper for FastList{T} constructors.</summary>
  public static class FastList {
    /// <summary>Contract-enforcing wrapper for FastList{T} constructor returning an IFastList{TItem}.</summary>
    /// <typeparam name="TItem">Type of the class/interface being indexed.</typeparam>
    /// <param name="array">An <c>T[]</c> to be indexed.</param>
    public static IFastList<TItem> New<TItem>(TItem[] array) { 
      return new FastList<TItem>(array);
    }
    /// <summary>Contract-enforcing wrapper for FastList{T} constructor returning an IFastList{TItem}.</summary>
    /// <typeparam name="TItem">Type of the class/interface being indexed.</typeparam>
    /// <param name="array">An <c>T[]</c> to be indexed.</param>
    internal static IFastListX<TItem> NewX<TItem>(TItem[] array) { 
      return new FastList<TItem>(array);
    }
  }

  /// <summary>TODO</summary>
  public static class FastListExtensions {
    /// <summary>Creates an <see cref="IFastList{T}"/> from a T[].</summary>
    /// <typeparam name="T">Type of the element in the supplied array and returned list.</typeparam>
    /// <param name="this">The T[] to be converted.</param>
    public static IFastList<T> ToFastList<T>(this T[] @this) {

      return FastList.New(@this ?? new T[0]);
    }
    ///// <summary>Creates an <see cref="IFastList{T}"/> from a <see cref="List{T}"/>.</summary>
    ///// <typeparam name="T">Type of the element in the supplied array and returned list.</typeparam>
    ///// <param name="this">The T[] to be converted.</param>
    //public static IFastList<T> ToFastList<T>(this List<T> @this) {

    //  return ( @this ?? new List<T>()).ToArray().ToFastList();
    //}

    /// <summary>Creates an <see cref="IFastListX{T}"/> from a T[].</summary>
    /// <typeparam name="T">Type of the element in the supplied array and returned list.</typeparam>
    /// <param name="this">The T[] to be converted.</param>
    internal static IFastListX<T> ToFastListX<T>(this T[] @this) {

      return FastList.NewX(@this ?? new T[0]);
    }
  }
}
