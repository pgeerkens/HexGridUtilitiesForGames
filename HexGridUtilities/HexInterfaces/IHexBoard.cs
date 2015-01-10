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
using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.FieldOfView;

namespace PGNapoleonics.HexUtilities {
  using HexSize = System.Drawing.Size;

  /// <summary>External interface exposed by the the implementation of <see cref="IHexBoard{THex}"/>.</summary>
  public interface IHexBoard<out THex> : IBoardStorage<THex>, INavigableBoard<THex>, IFovBoard<THex> where THex : class, IHex {
    /// <summary>Gets the extent in pixels o fhte grid on which hexes are to be laid out. </summary>
    HexSize  GridSize    { get; }

    /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
    int      RangeCutoff { get; }

    /// <summary>Returns whether the specified hex coordinates as a valid hex on this board.</summary>
    new bool IsOnboard(HexCoords coords);

    /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
   [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    new THex this[HexCoords coords] {get; }
  }
}
