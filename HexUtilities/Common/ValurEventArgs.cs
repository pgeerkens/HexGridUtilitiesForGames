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

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>TODO</summary>
  /// <typeparam name="T"></typeparam>
  public class ValueEventArgs<T> : EventArgs {
    /// <summary>TODO</summary>
    public ValueEventArgs(T value) : base() { _value = value; }
    /// <summary>TODO</summary>
    public T Value { get {return _value;} } readonly T _value;
  }

  /// <summary>TODO</summary>
  /// <typeparam name="T"></typeparam>
  public class ValueChangedEventArgs<T> : ValueEventArgs<T> {
    /// <summary>TODO</summary>
    public ValueChangedEventArgs(T value, T oldValue) : base(value) {
      _oldValue = oldValue;
    }
    /// <summary>TODO</summary>
    public T OldValue { get {return _oldValue;} } readonly T _oldValue;
  }
}
