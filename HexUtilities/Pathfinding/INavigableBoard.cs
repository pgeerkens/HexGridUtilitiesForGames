#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Interface required to make use of A* Path Finding utility.</summary>
    public interface INavigableBoard<THex>: IBoard<IHex> where THex : IHex {
        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>Returning the supplied range multiplied by the cheapest movement 
        /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
        int? Heuristic(int range);

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>Returning the supplied range multiplied by the cheapest movement 
        /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
        int? Heuristic(HexCoords source, HexCoords target);

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>Returning the supplied range multiplied by the cheapest movement 
        /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
        int? Heuristic(IHex source, IHex target);

        ///// <summary>Cost to move by exiting <paramref name="hex"/> through <paramref name="hexsideExit"/>.</summary>
        //int  ExitCost(IHex hex, Hexside hexsideExit);

        ///// <summary>Cost to move by exiting <paramref name="hex"/> through <paramref name="hexsideExit"/>.</summary>
        //int  EntryCost(IHex hex, Hexside hexsideExit);
    }
}
