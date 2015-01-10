using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1587
/// <summary>Joe Duffy's Simple (Fast) List enumerator.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.Common {

  #region Default implementations
  /// <inheritdoc/>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  public sealed class FastList<TItem> : AbstractFastList<TItem> {
    /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
    public FastList(TItem[] array) : base(array) { }
  }

  /// <inheritdoc/>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  public sealed class FastListDisposable<TItem> : AbstractFastList<TItem>, IDisposable 
    where TItem : IDisposable {
    /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
    public FastListDisposable(TItem[] array) : base(array)  { }

    #region IDisposable implementation with Finalizer
    private bool _isDisposed = false;  //!<True if already Disposed.
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Utility routine to do the hard-lifting for Dispose().</summary>
    private void Dispose(bool disposing) {
      if (! _isDisposed  &&  disposing) {
        ForEach ( item => item.Dispose() );
        _array = null;
      }
      _isDisposed = true;
    }
    #endregion
  }
  #endregion


  /// <summary>Adapted implementation of Joe Duffy's Simple (Fast) List enumerator.</summary>
  /// <remarks>
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>
  /// </remarks>
  /// <typeparam name="TItem">The Type of the Item to be stored and iterated over.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  [DebuggerDisplay("Count={Count}")]
  private abstract class AbstractFastList<TItem> : IFastList<TItem> {
    ///// <summary>Returns a new instance from <paramref name="array"/>.</summary>
    //[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    //[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    //public static IFastList<TItem> NewDisposableFastList(TItem[] array) {
    //  AbstractFastList<TItem>.FastListDisposable tempFastList = null;
    //  AbstractFastList<TItem>.FastListDisposable fastList     = null;
    //  try {
    //    tempFastList = new FastListDisposable(array);
    //    fastList     = tempFastList;
    //    tempFastList = null;
    //  } finally { if (tempFastList != null) tempFastList.Dispose(); }

    //  return fastList;
    //}

    /// <summary>TODO</summary>
    internal TItem[] _array;

    /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
    protected AbstractFastList(TItem[] array) { _array = array; }

    /// <inheritdoc/>>
    public IEnumerator<TItem>                     GetEnumerator(){
      return new ClassicEnumerable<TItem>(_array);
    }
    IEnumerator                       IEnumerable.GetEnumerator() {return this.GetEnumerator();}
    IFastEnumerator<TItem> IFastEnumerable<TItem>.GetEnumerator(){
      return new FastEnumerable<TItem>(_array);
    }

    /// <summary>IForEachable{TItem} implementation.</summary>
    public void  ForEach(Action<TItem> action) {
      if (action==null) throw new ArgumentNullException("action");

      TItem[] a = _array;
      for (int i = 0; i < a.Length; i++)    action(a[i]);
    }

    /// <summary>IForEachable2{TItem} implementation</summary>
    public void  ForEach(FastIteratorFunctor<TItem> functor) {
      if (functor==null) throw new ArgumentNullException("functor");

      TItem[] a = _array;
      for (int i = 0; i < a.Length; i++)    functor.Invoke(a[i]);
    }

    /// <inheritdoc/>
    public int   Count               { get {return _array.Length;} }
    /// <inheritdoc/>
    public TItem this[int index]     { get { return _array[index]; } }
    /// <inheritdoc/>
    public int   IndexOf(TItem item) { return Array.IndexOf(_array, item); }
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
  /// calls to enumerate a collection, neighbour isn’t a single interface method call.
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

  /// <summary>TODO</summary>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public interface IFastList<TItem> : IEnumerable<TItem>,
    IFastEnumerable<TItem>, IForEachable<TItem>, IForEachable2<TItem> {
    /// <summary>Gets the item at location <paramref name="index"/>.</summary>
    TItem this[int index] { get; }

    /// <summary>Returns the current size of the list.</summary>
    int Count { get; }

    /// <summary>Returns the index of the specified item in the internal array storage.</summary>
    /// <param name="item"></param>
    int IndexOf(TItem item);
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
  public sealed class ClassicEnumerable<TItem> : IEnumerator<TItem> {
    internal ClassicEnumerable(TItem[] a) { _a = a; }

    private TItem[] _a;
    private int     _index = -1;  ///< Index of the currently-enumerated element.

    /// <summary>Return the next item in the enumeration.</summary>
    public bool   MoveNext() { return ++_index < _a.Length; }

    /// <summary>Return the current item in the enumeration</summary>
    public TItem       Current  { get { return _a[_index]; } }
    object IEnumerator.Current  { get { return Current; } }

    /// <summary>Reset the enumerator to the start of the enumeration.</summary>
    public void Reset() { _index = -1; }

    #region IDisposable implementation with Finalizer
    private bool _isDisposed = false;  //!<True if already Disposed.
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Utility routine to do the hard-lifting for Dispose().</summary>
    private void Dispose(bool disposing) {
      if (! _isDisposed  &&  disposing  &&  typeof(TItem).GetInterfaces().Contains(typeof(IDisposable))) {
        foreach(var i in _a) ((IDisposable)i).Dispose();
      }
      _isDisposed = true;
    }
    #endregion
  }

  /// <summary>Implements IEnumerable{TItem} in the <i><b>fast</b></i> way:</summary>
  /// <typeparam name="TItem">Type of the objects being enumerated.</typeparam>
  [DebuggerDisplay("Count={Count}")]
  public sealed class FastEnumerable<TItem> : IFastEnumerator<TItem> {
    /// <summary>Construct a new instance from array <c>a</c>.</summary>
    /// <param name="a">The array of type <c>TItem</c> to make enumerable.</param>
    internal FastEnumerable(TItem[] a) { _array = a; } 

    private TItem[] _array;
    private int     _index = -1;

    /// <summary>Return the next item in the enumeration.</summary>
    public bool MoveNext(ref TItem item) {
      var a = _array;
      int i;
      if ((i = ++_index) >= a.Length)     return false;
      item = a[i];
      return true;
    }
  }

  /// <summary>TODO</summary>
  public static class ArrayExtensions {
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    public static FastList<T> ToFastList<T>(this T[] @this) {
      return new FastList<T>(@this);
    }
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    public static FastList<T> ToFastList<T>(this IList<T> @this) {
      return new FastList<T>(@this.ToArray());
    }
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    public static FastList<T> ToFastList<T>(this IEnumerable<T> @this) {
      return new FastList<T>(@this.ToArray());
    }
  }
}
