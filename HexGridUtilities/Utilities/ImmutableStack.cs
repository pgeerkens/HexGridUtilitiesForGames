#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities {
  /// <summary>Eric Lippert's implementation for use in A*.</summary>
  /// <remarks>An implementation of immutable stack for use in A* as a 'Path to here'..</remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx</cref>
  /// <typeparam name="T"></typeparam>
  public class ImmutableStack<T> : IEnumerable<T> {

    public T                 TopItem      { get; protected set; }
    public ImmutableStack<T> Remainder    { get; protected set; }
    public ImmutableStack<T> Push(T step) { return new ImmutableStack<T>(step, this); }

    public ImmutableStack(T start) : this(start, null) {}
    protected ImmutableStack(T topItem, ImmutableStack<T> remainder) {
      TopItem   = topItem;
      Remainder = remainder;
    }

    public IEnumerator<T> GetEnumerator() {
      for (ImmutableStack<T> p = this; p != null; p = p.Remainder)  yield return p.TopItem;
    }

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
  }
}
