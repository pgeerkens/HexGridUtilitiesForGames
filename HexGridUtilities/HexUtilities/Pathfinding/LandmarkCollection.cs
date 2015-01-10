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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>A <b>ReadOnlyCollection</b> of defined <see cref="Landmark"/> locations.</summary>
  public sealed class LandmarkCollection : IFastList<ILandmark>, ILandmarkCollection {
    /// <summary>Creates a populated <see cref="Collection{T}"/> of <see cref="Landmark"/>
    /// instances.</summary>
    /// <param name="board">The board on which the collection of landmarks is to be instantiated.</param>
    /// <param name="landmarkCoords">Board coordinates of the desired landmarks</param>
    public static ILandmarkCollection CreateLandmarks(
      IHexBoard<IHex> board, 
      IFastList<HexCoords> landmarkCoords
    ) {
      if (landmarkCoords==null) throw new ArgumentNullException("landmarkCoords");

      ILandmarkCollection tempLandmarks = null, landmarks = null;
      try {
        tempLandmarks = new LandmarkCollection( (
                              from coords in landmarkCoords.AsParallel().AsOrdered()
                              where board.IsOnboard(coords) 
                              select new Landmark(coords,board)
                        ).ToList<ILandmark>() );
        landmarks     = tempLandmarks;
        tempLandmarks = null;
      } finally { if (tempLandmarks != null) tempLandmarks.Dispose(); }
      return landmarks;
    }

    /// <summary>TODO</summary>
    /// <param name="board"></param>
    /// <param name="landmarkCoords"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Task<ILandmarkCollection> CreateLandmarksAsync(
      IHexBoard<IHex>      board, 
      IFastList<HexCoords> landmarkCoords,
      CancellationToken    token
    ) {
      return Task.Run( () => CreateLandmarks(board,landmarkCoords) );
    }

    /// <summary>TODO</summary>
    public ParallelLoopResult ResetLandmarks() {
      return Parallel.For(0, Count, i => { var l = this[i]; if (l!=null) l.FillLandmark(); } );
    }

    private LandmarkCollection(IList<ILandmark> list) { // : base(list.ToArray()) {
      fastList = list.ToFastList();
    }

    /// <inheritdoc/>
    public int IndexOf(ILandmark item) { return fastList.IndexOf(item); }

    #region IFastList implemenation
    /// <inheritdoc/>
    public IEnumerator<ILandmark>                         GetEnumerator(){
      return ((IEnumerable<ILandmark>)this).GetEnumerator();;
    }
    IEnumerator                               IEnumerable.GetEnumerator(){
      return ((IEnumerable)fastList).GetEnumerator();
    }
    IEnumerator<ILandmark>         IEnumerable<ILandmark>.GetEnumerator(){
      return ((IEnumerable<ILandmark>)fastList).GetEnumerator();
    }
    IFastEnumerator<ILandmark> IFastEnumerable<ILandmark>.GetEnumerator(){
      return ((IFastEnumerable<ILandmark>)fastList).GetEnumerator();
    }

    /// <inheritdoc/>
    public void ForEach(Action<ILandmark> action) {fastList.ForEach(action);}
    /// <inheritdoc/>
    public void ForEach(FastIteratorFunctor<ILandmark> functor) {fastList.ForEach(functor);}

    /// <inheritdoc/>
    public int       Count           { get {return fastList.Count;} }
    /// <inheritdoc/>
    public ILandmark this[int index] { get {return fastList[index]; } }

    IFastList<ILandmark> fastList;
    #endregion

    #region IDisposable implementation
    private bool isDisposed = false;  //!<True if already Disposed.
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Utility routine to do the hard-lifting for Dispose().</summary>
    private void Dispose(bool disposing) {
      if (!isDisposed) {
        if (disposing) {
          var disposable = fastList as IDisposable;
          if (disposable != null) disposable.Dispose();

          fastList = null;
        }
      }
    }
    #endregion
  }
}
