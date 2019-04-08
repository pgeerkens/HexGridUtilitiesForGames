#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>TODO</summary>
    public static class PriorityQueueFactory {
        /// <summary>TODO</summary>
        internal static IPriorityQueue<TPriority,TValue> NewDictionaryQueue<TPriority, TValue>()
        where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority>
        => new DictionaryPriorityQueue<TPriority,TValue>();

        /// <summary>TODO</summary>
        internal static IPriorityQueue<int,TValue> NewHotPriorityQueue<TValue>()
        => NewHotPriorityQueue<TValue>(256);

        /// <summary>TODO</summary>
        internal static IPriorityQueue<int,TValue> NewHotPriorityQueue<TValue>(int initialSize)
        => HotPriorityQueue.New<TValue>(0,initialSize);
    }
}
