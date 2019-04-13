#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Linq;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.Storage;

using PGNapoleonics.HexgridPanel.WinForms;
using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridPanel.Example {
    using MapGridDisplay = MapDisplay<IHex>;

    /// <summary>An example <see cref="Form"/> extending <see cref="HexgridPanelForm"/>.</summary>
    public sealed partial class HexgridPanelExample : HexgridPanelForm {
        private bool           _isPanelResizeSuppressed = false;

        public HexgridPanelExample() {
            InitializeComponent();

            MenuBarToolStrip.LoadTraceMenu();
            MenuBarToolStrip.LoadMapList(MapList.Maps.Select(item => item.MapName).ToArray());
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

        private static IPanelModel ParseMapName(string mapName)
        =>  MapList.Maps.First(item => item.MapName == mapName).MapBoard;

        private void SetMapBoard(IPanelModel mapBoard) {
            HexgridPanel.SetModel( MapBoard = mapBoard);
            MapBoard.ShowPathArrow = MenuBarToolStrip.ShowPathArrow;
            MapBoard.ShowFov       = MenuBarToolStrip.ShowFieldOfView;
            MapBoard.FovRadius     =
            MapBoard.RangeCutoff   = MenuBarToolStrip.PathCutover;
            MenuBarToolStrip.LoadLandmarkMenu(MapBoard.Landmarks);

            CustomCoords = new CustomCoords(new IntMatrix2D(2,0, 0,-2, 0,2*MapBoard.MapSizeHexes.Height-1, 2));
 
            HexgridPanel.Focus();
       }

        private void LandmarksReady(object sender,EventArgs<ILandmarkCollection> e) {
            if (InvokeRequired) {
                Invoke ((Action) delegate { MenuBarToolStrip.LoadLandmarkMenu(e.Value); });
            } else {
                MenuBarToolStrip.LoadLandmarkMenu(e.Value);
            }
        }
        #endregion
    }
}
