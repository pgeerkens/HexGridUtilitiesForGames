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
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>Structure returned by the A* Path Finding utility.</summary>
  [ContractClass(typeof(IDirectedPathCollectionContract))]
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
    DirectedPathStepHex     PathStep        { get; }
    /// <summary>The remaining steps of this path, as an <see cref="IDirectedPathCollection"/> instance.</summary>
    IDirectedPathCollection PathSoFar       { get; }

    /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
    /// <param name="neighbour"></param>
    /// <param name="stepCost"></param>
    /// <returns></returns>
    IDirectedPathCollection AddStep(DirectedPathStepHex neighbour, int stepCost);
    /// <summary>Returns a new instance composed by extending this DirectedPath by one hex.</summary>
    /// <param name="there"></param>
    /// <param name="hexsideEntry"></param>
    /// <param name="cost"></param>
    /// <returns></returns>
    IDirectedPathCollection AddStep(HexCoords there, Hexside hexsideEntry, int cost);
    /// <summary>Returns a descriptive text suitable for the Status Bar.</summary>
    string StatusText { get; }
  }

  /// <summary>Contract class for <c>IDirectedPathCollection</c>.</summary>
  [ContractClassFor(typeof(IDirectedPathCollection))]
  internal abstract class IDirectedPathCollectionContract : IDirectedPathCollection {
    private IDirectedPathCollectionContract() {}

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    }

    #region Properties
    public Hexside                  HexsideExit { get { return default(Hexside); } }
    public IDirectedPathCollection  PathSoFar   { get { return default(IDirectedPathCollection); } }
    public DirectedPathStepHex             PathStep    { get { return default(DirectedPathStepHex); } }
    public string                   StatusText  { get { return String.Empty; } }
    public HexCoords                StepCoords  { get { return default(HexCoords); } }
    public int                      TotalCost   { get { return default(int); } }
    public int                      TotalSteps  { get { return default(int); } }
    #endregion

    public IDirectedPathCollection AddStep(HexCoords coords, Hexside hexsideExit, int stepCost) {
      return default(DirectedPathCollection);
    }
    public IDirectedPathCollection AddStep(DirectedPathStepHex neighbour, int stepCost) {
      return default(DirectedPathCollection);
    }

    public abstract IEnumerator<IDirectedPathCollection> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
  }
}
