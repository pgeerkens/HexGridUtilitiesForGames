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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.FieldOfView;

namespace PGNapoleonics.HexUtilities {
  using HexPoint    = System.Drawing.Point;
  using HexSize     = System.Drawing.Size;
  using RectangleF  = System.Drawing.RectangleF;

  /// <summary>External interface exposed by the the implementation of <see cref="HexBoard{THex}"/>.</summary>
  public interface IBoard<out THex> : IDirectedNavigableBoard, IFovBoard<THex> where THex : IHex {
    /// <summary>Gets the extent in pixels o fhte grid on which hexes are to be laid out. </summary>
    HexSize GridSize    { get; }

    /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
    int     RangeCutoff { get; }

    /// <summary>Returns whether the specified hex coordinates as a valid hex on this board.</summary>
    new bool IsOnboard(HexCoords coords);

    /// <summary>Returns the Elevation Above-Sea-Level for a hex with the specified ElevationLevel.</summary>
    int      ElevationASL(int elevationLevel);
  }

  public interface IHexBoard<THex> : IBoard<THex> where THex : class, IHex {
  }

  /// <summary>Abstract implementation of a hexgrid map-board.</summary>
  public abstract class HexBoard<THex> : IHexBoard<THex>, IDisposable where THex : class, IHex {
    #region Constructors
    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex}"/> with default 
    /// landmarks for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="initializeBoard">Delegate that creates the <see cref="BoardStorage{T}"/> backing
    /// store for this instance.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected HexBoard(HexSize sizeHexes, HexSize gridSize) {
      _mapSizeHexes      = sizeHexes;
      _gridSize          = gridSize;
    }
    #endregion

    #region Properties
    /// <inheritdoc/>
    public          BoardStorage<THex> BoardHexes        { get; protected set; }
    /// <summary>Offset of hex centre from upper-left corner, as a <see cref="Size"/> struct.</summary>
    public          HexSize            CentreOfHexOffset { get; private set; }
    /// <inheritdoc/>
    public virtual  int                FovRadius         { get; set; }
    ///  <inheritdoc/>
    public          HexSize            GridSize          { 
      get { return _gridSize; } 
      set { _gridSize=value; ResetGrid();} 
    } HexSize _gridSize = new HexSize(27,30);
    /// <inheritdoc/>
    public          Hexgrid            Hexgrid           { get; private set; }
     ///  <inheritdoc/>
    public          bool               IsTransposed      { 
      get { return _isTransposed; } 
      set { _isTransposed=value; ResetGrid();} 
    } bool _isTransposed = false;
    /// <inheritdoc/>
    public          LandmarkCollection Landmarks         { get; private set; }
    ///  <inheritdoc/>
    public          float              MapScale          { 
      get { return _mapScale; } 
      set { _mapScale=value; ResetGrid();} 
    } float _mapScale = 1.00F;
    ///  <inheritdoc/>
    public          HexSize            MapSizeHexes      { 
      get { return _mapSizeHexes; } 
      set { _mapSizeHexes=value; ResetGrid();} 
    } HexSize _mapSizeHexes = new HexSize(1,1);
    ///  <inheritdoc/>
    public          HexSize            MapSizePixels     { get; private set; }
    /// <inheritdoc/>
    public          int                RangeCutoff       { get; set; }
    /// <inheritdoc/>
    public          THex               this[HexCoords coords]  { get { return BoardHexes[coords];} }
    #endregion

    #region Methods
    /// <summary>TODO</summary>
    protected void SetLandmarks(IList<HexCoords> landmarkCoords) {
      if (Landmarks != null) Landmarks.Dispose();
      Landmarks = LandmarkCollection.CreateLandmarks(this, landmarkCoords);
    }

    /// <summary>By default, landmark all four corners and midpoints of all 4 sides.</summary>
    /// <remarks>Pre-processing time on start-up can be reduced by decreasing the number of landmarks,
    /// though at the possible expense of longer path-findign times.</remarks>
    protected static readonly Func<HexSize, HexCoordsCollection> DefaultLandmarks = size => new HexCoordsCollection(
      new HexPoint[] {
        new HexPoint(0,            0), new HexPoint(size.Width/2,            0), new HexPoint(size.Width-1,            0),
        new HexPoint(0,size.Height/2),                                           new HexPoint(size.Width-1,size.Height/2),
        new HexPoint(0,size.Height-1), new HexPoint(size.Width/2,size.Height-1), new HexPoint(size.Width-1,size.Height-1)
      }.Select(p => HexCoords.NewUserCoords(p)).ToList());

    /// <inheritdoc/>
    public virtual  int  DirectedStepCost(IHex hex, Hexside hexsideExit) {
      return hex==null ? -1 : hex.DirectedStepCost(hexsideExit);
    }
    /// <inheritdoc/>
    public abstract int  ElevationASL(int elevationLevel);

    /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRectangle"/>, of the current clipping region.</summary>
    protected CoordsRectangle  GetClipInHexes(RectangleF visibleClipBounds, HexSize boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new CoordsRectangle (left, top, right-left, bottom-top);
    }
    
    /// <inheritdoc/>
    public virtual  int  Heuristic(int range) { return range; }

    /// <inheritdoc/>
    public          bool IsOnboard(HexCoords coords)  { return BoardHexes.IsOnboard(coords); }

    /// <inheritdoc/>
    public virtual  bool IsPassable(HexCoords coords) { return IsOnboard(coords); }

    /// <summary>Sets the board layout parameters</summary>
    protected       void ResetGrid() {
      Hexgrid           = IsTransposed ? new TransposedHexgrid(GridSize.Scale(MapScale)) 
                                       : new Hexgrid(GridSize.Scale(MapScale));  
      CentreOfHexOffset = new HexSize(GridSize.Width * 2/3, GridSize.Height /2);
      MapSizePixels     = MapSizeHexes 
                        * new IntMatrix2D(GridSize.Width,                 0, 
                                                       0,    GridSize.Height, 
                                          GridSize.Width/3,  GridSize.Height/2);;
    }

    /// <inheritdoc/>
    public virtual  int  StepCost(HexCoords coords, Hexside hexsideExit) {
      return IsOnboard(coords) ? this[coords].StepCost(hexsideExit) : -1;
    }
    #endregion

    #region IDisposable implementation with Finalizer
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (BoardHexes!=null) BoardHexes.Dispose();
        }
        _isDisposed = true;
      }
    }
    /// <inheritdoc/>
    ~HexBoard() { Dispose(false); }
    #endregion
  }

  /// <summary>TODO</summary>
  public class HexCoordsCollection : ReadOnlyCollection<HexCoords> {
    /// <summary>TODO</summary>
    public HexCoordsCollection(IList<HexCoords> list) : base(list) {}
  }

    /// <summary>Extensions for the HexBoard class.</summary>
  public static partial class HexBoardExtensions {
    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static IDirectedPath GetDirectedPath(this IBoard<IHex> @this, IHex start, IHex goal) {
      if (@this == null) throw new ArgumentNullException("this");
      if (start == null) throw new ArgumentNullException("start");
      if (goal == null) throw new OperationCanceledException("goal");

      if (@this.IsPassable(start.Coords) && @this.IsPassable(goal.Coords)) {
        return goal.Coords.Range(start.Coords) > @this.RangeCutoff
              ? BidirectionalPathfinder .FindDirectedPathFwd(start, goal, @this)
              : UnidirectionalPathfinder.FindDirectedPathFwd(start, goal, @this);
      } else
        return default(IDirectedPath);
    }

#if NET45
    /// <summary>Asynchronously returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static Task<IDirectedPath> GetDirectedPathAsync(
      this IBoard<IHex> @this, 
      IHex start,  IHex goal
    ) {
      if (@this == null) throw new ArgumentNullException("this");
      return Task.Run<IDirectedPath>(
          () => @this.GetDirectedPath(start, goal)
      );
    }
#endif
  }
}
