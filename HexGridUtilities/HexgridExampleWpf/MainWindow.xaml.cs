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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

using HexgridExampleCommon;

namespace HexgridExampleWpf {
  /// <summary></summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      this.AddHandler(MainWindow.MouseWheelEvent, new RoutedEventHandler(Window_MouseWheel), true);
      this.AddHandler(DockPanel.MouseWheelEvent, new RoutedEventHandler(Window_MouseWheel), true);
      this.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler(Window_MouseWheel), true);
      this.AddHandler(WindowsFormsHost.MouseWheelEvent, new RoutedEventHandler(Window_MouseWheel), true);
    }

    private void Window_Loaded (object sender, RoutedEventArgs e) {
      _hexgridPanel = GetHexgridPanel(
        new List<float> {0.707F,  0.841F, 1.000F, 1.189F, 1.414F}
        );

      _hexgridPanel.ScaleChange += (o,ea) => OnResizeEnd(ea);

      comboBoxMapSelection.SelectedIndex = 0;

      _hexgridPanel.SetScrollLimits();
      _host.Child  = _hexgridPanel;

      _host.Child.Focus();

      var sink = sender as System.Windows.Interop.IKeyboardInputSink;
      if (sink != null) 
        ((System.Windows.Interop.IKeyboardInputSink)sender).TabInto
              (new System.Windows.Input.TraversalRequest(FocusNavigationDirection.First));
    }

    HexgridPanel           _hexgridPanel;
    MapDisplay<MapGridHex> _mapBoard;

    HexgridPanel GetHexgridPanel() {
      return GetHexgridPanel(new List<float> {0.841F, 1.000F, 1.189F});
    }
    HexgridPanel GetHexgridPanel(IList<float> scales) {
      var hexgridPanel = new HexgridPanel();

      hexgridPanel.BeginInit();

      // 
      // hexgridPanel
      // 
      hexgridPanel.AutoScroll   = true;
      hexgridPanel.Dock         = System.Windows.Forms.DockStyle.Fill;
      hexgridPanel.Host         = null;
      hexgridPanel.IsTransposed = false;
      hexgridPanel.Location     = new System.Drawing.Point(0, 0);
      hexgridPanel.Name         = "hexgridPanel";
      hexgridPanel.Scales       = scales.ToList().AsReadOnly();
      hexgridPanel.ScaleIndex   = scales.Select((f,i) => new {value=f, index=i})
                                        .Where(s => s.value==1.0F)
                                        .Select(s => s.index).FirstOrDefault(); 
      hexgridPanel.Size         = new System.Drawing.Size(766, 366);
      hexgridPanel.TabIndex     = 0;
      hexgridPanel.HotspotHexChange += this.PanelBoard_HotSpotHexChange;
      hexgridPanel.MouseCtlClick    += this.PanelBoard_GoalHexChange;
      hexgridPanel.MouseLeftClick   += this.PanelBoard_StartHexChange;
      hexgridPanel.MouseMove        += this.hexgridPanel_MouseMove;

      hexgridPanel.EndInit();

      return hexgridPanel;
    }

    void LoadLandmarkMenu() {
      menuItemLandmarks.Items.Clear();
      menuItemLandmarks.Items.Add("None");
      foreach(var landmark in _mapBoard.Landmarks) {
        menuItemLandmarks.Items.Add(string.Format(CultureInfo.InvariantCulture, "{0}", landmark.Coords));
      }
      menuItemLandmarks.SelectionChanged += new SelectionChangedEventHandler(menuItemLandmarks_SelectedIndexChanged);
      menuItemLandmarks.SelectedIndex = 0; 
    }

    #region Event handlers
    bool isPanelResizeSuppressed = false;
    protected void OnResizeBegin(EventArgs e) {
//      base.OnResizeBegin(e);
      isPanelResizeSuppressed = true;
    }
    protected void OnResize(EventArgs e) {
//      base.OnResize(e);
      if (IsInitialized && ! isPanelResizeSuppressed) _hexgridPanel.SetScrollLimits();
    }
    protected void OnResizeEnd(EventArgs e) {
//      base.OnResizeEnd(e);
      isPanelResizeSuppressed = false;
      _hexgridPanel.SetScrollLimits();
    }

    void hexgridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
      var hotHex       = _mapBoard.HotspotHex;
      statusLabel.Content = string.Format(CultureInfo.InvariantCulture,
        "Hotspot Hex: {0:gi3} / {1:uI4} / {2:c5}; {3:r6}; Path Length = {4}",
        hotHex, hotHex, hotHex,
        _mapBoard.StartHex - hotHex, (_mapBoard.Path==null ? 0 : _mapBoard.Path.TotalCost));
    }

    void buttonFieldOfView_Click(object sender, RoutedEventArgs e) {
      _mapBoard.ShowFov = buttonFieldOfView.IsChecked ?? false;
      _hexgridPanel.Refresh();
    }
    void buttonPathArrow_Click(object sender, RoutedEventArgs e) {
      _mapBoard.ShowPathArrow = buttonPathArrow.IsChecked ?? false;
      _hexgridPanel.Refresh();
    }
    void buttonRangeLine_Click(object sender, RoutedEventArgs e) {
      _mapBoard.ShowRangeLine = buttonRangeLine.IsChecked ?? false;
      _hexgridPanel.SetMapDirty();
      _mapBoard.StartHex = _mapBoard.StartHex; // Indirect, but it works.
      _hexgridPanel.Refresh();
    }
    void buttonTransposeMap_Click(object sender, RoutedEventArgs e) {
      _hexgridPanel.IsTransposed = buttonTransposeMap.IsChecked ?? false;
    }

    void menuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
      _mapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex - 1;
      _hexgridPanel.SetMapDirty();
      _hexgridPanel.Refresh();
    }

    void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
      _mapBoard.GoalHex = e.Coords;
      _hexgridPanel.Refresh();
    }
    void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
      _mapBoard.StartHex = e.Coords;
      _hexgridPanel.Refresh();
    }
    void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
      _mapBoard.HotspotHex = e.Coords;
      _hexgridPanel.Refresh();
    }
    #endregion

    void comboBoxMapSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      var mapName = ((ListBoxItem)e.AddedItems[0]).Content.ToString();
      switch (mapName) {
        case "Maze Map":    SetMapBoard(new MazeMap());    break;
        case "Terrain Map": SetMapBoard(new TerrainMap()); break;
        default:            throw new ArgumentException(mapName,"mapName");
      }
    }

    void SetMapBoard(MapDisplay<MapGridHex> mapBoard) {
      _hexgridPanel.Host      = _mapBoard = mapBoard;
      _mapBoard.ShowPathArrow = buttonPathArrow.IsChecked ?? false;
      _mapBoard.ShowFov       = buttonFieldOfView.IsChecked ?? false;
      _mapBoard.FovRadius     =
      _mapBoard.RangeCutoff   = Int32.Parse(txtPathCutover.Tag.ToString());
      _mapBoard.MapMargin     = _hexgridPanel.MapMargin;
      LoadLandmarkMenu();

      CustomCoords.SetMatrices(new IntMatrix2D(2,0, 0,-2, 0,2*_mapBoard.MapSizeHexes.Height-1, 2));
    }

    void txtPathCutover_TextChanged(object sender, TextChangedEventArgs e) {
      if (this.IsInitialized) {
        int value;
        if (Int32.TryParse(txtPathCutover.Text, out value)) {
          txtPathCutover.Tag  = value;
        } else {
          txtPathCutover.Text = txtPathCutover.Tag.ToString();
          value = (int)txtPathCutover.Tag;
        }
        _mapBoard.FovRadius   =
        _mapBoard.RangeCutoff = value;
        _hexgridPanel.Refresh();
      }
    }

    private void _host_MouseWheel(object sender, MouseWheelEventArgs e) {
      MessageBox.Show("In - _host_MouseWheel!");
    }

    private void _scrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
      MessageBox.Show("In - _scrollViewer_MouseWheel!");
    }

    private void _dockPanel_MouseWheel(object sender, MouseWheelEventArgs e) {
      MessageBox.Show("In - _dockPanel_MouseWheel!");
    }

    private void Window_MouseWheel(object sender, MouseWheelEventArgs e) {
      MessageBox.Show("In - Window_MouseWheel!");
    }

    private void Window_MouseWheel(object sender, RoutedEventArgs e) {
      MessageBox.Show("In - Window_MouseWheel!");
    }
  }
}
