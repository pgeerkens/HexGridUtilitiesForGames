#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;

namespace PGNapoleonics.HexUtilities {
    using HexPoint  = System.Drawing.Point;
    using HexPointF = System.Drawing.PointF;
    using HexSize   = System.Drawing.Size;
    using HexSizeF  = System.Drawing.SizeF;
    using HexMatrix = System.Drawing.Drawing2D.Matrix;

    /// <summary>.</summary>
    public static class HexPickingExtensions {
        /// <summary>Scroll position on the (possibly transposed) HexGrid.</summary>
        /// <param name="this"></param>
        /// <param name="scrollPosition"></param>
        public static HexPoint GetScrollPosition(this IHexgrid @this, HexPoint scrollPosition)
        => @this.IsTransposed ? TransposePoint(scrollPosition)
                              : scrollPosition;

        /// <summary>.</summary>
        /// <param name="this"></param>
        public static HexSizeF GridSizeF(this IHexgrid @this) => @this.GridSize.Scale(@this.Scale);

        /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
        /// <param name="this"></param>
        /// <param name="point">Screen point specifying hex to be identified.</param>
        /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
        static HexCoords GetHexCoords(this IHexgrid @this, HexPointF point, HexSizeF autoScroll) {

            // Adjust for origin not as assumed by GetCoordinate().
            var grid  = new HexSizeF(@this.GridSizeF().Width*2F/3F, @this.GridSizeF().Height);
            point    -= autoScroll + grid - new HexSizeF(@this.Margin.Width,@this.Margin.Height);

            return HexCoords.NewCanonCoords( @this.GetCoordinate(@this.MatrixX(), point), 
                                             @this.GetCoordinate(@this.MatrixY(), point) );
        }

        /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
        /// <param name="this"></param>
        /// <param name="point">Screen point specifying hex to be identified.</param>
        /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
        public static HexCoords GetHexCoords(this IHexgrid @this, HexPoint point, HexSize autoScroll)
        => @this.IsTransposed ? @this.GetHexCoordsInner(TransposePoint(point), TransposeSize(autoScroll))
                              : @this.GetHexCoordsInner(point, autoScroll);
    
        /// <summary><c>HexCoords</c> for the hex at the screen point, with the given AutoScroll position.</summary>
        /// <param name="this"></param>
        /// <param name="point">Screen point specifying hex to be identified.</param>
        /// <param name="autoScroll">AutoScrollPosition for game-display Panel.</param>
        static HexCoords GetHexCoordsInner(this IHexgrid @this, HexPoint point, HexSize autoScroll) {
            // Adjust for origin not as assumed by GetCoordinate().
            var grid  = new HexSize((int)(@this.GridSizeF().Width*2F/3F), (int)@this.GridSizeF().Height);
            point    -= autoScroll + grid - @this.Margin;

            return HexCoords.NewCanonCoords( @this.GetCoordinate(@this.MatrixX(), point), 
                                             @this.GetCoordinate(@this.MatrixY(), point) );
        }

        /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
        /// <param name="this"></param>
        /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        public static HexPoint HexCenterPoint(this IHexgrid @this, HexCoords coordsNewULHex)
        => @this.IsTransposed ? TransposePoint(@this.HexCenterPointInner(coordsNewULHex))
                              : @this.HexCenterPointInner(coordsNewULHex);

        /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
        /// <param name="this"></param>
        /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        static HexPoint HexCenterPointInner(this IHexgrid @this, HexCoords coordsNewULHex)
        => @this.HexOrigin(coordsNewULHex)
         + new HexSize((int)(@this.GridSizeF().Width*2F/3F), (int)@this.GridSizeF().Height);

        /// <summary>Returns the pixel coordinates of the center of the specified hex.</summary>
        /// <param name="this"></param>
        /// <param name="coords"><see cref="HexCoords"/> specification for which pixel center is desired.</param>
        /// <returns>Pixel coordinates of the center of the specified hex.</returns>
        public static HexPoint HexOrigin(this IHexgrid @this, HexCoords coords)
        => new HexPoint(
                (int)(@this.GridSizeF().Width  * coords.User.X),
                (int)(@this.GridSizeF().Height * coords.User.Y   + @this.GridSizeF().Height/2 * (coords.User.X+1)%2)
            );

        /// <summary>Calculates a (canonical X or Y) grid-coordinate for a point, from the supplied 'picking' matrix.</summary>
        /// <param name="this"></param>
        /// <param name="matrix">The 'picking-matrix' matrix</param>
        /// <param name="point">The screen point identifying the hex to be 'picked'.</param>
        /// <returns>A (canonical X or Y) grid coordinate of the 'picked' hex.</returns>
        public static int GetCoordinate (this IHexgrid @this, HexMatrix matrix, HexPoint point) {
              var points = new HexPoint[] {point};
              matrix.TransformPoints(points);

		      return (int) Math.Floor( (points[0].X + points[0].Y + 2F) / 3F );
	    }

        /// <summary>Calculates a (canonical X or Y) grid-coordinate for a point, from the supplied 'picking' matrix.</summary>
        /// <param name="this"></param>
        /// <param name="matrix">The 'picking-matrix' matrix</param>
        /// <param name="point">The screen point identifying the hex to be 'picked'.</param>
        /// <returns>A (canonical X or Y) grid coordinate of the 'picked' hex.</returns>
        public static int GetCoordinate (this IHexgrid @this, HexMatrix matrix, HexPointF point) {
            var points = new HexPointF[] {point};
            matrix.TransformPoints(points);

		    return (int) Math.Floor( (points[0].X + points[0].Y + 2F) / 3F );
	    }

        /// <summary><see cref="HexMatrix"/> for 'picking' the <B>X</B> hex coordinate</summary>
        public static HexMatrix MatrixX(this IHexgrid @this)
        => new HexMatrix(
               (3.0F/2.0F)/@this.GridSizeF().Width, (3.0F/2.0F)/@this.GridSizeF().Width,
                     1.0F/@this.GridSizeF().Height,      -1.0F/@this.GridSizeF().Height,  -0.5F,-0.5F);
        /// <summary><see cref="HexMatrix"/> for 'picking' the <B>Y</B> hex coordinate</summary>
        public static HexMatrix MatrixY(this IHexgrid @this)
        => new HexMatrix(
                                        0.0F,  (3.0F/2.0F)/@this.GridSizeF().Width,
               2.0F/@this.GridSizeF().Height,        1.0F/@this.GridSizeF().Height,  -0.5F,-0.5F);

        static HexPoint TransposePoint(HexPoint point) => new HexPoint(point.Y, point.X);
        static HexSize  TransposeSize(HexSize  size)   => new HexSize (size.Height, size.Width);
    }
}
