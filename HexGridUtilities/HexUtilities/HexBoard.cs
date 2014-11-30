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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexUtilities {
  using HexPoint    = System.Drawing.Point;
  using HexSize     = System.Drawing.Size;
  using RectangleF  = System.Drawing.RectangleF;

  /// <summary>Abstract implementation of a hexgrid map-board.</summary>
  /// <remarks>No Finalizer is implemented as the class possesses no unmanaged resources.</remarks>
  public abstract class HexBoard<THex,TPath> : IHexBoard<THex>, IDisposable where THex : class, IHex {
    /// <summary>TODO</summary>
    /// <param name="size"></param>
    protected delegate IFastList<HexCoords> LandmarkFactory(HexSize size);

    /// <summary>By default, landmark all four corners and midpoints of all 4 sides.</summary>
    /// <remarks>Pre-processing time on start-up can be reduced by decreasing the number of landmarks,
    /// though at the possible expense of longer path-finding times.</remarks>
    protected static LandmarkFactory DefaultLandmarks { 
      get { return size => new HexPoint[] { new HexPoint(         0,            0),
                                            new HexPoint(size.Width/2,          0),
                                            new HexPoint(size.Width-1,          0),
                                            new HexPoint(         0,   size.Height/2),
                                            new HexPoint(size.Width-1, size.Height/2),
                                            new HexPoint(         0,   size.Height-1),
                                            new HexPoint(size.Width/2, size.Height-1),
                                            new HexPoint(size.Width-1, size.Height-1)
                                          }.Select(p => HexCoords.NewUserCoords(p)).ToFastList();
      }
    }

    #region Constructors
    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex,TPath}"/> with default 
    /// landmarks for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="landmarkCoords">IEnumerable{HexCoords} of the hexes to be used as Path-Finding landmarks.</param>
    /// <param name="hexgridPath">Implementation of a graphics path for the hex border.</param>
    protected HexBoard(HexSize sizeHexes, HexSize gridSize, IFastList<HexCoords> landmarkCoords, 
      Func<HexSize,TPath> hexgridPath
    ) {
      if (hexgridPath == null) throw new ArgumentNullException("hexgridPath");

      _mapSizeHexes   = sizeHexes;
      _gridSize       = gridSize;
      _landmarkCoords = landmarkCoords;
      HexgridPath     = hexgridPath(gridSize);
      ResetGrid();
    }
    #endregion

    IFastList<HexCoords> _landmarkCoords;

    #region Properties
    /// <inheritdoc/>
    protected abstract BoardStorage<THex>  BoardHexes        { get; }
    /// <summary>Offset of hex centre from upper-left corner, as a <see cref="System.Drawing.Size"/> struct.</summary>
    public             HexSize             CentreOfHexOffset { get; private set; }
    /// <inheritdoc/>
    public virtual     int                 FovRadius         { get; set; }
    ///  <inheritdoc/>
    public             HexSize             GridSize          { 
      get { return _gridSize; } 
      set { _gridSize=value; ResetGrid();} 
    } HexSize _gridSize = new HexSize(27,30);
    /// <inheritdoc/>
    public             Hexgrid             Hexgrid           { get; private set; }
    ///  <inheritdoc/>
    public             TPath               HexgridPath       { get; private set; }
     ///  <inheritdoc/>
    public             bool                IsTransposed      { 
      get { return _isTransposed; } 
      set { _isTransposed=value; ResetGrid();} 
    } bool _isTransposed = false;
    /// <inheritdoc/>
    public             ILandmarkCollection Landmarks         {
      get { 
        if (_landmarks == null) _landmarks =  LandmarkCollection.CreateLandmarks(this, _landmarkCoords);
        return _landmarks; 
      }
    } ILandmarkCollection _landmarks = null;
    ///  <inheritdoc/>
    public             float               MapScale          { 
      get { return _mapScale; } 
      set { _mapScale=value; ResetGrid();} 
    } float _mapScale = 1.00F;
    ///  <inheritdoc/>
    public             HexSize             MapSizeHexes      { 
      get { return _mapSizeHexes; } 
      set { _mapSizeHexes=value; ResetGrid();} 
    } HexSize _mapSizeHexes = new HexSize(1,1);
    ///  <inheritdoc/>
    public             HexSize             MapSizePixels     { get; private set; }
    /// <inheritdoc/>
    public             int                 RangeCutoff       { get; set; }
    /// <inheritdoc/>
    public             THex          this[HexCoords coords]  { get { return BoardHexes[coords];} }
    #endregion

    #region Methods
    /// <inheritdoc/>
    public virtual   int  GetDirectedCostToExit(IHex hex, Hexside hexsideExit) {
      return hex==null ? -1 : hex.GetDirectedCostToExit(hexsideExit);
    }
    /// <summary>TODO</summary>
    public async Task<ILandmarkCollection> GetLandmarksAsync(System.Threading.CancellationToken token) {
        if (_landmarks == null) _landmarks = await LandmarkCollection.CreateLandmarksAsync(this, _landmarkCoords, token);
        return _landmarks; 
    }
    /// <inheritdoc/>
    public abstract  int  ElevationASL(int elevationLevel);

    /// <summary>TODO</summary>
    /// <param name="action"></param>
    public           void ForEachHex(Action<IHex> action) { BoardHexes.ForEach(action); }

    /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRectangle"/>, of the current clipping region.</summary>
    protected CoordsRectangle  GetClipInHexes(RectangleF visibleClipBounds, HexSize boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new CoordsRectangle (left, top, right-left, bottom-top);
    }
    
    /// <inheritdoc/>
    public virtual   int  Heuristic(int range) { return range; }

    /// <inheritdoc/>
    public           bool IsOnboard(HexCoords coords)  { return BoardHexes.IsOnboard(coords); }

    /// <inheritdoc/>
    public virtual   bool IsPassable(HexCoords coords) { return IsOnboard(coords); }

    /// <summary>Sets the board layout parameters</summary>
    protected        void ResetGrid() {
      Hexgrid           = IsTransposed ? new TransposedHexgrid(GridSize.Scale(MapScale)) 
                                       : new Hexgrid(GridSize.Scale(MapScale));  
      CentreOfHexOffset = new HexSize(GridSize.Width * 2/3, GridSize.Height /2);
      MapSizePixels     = MapSizeHexes 
                        * new IntMatrix2D(GridSize.Width,                0, 
                                                       0,   GridSize.Height, 
                                          GridSize.Width/3, GridSize.Height/2);;
    }

    /// <inheritdoc/>
    public virtual   int  StepCost(HexCoords coords, Hexside hexsideExit) {
      return IsOnboard(coords) ? this[coords].StepCost(hexsideExit) : -1;
    }
    #endregion

    #region IDisposable implementation
    private bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (_landmarks != null) { _landmarks.Dispose(); _landmarks = null; }
        }
        _isDisposed = true;
      }
    }
    #endregion
  }

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

    /// <summary>TODO</summary>
    /// <param name="this"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static IPathfinder GetBidirectionalPathfinder(this INavigableBoard @this, IHex source, IHex target) {
      return new BidirectionalPathfinder(@this, source, target);
    }
    /// <summary>TODO</summary>
    /// <param name="this"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static IPathfinder GetUnidirectionalPathfinder(this INavigableBoard @this, IHex source, IHex target) {
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
