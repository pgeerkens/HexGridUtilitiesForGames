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
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.HexgridPanel = new PGNapoleonics.HexgridPanel.HexgridBufferedPanel();

            this.ToolStripContainer.ContentPanel.SuspendLayout();
            this.ToolStripContainer.TopToolStripPanel.SuspendLayout();
            this.ToolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.ToolStripContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HexgridPanel)).BeginInit();
            this.SuspendLayout();

            // 
            // ToolStripContainer
            // 
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ToolStripContainer.ContentPanel.Controls.Add(this.HexgridPanel);
            this.ToolStripContainer.ContentPanel.Padding = new System.Windows.Forms.Padding(5);
            this.ToolStripContainer.ContentPanel.Size = new System.Drawing.Size(770, 370);
            this.ToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.ToolStripContainer.Margin = new System.Windows.Forms.Padding(0);
            this.ToolStripContainer.Name = "ToolStripContainer";
            this.ToolStripContainer.Size = new System.Drawing.Size(770, 420);
            this.ToolStripContainer.TabIndex = 0;
            this.ToolStripContainer.Text = "ToolStripContainer";

            // 
            // HexgridPanel
            // 
            this.HexgridPanel.AutoScroll = true;
            this.HexgridPanel.AutoScrollMinSize = new System.Drawing.Size(34, 45);
            this.HexgridPanel.AutoSize = true;
            this.HexgridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HexgridPanel.IsTransposed = false;
            this.HexgridPanel.Location = new System.Drawing.Point(5, 5);
            this.HexgridPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HexgridPanel.Name = "HexgridPanel";
            this.HexgridPanel.ScaleIndex = 0;
            this.HexgridPanel.Size = new System.Drawing.Size(756, 356);
            this.HexgridPanel.TabIndex = 0;
            this.HexgridPanel.TabStop = true;
            this.HexgridPanel.UnappliedScroll = new System.Drawing.Point(0, 0);
            this.HexgridPanel.HotspotHexChange += new System.EventHandler<PGNapoleonics.HexgridPanel.HexEventArgs>(this.PanelBoard_HotSpotHexChange);
            this.HexgridPanel.MouseCtlClick += new System.EventHandler<PGNapoleonics.HexgridPanel.HexEventArgs>(this.PanelBoard_GoalHexChange);
            this.HexgridPanel.MouseLeftClick += new System.EventHandler<PGNapoleonics.HexgridPanel.HexEventArgs>(this.PanelBoard_StartHexChange);
            this.HexgridPanel.ScaleChange += new System.EventHandler<System.EventArgs>(this.HexgridPanel_ScaleChange);
            this.HexgridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HexgridPanel_MouseMove);

            // 
            // HexgridPanelForm
            // 
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800,450);
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "HexgridPanelExampleForm";
            this.Text = "HexgridPanelForm";

            this.Load += new System.EventHandler(this.HexgridPanelForm_Load);
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.ContentPanel.PerformLayout();
            this.ToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.BottomToolStripPanel.PerformLayout();
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HexgridPanel)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
        protected System.Windows.Forms.ToolStripContainer ToolStripContainer;
        protected PGNapoleonics.HexgridPanel.HexgridBufferedPanel HexgridPanel;
    }
}