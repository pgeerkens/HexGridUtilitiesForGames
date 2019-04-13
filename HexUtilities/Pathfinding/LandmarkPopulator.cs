#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    using HexSize = System.Drawing.Size;

    /// <summary>TODO</summary>
    /// <param name="here"></param>
    /// <param name="hexside"></param>
    /// <returns></returns>
    public delegate int TryDirectedCost(HexCoords here, Hexside hexside);

    /// <summary>TODO</summary>
    public interface ILandmarkPopulator {
        /// <summary>TODO</summary>
        BoardStorage<int> Fill();
    }

    /// <summary>The default implementation of <see cref="ILandmarkPopulator"/>.</summary>
    internal sealed partial class LandmarkPopulator : LandmarkPopulatorFunctor {
        public LandmarkPopulator(
            HexCoords                            hex, 
            HexSize                              mapSizeHexes, 
            Func<IPriorityQueue<int, HexCoords>> queueFactory, 
            TryDirectedCost                      tryDirectedStepCost
        ) : base(hex, mapSizeHexes, queueFactory, tryDirectedStepCost) { }

        public override BoardStorage<int> Fill() {

            // Reduce field references by keeping all these on stack.
            var queue = _queue;
            var store = _store;
            var tryDirectedStepCost = _tryDirectedStepCost;

            while (queue.TryDequeue(out var item)) {
                var here = item.Value;
                var key  = item.Key;
                Tracing.FindPathDetail.Trace("Dequeue Path at {0} w/ cost={1,4}.",here,key);

                Hexside.ForEach(hexside => Action(queue,store,tryDirectedStepCost,here,key,hexside));
            }
            return store;
        }

        private void Action(
            IPriorityQueue<int,HexCoords> queue, 
            BoardStorage<int>    store,
            TryDirectedCost       tryDirectedStepCost,
            HexCoords here, int key, Hexside hexside
        ) {
            var neighbour = here.GetNeighbour(hexside);
            var stepCost = tryDirectedStepCost(here, hexside);
            if (stepCost > 0) {
                Enqueue(key+stepCost,neighbour,_store);
            }
        }
    }

    /// <summary>The default implementation of <see cref="ILandmarkPopulator"/>.</summary>
    internal partial class LandmarkPopulatorFunctor : FastIteratorFunctor<Hexside>, ILandmarkPopulator {
        public LandmarkPopulatorFunctor(
          HexCoords                            hexCoords, 
          HexSize                              mapSizeHexes, 
          Func<IPriorityQueue<int, HexCoords>> queueFactory, 
          TryDirectedCost                      tryDirectedStepCost
        ) {
            Tracing.FindPathDetail.Trace("Find distances from {0}", hexCoords);

            _queue               = queueFactory();
            _store               = BlockedBoardStorage.New32x32(mapSizeHexes, c=>-1);
            _tryDirectedStepCost = tryDirectedStepCost;

            Enqueue(0,hexCoords,_store);
        }

        protected readonly IPriorityQueue<int,HexCoords> _queue;
        protected readonly BoardStorage<int>             _store;
        protected readonly TryDirectedCost               _tryDirectedStepCost;
        private            HexCoords                     _here;
        private            int                           _key;

        public virtual BoardStorage<int> Fill() {

            var queue = _queue; //!< Reduce field references by keeping on stack.

            while (queue.TryDequeue(out var item)) {
                _here = item.Value;
                _key  = item.Key;
                Tracing.FindPathDetail.Trace("Dequeue Path at {0} w/ cost={1,4}.", _here, _key);

                Hexside.ForEach(this);
            }
            return _store;
        }

        public sealed override void Invoke(Hexside hexside) {
            var stepCost = _tryDirectedStepCost(_here, hexside);
            if (stepCost > 0) { InvokeInner(hexside, _here, _key+stepCost); }
        }

        private void InvokeInner(Hexside hexside, HexCoords here, int cost) {
            var neighbour = here.GetNeighbour(hexside);
            if (_store.MapSizeHexes.IsOnboard(neighbour)) {
                if (_store[neighbour] > 0) { Enqueue(cost,neighbour,_store); }
            }
        }

        protected void Enqueue(int cost, HexCoords neighbour, BoardStorage<int> store) {
            Tracing.FindPathDetail.Trace("   Enqueue {0}: {1,4}", neighbour, cost);
            store.SetItem(neighbour, cost);
            _queue.Enqueue(cost, neighbour);
        }
    }
}
