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
      this.toolStrip2 = new System.Windows.Forms.ToolStrip();
      this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
      this.hexgridPanel = new PG_Napoleonics.Utilities.HexUtilities.HexgridPanel(this.components);
      this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
      this.toolStripContainer1.ContentPanel.SuspendLayout();
      this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
      this.toolStripContainer1.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      this.toolStrip2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hexgridPanel)).BeginInit();
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
      this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(458, 370);
      this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
      this.toolStripContainer1.Name = "toolStripContainer1";
      this.toolStripContainer1.Size = new System.Drawing.Size(458, 420);
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
      this.toolStrip1.Size = new System.Drawing.Size(388, 25);
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
      this.statusLabel.Size = new System.Drawing.Size(300, 22);
      this.statusLabel.Text = "toolStripLabel2";
      this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // toolStrip2
      // 
      this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
      this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
      this.toolStrip2.Location = new System.Drawing.Point(3, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new System.Drawing.Size(35, 25);
      this.toolStrip2.TabIndex = 0;
      // 
      // toolStripButton1
      // 
      this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
      this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      // 
      // hexgridPanel
      // 
      this.hexgridPanel.AutoScroll = true;
      this.hexgridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.hexgridPanel.Host = null;
      this.hexgridPanel.Location = new System.Drawing.Point(0, 0);
      this.hexgridPanel.MapMargin = new System.Drawing.Size(5, 5);
      this.hexgridPanel.Name = "hexgridPanel";
      this.hexgridPanel.ScaleIndex = 1;
      this.hexgridPanel.Scales = new float[] {
        0.707F,
        1F,
        1.414F};
      this.hexgridPanel.Size = new System.Drawing.Size(454, 366);
      this.hexgridPanel.TabIndex = 0;
      this.hexgridPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.hexgridPanel_MouseClick);
      this.hexgridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.hexgridPanel_MouseMove);
      // 
      // HexGridExampleForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(458, 420);
      this.Controls.Add(this.toolStripContainer1);
      this.Name = "HexGridExampleForm";
      this.Text = "Hex Grid Example";
      this.Load += new System.EventHandler(this.HexGridExampleForm_Load);
      this.ResizeEnd += new System.EventHandler(this.HexGridExampleForm_ResizeEnd);
      this.Resize += new System.EventHandler(this.HexGridExampleForm_Resize);
      this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
      this.toolStripContainer1.ContentPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.PerformLayout();
      this.toolStripContainer1.ResumeLayout(false);
      this.toolStripContainer1.PerformLayout();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.hexgridPanel)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    private PG_Napoleonics.Utilities.HexUtilities.HexgridPanel hexgridPanel;
    private System.Windows.Forms.ToolStrip toolStrip2;
    private System.Windows.Forms.ToolStripButton toolStripButton1;
    private System.Windows.Forms.ToolStripLabel statusLabel;
  }
}