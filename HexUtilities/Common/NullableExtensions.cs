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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>TODO</summary>
    public static partial class NullableExtensions {
        /// <summary>TODO</summary>
        public static TResult? Bind<TValue,TResult>(this TValue? @this, Func<TValue, TResult?> projection)
            where TValue:struct where TResult:struct =>
            @this.HasValue ? projection(@this.Value) : null;

        /// <summary>TODO</summary>
        public static Maybe<T> ToMaybe<T>(this Maybe<T?> maybe) where T:struct =>
            Maybe<T>.ToMaybe(maybe);

        /// <summary>TODO</summary>
        public static T? ToNullable<T>(this Maybe<T?> maybe) where T:struct =>
            Maybe<T>.ToNullable(maybe);

        /// <summary>TODO</summary>
        [Pure]public static TOut  Match<T,TOut>(this T? @this, Func<T,TOut> projection, Func<TOut> alternate) where T:struct {
            projection.RequiredNotNull("projection");
            alternate.RequiredNotNull("alternate");
          //  Contract.Ensures(Contract.Result<TOut>() != null || alternate() == null);

            return @this.HasValue ? projection(@this.Value) : alternate();
        }

        /// <summary>Returns the value of <paramref name="this">this</paramref> it it has one; otherwise returns <paramref name="alternate"/>().</summary>
        /// <param name="this">The <see cref="{TOut}?"/> being operated upon.</param>
        /// <param name="alternate">The action to be perrofmed if <paramref name="this"/> has no value.</param>
        [Pure]public static TOut  Else<TOut>(this TOut? @this, Func<TOut> alternate) where TOut:struct {
            alternate.RequiredNotNull("alternate");
            return @this.Match(e => e, alternate); 
        }
        /// <summary>Returns the value of <paramref name="this">this</paramref> it it has one; otherwise returns default(<typeparamref name="TOut"/>).().</summary>
        /// <param name="this">The <see cref="Maybe{T}"/> being operated upon.</param>
        [Pure]public static TOut  ElseDefault<TOut>(this TOut? @this) where TOut:struct =>
            @this.Match(e => e, ()=>default(TOut)); 

        /// <summary>Executes <paramref name="action"/> on <paramref name="this"/> exactly if it hasn't any value.</summary>
        /// <returns>Returns <paramref name="this"/>.</returns>
        [Pure]public static TOut? ElseDo<TOut>(this TOut? @this, Action action) where TOut:struct {
            action.RequiredNotNull("action");
            return @this.Match(value => @this, () => { action(); return @this; });
        }

        /// <summary>Returns the value of <paramref name="this"/> if it has one, otherwise throws an <see cref="InvalidOperationException"/>.</summary>
        /// <returns>Returns the value of <paramref name="this"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [Pure]public static TOut  ForceGetValue<TOut>(this TOut? @this) where TOut:struct {
            //Contract.Ensures(Contract.Result<TOut>() != null);
          //  Contract.EnsuresOnThrow<InvalidOperationException>( ! @this.HasValue);
            return @this.Match(e => e, () => { throw new InvalidOperationException(@this.ToString()); } );
        }

        /// <summary>Executes <paramref name="action"/> on <paramref name="this"/> exactly if it has a value.</summary>
        /// <returns>Returns <paramref name="this"/>.</returns>
        [Pure]public static TOut? IfHasValueDo<TOut>(this TOut? @this, Action<TOut> action) where TOut:struct {
            action.RequiredNotNull("action");
            return @this.HasValue ? @this.Match(value => { action(value); return @this.Value; }, () => @this.Value)
                                  : null as TOut?;
        }

        /// <summary>Projects the value of a <see cref="{T}?"/> into a <see cref="{TOut}?"/>.</summary>
        [Pure]public static TOut? Select<T,TOut>(this T? @this, Func<T,TOut> projection) where T:struct where TOut:struct {
            projection.RequiredNotNull("projection");
            return @this.Bind<T,TOut>(value => projection(value));
        }

        ///// <summary>Projects the value of a <see cref="Maybe{T}"/> into a <see cref="Maybe{TOut}"/>.</summary>
        //[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        //[Pure]public static Maybe<TOut> SelectMany<T, TOut>(this Maybe<T> @this, 
        //                                              Func<T,Maybe<TOut>>    projection) {
        //  projection.RequiredNotNull("projection");
        //  return @this.Bind<TOut>(value => projection(value));
        //}

        /// <summary>Projects the value of a <see cref="Maybe{T}"/> into a <see cref="Maybe{TOut}"/>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [Pure]public static TOut? SelectMany<T, TMid, TOut>(this T? @this, 
                                                Func<T,TMid?> maybeProjection, 
                                                Func<T,TMid,TOut>    resultProjection)
        where T:struct where TMid:struct where TOut:struct {
              maybeProjection.RequiredNotNull("maybeProjection");
              resultProjection.RequiredNotNull("resultProjection");
              return @this.Bind(              a =>
                    maybeProjection(a).Bind<TMid,TOut>( b =>
                    resultProjection(a,b)));
        }

#if false
        /// <summary>Returns a new <see cref="Maybe{T}"/> instance with value <paramref name="this"/>.</summary>
        /// <returns>Returns <see cref="Maybe{T}.NoValue"/>() if <paramref name="this"/> is null.</returns>
        [Pure]public static TOut? ToMaybe<TOut>(this TOut @this) where TOut:struct => (TOut?)(@this);

#endif
        /// <summary>TODO</summary>
        [Pure]public static TOut? Where<TOut>(this TOut? @this, Func<TOut, bool> predicate) where TOut:struct {
          predicate.RequiredNotNull("predicate");
          return @this.Bind(e => predicate(e) ? e : default(TOut?) );
        }

        /// <summary>TODO</summary>
        [Pure]public static int CompareTo<TOut>(this TOut? @this, TOut? other) where TOut : struct,IComparable =>
            (from lhs in @this from rhs in other select lhs.CompareTo(rhs)).ElseDefault();
        ///// <summary>TODO</summary>
        //[Pure]public static Maybe<TOut> Max<TOut>(this TOut? @this, TOut? other) where TOut : struct,IComparable =>
        //    ( from lhs in @this from rhs in other let comparison = lhs.CompareTo(rhs)
        //      select comparison < 0 ? lhs : rhs
        //    ).ToMaybe().Else(default(TOut?));
    }
}
 