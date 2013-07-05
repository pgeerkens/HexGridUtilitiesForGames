using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>Joe Duffy's Simple (Fast) List enumerator.</summary>
  /// <seealso cref="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx"/>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", 
    "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
    "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "type", 
  Target = "PGNapoleonics.HexUtilities.Common.FastList")]
  public sealed class FastList<TItem> : IEnumerable<TItem>, IFastEnumerable<TItem>, 
    IForEachable<TItem>, IForEachable2<TItem>{
    private TItem[] m_array;

    public FastList(TItem[] array) { m_array = array; }

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator(){
      return new ClassicEnumerable<TItem>(m_array);
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
      return new ClassicEnumerable<TItem>(m_array);
    }
    IFastEnumerator<TItem> IFastEnumerable<TItem>.GetEnumerator() {
      return new FastEnumerable<TItem>(m_array);
    }

    /// <summary>IForEachable&lt;TItem> implementation.</summary>
    public void ForEach(Action<TItem> action) {
#if CHECKED
      if (action==null) throw new ArgumentNullException("action");
#endif
      TItem[] a = m_array;
      for (int i = 0; i < a.Length; i++)    action(a[i]);
    }

    /// <summary>IForEachable2&lt;TItem> implementation</summary>
    public void ForEach(Functor<TItem> functor) {
#if CHECKED
      if (functor==null) throw new ArgumentNullException("functor");
#endif
      TItem[] a = m_array;
      for (int i = 0; i < a.Length; i++)    functor.Invoke(a[i]);
    }

    public TItem this[int index] { get { return m_array[index]; } }
  }

  /// <remarks>
  /// The idea behind IFastEnumerable&lt;TItem> (and specifically IFastEnumerator&lt;TItem>) is to return
  /// the current element during the call to MoveNext itself.  This cuts the number of
  /// interface method calls necessary to enumerate a list in half.  The impact to performance
  /// isn’t huge, but it was enough to cut our overhead from about 3X to 2.3X.  Every little
  /// bit counts.
  /// </remarks>
  /// <typeparam name="TItem"></typeparam>
  public interface IFastEnumerable<TItem> {
    IFastEnumerator<TItem> GetEnumerator();
  }
  public interface IFastEnumerator<T>{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
    bool MoveNext(ref T item);
  }

  /// <summary>Delegated ForEach.</summary>
  /// <remarks>
  /// The IForEachable&lt;T> interface is a push model in the sense that the caller provides a
  /// delegate and the ForEach method is responsible for invoking it once per element in the
  /// collection.  ForEach doesn’t return until this is done.  In addition to having far
  /// fewer method calls to enumerate a collection, there isn’t a single interface method call.
  /// Delegate dispatch is also much faster than interface method dispatch.  The result is
  /// nearly twice as fast as the classic IEnumerator<T> pattern (when /o+ isn’t defined).  
  /// Now we’re really getting somewhere!
  /// </remarks>
  /// <typeparam name="TItem"></typeparam>
  public interface IForEachable<TItem>{
    void ForEach(Action<TItem> action);
  }

  /// <summary>Replaces delegate calls with virtual method calls to a <c>Functor</c> instance.</summary>
  /// <remarks>
  /// Somebody calling it will pass an instance of the Functor<T> class with the Invoke 
  /// method overridden.  The implementation of ForEach then looks quite a bit like 
  /// IForEachable<T>’s, just with virtual method calls in place of delegate calls:
  /// </remarks>
  public interface IForEachable2<TItem> {
    void ForEach(Functor<TItem> functor);
  }

  public abstract class Functor<TItem>{
    public abstract void Invoke(TItem item);
  }

  /// <summary>Implements IEnumerable<TItem> in the standard way:</summary>
  public class ClassicEnumerable<TItem> : IEnumerator<TItem>, IDisposable {
    private TItem[] m_a;
    private int m_index = -1;
    internal ClassicEnumerable(TItem[] a) { m_a = a; }

    public bool MoveNext() { return ++m_index < m_a.Length; }
    public TItem Current { get { return m_a[m_index]; } }
    object System.Collections.IEnumerator.Current { get { return Current; } }
    public void Reset() { m_index = -1; }

    #region IDisposable implementation with Finalizer
    private bool isDisposed = false;
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing) {
      if (!isDisposed) {
        if (disposing) {
        }
      }
    }
    ~ClassicEnumerable() { Dispose(false); }
    #endregion
  }

  public class FastEnumerable<TItem> : IFastEnumerator<TItem> {
    private TItem[] m_a;
    private int m_index = -1;
    internal FastEnumerable(TItem[] a) { m_a = a; } 

    public bool MoveNext(ref TItem item) {
      TItem[] a = m_a;
      int i;
      if ((i = ++m_index) >= a.Length)     return false;
      item = a[i];
      return true;
    }
  }
}
