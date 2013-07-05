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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.PathFinding {
  /// <summary>Structure returned by the A* Path Finding utility.</summary>
  public interface IPath
  { 
    Hexside   HexsideExit { get; }
    HexCoords StepCoords  { get; }
    IPath     PathSoFar   { get; }
    int       TotalCost   { get; }
    int       TotalSteps  { get; }
  }

  /// <summary>Eric Lippert's implementation for use in A*, modified for a hex-grid.</summary>
  /// <remarks>An implementation of 'path' using an immutable stack.</remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx</cref>
  /// <typeparam name="TNode">The type of a path-'node'.</typeparam>
  internal sealed class Path : IPath
  {
    #region IPath implementation
    public Hexside   HexsideExit { get; private set; }
    public HexCoords StepCoords  { get; private set; }
    public IPath     PathSoFar   { get; private set; }
    public int       TotalCost   { get; private set; }
    public int       TotalSteps  { get; private set; }
    #endregion

    public override string ToString() {
      return string.Format(CultureInfo.InvariantCulture,
        "Hex: {0} with TotalCost={1,3} (as {2}/{3})",
        StepCoords, TotalCost, TotalCost>>16, TotalCost &0xFFFF);
    }

    //public IEnumerator<HexCoords> GetEnumerator() {
    //  for (var p = (IPath)this; p.StepCoords != null; p = p.PathSoFar) 
    //    yield return p.StepCoords;
    //}

    /////////////////////////////  Internals  //////////////////////////////////
    internal Path(HexCoords start) : this(null, start, Hexside.North, 0) { }

    internal Path    AddStep(NeighbourCoords neighbour, int stepCost) {
      return new Path(this, neighbour.Coords, neighbour.Hexside, TotalCost + stepCost);
    }

    private  Path(Path previousSteps, HexCoords step, Hexside direction, int totalCost) {
      HexsideExit  = direction;
      StepCoords   = step;
      PathSoFar    = previousSteps;
      TotalCost    = totalCost;
      TotalSteps   = previousSteps==null ? 0 : previousSteps.TotalSteps+1;
    }
  }

}
