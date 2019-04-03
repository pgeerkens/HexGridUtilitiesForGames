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
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.WinForms;

using PGNapoleonics.HexgridExampleCommon;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridPanel.Example {
    using HexSize        = System.Drawing.Size;
    using MapGridDisplay = MapDisplay<IHex>;

    public sealed partial class HexgridBufferedPanelForm : HexgridPanelForm {
        private bool           _isPanelResizeSuppressed = false;

        public HexgridBufferedPanelForm() {
            InitializeComponent();

            //var resources = new ComponentResourceManager(typeof(HexgridScrollableExample));

            LoadTraceMenu(MenuItemDebugTracing_Click);

            comboBoxMapSelection.Items.AddRange(Map.MapList.Select(item => item.MapName).ToArray());
            comboBoxMapSelection.SelectedIndex = 0;

//          helpProvider1.SetShowHelp(this,true);
        }
        protected override CreateParams CreateParams => this.SetCompositedStyle(base.CreateParams);

        [Conditional("TRACE")]
        void LoadTraceMenu(EventHandler handler) =>
            Tracing.ForEachKey(item => LoadTraceMenuItem(item,handler), n => n!="None");
        
        [Conditional("TRACE")]
        void LoadTraceMenuItem(string item, EventHandler handler) {
            var menuItem = new ToolStripMenuItem();
            menuItemDebug.DropDownItems.Add(menuItem);
            menuItem.Name         = "menuItemDebugTracing" + item.ToString();
            menuItem.Size         = new HexSize(143, 22);
            menuItem.Text         = item.ToString();
            menuItem.CheckOnClick = true;
            menuItem.Click       += handler;
        }

        private void LoadLandmarkMenu(ILandmarkCollection landmarks) {
            menuItemLandmarks.Items.Clear();
            menuItemLandmarks.Items.Add("None");
            landmarks?.ForEach(landmark => menuItemLandmarks.Items.Add($"{landmark.Coords}") );

            menuItemLandmarks.SelectedIndexChanged += new EventHandler(MenuItemLandmarks_SelectedIndexChanged);
            menuItemLandmarks.SelectedIndex = 0; 
        }

        #region Event handlers
        private void HexGridExampleForm_Load(object sender, EventArgs e) {
            HexgridPanel.ScaleIndex = HexgridPanel.Scales
                                                    .Select((f,i) => new {value=f, index=i})
                                                    .Where(s => s.value==1.0F)
                                                    .Select(s => s.index).FirstOrDefault(); 
            var padding = ToolStripContainer.ContentPanel.Padding;
            Size = HexgridPanel.MapSizePixels  + new HexSize(21,93)
                 + new HexSize(padding.Left+padding.Right, padding.Top+padding.Bottom);
        }

        protected override void OnResizeBegin(EventArgs e) {
            base.OnResizeBegin(e);
            _isPanelResizeSuppressed = true;
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (IsHandleCreated && ! _isPanelResizeSuppressed) HexgridPanel.SetScrollLimits(MapBoard);
        }
        protected override void OnResizeEnd(EventArgs e) {
            base.OnResizeEnd(e);
            _isPanelResizeSuppressed = false;
            HexgridPanel.SetScrollLimits(MapBoard);
        }

        protected override void HexgridPanel_MouseMove(object sender, MouseEventArgs e) {
            var hotHex       = MapBoard.HotspotHex;
            StatusBarToolStrip.StatusLabelText = string.Format(CustomCoords,
                    Properties.Resources.StatusLabelText,
                    hotHex, hotHex, hotHex,
                    MapBoard.StartHex - hotHex, MapBoard.Path.Match(path=>path.TotalCost, ()=>0))
                + $"  Elevation: Ground={MapBoard.ElevationGroundASL(hotHex)};"
                + $" Observer={MapBoard.ElevationObserverASL(hotHex)};"
                + $" Target={MapBoard.ElevationTargetASL(hotHex)}"; 
        }

        private void TxtPathCutover_TextChanged(object sender, EventArgs e) {
            if ( int.TryParse( txtPathCutover.Text, out var value ) ) {
                txtPathCutover.Tag = value;
            } else {
                txtPathCutover.Text = txtPathCutover.Tag.ToString();
                value = (int)txtPathCutover.Tag;
            }
            MapBoard.FovRadius   =
            MapBoard.RangeCutoff = value;
            Refresh();
        }

        private void MenuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
            MapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex;
            HexgridPanel.SetMapDirty();
            Update();
        }

        private void MenuItemDebugTracing_Click(object sender, EventArgs e) {
            var item = (ToolStripMenuItem)sender;
            item.CheckState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
            var name = item.Name.Replace("menuItemDebugTracing","");
            var flag = Tracing.Item(name);
            if (item.Checked)   Tracing.EnabledTraces |=  flag;
            else                Tracing.EnabledTraces &= ~flag;
        }

        private void MenuItemHelpContents_Click(object sender, EventArgs e) {
    //      helpProvider1.SetShowHelp(this,true);
        }

        private void ComboBoxMapSelection_SelectionChanged(object sender, EventArgs e)
        =>  SetMapBoard(ParseMapName(((ToolStripItem)sender).Text));
        
        private static MapGridDisplay ParseMapName(string mapName) 
        =>  Map.MapList.First(item => item.MapName == mapName).MapBoard;

        private void SetMapBoard(MapGridDisplay mapBoard) {
            HexgridPanel.SetModel( MapBoard = mapBoard);
            MapBoard.ShowPathArrow = buttonPathArrow.Checked;
            MapBoard.ShowFov       = buttonFieldOfView.Checked;
            MapBoard.FovRadius     =
            MapBoard.RangeCutoff   = int.Parse(txtPathCutover.Tag.ToString(),CultureInfo.InvariantCulture);
            LoadLandmarkMenu(MapBoard.Landmarks);

            CustomCoords = new CustomCoords(new IntMatrix2D(2,0, 0,-2, 0,2*MapBoard.MapSizeHexes.Height-1, 2));
   
            HexgridPanel.Focus();
       }

        private void ButtonFieldOfView_Click(object sender, EventArgs e) {
            MapBoard.ShowFov = buttonFieldOfView.Checked;
            HexgridPanel.Refresh();
        }

        private void ButtonPathArrow_Click(object sender, EventArgs e) {
            MapBoard.ShowPathArrow = buttonPathArrow.Checked;
            HexgridPanel.Refresh();
        }

        private void ButtonRangeLine_Click(object sender, EventArgs e) {
            MapBoard.ShowRangeLine = buttonRangeLine.Checked;
            HexgridPanel.SetMapDirty();
            MapBoard.StartHex = MapBoard.StartHex; // Indirect, but it works.
            HexgridPanel.Refresh();
        }

        private void ButtonTransposeMap_Click(object sender, EventArgs e) =>
            HexgridPanel.IsTransposed = buttonTransposeMap.Checked;

        private void LandmarksReady(object sender, ValueEventArgs<ILandmarkCollection> e) {
            if (InvokeRequired) {
                Invoke ((Action) delegate { LoadLandmarkMenu(e.Value); });
            } else {
                LoadLandmarkMenu(e.Value);
            }
        }
        #endregion
    }
}
