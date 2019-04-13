#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using Queue    = IPriorityQueue<int,IHex>;
    using StepCost = Func<IHex,Hexside,int>; 

    /// <summary>Static methods to populate a <see cref="ILandmarkCollection"/> for a <see cref="INavigableBoard{THex}"/>"/></summary>
    public static class LandmarkPopulator {
        /// <summary>.</summary>
        /// <param name="board"></param>
        /// <param name="landmarkCoords"></param>
        public static ILandmarkCollection CreateLandmarksDictQueue(this IBoard<IHex> board,
            IFastList<HexCoords> landmarkCoords)
        => board.CreateLandmarks(landmarkCoords, PriorityQueueFactory.NewDictionaryQueue<int,IHex>);

        /// <summary>.</summary>
        /// <param name="board"></param>
        /// <param name="landmarkCoords"></param>
        public static ILandmarkCollection CreateLandmarksHotQueue(this IBoard<IHex> board,
            IFastList<HexCoords> landmarkCoords)
        => board.CreateLandmarks(landmarkCoords, PriorityQueueFactory.NewHotPriorityQueue<IHex>);

        /// <summary>Creates a populated <see cref="Collection{T}"/> of <see cref="Landmark"/>
        /// instances.</summary>
        /// <param name="board">The board on which the collection of landmarks is to be instantiated.</param>
        /// <param name="queueGenerator"></param>
        /// <param name="landmarkCoords">Board coordinates of the desired landmarks</param>
        public static ILandmarkCollection CreateLandmarks(this IBoard<IHex> board,
            IFastList<HexCoords> landmarkCoords, Func<Queue> queueGenerator)
        => new LandmarkCollection(
            from coords in ( landmarkCoords ?? new List<HexCoords>().ToFastList() )
                           .AsParallel()
                           .WithDegreeOfParallelism(Math.Max(1, Environment.ProcessorCount - 2))
            select ( from landmark in board[coords]
                     where landmark != null
                     select new Landmark(coords,
                           board?.PopulateLandmark(EntryCost,queueGenerator,landmark),
                           board?.PopulateLandmark(ExitCost, queueGenerator,landmark) )
                   ).ElseDefault()
        );

        static int EntryCost(IHex hex, Hexside hexside) => hex.EntryCost(hexside);
        static int ExitCost (IHex hex, Hexside hexside) => hex.ExitCost(hexside);

        static IBoardStorage<int> PopulateLandmark(this IBoard<IHex> board,
                StepCost directedStepCost, Func<Queue> queueGenerator, IHex landmark
        ) {
            TraceNewLine($"Find distances from {landmark.Coords}");

            var costs = new BlockedBoardStorage32x32<int>(board.MapSizeHexes, c => -1);
            var queue = queueGenerator();
            queue.Enqueue (0, landmark);

            while (queue.TryDequeue(out var item)) {
                var here = item.Value;
                var key  = item.Key;
                if (costs[here.Coords] > 0) continue;

                Trace($"Dequeue Path at {here} w/ cost={key,4}.");

                costs.SetItem(here.Coords,key);

                void SetHexside(Hexside hexside)
                => board.ExpandNode(directedStepCost,costs,queue,here,key,hexside);
                Hexside.ForEach(SetHexside);
            }

            return costs;
        }

        static void ExpandNode(this IBoard<IHex> board,StepCost directedStepCost,
            BoardStorage<int> costs,Queue queue,IHex here,int key,Hexside hexside
        ) {
            var neighbourCoords = here.Coords.GetNeighbour(hexside);

            board[neighbourCoords].IfHasValueDo(neighbour => {
                var cost = directedStepCost(here, hexside);
                if (cost > 0  &&  costs[neighbourCoords] < 0) {

                    Trace($"   Enqueue {neighbourCoords}: {cost,4}");

                    queue.Enqueue(key + cost,neighbour);
                }
            });
            //if (neighbourHex != null) {
            //    var cost = directedStepCost(here, hexside);
            //    if (cost > 0  &&  costs[neighbourCoords] < 0) {

            //        Trace($"   Enqueue {neighbourCoords}: {cost,4}");

            //        queue.Enqueue(key + cost,neighbourHex);
            //    }
            //}
        }

        #region Tracing partial methods
        [Conditional("TRACE")]
        static void Trace(string format,params object[] paramArgs)
        => Tracing.FindPathDetail.Trace(format,paramArgs);

        [Conditional("TRACE")]
        static void TraceNewLine(string format,params object[] paramArgs)
        => Tracing.FindPathDetail.Trace(true,format,paramArgs);
        #endregion
    }
}
