//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
//
//  File: ConcurrentPriorityQueue.cs
//
// http://code.msdn.microsoft.com/Samples-for-Parallel-b4b76364/sourcecode?fileId=44488&pathId=1696822056
//
/* Used under the terms of the Microsoft Limited Public License:
MICROSOFT LIMITED PUBLIC LICENSE

This license governs use of code marked as “sample” or “example” available on this web site without 
 * a license agreement, as provided under the section above titled 
 * NOTICE SPECIFIC TO SOFTWARE AVAILABLE ON THIS WEB SITE.” 
 * If you use such code (the “software”), you accept this license. If you do not accept the license, 
 * do not use the software.

1. Definitions
The terms “reproduce,” “reproduction,” “derivative works,” and “distribution” have the same meaning 
 * here as under U.S. copyright law.
A “contribution” is the original software, or any additions or changes to the software.
A “contributor” is any person that distributes its contribution under this license.
“Licensed patents” are a contributor’s patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant - Subject to the terms of this license, including the license conditions and 
 * limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free 
 * copyright license to reproduce its contribution, prepare derivative works of its contribution, 
 * and distribute its contribution or any derivative works that you create.
(B) Patent Grant - Subject to the terms of this license, including the license conditions and 
 * limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free 
 * license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or 
 * otherwise dispose of its contribution in the software or derivative works of the contribution in 
 * the software.

3. Conditions and Limitations
(A) No Trademark License- 
 * This license does not grant you rights to use any contributors’ name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed 
 * by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark,
 * and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this 
 * license by including a complete copy of this license with your distribution.  If you distribute 
 * any portion of the software in compiled or object code form, you may only do so under a license 
 * that complies with this license.
(E) The software is licensed “as-is.” You bear the risk of using it. The contributors give no express 
 * warranties, guarantees or conditions.  You may have additional consumer rights under your local 
 * laws which this license cannot change. To the extent permitted under your local laws, the contri-
 * butors exclude the implied warranties of merchantability, fitness for a particular purpose and 
 * non-infringement.
(F) Platform Limitation - The licenses granted in sections 2(A) and 2(B) extend only to the software 
 * or derivative works that you create that run on a Microsoft Windows operating system product.
*/
//--------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using PG_Napoleonics.HexUtilities.Common;

namespace PG_Napoleonics.HexUtilities.Common {
    /// <summary>Provides a thread-safe priority queue data structure.</summary>
    /// <typeparam name="TKey">Specifies the type of keys used to prioritize values.</typeparam>
    /// <typeparam name="TValue">Specifies the type of elements in the queue.</typeparam>
    /// <remarks>The implementation is slightly modified from that posted on the Microsoft site, 
    /// by the change to a custom <c>HexKeyValuePair</c> and the implementation of the interface 
    /// <c>IPriorityQueue</c> to facilitate interchane with DictionaryPriorityQueue at shorter 
    /// ranges.</remarks>
    [DebuggerDisplay("Count={Count}")]
    public class ConcurrentPriorityQueue<TKey, TValue> :
      IProducerConsumerCollection<HexKeyValuePair<TKey,TValue>>,
      IPriorityQueue<TKey,TValue>
      where TKey : struct, IEquatable<TKey>, IComparable<TKey>, IComparable
    {
        private readonly object _syncLock = new object();
        private readonly MinBinaryHeap _minHeap = new MinBinaryHeap();

        /// <summary>Initializes a new instance of the ConcurrentPriorityQueue class.</summary>
        public ConcurrentPriorityQueue() {}

        /// <summary>Initializes a new instance of the ConcurrentPriorityQueue class that contains elements copied from the specified collection.</summary>
        /// <param name="collection">The collection whose elements are copied to the new ConcurrentPriorityQueue.</param>
        public ConcurrentPriorityQueue(IEnumerable<HexKeyValuePair<TKey, TValue>> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            foreach (var item in collection) _minHeap.Insert(item);
        }

        /// <summary>Adds the key/value pair to the priority queue.</summary>
        /// <param name="priority">The priority of the item to be added.</param>
        /// <param name="value">The item to be added.</param>
        public void Enqueue(TKey priority, TValue value)
        {
            Enqueue(new HexKeyValuePair<TKey, TValue>(priority, value));
        }

        /// <summary>Adds the key/value pair to the priority queue.</summary>
        /// <param name="item">The key/value pair to be added to the queue.</param>
        public void Enqueue(HexKeyValuePair<TKey, TValue> item)
        {
            lock (_syncLock) _minHeap.Insert(item);
        }

        /// <summary>Attempts to remove and return the next prioritized item in the queue.</summary>
        /// <param name="result">
        /// When this method returns, if the operation was successful, result contains the object removed. If
        /// no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an element was removed and returned from the queue succesfully; otherwise, false.
        /// </returns>
        public bool TryDequeue(out HexKeyValuePair<TKey, TValue> result)
        {
            lock (_syncLock)
            {
                if (_minHeap.Any())
                {
                    result = _minHeap.Remove();
                    return true;
                }
            }
            result = default(HexKeyValuePair<TKey, TValue>);
            return false;
        }

        /// <summary>Attempts to return the next prioritized item in the queue.</summary>
        /// <param name="result">
        /// When this method returns, if the operation was successful, result contains the object.
        /// The queue was not modified by the operation.
        /// </param>
        /// <returns>
        /// true if an element was returned from the queue succesfully; otherwise, false.
        /// </returns>
        public bool TryPeek(out HexKeyValuePair<TKey, TValue> result)
        {
            lock (_syncLock)
            {
                if (_minHeap.Any())
                {
                    result = _minHeap.Peek();
                    return true;
                }
            }
            result = default(HexKeyValuePair<TKey, TValue>);
            return false;
        }

        /// <inheritdoc/>
        public bool Any()   { lock (_syncLock) return _minHeap.Any(); }

        /// <summary>Empties the queue.</summary>
        public void Clear() { lock (_syncLock) _minHeap.Clear(); }

        /// <summary>Gets the number of elements contained in the queue.</summary>
        public int Count    { get { lock (_syncLock) return _minHeap.Count; } }

        /// <summary>Copies the elements of the collection to an array, starting at a particular array index.</summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the queue.
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <remarks>The elements will not be copied to the array in any guaranteed order.</remarks>
        public void CopyTo(HexKeyValuePair<TKey, TValue>[] array, int index)
        {
          lock (_syncLock) _minHeap.Items.CopyTo(array, index); 
        }

        /// <summary>Copies the elements stored in the queue to a new array.</summary>
        /// <returns>A new array containing a snapshot of elements copied from the queue.</returns>
        public HexKeyValuePair<TKey, TValue>[] ToArray()
        {
          lock (_syncLock)
          {
            var clonedHeap = new MinBinaryHeap(_minHeap);
            var result = new HexKeyValuePair<TKey, TValue>[_minHeap.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = clonedHeap.Remove();
            }
            return result;
          }
        }

        /// <summary>Attempts to add an item in the queue.</summary>
        /// <param name="item">The key/value pair to be added.</param>
        /// <returns>
        /// true if the pair was added; otherwise, false.
        /// </returns>
        bool IProducerConsumerCollection<HexKeyValuePair<TKey, TValue>>.TryAdd(HexKeyValuePair<TKey, TValue> item)
        {
          Enqueue(item);
          return true;
        }

        /// <summary>Attempts to remove and return the next prioritized item in the queue.</summary>
        /// <param name="item">
        /// When this method returns, if the operation was successful, result contains the object removed. If
        /// no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an element was removed and returned from the queue succesfully; otherwise, false.
        /// </returns>
        bool IProducerConsumerCollection<HexKeyValuePair<TKey, TValue>>.TryTake(out HexKeyValuePair<TKey, TValue> item)
        {
          return TryDequeue(out item);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator for the contents of the queue.</returns>
        /// <remarks>
        /// The enumeration represents a moment-in-time snapshot of the contents of the queue. It does not
        /// reflect any updates to the collection after GetEnumerator was called. The enumerator is safe to
        /// use concurrently with reads from and writes to the queue.
        /// </remarks>
        public IEnumerator<HexKeyValuePair<TKey, TValue>> GetEnumerator()
        {
          var arr = ToArray();
          return ((IEnumerable<HexKeyValuePair<TKey, TValue>>)arr).GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /// <summary>Copies the elements of the collection to an array, starting at a particular array index.</summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the queue.
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins.
        /// </param>
        void ICollection.CopyTo(Array array, int index)
        {
          lock (_syncLock) ((ICollection)_minHeap.Items).CopyTo(array, index);
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized with the SyncRoot.
        /// </summary>
        bool ICollection.IsSynchronized { get { return true; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        object ICollection.SyncRoot { get { return _syncLock; } }

        /// <summary>Implements a binary heap that prioritizes smaller values.</summary>
        private sealed class MinBinaryHeap
        {
          private readonly List<HexKeyValuePair<TKey, TValue>> _items;

          /// <summary>Initializes an empty heap.</summary>
          public MinBinaryHeap()
          {
              _items = new List<HexKeyValuePair<TKey, TValue>>();
          }

          /// <summary>Initializes a heap as a copy of another heap instance.</summary>
          /// <param name="heapToCopy">The heap to copy.</param>
          /// <remarks>Key/Value values are not deep cloned.</remarks>
          public MinBinaryHeap(MinBinaryHeap heapToCopy)
          {
              _items = new List<HexKeyValuePair<TKey, TValue>>(heapToCopy.Items);
          }

          /// <summary>Empties the heap.</summary>
          public void Clear() { _items.Clear(); }

          /// <summary>Adds an item to the heap.</summary>
          public void Insert(TKey key, TValue value)
          {
              // Create the entry based on the provided key and value
              Insert(new HexKeyValuePair<TKey, TValue>(key, value));
          }

          /// <summary>Adds an item to the heap.</summary>
          public void Insert(HexKeyValuePair<TKey,TValue> entry)
          {
              // Add the item to the list, making sure to keep track of where it was added.
              _items.Add(entry);
              int pos = _items.Count - 1;

              // If the new item is the only item, we're done.
              if (pos == 0) return;

              // Otherwise, perform log(n) operations, walking up the tree, swapping
              // where necessary based on key values
              while (pos > 0)
              {
                  // Get the next position to check
                  int nextPos = (pos-1) / 2;

                  // Extract the entry at the next position
                  var toCheck = _items[nextPos];

                  // Compare that entry to our new one.  If our entry has a smaller key, move it up.
                  // Otherwise, we're done.
                  if (entry.Key.CompareTo(toCheck.Key) < 0)
                  {
                      _items[pos] = toCheck;
                      pos = nextPos;
                  }
                  else break;
              }

              // Make sure we put this entry back in, just in case
              _items[pos] = entry;
          }

          /// <summary>Returns the entry at the top of the heap.</summary>
          public HexKeyValuePair<TKey, TValue> Peek()
          {
              // Returns the first item
              if (_items.Count == 0) throw new InvalidOperationException("The heap is empty.");
              return _items[0];
          }

          /// <summary>Removes the entry at the top of the heap.</summary>
          public HexKeyValuePair<TKey, TValue> Remove()
          {
              // Get the first item and save it for later (this is what will be returned).
              if (_items.Count == 0) throw new InvalidOperationException("The heap is empty.");
              HexKeyValuePair<TKey, TValue> toReturn = _items[0];

              // Remove the first item if there will only be 0 or 1 items left after doing so.  
              if (_items.Count <= 2) _items.RemoveAt(0);
              // A reheapify will be required for the removal
              else
              {
                  // Remove the first item and move the last item to the front.
                  _items[0] = _items[_items.Count - 1];
                  _items.RemoveAt(_items.Count - 1);

                  // Start reheapify
                  int current = 0, possibleSwap = 0;

                  // Keep going until the tree is a heap
                  while (true)
                  {
                      // Get the positions of the node's children
                      int leftChildPos = 2 * current + 1;
                      int rightChildPos = leftChildPos + 1;

                      // Should we swap with the left child?
                      if (leftChildPos < _items.Count)
                      {
                          // Get the two entries to compare (node and its left child)
                          var entry1 = _items[current];
                          var entry2 = _items[leftChildPos];

                          // If the child has a lower key than the parent, set that as a possible swap
                          if (entry2.Key.CompareTo(entry1.Key) < 0) possibleSwap = leftChildPos;
                      }
                      else break; // if can't swap this, we're done

                      // Should we swap with the right child?  Note that now we check with the possible swap
                      // position (which might be current and might be left child).
                      if (rightChildPos < _items.Count)
                      {
                          // Get the two entries to compare (node and its left child)
                          var entry1 = _items[possibleSwap];
                          var entry2 = _items[rightChildPos];

                          // If the child has a lower key than the parent, set that as a possible swap
                          if (entry2.Key.CompareTo(entry1.Key) < 0) possibleSwap = rightChildPos;
                      }

                      // Now swap current and possible swap if necessary
                      if (current != possibleSwap)
                      {
                          var temp = _items[current];
                          _items[current] = _items[possibleSwap];
                          _items[possibleSwap] = temp;
                      }
                      else break; // if nothing to swap, we're done

                      // Update current to the location of the swap
                      current = possibleSwap;
                  }
              }

              // Return the item from the heap
              return toReturn;
          }

          /// <summary>Returns whether any objects are stored in the heap.</summary>
          public bool Any() { return _items.Any(); }

          /// <summary>Gets the number of objects stored in the heap.</summary>
          public int Count { get { return _items.Count; } }

          internal List<HexKeyValuePair<TKey, TValue>> Items { get { return _items; } }
        }
    }
}