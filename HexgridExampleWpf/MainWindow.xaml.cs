#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridExampleWpf {
    using MapGridDisplay = IMapDisplayWinForms<IHex>;
    using MyMapDisplay   = MapDisplayBlocked<IHex>;
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
            HexgridPanel.MouseMove   += hexgridPanel_MouseMove;

            if(sender is IKeyboardInputSink sink) {
                ((IKeyboardInputSink)sender).TabInto(new TraversalRequest(FocusNavigationDirection.First));
            }
            SelectedMapIndex     = 0;
        }

        /// <summary>TODO</summary>
        public   HexgridPanel HexgridPanel { get; private set; }

        /// <summary>TODO</summary>
        public   MapGridDisplay    Model        => HexgridPanel.DataContext.Model;

        /// <summary>TODO</summary>
        public   int               SelectedMapIndex  { 
            get => _selectedMapIndex;
            set {
                _selectedMapIndex = value;
                var mapName = ((ListBoxItem)comboBoxMapSelection.Items[_selectedMapIndex]).Name;
                switch (mapName) {
                    case "MazeMap":    HexgridPanel.SetModel(SetMapBoard(new MazeMap(),     Model.FovRadius)); break;
                    case "TerrainMap": HexgridPanel.SetModel(SetMapBoard(new TerrainMap(),  Model.FovRadius)); break;
                    case "A* Bug Map": HexgridPanel.SetModel(SetMapBoard(new AStarBugMap(), Model.FovRadius)); break;
                    default:           break;
                }
                sliderFovRadius.Value = Model.FovRadius;

                HexgridPanel.Refresh();
            }
        } int _selectedMapIndex = 0;

        MyMapDisplay SetMapBoard(MyMapDisplay mapBoard, int fovRadius) {
            mapBoard.FovRadius  = fovRadius;
            RefreshLandmarkMenu(mapBoard);

            var customCoords = new CustomCoords(new IntMatrix2D(2,0, 0,-2, 0,2*mapBoard.MapSizeHexes.Height-1, 2));
            return mapBoard;
        }

        /// <summary>TODO</summary>
        public ObservableCollection<ListBoxItem> LandmarkItems { get; }
                = new ObservableCollection<ListBoxItem>()
                        { new ListBoxItem() { Name = "None", Content = "None" } };

        void RefreshLandmarkMenu(MyMapDisplay model) {
            Model.LandmarkToShow = 0;
            while(LandmarkItems.Count > 1) LandmarkItems.RemoveAt(1);

            if (model.Landmarks != null) {
                foreach(var item in model.Landmarks?.Select((l,i) => new ListBoxItem
                                       { Name = $"No_{i}", Content = $"{l.Coords}" } ) )
                { LandmarkItems.Add(item); }
            }
            HexgridPanel.SetMapDirty();
        }

        void hexgridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            var hotHex = Model.HotspotHex;
            var vector = Model.StartHex - hotHex;
            var cost = Model.Path.ElseDefault()?.TotalCost ?? -1;
            statusLabel.Content =
                $"Hotspot Hex: {hotHex:gi3} / {hotHex:uI4} / {hotHex:c5}; {vector:r6}; Path Length = {cost}";
        }
    }
}