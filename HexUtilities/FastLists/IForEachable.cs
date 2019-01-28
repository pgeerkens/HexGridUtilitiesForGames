using System;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>Delegated ForEach - <c>action</c> describes the work to be performed on each iteration.</summary>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  /// <remarks>Adapted from 
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>:
  /// 
  /// The IForEachable{T} interface is a push model: the caller provides a delegate and the 
  /// ForEach method is responsible for invoking it once per element in the collection.
  /// ForEach doesn’t return until this is done.  In addition to having far fewer method
  /// calls to enumerate a collection, neighbour isn’t a single interface method call.
  /// Delegate dispatch is also much faster than interface method dispatch.  The result is
  /// nearly twice as fast as the classic IEnumerator{T} pattern (when /o+ isn’t defined).  
  /// </remarks>
  public interface IForEachable<TItem>{
    /// <summary>Perform the supplied <paramref name="action"/> for every item in the enumeration.</summary>
    void ForEach(Action<TItem> action);
  }
}
