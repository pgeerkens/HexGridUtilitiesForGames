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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
  /// <summary>TODO</summary>
  public partial class BufferedHexgridScrollable : HexgridScrollable {
    /// <summary>TODO</summary>
    public BufferedHexgridScrollable() {
      InitializeComponent();
    }
    /// <summary>Creates a new instance of HexgridPanel.</summary>
    public BufferedHexgridScrollable(IContainer container) {
      if (container==null) throw new ArgumentNullException("container");
      container.Add(this);

      InitializeComponent();
    }
    /// <summary>Force repaint of backing buffer for Map underlay.</summary>
    public override void SetMapDirty() { 
      if (MapBuffer!=null) PaintBuffer(ScaleRectangle(base.ClientRectangle, MapScale)); 
      base.SetMapDirty();; 
    }

    /// <summary>TODO</summary>
    protected override void RenderMap(Graphics g) {
      if (g == null) throw new ArgumentNullException("g");
      using(var brush = new SolidBrush(this.BackColor)) g.FillRectangle(brush,ClientRectangle);
      g.ScaleTransform(1.0f/MapScale, 1.0f/MapScale);
      MapBuffer.Render(g, Point.Empty, ClientSize);
    }

     /// <summary>TODO</summary>
    static Rectangle ScaleRectangle(Rectangle rectangle, float scale) {
      return new Rectangle(new Point(new SizeF(rectangle.Location.Scale(scale)).ToSize()), 
                                               rectangle.Size.Scale(scale).ToSize());
    }
   #region BufferedGraphics
    /// <summary>Gets or sets backing buffer for the map underlay.</summary>
    protected BufferedGraphics MapBuffer { get; private set; }
    /// <summary>Gets or sets spare buffer for the map underlay.</summary>
    protected BufferedGraphics MapSpare  { get; private set; }

    /// <summary>Service routine to paint the backing store bitmap for the map underlay.</summary>
    protected virtual void PaintBuffer(Rectangle clipBounds) {
      if (DataContext.Model==null  ||  MapBuffer==null) return;

      var g = MapBuffer.Graphics;
      if (g != null) {
        var state = g.Save();

        g.Clip = new Region(clipBounds);
        if (IsTransposed) { g.Transform = TransposeMatrix; }
        var scroll = DataContext.Hexgrid.GetScrollPosition(AutoScrollPosition);
        g.TranslateTransform(scroll.X, scroll.Y);
        g.TranslateTransform(Margin.Left,Margin.Top);
        g.ScaleTransform(MapScale,MapScale);
        TraceFlags.PaintDetail.Trace("{0}.PaintBuffer - VisibleClipBounds: ({1})", Name, g.VisibleClipBounds);

        using(var brush = new SolidBrush(this.BackColor)) g.FillRectangle(brush, g.VisibleClipBounds);
        DataContext.Model.PaintMap(g);

        g.Restore(state);
      }
    }

    /// <inheritdoc/>
    protected override void OnResize(EventArgs e) {
      if(Application.OpenForms.Count > 0 
      && Application.OpenForms[0].WindowState != FormWindowState.Minimized) ResizeBuffer();
      base.OnResize(e);
    }

    /// <inheritdoc/>
    protected override void OnScaleChange(EventArgs e) {
      ResizeBuffer();
      base.OnScaleChange(e);
    }

    BufferedGraphicsContext _bufferedGraphicsContext = new BufferedGraphicsContext();

    /// <inheritdoc/>
    protected override void OnScroll(ScrollEventArgs se) {
      if (se==null) throw new ArgumentNullException("se");
      var clip = (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
               ? HorizontalScrollBufferedGraphics(se.NewValue - se.OldValue)
               : VerticalScrollBufferedGraphics(se.NewValue - se.OldValue);

      if (clip.Size != Size.Empty) PaintBuffer(clip);
      base.OnScroll(se);
    }

    /// <summary>TODO</summary>
    /// <param name="delta"></param>
    /// <returns>Clipping rectangle requiring re-painting.</returns>
    protected virtual Rectangle HorizontalScrollBufferedGraphics(int delta) {
      if (delta == 0)    return Rectangle.Empty;

//      Render(MapBuffer, MapSpare.Graphics, new Point(-delta,0));
      MapBuffer.Render(MapSpare.Graphics, new Point(-delta,0), ClientSize);
      var temp = MapBuffer; MapBuffer = MapSpare; MapSpare = temp;
      if (delta < 0) 
        return new Rectangle(0, 0, -delta,ClientSize.Height);
      else
        return new Rectangle(ClientSize.Width-delta,0, delta,ClientSize.Height);
    }

    /// <summary>TODO</summary>
    /// <param name="delta"></param>
    /// <returns>Clipping rectangle requiring re-painting.</returns>
    protected virtual Rectangle VerticalScrollBufferedGraphics(int delta) {
      if (delta == 0)    return Rectangle.Empty;

//      Render(MapBuffer, MapSpare.Graphics, new Point(0,-delta));
      MapBuffer.Render(MapSpare.Graphics, new Point(0,-delta), ClientSize);
      var temp = MapBuffer; MapBuffer = MapSpare; MapSpare = temp;
      if (delta < 0) 
        return new Rectangle(0, 0, ClientSize.Width,-delta);
      else
        return new Rectangle(0,ClientSize.Height-delta, ClientSize.Width,delta);
    }

    /// <summary>TODO</summary>
    void ResizeBuffer() {
      var rectangle = new Rectangle(Point.Empty, ClientSize);
      if (ClientSize != _bufferedGraphicsContext.MaximumBuffer) {
        _bufferedGraphicsContext.MaximumBuffer = ClientSize;

        if (MapBuffer != null) MapBuffer.Dispose();
        MapBuffer = _bufferedGraphicsContext.Allocate(this.CreateGraphics(), rectangle);

        if (MapSpare != null) MapSpare.Dispose();
        MapSpare  = _bufferedGraphicsContext.Allocate(this.CreateGraphics(), rectangle);
      }
      PaintBuffer(rectangle);
    }
    #endregion
  }

  /// <summary>TODO</summary>
  public static partial class BufferedGraphicsExtensions {
    /// <summary>TODO</summary>
    /// <param name="buffer"></param>
    /// <param name="target"></param>
    /// <param name="scrollPosition"></param>
    /// <param name="virtualSize"></param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    public static void Render(this BufferedGraphics buffer, Graphics target, Point scrollPosition, Size virtualSize) {
      if (target != null) {
        IntPtr targetDC = target.GetHdc();
 
        try { RenderInternal(new HandleRef(target, targetDC), buffer, scrollPosition, virtualSize); } 
        finally { target.ReleaseHdcInternal(targetDC);  }
      }
    }
    /// <summary>TODO</summary>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
    private static void RenderInternal(HandleRef refTargetDC, BufferedGraphics buffer, Point scrollPosition, Size virtualSize) {
      const int  rop = GdiRasterOps.SrcCopy;

      var sourceDC    = buffer.Graphics.GetHdc(); 
//      var virtualSize = ClientSize;
      try { 
        NativeMethods.BitBlt(refTargetDC, scrollPosition.X,  scrollPosition.Y, 
                                          virtualSize.Width, virtualSize.Height, 
                            new HandleRef(buffer.Graphics, sourceDC), 0, 0, rop);
      } 
      finally { buffer.Graphics.ReleaseHdcInternal(sourceDC); }
    } 
  }
}
