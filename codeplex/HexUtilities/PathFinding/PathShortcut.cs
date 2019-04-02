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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.PathFinding;

namespace PGNapoleonics.HexUtilities.PathFinding {
  public class PathShortcut {
    public IDirectedPath DirectedPath { get; private set; }
    public IHex          Goal         { get; private set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1062:Validate arguments of public methods", MessageId = "2")]
    public PathShortcut(IHex start, IHex goal, IDirectedNavigableBoard board) { 
      DirectedPath = BidirectionalPathfinder.FindDirectedPathFwd(start, goal, board);
      Goal         = goal;
    }

    //static void ExtendShortcut(IHex goal, IntVector2D vectorGoal,
    //  IPriorityQueue<int, DirectedPath> queue,
    //  DirectedPath path,
    //  Func<int,int> heuristic
    //) {
    //  foreach (var shortcut in path.PathStep.Hex.Shortcuts) {
    //    var pathFwd = shortcut.DirectedPath;
    //    var newPath = path;
    //    var hexside = pathFwd.PathStep.HexsideEntry;
    //    if (newPath.PathStep.Hex.Equals(pathFwd.PathStep.Hex)) pathFwd = pathFwd.PathSoFar;
    //    while (pathFwd.PathSoFar != null) {
    //      var step     = pathFwd.PathStep;
    //      var cost     = pathFwd.TotalCost - pathFwd.PathSoFar.TotalCost;
    //      newPath      = newPath.AddStep(new NeighbourHex(step.Hex, hexside.Reversed()), cost);
    //      var estimate = Pathfinder.Estimate(heuristic, vectorGoal, goal.Coords, 
    //                        newPath.PathStep.Hex.Coords, newPath.TotalCost);
    //      queue.Enqueue(estimate, newPath);
    //      TraceFlags.FindPathDetail.Trace("   Enqueue {0}: {1,4}:{2,3}",
    //          step.Hex.Coords, estimate >> 16, estimate & 0xFFFF);
    //      hexside      = step.HexsideEntry;
    //      pathFwd      = pathFwd.PathSoFar;
    //    }
    //  }
    //}
  }
}
