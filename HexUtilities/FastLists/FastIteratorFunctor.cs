using System;
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>Abstract base class for a <c>FastList</c> functor.</summary>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  /// <remarks>Adapted from 
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>:
  /// </remarks>
  [DebuggerDisplay("Count={Count}")]
  public abstract class FastIteratorFunctor<TItem> {
    /// <summary>Perform the action associated with this functor on <paramref name="item"/>.</summary>
    public abstract void Invoke(TItem item);
  }
}
