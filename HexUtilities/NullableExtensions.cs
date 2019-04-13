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
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities {
    /// <summary>TODO</summary>
    public static partial class NullableExtensions {
        /// <summary>Returns the value of <paramref name="this">this</paramref> it it has one; otherwise returns <paramref name="alternate"/>().</summary>
        /// <typeparam name="TOut">The struct basis for the rtuern type TOut?.</typeparam>
        /// <param name="this">The <typeparamref name="TOut"/>? being operated upon.</param>
        /// <param name="alternate">The action to be perrofmed if <paramref name="this"/> has no value.</param>
        public static TOut? Else<TOut>(this TOut? @this, Func<TOut> alternate) where TOut:struct
        => @this.Match(e => e, alternate); 

        /// <summary>Returns the value of <paramref name="this">this</paramref> it it has one; otherwise returns default(<typeparamref name="TOut"/>).</summary>
        /// <typeparam name="TOut">The struct basis for the rtuern type TOut?.</typeparam>
        /// <param name="this">The <see cref="Maybe{T}"/> being operated upon.</param>
        public static TOut  ElseDefault<TOut>(this TOut? @this) where TOut:struct
        => @this.Match(e => e, ()=>default(TOut)); 

        /// <summary>Executes <paramref name="action"/> on <paramref name="this"/> exactly if it hasn't any value.</summary>
        /// <typeparam name="TOut">The struct basis for the rtuern type TOut?.</typeparam>
        /// <returns>Returns <paramref name="this"/>.</returns>
        public static TOut? ElseDo<TOut>(this TOut? @this, Action action) where TOut:struct
        => @this.Match(value => @this, () => { action(); return @this; });

        /// <summary>Returns the value of <paramref name="this"/> if it has one, otherwise throws an <see cref="InvalidOperationException"/>.</summary>
        /// <typeparam name="TOut">The struct basis for the rtuern type TOut?.</typeparam>
        /// <returns>Returns the value of <paramref name="this"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TOut  ForceGetValue<TOut>(this TOut? @this) where TOut:struct
        => @this.Match(e => e, () => { throw new InvalidOperationException(@this.ToString()); } );

        /// <summary>Executes <paramref name="action"/> on <paramref name="this"/> exactly if it has a value.</summary>
        /// <typeparam name="TOut">The struct basis for the rtuern type TOut?.</typeparam>
        /// <returns>Returns <paramref name="this"/>.</returns>
        public static TOut? IfHasValueDo<TOut>(this TOut? @this, Action<TOut> action)
        where TOut:struct
        => @this.HasValue ? @this.Match(value => { action(value); return @this.Value; }, () => @this.Value)
                          : null as TOut?;

        /// <summary>Projects the value of a <see cref="Nullable{T}"/> into a <see cref="Nullable{TOut}"/>.</summary>
        public static TOut? Select<T,TOut>(this T? @this, Func<T,TOut> projection)
        where T:struct where TOut:struct
        => @this.Bind<T,TOut>(value => projection(value));

        /// <summary>Projects the value of a <see cref="Nullable{T}"/> into a <see cref="Nullable{TOut}"/>.</summary>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures" )]
        public static TOut? SelectMany<T, TOut>(this T? @this,
                                                Func<T, TOut?> projection)
        where T:struct where TOut:struct
        => @this.Bind(value => projection(value));

        /// <summary>Projects the value of a <see cref="Nullable{T}"/> into a <see cref="Nullable{TOut}"/>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static TOut? SelectMany<T,TMid,TOut>(this T? @this, 
                                                    Func<T,TMid?> maybeProjection, 
                                                    Func<T,TMid,TOut> resultProjection)
        where T:struct where TMid:struct where TOut:struct
        => @this.Bind(                         a =>
           maybeProjection(a).Bind<TMid,TOut>( b =>
           resultProjection(a,b)));

        /// <summary>TODO</summary>
        public static TOut? Where<TOut>(this TOut? @this, Func<TOut, bool> predicate) where TOut:struct
        => @this.Bind(e => predicate(e) ? e : default(TOut?) );

        /// <summary>TODO</summary>
        public static int? CompareTo<TOut>(this TOut? @this, TOut? other) where TOut : struct,IComparable
        => (from lhs in @this from rhs in other select lhs.CompareTo(rhs)).ElseDefault();

        /// <summary>TODO</summary>
        public static TOut? Max<TOut>(this TOut? @this, TOut? other) where TOut:struct, IComparable
        => from lhs in @this from rhs in other select lhs.CompareTo(rhs) < 0 ? lhs : rhs;

        /// <summary>TODO</summary>
        public static Maybe<T> ToMaybe<T>(this Maybe<T?> maybe) where T:struct
        => Maybe<T>.ToMaybe(maybe);

        /// <summary>TODO</summary>
        public static TResult? Bind<TValue,TResult>(this TValue? @this, Func<TValue, TResult?> projection)
        where TValue:struct where TResult:struct
        => @this.HasValue ? projection(@this.Value) : null;

        /// <summary>TODO</summary>
        public static TOut  Match<T,TOut>(this T? @this, Func<T,TOut> projection, Func<TOut> alternate) where T:struct
        => @this.HasValue ? projection(@this.Value) : alternate();
    }
}
 