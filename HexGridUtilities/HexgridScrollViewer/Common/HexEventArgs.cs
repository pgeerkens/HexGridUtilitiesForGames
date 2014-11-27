#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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
using System.Windows;
using System.Windows.Input;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridScrollViewer {
  /// <summary></summary>
  public class HexEventArgs : System.Windows.Forms.MouseEventArgs {
    /// <summary>TODO</summary>
    public HexCoords  Coords       { get; private set; }

    /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
    public static  bool    IsAltKeyDown      { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Alt); } }
    /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
    public static  bool    IsCtlKeyDown      { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control); } }
    /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
    public static  bool    IsShiftKeyDown    { get { return Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift); } }

    /// <summary>TODO</summary>
    public HexEventArgs(HexCoords coords) 
      : this(coords, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None,0,0,0,0)) {}
    /// <summary>TODO</summary>
    public HexEventArgs(HexCoords coords, System.Windows.Forms.Keys modifierKeys) 
      : this(coords, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None,0,0,0,0), modifierKeys) {}
    ///// <summary>TODO</summary>
    //public HexEventArgs(HexCoords coords, System.Windows.Forms.MouseEventArgs e) 
    //  : this(coords, e, System.Windows.Forms.Keys.None) {}
    /// <summary>TODO</summary>
   [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
    public HexEventArgs(HexCoords coords, System.Windows.Forms.MouseEventArgs e, System.Windows.Forms.Keys modifierKeys)
      : base(e.Button,e.Clicks,e.X,e.Y,e.Delta) {
      Coords       = coords;
    }

    public HexEventArgs(HexCoords coords, System.Windows.Forms.MouseEventArgs e) 
      : base(e.Button,e.Clicks,e.X,e.Y,e.Delta) {
      Coords       = coords;
      //ModifierKeys = (isShiftKeyDown ? Keys.Shift   : Keys.None)
      //             | (isCtlKeyDown   ? Keys.Control : Keys.None)
      //             | (isAltKeyDown   ? Keys.Alt     : Keys.None);
    }
  }

  /// <summary></summary>
  public class HexMouseEventArgs : MouseEventArgs {

    /// <summary>TODO</summary>
    public HexCoords  GetCoords { 
      get; private set;
    }

    public HexMouseEventArgs(
        MouseDevice mouse,
        int timestamp,
        MouseButton button
    ) : base(mouse,timestamp){
    }

    public HexMouseEventArgs(
        MouseDevice mouse,
        int timestamp,
        MouseButton button,
        StylusDevice stylusDevice
    )  : base(mouse,timestamp,stylusDevice) {

    }

  }
}
