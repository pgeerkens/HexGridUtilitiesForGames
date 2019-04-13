#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Pathfinding {
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification="The suffix has an unambiguous meaning in the application domain.")]
    public interface IPriorityQueue<TPriority,TValue> 
    where TPriority : struct, IEquatable<TPriority>, IComparable<TPriority> {
        /// <summary>The number of items in the queue.</summary>
        int  Count { get; }

        /// <summary>Returns whether any elements exist in the heap.</summary>
        bool Any();

        /// <summary>Creates and adds an entry with the specified <c>priority</c> and <c>value</c> to the queue.></summary>
        /// <param name="priority">The <c>TPriority</c> value for the new entry.</param>
        /// <param name="value">The <c>TValue</c> value for the new entry.</param>
        void Enqueue(TPriority priority, TValue value);

        /// <summary>Adds the specified <c>HexKeyValuePair</c> to the queue.</summary>
        /// <param name="item">The <c>HexKeyValuePair {Tpriority,TValue}</c> entry to be added to the queue. </param>
        void Enqueue(HexKeyValuePair<TPriority,TValue> item);

        /// <summary>Returns whether the top queue entry has been successfully stored in <c>result</c> 
        /// and removed from the queue.</summary>
        bool TryDequeue(out HexKeyValuePair<TPriority,TValue> result);

        /// <summary>Returns whether the top queue entry has been successfully stored in <c>result</c>.</summary>
        bool TryPeek(out HexKeyValuePair<TPriority,TValue> result);
    }
}
