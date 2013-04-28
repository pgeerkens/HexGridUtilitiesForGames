using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities {
  /// <summary>Joe Duffy's Simple Fast List enumerator.</summary>
  /// <seealso cref="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx"/>
  public class FastList<T> : IEnumerable<T>, IFastEnumerable<T>, IForEachable<T>, IForEachable2<T>{
    private T[] m_array;

    public FastList(T[] array) { m_array = array; }

    IEnumerator<T> IEnumerable<T>.GetEnumerator(){
      return new ClassicEnumerable<T>(m_array);
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
      return new ClassicEnumerable<T>(m_array);
    }
    IFastEnumerator<T> IFastEnumerable<T>.GetEnumerator() {
      return new FastEnumerable<T>(m_array);
    }
    void IForEachable<T>.ForEach(Action<T> action) {
      T[] a = m_array;
      for (int i = 0, c = a.Length; i < c; i++)    action(a[i]);
    }
    void IForEachable2<T>.ForEach(Functor<T> functor) {
      T[] a = m_array;
      for (int i = 0, c = a.Length; i < c; i++)    functor.Invoke(a[i]);
    }

    public T this[int index] { get { return m_array[index]; } }
  }

  /// <remarks>
  /// The idea behind IFastEnumerable<T> (and specifically IFastEnumerator<T>) is to return
  /// the current element during the call to MoveNext itself.  This cuts the number of
  /// interface method calls necessary to enumerate a list in half.  The impact to performance
  /// isn’t huge, but it was enough to cut our overhead from about 3X to 2.3X.  Every little
  /// bit counts.
  /// </remarks>
  /// <typeparam name="T"></typeparam>
  public interface IFastEnumerable<T> {
    IFastEnumerator<T> GetEnumerator();
  }
  public interface IFastEnumerator<T>{
    bool MoveNext(ref T elem);
  }

  /// <summary>Delegated ForEach.</summary>
  /// <remarks>
  /// The IForEachable<T> interface is a push model in the sense that the caller provides a
  /// delegate and the ForEach method is responsible for invoking it once per element in the
  /// collection.  ForEach doesn’t return until this is done.  In addition to having far
  /// fewer method calls to enumerate a collection, there isn’t a single interface method call.
  /// Delegate dispatch is also much faster than interface method dispatch.  The result is
  /// nearly twice as fast as the classic IEnumerator<T> pattern (when /o+ isn’t defined).  
  /// Now we’re really getting somewhere!
  /// </remarks>
  /// <typeparam name="T"></typeparam>
  public interface IForEachable<T>{
    void ForEach(Action<T> action);
  }

  /// <summary>Replaces delegate calls with virtual method calls.</summary>
  /// <remarks>
  /// Somebody calling it will pass an instance of the Functor<T> class with the Invoke 
  /// method overridden.  The implementation of ForEach then looks quite a bit like 
  /// IForEachable<T>’s, just with virtual method calls in place of delegate calls:
  /// </remarks>
  public interface IForEachable2<T> {
    void ForEach(Functor<T> functor);
  }

  public abstract class Functor<T>{
    public abstract void Invoke(T t);
  }

  /// <summary>Implements IEnumerable<T> in the standard way:</summary>
  public class ClassicEnumerable<T> : IEnumerator<T>{
    private T[] m_a;
    private int m_index = -1;
    internal ClassicEnumerable(T[] a) { m_a = a; }

    public bool MoveNext() { return ++m_index < m_a.Length; }
    public T Current { get { return m_a[m_index]; } }
    object System.Collections.IEnumerator.Current { get { return Current; } }
    public void Reset() { m_index = -1; }
    public void Dispose() { }
  }

  public class FastEnumerable<T> : IFastEnumerator<T>{
    private T[] m_a;
    private int m_index = -1;
    internal FastEnumerable(T[] a) { m_a = a; } 

    public bool MoveNext(ref T elem) {
      T[] a = m_a;
      int i;
      if ((i = ++m_index) >= a.Length)     return false;
      elem = a[i];
      return true;
    }
  }
}
