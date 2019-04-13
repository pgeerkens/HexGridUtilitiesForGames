#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using HexSize       = System.Drawing.Size;
    using IDirectedPath = IDirectedPathCollection;
    using ILandmarks    = ILandmarkCollection;
 
    /// <summary>TODO</summary>
    public enum Direction {
        ///<summary>TODO</summary>
        ToHex   = 0,
        ///<summary>TODO</summary>
        FromHex = 1
    }

    /// <summary>TODO</summary>
    public interface IPathfinder {
        /// <summary>Returns an <c>IDirectedPath</c> for the optimal path from <paramref name="source"/> to <paramref name="target"/>.</summary>
        /// <param name="board">An object satisfying the interface <c>INavigableBoardFwd</c>.</param>
        /// <param name="source">Coordinates for the <c>first</c> step on the desired path.</param>
        /// <param name="target">Coordinates for the <c>last</c> step on the desired path.</param>
        /// <returns>A <c>IDirectedPathCollection</c>  for the shortest path found, or null if no path was found.</returns>
        Maybe<IDirectedPath> GetPath(ILandmarkBoard board, IHex source, IHex target);
    }

    /// <summary>TODO</summary>
    public interface ILandmark {
        /// <summary>Board coordinates for the landmark location.</summary>
        HexCoords Coords { get; }

        /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
        int  DistanceFrom(HexCoords coords);
        /// <summary>Returns the shortest-path directed-distance to the specified hex from the landmark.</summary>
        int  DistanceTo  (HexCoords coords);
    }

    /// <summary>TODO</summary>
    public interface IDirectedLandmark {
        /// <summary>Returns the shortest-path directed-distance from the specified hex to the landmark.</summary>
        int  Distance(HexCoords coords);
    }

    /// <summary>An <see cref="IList"/> of defined <see cref="ILandmark"/> locations.</summary>
    public interface ILandmarkCollection : IFastList<ILandmark> {
    }

    public static partial class LandmarkExtensions {
        ///// <summary>TODO</summary>
        //public static ILandmark DictionaryPriorityQueueLandmark(this INavigableBoard board, HexCoords coords) {
        //    var backingStore = BackingStore(board, coords, PriorityQueueFactory.NewDictionaryQueue<int,HexCoords>);
        //    return new Landmark(coords, backingStore);
        //}

        ///// <summary>TODO</summary>
        //public static ILandmark HotPriorityQueueLandmark(this INavigableBoard board, HexCoords coords) {
        //    var backingStore = BackingStore(board, coords, PriorityQueueFactory.NewHotPriorityQueue<HexCoords>);
        //    return new Landmark(coords, backingStore);
        //}

        ///// <param name="coords">The <see cref="HexCoords"/> for this landmark.</param>
        ///// <param name="board">IBoard{IHex} on which the landmark is to be created.</param>
        ///// <param name="queueFactory">Function that returns a new <see cref="IPriorityQueue{TPriority,TValue}"</param>
        //private static IList<DirectedLandmark> BackingStore(this INavigableBoard board, HexCoords coords,
        //    Func<IPriorityQueue<int,HexCoords>> queueFactory)
        //=> new List<DirectedLandmark> {
        //        DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryEntryCost),
        //        DirectedLandmark.New(coords, board.MapSizeHexes, queueFactory, board.TryExitCost)
        //}.AsReadOnly();

        /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
        /// <param name="this"></param>
        /// <param name="coords">Type HexCoords - Hex for which to return Landmark distanace.</param>
        /// <param name="landmarkToShow">Type int - Index of the Landmark from which to display distances.</param>
        public static string DistanceFrom(this ILandmarks @this,HexCoords coords, int landmarkToShow)
        => $"{@this?[landmarkToShow]?.DistanceFrom(coords),3}";

        /// <summary>String representation of the distance from a given landmark to a specified hex</summary>
        /// <param name="this"></param>
        /// <param name="coords">Type HexCoords - Hex for which to return Landmark distanace.</param>
        /// <param name="landmarkToShow">Type int - Index of the Landmark from which to display distances.</param>
        public static string DistanceTo(this ILandmarks @this,HexCoords coords, int landmarkToShow)
        => $"{@this?[landmarkToShow]?.DistanceTo(coords),3}";
    }

    /// <summary>Interface required to make use of A* Path Finding utility.</summary>
    public interface INavigableBoard {
        /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
        HexSize MapSizeHexes           { get; }

        /// <summary>Returns an A* heuristic value from the supplied hexagonal Manhattan distance <c>range</c>.</summary>
        /// <remarks>
        /// Returning the supplied range multiplied by the cheapest movement cost for a single hex
        /// is usually suffficient. Note that <c>heuristic</c> <b>must</b> be monotonic in order 
        /// for the algorithm to perform properly and reliably return an optimum path.
        /// </remarks>
        int?  Heuristic(IHex source, IHex target);

        ///// <summary>TODO</summary>
        //int  TryExitCost(HexCoords hex, Hexside hexside);
        ///// <summary>TODO</summary>
        //int  TryEntryCost(HexCoords hex, Hexside hexside);
    }

    /// <summary>Interface required to make use of A* Path Finding utility with Landmark heuristic.</summary>
    public interface ILandmarkBoard : INavigableBoard {
        /// <summary>TODO</summary>
        ILandmarks Landmarks { get; }

        /// <summary>TODO</summary>
        Maybe<IHex> Neighbour(IHex hex, Hexside hexside);

        /// <summary>TODO</summary>
        Task<Exception> ResetLandmarksAsync();

        /// <summary>TODO</summary>
        void ResetLandmarks();
    }
  
    /// <summary>Interface of common data structures exposed by <see cref="BidirectionalAltPathfinder"/> to <see cref="AltPathfinder"/>s.</summary>
    public interface IPathHalves<THex> where THex: class,IHex {
        /// <summary>Retrieves the cost of the shortest path found so far.</summary>
        int              BestSoFar { get; }

         /// <summary>.</summary>
       ILandmarkBoard<THex>  Board { get; }

        /// <summary>.</summary>
        ISet<HexCoords>  ClosedSet { get; }

        /// <summary>.</summary>
        THex             Source    { get; }

        /// <summary>.</summary>
        THex             Target    { get; }

        /// <summary>.</summary>
        Maybe<IDirectedPath> PathFwd   { get; }

        /// <summary>.</summary>
        void             SetBestSoFar(IDirectedPath pathRev, IDirectedPath pathFwd);
    }

    /// <summary>.</summary>
    /// <typeparam name="THex"></typeparam>
    public interface ILandmarkBoard<THex>: ILandmarkBoard, INavigableBoard<THex> where THex: IHex {
    }

    ///// <summary>TODO</summary>
    //public interface ILandmarkPopulator {
    //    /// <summary>TODO</summary>
    //    BoardStorage<int> Fill();
    //}
}
