namespace PGNapoleonics.HexgridPanel {
  partial class CachedMapPanel {
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>True if already Disposed.</summary>
    private bool _isDisposed = false;
    /// <summary>Clean up any resources being used.</summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if ( ! _isDisposed) {
        if (disposing) {
          if (components    != null) components.Dispose();
          if (BufferCache   != null) { BufferCache.Dispose();   BufferCache   = null; }
          BufferBack.Dispose();    BufferBack    = null;
          BufferMap.Dispose();     BufferMap     = null;
          BufferShading.Dispose(); BufferShading = null;
          BufferUnits.Dispose();   BufferUnits   = null;
        }
        _isDisposed = true;
        base.Dispose(disposing);
      }
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
    }

    #endregion
  }
}
