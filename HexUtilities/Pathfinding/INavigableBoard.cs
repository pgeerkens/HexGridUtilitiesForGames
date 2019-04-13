#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>Interface required to make use of A* Path Finding utility.</summary>
    public interface INavigableBoard<out THex> where THex : IHex {
        ///// <summary>The cost of entering the hex at location <c>coords</c> heading <c>hexside</c>.</summary>
        //int   StepCost(HexCoords coords, Hexside hexsideExit);

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>Returning the supplied range multiplied by the cheapest movement 
        /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
        int?  Heuristic(int range);

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>Returning the supplied range multiplied by the cheapest movement 
        /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
        int?  Heuristic(HexCoords source, HexCoords target);

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>Returning the supplied range multiplied by the cheapest movement 
        /// cost for a single hex is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic 
        /// in order for the algorithm to perform properly and reliably return an optimum path.</remarks>
        int? Heuristic(IHex source, IHex target);

        /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
        bool  IsOnboard(HexCoords coords);

        /// <summary>Perform <paramref name="action"/> for all neighbours of this hex.</summary>
        void ForAllNeighbours(HexCoords coords, Action<THex,Hexside> action);

        /// <summary>Cost to move by exiting <paramref name="hex"/> through <paramref name="hexsideExit"/>.</summary>
        int  ExitCost(IHex hex, Hexside hexsideExit);

        /// <summary>Cost to move by exiting <paramref name="hex"/> through <paramref name="hexsideExit"/>.</summary>
        int  EntryCost(IHex hex, Hexside hexsideExit);

        /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        THex                this[HexCoords coords] { get; }
    }
}
