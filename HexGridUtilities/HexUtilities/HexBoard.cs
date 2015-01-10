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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexUtilities {
  using HexPoint    = System.Drawing.Point;
  using HexSize     = System.Drawing.Size;
  using RectangleF  = System.Drawing.RectangleF;

  using ILandmarks  = ILandmarkCollection;
  using CoordsRect  = CoordsRectangle;

  /// <summary>TODO </summary>
  public delegate BoardStorage<THex> BoardStorageInitializer<THex,TPath>()
  where THex : class, IHex;

  /// <summary>Abstract implementation of a hexgrid map-board.</summary>
  /// <typeparam name="THex">TODO</typeparam>
  /// <typeparam name="TPath">TODO</typeparam>
  /// <remarks>No Finalizer is implemented as the class possesses no unmanaged resources.</remarks>
  public abstract class HexBoard<THex,TPath> : IHexBoard<THex>, IDisposable where THex : class, IHex {

    /// <summary>By default, landmark all four corners and midpoints of all 4 sides.</summary>
    /// <remarks>Pre-processing time on start-up can be reduced by decreasing the number of landmarks,
    /// though at the possible expense of longer path-finding times.</remarks>
    /// <param name="size"></param>
    protected static IFastList<HexCoords> DefaultLandmarks(HexSize size) {
      return new HexPoint[] { new HexPoint(           0,             0),
                              new HexPoint(size.Width/2,             0),
                              new HexPoint(size.Width-1,             0),
                              new HexPoint(           0, size.Height/2),
                              new HexPoint(size.Width-1, size.Height/2),
                              new HexPoint(           0, size.Height-1),
                              new HexPoint(size.Width/2, size.Height-1),
                              new HexPoint(size.Width-1, size.Height-1)
                            }.Select(p => HexCoords.NewUserCoords(p)).ToFastList();
    }

    /// <summary>Returns an array of six <see cref="HexPoint"/>s describing the corners of a hex on this <see cref="HexBoard{THex,TPath}"/>.</summary>
    /// <param name="gridSize">Dimensions of a hex on this <see cref="HexBoard{THex,TPath}"/> in pixels.</param>
    protected static HexPoint[] HexgridPathPoints(HexSize gridSize) {
      return new HexPoint[] { new HexPoint(gridSize.Width*1/3,              0  ), 
                              new HexPoint(gridSize.Width*3/3,              0  ),
                              new HexPoint(gridSize.Width*4/3,gridSize.Height/2),
                              new HexPoint(gridSize.Width*3/3,gridSize.Height  ),
                              new HexPoint(gridSize.Width*1/3,gridSize.Height  ),
                              new HexPoint(             0,    gridSize.Height/2)
                            };
    }

    #region Constructors
    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex,TPath}"/> with landmarks as specified for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a <see cref="System.Drawing.Size"/>.</param>
    /// <param name="landmarkCoords"><see cref="IFastList{HexCoords}"/> of the hexes to be used as Path-Finding landmarks.</param>
    /// <param name="hexgridPath">Implementation of a graphics path for the hex border.</param>
    /// <param name="boardStorage">TODO</param>
    protected HexBoard(HexSize sizeHexes, HexSize gridSize, IFastList<HexCoords> landmarkCoords, 
      Func<HexSize,TPath> hexgridPath, BoardStorageInitializer<THex,TPath> boardStorage
    ) : this(sizeHexes,gridSize,hexgridPath,boardStorage) {
      SetLandmarks(landmarkCoords);
    }
    private HexBoard(HexSize sizeHexes, HexSize gridSize, Func<HexSize,TPath> hexgridPath
      , BoardStorageInitializer<THex,TPath> boardStorage
    ) {
      if (hexgridPath  == null) throw new ArgumentNullException("hexgridPath");
      if (boardStorage == null) throw new ArgumentNullException("boardStorage");

      MapScale      = 1.00F;
      IsTransposed  = false;
      MapSizeHexes  = sizeHexes;
      GridSize      = gridSize;
      HexgridPath   = hexgridPath(gridSize);
      _boardHexes   = boardStorage();
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    private void SetLandmarks(IFastList<HexCoords> landmarkCoords) {
      ILandmarks tempLandmarks = null;
      ILandmarks landmarks     = null;
      try {
        tempLandmarks = LandmarkCollection.CreateLandmarks(this, landmarkCoords);
        landmarks     = tempLandmarks;
        tempLandmarks = null;
      } finally { if (tempLandmarks != null) tempLandmarks.Dispose(); }

      Landmarks = landmarks;
    }
    #endregion

    #region Properties & Fields
    /// <summary>Offset of hex centre from upper-left corner, as a <see cref="HexSize"/> struct.</summary>
    public             HexSize    CentreOfHexOffset { get { return new HexSize(GridSize.Width * 2/3, GridSize.Height /2); } }
    /// <summary>TODO </summary>
    protected virtual  int        ElevationBase     { get {return  0;} } //!< Height in units of elevation level 0 (zero).
    /// <summary>TODO </summary>
    protected virtual  int        ElevationStep     { get {return 10;} } //!< Height increase in units of each elevation level.
    /// <inheritdoc/>
    public virtual     int        FovRadius         { get; set; }
    /// <summary>TODO </summary>
    public    virtual  int        HeightOfMan       { get {return  1;} } //!< Height in metres.
    /// <inheritdoc/>
    public             IHexgrid   Hexgrid           { 
      get {return TransposableHexgrid.GetNewGrid(IsTransposed,GridSize,MapScale);}
    }
    ///  <inheritdoc/>
    public             HexSize    GridSize          { get; private set; }
    ///  <inheritdoc/>
    public             TPath      HexgridPath       { get; private set; }
     ///  <inheritdoc/>
    public             bool       IsTransposed      { get; set; }
    /// <inheritdoc/>
    public             ILandmarks Landmarks         { get; private set; }
    ///  <inheritdoc/>
    public             float      MapScale          { get; set; }
    ///  <inheritdoc/>
    public             HexSize    MapSizeHexes      { get; private set; }
    ///  <inheritdoc/>
    public             HexSize    MapSizePixels     { 
      get { return MapSizeHexes * new IntMatrix2D(GridSize.Width,                0, 
                                                               0,   GridSize.Height, 
                                                GridSize.Width/3, GridSize.Height/2); }
    }
    /// <inheritdoc/>
    public             int        RangeCutoff       { get; set; }
    /// <inheritdoc/>
    public             THex  this[HexCoords coords] { get { return _boardHexes[coords];} }

    private   IBoardStorage<THex> _boardHexes;
    #endregion

    #region Methods
    /// <inheritdoc/>
    public virtual   int  GetDirectedCostToExit(IHex hex, Hexside hexsideExit) {
      //return hex==null ? -1 
      //                 : this[hex.Coords].StepCost(hexsideExit);
      return hex==null ? -1 
                       : hex.StepCost(hexsideExit);
    }
    ///// <summary>TODO</summary>
    //public async Task<ILandmarks> GetLandmarksAsync(System.Threading.CancellationToken token) {
    //    if (_landmarks == null) _landmarks = await LandmarkCollection.CreateLandmarksAsync(this, _landmarkCoords, token);
    //    return _landmarks; 
    //}
    /// <inheritdoc/>
    public           int  ElevationGroundASL(HexCoords coords) {
      return ElevationBase + this[coords].ElevationLevel * ElevationStep;
    }
    /// <inheritdoc/>
    public           int  ElevationHexsideASL(HexCoords coords, Hexside hexside) {
      var hex = this[coords];
      return ElevationBase + hex.ElevationLevel * ElevationStep + hex.HeightHexside(hexside);
    }
    /// <inheritdoc/>
    public           int  ElevationTargetASL(HexCoords coords) {
      return ElevationGroundASL(coords) + HeightOfMan;
    }
    /// <inheritdoc/>
    public           int  ElevationObserverASL(HexCoords coords) {
      return ElevationGroundASL(coords) + HeightOfMan;
    }
    /// <inheritdoc/>
    public           int  ElevationTerrainASL(HexCoords coords) {
      return ElevationGroundASL(coords) + this[coords].HeightTerrain;
    }

    /// <summary>Perform <paramref name="action"/> for all neighbours of <paramref name="coords"/>.</summary>
    public           void ForAllNeighbours(HexCoords coords, Action<THex,Hexside> action) {
      _boardHexes.ForAllNeighbours(coords,action);
    }

    /// <inheritdoc/>
    public           void ForEach(Action<THex> action) { _boardHexes.ForEach(action); }

    /// <summary>TODO</summary>
    /// <param name="action"></param>
    public           void ForEachHex(Action<IHex> action) { _boardHexes.ForEach(action); }

    /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRectangle"/>, of the current clipping region.</summary>
    protected  CoordsRect GetClipInHexes(RectangleF visibleClipBounds, HexSize boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new CoordsRectangle (left, top, right-left, bottom-top);
    }
    
    /// <inheritdoc/>
    public virtual   int  Heuristic(int range) { return range; }

    /// <inheritdoc/>
    public           bool IsOnboard(HexCoords coords)  { return _boardHexes.IsOnboard(coords); }

    /// <inheritdoc/>
    public virtual   bool IsPassable(HexCoords coords) { return IsOnboard(coords); }

    /// <inheritdoc/>
    public           THex Neighbour(HexCoords coords, Hexside hexside) {
      return _boardHexes.Neighbour(coords,hexside);
    }

    /// <inheritdoc/>
    public virtual   int  StepCost(HexCoords coords, Hexside hexsideExit) {
      return IsOnboard(coords) ? this[coords].StepCost(hexsideExit) : -1;
    }
    #endregion

    #region IDisposable implementation (w/o Finalizer as the class possesses no unmanaged resources.)
    private bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing) {
      if (! _isDisposed) {
        if (disposing) {
          var path = HexgridPath as IDisposable;
          if (path != null) {  path.Dispose(); path = null;}
          if (Landmarks   != null) { Landmarks.Dispose();   Landmarks   = null; }
          if (_boardHexes != null) { _boardHexes.Dispose(); _boardHexes = null;}
        }
        _isDisposed = true;
      }
    }
    #endregion
  }
}

namespace PGNapoleonics.HexUtilities.Pathfinding {

  /// <summary>Extensions for the HexBoard class.</summary>
  public static partial class HexBoardExtensions {
    /// <summary>Returns a least-cost path from the hex <c>source</c> to the hex <c>target.</c></summary>
    public static IDirectedPathCollection GetDirectedPath(this IHexBoard<IHex> @this, IHex source, IHex target) {
      return GetDirectedPath(@this, source, target, false);
    }
    /// <summary>Returns a least-cost path from the hex <c>source</c> to the hex <c>target.</c></summary>
    public static IDirectedPathCollection GetDirectedPath(this IHexBoard<IHex> @this, IHex source, IHex target
      ,bool forceUnidirectional) {
      if (@this  == null) throw new ArgumentNullException("this");
      if (source == null) throw new ArgumentNullException("source");
      if (target == null) throw new OperationCanceledException("target");

      if (@this.IsPassable(source.Coords)  &&  @this.IsPassable(target.Coords)) {
        return (target.Coords.Range(source.Coords) <= @this.RangeCutoff  ||  forceUnidirectional)
              ? UnidirectionalPathfinder.FindDirectedPathFwd(@this, source, target)
              : (new BidirectionalPathfinder(@this, source, target)).PathRev;
      } else
        return default(IDirectedPathCollection);
    }

    /// <summary>Returns an <see cref="BidirectionalPathfinder"/>.</summary>
    /// <param name="this"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns>Returns an <see cref="IPathfinder{IHex}"/> with a shortest path from <paramref name="source"/> to <paramref name="target"/>
    /// as well as statistice on the Bidirectional path determination.</returns>
    public static IPathfinder<IHex> GetBidirectionalPathfinder(this INavigableBoard<IHex> @this, IHex source, IHex target) {
      return new BidirectionalPathfinder(@this, source, target);
    }
    /// <summary>Returns an <see cref="UnidirectionalPathfinder"/>.</summary>
    /// <param name="this"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns>Returns an <see cref="IPathfinder{IHex}"/> with a shortest path from <paramref name="source"/> to <paramref name="target"/>
    /// as well as statistice on the Unidirectional path determination.</returns>
    public static IPathfinder<IHex> GetUnidirectionalPathfinder(this INavigableBoard<IHex> @this, IHex source, IHex target) {
      return new UnidirectionalPathfinder(@this, source, target);
    }

#if NET45
    /// <summary>Asynchronously returns a least-cost path from the hex <c>source</c> to the hex <c>target.</c></summary>
    public static Task<IDirectedPathCollection> GetDirectedPathAsync(
      this IHexBoard<IHex> @this, IHex source,  IHex target
    ) {
      return @this.GetDirectedPathAsync(source, target, false);
    }
    /// <summary>Asynchronously returns a least-cost path from the hex <c>source</c> to the hex <c>target.</c></summary>
    public static Task<IDirectedPathCollection> GetDirectedPathAsync(
      this IHexBoard<IHex> @this, IHex source,  IHex target, bool forceUnidirectional
    ) {
      if (@this == null) throw new ArgumentNullException("this");
      return Task.Run<IDirectedPathCollection>(
          () => @this.GetDirectedPath(source, target, forceUnidirectional)
      );
    }
#endif
  }
}
