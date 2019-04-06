#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Drawing;

namespace PGNapoleonics.HexgridPanel.WinForms {
    /// <summary>The interface that must be supported for controls leveraging the <see cref="ScrollableControlExtensions"/> methods.</summary>
    internal interface IScrollableControl {
        /// <summary>Returns or sets the current scrolling position for the control.</summary>
        /// <remarks>
        /// The getter must return the current scrolling position; and 
        /// the setter must scroll to the specified position when assigned.
        /// </remarks>
        Point AutoScrollPosition { get; set; }

        /// <summary>The horizontal (X) and vertical (Y) scrolling increments for 'paging'.</summary>
        Point ScrollLargeChange  { get; }

        /// <summary>Reserved for internal use by <see cref="ScrollableControlExtensions"/>.</summary>
        /// <remarks>Gets or sets the current amount of unapplied scroll, as a <see cref="Point"/> object.</remarks>
        Point UnappliedScroll    { get; set; }
    }
}
