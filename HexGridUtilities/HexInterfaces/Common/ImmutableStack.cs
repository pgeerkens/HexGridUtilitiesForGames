#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Collections;
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Eric Lippert's implementation for use in A*.</summary>
  /// <remarks>An implementation of immutable stack for use in A* as a 'Path to here'..</remarks>
  /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx">Path Finding Using A* Part THree</a>
  /// <typeparam name="T"></typeparam>
  public class ImmutableStackCollection<T> : IEnumerable<T> {

    /// <summary>Gets the top item on the stack.</summary>
    public T                 TopItem      { get; private set; }
    /// <summary>Gets the remainder of the stack.</summary>
    public ImmutableStackCollection<T> Remainder    { get; private set; }
    /// <summary>Returns a new ImmutableStack by adding <paramref name="item"/> to this stack.</summary>
    public ImmutableStackCollection<T> Push(T item) { return new ImmutableStackCollection<T>(item, this); }

    /// <summary>Construct a new empty instance.</summary>
    public ImmutableStackCollection(T start) : this(start, null) {}
    /// <summary>Construct a new instance by Push-ing <paramref name="item"/> onto <paramref name="remainder"/>.</summary>
    private ImmutableStackCollection(T item, ImmutableStackCollection<T> remainder) {
      TopItem   = item;
      Remainder = remainder;
    }

    /// <summary>Returns the stackitems in order from top to bottom.</summary>
    public IEnumerator<T> GetEnumerator() {
      for (ImmutableStackCollection<T> p = this; p != null; p = p.Remainder)  yield return p.TopItem;
    }

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
  }
}
