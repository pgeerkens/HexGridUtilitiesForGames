#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexSize     = System.Drawing.Size;

    /// <summary>Abstract specification and partial implementation of the <c>BoardStorage</c> required by <c>HexBoard</c>.</summary>
    /// <typeparam name="T">The type of the information being stored. 
    /// If {T} implements IDisposable then the Dispose() method will dispose all elements.
    /// </typeparam>
    public abstract class BoardStorage<T> : IBoardStorage<T>, IForEachable<T>, IForEachable2<T> {
        /// <summary>Initializes a new instance with the specified hex extent.</summary>
        /// <param name="sizeHexes"></param>
        protected BoardStorage(HexSize sizeHexes) {
              MapSizeHexes   = sizeHexes;
        }

        /// <inheritdoc/>
        public       HexSize MapSizeHexes           { get; }

        /// <summary>Returns the <c>THex</c> instance at the specified coordinates.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public          T    this[HexCoords coords] => this[coords.User];

        /// <summary>Returns the <c>THex</c> instance at the specified user coordinates.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public virtual  T    this[IntVector2D userCoords] =>
            MapSizeHexes.IsOnboard(userCoords) ? ItemInner (userCoords.X,userCoords.Y) : default(T);

        #pragma warning disable 3008
        /// <summary>TODO</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected abstract T ItemInner(int x, int y);

        /// <summary>TOTO</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        protected abstract void SetItemInner(int x, int y, T value);
        #pragma warning restore 3008

        /// <inheritdoc/>
        public          void ForAllNeighbours(HexCoords coords, Action<T,Hexside> action)
        =>  Hexside.ForEach( hexside =>
                action(this[coords.GetNeighbour(hexside)], hexside)
            );

        /// <inheritdoc/>
        void IBoardStorage<T>.ForAllNeighbours(HexCoords coords, Action<T, Hexside> action) =>
            ForAllNeighbours(coords, action);

        /// <inheritdoc/>
        public          T    Neighbour(HexCoords coords, Hexside hexside) =>
           this[coords.GetNeighbour(hexside)]; 


        /// <summary>Perform the specified <paramref name="action"/> in parallel on all hexes.</summary>
        public abstract void ForEach(Action<T> action);

        /// <summary>Perform the Invoke action of the specified <paramref name="functor"/> in parallel on all hexes.</summary>
        public abstract void ForEach(FastIteratorFunctor<T> functor);

        /// <summary>Perform the specified <paramref name="action"/> serially on all hexes.</summary>
        public abstract void ForEachSerial(Action<T> action);

        /// <summary>Perform the Invoke action of the specified <paramref name="functor"/> serially on all hexes.</summary>
        public abstract void ForEachSerial(FastIteratorFunctor<T> functor);

        /// <summary>Sets the location to the specified value.</summary>
        /// <remarks>Use carefully - can interfere with iterators.</remarks>
        internal virtual void SetItem(HexCoords coords, T value) {
            if(MapSizeHexes.IsOnboard(coords)) SetItemInner(coords.User.X, coords.User.Y, value);
        }

        //#region IDisposable implementation with Finalizeer
        ///// <summary>Clean up any resources being used, and suppress finalization.</summary>
        //public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        ///// <summary>True if already Disposed.</summary>
        //private bool _isDisposed = false;
        ///// <summary>Clean up any resources being used.</summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected virtual void Dispose(bool disposing) {
        //    if (!_isDisposed) {
        //        if (disposing) {
        //            if (typeof(T).GetInterfaces().Contains(typeof(IDisposable))) {
        //                ForEach(i => { var item = i as IDisposable; if(item != null) item.Dispose(); i = default(T);} );
        //            }
        //        }
        //        _isDisposed = true;
        //    }
        //}
        ///// <summary>Finalize this instance.</summary>
        //~BoardStorage() { Dispose(false); }
        //#endregion
    }
}
