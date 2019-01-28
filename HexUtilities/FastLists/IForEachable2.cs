using System;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>Functored ForEach - <c>functor</c> describes the work to be performed on each iteration.</summary>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  /// <remarks>Adapted from 
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>:
  /// 
  /// Clients will pass an instance of the Functor{T} class with the Invoke method
  /// overridden.  The implementation of ForEach then looks quite a bit like 
  /// IForEachable{T}’s, just with virtual method calls in place of delegate calls:
  /// </remarks>
  [ContractClass(typeof(IForEachable2Contract<>))]
  public interface IForEachable2<TItem> {
    /// <summary>Perform the action specified by <paramref name="functor"/> for every item in the enumeration.</summary>
    void ForEach(FastIteratorFunctor<TItem> functor);
  }

  /// <summary>Internal contract-class for <see cref="IForEachable2{TItem}"/></summary>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  [ContractClassFor(typeof(IForEachable2<>))]
  internal abstract class IForEachable2Contract<TItem> : IForEachable2<TItem> {
    public   void  ForEach(FastIteratorFunctor<TItem> functor) {
    }
  }
}
