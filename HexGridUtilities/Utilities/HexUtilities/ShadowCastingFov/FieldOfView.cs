#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;
using PG_Napoleonics.Utilities.HexUtilities.ShadowCastingFov;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public enum FovTargetMode {
    EqualHeights,
    TargetHeightEqualZero,
    TargetHeightEqualActual
  }
  public interface IFieldOfView {
    bool this[ICoordsUser coords] { get; }
  }
  public class FieldOfView : IFieldOfView {
    public static IFieldOfView GetFieldOfView(IBoard<IGridHex> board, ICoordsUser origin, int range) {
      if ( ! board.IsOnBoard(origin)) return null;
      return GetFieldOfView(board, origin, range, FovTargetMode.EqualHeights);
    }
    public static IFieldOfView GetFieldOfView(IBoard<IGridHex> board, ICoordsUser origin, int range, FovTargetMode targetMode) {
      DebugTracing.LogTime(TraceFlag.FieldOfView,"FieldOfView - begin");
      var fov = new FieldOfView(board);
      Func<ICoordsCanon,int> target;
      int                    observer;
      switch (targetMode) {
        case FovTargetMode.EqualHeights: 
          observer = board[origin].ElevationASL + 1;
          target   = canon => board[canon.User].ElevationASL + 1;
          break;
        case FovTargetMode.TargetHeightEqualZero:
          observer = board[origin].HeightObserver;
          target   = canon => board[canon.User].ElevationASL;
          break;
        default:
        case FovTargetMode.TargetHeightEqualActual:
          observer = board[origin].HeightObserver;
          target   = canon => board[canon.User].HeightTarget;
          break;
      }
      ShadowCasting.ComputeFieldOfView(
        origin.Canon, 
        range, 
        observer,
        canon=>board.IsOnBoard(canon.User),
        target,
        canon=>board[canon.User].HeightTerrain,
        canon=>fov[canon.User] = true
      );
      DebugTracing.LogTime(TraceFlag.FieldOfView,"FieldOfView - end");
      return fov;
    }

    public FieldOfView(IBoard<IGridHex> board) {
      Board      = board;
      FovBacking = new bool[board.SizeHexes.Width, board.SizeHexes.Height];
    }

    public bool this[ICoordsUser coords] { 
      get { return Board.IsOnBoard(coords) && FovBacking[coords.X, coords.Y]; } 
      set { if (Board.IsOnBoard(coords)) { FovBacking[coords.X,coords.Y] = value; } }
    } bool[,] FovBacking;

    IBoard<IGridHex>       Board;
  }
}
