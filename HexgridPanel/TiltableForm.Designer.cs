namespace PGNapoleonics.HexgridPanel {
    partial class TiltableForm {
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
            this.ToolStripContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStripContainer
            // 
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ToolStripContainer.ContentPanel.Padding = new System.Windows.Forms.Padding(5);
            this.ToolStripContainer.ContentPanel.Size = new System.Drawing.Size(800, 425);
            this.ToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.ToolStripContainer.Margin = new System.Windows.Forms.Padding(0);
            this.ToolStripContainer.Name = "ToolStripContainer";
            this.ToolStripContainer.Size = new System.Drawing.Size(800, 450);
            this.ToolStripContainer.TabIndex = 0;
            this.ToolStripContainer.Text = "ToolStripContainer";
            // 
            // TiltableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "TiltableForm";
            this.Text = "TiltableForm";
            this.Load += new System.EventHandler(this.HexgridPanelForm_Load);
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.ToolStripContainer ToolStripContainer;

        protected virtual HexgridPanel HexgridPanel { get; } = null;
    }
}