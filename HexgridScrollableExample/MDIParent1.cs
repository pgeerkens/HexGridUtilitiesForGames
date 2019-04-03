#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using PGNapoleonics.HexgridPanel.Example;

namespace PGNapoleonics.HexgridExampleWinforms {
    /// <summary>TODO</summary>
    public partial class MdiParent : Form {
        private int childFormNumber = 0;

        private static string FileExtensionMask = Properties.Resources.FileExtensionMask;

        /// <summary>TODO</summary>
        public MdiParent() {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
            CreateDefaultChildren();
        }

        [System.CodeDom.Compiler.GeneratedCode("","")]
        private void CreateDefaultChildren() {
            ShowChild(new HexgridBufferedPanelExample());
            ShowChild(new HexgridPanelExample());
        }

        [System.CodeDom.Compiler.GeneratedCode("","")]
        private void ShowChild(Form child) {
            child.MdiParent = this;
            child.WindowState = FormWindowState.Maximized;
            components.Add(child);
            child.Show();
        }

        [System.CodeDom.Compiler.GeneratedCode("","")]
        private void ShowNewForm(object sender, EventArgs e) {
            var child = new Form();
            child.MdiParent = this;
            child.Text = "Window " + childFormNumber++;
            components.Add(child);
            child.Show();
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "FileName")]
        private void OpenFile(object sender, EventArgs e) {
            using(var openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                openFileDialog.Filter = FileExtensionMask;
                if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                    string FileName = openFileDialog.FileName;
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "FileName")]
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            using(var saveFileDialog = new SaveFileDialog()) {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = FileExtensionMask;
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
                    string FileName = saveFileDialog.FileName;
                }
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void CutToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        =>  toolStrip.Visible = toolBarToolStripMenuItem.Checked;

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        =>  statusStrip.Visible = statusBarToolStripMenuItem.Checked;

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        =>  LayoutMdi(MdiLayout.Cascade);

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        =>  LayoutMdi(MdiLayout.TileVertical);

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        =>  LayoutMdi(MdiLayout.TileHorizontal);

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        =>  LayoutMdi(MdiLayout.ArrangeIcons);

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (var childForm in MdiChildren) { childForm.Close(); }
        }

        private void MdiParent_FormClosing(object sender, FormClosingEventArgs e) {
            foreach (var child in MdiChildren) { child.Hide(); child?.Close(); }
        }
    }
}
