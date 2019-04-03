#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Windows.Controls;
using System.Windows.Input;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexgridExampleWinforms2 {
    /// <summary>Interaction logic for HexgridScrollableViewer.xaml</summary>
    public partial class TiltAwareScrollViewer : ScrollViewer {
        /// <summary>TODO</summary>
        public TiltAwareScrollViewer() : base() {
    //      InitializeComponent();
        }

        #region Mouse Tilt Wheel (MouseHWheel) event implementation
        /// <summary>Occurs when the mouse tilt-wheel moves while the control has focus.</summary>
        public event EventHandler<MouseEventArgs> MouseHWheel;

        private int _wheelHPos = 0;   //!< <summary>Unapplied horizontal scroll.</summary>

        /// <summary>Scrolls horizontally and raises the MouseHWheel event</summary>
        /// <param name="e"></param>
        protected virtual void OnMouseHWheel(MouseWheelEventArgs e) {
          if (e == null) throw new ArgumentNullException(nameof(e));
            if (CanContentScroll) {
                ScrollToHorizontalOffset(HorizontalOffset + e.Delta);

                if (MouseHWheel != null) MouseHWheel.Raise(this, e);
            }
        }
        #endregion
    }
}
