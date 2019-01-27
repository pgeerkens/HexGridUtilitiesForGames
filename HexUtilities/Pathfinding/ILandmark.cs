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

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities.Pathfinding {
  /// <summary>TODO</summary>
  public interface ILandmark : IDisposable {
    /// <summary>Board coordinates for the landmark location.</summary>
    HexCoords Coords { get; }

    /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
    Maybe<short> DistanceFrom(HexCoords coords);
    /// <summary>Returns the shortest-path directed-distance to the specified hex from the landmark.</summary>
    Maybe<short> DistanceTo  (HexCoords coords);
}

  /// <summary>An <see cref="IList"/> of defined <see cref="ILandmark"/> locations.</summary>
  public interface ILandmarkCollection : IFastList<ILandmark>, IDisposable {
    /// <summary>Calculated distance from the specified landmark to the specified hex</summary>
    /// <param name="coords"></param>
    /// <param name="landmarkToShow"></param>
    /// <returns></returns>
    string DistanceFrom(HexCoords coords, int landmarkToShow);
    /// <summary>Calculated distance to the specified landmark from the specified hex</summary>
    /// <param name="coords"></param>
    /// <param name="landmarkToShow"></param>
    /// <returns></returns>
    string DistanceTo(HexCoords coords, int landmarkToShow);
  }


  /// <summary>TODO</summary>
  public interface IDirectedLandmark : IDisposable {
    ///// <summary>Board coordinates for the landmark location.</summary>
    //HexCoords Coords    { get; }

    /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
    Maybe<short> Distance(HexCoords coords);
  }
}
