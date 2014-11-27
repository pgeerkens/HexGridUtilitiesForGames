#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;

using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
  using PaintAction = Action<Graphics>;

    /// <summary>TODO</summary>
  public partial class LayeredScrollable : ScrollableControl, ISupportInitialize { //, IMessageFilter {
    /// <summary>TODO</summary>
    public LayeredScrollable() {
      InitializeComponent();
    }

    #region ISupportInitialize implementation
    /// <summary>Signals the object that initialization is starting.</summary>
    public virtual void BeginInit() { 
    }
    /// <summary>Signals the object that initialization is complete.</summary>
    public virtual void EndInit() { 
//			Application.AddMessageFilter(this);

      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.Opaque, true);

      Layers = new LayerCollection(this.CreateGraphics(), DisplayRectangle.Size);
    }
    #endregion

    /// <summary>TODO</summary>
    public    LayerCollection   Layers    { get; private set; }

    /// <inheritdoc/>
    protected override void OnResize(EventArgs e) {
      Layers.Resize(ClientRectangle);
      base.OnResize(e);
    }

    //#region IMessageFilter implementation
    ///// <summary>Redirect WM_MouseWheel messages to window under mouse.</summary>
    ///// <remarks>Redirect WM_MouseWheel messages to window under mouse (rather than 
    ///// that with focus) with adjusted delta.
    ///// <a href="http://www.flounder.com/virtual_screen_coordinates.htm">Virtual Screen Coordinates</a>
    ///// Dont forget to add this to constructor:
    ///// 			Application.AddMessageFilter(this);
    ///// </remarks>
    ///// <param name="m">The Windows Message to filter and/or process.</param>
    ///// <returns>Success (true) or failure (false) to OS.</returns>
    //[System.Security.Permissions.PermissionSetAttribute(
    //  System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
    //public bool PreFilterMessage(ref Message m) {
    //  if ((WM)m.Msg != WM.MOUSEHWHEEL && (WM)m.Msg != WM.MOUSEWHEEL) return false;
    //  var hWnd = NativeMethods.WindowFromPoint(WindowsMouseInput.GetPointLParam(m.LParam));
    //  var ctl	  = Control.FromHandle(hWnd);
    //  if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
    //    switch((WM)m.Msg) {
    //      case WM.MOUSEHWHEEL:
    //      case WM.MOUSEWHEEL:
    //        DebugTracing.Trace(TraceFlags.ScrollEvents, true," - {0}.WM.{1}: ", Name, ((WM)m.Msg)); 
    //        return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
    //      default: break;
    //    }
    //  }
    //  return false;
    //}
    //#endregion
  }

    /// <summary>TODO</summary>
  public class LayerCollection : ReadOnlyCollection<Layer> {
    /// <summary>TODO</summary>
    internal LayerCollection(Graphics g, Size size) : this(g, size, new List<PaintAction>()) {}

    /// <summary>TODO</summary>
    internal LayerCollection(Graphics g, Size size, IList<PaintAction> list) : base(new List<Layer>()) {
      Context   = new BufferedGraphicsContext();
      Graphics  = g;
      Size      = size;

      foreach (var action in list) this.AddLayer(action);
    }

    /// <summary>TODO</summary>
    public void AddLayer(PaintAction paintAction) { Items.Add(NewLayer(paintAction)); }

    /// <summary>TODO</summary>
    public void Render(Graphics g, Point scrollPosition) {
      for(var i=0; i < this.Count; i++) this[i].Render(g, scrollPosition); 
    }

    /// <summary>TODO</summary>
    public void Resize (Rectangle rectangle) {
      Size = rectangle.Size;
      Context.MaximumBuffer = Size;
      foreach(var layer in this) {
        layer.Resize(Context.Allocate(Graphics, rectangle));
      }
    }

    BufferedGraphicsContext Context   { get; set; }
    Graphics                Graphics  { get; set; }
    Size                    Size      { get; set; }

    /// <summary>TODO</summary>
    Layer            NewLayer(PaintAction paintAction) {
      return new Layer(
        Context.Allocate(Graphics, new Rectangle(Point.Empty,Size)), 
        paintAction
      );
    }

    #region IDisposable implementation
    bool _isDisposed = false;

    /// <summary>Anchors the Dispose chain for sub-classes.</summary>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          if (Context  != null) Context.Dispose();  Context  = null;
          if (Graphics != null) Graphics.Dispose(); Graphics = null;
        }
        _isDisposed = true;
      }
    }
    #endregion
  }

}
namespace PGNapoleonics.HexgridPanel {
  using System.Reflection;

  using PaintAction = Action<Graphics>;

  /// <summary>TODO</summary>
  public sealed class Layer {

    /// <summary>TODO</summary>
    internal Layer(BufferedGraphics buffer, PaintAction paintAction) {
      Buffer      = buffer;
      PaintAction = paintAction;

      Background  = Color.Transparent;
      IsOn        = true;
      Resize(Buffer);
    }

    /// <summary>TODO</summary>
    internal void Resize(BufferedGraphics buffer) {
      const BindingFlags binding = BindingFlags.NonPublic | BindingFlags.Instance;

      Buffer   = buffer;
      var info = Buffer.GetType().GetField("virtualSize",binding);
      Size     = (Size)info.GetValue(Buffer);
    }

    /// <summary>TODO</summary>
    public Color            Background      { get; private set; }
    /// <summary>TODO</summary>
    public BufferedGraphics Buffer          { 
      get { return _buffer; }
      private set { if (_buffer != null) _buffer.Dispose(); _buffer = value; }
    } BufferedGraphics _buffer;
    /// <summary>TODO</summary>
    public bool             IsOn            { get; set; }
    /// <summary>TODO</summary>
    public PaintAction      PaintAction     { get; private set; }
    /// <summary>TODO</summary>
    public Size             Size            { get; private set; }

    /// <summary>TODO</summary>
    public void Refresh() {
      if (IsOn) {
        var g     = Buffer.Graphics;
        var state = g.Save();

        g.Clear(Background);

        PaintAction(g);

        g.Restore(state);
      }
    }

#if NET45
    /// <summary>TODO</summary>
    public async Task RefreshAsync() { await Task.Run(() => Refresh()); }

    /// <summary>TODO</summary>
    public async Task RenderAsync(Graphics target, Point scrollPosition) {
      await Task.Run(() => Render(target,scrollPosition));
    }
#endif

    /// <summary>TODO</summary>
    /// <param name="target"></param>
    /// <param name="scrollPosition"></param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    public void Render(Graphics target, Point scrollPosition) {
      if (target != null) {
        IntPtr targetDC = target.GetHdc();
 
        try { RenderInternal(new HandleRef(target, targetDC), Buffer, scrollPosition); } 
        finally { target.ReleaseHdcInternal(targetDC);  }
      }
    }

    /// <summary>TODO</summary>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
    private void RenderInternal(HandleRef refTargetDC, BufferedGraphics buffer, Point scrollPosition) {
      const int  rop = 0xcc0020; // RasterOp.SOURCE.GetRop();

      var sourceDC    = buffer.Graphics.GetHdc(); 
      var virtualSize = Size;
      try { 
        NativeMethods.BitBlt(refTargetDC, scrollPosition.X,  scrollPosition.Y, 
                                              virtualSize.Width, virtualSize.Height, 
                                  new HandleRef(buffer.Graphics, sourceDC), 0, 0, rop);
      } 
      finally { buffer.Graphics.ReleaseHdcInternal(sourceDC); }
    } 
  }
}
