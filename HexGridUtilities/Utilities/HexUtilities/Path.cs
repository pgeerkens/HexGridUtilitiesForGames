#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PG_Napoleonics.Utilities.HexUtilities {
  public interface IPath<TNode> : IEnumerable<TNode> where TNode:ICoordsCanon {
    Hexside      LastDirection { get; }
    TNode        LastStep      { get; }
    IPath<TNode> PreviousSteps { get; }
    uint         TotalCost     { get; }
  }

  /// <summary>Eric Lippert's implementation for use in A*, modified for a hex-grid.</summary>
  /// <remarks>An implementation of 'path' using an immutable stack.</remarks>
  /// <cref>http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx</cref>
  /// <typeparam name="TNode">The type of a path-'node'.</typeparam>
  public sealed class Path<TNode> : IPath<TNode> where TNode : ICoordsCanon
  {
    public Hexside      LastDirection { get; private set; }
    public TNode        LastStep      { get; private set; }
    public IPath<TNode> PreviousSteps { get{ return _previousSteps;} } Path<TNode> _previousSteps;
    public uint         TotalCost     { get; private set; }

    public Path<TNode> AddStep(TNode step, uint stepCost, Hexside direction) {
      return new Path<TNode>(step, this, TotalCost + stepCost, direction);
    }

    public Path(TNode start) : this(start, null, 0, Hexside.None) {}
    private Path(TNode lastStep, Path<TNode> previousSteps, uint totalCost, Hexside direction) {
      LastDirection  = direction;
      LastStep       = lastStep;
      _previousSteps = previousSteps;
      TotalCost      = totalCost;
    }

    public IEnumerator<TNode> GetEnumerator() {
      for (IPath<TNode> p = this; p != null; p = p.PreviousSteps) yield return p.LastStep;
    }

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    public override string ToString() {
      return string.Format("Hex: {0} with TotalCost={1,3} (as {2}/{3})",
        LastStep.User, TotalCost, TotalCost>>16, TotalCost &0xFFFF);
    }
  }
}
