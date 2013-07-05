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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PGNapoleonics;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.PathFinding;

namespace PGNapoleonics.HexUtilities {
  public interface IBoard<out THex> : IDirectedNavigableBoard, IFovBoard<THex> where THex : IHex {
    Size     GridSize  { get; }
    /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
    int RangeCutoff { get; }

    new bool IsOnboard(HexCoords coords);
    int      ElevationASL(int elevationLevel);
  }

  public abstract class HexBoard<THex> : IBoard<THex>, IDisposable where THex : class, IHex {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sizeHexes"></param>
    /// <param name="gridSize"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected HexBoard(Size sizeHexes, Size gridSize, Func<HexBoard<THex>,BoardStorage<THex>> initializeBoard) {
      if (initializeBoard==null) throw new ArgumentNullException("initializeBoard");
      SetGridSize(sizeHexes, gridSize);
      BoardHexes = initializeBoard(this); 
    }

    #region IBoard<THex> implementation
    ///  <inheritdoc/>
    public virtual  int  FovRadius      { get; set; }

    /// <inheritdoc/>
    public          int  RangeCutoff    { get; set; }

    ///  <inheritdoc/>
    public virtual  int  Heuristic(int range) { return range; }

    ///  <inheritdoc/>
    public abstract int  ElevationASL(int elevationLevel);

    ///  <inheritdoc/>
    public          bool IsOnboard(HexCoords coords)  { return BoardHexes.IsOnboard(coords); }

    ///  <inheritdoc/>
    public virtual  bool IsPassable(HexCoords coords) { return IsOnboard(coords); }

    ///  <inheritdoc/>
    public virtual  int  StepCost(HexCoords coords, Hexside hexsideExit) {
      return IsOnboard(coords) ? this[coords].StepCost(hexsideExit) : -1;
    }

    ///  <inheritdoc/>
    public virtual  int  DirectedStepCost(IHex hex, Hexside hexsideExit) {
      return hex==null ? -1 : hex.DirectedStepCost(hexsideExit);
    }

    ///  <inheritdoc/>
    public          THex this[HexCoords coords]  { get { return BoardHexes[coords];} }
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

    /// <summary>Sets the board layout parameters</summary>
    /// <param name="mapSizeHexes"><c>Size</c> struct of the  board horizontal
    /// and vertical extent in hexes.</param>
    /// <param name="gridSize"><c>Size</c> struct of the horizontal and vertical
    /// extent (in pixels) of the grid on which hexes are to be laid out on.</param>
    public void SetGridSize(Size mapSizeHexes, Size gridSize) {
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

    #region IDisposable implementation with Finalizeer
    bool _isDisposed = false;
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (BoardHexes!=null) BoardHexes.Dispose();
        }
        _isDisposed = true;
      }
    }
    ~HexBoard() { Dispose(false); }
    #endregion
  }

  public static partial class HexBoardExtensions {
    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static IPath GetUndirectedPath(this IBoard<IHex> @this, HexCoords start, HexCoords goal) {
      if (@this == null) throw new ArgumentNullException("this");
      return Pathfinder.FindPath(start, goal, @this);
    }

    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static IDirectedPath GetDirectedPath(this IBoard<IHex> @this, IHex start, IHex goal) {
      if (@this == null) throw new ArgumentNullException("this");
      if (start == null) throw new ArgumentNullException("start");
      if (goal == null) throw new ArgumentNullException("goal");
      if (@this.IsPassable(start.Coords) && @this.IsPassable(goal.Coords)) {
        return goal.Coords.Range(start.Coords) > @this.RangeCutoff
              ? BidirectionalPathfinder.FindDirectedPathFwd(start, goal, @this)
              : Pathfinder.FindDirectedPath(start, goal, @this);
      } else
        return null;
    }
  }
}
