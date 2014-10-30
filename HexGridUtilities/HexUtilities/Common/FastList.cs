using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1587
/// <summary>Joe Duffy's Simple (Fast) List enumerator.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.Common.FastIterator {
  /// <summary>Adapted implementation of Joe Duffy's Simple (Fast) List enumerator.</summary>
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [DebuggerDisplay("Count={Count}")]
  public sealed class FastList<TItem> : IEnumerable<TItem>
      ,IFastEnumerable<TItem> ,IForEachable<TItem>, IForEachable2<TItem>
  {
    private TItem[] _array;

    /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
    public FastList(TItem[] array) { _array = array; }

    IEnumerator                       IEnumerable.GetEnumerator(){
      return new ClassicEnumerable<TItem>(_array);
    }
    IEnumerator<TItem>         IEnumerable<TItem>.GetEnumerator(){
      return new ClassicEnumerable<TItem>(_array);
    }
    IFastEnumerator<TItem> IFastEnumerable<TItem>.GetEnumerator(){
      return new FastEnumerable<TItem>(_array);
    }

    /// <summary>IForEachable{TItem} implementation.</summary>
    public void ForEach(Action<TItem> action) {
#if CHECKED
      if (action==null) throw new ArgumentNullException("action");
#endif
      TItem[] a = _array;
      for (int i = 0; i < a.Length; i++)    action(a[i]);
    }

    /// <summary>IForEachable2{TItem} implementation</summary>
    public void ForEach(FastIteratorFunctor<TItem> functor) {
#if CHECKED
      if (functor==null) throw new ArgumentNullException("functor");
#endif
      TItem[] a = _array;
      for (int i = 0; i < a.Length; i++)    functor.Invoke(a[i]);
    }

    /// <summary>Gets the item at location <paramref name="index"/>.</summary>
    public TItem this[int index] { get { return _array[index]; } }
  }

  /// <summary>Optimized ForEach.</summary>
  /// <remarks>
  /// The idea behind IFastEnumerable{TItem} (and specifically IFastEnumerator{TItem}) is to return
  /// the current element during the call to MoveNext itself.  This cuts the number of
  /// interface method calls necessary to enumerate a list in half.  The impact to performance
  /// isn’t huge, but it was enough to cut our overhead from about 3X to 2.3X.  Every little
  /// bit counts.
  /// </remarks>
  /// <typeparam name="TItem"></typeparam>
  public interface IFastEnumerable<TItem> {
    /// <summary>Returns the items of a list in order.</summary>
    IFastEnumerator<TItem> GetEnumerator();
  }
    /// <summary>Returns the items of a list in order.</summary>
  public interface IFastEnumerator<T>{
    /// <summary>Return the next item in the enumeration.</summary>
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
    bool MoveNext(ref T item);
  }

  /// <summary>Delegated ForEach - <c>action</c> describes the work to be performed on each iteration.</summary>
  /// <remarks>
  /// The IForEachable{T} interface is a push model: the caller provides a delegate and the 
  /// ForEach method is responsible for invoking it once per element in the collection.
  /// ForEach doesn’t return until this is done.  In addition to having far fewer method
  /// calls to enumerate a collection, there isn’t a single interface method call.
  /// Delegate dispatch is also much faster than interface method dispatch.  The result is
  /// nearly twice as fast as the classic IEnumerator{T} pattern (when /o+ isn’t defined).  
  /// Now we’re really getting somewhere!
  /// </remarks>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  public interface IForEachable<TItem>{
    /// <summary>Perform the supplied <paramref name="action"/> for every item in the enumeration.</summary>
    void ForEach(Action<TItem> action);
  }

  /// <summary>Functored ForEach - <c>functor</c> describes the work to be performed on each iteration.</summary>
  /// <remarks>
  /// Clients will pass an instance of the Functor{T} class with the Invoke method
  /// overridden.  The implementation of ForEach then looks quite a bit like 
  /// IForEachable{T}’s, just with virtual method calls in place of delegate calls:
  /// </remarks>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  public interface IForEachable2<TItem> {
    /// <summary>Perform the action specified by <paramref name="functor"/> for every item in the enumeration.</summary>
    void ForEach(FastIteratorFunctor<TItem> functor);
  }

  /// <summary>Abstract base class for a <c>FastList</c> functor.</summary>
  /// <typeparam name="TItem">The type of object being iterated.</typeparam>
  [DebuggerDisplay("Count={Count}")]
  public abstract class FastIteratorFunctor<TItem>{
    /// <summary>Perform the action associated with this functor on <paramref name="item"/>.</summary>
    public abstract void Invoke(TItem item);
  }

  /// <summary>Implements IEnumerable{TItem} in the <i><b>standard</b></i> way:</summary>
  /// <typeparam name="TItem">Type of the objects being enumerated.</typeparam>
  [DebuggerDisplay("Count={Count}")]
  public class ClassicEnumerable<TItem> : IEnumerator<TItem>, IDisposable {
    internal ClassicEnumerable(TItem[] a) { _a = a; }

    private TItem[] _a;
    private int     _index = -1;

    /// <summary>Return the next item in the enumeration.</summary>
    public bool   MoveNext() { return ++_index < _a.Length; }

    /// <summary>Return the current item in the enumeration</summary>
    public TItem       Current  { get { return _a[_index]; } }
    object IEnumerator.Current  { get { return Current; } }

    /// <summary>Reset the enumerator to the start of the enumeration.</summary>
    public void Reset() { _index = -1; }

    #region IDisposable implementation with Finalizer
    private bool isDisposed = false;  //!<True if already Disposed.
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Utility routine to do the hard-lifting for Dispose().</summary>
    protected virtual void Dispose(bool disposing) {
      if (!isDisposed) {
        if (disposing) {
        }
      }
    }
    /// <summary>Destructor.</summary>
    ~ClassicEnumerable() { Dispose(false); }
    #endregion
  }

  /// <summary>Implements IEnumerable{TItem} in the <i><b>fast</b></i> way:</summary>
  /// <typeparam name="TItem">Type of the objects being enumerated.</typeparam>
  [DebuggerDisplay("Count={Count}")]
  public class FastEnumerable<TItem> : IFastEnumerator<TItem> {
    /// <summary>Construct a new instance from array <c>a</c>.</summary>
    /// <param name="a">The array of type <c>TItem</c> to make enumerable.</param>
    internal FastEnumerable(TItem[] a) { _a = a; } 

    private TItem[] _a;
    private int     _index = -1;

    /// <summary>Return the next item in the enumeration.</summary>
    public bool MoveNext(ref TItem item) {
      TItem[] a = _a;
      int i;
      if ((i = ++_index) >= a.Length)     return false;
      item = a[i];
      return true;
    }
  }
}
