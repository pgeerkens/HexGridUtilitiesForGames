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
    }

    private void Window_Loaded (object sender, RoutedEventArgs e) {
      HexgridPanel = GetHexgridPanel();

      //this.hexgridPanel.ScaleChange += new EventHandler<EventArgs>((o,e) => OnResizeEnd(e));

      //LoadTraceMenu();

//      CustomCoords.SetMatrices(new IntMatrix2D(2,0, 0,-2, 0,2*MapBoard.MapSizeHexes.Height-1, 2));

      comboBoxMapSelection.SelectedIndex = 0;

      _host.Child = HexgridPanel;

      Width  = 1464;
      Height = 1014;

      LoadLandmarkMenu();
      HexgridPanel.SetScroll();
      HexgridPanel.Refresh();
    }

    HexgridPanel           HexgridPanel { get; set; }
    MapDisplay<MapGridHex> MapBoard     { 
      get {return _mapBoard;}
      set {_mapBoard = value; 
        _mapBoard.RangeCutoff = 20; //(int)txtPathCutover.Tag; 
//        LoadLandMarkMenu();
      }
    } MapDisplay<MapGridHex> _mapBoard;

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
    void hexgridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
      var hotHex       = MapBoard.HotspotHex;
      statusLabel.Content = string.Format(CultureInfo.InvariantCulture,
        "Hotspot Hex: {0:gi3} / {1:uI4} / {2:c5}; {3:r6}; Path Length = {4}",
        hotHex, hotHex, hotHex,
        MapBoard.StartHex - hotHex, (MapBoard.Path==null ? 0 : MapBoard.Path.TotalCost));
    }

    void buttonFieldOfView_Click(object sender, RoutedEventArgs e) {
      MapBoard.ShowFov = buttonFieldOfView.IsChecked ?? false;
      HexgridPanel.Refresh();
    }
    void buttonPathArrow_Click(object sender, RoutedEventArgs e) {
      MapBoard.ShowPathArrow = buttonPathArrow.IsChecked ?? false;
      HexgridPanel.Refresh();
    }
    void buttonRangeLine_Click(object sender, RoutedEventArgs e) {
      MapBoard.ShowRangeLine = buttonRangeLine.IsChecked ?? false;
      HexgridPanel.SetMapDirty();
      MapBoard.StartHex = MapBoard.StartHex; // Indirect, but it works.
      HexgridPanel.Refresh();
    }
    void buttonTransposeMap_Click(object sender, RoutedEventArgs e) {
      HexgridPanel.IsTransposed = buttonTransposeMap.IsChecked ?? false;
    }

    void menuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
      _mapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex - 1;
      HexgridPanel.SetMapDirty();
      HexgridPanel.Refresh();
    }

    void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
      MapBoard.GoalHex = e.Coords;
      HexgridPanel.Refresh();
    }
    void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
      MapBoard.StartHex = e.Coords;
      HexgridPanel.Refresh();
    }
    void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
      MapBoard.HotspotHex = e.Coords;
      HexgridPanel.Refresh();
    }
    #endregion

    void comboBoxMapSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      SetMap(((ListBoxItem)e.AddedItems[0]).Content.ToString());
    }

    void SetMap(string mapName) {
      switch (mapName) {
        case "Maze Map":    HexgridPanel.Host = MapBoard = new MazeMap();    break;
        case "Terrain Map": HexgridPanel.Host = MapBoard = new TerrainMap(); break;
        default:  break;
      }
      MapBoard.ShowPathArrow = buttonPathArrow.IsChecked ?? false;
      MapBoard.ShowFov       = buttonFieldOfView.IsChecked ?? false;
      MapBoard.FovRadius     =
      MapBoard.RangeCutoff   = Int32.Parse(txtPathCutover.Tag.ToString());
      MapBoard.MapMargin     = HexgridPanel.MapMargin;
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
        MapBoard.FovRadius   =
        MapBoard.RangeCutoff = value;
        HexgridPanel.Refresh();
      }
    }

    private void _host_MouseWheel(object sender, MouseWheelEventArgs e) {
      MessageBox.Show("In!");
    }
  }
}
