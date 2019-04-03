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
            this.MenuBarToolStrip = new PGNapoleonics.HexgridPanel.Example.MenuBarToolStrip();

            this.SuspendLayout();

            // 
            // ToolStripContainer.TopToolStripPanel
            // 
            this.ToolStripContainer.TopToolStripPanel.Controls.Add(this.MenuBarToolStrip.ToolStrip);
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
            // MenuBarToolStrip
            // 
            this.StatusBarToolStrip.Location = new System.Drawing.Point(3, 0);
            this.StatusBarToolStrip.Name = "StatusBarToolStrip";
            this.StatusBarToolStrip.Size = new System.Drawing.Size(744, 25);
            this.StatusBarToolStrip.StatusLabelText = "toolStripLabel2";
            this.StatusBarToolStrip.TabIndex = 0;

            this.MenuBarToolStrip.TextPathCutoverChanged += TextPathCutoverChanged;
            this.MenuBarToolStrip.SelectedLandmarkChanged += SelectedLandmarkChanged;
            this.MenuBarToolStrip.MapChanged += MapChanged;
            this.MenuBarToolStrip.ShowFovToggled += ShowFovToggled;
            this.MenuBarToolStrip.ShowPathArrowToggled += ShowPathArrowToggled;
            this.MenuBarToolStrip.ShowRangeLineToggled += ShowRangeLineToggled;
            this.MenuBarToolStrip.IsTransposedToggled += IsTransposedToggled;
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
        private PGNapoleonics.HexgridPanel.Example.MenuBarToolStrip MenuBarToolStrip;
    }
}