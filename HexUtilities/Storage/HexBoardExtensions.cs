#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexPoint   = System.Drawing.Point;
    using HexSize    = System.Drawing.Size;
    using RectangleF = System.Drawing.RectangleF;
    using Matrix     = System.Drawing.Drawing2D.Matrix;
    using CoordsRect = CoordsRectangle;

    /// <summary>.</summary>
    public static partial class HexBoardExtensions {
        /// <summary>Returns the translation transform-@this for the upper-left corner of the specified hex.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="coords">Type: HexCoords - Coordinates of the hex to be tanslated.</param>
        public static Matrix TranslateToHex<THex>(this HexBoard<THex> @this, HexCoords coords)
        where THex:IHex {
            var offset  = @this.UpperLeftOfHex(coords);
            return new Matrix(1, 0, 0, 1, offset.X, offset.Y);
        }

        /// <summary>Returns pixel coordinates of upper-left corner of specified hex.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="coords">The {HexCoords} of the hex of current interest.</param>
        /// <returns>A Point structure containing pixel coordinates for the (upper-left corner of the) specified hex.</returns>
        public static HexPoint UpperLeftOfHex<THex>(this HexBoard<THex> @this, HexCoords coords)
        where THex:IHex
        => new HexPoint(
            coords.User.X * @this.GridSize.Width,
            coords.User.Y * @this.GridSize.Height + (coords.User.X + 1) % 2 * @this.GridSize.Height / 2
        );

        /// <summary>Returns pixel coordinates of centre of specified hex.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="coords">The {HexCoords} of the hex of current interest.</param>
        /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
        public static HexPoint CentreOfHex<THex>(this HexBoard<THex> @this, HexCoords coords)
        where THex:IHex
        => @this.UpperLeftOfHex(coords) + @this.HexCentreOffset;

        /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRect"/>, of the current clipping region.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="visibleClipBounds"></param>
        /// <param name="boardSizeHexes"></param>
        public static CoordsRect GetClipInHexes<THex>(this HexBoard<THex> @this,
                RectangleF visibleClipBounds, HexSize boardSizeHexes)
        where THex:IHex {
            var left   = Math.Max((int)visibleClipBounds.Left   / @this.GridSize.Width  - 1, 0);
            var top    = Math.Max((int)visibleClipBounds.Top    / @this.GridSize.Height - 1, 0);
            var right  = Math.Min((int)visibleClipBounds.Right  / @this.GridSize.Width  + 1, boardSizeHexes.Width);
            var bottom = Math.Min((int)visibleClipBounds.Bottom / @this.GridSize.Height + 1, boardSizeHexes.Height); 
            return new CoordsRect(left, top, right-left, bottom-top);
        }

        /// <summary>Rectangular extent in pixels of the defined mapboard.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        public static HexSize MapSizePixels<THex>(this HexBoard<THex> @this)
        where THex:IHex
        => @this.MapSizeHexes * @this.GridSizePixels;

        ///// <summary>Perform the supplied <paramref name="action"/> for every item in the enumeration.</summary>
        ///// <param name="this">The current {HexBoard}.</param>
        ///// <param name="action"></param>
        //public static void ForEachHex<THex,TBoard>(this TBoard @this, Action<Maybe<THex>> action)
        //where THex:class,IHex where TBoard:HexBoard<THex>
        //=> @this.BoardHexes.ForEachSerial(hex => action(from h in hex select h as THex));
    }
}

namespace PGNapoleonics.HexUtilities {
    using HexPoint   = System.Drawing.Point;
    using HexPointF  = System.Drawing.PointF;
    using HexSize    = System.Drawing.Size;
    using HexSizeF   = System.Drawing.SizeF;
    using Matrix     = System.Drawing.Drawing2D.Matrix;
    using RectangleF = System.Drawing.RectangleF;

    using CoordsRect = CoordsRectangle;

    /// <summary>.</summary>
    public static partial class PanelModelExtensions {
        /// <summary>Returns the translation transform-@this for the upper-left corner of the specified hex.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="coords">Type: HexCoords - Coordinates of the hex to be tanslated.</param>
        public static Matrix TranslateToHex(this IPanelModel @this, HexCoords coords) {
            var offset  = @this.UpperLeftOfHex(coords);
            return new Matrix(1, 0, 0, 1, offset.X, offset.Y);
        }

        /// <summary>Returns pixel coordinates of upper-left corner of specified hex.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="coords">The {HexCoords} of the hex of current interest.</param>
        /// <returns>A Point structure containing pixel coordinates for the (upper-left corner of the) specified hex.</returns>
        public static HexPoint UpperLeftOfHex(this IPanelModel @this, HexCoords coords)
        => new HexPoint(
            coords.User.X * @this.GridSize.Width,
            coords.User.Y * @this.GridSize.Height + (coords.User.X + 1) % 2 * @this.GridSize.Height / 2
        );

        /// <summary>Returns pixel coordinates of centre of specified hex.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="coords">The {HexCoords} of the hex of current interest.</param>
        /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
        public static HexPoint CentreOfHex(this IPanelModel @this, HexCoords coords)
        => @this.UpperLeftOfHex(coords) + @this.HexCentreOffset;

        /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRect"/>, of the current clipping region.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <returns>A Point structure containing pixel coordinates for the (centre of the) specified hex.</returns>
        public static CoordsRect GetClipInHexes(this IPanelModel @this,HexPointF point, HexSizeF size)
        => @this.GetClipInHexes(new RectangleF(point, size), @this.MapSizeHexes);

        /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRect"/>, of the current clipping region.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="visibleClipBounds"></param>
        /// <param name="boardSizeHexes"></param>
        public static CoordsRect GetClipInHexes(this IPanelModel @this,
                RectangleF visibleClipBounds, HexSize boardSizeHexes) {
            var left   = Math.Max((int)visibleClipBounds.Left   / @this.GridSize.Width  - 1, 0);
            var top    = Math.Max((int)visibleClipBounds.Top    / @this.GridSize.Height - 1, 0);
            var right  = Math.Min((int)visibleClipBounds.Right  / @this.GridSize.Width  + 1, boardSizeHexes.Width);
            var bottom = Math.Min((int)visibleClipBounds.Bottom / @this.GridSize.Height + 1, boardSizeHexes.Height); 
            return new CoordsRect(left, top, right-left, bottom-top);
        }

        /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRect"/>, of the current clipping region.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="visibleClipBounds"></param>
        public static CoordsRect GetClipInHexes(this IPanelModel @this, RectangleF visibleClipBounds)
        => @this.GetClipInHexes(visibleClipBounds, @this.MapSizeHexes);

        /// <summary>Rectangular extent in pixels of the defined mapboard.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        public static HexSize MapSizePixels(this IPanelModel @this)
        => @this.MapSizeHexes * @this.GridSizePixels;

        /// <summary>Perform the supplied <paramref name="action"/> for every item in the enumeration.</summary>
        /// <param name="this">The current {HexBoard}.</param>
        /// <param name="action"></param>
        public static void ForEachHex<THex,TBoard>(this IPanelModel @this, Action<Maybe<THex>> action)
        where THex:class,IHex where TBoard:Storage.HexBoard<THex>
        => @this.ForEachHexSerial<IHex>(hex => action(from h in hex select h as THex));
    }
}