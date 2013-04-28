#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
namespace PG_Napoleonics.HexGridExample {
  partial class HexGridExampleForm {
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
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexGridExampleForm));
      this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
      this.statusLabel = new System.Windows.Forms.ToolStripLabel();
      this.hexgridPanel = new PG_Napoleonics.Utilities.HexUtilities.HexgridPanel(this.components);
      this.toolStrip2 = new System.Windows.Forms.ToolStrip();
      this.buttonTransposeMap = new System.Windows.Forms.ToolStripButton();
      this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
      this.buttonFieldOfView = new System.Windows.Forms.ToolStripButton();
      this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
      this.toolStripContainer1.ContentPanel.SuspendLayout();
      this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
      this.toolStripContainer1.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hexgridPanel)).BeginInit();
      this.toolStrip2.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStripContainer1
      // 
      // 
      // toolStripContainer1.BottomToolStripPanel
      // 
      this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.toolStrip1);
      // 
      // toolStripContainer1.ContentPanel
      // 
      this.toolStripContainer1.ContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.toolStripContainer1.ContentPanel.Controls.Add(this.hexgridPanel);
      this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(558, 370);
      this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
      this.toolStripContainer1.Name = "toolStripContainer1";
      this.toolStripContainer1.Size = new System.Drawing.Size(558, 420);
      this.toolStripContainer1.TabIndex = 0;
      this.toolStripContainer1.Text = "toolStripContainer1";
      // 
      // toolStripContainer1.TopToolStripPanel
      // 
      this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip2);
      // 
      // toolStrip1
      // 
      this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.statusLabel});
      this.toolStrip1.Location = new System.Drawing.Point(3, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(513, 25);
      this.toolStrip1.TabIndex = 0;
      // 
      // toolStripLabel1
      // 
      this.toolStripLabel1.Name = "toolStripLabel1";
      this.toolStripLabel1.Size = new System.Drawing.Size(45, 22);
      this.toolStripLabel1.Text = "Status: ";
      // 
      // statusLabel
      // 
      this.statusLabel.AutoSize = false;
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(425, 22);
      this.statusLabel.Text = "toolStripLabel2";
      this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // hexgridPanel
      // 
      this.hexgridPanel.AutoScroll = true;
      this.hexgridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.hexgridPanel.Host = null;
      this.hexgridPanel.IsTransposed = false;
      this.hexgridPanel.Location = new System.Drawing.Point(0, 0);
      this.hexgridPanel.Name = "hexgridPanel";
      this.hexgridPanel.ScaleIndex = 1;
      this.hexgridPanel.Scales = new float[] {
        0.707F,
        1F,
        1.414F};
      this.hexgridPanel.Size = new System.Drawing.Size(554, 366);
      this.hexgridPanel.TabIndex = 0;
      this.hexgridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.hexgridPanel_MouseMove);
      this.hexgridPanel.MouseCtlClick += PanelBoard_GoalHexChange;
      this.hexgridPanel.HotSpotHexChange += PanelBoard_HotSpotHexChange;
      this.hexgridPanel.MouseLeftClick += PanelBoard_StartHexChange;
      // 
      // toolStrip2
      // 
      this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
      this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonTransposeMap,
            this.toolStripComboBox1,
            this.buttonFieldOfView});
      this.toolStrip2.Location = new System.Drawing.Point(3, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new System.Drawing.Size(233, 25);
      this.toolStrip2.TabIndex = 0;
      // 
      // buttonTransposeMap
      // 
      this.buttonTransposeMap.CheckOnClick = true;
      this.buttonTransposeMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.buttonTransposeMap.Image = ((System.Drawing.Image)(resources.GetObject("buttonTransposeMap.Image")));
      this.buttonTransposeMap.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.buttonTransposeMap.Name = "buttonTransposeMap";
      this.buttonTransposeMap.Size = new System.Drawing.Size(65, 22);
      this.buttonTransposeMap.Text = "Transpose";
      this.buttonTransposeMap.ToolTipText = "Transpose Map";
      this.buttonTransposeMap.Click += new System.EventHandler(this.buttonTransposeMap_Click);
      // 
      // toolStripComboBox1
      // 
      this.toolStripComboBox1.AutoSize = false;
      this.toolStripComboBox1.CausesValidation = false;
      this.toolStripComboBox1.Items.AddRange(new object[] {
            "MazeMap",
            "TerrainMap"});
      this.toolStripComboBox1.Name = "toolStripComboBox1";
      this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
      this.toolStripComboBox1.Sorted = true;
      this.toolStripComboBox1.Text = "Map:";
      this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_Click);
      // 
      // buttonFieldOfView
      // 
      this.buttonFieldOfView.CheckOnClick = true;
      this.buttonFieldOfView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.buttonFieldOfView.Image = ((System.Drawing.Image)(resources.GetObject("buttonFieldOfView.Image")));
      this.buttonFieldOfView.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.buttonFieldOfView.Name = "buttonFieldOfView";
      this.buttonFieldOfView.Size = new System.Drawing.Size(33, 22);
      this.buttonFieldOfView.Text = "FOV";
      this.buttonFieldOfView.Click += new System.EventHandler(this.buttonFieldOfView_Click);
      // 
      // HexGridExampleForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(558, 420);
      this.Controls.Add(this.toolStripContainer1);
      this.Name = "HexGridExampleForm";
      this.Text = "Hex Grid Example";
      this.Load += new System.EventHandler(this.HexGridExampleForm_Load);
      this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
      this.toolStripContainer1.ContentPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.PerformLayout();
      this.toolStripContainer1.ResumeLayout(false);
      this.toolStripContainer1.PerformLayout();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hexgridPanel)).EndInit();
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    private PG_Napoleonics.Utilities.HexUtilities.HexgridPanel hexgridPanel;
    private System.Windows.Forms.ToolStrip toolStrip2;
    private System.Windows.Forms.ToolStripButton buttonTransposeMap;
    private System.Windows.Forms.ToolStripLabel statusLabel;
    private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
    private System.Windows.Forms.ToolStripButton buttonFieldOfView;
  }
}