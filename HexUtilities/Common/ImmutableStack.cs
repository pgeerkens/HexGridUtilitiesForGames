#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>Eric Lippert's implementation for use in A*.</summary>
    /// <remarks>An implementation of immutable stack for use in A* as a 'Path to here'..</remarks>
    /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx">Path Finding Using A* Part THree</a>
    /// <typeparam name="T"></typeparam>
    public class ImmutableStack<T> : IEnumerable<T> {
        /// <summary>Construct a new empty instance.</summary>
        public ImmutableStack(T start) : this(start, null) {}

        /// <summary>Construct a new instance by Push-ing <paramref name="item"/> onto <paramref name="remainder"/>.</summary>
        private ImmutableStack(T item, ImmutableStack<T> remainder) {
            TopItem   = item;
            Remainder = remainder;
        }

        /// <summary>Gets the top item on the stack.</summary>
        public T                 TopItem      { get; }

        /// <summary>Gets the remainder of the stack.</summary>
        public ImmutableStack<T> Remainder    { get; }

        /// <summary>Returns a new ImmutableStack by adding <paramref name="item"/> to this stack.</summary>
        public ImmutableStack<T> Push(T item) => new ImmutableStack<T>(item,this);

        /// <summary>Returns the stackitems in order from top to bottom.</summary>
        public IEnumerator<T> GetEnumerator() {
            for (var p = this; p != null; p = p.Remainder) { yield return p.TopItem; }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
