#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using PGNapoleonics.HexgridExampleCommon;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexgridExampleWpf {
    using MapGridDisplay = IMapDisplayWinForms<IHex>;
    using MyMapDisplay   = MapDisplayBlocked<TerrainGridHex>;
    using HexgridPanel   = HexgridPanel.HexgridPanel;

    /// <summary>Interaction logic for MainWindow.xaml.</summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = this; }

        void RefreshCmdExecuted(object target, ExecutedRoutedEventArgs e) { 
            if (e.Parameter != null) HexgridPanel.SetMapDirty();
            HexgridPanel.Refresh();  
        }
        public void RefreshCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            HexgridPanel = (HexgridPanel)_host.Child;
            _host.Child.Focus();

            HexgridPanel.ScaleIndex   = HexgridPanel.Scales
                                                    .Select((f,i) => new {value=f, index=i})
                                                    .Where(s => s.value==1.0F)
                                                    .Select(s => s.index).FirstOrDefault(); 
            HexgridPanel.MouseMove   += HexgridPanel_MouseMove;

            if(sender is IKeyboardInputSink sink) {
                ((IKeyboardInputSink)sender).TabInto(new TraversalRequest(FocusNavigationDirection.First));
            }
            SelectedMapIndex     = 0;
        }

        /// <summary>TODO</summary>
        public   HexgridPanel      HexgridPanel { get; private set; }

        /// <summary>TODO</summary>
        public   IPanelModel       Model        => HexgridPanel.DataContext.Model;

        /// <summary>TODO</summary>
        public   int               SelectedMapIndex  { 
            get => _selectedMapIndex;
            set {
                _selectedMapIndex = value;
                var mapName = ((ListBoxItem)comboBoxMapSelection.Items[_selectedMapIndex]).Name;
                switch (mapName) {
                    case "MazeMap":    HexgridPanel.SetModel(SetMapBoard(HexgridExampleCommon.MazeMap.New(),     Model.FovRadius)); break;
                    case "TerrainMap": HexgridPanel.SetModel(SetMapBoard(HexgridExampleCommon.TerrainMap.New(),  Model.FovRadius)); break;
                    case "A* Bug Map": HexgridPanel.SetModel(SetMapBoard(HexgridExampleCommon.AStarBugMap.New(), Model.FovRadius)); break;
                    default:           break;
                }
                sliderFovRadius.Value = Model.FovRadius;

                HexgridPanel.Refresh();
            }
        } int _selectedMapIndex = 0;

        IPanelModel SetMapBoard(IPanelModel mapBoard, int fovRadius) {
            mapBoard.FovRadius  = fovRadius;
            RefreshLandmarkMenu(mapBoard);

            var customCoords = new CustomCoords(new IntMatrix2D(2,0, 0,-2, 0,2*mapBoard.MapSizeHexes.Height-1, 2));
            return mapBoard;
        }

        /// <summary>TODO</summary>
        public ObservableCollection<ListBoxItem> LandmarkItems { get; }
                = new ObservableCollection<ListBoxItem>()
                        { new ListBoxItem() { Name = "None", Content = "None" } };

        void RefreshLandmarkMenu(IPanelModel model) {
            Model.LandmarkToShow = 0;
            while(LandmarkItems.Count > 1) LandmarkItems.RemoveAt(1);

            if (model.Landmarks != null) {
                foreach(var item in model.Landmarks?.Select((l,i) => new ListBoxItem
                                       { Name = $"No_{i}", Content = $"{l.Coords}" } ) )
                { LandmarkItems.Add(item); }
            }
            HexgridPanel.SetMapDirty();
        }

        void HexgridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            var hotHex = Model.HotspotHex;
            var vector = Model.StartHex - hotHex;
            var cost = Model.Path.ElseDefault()?.TotalCost ?? -1;
            statusLabel.Content =
                $"Hotspot Hex: {hotHex:gi3} / {hotHex:uI4} / {hotHex:c5}; {vector:r6}; Path Length = {cost}";
        }
    }
}