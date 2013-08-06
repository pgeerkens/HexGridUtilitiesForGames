using System;
using System.Collections.Generic;
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

using HexGridExampleCommon;

namespace HexgridExampleWpf {
  /// <summary></summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
    }

    private void Window_Loaded (object sender, RoutedEventArgs e) {
      HexgridPanel = GetHexgridPanel();

      toolStripComboBox1_Click(this,EventArgs.Empty);

      _host.Child = HexgridPanel;

      HexgridPanel.SetScroll();
      HexgridPanel.Refresh();

    }

    private void toolStripComboBox1_Click(object sender, EventArgs e) {
      var name = "TerrainMap"; //((ToolStripItem)sender).Text;
      switch (name) {
        case "MazeMap":    HexgridPanel.Host = MapBoard = new MazeMap();    break;
        case "TerrainMap": HexgridPanel.Host = MapBoard = new TerrainMap(); break;
        default:  break;
      }
      MapBoard.ShowPathArrow = true; //buttonPathArrow.Checked;
      MapBoard.ShowFov       = true; //buttonFieldOfView.Checked;
      MapBoard.FovRadius     =
      MapBoard.RangeCutoff   = 20; //(int)txtPathCutover.Tag;
      MapBoard.MapMargin     = HexgridPanel.MapMargin;
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

  
    #region Event handlers
    void hexgridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
      var hotHex       = MapBoard.HotspotHex;
      //statusLabel.Text = string.Format(CultureInfo.InvariantCulture,
      //  "Hotspot Hex: {0:gi3} / {1:uI4} / {2:c5}; {3:r6}; Path Length = {4}",
      //  hotHex, hotHex, hotHex,
      //  MapBoard.StartHex - hotHex, (MapBoard.Path==null ? 0 : MapBoard.Path.TotalCost));
    }

    private void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
      MapBoard.GoalHex = e.Coords;
      HexgridPanel.Refresh();
    }
    private void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
      MapBoard.StartHex = e.Coords;
      HexgridPanel.Refresh();
    }
    private void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
      MapBoard.HotspotHex = e.Coords;
      HexgridPanel.Refresh();
    }
    #endregion
  }
}
