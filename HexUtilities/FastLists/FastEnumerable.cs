#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.FastLists {
  public abstract partial class AbstractFastList<TItem> {
    /// <summary>Implements IEnumerable{TItem} in the <i><b>fast</b></i> way:</summary>
    /// <typeparam name="TItem2">Type of the objects being enumerated.</typeparam>
    /// <remarks>Adapted from 
    /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
    /// The Cost of Enumeration in DotNet</a>
    /// </remarks>
    [DebuggerDisplay("Count={Count}")]
    [SuppressMessage("Microsoft.Contracts", "Ensures-!Contract.Result<bool>()  ||  (Contract.ValueAtReturn<TItem>(out item) != null)")]
    internal sealed partial class FastEnumerable<TItem2> : IFastEnumerator<TItem2> {
      private readonly TItem2[] _array;       //!< Array being enumerated..
      private          int      _index = -1;  //!< Index of the currently-enumerated element.

      /// <summary>Return the next item in the enumeration.</summary>
      /// <remarks>
      /// Adopts a well-recognized JIT pattern to ensure redundant array-bounds-check is optimized out.
      /// </remarks>
      public bool MoveNext(ref TItem2 item) {
        var array = _array;
        int i;

        if ((i = ++_index) >= array.Length) return false;

        item = array[i];

        return true;
      }
    }
  }
}
