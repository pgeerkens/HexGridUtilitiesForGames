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

using PG_Napoleonics;
using PG_Napoleonics.HexUtilities;
using PG_Napoleonics.HexUtilities.Common;
using PG_Napoleonics.HexUtilities.PathFinding;
using PG_Napoleonics.HexUtilities.ShadowCastingFov;

namespace PG_Napoleonics.HexUtilities {
    public interface IBoard<TGridHex> 
      : INavigableBoardFwd, IFovBoard  where TGridHex : class, IHex {
      new bool   IsOnBoard(HexCoords coords);
      IPathFwd GetPathFwd(IHex start, IHex goal);
    }

  public abstract class HexBoard : IBoard<IHex> {
    public HexBoard(Size sizeHexes) { SizeHexes = sizeHexes; }

    ///  <inheritdoc/>
    public virtual  int  FovRadius      { get; set; }

    /// <inheritdoc/>
    public int           RangeCutoff    { get; set; }

    ///  <inheritdoc/>
    public virtual  Size SizeHexes      { get; private set; }

    ///  <inheritdoc/>
    public virtual  int  Heuristic(int range) { return range; }

    ///  <inheritdoc/>
    public virtual  bool IsOnBoard(HexCoords coords)  {
      return 0<=coords.User.X && coords.User.X < SizeHexes.Width
          && 0<=coords.User.Y && coords.User.Y < SizeHexes.Height;
    }

    ///  <inheritdoc/>
    public virtual  bool IsPassable(HexCoords coords) { return IsOnBoard(coords); }

    ///  <inheritdoc/>
    public virtual  int  StepCost(HexCoords coords, Hexside hexSide) {
      return IsOnBoard(coords) ? GetGridHex(coords).StepCostFwd(hexSide) : -1;
    }

    ///  <inheritdoc/>
    public virtual  int  StepCostFwd(HexCoords coords, Hexside hexSideExit) {
      return IsOnBoard(coords) 
        ? GetGridHex(coords.StepOut(hexSideExit)).StepCost(hexSideExit) 
        : -1;
    }

    ///  <inheritdoc/>
    IHex IFovBoard.this[HexCoords coords]  { get { return IsOnBoard(coords) ? GetGridHex(coords) : null; } }

    /// <summary>Returns the hex at coordinates specified by <c>coords</c>.</summary>
    protected abstract IHex GetGridHex(HexCoords coords);

    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public virtual IPathFwd GetPathFwd(IHex start, IHex goal) {
      return PathFinder.FindPathFwd(start, goal, this);
    }
  }

  public static partial class HexExtensions {
    /// <summary>Returns the field-of-view on <c>board</c> from the hex specified by coordinates <c>coords</c>.</summary>
    public static IFov GetFov(this IFovBoard @this, HexCoords origin) {
      return FovFactory.GetFieldOfView(@this,origin);
    }

    /// <summary>Returns the field-of-view from this hex.</summary>
    public static IFov GetFov(this IHex @this) {
      return FovFactory.GetFieldOfView(@this.Board, @this.Coords);
    }

    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static IPath GetPath(this INavigableBoard board, HexCoords start, HexCoords goal) {
      return PathFinder.FindPath(start, goal, board);
    }

    /// <summary>Returns a least-cost path from the hex <c>start</c> to the hex <c>goal.</c></summary>
    public static IPathFwd GetPathFwd(this INavigableBoardFwd board, IHex start, IHex goal) {
      return PathFinder.FindPathFwd(start, goal, board);
    }

    /// <summary>Returns a least-cost path from this hex to the hex <c>goal.</c></summary>
    public static IPathFwd GetPathFwd(this IHex @this, IHex goal) {
      return PathFinder.FindPathFwd(@this, goal, @this.Board);
    }

    /// <summary>Returns whether this hex is "On Board".</summary>
    public static bool IsOnBoard(this IHex @this) {
      return @this.Board.IsOnBoard(@this.Coords);
    }
  }
}
