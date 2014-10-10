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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

using HexgridExampleCommon;

namespace HexgridExampleWinForms {
  using MapGridDisplay = PGNapoleonics.HexgridPanel.MapDisplay<MapGridHex>;

  internal sealed partial class HexgridExampleWinForms : Form, IMessageFilter {
    private bool            _isPanelResizeSuppressed = false;
    private MapGridDisplay _mapBoard;

    public HexgridExampleWinForms() {
      InitializeComponent();
      Application.AddMessageFilter(this);

      ComponentResourceManager resources = new ComponentResourceManager(typeof(HexgridExampleWinForms));
      this._hexgridPanel.SetScales ( (IList<float>)(resources.GetObject("_hexgridPanel.Scales")) );

      this._hexgridPanel.ScaleChange += new EventHandler<EventArgs>((o,e) => OnResizeEnd(e));

      LoadTraceMenu();

      comboBoxMapSelection.Items.AddRange(
         Map.MapList.Select(item => item.MapName).ToArray()
      );
      comboBoxMapSelection.SelectedIndex = 0;

//      helpProvider1.SetShowHelp(this,true);
    }
    protected override CreateParams CreateParams { 
			get { return this.SetCompositedStyle(base.CreateParams); }
		}

    partial void LoadTraceMenu();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.Conditional("TRACE")]
    partial void LoadTraceMenu() {
      foreach(var item in Enum.GetValues(typeof(TraceFlags))) {
        var menuItem = new System.Windows.Forms.ToolStripMenuItem();
        menuItemDebug.DropDownItems.Add(menuItem);
        menuItem.Name         = "menuItemDebugTracing" + item.ToString();
        menuItem.Size         = new System.Drawing.Size(143, 22);
        menuItem.Text         = item.ToString();
        menuItem.CheckOnClick = true;
        menuItem.Click       += new System.EventHandler(this.menuItemDebugTracing_Click);
      }
    }

    private void LoadLandmarkMenu() {
      menuItemLandmarks.Items.Clear();
      menuItemLandmarks.Items.Add("None");
      foreach(var landmark in _mapBoard.Landmarks) {
        menuItemLandmarks.Items.Add(string.Format(CultureInfo.InvariantCulture, "{0}", landmark.Coords));
      }
      menuItemLandmarks.SelectedIndexChanged += new EventHandler(menuItemLandmarks_SelectedIndexChanged);
      menuItemLandmarks.SelectedIndex = 0; 
    }

    #region Event handlers
    private void HexGridExampleForm_Load(object sender, EventArgs e) {
      _hexgridPanel.SetScales (new List<float>() {0.707F,  0.841F, 1.000F, 1.189F, 1.414F}.AsReadOnly());
      _hexgridPanel.ScaleIndex = _hexgridPanel.Scales
                              .Select((f,i) => new {value=f, index=i})
                              .Where(s => s.value==1.0F)
                              .Select(s => s.index).FirstOrDefault(); 
      var padding = this.toolStripContainer1.ContentPanel.Padding;
      Size = _hexgridPanel.MapSizePixels  + new Size(21,93)
           + new Size(padding.Left+padding.Right, padding.Top+padding.Bottom);
    }

    protected override void OnResizeBegin(EventArgs e) {
      base.OnResizeBegin(e);
      _isPanelResizeSuppressed = true;
    }
    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      if (IsHandleCreated && ! _isPanelResizeSuppressed) _hexgridPanel.SetScrollLimits(_mapBoard);
    }
    protected override void OnResizeEnd(EventArgs e) {
      base.OnResizeEnd(e);
      _isPanelResizeSuppressed = false;
      _hexgridPanel.SetScrollLimits(_mapBoard);
    }

    private void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
      var hotHex       = _mapBoard.HotspotHex;
      statusLabel.Text = string.Format(CultureInfo.InvariantCulture,
        // "Hotspot Hex: {0:gi3} / {1:uI4} / {2:c5}; {3:r6}; Path Length = {4}",
        PGNapoleonics.HexGridExample2.Properties.Resources.StatusLabelText,
        hotHex, hotHex, hotHex,
        _mapBoard.StartHex - hotHex, (_mapBoard.Path==null ? 0 : _mapBoard.Path.TotalCost));
    }

    private void txtPathCutover_TextChanged(object sender, EventArgs e) {
      int value;
      if (Int32.TryParse(txtPathCutover.Text, out value)) {
        txtPathCutover.Tag = value;
      } else {
        txtPathCutover.Text = txtPathCutover.Tag.ToString();
        value = (int)txtPathCutover.Tag;
      }
      _mapBoard.FovRadius   =
      _mapBoard.RangeCutoff = value;
      Refresh();
    }

    private void menuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
      _mapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex;
      _hexgridPanel.SetMapDirty();
      Update();
    }

    private void menuItemDebugTracing_Click(object sender, EventArgs e) {
      var item = (ToolStripMenuItem)sender;
      item.CheckState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
      var name = item.Name.Replace("menuItemDebugTracing","");
      var flag = (TraceFlags)Enum.Parse(typeof(TraceFlags),name);
      if( item.Checked)   DebugTracing.EnabledFlags |=  flag;
      else                DebugTracing.EnabledFlags &= ~flag;
    }

    private void menuItemHelpContents_Click(object sender, EventArgs e) {
//      helpProvider1.SetShowHelp(this,true);
    }

    private void comboBoxMapSelection_SelectionChanged(object sender, EventArgs e) {
      SetMapBoard(ParseMapName(((ToolStripItem)sender).Text));
    }
    private static MapGridDisplay ParseMapName(string mapName) {
     return Map.MapList.First(item => item.MapName == mapName).MapBoard;
   }

    private void SetMapBoard(MapGridDisplay mapBoard) {
      _hexgridPanel.SetModel( _mapBoard = mapBoard);
      _mapBoard.ShowPathArrow = buttonPathArrow.Checked;
      _mapBoard.ShowFov       = buttonFieldOfView.Checked;
      _mapBoard.FovRadius     =
      _mapBoard.RangeCutoff   = Int32.Parse(txtPathCutover.Tag.ToString(),CultureInfo.InvariantCulture);
      LoadLandmarkMenu();

      CustomCoords.SetMatrices(new IntMatrix2D(2,0, 0,-2, 0,2*_mapBoard.MapSizeHexes.Height-1, 2));
   
      _hexgridPanel.Focus();
   }

    private void buttonFieldOfView_Click(object sender, EventArgs e) {
      _mapBoard.ShowFov = buttonFieldOfView.Checked;
      this._hexgridPanel.Refresh();
    }
    private void buttonPathArrow_Click(object sender, EventArgs e) {
      _mapBoard.ShowPathArrow = buttonPathArrow.Checked;
      this._hexgridPanel.Refresh();
    }
    private void buttonRangeLine_Click(object sender, EventArgs e) {
      _mapBoard.ShowRangeLine = buttonRangeLine.Checked;
      _hexgridPanel.SetMapDirty();
      _mapBoard.StartHex = _mapBoard.StartHex; // Indirect, but it works.
      this._hexgridPanel.Refresh();
    }
    private void buttonTransposeMap_Click(object sender, EventArgs e) {
      _hexgridPanel.IsTransposed = buttonTransposeMap.Checked;
    }

    private void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
      _mapBoard.GoalHex = e.Coords;
      this._hexgridPanel.Refresh();
    }
    private void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
      _mapBoard.StartHex = e.Coords;
      this._hexgridPanel.Refresh();
    }
    private void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
      _mapBoard.HotspotHex = e.Coords;
      this._hexgridPanel.Refresh();
    }
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
    [System.Security.Permissions.PermissionSetAttribute(
      System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public bool PreFilterMessage(ref Message m) {
      if ((WM)m.Msg != WM.MOUSEHWHEEL && (WM)m.Msg != WM.MOUSEWHEEL) return false;

      var hWnd = NativeMethods.WindowFromPoint(WindowsMouseInput.GetPointLParam(m.LParam));
      var ctl = Control.FromChildHandle(hWnd);
      if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
        switch ((WM)m.Msg) {
          case WM.MOUSEHWHEEL:
          case WM.MOUSEWHEEL:
            DebugTracing.Trace(TraceFlags.ScrollEvents, true, " - {0}.WM.{1}: ", Name, ((WM)m.Msg));
            return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
          default: break;
        }
      }
      return false;
    }
    #endregion
  }
}
