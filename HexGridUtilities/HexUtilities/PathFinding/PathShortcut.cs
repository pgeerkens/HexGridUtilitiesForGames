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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

using PG_Napoleonics.HexUtilities.Common;
using PG_Napoleonics.HexUtilities.PathFinding;

namespace PG_Napoleonics.HexUtilities.PathFinding {
  public class PathShortcut {
    public IPathFwd PathFwd { get; private set; }
    public IHex     Goal    { get; private set; }

    public PathShortcut(IHex start, IHex gGoal, INavigableBoardFwd board) 
    : this(start, gGoal, board.RangeCutoff, board.StepCostFwd, board.Heuristic, board.IsOnBoard) { }
    public PathShortcut(
      IHex start, 
      IHex goal, 
      int  rangeCutoff,
      Func<ICoords, Hexside, int> stepCostFwd,
      Func<int,int>               heuristic,
      Func<ICoords,bool>          isOnBoard
    ) {
      PathFwd = PathFinder.FindPathFwd(start, goal, rangeCutoff,
        (c,h) => stepCostFwd(c.StepOut(h), h.Reversed()), 
        heuristic, isOnBoard, false);
      Goal = goal;
    }
  }
}
