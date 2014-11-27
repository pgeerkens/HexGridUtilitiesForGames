#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities {
  using HexSize     = System.Drawing.Size;

  /// <summary>Abstract specification and partial implementation of the <c>BoardStorage</c> required by <c>HexBoard</c>.</summary>
  /// <typeparam name="T">The type of the information being stored. 
  /// If {T} implements IDisposable then the Dispose() method will dispose all elements.</typeparam>
  public abstract class BoardStorage<T> : IDisposable {
    /// <summary>Initializes a new instance with the specified hex extent.</summary>
    /// <param name="sizeHexes"></param>
    protected BoardStorage(HexSize sizeHexes) {
      MapSizeHexes   = sizeHexes;
    }

    /// <summary>Extent in hexes of the board, as a <see cref="System.Drawing.Size"/> struct.</summary>
    public       HexSize MapSizeHexes           { get; private set; }

    /// <summary>Returns the <c>THex</c> instance at the specified coordinates.</summary>
    [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    public abstract T this[HexCoords coords] { get; internal set; }

    /// <summary>Returns whether the hex with <see cref="HexCoords"/> <c>coords</c> is 
    /// within the extent of the board.</summary>
    public          bool IsOnboard(HexCoords coords) { 
      return 0<=coords.User.X && coords.User.X < MapSizeHexes.Width
          && 0<=coords.User.Y && coords.User.Y < MapSizeHexes.Height;
    }

    /// <summary>Perform the specified <c>action</c> serially on all hexes.</summary>
    public abstract void ForEach(Action<T> action);

    /// <summary>Perform the specified <c>action</c> serially on all hexes satisfying <paramref name="predicate"/>/>.</summary>
    public abstract void ForEach(Func<T,bool> predicate, Action<T> action);

    /// <summary>Perform the specified <c>action</c> in parallel on all hexes.</summary>
    public abstract ParallelLoopResult ParallelForEach(Action<T> action);

    /// <summary>Perform the specified <c>action</c> in parallel on all hexes satisfying <paramref name="predicate"/>.</summary>
    public abstract ParallelLoopResult ParallelForEach(Func<T,bool> predicate, Action<T> action);

    #region IDisposable implementation with Finalizeer
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Anchors the Dispose chain for sub-classes.</summary>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (typeof(T).GetInterfaces().Contains(typeof(IDisposable))) {
            ForEach(i => ((IDisposable)i).Dispose());
          }
        }
        _isDisposed = true;
      }
    }
    /// <summary>Finalize this instance.</summary>
    ~BoardStorage() { Dispose(false); }
    #endregion
  }
}
