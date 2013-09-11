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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

using HexgridExampleCommon;

namespace HexgridExampleWinForms {
  internal sealed partial class HexgridExampleForm : Form {
    public HexgridExampleForm() {
      InitializeComponent();

      this._hexgridPanel.ScaleChange += new EventHandler<EventArgs>((o,e) => OnResizeEnd(e));

      LoadTraceMenu();

      comboBoxMapSelection.SelectedIndex = 1;

//      helpProvider1.SetShowHelp(this,true);
    }
    protected override CreateParams CreateParams { 
			get { return this.SetCompositedStyle(base.CreateParams); }
		}

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    void LoadTraceMenu() {
      #if TRACE
        foreach(var item in Enum.GetValues(typeof(TraceFlags))) {
          var menuItem = new System.Windows.Forms.ToolStripMenuItem();
          menuItemDebug.DropDownItems.Add(menuItem);
          menuItem.Name         = "menuItemDebugTracing" + item.ToString();
          menuItem.Size         = new System.Drawing.Size(143, 22);
          menuItem.Text         = item.ToString();
          menuItem.CheckOnClick = true;
          menuItem.Click       += new System.EventHandler(this.menuItemDebugTracing_Click);
        }
      #endif
    }

    void LoadLandmarkMenu() {
      menuItemLandmarks.Items.Clear();
      menuItemLandmarks.Items.Add("None");
      foreach(var landmark in _mapBoard.Landmarks) {
        menuItemLandmarks.Items.Add(string.Format(CultureInfo.InvariantCulture, "{0}", landmark.Coords));
      }
      menuItemLandmarks.SelectedIndexChanged += new EventHandler(menuItemLandmarks_SelectedIndexChanged);
      menuItemLandmarks.SelectedIndex = 0; 
    }

    MapDisplay<MapGridHex>          _mapBoard { get; set; }

    #region Event handlers
    void HexGridExampleForm_Load(object sender, EventArgs e) {
      _hexgridPanel.Scales = new List<float>() {0.707F,  0.841F, 1.000F, 1.189F, 1.414F}.AsReadOnly();
      _hexgridPanel.ScaleIndex = _hexgridPanel.Scales
                              .Select((f,i) => new {value=f, index=i})
                              .Where(s => s.value==1.0F)
                              .Select(s => s.index).FirstOrDefault(); 
      Size = _hexgridPanel.MapSizePixels + new Size(21,93);
    }

    bool isPanelResizeSuppressed = false;
    protected override void OnResizeBegin(EventArgs e) {
      base.OnResizeBegin(e);
      isPanelResizeSuppressed = true;
    }
    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      if (IsHandleCreated && ! isPanelResizeSuppressed) _hexgridPanel.SetScrollLimits();
    }
    protected override void OnResizeEnd(EventArgs e) {
      base.OnResizeEnd(e);
      isPanelResizeSuppressed = false;
      _hexgridPanel.SetScrollLimits();
    }

    void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
      var hotHex       = _mapBoard.HotspotHex;
      statusLabel.Text = string.Format(CultureInfo.InvariantCulture,
        // "Hotspot Hex: {0:gi3} / {1:uI4} / {2:c5}; {3:r6}; Path Length = {4}",
        PGNapoleonics.HexGridExample2.Properties.Resources.StatusLabelText,
        hotHex, hotHex, hotHex,
        _mapBoard.StartHex - hotHex, (_mapBoard.Path==null ? 0 : _mapBoard.Path.TotalCost));
    }

    void txtPathCutover_TextChanged(object sender, EventArgs e) {
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

    void menuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
      _mapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex - 1;
      _hexgridPanel.SetMapDirty();
      Update();
    }

    void menuItemDebugTracing_Click(object sender, EventArgs e) {
      var item = (ToolStripMenuItem)sender;
      item.CheckState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
      var name = item.Name.Replace("menuItemDebugTracing","");
      var flag = (TraceFlags)Enum.Parse(typeof(TraceFlags),name);
      if( item.Checked)
        DebugTracing.EnabledFags |=  flag;
      else
        DebugTracing.EnabledFags &= ~flag;
    }

    void menuItemHelpContents_Click(object sender, EventArgs e) {
//      helpProvider1.SetShowHelp(this,true);
    }

   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", 
     "CA2000:Dispose objects before losing scope")]
   void comboBoxMapSelection_SelectionChanged(object sender, EventArgs e) {
      var mapName = ((ToolStripItem)sender).Text;
      MapDisplay<MapGridHex> tempMapBoard = null;
      try {
        switch (mapName) {
          case "Maze Map":    tempMapBoard = new MazeMap();    break;
          case "Terrain Map": tempMapBoard = new TerrainMap(); break;
          default:            throw new ArgumentException("'" + mapName + "' is invalid value.","sender");
        }
        SetMapBoard(tempMapBoard);
        tempMapBoard = null;
      } finally { if (tempMapBoard!=null) tempMapBoard.Dispose(); }
    }

    void SetMapBoard(MapDisplay<MapGridHex> mapBoard) {
      _hexgridPanel.Host      = 
      _mapBoard               = mapBoard;
      _mapBoard.ShowPathArrow = buttonPathArrow.Checked;
      _mapBoard.ShowFov       = buttonFieldOfView.Checked;
      _mapBoard.FovRadius     =
      _mapBoard.RangeCutoff   = Int32.Parse(txtPathCutover.Tag.ToString(),CultureInfo.InvariantCulture);
      _mapBoard.MapMargin     = _hexgridPanel.MapMargin;
      LoadLandmarkMenu();

      CustomCoords.SetMatrices(new IntMatrix2D(2,0, 0,-2, 0,2*_mapBoard.MapSizeHexes.Height-1, 2));
    }

    void buttonFieldOfView_Click(object sender, EventArgs e) {
      _mapBoard.ShowFov = buttonFieldOfView.Checked;
      this._hexgridPanel.Refresh();
    }
    void buttonPathArrow_Click(object sender, EventArgs e) {
      _mapBoard.ShowPathArrow = buttonPathArrow.Checked;
      this._hexgridPanel.Refresh();
    }
    void buttonRangeLine_Click(object sender, EventArgs e) {
      _mapBoard.ShowRangeLine = buttonRangeLine.Checked;
      _hexgridPanel.SetMapDirty();
      _mapBoard.StartHex = _mapBoard.StartHex; // Indirect, but it works.
      this._hexgridPanel.Refresh();
    }
    void buttonTransposeMap_Click(object sender, EventArgs e) {
      _hexgridPanel.IsTransposed = buttonTransposeMap.Checked;
    }

    void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
      _mapBoard.GoalHex = e.Coords;
      this._hexgridPanel.Refresh();
    }
    void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
      _mapBoard.StartHex = e.Coords;
      this._hexgridPanel.Refresh();
    }
    void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
      _mapBoard.HotspotHex = e.Coords;
      this._hexgridPanel.Refresh();
    }
    #endregion

    public event EventHandler<MouseEventArgs> MouseHWheel;
    void FireMouseHWheel(IntPtr wParam, IntPtr lParam) {
      var wheelDelta = WindowsMouseInput.WheelDelta(wParam);
      var buttons    = WindowsMouseInput.GetKeyStateWParam(wParam);
      var point      = WindowsMouseInput.GetPointLParam(lParam);

      MouseHWheel.Raise(this, 
        new MouseEventArgs(MouseButtons.None,wheelDelta,point.X,point.Y,wheelDelta));
    }

    protected override void WndProc(ref Message m) {
      base.WndProc(ref m);
//      if (m.HWnd != this.Handle) return;
      switch (m.Msg) {
        case (int)WM.MOUSEHWHEEL: FireMouseHWheel(m.WParam, m.LParam);
                                  m.Result = (IntPtr)1;
                                  break;
        default:                  break;
      }
    }
  }
}
