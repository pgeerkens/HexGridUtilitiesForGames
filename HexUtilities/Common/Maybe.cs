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
    /// <summary>A Maybe monad wrapping an instance of type<typeparam name="T">T</typeparam>.</summary>
    public struct Maybe<T> : IEquatable<Maybe<T>> {
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static Maybe<T> NoValue() => default(Maybe<T>);

        /// <summary>TODO</summary>
        public Maybe(T value) : this() {
          //  Contract.Ensures( !HasValue  ||  Value != null);
            Value    = value;
            HasValue = typeof(ValueType).IsAssignableFrom(typeof(T)) || value != null;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void ObjectInvariant() => Contract.Invariant(! HasValue  ||  Value != null);
       

        /// <summary>TODO</summary>
        public  bool HasValue { get; }
        /// <summary>TODO</summary>
        private T    Value    { get;}

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [Pure]
        public Maybe<TOut> Bind<TOut>(Func<T, Maybe<TOut>> projection) {
            projection.RequiredNotNull("projection");
            return HasValue ? projection(Value) : Maybe<TOut>.NoValue();
        }

        /// <summary>TODO</summary>
        [Pure]
        public TOut        Match<TOut>(Func<T,TOut> projection, Func<TOut> alternate) {
            projection.RequiredNotNull("projection");
            alternate.RequiredNotNull("alternate");
          //  Contract.Ensures(Contract.Result<TOut>() != null || alternate() == null);

            return HasValue ? projection(Value) : alternate();
        }

        /// <summary>TODO</summary>
        [Pure]
        public bool        ValueContract(Func<T, bool> contract) {
            contract.RequiredNotNull("contract");
          //  Contract.Ensures(Contract.Result<bool>() == (! HasValue  ||  contract(Value)) );
            return ! HasValue  ||  contract(Value);
        }
        [Pure]
        private static  string PreferredName(T value) {
            value.RequiredNotNull("value");
          //  Contract.Ensures(Contract.Result<string>() != null);
            return value.ToString();
        }
        [Pure]
        private static  string AlternateName() {
          //  Contract.Ensures(Contract.Result<string>() != null);
            return "NoValue<" + typeof(T).Name + ">";
        }

        /// <summary>TODO</summary>
        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        #region Value Equality with IEquatable<T>
        /// <summary>Tests value-equality.</summary>
        [Pure]
        public override bool Equals(object obj) => (obj as Maybe<T>?)?.Equals(this) ?? false;

        /// <inheritdoc/>
        [Pure]
        public bool Equals(Maybe<T> other) => 
                ( HasValue  &&  other.HasValue  &&  (Value.Equals(other.Value)) )
             || (!HasValue  && !other.HasValue);

        /// <summary>Tests value-inequality.</summary>
        [Pure]
        public static bool operator != (Maybe<T> lhs, Maybe<T> rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        [Pure]
        public static bool operator == (Maybe<T> lhs, Maybe<T> rhs) => lhs.Equals(rhs);

        /// <summary>TODO</summary>
        [Pure]
        public override string ToString() {
          //  Contract.Ensures(Contract.Result<string>() != null);
            return (HasValue && Value!=null) ? PreferredName(Value) : AlternateName();
        }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => Value.GetHashCode();
        #endregion

        /// <summary>TODO</summary>
        public static Maybe<V> ToMaybe<V>(Maybe<V?> maybe) where V : struct =>
            ! maybe.HasValue || ! maybe.Value.HasValue ? Maybe<V>.NoValue() : maybe.Value.Value;

        /// <summary>TODO</summary>
        public static V? ToNullable<V>(Maybe<V?> maybe) where V : struct =>
            ! maybe.HasValue || ! maybe.Value.HasValue ? (V?)null : maybe.Value.Value;
    }
}
