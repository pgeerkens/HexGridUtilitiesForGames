#region License - Copyright (C) 2012-2019 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
namespace PGNapoleonics.HexgridPanel.Example {
    partial class HexgridBufferedPanelForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCode("","")]
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexgridBufferedPanelForm));

            this.StatusBarToolStrip = new PGNapoleonics.HexgridPanel.Example.StatusBarToolStrip();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonTransposeMap = new System.Windows.Forms.ToolStripButton();
            this.buttonRangeLine = new System.Windows.Forms.ToolStripButton();
            this.buttonFieldOfView = new System.Windows.Forms.ToolStripButton();
            this.buttonPathArrow = new System.Windows.Forms.ToolStripButton();
            this.comboBoxMapSelection = new System.Windows.Forms.ToolStripComboBox();
            this.lblPathCutover = new System.Windows.Forms.ToolStripLabel();
            this.txtPathCutover = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lblLandmark = new System.Windows.Forms.ToolStripLabel();
            this.menuItemLandmarks = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemDebug = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuItemHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();

            this.SuspendLayout();

            // 
            // ToolStripContainer.TopToolStripPanel
            // 
            this.ToolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // ToolStripContainer.BottomToolStripPanel
            // 
            this.ToolStripContainer.BottomToolStripPanel.Controls.Add(this.StatusBarToolStrip.ToolStrip);
            // 
            // StatusBarToolStrip
            // 
            this.StatusBarToolStrip.Location = new System.Drawing.Point(0, 0);
            this.StatusBarToolStrip.Name = "StatusBarToolStrip";
            this.StatusBarToolStrip.Size = new System.Drawing.Size(609, 25);
            this.StatusBarToolStrip.StatusLabelText = "toolStripLabel2";
            this.StatusBarToolStrip.TabIndex = 0;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.buttonTransposeMap,
                this.buttonRangeLine,
                this.buttonFieldOfView,
                this.buttonPathArrow,
                this.comboBoxMapSelection,
                this.lblPathCutover,
                this.txtPathCutover,
                this.toolStripSeparator2,
                this.lblLandmark,
                this.menuItemLandmarks,
                this.toolStripSeparator1,
                this.menuItemDebug,
                this.toolStripSeparator3,
                this.menuItemHelp});
            this.toolStrip2.Location = new System.Drawing.Point(3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(767, 25);
            this.toolStrip2.TabIndex = 0;
            // 
            // buttonTransposeMap
            // 
            this.buttonTransposeMap.AutoSize = false;
            this.buttonTransposeMap.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonTransposeMap.CheckOnClick = true;
            this.buttonTransposeMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonTransposeMap.Image = ((System.Drawing.Image)(resources.GetObject("buttonTransposeMap.Image")));
            this.buttonTransposeMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTransposeMap.Name = "buttonTransposeMap";
            this.buttonTransposeMap.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.buttonTransposeMap.Size = new System.Drawing.Size(75, 22);
            this.buttonTransposeMap.Text = "Transpose";
            this.buttonTransposeMap.ToolTipText = "Toggles transposition of the current map.";
            this.buttonTransposeMap.Click += this.ButtonTransposeMap_Click;
            // 
            // buttonRangeLine
            // 
            this.buttonRangeLine.AutoSize = false;
            this.buttonRangeLine.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonRangeLine.CheckOnClick = true;
            this.buttonRangeLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonRangeLine.Image = ((System.Drawing.Image)(resources.GetObject("buttonRangeLine.Image")));
            this.buttonRangeLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRangeLine.Name = "buttonRangeLine";
            this.buttonRangeLine.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.buttonRangeLine.Size = new System.Drawing.Size(75, 22);
            this.buttonRangeLine.Text = "Range Line";
            this.buttonRangeLine.ToolTipText =
                "Toggles (a) display of Range Line; and (b) Field-of-View source between Start-Hex and Hotspot-Hex.";
            this.buttonRangeLine.CheckedChanged += this.ButtonRangeLine_Click;
            // 
            // buttonFieldOfView
            // 
            this.buttonFieldOfView.AutoSize = false;
            this.buttonFieldOfView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonFieldOfView.Checked = true;
            this.buttonFieldOfView.CheckOnClick = true;
            this.buttonFieldOfView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonFieldOfView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonFieldOfView.Image = ((System.Drawing.Image)(resources.GetObject("buttonFieldOfView.Image")));
            this.buttonFieldOfView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonFieldOfView.Name = "buttonFieldOfView";
            this.buttonFieldOfView.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.buttonFieldOfView.Size = new System.Drawing.Size(75, 22);
            this.buttonFieldOfView.Text = "FOV";
            this.buttonFieldOfView.ToolTipText = "Toggles display of current Field-of-View";
            this.buttonFieldOfView.Click += this.ButtonFieldOfView_Click;
            // 
            // buttonPathArrow
            // 
            this.buttonPathArrow.AutoSize = false;
            this.buttonPathArrow.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonPathArrow.Checked = true;
            this.buttonPathArrow.CheckOnClick = true;
            this.buttonPathArrow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonPathArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonPathArrow.Image = ((System.Drawing.Image)(resources.GetObject("buttonPathArrow.Image")));
            this.buttonPathArrow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPathArrow.Name = "buttonPathArrow";
            this.buttonPathArrow.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.buttonPathArrow.Size = new System.Drawing.Size(75, 22);
            this.buttonPathArrow.Text = "Path Arrow";
            this.buttonPathArrow.ToolTipText = "Toggle display of Path Arrow.";
            this.buttonPathArrow.CheckedChanged += this.ButtonPathArrow_Click;
            // 
            // comboBoxMapSelection
            // 
            this.comboBoxMapSelection.AutoSize = false;
            this.comboBoxMapSelection.CausesValidation = false;
            this.comboBoxMapSelection.Name = "comboBoxMapSelection";
            this.comboBoxMapSelection.Size = new System.Drawing.Size(90, 23);
            this.comboBoxMapSelection.Text = "Map:";
            this.comboBoxMapSelection.ToolTipText = "Selects map to display.";
            this.comboBoxMapSelection.SelectedIndexChanged += this.ComboBoxMapSelection_SelectionChanged;
            // 
            // lblPathCutover
            // 
            this.lblPathCutover.Name = "lblPathCutover";
            this.lblPathCutover.Size = new System.Drawing.Size(79, 22);
            this.lblPathCutover.Text = "Path Cutover:";
            this.lblPathCutover.ToolTipText = resources.GetString("lblPathCutover.ToolTipText");
            // 
            // txtPathCutover
            // 
            this.txtPathCutover.Name = "txtPathCutover";
            this.txtPathCutover.Size = new System.Drawing.Size(40, 25);
            this.txtPathCutover.Tag = 20;
            this.txtPathCutover.Text = "20";
            this.txtPathCutover.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPathCutover.ToolTipText = resources.GetString("txtPathCutover.ToolTipText");
            this.txtPathCutover.TextChanged += this.TxtPathCutover_TextChanged;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // lblLandmark
            // 
            this.lblLandmark.Name = "lblLandmark";
            this.lblLandmark.Size = new System.Drawing.Size(63, 22);
            this.lblLandmark.Text = "Landmark:";
            this.lblLandmark.ToolTipText = "Landmark from which shortest-paths distances are to be displayed.";
            // 
            // menuItemLandmarks
            // 
            this.menuItemLandmarks.AutoSize = false;
            this.menuItemLandmarks.Items.AddRange(new object[] {
                "None"});
            this.menuItemLandmarks.Name = "menuItemLandmarks";
            this.menuItemLandmarks.Size = new System.Drawing.Size(50, 23);
            this.menuItemLandmarks.ToolTipText = "Landmark from which shortest-paths distances are to be displayed.";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // menuItemDebug
            // 
            this.menuItemDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuItemDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemDebug.Name = "menuItemDebug";
            this.menuItemDebug.Size = new System.Drawing.Size(87, 22);
            this.menuItemDebug.Text = "&Debug Trace";
            this.menuItemDebug.ToolTipText = "Build with DEBUG to enable this feature.";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuItemHelpContents,
                this.toolStripSeparator4,
                this.menuItemHelpAbout});
            this.menuItemHelp.Image = ((System.Drawing.Image)(resources.GetObject("menuItemHelp.Image")));
            this.menuItemHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemHelp.Name = "menuItemHelp";
            this.menuItemHelp.ShowDropDownArrow = false;
            this.menuItemHelp.Size = new System.Drawing.Size(36, 19);
            this.menuItemHelp.Text = "&Help";
            this.menuItemHelp.Visible = false;
            // 
            // menuItemHelpContents
            // 
            this.menuItemHelpContents.Name = "menuItemHelpContents";
            this.menuItemHelpContents.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.menuItemHelpContents.Size = new System.Drawing.Size(190, 22);
            this.menuItemHelpContents.Text = "&Contents";
            this.menuItemHelpContents.Click += this.MenuItemHelpContents_Click;
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(187, 6);
            // 
            // menuItemHelpAbout
            // 
            this.menuItemHelpAbout.Name = "menuItemHelpAbout";
            this.menuItemHelpAbout.Size = new System.Drawing.Size(190, 22);
            this.menuItemHelpAbout.Text = "&ABout HexgridUtilities";
            // 
            // HexgridBufferedPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 420);
            this.Name = "HexgridBufferedPanelForm";
            this.Text = "HexgridScrollableExample (Buffered-Winforms)";
            this.ResumeLayout(false);

        }

        #endregion

        private PGNapoleonics.HexgridPanel.Example.StatusBarToolStrip StatusBarToolStrip;

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonTransposeMap;
        private System.Windows.Forms.ToolStripComboBox comboBoxMapSelection;
        private System.Windows.Forms.ToolStripButton buttonFieldOfView;
        private System.Windows.Forms.ToolStripLabel lblPathCutover;
        private System.Windows.Forms.ToolStripTextBox txtPathCutover;
        private System.Windows.Forms.ToolStripDropDownButton menuItemDebug;
        private System.Windows.Forms.ToolStripComboBox menuItemLandmarks;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel lblLandmark;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonPathArrow;
        private System.Windows.Forms.ToolStripButton buttonRangeLine;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton menuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelpContents;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelpAbout;
    }
}