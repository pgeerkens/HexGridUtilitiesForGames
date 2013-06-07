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
  public interface IPathFwd : IEnumerable<IPathFwd> { 
    Hexside  HexsideExit { get; }
    NeighbourHex  Step        { get; }
    IPathFwd      NextSteps   { get; }
    uint          TotalCost   { get; }
    uint          TotalSteps  { get; }

    IPathFwd AddStep(NeighbourHex neighbour, uint stepCost);
    IPathFwd AddStep(IPathFwd pathFront);
  }
  public class PathFwd : IPathFwd {
    #region IPath implementation
    public virtual Hexside  HexsideExit { get; private set; }
    public virtual NeighbourHex  Step        { get; private set; }
    public virtual IPathFwd      NextSteps   { get; private set; }
    public virtual uint          TotalCost   { get; private set; }
    public virtual uint          TotalSteps  { get; private set; }

    public virtual IPathFwd AddStep(NeighbourHex neighbour, uint stepCost) {
      return new PathFwd(this, neighbour, TotalCost + stepCost);
    }
    public virtual IPathFwd AddStep(IPathFwd pathFront) {
      return new PathWaypoint(pathFront, this);
    }
    #endregion

    public override string ToString() {
      if (NextSteps == null) 
        return string.Format("Hex: {0} arrives with TotalCost={1,3}",
          Step.Hex.Coords, TotalCost);
      else
        return string.Format("Hex: {0} exits {1} with TotalCost={2,3}",
          Step.Hex.Coords, Step.HexsideEntry, TotalCost);
    }

    public IEnumerator<IPathFwd> GetEnumerator() {
      yield return this;
      for (var p = (IPathFwd)this; p.NextSteps != null; p = p.NextSteps) 
        yield return p.NextSteps;
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    /////////////////////////////  Internals  //////////////////////////////////
    internal PathFwd(IHex start) : this(null, new NeighbourHex(start), 0) {}

    internal PathFwd(IPathFwd nextSteps, NeighbourHex neighbour, uint totalCost) {
      HexsideExit = neighbour.HexsideExit;
      Step        = neighbour;
      NextSteps   = nextSteps;
      TotalCost   = totalCost;
      TotalSteps  = nextSteps==null ? 0 : nextSteps.TotalSteps+1;
    }
  }

  internal class PathWaypoint : IPathFwd {
    #region IPath implementation
    public Hexside  HexsideExit { get {return PathFront.HexsideExit;} }
    public NeighbourHex  Step        { get {return PathFront.Step;} }
    public IPathFwd      NextSteps   { 
      get { return (PathFront.NextSteps == null) 
            ? PathBack
            : new PathWaypoint(PathFront.NextSteps, PathBack);
      } 
    }
    public uint          TotalCost   { get {return PathFront.TotalCost  + PathBack.TotalCost;} }
    public uint          TotalSteps  { get {return PathFront.TotalSteps + PathBack.TotalSteps;} }

           IPathFwd      PathFront   { get; set; }
           IPathFwd      PathBack    { get; set; }

    public IPathFwd AddStep(NeighbourHex neighbour, uint stepCost) {
      return new PathFwd(this, neighbour, TotalCost + stepCost);
    }
    public IPathFwd AddStep(IPathFwd pathFront) {
      return new PathWaypoint(pathFront, this);
    }
    #endregion

    public override string ToString() {
      return string.Format("Hex: {0} with TotalCost={1,3} (as {2}/{3})",
        Step, TotalCost, TotalCost>>16, TotalCost &0xFFFF);
    }

    public IEnumerator<IPathFwd> GetEnumerator() {
      yield return this;
      for (var p = (IPathFwd)this; p.NextSteps != null; p = p.NextSteps) 
        yield return p.NextSteps;
    }

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    /////////////////////////////  Internals  //////////////////////////////////
    internal PathWaypoint(IPathFwd pathFront, IPathFwd pathBack) {
      PathFront = pathFront;
      PathBack  = pathBack;
    }
  }
}
