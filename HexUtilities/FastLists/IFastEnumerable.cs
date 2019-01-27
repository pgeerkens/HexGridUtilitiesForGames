using System;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>Optimized ForEach.</summary>
  /// <typeparam name="TItem"></typeparam>
  /// <remarks>Adapted from 
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>:
  /// 
  /// The idea behind IFastEnumerable{TItem} (and specifically IFastEnumerator{TItem}) is to return
  /// the current element during the call to MoveNext itself.  This cuts the number of
  /// interface method calls necessary to enumerate a list in half.  The impact to performance
  /// isn’t huge, but it was enough to cut our overhead from about 3X to 2.3X.  Every little
  /// bit counts.
  /// </remarks>
  public interface IFastEnumerable<TItem> {
    /// <summary>Returns the items of a list in order.</summary>
    IFastEnumerator<TItem> GetEnumerator();
  }
}
