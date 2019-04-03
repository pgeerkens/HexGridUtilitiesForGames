#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Linq;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexUtilities.Pathfinding;

using PGNapoleonics.HexgridPanel.WinForms;
using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridPanel.Example {
    using MapGridDisplay = MapDisplay<IHex>;

    public sealed partial class HexgridPanelExample : HexgridPanelForm {
        private bool           _isPanelResizeSuppressed = false;

        public HexgridPanelExample() {
            InitializeComponent();

            MenuBarToolStrip.LoadTraceMenu();
            MenuBarToolStrip.LoadMapList(Map.MapList.Select(item => item.MapName).ToArray());
        }

        protected override CreateParams CreateParams => this.SetCompositedStyle(base.CreateParams);

        #region Event handlers
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

        private void TextPathCutoverChanged(object sender, EventArgs<int> e)
        =>  RefreshAfter(() => {
                MapBoard.FovRadius   =
                MapBoard.RangeCutoff = e.Value;
            }); 

        private void SelectedLandmarkChanged(object sender, EventArgs<int> e)
        => RefreshAfter(() => {
            MapBoard.LandmarkToShow = e.Value;
            HexgridPanel.SetMapDirty();
        });

        private void MapChanged(object sender, EventArgs<string> e)
        =>  SetMapBoard(ParseMapName(e.Value));

        private void ShowFovToggled(object sender, EventArgs<bool> e)
        =>  RefreshAfter(() => MapBoard.ShowFov = e.Value);

        private void ShowPathArrowToggled(object sender, EventArgs<bool> e)
        =>  RefreshAfter(() => MapBoard.ShowPathArrow = e.Value);

        private void ShowRangeLineToggled(object sender, EventArgs<bool> e)
        =>  RefreshAfter(() => {
            MapBoard.ShowRangeLine = e.Value;
            HexgridPanel.SetMapDirty();
            MapBoard.StartHex = MapBoard.StartHex; // Indirect, but it works.
        } );

        private void IsTransposedToggled(object sender, EventArgs<bool> e)
        =>  RefreshAfter(() => {
            HexgridPanel.IsTransposed = e.Value;
        });

        protected override void HexgridPanel_MouseMove(object sender, MouseEventArgs e) {
            var hotHex       = MapBoard.HotspotHex;
            base.HexgridPanel_MouseMove(sender,e);
            StatusBarToolStrip.StatusLabelText = string.Format(CustomCoords,
                    Properties.Resources.StatusLabelText,
                    hotHex, hotHex, hotHex,
                    MapBoard.StartHex - hotHex, MapBoard.Path.Match(path=>path.TotalCost, ()=>0))
                + $"  Elevation: Ground={MapBoard.ElevationGroundASL(hotHex)};"
                + $" Observer={MapBoard.ElevationObserverASL(hotHex)};"
                + $" Target={MapBoard.ElevationTargetASL(hotHex)}"; 
        }

        private void MenuItemHelpContents_Click(object sender, EventArgs e) {
    //      helpProvider1.SetShowHelp(this,true);
        }

        private static MapGridDisplay ParseMapName(string mapName)
        =>  Map.MapList.First(item => item.MapName == mapName).MapBoard;

        private void SetMapBoard(MapGridDisplay mapBoard) {
            HexgridPanel.SetModel( MapBoard = mapBoard);
            MapBoard.ShowPathArrow = MenuBarToolStrip.ShowPathArrow;
            MapBoard.ShowFov       = MenuBarToolStrip.ShowFieldOfView;
            MapBoard.FovRadius     =
            MapBoard.RangeCutoff   = MenuBarToolStrip.PathCutover;
            MenuBarToolStrip.LoadLandmarkMenu(MapBoard.Landmarks);

            CustomCoords = new CustomCoords(new IntMatrix2D(2,0, 0,-2, 0,2*MapBoard.MapSizeHexes.Height-1, 2));
 
            HexgridPanel.Focus();
       }

        private void LandmarksReady(object sender,HexUtilities.Common.EventArgs<ILandmarkCollection> e) {
            if (InvokeRequired) {
                Invoke ((Action) delegate { MenuBarToolStrip.LoadLandmarkMenu(e.Value); });
            } else {
                MenuBarToolStrip.LoadLandmarkMenu(e.Value);
            }
        }
        #endregion
    }
}
