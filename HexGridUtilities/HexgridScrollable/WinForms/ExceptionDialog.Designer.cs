namespace PGNapoleonics.WinForms {
  partial class ExceptionDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
      this.ErrorText = new System.Windows.Forms.TextBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // ErrorText
      // 
      resources.ApplyResources(this.ErrorText, "ErrorText");
      this.ErrorText.Name = "ErrorText";
      // 
      // OkButton
      // 
      resources.ApplyResources(this.OkButton, "OkButton");
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.OkButton.Name = "OkButton";
      this.OkButton.UseVisualStyleBackColor = true;
      // 
      // ExceptionDialog
      // 
      this.AcceptButton = this.OkButton;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.OkButton;
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.ErrorText);
      this.Name = "ExceptionDialog";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.TopMost = true;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox ErrorText;
    private System.Windows.Forms.Button OkButton;
  }
}