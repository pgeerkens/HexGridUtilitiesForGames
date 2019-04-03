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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
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
            get => _buffer;
            private set { if (_buffer != null) _buffer.Dispose(); _buffer = value; }
        } BufferedGraphics _buffer;
        /// <summary>TODO</summary>
        public bool             IsOn            { get; set; }
        /// <summary>TODO</summary>
        public PaintAction      PaintAction     { get; }
        /// <summary>TODO</summary>
        public Size             Size            { get; private set; }

        /// <summary>TODO</summary>
        public void Refresh() { if (IsOn) Buffer.Graphics.Contain(Refresh); }

        private void Refresh(Graphics graphics) {
            graphics.Clear(Background);
            PaintAction(graphics);
        }

        /// <summary>TODO</summary>
        public async Task RefreshAsync() => await Task.Run(() => Refresh());

        /// <summary>TODO</summary>
        public async Task RenderAsync(Graphics target, Point scrollPosition) =>
            await Task.Run(() => Render(target,scrollPosition));

        /// <summary>TODO</summary>
        /// <param name="target"></param>
        /// <param name="scrollPosition"></param>
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
        public void Render(Graphics target, Point scrollPosition) {
            if (target != null) {
                var targetDC = target.GetHdc();
 
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
            } finally { buffer.Graphics.ReleaseHdcInternal(sourceDC); }
        } 
    }
}
