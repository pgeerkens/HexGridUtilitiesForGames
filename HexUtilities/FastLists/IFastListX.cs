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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>TODO</summary>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  [ContractClass(typeof(IFastListXContract<>))]
  internal interface IFastListX<T> : IFastList<T>  {
    void SetItem(int index, T value);
  }

  [ContractClassFor(typeof(IFastListX<>))]
  internal abstract class IFastListXContract<T> : IFastListX<T> {
    private IFastListXContract() { }

    public void SetItem(int index, T value) {
      Contract.Requires(0 <= index  &&  index < Count);
      Contract.Requires(value != null);

      Contract.Ensures(this[index] != null);
    }

    public int Count { get { return default(int); } }

    public abstract T   this[int index] { get; }
    public abstract int IndexOf(T item);

    public abstract void ForEach(Action<T> action);
    public abstract void ForEach(FastIteratorFunctor<T> functor);
    
    public abstract IEnumerator<T>        GetEnumerator();
    IEnumerator               IEnumerable.GetEnumerator() {return GetEnumerator(); }
    IFastEnumerator<T> IFastEnumerable<T>.GetEnumerator() {return default(IFastEnumerator<T>);}
  }
}
