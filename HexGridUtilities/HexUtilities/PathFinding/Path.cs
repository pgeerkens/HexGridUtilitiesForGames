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
using System.Linq;
using System.Text;

using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities.PathFinding {
  /// <summary>Structure returned by the A* Path Finding utility.</summary>
  public interface IPath : IEnumerable<ICoords>
  { 
    Hexside   LastDirection { get; }
    ICoords   LastStep      { get; }
    IPath     PreviousSteps { get; }
    uint      TotalCost     { get; }
    uint      TotalSteps    { get; }
  }

  /// <summary>Eric Lippert's implementation for use in A*, modified for a hex-grid.</summary>
  /// <remarks>An implementation of 'path' using an immutable stack.</remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx</cref>
  /// <typeparam name="TNode">The type of a path-'node'.</typeparam>
  internal sealed class Path : IPath
  {
    #region IPath implementation
    public Hexside  LastDirection { get; private set; }
    public ICoords  LastStep      { get; private set; }
    public IPath    PreviousSteps { get; private set; }
    public uint     TotalCost     { get; private set; }
    public uint     TotalSteps    { get; private set; }
    #endregion

    public override string ToString() {
      return string.Format("Hex: {0} with TotalCost={1,3} (as {2}/{3})",
        LastStep, TotalCost, TotalCost>>16, TotalCost &0xFFFF);
    }

    public IEnumerator<ICoords> GetEnumerator() {
      for (var p = (IPath)this; p.LastStep != null; p = p.PreviousSteps) 
        yield return p.LastStep;
    }

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    /////////////////////////////  Internals  //////////////////////////////////
    internal Path(ICoords start) : this(null, start, Hexside.None, 0) { }

    internal Path    AddStep(NeighbourCoords neighbour, uint stepCost) {
      return new Path(this, neighbour.Coords, neighbour.Direction, TotalCost + stepCost);
    }

    private  Path(Path previousSteps, ICoords thisStep, Hexside direction, uint totalCost) {
      LastDirection  = direction;
      LastStep       = thisStep;
      PreviousSteps  = previousSteps;
      TotalCost      = totalCost;
      TotalSteps     = previousSteps==null ? 0 : previousSteps.TotalSteps+1;
    }
  }

}
