using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace PGNapoleonics.HexgridScrollableExample {
    /// <summary>TODO</summary>
    public partial class MdiParent1 : Form, ISupportInitialize {
        private int childFormNumber = 0;

        static CultureInfo Culture = CultureInfo.CurrentCulture;
        static ResourceManager StringManager =
                new ResourceManager("en-US", Assembly.GetExecutingAssembly());
        private static string FileExtensionMask = "";// Properties.Resources.FileExtensionMask;

        /// <summary>TODO</summary>
        public MdiParent1() {
            InitializeComponent();
            CreateDefaultChildren();
        }

        /// <summary>Signals the object that initialization is starting.</summary>
        public virtual void BeginInit() { ; }
        /// <summary>Signals the object that initialization is complete.</summary>
        public virtual void EndInit() { 
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
        }

        [System.CodeDom.Compiler.GeneratedCode("","")]
        private void CreateDefaultChildren() {
            ShowDefaultChildren (new ExampleHexgridPanel());
            ShowDefaultChildren (new ExampleBufferedHexgridScrollable());
            ShowDefaultChildren (new ExampleHexgridScrollable());
        }

        [System.CodeDom.Compiler.GeneratedCode("","")]
        private void ShowDefaultChildren(Form child) {
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

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void CutToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) =>
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) =>
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e) =>
            LayoutMdi(MdiLayout.Cascade);

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e) =>
            LayoutMdi(MdiLayout.TileVertical);

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e) =>
            LayoutMdi(MdiLayout.TileHorizontal);

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e) =>
            LayoutMdi(MdiLayout.ArrangeIcons);

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (Form childForm in MdiChildren) {
                childForm.Close();
            }
        }

        private void MdiParent_FormClosing(object sender, FormClosingEventArgs e) {
            foreach (var child in this.MdiChildren) { child.Hide(); if (child!=null) child.Close(); }
        }
    }
}
