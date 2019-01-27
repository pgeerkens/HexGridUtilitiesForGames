#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.ComponentModel;
using System.Windows.Forms;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>TODO</summary>
    public partial class LayeredScrollable : ScrollableControl, ISupportInitialize { //, IMessageFilter {
        /// <summary>TODO</summary>
        public LayeredScrollable() => InitializeComponent();        

        #region ISupportInitialize implementation
        /// <summary>Signals the object that initialization is starting.</summary>
        public virtual void BeginInit() { 
        }
        /// <summary>Signals the object that initialization is complete.</summary>
        public virtual void EndInit() { 
    	//		Application.AddMessageFilter(this);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);

            Layers = new LayerCollection(this.CreateGraphics(), DisplayRectangle.Size);
        }
        #endregion

        /// <summary>TODO</summary>
        public    LayerCollection   Layers    { get; private set; }

        /// <inheritdoc/>
        protected override void OnResize(EventArgs e) {
            Layers.Resize(ClientRectangle);
            base.OnResize(e);
        }

        //#region IMessageFilter implementation
        ///// <summary>Redirect WM_MouseWheel messages to window under mouse.</summary>
        ///// <remarks>Redirect WM_MouseWheel messages to window under mouse (rather than 
        ///// that with focus) with adjusted delta.
        ///// <a href="http://www.flounder.com/virtual_screen_coordinates.htm">Virtual Screen Coordinates</a>
        ///// Dont forget to add this to constructor:
        ///// 			Application.AddMessageFilter(this);
        ///// </remarks>
        ///// <param name="m">The Windows Message to filter and/or process.</param>
        ///// <returns>Success (true) or failure (false) to OS.</returns>
        //[System.Security.Permissions.PermissionSetAttribute(
        //  System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
        //public bool PreFilterMessage(ref Message m) {
        //  if ((WM)m.Msg != WM.MOUSEHWHEEL && (WM)m.Msg != WM.MOUSEWHEEL) return false;
        //  var hWnd = NativeMethods.WindowFromPoint(WindowsMouseInput.GetPointLParam(m.LParam));
        //  var ctl	  = Control.FromHandle(hWnd);
        //  if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
        //    switch((WM)m.Msg) {
        //      case WM.MOUSEHWHEEL:
        //      case WM.MOUSEWHEEL:
        //        DebugTracing.Trace(Tracing.ScrollEvents, true," - {0}.WM.{1}: ", Name, ((WM)m.Msg)); 
        //        return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
        //      default: break;
        //    }
        //  }
        //  return false;
        //}
        //#endregion
    }
}
