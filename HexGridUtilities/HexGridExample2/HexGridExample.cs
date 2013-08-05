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

namespace PGNapoleonics.HexGridExample2 {
  internal sealed partial class HexgridExampleForm : Form, IMessageFilter {
    public HexgridExampleForm() {
      InitializeComponent();
			Application.AddMessageFilter(this);

      this.hexgridPanel.ScaleChange += new EventHandler<EventArgs>((o,e) => OnResizeEnd(e));

      LoadTraceMenu();

      toolStripComboBox1.SelectedIndex = 1;
      CustomCoords.SetMatrices(new IntMatrix2D(2,0, 0,-2, 0,2*MapBoard.MapSizeHexes.Height-1, 2));

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

    void LoadLandMarkMenu() {
      menuItemLandmarks.Items.Clear();
      menuItemLandmarks.Items.Add("None");
      foreach(var landmark in _mapBoard.Landmarks) {
        menuItemLandmarks.Items.Add(string.Format(CultureInfo.InvariantCulture, "{0}", landmark.Coords));
      }
      menuItemLandmarks.SelectedIndexChanged += menuItemLandmarks_SelectedIndexChanged;
      menuItemLandmarks.SelectedIndex = 0; 
    }

    MapDisplay<MapGridHex>          MapBoard { 
      get {return _mapBoard;}
      set {_mapBoard = value; _mapBoard.RangeCutoff = (int)txtPathCutover.Tag; LoadLandMarkMenu();}
    } MapDisplay<MapGridHex> _mapBoard;

    #region Event handlers
    void HexGridExampleForm_Load(object sender, EventArgs e) {
      //hexgridPanel.SetScaleList(new List<float>() {0.707F,  0.841F, 1.000F, 1.189F, 1.414F}.AsReadOnly());
      hexgridPanel.Scales = new List<float>() {0.707F,  0.841F, 1.000F, 1.189F, 1.414F}.AsReadOnly();
      hexgridPanel.ScaleIndex = hexgridPanel.Scales
                              .Select((f,i) => new {value=f, index=i})
                              .Where(s => s.value==1.0F)
                              .Select(s => s.index).FirstOrDefault(); 
      Size = hexgridPanel.MapSizePixels + new Size(21,93);
    }

    bool isPanelResizeSuppressed = false;
    protected override void OnResizeBegin(EventArgs e) {
      base.OnResizeBegin(e);
      isPanelResizeSuppressed = true;
    }
    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      if (IsHandleCreated && ! isPanelResizeSuppressed) hexgridPanel.SetScroll();
    }
    protected override void OnResizeEnd(EventArgs e) {
      base.OnResizeEnd(e);
      isPanelResizeSuppressed = false;
      hexgridPanel.SetScroll();
    }

    void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
      var hotHex       = MapBoard.HotspotHex;
      statusLabel.Text = string.Format(CultureInfo.InvariantCulture,
        "Hotspot Hex: {0:gi3} / {1:uI4} / {2:c5}; {3:r6}; Path Length = {4}",
        hotHex, hotHex, hotHex,
        MapBoard.StartHex - hotHex, (MapBoard.Path==null ? 0 : MapBoard.Path.TotalCost));
    }

    void buttonTransposeMap_Click(object sender, EventArgs e) {
      hexgridPanel.IsTransposed = buttonTransposeMap.Checked;
    }

    private void txtPathCutover_TextChanged(object sender, EventArgs e) {
      int value;
      if (Int32.TryParse(txtPathCutover.Text, out value)) {
        txtPathCutover.Tag = value;
      } else {
        txtPathCutover.Text = txtPathCutover.Tag.ToString();
        value = (int)txtPathCutover.Tag;
      }
      MapBoard.FovRadius   =
      MapBoard.RangeCutoff = value;
      Refresh();
    }

    private void menuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
      _mapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex - 1;
      hexgridPanel.SetMapDirty(); // .Host = this.MapBoard;
      Update();
    }

    private void menuItemDebugTracing_Click(object sender, EventArgs e) {
      var item = (ToolStripMenuItem)sender;
      item.CheckState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
      var name = item.Name.Replace("menuItemDebugTracing","");
      var flag = (TraceFlags)Enum.Parse(typeof(TraceFlags),name);
      if( item.Checked)
        DebugTracing.EnabledFags |=  flag;
      else
        DebugTracing.EnabledFags &= ~flag;
    }

    private void menuItemHelpContents_Click(object sender, EventArgs e) {
//      helpProvider1.SetShowHelp(this,true);
    }

    private void toolStripComboBox1_Click(object sender, EventArgs e) {
      var name = ((ToolStripItem)sender).Text;
      switch (name) {
        case "MazeMap":    hexgridPanel.Host = MapBoard = new MazeMap();    break;
        case "TerrainMap": hexgridPanel.Host = MapBoard = new TerrainMap(); break;
        default:  break;
      }
      MapBoard.ShowPathArrow = buttonPathArrow.Checked;
      MapBoard.ShowFov       = buttonFieldOfView.Checked;
      MapBoard.FovRadius     =
      MapBoard.RangeCutoff   = (int)txtPathCutover.Tag;
      MapBoard.MapMargin     = hexgridPanel.MapMargin;
      hexgridPanel.Refresh();
    }

    private void buttonFieldOfView_Click(object sender, EventArgs e) {
      MapBoard.ShowFov = buttonFieldOfView.Checked;
      Refresh();
    }
    private void buttonPathArrow_Click(object sender, EventArgs e) {
      MapBoard.ShowPathArrow = buttonPathArrow.Checked;
      Refresh();
    }
    private void buttonRangeLine_Click(object sender, EventArgs e) {
      MapBoard.ShowRangeLine = buttonRangeLine.Checked;
      Refresh();
    }

    private void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
      MapBoard.GoalHex = e.Coords;
      Refresh();
    }
    private void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
      MapBoard.StartHex = e.Coords;
      Refresh();
    }
    private void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
      MapBoard.HotspotHex = e.Coords;
//      Refresh();
      this.hexgridPanel.Refresh();
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
			System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			var hWnd  = NativeMethods.WindowFromPoint( WindowsMouseInput.GetPointLParam(m.LParam) );
			var ctl	  = Control.FromHandle(hWnd);
      if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
        switch((WM)m.Msg) {
          default:  break;
          case WM.MOUSEWHEEL:
            #if DEBUG
              DebugTracing.Trace(TraceFlags.ScrollEvents, true," - {0}.WM.{1}: ", Name, 
                ((WM)m.Msg)); 
            #endif
            if (ctl is HexgridPanel.HexgridPanel  ||  ctl is HexgridExampleForm) {
              return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
            }
            break;
        }
      }
      return false;
		}
    #endregion
  }
}
