#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PGNapoleonics.HexgridPanel.WinForms {
    /// <summary>The interface that must be supported for controls leveraging the <see cref="ScrollableControlExtensions"/> methods.</summary>
    public interface IScrollableControl {
        /// <summary>Returns or sets the current scrolling position for the control.</summary>
        /// <remarks>
        /// The getter must return the current scrolling position; and 
        /// the setter must scroll to the specified position when assigned.
        /// </remarks>
        Point AutoScrollPosition { get; set; }

        /// <summary>Reserved for internal use by <see cref="ScrollableControlExtensions"/>.</summary>
        Point UnappliedScroll    { get; set; }

        /// <summary>The horizontal <see cref="X"/> and vertical <see cref="Y"/> scrolling increments for 'paging'.</summary>
        Point ScrollLargeChange  { get; }
    }

    /// <summary>Extension methoda supporting  horizontal (ie tilt) scrolling on the <see cref="IScrollableControl"/> interface.</summary>
    public static class ScrollableControlExtensions {
        /// <summary>Service routine to execute a Panel scroll.</summary>
        [Obsolete("Use ScrollPanelVertical or ScrollPanelHorizontal instead.")]
        public static void ScrollPanel(this IScrollableControl @this, ScrollEventType type,
                    ScrollOrientation orientation, int sign) {
            ScrollActions [
                    ( (type.HasFlag(ScrollEventType.SmallDecrement))      ? 4 : 0 )
                  + ( (orientation == ScrollOrientation.HorizontalScroll) ? 2 : 0 )
                  + ( (sign == +1)                                        ? 1 : 0 ) ]
            (@this);
        }

        /// <summary>Returns a new <see cref="MouseEventArgs" from the supplies <see cref="Message"./>/></summary>
        /// <param name="this"></param>
        public static MouseEventArgs CreateMouseEventArgs(this Message @this) 
        => new MouseEventArgs(  
              (MouseButtons)@this.WParam.LoWord(),
                            0,
                            @this.LParam.LoWord(),
                            @this.LParam.HiWord(),
                            @this.WParam.HiWord() );

        /// <summary>TODO</summary>
        /// <param name="this">The <see cref="IScrollableControl"/> to be scrolled.</param>
        /// <param name="delta"></param>
        public static void RollHorizontal(this IScrollableControl @this, int delta) {
            @this.UnappliedScroll += new Size(delta, 0);// += delta;
            while (@this.UnappliedScroll.X >= MouseWheelStep) {
                @this.HScrollByOffset( + MouseWheelStep);
                @this.UnappliedScroll -= new Size(MouseWheelStep, 0);
            }
            while (@this.UnappliedScroll.X <= -MouseWheelStep) {
                @this.HScrollByOffset( - MouseWheelStep);
                @this.UnappliedScroll += new Size(MouseWheelStep, 0);
            }
        }

        /// <summary>TODO</summary>
        /// <param name="this">The <see cref="IScrollableControl"/> to be scrolled.</param>
        /// <param name="delta"></param>
        public static void RollVertical(this IScrollableControl @this, int delta) {
            @this.UnappliedScroll += new Size(0, delta);
            while (@this.UnappliedScroll.Y >= MouseWheelStep) {
                @this.VScrollByOffset( + MouseWheelStep);
                @this.UnappliedScroll -= new Size(0, MouseWheelStep);
            }
            while (@this.UnappliedScroll.Y <= -MouseWheelStep) {
                @this.VScrollByOffset( - MouseWheelStep);
                @this.UnappliedScroll += new Size(0, MouseWheelStep);
            }
        }

        /// <summary>TODO</summary>
        /// <param name="this">The <see cref="IScrollableControl"/> to be scrolled.</param>
        /// <param name="delta">The amount to be scrolled.</param>
        public static void HScrollByOffset(this IScrollableControl @this, int delta)
        =>   @this.AutoScrollPosition = new Point (- @this.AutoScrollPosition.X + delta,
                                                   - @this.AutoScrollPosition.Y);

        /// <summary>TODO</summary>
        /// <param name="this">The <see cref="IScrollableControl"/> to be scrolled.</param>
        /// <param name="delta">The amount to be scrolled.</param>
        public static void VScrollByOffset(this IScrollableControl @this, int delta)
        =>  @this.AutoScrollPosition = new Point (-@this.AutoScrollPosition.X,
                                                  -@this.AutoScrollPosition.Y + delta);

        /// <summary>TODO</summary>
        public static void PageUp(this IScrollableControl @this)    
            => @this.RollVertical(-1 *  @this.ScrollLargeChange.Y);
        /// <summary>TODO</summary>
        public static void PageDown(this IScrollableControl @this)  
            => @this.RollVertical(+1 *  @this.ScrollLargeChange.Y);
        /// <summary>TODO</summary>
        public static void PageLeft(this IScrollableControl @this)
            => @this.RollHorizontal(-1 *  @this.ScrollLargeChange.X);
        /// <summary>TODO</summary>
        public static void PageRight(this IScrollableControl @this)
            => @this.RollHorizontal(+1 *  @this.ScrollLargeChange.X);
        /// <summary>TODO</summary>
        public static void LineUp(this IScrollableControl @this)    
            => @this.RollVertical(-1 * MouseWheelStep);
        /// <summary>TODO</summary>
        public static void LineDown(this IScrollableControl @this)  
            => @this.RollVertical(+1 * MouseWheelStep);
        /// <summary>TODO</summary>
        public static void LineLeft(this IScrollableControl @this)
            => @this.RollHorizontal(-1 * MouseWheelStep);
        /// <summary>TODO</summary>
        public static void LineRight(this IScrollableControl @this)
            => @this.RollHorizontal(+1 * MouseWheelStep);

        /// <summary>TODO</summary>
        public static int MouseWheelStep
        => SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines;

        private delegate void ScrollAction(IScrollableControl ctrl);

        private static IReadOnlyList<ScrollAction> ScrollActions = new List<ScrollAction> {
                PageUp,   PageDown,  PageLeft, PageRight,
                LineUp,   LineDown,  LineLeft, LineRight
            };
    }
}
