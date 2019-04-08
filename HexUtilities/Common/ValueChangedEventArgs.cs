#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    public class ValueChangedEventArgs<T> : EventArgs<T> {
        /// <summary>TODO</summary>
        public ValueChangedEventArgs(T value, T oldValue) : base(value) => OldValue = oldValue;

        /// <summary>TODO</summary>
        public T OldValue { get; }
    }
}
