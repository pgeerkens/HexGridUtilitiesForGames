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
    public static class MaybeExtensions {
        /// <summary>Returns the value of <paramref name="this">this</paramref> it it has one; otherwise returns <paramref name="alternate"/>().</summary>
        /// <param name="this">The <see cref="Maybe{T}"/> being operated upon.</param>
        /// <param name="alternate">The action to be perrofmed if <paramref name="this"/> has no value.</param>
        [Pure]public static TOut        Else<TOut>(this Maybe<TOut> @this, Func<TOut> alternate) {
            alternate.RequiredNotNull("alternate");
            return @this.Match(e => e, alternate); 
        }
        /// <summary>Returns the value of <paramref name="this">this</paramref> it it has one; otherwise returns default(<typeparamref name="TOut"/>).().</summary>
        /// <param name="this">The <see cref="Maybe{T}"/> being operated upon.</param>
        [Pure]public static TOut        ElseDefault<TOut>(this Maybe<TOut> @this) =>
            @this.Match(e => e, ()=>default(TOut)); 

        /// <summary>Executes <paramref name="action"/> on <paramref name="this"/> exactly if it hasn't any value.</summary>
        /// <returns>Returns <paramref name="this"/>.</returns>
        [Pure]public static Maybe<TOut> ElseDo<TOut>(this Maybe<TOut> @this, Action action) {
            action.RequiredNotNull("action");
            return @this.Match(value => @this, () => { action(); return @this; });
        }

        /// <summary>Returns the value of <paramref name="this"/> if it has one, otherwise throws an <see cref="InvalidOperationException"/>.</summary>
        /// <returns>Returns the value of <paramref name="this"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [Pure]public static TOut        ForceGetValue<TOut>(this Maybe<TOut> @this) {
            Contract.Ensures(Contract.Result<TOut>() != null);
            Contract.EnsuresOnThrow<InvalidOperationException>( ! @this.HasValue);
            return @this.Match(e => e, () => { throw new InvalidOperationException(@this.ToString()); } );
        }

        /// <summary>Executes <paramref name="action"/> on <paramref name="this"/> exactly if it has a value.</summary>
        /// <returns>Returns <paramref name="this"/>.</returns>
        [Pure]public static Maybe<TOut> IfHasValueDo<TOut>(this Maybe<TOut> @this, Action<TOut> action) {
            action.RequiredNotNull("action");
            return @this.Match(value => { action(value); return @this; }, () => @this);
        }

        /// <summary>Projects the value of a <see cref="Maybe{T}"/> into a <see cref="Maybe{TOut}"/>.</summary>
        [Pure]public static Maybe<TOut> Select<T,TOut>(this Maybe<T> @this, Func<T,TOut> projection) {
            projection.RequiredNotNull("projection");
            return @this.Bind<TOut>(value => projection(value));
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
        [Pure]public static Maybe<TOut> SelectMany<T, TMid, TOut>(this Maybe<T> @this, 
                                                      Func<T, Maybe<TMid>> maybeProjection, 
                                                      Func<T,TMid,TOut>    resultProjection) {
          maybeProjection.RequiredNotNull("maybeProjection");
          resultProjection.RequiredNotNull("resultProjection");
          return @this.Bind<TOut>(              a =>
                 maybeProjection(a).Bind<TOut>( b =>
                 resultProjection(a,b)));
        }

        /// <summary>Returns a new <see cref="Maybe{T}"/> instance with value <paramref name="this"/>.</summary>
        /// <returns>Returns <see cref="Maybe{T}.NoValue"/>() if <paramref name="this"/> is null.</returns>
        [Pure]public static Maybe<TOut> ToMaybe<TOut>(this TOut @this) => new Maybe<TOut>(@this);

        /// <summary>TODO</summary>
        [Pure]public static Maybe<TOut> Where<TOut>(this Maybe<TOut> @this, Func<TOut, bool> predicate) {
          predicate.RequiredNotNull("predicate");
          return @this.Bind(e => predicate(e) ? e.ToMaybe() : default(Maybe<TOut>) );
        }

        /// <summary>TODO</summary>
        [Pure]public static int CompareTo<TOut>(this Maybe<TOut> @this, Maybe<TOut> other) where TOut : IComparable =>
            (from lhs in @this from rhs in other select lhs.CompareTo(rhs)).ElseDefault();
        /// <summary>TODO</summary>
        [Pure]public static Maybe<TOut> Max<TOut>(this Maybe<TOut> @this, Maybe<TOut> other) where TOut : IComparable =>
            ( from lhs in @this from rhs in other let comparison = lhs.CompareTo(rhs)
              select comparison < 0 ? lhs : rhs
            ).ToMaybe().Else(Maybe<TOut>.NoValue);
    }
}
 