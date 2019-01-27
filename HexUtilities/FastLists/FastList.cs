using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FastLists {
  /// <summary>Default concrete implementation of <see cref="AbstractFastList{TItem}"/>.</summary>
  /// <typeparam name="TItem">The Type of the Item to be stored and iterated over.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  internal sealed class FastList<TItem> : AbstractFastList<TItem> {
    /// <summary>Constructs a new instance from <paramref name="array"/>.</summary>
    internal FastList(TItem[] array) : base(array) { array.RequiredNotNull("array"); }
  }

  /// <summary>Adapted implementation of Joe Duffy's Simple (Fast) List enumerator.</summary>
  /// <remarks>
  /// <a href="http://www.bluebytesoftware.com/blog/2008/09/21/TheCostOfEnumeratingInNET.aspx">
  /// The Cost of Enumeration in DotNet</a>
  /// </remarks>
  /// <typeparam name="TItem">The Type of the Item to be stored and iterated over.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification="The suffix has an unambiguous meaning in the application domain.")]
  [DebuggerDisplay("Count={Count}")]
  public abstract partial class AbstractFastList<TItem> : IFastList<TItem>, IFastListX<TItem> {
    /// <inheritdoc/>>
    public IEnumerator<TItem>                     GetEnumerator(){
      return Extensions.InitializeDisposable( () =>
        new ClassicEnumerable<TItem>(_array) );
    }
    IEnumerator                       IEnumerable.GetEnumerator() { return GetEnumerator(); }
    [Pure]
    IFastEnumerator<TItem> IFastEnumerable<TItem>.GetEnumerator(){
      return new FastEnumerable<TItem>(_array);
    }

    /// <summary>IForEachable{TItem} implementation.</summary>
    public   void  ForEach(Action<TItem> action) {
      action.RequiredNotNull("action"); // for Code Analysis

      TItem[] array = _array;
      for (int i = 0; i < array.Length; i++)    action(array[i]);
    }
    /// <inheritsdoc/>
    void IForEachable<TItem>.ForEach(Action<TItem> action) { ForEach(action); }

    /// <summary>IForEachable2{TItem} implementation</summary>
    public   void  ForEach(FastIteratorFunctor<TItem> functor) {
      functor.RequiredNotNull("functor"); // for Code Analysis

      TItem[] array = _array;
      for (int i = 0; i < array.Length; i++)    functor.Invoke(array[i]);
    }
    /// <inheritsdoc/>
    void IForEachable2<TItem>.ForEach(FastIteratorFunctor<TItem> functor) { ForEach(functor); }

    /// <inheritdoc/>
    public   int   Count               { get {return _array.Length;} }
    /// <inheritdoc/>
    public   TItem this[int index]     { get {
      Contract.Assume(_array[index] != null); // required even though provable
      return _array[index];
    } }
    /// <inheritdoc/>
    public   int   IndexOf(TItem item) {
      _array.AssumeInvariant();
      Contract.Assume(Contract.Result<int>() < Count);
      return Array.IndexOf(_array, item, 0, _array.Length);
    }

    /// <summary>Use carefully - must not interfere with iterators.</summary>
    void  IFastListX<TItem>.SetItem(int index, TItem value) { _array[index] = value; }

    /// <summary>TODO</summary>
    private readonly TItem[] _array;
  }
}
