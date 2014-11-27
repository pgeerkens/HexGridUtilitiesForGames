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
using System.Collections;
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Structure returned by the A* Path Finding utility.</summary>
  public interface IDirectedPathCollection : IEnumerable<IDirectedPathCollection>, IEnumerable { 
    /// <summary>The <see cref="Hexside"/> through which an agent must move in taking the first step of this path.</summary>
    Hexside                 HexsideExit     { get; }
    /// <summary>The coordinates of the first step on this path.</summary>
    HexCoords               StepCoords      { get; }
    /// <summary>The total movement cost for this path.</summary>
    int                     TotalCost       { get; }
    /// <summary>The total number of movement steps for this path.</summary>
    int                     TotalSteps      { get; }
    /// <summary>The first step on this path.</summary>
    NeighbourHex            PathStep        { get; }
    /// <summary>The remaining steps of this path, as an <see cref="IDirectedPathCollection"/> instance.</summary>
    IDirectedPathCollection PathSoFar       { get; }

    /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
    /// <param name="neighbour"></param>
    /// <param name="stepCost"></param>
    /// <returns></returns>
    IDirectedPathCollection AddStep(NeighbourHex neighbour, int stepCost);
    /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
    /// <param name="there"></param>
    /// <param name="hexsideEntry"></param>
    /// <param name="cost"></param>
    /// <returns></returns>
    IDirectedPathCollection AddStep(IHex there, Hexside hexsideEntry, int cost);
  }
}
