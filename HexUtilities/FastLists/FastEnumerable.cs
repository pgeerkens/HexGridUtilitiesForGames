using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

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
      //  Contract.Assume(Contract.Result<bool>() == (_index < array.Length));
      //  Contract.Assume(!Contract.Result<bool>() || item != null);

        if ((i = ++_index) >= array.Length) return false;

        item = array[i];

        return true;
      }
    }
  }
}
