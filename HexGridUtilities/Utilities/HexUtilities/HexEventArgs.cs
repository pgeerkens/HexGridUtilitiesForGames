#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PG_Napoleonics.Utilities.HexUtilities {
  /// <summary></summary>
  public class HexEventArgs : MouseEventArgs {
    public ICoordsUser  Coords       { get; private set; }

    public bool         Alt          { get { return ModifierKeys.HasFlag(Keys.Alt);  } }
    public bool         Control      { get { return ModifierKeys.HasFlag(Keys.Control); } }
    public bool         Shift        { get { return ModifierKeys.HasFlag(Keys.Shift); } }

    public Keys         ModifierKeys { get; private set; }

    public HexEventArgs(ICoordsUser coords) 
      : this(coords, new MouseEventArgs(MouseButtons.None,0,0,0,0)) {}
    public HexEventArgs(ICoordsUser coords, Keys modifierKeys) 
      : this(coords, new MouseEventArgs(MouseButtons.None,0,0,0,0), modifierKeys) {}
    public HexEventArgs(ICoordsUser coords, MouseEventArgs e) 
      : this(coords, e, Keys.None) {}
    public HexEventArgs(ICoordsUser coords, MouseEventArgs e, Keys modifierKeys)
      : base(e.Button,e.Clicks,e.X,e.Y,e.Delta) {
      Coords       = coords;
      ModifierKeys = modifierKeys;
    }
  }
}
