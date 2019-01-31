#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace PGNapoleonics.HexgridPanel {
    using PaintAction = Action<Graphics>;

    /// <summary>TODO</summary>
    public class LayerCollection : ReadOnlyCollection<Layer> {
        /// <summary>TODO</summary>
        internal LayerCollection(Graphics g, Size size) : this(g, size, new List<PaintAction>()) {}

        /// <summary>TODO</summary>
        internal LayerCollection(Graphics g, Size size, IList<PaintAction> list) : base(new List<Layer>()) {
            Context   = new BufferedGraphicsContext();
            Graphics  = g;
            Size      = size;

            foreach (var action in list) AddLayer(action);
        }

        /// <summary>TODO</summary>
        public void AddLayer(PaintAction paintAction) => Items.Add(NewLayer(paintAction));

        /// <summary>TODO</summary>
        public void Render(Graphics g, Point scrollPosition) {
            for(var i=0; i < Count; i++) this[i].Render(g, scrollPosition); 
        }

        /// <summary>TODO</summary>
        public void Resize (Rectangle rectangle) {
            Size = rectangle.Size;
            Context.MaximumBuffer = Size;
            foreach(var layer in this) { layer.Resize(Context.Allocate(Graphics, rectangle)); }
        }

        BufferedGraphicsContext Context   { get; set; }
        Graphics                Graphics  { get; set; }
        Size                    Size      { get; set; }

        /// <summary>TODO</summary>
        Layer            NewLayer(PaintAction paintAction) =>
            new Layer(
                Context.Allocate(Graphics, new Rectangle(Point.Empty,Size)), 
                paintAction
            );

        #region IDisposable implementation
        /// <summary>Clean up any resources being used, and suppress finalization.</summary>
        public void Dispose() => Dispose(true);
        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;
        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
