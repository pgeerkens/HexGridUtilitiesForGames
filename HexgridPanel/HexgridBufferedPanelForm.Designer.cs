namespace PGNapoleonics.HexgridPanel {
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
        private void InitializeComponent() {
            this.Panel = new PGNapoleonics.HexgridPanel.HexgridBufferedPanel();
            this.ToolStripContainer.ContentPanel.SuspendLayout();
            this.ToolStripContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Panel)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolStripContainer
            // 
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.Controls.Add(this.Panel);
            // 
            // Panel
            // 
            this.Panel.AutoScroll = true;
            this.Panel.AutoScrollMinSize = new System.Drawing.Size(34, 45);
            this.Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel.IsMapDirty = false;
            this.Panel.IsTransposed = false;
            this.Panel.IsUnitsDirty = false;
            this.Panel.Location = new System.Drawing.Point(5, 5);
            this.Panel.MapOrientation = PGNapoleonics.HexgridPanel.MapOrientation.ZeroDegrees;
            this.Panel.Margin = new System.Windows.Forms.Padding(0);
            this.Panel.Name = "Panel";
            this.Panel.ScaleIndex = 0;
            this.Panel.Size = new System.Drawing.Size(786, 436);
            this.Panel.TabIndex = 1;
            this.Panel.UnappliedScroll = new System.Drawing.Point(0, 0);

            this.Panel.HotspotHexChange += this.PanelBoard_HotSpotHexChange;
            this.Panel.MouseCtlClick += this.PanelBoard_GoalHexChange;
            this.Panel.MouseLeftClick += this.PanelBoard_StartHexChange;
            this.Panel.ScaleChange += this.HexgridPanel_ScaleChange;
            this.Panel.MouseMove += this.HexgridPanel_MouseMove;
            // 
            // HexgridBufferedPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "HexgridBufferedPanelForm";
            this.Text = "HexgridBufferedPanelForm";
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Panel)).EndInit();
            this.ResumeLayout(false);

        }

        private HexgridBufferedPanel Panel;

        #endregion
    }
}