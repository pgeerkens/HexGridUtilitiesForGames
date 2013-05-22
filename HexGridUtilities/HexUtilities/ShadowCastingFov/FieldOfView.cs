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
  public class FieldOfView : IFov {
    public static IFov GetFieldOfView(IFovBoard board, ICoords origin) {
      return GetFieldOfView(board, origin, board.FovRadius, FovTargetMode.EqualHeights);
    }
    public static IFov GetFieldOfView(IFovBoard board, ICoords origin, int range, 
      FovTargetMode targetMode) {
      DebugTracing.LogTime(TraceFlag.FieldOfView,"FieldOfView - begin");
      var fov = new FieldOfView(board);
      if (board.IsPassable(origin)) {
        Func<ICoords,int> target;
        int               observer;
        switch (targetMode) {
          case FovTargetMode.EqualHeights: 
            observer = board[origin].ElevationASL + 1;
            target   = coords => board[coords].ElevationASL + 1;
            break;
          case FovTargetMode.TargetHeightEqualZero:
            observer = board[origin].HeightObserver;
            target   = coords => board[coords].ElevationASL;
            break;
          default:
          case FovTargetMode.TargetHeightEqualActual:
            observer = board[origin].HeightObserver;
            target   = coords => board[coords].HeightTarget;
            break;
        }
        ShadowCasting.ComputeFieldOfView(
          origin, 
          board.FovRadius, 
          observer,
          coords => board.IsOnBoard(coords),
          target,
          coords => board[coords].HeightTerrain,
          coords => fov[coords] = true
        );
      }
      DebugTracing.LogTime(TraceFlag.FieldOfView,"FieldOfView - end");
      return fov;
    }

    public FieldOfView(IFovBoard board) {
      Board      = board;
      FovBacking = new bool[board.SizeHexes.Width, board.SizeHexes.Height];
    }

    public bool this[ICoords coords] { 
      get { return Board.IsOnBoard(coords) && FovBacking[coords.User.X, coords.User.Y]; } 
      set { if (Board.IsOnBoard(coords)) { FovBacking[coords.User.X,coords.User.Y] = value; } }
    } bool[,] FovBacking;

    IFovBoard           Board;
  }
}
