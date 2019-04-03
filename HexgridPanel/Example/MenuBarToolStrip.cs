#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridPanel.Example {
    using HexSize        = System.Drawing.Size;

    public partial class MenuBarToolStrip: UserControl {
        public MenuBarToolStrip() => InitializeComponent();

        public event EventHandler<EventArgs<int>>    TextPathCutoverChanged;
        public event EventHandler<EventArgs<int>>    SelectedLandmarkChanged;
        public event EventHandler<EventArgs<string>> MapChanged;
        public event EventHandler<EventArgs<bool>>   ShowFovToggled;
        public event EventHandler<EventArgs<bool>>   ShowPathArrowToggled;
        public event EventHandler<EventArgs<bool>>   ShowRangeLineToggled;
        public event EventHandler<EventArgs<bool>>   IsTransposedToggled;

        public ToolStrip ToolStrip       => toolStrip;
        public int       PathCutover     => int.Parse(txtPathCutover.Tag.ToString(),CultureInfo.InvariantCulture);
        public bool      ShowPathArrow   => buttonPathArrow.Checked;
        public bool      ShowFieldOfView => buttonFieldOfView.Checked;

        public Tracing EnabledTraces { get; private set; } = Tracing.None;

        public void LoadMapList(object[] mapNames) {
            comboBoxMapSelection.Items.AddRange(mapNames);
            comboBoxMapSelection.SelectedIndex = 0;
        }

        public void LoadLandmarkMenu(ILandmarkCollection landmarks) {
            menuItemLandmarks.Items.Clear();
            menuItemLandmarks.Items.Add("None");
            landmarks?.ForEach(landmark => menuItemLandmarks.Items.Add($"{landmark.Coords}") );

            menuItemLandmarks.SelectedIndexChanged += new EventHandler(MenuItemLandmarks_SelectedIndexChanged);
            menuItemLandmarks.SelectedIndex = 0; 
        }

        [Conditional("TRACE")]
        public void LoadTraceMenu()
        =>  Tracing.ForEachKey(item => LoadTraceMenuItem(item,MenuItemDebugTracing_Click), n => n!="None");

        [Conditional("TRACE")]
        void LoadTraceMenuItem(string item, EventHandler handler) {
            var menuItem = new ToolStripMenuItem() {
                Name         = "menuItemDebugTracing" + item.ToString(),
                Size         = new HexSize(143, 22),
                Text         = item.ToString(),
                CheckOnClick = true
            };
            menuItem.Click += handler;
            menuItemDebug.DropDownItems.Add(menuItem);
        }

        private void TxtPathCutover_TextChanged(object sender, EventArgs e) {
            if(int.TryParse(txtPathCutover.Text, out var value)) {
                txtPathCutover.Tag = value;
            } else {
                txtPathCutover.Text = txtPathCutover.Tag.ToString();
                value = (int)txtPathCutover.Tag;
            }

            TextPathCutoverChanged?.Invoke(sender, new EventArgs<int>(value));
        }

        private void MenuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e)
        =>  SelectedLandmarkChanged?.Invoke(sender, new EventArgs<int>(menuItemLandmarks.SelectedIndex));

        private void MenuItemDebugTracing_Click(object sender, EventArgs e) {
            var item = (ToolStripMenuItem)sender;
            item.CheckState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
            var name = item.Name.Replace("menuItemDebugTracing","");
            var flag = Tracing.Item(name);
            if( item.Checked)   EnabledTraces |=  flag;
            else                EnabledTraces &= ~flag;
        }

        private void MenuItemHelpContents_Click(object sender, EventArgs e) {
    //      helpProvider1.SetShowHelp(this,true);
        }

        private void ComboBoxMapSelection_SelectionChanged(object sender, EventArgs e)
        =>  MapChanged?.Invoke(sender, new EventArgs<string>(((ToolStripItem)sender).Text));
        
        private void ButtonFieldOfView_Click(object sender, EventArgs e)
        => ShowFovToggled?.Invoke(sender, new EventArgs<bool>(buttonFieldOfView.Checked));   

        private void ButtonPathArrow_Click(object sender, EventArgs e)
        => ShowPathArrowToggled?.Invoke(sender, new EventArgs<bool>(buttonPathArrow.Checked));

        private void ButtonRangeLine_Click(object sender, EventArgs e)
        => ShowRangeLineToggled?.Invoke(sender, new EventArgs<bool>(buttonRangeLine.Checked));
        
        private void ButtonTransposeMap_Click(object sender, EventArgs e)
        => IsTransposedToggled?.Invoke(sender, new EventArgs<bool>(buttonTransposeMap.Checked));
    }
}
