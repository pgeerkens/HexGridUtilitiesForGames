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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    //using HexSize     = System.Drawing.Size;
    using ILandmarks  = ILandmarkCollection;

    /// <summary>Interface required to make use of A* Path Finding utility with Landmark heuristic.</summary>
    //[ContractClass(typeof(ILandmarkBoardContract))]
    //public interface ILandmarkBoard : INavigableBoard {
    public interface ILandmarkBoard {
        /// <summary>TODO</summary>
        ILandmarks Landmarks { get; }
    }

    //[ContractClassFor(typeof(ILandmarkBoard))]
    //internal abstract class ILandmarkBoardContract : ILandmarkBoard {
    //    private ILandmarkBoardContract() { }

    //    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    //    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    //    private void ObjectInvariant() => Contract.Invariant(Landmarks != null);

    //    public abstract ILandmarks Landmarks { get; }
    //}
}
