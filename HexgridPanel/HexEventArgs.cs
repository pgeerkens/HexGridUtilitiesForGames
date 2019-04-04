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
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridPanel {
    /// <summary></summary>
    public class HexEventArgs : MouseEventArgs {
        /// <summary>TODO</summary>
        public HexEventArgs(HexCoords coords) 
        : this(coords, Keys.None, MouseButtons.None,0,0,0,0) {}

        /// <summary>TODO</summary>
        public HexEventArgs(HexCoords coords, Keys modifierKeys) 
        : this(coords, modifierKeys, MouseButtons.None,0,0,0,0) {}

        /// <summary>TODO</summary>
        public HexEventArgs(HexCoords coords, Keys modifierKeys, 
        MouseButtons buttons, int clicks, int x, int y, int delta)
        : base(buttons,clicks,x,y,delta) {
            Coords       = coords;
            ModifierKeys = modifierKeys;
        }

        /// <summary>TODO</summary>
        public HexCoords  Coords   { get; }

        /// <summary>TODO</summary>
        public Keys   ModifierKeys { get; }

        /// <summary>TODO</summary>
        public bool       Alt      => ModifierKeys.HasFlag(Keys.Alt);
        /// <summary>TODO</summary>
        public bool       Control  => ModifierKeys.HasFlag(Keys.Control);
        /// <summary>TODO</summary>
        public bool       Shift    => ModifierKeys.HasFlag(Keys.Shift);
    }
}
