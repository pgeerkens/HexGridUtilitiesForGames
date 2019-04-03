#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>TODO</summary>
    public partial class MapPanel : HexgridScrollable {
        /// <summary>TODO</summary>
        public MapPanel() => InitializeComponent();

        /// <inheritdoc/>
        public override int ScaleIndex {
            set {
                var CenterHex   = PanelCenterHex;
                base.ScaleIndex = value;
                SetScroll(CenterHex);
            }
        }

        /// <summary>Force repaint of backing buffer for Map underlay.</summary>
        public override void SetMapDirty() { 
            _cacheStatus = _NEEDS_PAINTING;
            base.SetMapDirty();
        }

        #region Mouse Events
        /// <inheritdoc/>
        protected override void OnMouseHWheel(MouseEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));

            var oldPosition    = - AutoScrollPosition.X;
            var newPosition    = NewPosition(HorizontalScroll, ClientSize.Width, oldPosition,  e.Delta);
            AutoScrollPosition = new Point(newPosition, - AutoScrollPosition.Y);

            OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, oldPosition, newPosition, 
                                         ScrollOrientation.HorizontalScroll));
        }
        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));

            var oldPosition    = - AutoScrollPosition.Y;
            var newPosition    = NewPosition(VerticalScroll, ClientSize.Height, oldPosition, -e.Delta);
            AutoScrollPosition = new Point(- AutoScrollPosition.X, newPosition);
      
            OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, oldPosition, newPosition, 
                                         ScrollOrientation.VerticalScroll));
        }
        /// <summary>TODO</summary>
        /// <param name="scroll"></param>
        /// <param name="limit"></param>
        /// <param name="oldPosition"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private static int NewPosition(ScrollProperties scroll, int limit, int oldPosition, int delta)
        => Math.Max(scroll.Minimum,
           Math.Min(scroll.Maximum - limit + 1, oldPosition + delta));
        #endregion

        #region Other Events
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
        /// <inheritdoc/>
        protected override void OnScroll(ScrollEventArgs se) {
            if (se==null) throw new ArgumentNullException("se");
            var clip = (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                     ? HorizontalScrollBuffer(se.NewValue - se.OldValue)
                     : VerticalScrollBuffer  (se.NewValue - se.OldValue);

            if (clip.Size != Size.Empty) {
                var status = Interlocked.CompareExchange(ref _cacheStatus, _IS_SCROLLING, _IS_READY);
                if (status == _IS_READY) {
                  try     { _backBuffer = Interlocked.Exchange(ref _mapBuffer, ScrollBuffer(clip)); }
                  finally { Interlocked.CompareExchange(ref _cacheStatus, _IS_READY, _IS_SCROLLING); }
                }
            }
            base.OnScroll(se);
        }

        /// <summary>Clipping rectangle requiring re-painting after a horizontal scroll.</summary>
        /// <param name="delta">The signed horizontal scroll amount</param>
        protected virtual Rectangle HorizontalScrollBuffer(int delta) {
            if (delta == 0)    return Rectangle.Empty;  // Combinig this with below impacts performance.

            _mapBuffer.Render(_backBuffer, new Point(-delta,0));
            return (delta < 0) ? new Rectangle(                     0,0, -delta,ClientSize.Height)
                               : new Rectangle(ClientSize.Width-delta,0,  delta,ClientSize.Height);
        }

        /// <summary>Clipping rectangle requiring re-painting after a vertical scroll.</summary>
        /// <param name="delta">The signed vertical scroll amount</param>
        protected virtual Rectangle VerticalScrollBuffer(int delta) {
            if (delta == 0)    return Rectangle.Empty;  // Combinig this with below impacts performance.

            _mapBuffer.Render(_backBuffer, new Point(0,-delta));
            return (delta < 0) ? new Rectangle(0,                      0, ClientSize.Width,-delta)
                               : new Rectangle(0,ClientSize.Height-delta, ClientSize.Width, delta);
        }
        #endregion

        #region BufferedGraphics
        /// <summary>Gets or sets backing buffer for the map underlay.</summary>
        private Bitmap _mapBuffer  = null;
        /// <summary>Gets or sets double-buffered buffer for the map underlay.</summary>
        private Bitmap _backBuffer = null;

        /// <summary>Service routine to paint the backing store bitmap for the map underlay, asynchronously.</summary>
        protected virtual async Task<Bitmap> PaintBufferAsync(Rectangle clipBounds)
        => await Task.Run(() => PaintBuffer(clipBounds));

        /// <summary>Service routine to paint the backing store bitmap for the map underlay.</summary>
        protected virtual Bitmap PaintBuffer(Rectangle clipBounds) {
            if (DataContext.Model==null) return null;

            Bitmap bitmap = null, temp = null;
            try {
                temp = ClientSize.AllocateBitmap();
                using(var graphics = Graphics.FromImage(temp)) {
                    graphics.Clip = new Region(clipBounds);
                    graphics.Contain(PaintBuffer);
                }

                bitmap = temp;
                temp   = null;
            } finally { if(temp != null) temp.Dispose(); }

            return bitmap;
        }

        /// <summary>Service routine to paint the backing store bitmap for the map underlay.</summary>
        protected virtual Bitmap ScrollBuffer(Rectangle clipBounds) {
            if (DataContext.Model==null) return null;

            using(var graphics = Graphics.FromImage(_backBuffer)) {
                graphics.Clip = new Region(clipBounds);
                graphics.Contain(PaintBuffer);
            }
            this.UIThread(Invalidate);
            return _backBuffer;
        }
        /// <summary>TODO</summary>
        /// <param name="graphics"></param>
        private void PaintBuffer(Graphics graphics) {
            if (IsTransposed) { graphics.Transform = TransposeMatrix; }

            var scroll = DataContext.Grid.GetScrollPosition(AutoScrollPosition);
            graphics.TranslateTransform(scroll.X + Margin.Left,  scroll.Y + Margin.Top);
            graphics.ScaleTransform(MapScale,MapScale);
            Tracing.PaintDetail.Trace($"{Name}.PaintBuffer - VisibleClipBounds: ({graphics.VisibleClipBounds})");

            using(var brush = new SolidBrush(BackColor)) {
                graphics.FillRectangle(brush, graphics.VisibleClipBounds);
            }
            graphics.Paint(Point.Empty, 1.0F, g => {
                var model = DataContext.Model;
                model.PaintMap(g, true, model.BoardHexes, model.Landmarks);
            });
        }

        /// <summary>TODO</summary>
        void ResizeBuffer() { _cacheStatus = _NEEDS_PAINTING; }
        #endregion

        #region Painting
        private const int _NEEDS_PAINTING = 0;
        private const int _IS_PAINTING    = 1;
        private const int _IS_SCROLLING   = 2;
        private const int _IS_READY       = 3;
        private       int _cacheStatus    = _NEEDS_PAINTING;

        private int _count = 0;

        /// <inheritdoc/>
        protected override async void OnPaint(PaintEventArgs e) {
            if(e==null) throw new ArgumentNullException(nameof(e));
            if (DesignMode) { e.Graphics.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

            if (IsMapDirty) { OnResize(EventArgs.Empty); IsMapDirty = false; }

            // TODO - Fix thread timing error work-around of catching and discarding InvalidOperationException.
            if (_mapBuffer != null) try { base.OnPaint(e); } catch (InvalidOperationException) { _count++; }

            var status = Interlocked.CompareExchange(ref _cacheStatus, _IS_PAINTING, _NEEDS_PAINTING);
            if (status == _NEEDS_PAINTING) {
                try {
                    if (_backBuffer != null) { _backBuffer.Dispose(); }

                    _backBuffer = Interlocked.Exchange(ref _mapBuffer, await PaintBufferAsync(ClientRectangle));

                    if (_backBuffer != null) { _backBuffer.Dispose(); }   _backBuffer = ClientSize.AllocateBitmap();
                } catch (InvalidOperationException) {
                    if (_backBuffer == null) _backBuffer = ClientSize.AllocateBitmap();

                    Interlocked.CompareExchange(ref _cacheStatus, _NEEDS_PAINTING, _IS_PAINTING);
                    Thread.Sleep(250);
                } finally { Interlocked.CompareExchange(ref _cacheStatus, _IS_READY, _IS_PAINTING); }
                Refresh();
            }
            IsUnitsDirty = false;
        }
        /// <inheritdoc/>
        protected override void PaintMe(Graphics graphics) {
            if (graphics==null) throw new ArgumentNullException("graphics");

            graphics.Contain(RenderMapLocal);
            base.PaintMe(graphics);
        }

        /// <summary>TODO</summary>
        protected override void RenderMap(Graphics graphics) { }

        private void RenderMapLocal(Graphics graphics) {
            if (graphics == null) throw new ArgumentNullException("graphics");

            using(var brush = new SolidBrush(this.BackColor)) {
                graphics.FillRectangle(brush,ClientRectangle);
            }
            _mapBuffer.Render(graphics, ClientRectangle.Location, 1.0F);
        }
        #endregion
    }
}
