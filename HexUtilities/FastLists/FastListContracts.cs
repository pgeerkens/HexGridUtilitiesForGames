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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGNapoleonics.HexUtilities.FastLists {
  public abstract partial class AbstractFastList<TItem> {
    /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
    protected AbstractFastList(TItem[] array) { 
      _array = array;
      Contract.Assert(_array.Length == Count);
    }

    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    }

    private sealed partial class ClassicEnumerable<TItem2> : IEnumerator<TItem2> {
      /// <summary>Construct a new instance from array <c>a</c>.</summary>
      /// <param name="array">The array of type <c>TItem</c> to make enumerable.</param>
      internal ClassicEnumerable(TItem2[] array) {
        _array = array;
      }

      [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      private void ObjectInvariant() {
      }
    }

    internal sealed partial class FastEnumerable<TItem2> {
      /// <summary>Construct a new instance from array <c>a</c>.</summary>
      /// <param name="array">The array of type <c>TItem</c> to make enumerable.</param>
      internal FastEnumerable(TItem2[] array) {
        _array = array;
      }

      [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      private void ObjectInvariant() {
      }
    }
  }
}
