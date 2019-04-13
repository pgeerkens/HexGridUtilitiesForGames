#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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

using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using HexSize = System.Drawing.Size;

    //internal sealed partial class DirectedLandmark : IDirectedLandmark {
    //    public static DirectedLandmark New(HexCoords hexCoords,HexSize mapSizeHexes,
    //        Func<IPriorityQueue<int,HexCoords>> queueFactory,
    //        TryDirectedCost tryDirectedCosts
    //    ) =>  new DirectedLandmark(hexCoords,mapSizeHexes,queueFactory,tryDirectedCosts);

    //    /// <summary>Populates and returns a new landmark at the specified board coordinates.</summary>
    //    /// <param name="hexCoords"><see cref="HexCoords"/> for the landmark to be created.</param>
    //    /// <param name="mapSizeHexes">Hex dimensions of the IBoard{IHex} on which the landmark is to be created.</param>
    //    /// <param name="queueFactory">Factory that creates empty instances of <c>IPriorityQueue</c>.</param>
    //    /// <param name="tryDirectedCosts">TODO</param>
    //    private DirectedLandmark(HexCoords hexCoords,HexSize mapSizeHexes,
    //        Func<IPriorityQueue<int,HexCoords>> queueFactory,
    //        TryDirectedCost tryDirectedCosts
    //    ) => _backingStore = new LandmarkPopulatorFunctor(hexCoords,mapSizeHexes,queueFactory,tryDirectedCosts).Fill();

    //    ///// <summary>Board coordinates for the landmark location.</summary>
    //    //public  HexCoords Coords      { get {return _hex.Coords;} } 
    //    ///// <summary>Board coordinates for the landmark location.</summary>
    //    //public  IHex      Hex         { get {return _hex;} } readonly IHex _hex;
    //    /// <inheritdoc/>
    //    public int Distance(HexCoords coords) => _backingStore[coords];

    //    internal readonly BoardStorage<int>     _backingStore;
    //}
}
