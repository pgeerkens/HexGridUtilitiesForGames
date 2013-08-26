using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

using HexGridExampleCommon;

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
      _hexgridPanel = GetHexgridPanel();

      _hexgridPanel.ScaleChange += new EventHandler<EventArgs>((o,ea) => OnResizeEnd(ea));

      comboBoxMapSelection.SelectedIndex = 0;

      _hexgridPanel.SetScrollLimits();
      _host.Child  = _hexgridPanel;

      _host.Child.Focus();

      var sink = sender as System.Windows.Interop.IKeyboardInputSink;
      if (sink != null) 
        ((System.Windows.Interop.IKeyboardInputSink)sender)
            .TabInto(new System.Windows.Input.TraversalRequest(FocusNavigationDirection.First));
    }

    HexgridPanel           _hexgridPanel;
    MapDisplay<MapGridHex> _mapBoard;

    HexgridPanel GetHexgridPanel() {
      var hexgridPanel = new HexgridPanel();

      ((System.ComponentModel.ISupportInitialize)(hexgridPanel)).BeginInit();

      // 
      // hexgridPanel
      // 
      hexgridPanel.AutoScroll = true;
      hexgridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      hexgridPanel.Host = null;
      hexgridPanel.IsTransposed = false;
      hexgridPanel.Location = new System.Drawing.Point(0, 0);
      hexgridPanel.Name = "hexgridPanel";
      hexgridPanel.ScaleIndex = 0;
//      hexgridPanel.Scales = ((System.Collections.ObjectModel.ReadOnlyCollection<float>)(resources.GetObject("hexgridPanel.Scales")));
      hexgridPanel.Size = new System.Drawing.Size(766, 366);
      hexgridPanel.TabIndex = 0;
      hexgridPanel.HotspotHexChange += new System.EventHandler<PGNapoleonics.HexgridPanel.HexEventArgs>(this.PanelBoard_HotSpotHexChange);
      hexgridPanel.MouseCtlClick += new System.EventHandler<PGNapoleonics.HexgridPanel.HexEventArgs>(this.PanelBoard_GoalHexChange);
      hexgridPanel.MouseLeftClick += new System.EventHandler<PGNapoleonics.HexgridPanel.HexEventArgs>(this.PanelBoard_StartHexChange);
      hexgridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(hexgridPanel_MouseMove);

      ((System.ComponentModel.ISupportInitialize)(hexgridPanel)).EndInit();

      var scales = new List<float>() {0.707F,  0.841F, 1.000F, 1.189F, 1.414F}.AsReadOnly();
      hexgridPanel.Scales     = scales;
      hexgridPanel.ScaleIndex = scales.Select((f,i) => new {value=f, index=i})
                                      .Where(s => s.value==1.0F)
                                      .Select(s => s.index).FirstOrDefault(); 

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
          txtPathCutover.Tag = value;
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
