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
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridPanel {
    using HexSize    = System.Drawing.Size;
    using StepCosts  = IBoardStorage<Maybe<HexsideCosts>>;

    /// <summary>TODO</summary>
    public class LandmarkBoard : NavigableBoard, ILandmarkBoard {
        /// <summary>TODO</summary>
        public LandmarkBoard(HexSize mapSizeHexes, StepCosts entryCosts, StepCosts exitCosts, ILandmarkCollection landmarks)
        : this(mapSizeHexes, entryCosts, exitCosts, 1, landmarks) { }
        /// <summary>TODO</summary>
        public LandmarkBoard(HexSize mapSizeHexes, StepCosts entryCosts, StepCosts exitCosts, int minimumCost, ILandmarkCollection landmarks)
        : base(mapSizeHexes, entryCosts, exitCosts, minimumCost) {
            Landmarks = landmarks;
        }

        /// <summary>TODO</summary>
        public ILandmarkCollection Landmarks { get; }
    }
}
