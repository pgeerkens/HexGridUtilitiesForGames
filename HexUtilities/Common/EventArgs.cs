#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs {
        /// <summary>TODO</summary>
        public EventArgs() : this(default(T)) { }
        /// <summary>TODO</summary>
        public EventArgs(T value) : base() => Value = value;
        /// <summary>TODO</summary>
        public T Value { get; }
    }
}
