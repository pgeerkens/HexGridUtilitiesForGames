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

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>A Maybe monad wrapping an instance of type <typeparamref name="T"/>.</summary>
    public struct Maybe<T> : IEquatable<Maybe<T>> {
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static Maybe<T> NoValue() => default;

        /// <summary>TODO</summary>
        public Maybe(T value) : this() => Value    = value;

        /// <summary>TODO</summary>
        public  bool HasValue => Value != null;
        private T    Value    { get; }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        
        public Maybe<TOut> Bind<TOut>(Func<T, Maybe<TOut>> projection)
        => HasValue ? projection(Value) : Maybe<TOut>.NoValue();

        /// <summary>TODO</summary>
        public TOut        Match<TOut>(Func<T,TOut> projection, Func<TOut> alternate)
        => HasValue ? projection(Value) : alternate();

        /// <summary>TODO</summary>
        public bool        ValueContract(Func<T, bool> contract) => ! HasValue || contract(Value);
        
        private static string PreferredName(T value) => value.ToString();
        
        private static string AlternateName() => "NoValue<" + typeof(T).Name + ">";

        /// <summary>TODO</summary>
        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        #region Value Equality with IEquatable<T>
        /// <summary>Tests value-equality.</summary>
        public override bool Equals(object obj) => (obj is Maybe<T> other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(Maybe<T> other)
        => ( HasValue  &&  other.HasValue  &&  Value.Equals(other.Value) )
        || (!HasValue  && !other.HasValue);

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator != (Maybe<T> lhs, Maybe<T> rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator == (Maybe<T> lhs, Maybe<T> rhs) => lhs.Equals(rhs);

        /// <summary>TODO</summary>
        public override string ToString()
        => (HasValue && Value!=null) ? PreferredName(Value) : AlternateName();
        #endregion

        /// <summary>TODO</summary>
        public static Maybe<V> ToMaybe<V>(Maybe<V?> maybe) where V:struct
        => ! maybe.HasValue || ! maybe.Value.HasValue ? Maybe<V>.NoValue() : maybe.Value.Value;

        /// <summary>TODO</summary>
        public static V? ToNullable<V>(Maybe<V?> maybe) where V:struct
        => ! maybe.HasValue || ! maybe.Value.HasValue ? (V?)null : maybe.Value.Value;
    }
}
