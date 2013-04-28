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

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface IPath<TNode> : IEnumerable<ICoordsCanon> where TNode:ICoordsCanon {
    Hexside      LastDirection { get; }
    ICoordsCanon LastStep      { get; }
    IPath<TNode> PreviousSteps { get; }
    int          Count         { get; }
    uint         TotalCost     { get; }
    uint         TotalSteps    { get; }
  }

  /// <summary>Eric Lippert's implementation for use in A*, modified for a hex-grid.</summary>
  /// <remarks>An implementation of 'path' using an immutable stack.</remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx</cref>
  /// <typeparam name="TNode">The type of a path-'node'.</typeparam>
  public sealed class Path<TNode> : IPath<TNode> where TNode : ICoordsCanon
  {
    public Hexside      LastDirection { get; private set; }
    public ICoordsCanon LastStep      { get; private set; }
    public IPath<TNode> PreviousSteps { get{ return _previousSteps;} } Path<TNode> _previousSteps;
    public int          Count         { get; private set; }
    public uint         TotalCost     { get; private set; }
    public uint         TotalSteps    { get; private set; }

    public Path<TNode> AddStep(TNode step, uint stepCost, Hexside direction) {
      return new Path<TNode>(this, step, direction, TotalCost + stepCost);
    }

    public Path(TNode start) : this(null, start, Hexside.None, 0) { }
    private Path(Path<TNode> previousSteps, TNode thisStep, Hexside direction, uint totalCost)
    : this(previousSteps, thisStep, direction, totalCost, 0) { }
    private Path(Path<TNode> previousSteps, TNode thisStep, Hexside direction, uint totalCost, int count) {
      _previousSteps = previousSteps;
      LastDirection  = direction;
      LastStep       = thisStep;
      Count          = count;
      TotalCost      = totalCost;
      TotalSteps     = previousSteps==null ? 0 : previousSteps.TotalSteps+1;
    }

    public IEnumerator<ICoordsCanon> GetEnumerator() {
      for (var p = (IPath<TNode>)this; p != null; p = p.PreviousSteps) {
        yield return p.LastStep;
        var step = p.LastStep;
        for (var i=0; i<p.Count; i++) {
          step = step.StepOut(p.LastDirection);
          yield return step;
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    public override string ToString() {
      return string.Format("Hex: {0} with TotalCost={1,3} (as {2}/{3})",
        LastStep.User, TotalCost, TotalCost>>16, TotalCost &0xFFFF);
    }
  }
}
