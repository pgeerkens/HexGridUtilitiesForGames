#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.ShadowCasting;

namespace PGNapoleonics.HexUtilities {
  /// <summary>External interface exposed by the the implementation of <see cref="HexBoard{THex}"/>.</summary>
  public interface IBoard<out THex> : IDirectedNavigableBoard, IFovBoard<THex> where THex : IHex {
    /// <summary>TODO</summary>
    Size GridSize    { get; }

    /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
    int  RangeCutoff { get; }

    /// <summary>TODO</summary>
    new bool IsOnboard(HexCoords coords);

    /// <summary>TODO</summary>
    int      ElevationASL(int elevationLevel);
  }

  /// <summary>Abstract implementation of a hexgrid map-board.</summary>
  public abstract class HexBoard<THex> : IBoard<THex>, IDisposable where THex : class, IHex {
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
    protected HexBoard(Size sizeHexes, Size gridSize, 
                       Func<HexBoard<THex>,BoardStorage<THex>> initializeBoard) 
    : this(sizeHexes, gridSize, initializeBoard, DefaultLandmarks(sizeHexes)) {}

    /// <summary>Initializes the internal contents of <see cref="HexBoard{THex}"/> with the specified set of 
    /// landmarks for pathfinding.</summary>
    /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a 
    /// <see cref="System.Drawing.Size"/>.</param>
    /// <param name="initializeBoard">Delegate that creates the <see cref="BoardStorage{T}"/> backing
    /// store for this instance.</param>
    /// <param name="landmarkCoords">Collection of <see cref="HexCoords"/> specifying the landmark 
    /// locations to be used for pathfinding.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected HexBoard(Size sizeHexes, Size gridSize, 
                       Func<HexBoard<THex>,BoardStorage<THex>> initializeBoard, 
                       ReadOnlyCollection<HexCoords> landmarkCoords
    ) {
      if (initializeBoard==null) throw new ArgumentNullException("initializeBoard");

      SetGridSize(sizeHexes, gridSize);
      BoardHexes = initializeBoard(this); 

      Landmarks  = LandmarkCollection.CreateLandmarks(this, landmarkCoords);
    }

    /// <summary>By default, landmark all four corners and midpoints of all 4 sides.</summary>
    /// <remarks>Pre-processing time on start-up can be reduced by decreasing the number of landmarks,
    /// though at the possible expense of longer path-findign times.</remarks>
    static readonly Func<Size, ReadOnlyCollection<HexCoords>> DefaultLandmarks = size =>new Point[] {
        new Point(0,            0), new Point(size.Width/2,            0), new Point(size.Width-1,            0),
        new Point(0,size.Height/2),                                        new Point(size.Width/2,size.Height/2),
        new Point(0,size.Height-1), new Point(size.Width/2,size.Height-1), new Point(size.Width-1,size.Height-1)
      }.Select(p => HexCoords.NewUserCoords(p)).ToList().AsReadOnly();

    #region IBoard<THex> implementation
    /// <inheritdoc/>
    public virtual  int  FovRadius      { get; set; }

    /// <inheritdoc/>
    public          int  RangeCutoff    { get; set; }

    /// <inheritdoc/>
    public virtual  int  Heuristic(int range) { return range; }

    /// <inheritdoc/>
    public abstract int  ElevationASL(int elevationLevel);

    /// <inheritdoc/>
    public          bool IsOnboard(HexCoords coords)  { return BoardHexes.IsOnboard(coords); }

    /// <inheritdoc/>
    public virtual  bool IsPassable(HexCoords coords) { return IsOnboard(coords); }

    /// <inheritdoc/>
    public virtual  int  StepCost(HexCoords coords, Hexside hexsideExit) {
      return IsOnboard(coords) ? this[coords].StepCost(hexsideExit) : -1;
    }

    /// <inheritdoc/>
    public virtual  int  DirectedStepCost(IHex hex, Hexside hexsideExit) {
      return hex==null ? -1 : hex.DirectedStepCost(hexsideExit);
    }

    /// <inheritdoc/>
    public          THex this[HexCoords coords]  { get { return BoardHexes[coords];} }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
      "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public LandmarkCollection Landmarks { get; protected set; }
    #endregion

    #region Drawing support
    ///  <inheritdoc/>
    public BoardStorage<THex> BoardHexes    { get; protected set; }
    ///  <inheritdoc/>
    public Size               GridSize      { get; private set; }
    ///  <inheritdoc/>
    public GraphicsPath       HexgridPath   { get; private set; }
    ///  <inheritdoc/>
    public Size               MapSizeHexes  { get; private set; }
    ///  <inheritdoc/>
    public Size               MapSizePixels { get; private set; }

    /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRectangle"/>, of the current clipping region.</summary>
    protected CoordsRectangle  GetClipInHexes(RectangleF visibleClipBounds, Size boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new CoordsRectangle (left, top, right-left, bottom-top);
    }
    
    /// <summary>Sets the board layout parameters</summary>
    /// <param name="mapSizeHexes"><c>Size</c> struct of the  board horizontal
    /// and vertical extent in hexes.</param>
    /// <param name="gridSize"><c>Size</c> struct of the horizontal and vertical
    /// extent (in pixels) of the grid on which hexes are to be laid out on.</param>
    public void  SetGridSize(Size mapSizeHexes, Size gridSize) {
      MapSizeHexes  = mapSizeHexes;
      GridSize      = gridSize;
      HexgridPath   = SetGraphicsPath();
      MapSizePixels = MapSizeHexes 
                    * new IntMatrix2D(GridSize.Width,                 0, 
                                                   0,    GridSize.Height, 
                                      GridSize.Width/3,  GridSize.Height/2);;
    }
    GraphicsPath SetGraphicsPath() {
      GraphicsPath path     = null;
      GraphicsPath tempPath = null;
      try {
        tempPath  = new GraphicsPath();
        tempPath.AddLines(new Point[] {
          new Point(GridSize.Width*1/3,                0), 
          new Point(GridSize.Width*3/3,                0),
          new Point(GridSize.Width*4/3,GridSize.Height/2),
          new Point(GridSize.Width*3/3,GridSize.Height  ),
          new Point(GridSize.Width*1/3,GridSize.Height  ),
          new Point(                 0,GridSize.Height/2),
          new Point(GridSize.Width*1/3,                0)
        } );
        path     = tempPath;
        tempPath = null;
      } finally { if(tempPath!=null) tempPath.Dispose(); }
      return path;
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
  public static partial class HexBoardExtensions {
    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static IDirectedPath GetDirectedPath(this IBoard<IHex> @this, IHex start, IHex goal) {
      if (@this == null) throw new ArgumentNullException("this");
      if (start == null) throw new ArgumentNullException("start");
      if (goal == null) throw new ArgumentNullException("goal");

      if (@this.IsPassable(start.Coords) && @this.IsPassable(goal.Coords)) {
        return goal.Coords.Range(start.Coords) > @this.RangeCutoff
              ? BidirectionalPathfinder .FindDirectedPathFwd(start, goal, @this)
              : UnidirectionalPathfinder.FindDirectedPathFwd(start, goal, @this);
      } else
        return default(IDirectedPath);
    }

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

    /// <summary>Returns the field-of-view on <c>board</c> from the hex specified by coordinates <c>coords</c>.</summary>
    [Obsolete("Use GetFieldOfView(HexCoords) instead.")]
    public static IFov GetFov(this IFovBoard<IHex> @this, HexCoords origin) {
      return @this.GetFieldOfView(origin);
    }

    /// <summary>TODO</summary>
    public static Task<IFov> GetFieldOfViewAsync(this IFovBoard<IHex> @this, HexCoords origin) {
      return @this.GetFieldOfViewAsync(origin, FovTargetMode.EqualHeights);
    }

    /// <summary>TODO</summary>
    public static Task<IFov> GetFieldOfViewAsync(this IFovBoard<IHex> @this, HexCoords origin, FovTargetMode targetMode) {
      return Task.Run<IFov>(
        () => @this.GetFieldOfView(origin, targetMode)
      );
    }
    
    /// <summary>TODO</summary>
    public static IFov GetFieldOfView(this IFovBoard<IHex> @this, HexCoords origin) {
      if (@this==null) throw new ArgumentNullException("this");
      return @this.GetFieldOfView(origin, FovTargetMode.EqualHeights);
    }

    /// <summary>TODO</summary>
    public static IFov GetFieldOfView(this IFovBoard<IHex> @this, HexCoords origin, FovTargetMode targetMode) {
      TraceFlags.FieldOfView.Trace("GetFieldOfView");
      var fov = new ArrayFieldOfView(@this);
      if (@this.IsPassable(origin)) {
        Func<HexCoords,int> target;
        int               observer;
        switch (targetMode) {
          case FovTargetMode.EqualHeights: 
            observer = @this[origin].ElevationASL + 1;
            target   = coords => @this[coords].ElevationASL + 1;
            break;
          case FovTargetMode.TargetHeightEqualZero:
            observer = @this[origin].HeightObserver;
            target   = coords => @this[coords].ElevationASL;
            break;
          default:
          case FovTargetMode.TargetHeightEqualActual:
            observer = @this[origin].HeightObserver;
            target   = coords => @this[coords].HeightTarget;
            break;
        }
        ShadowCasting.ShadowCasting.ComputeFieldOfView(
          origin, 
          @this.FovRadius, 
          observer,
          coords => @this.IsOnboard(coords),
          target,
          coords => @this[coords].HeightTerrain,
          coords => fov[coords] = true
        );
      }
      return fov;
    }
  }
}
