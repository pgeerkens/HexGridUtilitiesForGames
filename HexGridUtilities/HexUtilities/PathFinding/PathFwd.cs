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
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public interface IDirectedPath : IPath, IEnumerable<IDirectedPath> { 
    NeighbourHex PathStep    { get; }
    new IDirectedPath PathSoFar   { get; }
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  internal sealed class DirectedPath : IDirectedPath, IEquatable<HexCoords> {
    #region IPath implementation
    IPath       IPath.PathSoFar { get { return PathSoFar; } }
    IDirectedPath IDirectedPath.PathSoFar { get { return PathSoFar; } }

    public Hexside      HexsideExit { get { return PathStep.HexsideExit; } }
    public NeighbourHex PathStep    { get; private set; }
    public HexCoords    StepCoords  { get { return PathStep.Hex.Coords; } }
    public DirectedPath      PathSoFar   { get; private set; }
    public int          TotalCost   { get; private set; }
    public int          TotalSteps  { get; private set; }
    #endregion

    #region IEquatable implementation
    bool IEquatable<HexCoords>.Equals(HexCoords coords) { 
      return StepCoords.Equals(coords); 
    }
    public override bool Equals(object obj) { return Equals(obj as DirectedPath); }
    public override int  GetHashCode() { return StepCoords.GetHashCode(); }
    #endregion

    public DirectedPath AddStep(IHex hex, Hexside hexside, int stepCost) {
      return AddStep(new NeighbourHex(hex,hexside), stepCost);
    }
    public DirectedPath AddStep(NeighbourHex neighbour, int stepCost) {
      return new DirectedPath(this, neighbour, TotalCost + stepCost);
    }

    public override string ToString() {
      if (PathSoFar == null) 
        return string.Format(CultureInfo.InvariantCulture,"Hex: {0} arrives with TotalCost={1,3}",
          PathStep.Hex.Coords, TotalCost);
      else
        return string.Format(CultureInfo.InvariantCulture,"Hex: {0} exits {1} with TotalCost={2,3}",
          PathStep.Hex.Coords, PathStep.HexsideEntry, TotalCost);
    }

    public IEnumerator<IDirectedPath> GetEnumerator() {
      yield return this;
      for (var p = (IDirectedPath)this; p.PathSoFar != null; p = p.PathSoFar) 
        yield return p.PathSoFar;
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    /////////////////////////////  Internals  //////////////////////////////////
    internal DirectedPath(IHex start) : this(null, new NeighbourHex(start), 0) {}

    internal DirectedPath(DirectedPath nextSteps, NeighbourHex neighbour, int totalCost) {
      PathStep        = neighbour;
      PathSoFar   = nextSteps;
      TotalCost   = totalCost;
      TotalSteps  = nextSteps==null ? 0 : nextSteps.TotalSteps+1;
    }
  }

  //internal class PathWaypoint : IPathFwd {
  //  #region IPath implementation
  //  IPath       IPath.PathSoFar { get { return PathSoFar; } }
  //  IPathFwd IPathFwd.PathSoFar { get { return PathSoFar; } }

  //  public Hexside      HexsideExit { get {return PathFront.HexsideExit;} }
  //  public NeighbourHex Step        { get {return PathFront.Step;} }
  //  public HexCoords    StepCoords  { get { return Step.Hex.Coords; } }
  //  public IPathFwd     PathSoFar   { 
  //    get { return (PathFront.PathSoFar == null) 
  //          ? PathBack
  //          : new PathWaypoint(PathFront.PathSoFar, PathBack);
  //    } 
  //  }
  //  public int          TotalCost   { get {return PathFront.TotalCost  + PathBack.TotalCost;} }
  //  public int          TotalSteps  { get {return PathFront.TotalSteps + PathBack.TotalSteps;} }

  //         IPathFwd      PathFront   { get; set; }
  //         IPathFwd      PathBack    { get; set; }

  //  public virtual IPathFwd AddStep(IHex hex, Hexside hexside, int stepCost) {
  //    return AddStep(new NeighbourHex(hex,hexside), stepCost);
  //  }
  //  public IPathFwd AddStep(NeighbourHex neighbour, int stepCost) {
  //    return new PathFwd(this, neighbour, TotalCost + stepCost);
  //  }
  //  public IPathFwd AddStep(IPathFwd pathFront) {
  //    return new PathWaypoint(pathFront, this);
  //  }
  //  #endregion

  //  public override string ToString() {
  //    return string.Format("Hex: {0} with TotalCost={1,3} (as {2}/{3})",
  //      Step, TotalCost, TotalCost>>16, TotalCost &0xFFFF);
  //  }

  //  public IEnumerator<IPathFwd> GetEnumerator() {
  //    yield return this;
  //    for (var p = (IPathFwd)this; p.PathSoFar != null; p = p.PathSoFar) 
  //      yield return p.PathSoFar;
  //  }

  //  IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

  //  /////////////////////////////  Internals  //////////////////////////////////
  //  internal PathWaypoint(IPathFwd pathFront, IPathFwd pathBack) {
  //    PathFront = pathFront;
  //    PathBack  = pathBack;
  //  }
  //}
}
