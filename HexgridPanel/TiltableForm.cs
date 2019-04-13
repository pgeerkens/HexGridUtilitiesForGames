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
using System;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Storage;
using PGNapoleonics.HexgridPanel.WinForms;

namespace PGNapoleonics.HexgridPanel {
    using HexSize        = System.Drawing.Size;

    public partial class TiltableForm: Form, IMessageFilter {
        public TiltableForm() {
            InitializeComponent();
            Application.AddMessageFilter(this);
        }

        protected IPanelModel    MapBoard;
        protected CustomCoords   CustomCoords;

        #region Event handlers
        protected virtual void HexgridPanel_ScaleChange(object sender,EventArgs e) => OnResizeEnd(e);

        protected virtual void HexgridPanel_MouseMove(object sender, MouseEventArgs e) { }

        protected virtual void HexgridPanelForm_Load(object sender, EventArgs e) {
            if (HexgridPanel == null) return;
            HexgridPanel.ScaleIndex = HexgridPanel.Scales
                                                  .Select((f,i) => new {value=f, index=i})
                                                  .Where(s => s.value==1.0F)
                                                  .Select(s => s.index).FirstOrDefault(); 
            var padding = ToolStripContainer.ContentPanel.Padding;
            Size = HexgridPanel.MapSizePixels  + new HexSize(21,93)
                 + new HexSize(padding.Left+padding.Right, padding.Top+padding.Bottom);
        }

        protected void PanelBoard_GoalHexChange(object sender, HexEventArgs e)
        =>  RefreshAfter(()=>{MapBoard.GoalHex = e.Coords;} );

        protected void PanelBoard_StartHexChange(object sender, HexEventArgs e)
        =>  RefreshAfter(()=>{MapBoard.StartHex = e.Coords;} );

        protected void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e)
        =>  RefreshAfter(()=>{MapBoard.HotspotHex = e.Coords;} );

        protected void RefreshAfter(Action action) { action?.Invoke(); HexgridPanel.Refresh(); }
        #endregion

        #region IMessageFilter implementation
        /// <summary>Redirect WM_MouseWheel messages to window under mouse.</summary>
        /// <remarks>Redirect WM_MouseWheel messages to window under mouse (rather than 
        /// that with focus) with adjusted delta.
        /// <a href="http://www.flounder.com/virtual_screen_coordinates.htm">Virtual Screen Coordinates</a>
        /// Dont forget to add this to constructor:
        /// 			Application.AddMessageFilter(this);
        ///</remarks>
        /// <param name="m">The Windows Message to filter and/or process.</param>
        /// <returns>Success (true) or failure (false) to OS.</returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool PreFilterMessage(ref Message m) {
            if ((WM)m.Msg != WM.MouseHWheel && (WM)m.Msg != WM.MouseWheel) return false;

            var hWnd = NativeMethods.WindowFromPoint(WindowsMouseInput.GetPointLParam(m.LParam));
            var ctl = FromChildHandle(hWnd);
            if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
                switch ((WM)m.Msg) {
                    case WM.MouseHWheel:
                    case WM.MouseWheel:
                        Tracing.ScrollEvents.Trace(true, $" - {Name}.WM.{(WM)m.Msg}: ");
                        return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
                    default: break;
                }
            }
            return false;
        }
        #endregion
    }
}
