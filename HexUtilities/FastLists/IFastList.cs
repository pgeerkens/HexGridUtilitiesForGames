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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>TODO</summary>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  [ContractClass(typeof(IFastListContract<>))]
  public interface IFastList<TItem> : IEnumerable<TItem>,
    IFastEnumerable<TItem>, IForEachable<TItem>, IForEachable2<TItem> {
    /// <summary>Gets the item at location <paramref name="index"/>.</summary>
    TItem this[int index] { get; }

    /// <summary>Returns the current size of the list.</summary>
    int   Count { get; }

    /// <summary>Returns the index of the specified item in the internal array storage.</summary>
    /// <param name="item"></param>
    int   IndexOf(TItem item);
  }

  /// <summary>Internal contract class for <see cref="IFastList{TItem}"/></summary>
  /// <typeparam name="TItem"></typeparam>
  [ContractClassFor(typeof(IFastList<>))]
  internal abstract class IFastListContract<TItem> : IFastList<TItem> {
    private IFastListContract() { }

    public int Count { get { return default(int); } }

    public TItem this[int index] { get {
        return default(TItem);
    } }
    public int   IndexOf(TItem item) {
      return default(int);
    }

    public abstract void  ForEach(Action<TItem> action);
    public abstract void  ForEach(FastIteratorFunctor<TItem> functor);
    
    public abstract IEnumerator<TItem>            GetEnumerator();
    IEnumerator                       IEnumerable.GetEnumerator() { return GetEnumerator(); }
    IFastEnumerator<TItem> IFastEnumerable<TItem>.GetEnumerator() { return null; }
  }
}
