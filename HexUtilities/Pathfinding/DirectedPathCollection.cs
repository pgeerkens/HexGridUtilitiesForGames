#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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

using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Pathfinding {

  /// <summary>A directed sequence of <see cref="Hex{TDrawingSurface,TPath}"/>steps comprising a travel path.</summary>
  [DebuggerDisplay("TotalCost={TotalCost} / TotalSteps={TotalSteps}")]
  internal sealed class DirectedPathCollection : IDirectedPathCollection {
    #region Properties
    /// <inheritdoc/>
    public Hexside                  HexsideExit { get { return PathStep.HexsideExit; } }
    /// <inheritdoc/>
    public IDirectedPathCollection  PathSoFar   { get { return _pathSoFar; } }
    /// <inheritdoc/>
    public DirectedPathStepHex      PathStep    { get { return _pathStep; } }
    /// <inheritdoc/>
    public string                   StatusText  { 
      get { return string.Format(CultureInfo.InvariantCulture,
                  "Path Length: {0}/{1}", TotalCost,TotalSteps);
      }
    }
    /// <inheritdoc/>
    public HexCoords                StepCoords  { get { return PathStep.Coords; } }
    /// <inheritdoc/>
    public int                      TotalCost   { get { return _totalCost; } }
    /// <inheritdoc/>
    public int                      TotalSteps  { get { return _totalSteps; } }

    private readonly IDirectedPathCollection _pathSoFar;
    private readonly DirectedPathStepHex     _pathStep;
    private readonly int                     _totalCost;
    private readonly int                     _totalSteps;
    #endregion

    /// <inheritdoc/>
    public IDirectedPathCollection AddStep(HexCoords coords, Hexside hexsideExit, int stepCost) {
      return AddStep(new DirectedPathStepHex(coords,hexsideExit), stepCost);
    }
    /// <inheritdoc/>
    public IDirectedPathCollection AddStep(DirectedPathStepHex neighbour, int stepCost) {
      return new DirectedPathCollection(this, neighbour, TotalCost + stepCost);
    }

    /// <inheritdoc/>
    public override string ToString() {
      if (PathSoFar == null) 
        return string.Format(CultureInfo.InvariantCulture,"Hex: {0} arrives with TotalCost={1,3}",
          PathStep.Coords, TotalCost);
      else
        return string.Format(CultureInfo.InvariantCulture,"Hex: {0} exits {1} with TotalCost={2,3}",
          PathStep.Coords, PathStep.HexsideEntry, TotalCost);
    }

    /// <summary>Returns the ordered sequence of sub-paths comprising this DirectedPath.</summary>
    public IEnumerator<IDirectedPathCollection> GetEnumerator() {
      yield return this;
      for (var p = (IDirectedPathCollection)this; p.PathSoFar != null; p = p.PathSoFar) 
        yield return p.PathSoFar;
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    /////////////////////////////  Internals  //////////////////////////////////
    /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
    internal DirectedPathCollection(HexCoords start) : this(null, new DirectedPathStepHex(start, Hexside.North), 0) {
    }

    /// <summary>Returns a DirectedPath composed by extending this DirectedPath by one hex.</summary>
    internal DirectedPathCollection(IDirectedPathCollection pathSoFar, DirectedPathStepHex pathStep, int totalCost) {

      _pathStep    = pathStep;
      _pathSoFar   = pathSoFar;
      _totalCost   = totalCost;
      _totalSteps  = pathSoFar==null ? 0 : pathSoFar.TotalSteps+1;
    }
  }
}
