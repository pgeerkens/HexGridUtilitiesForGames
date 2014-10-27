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

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Interface required to make use of A* Path Finding utility.</summary>
  public interface IDirectedNavigableBoard {
    /// <summary>The cost of entering the hex at location <c>coords</c> heading <c>hexside</c>.</summary>
    int   StepCost(HexCoords coords, Hexside hexsideExit);

    /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
    /// <remarks>Returning the supplied range multiplied by the cheapest movement 
    /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
    /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
    int   Heuristic(int range);

    /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
    bool  IsOnboard(HexCoords coords);

    /// <summary>Cost to extend path by exiting the hex at <c>coords</c> through <c>hexside</c>.</summary>
    int  DirectedStepCost(IHex hex, Hexside hexsideExit);

    /// <summary>Returns the collecction of defined landmarks on this board.</summary>
    LandmarkCollection Landmarks { get; }
  }
}
