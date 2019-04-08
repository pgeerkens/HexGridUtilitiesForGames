#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

#pragma warning disable 1587
/// <summary>Fast efficient <b>Shadow-Casting</b> 
/// implementation of 3D Field-of-View on a <see cref="Hexgrid"/> map.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.FieldOfView {
    using HexSize = System.Drawing.Size;

    /// <summary>Implementation of <see cref="IShadingMask"/> using a backing array of BitArray.</summary>
    internal class ArrayFieldOfView : IShadingMask {
        private readonly object _syncLock = new object();

        public ArrayFieldOfView(IFovBoard board) {
            _mapSizeHexes = board.MapSizeHexes;
            _fovBacking   = ( from i in Enumerable.Range(0,board.MapSizeHexes.Width)
                              select new BitArray(board.MapSizeHexes.Height)
                            ).ToArray();
        }

        public bool this[HexCoords coords] {
            get => _mapSizeHexes.IsOnboard(coords) && _fovBacking[coords.User.X][coords.User.Y];
            internal set {
                lock (_syncLock) {
                    if (_mapSizeHexes.IsOnboard(coords)) { _fovBacking[coords.User.X][coords.User.Y] = value; }
                }
            }
        }

        private readonly BitArray[] _fovBacking;

        private readonly HexSize _mapSizeHexes;
    }
}
