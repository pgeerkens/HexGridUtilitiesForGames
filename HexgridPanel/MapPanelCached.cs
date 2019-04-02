#region License - Copyright (C) 2012-2019 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>TODO</summary>
    public sealed partial class CachedMapPanel : HexgridScrollable {
        /// <summary>TODO</summary>/>
        public CachedMapPanel() { InitializeComponent(); }

        #region EventHandlers
        /// <inheritdoc/>
        protected override void OnResize(EventArgs e) {
            BufferMap = BufferUnits = BufferShading = BufferBack = null;
            base.OnResize(e);
        }
        #endregion

        #region Map Caching
        /// <summary>The fraction of 100% at which the map is drawn (and stored) to <seealso name="BufferCache"/>.</summary>
        [Browsable(false)]
        public  float  CacheScale    { 
            get { return _cacheScale; }
            set { _cacheScale = value; BufferCache = null; }
        } float  _cacheScale = 1.00F;
        private Bitmap BufferCache   {
            get { return _bufferCache; }
            set { if(_bufferCache!=null) _bufferCache.Dispose(); _bufferCache = value; }
        } Bitmap _bufferCache = null;
        private Bitmap BufferBack    {
            get { return _bufferBack    ?? ( _bufferBack  = AllocateBuffer(ClientSize,"Back") ); }
            set { if(_bufferBack != null) _bufferBack.Dispose(); _bufferBack = value; }
        } Bitmap _bufferBack = null;
        private Bitmap BufferMap     {
            get { return _bufferMap     ?? ( _bufferMap   = AllocateBuffer(ClientSize,"Map") ); }
            set { if(_bufferMap != null) _bufferMap.Dispose(); _bufferMap = value; }
        } Bitmap _bufferMap = null;
        private Bitmap BufferShading {
            get { return _bufferShading ?? ( _bufferShading = AllocateBuffer(ClientSize,"Shading") ); }
            set { if(_bufferShading != null) _bufferShading.Dispose(); _bufferShading = value; }
        } Bitmap _bufferShading = null;
        private Bitmap BufferUnits   {
            get { return _bufferUnits   ?? ( _bufferUnits = AllocateBuffer(ClientSize,"Units") ); }
            set { if(_bufferUnits != null) _bufferUnits.Dispose(); _bufferUnits = value; }
        } Bitmap _bufferUnits = null;

        private static Bitmap AllocateBuffer(Size size, string tag) {
          Bitmap temp = null, buffer = null;
            try {
                var width  = Math.Max(1,size.Width);
                var height = Math.Max(1,size.Height);
                temp       = new Bitmap(width, height) {Tag = tag};
                //temp       = new Bitmap(width, height); temp.Tag = tag;
                buffer     = temp;
                temp       = null;
            } finally {
                if (temp != null) temp.Dispose(); 
            }
            return buffer;
        }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private async Task<Bitmap> PaintedCacheBufferAsync(string tag) {
            return await Task.Run( () => PaintedCacheBuffer(tag) );
        }
        private Bitmap PaintedCacheBuffer(string tag) {
            Bitmap temp   = null;
            Bitmap bitmap = null;

            try {
                var size   = Size.Round(MapSizePixels.Scale(CacheScale));
                var width  = Math.Max(1,size.Width);
                var height = Math.Max(1,size.Height);

                temp = new Bitmap(width, height) { Tag = tag };
                temp.Paint(Point.Empty, CacheScale, g => {
                    var model = DataContext.Model;
                    model.PaintMap(g, true, model.BoardHexes, model.Landmarks);
                });
                bitmap = temp;
                temp   = null;
            } finally { if(temp != null) temp.Dispose(); }
            return bitmap;
        }
        #endregion

        #region Painting
        private const int CACHE_IS_FREE      = 0;
        private const int CACHE_IS_PAINTING  = 1;
    //    private const int CACHE_IS_DISPOSING = 2;
        private       int _isCacheBusy       = CACHE_IS_FREE;

        /// <inheritdoc/>
        protected override async void OnPaint(PaintEventArgs e) {
            if(e==null) throw new ArgumentNullException(nameof(e));
            if (DesignMode) { e.Graphics.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

            if (BufferCache != null) {
                var mapScale = DataContext.Scales[ScaleIndex];
                var location = AutoScrollPosition + Margin.OffsetSize();

                BufferCache  .Render(BufferMap,   location, mapScale / CacheScale);
                BufferMap    .Render(BufferUnits, location, mapScale, g => {
                    var model = DataContext.Model;
                    model.PaintUnits(g);
                });
                BufferUnits  .Render(BufferBack,  location, mapScale, g => {
                    var model = DataContext.Model;
                    model.PaintHighlight(g, true);
                });
                BufferShading.Render(BufferBack, location, mapScale, g => {
                    var model = DataContext.Model;
                    model.PaintShading(g, model.Fov, model.ShadeBrushAlpha, model.ShadeBrushColor);
                });

                e.Graphics.DrawImageUnscaled(BufferBack, Point.Empty);
            } else {
                var isBusy = Interlocked.CompareExchange(ref _isCacheBusy, CACHE_IS_PAINTING, CACHE_IS_FREE);
                if (isBusy == CACHE_IS_FREE) {
                    try     { BufferCache  = await PaintedCacheBufferAsync("Cache"); }
                    finally { _isCacheBusy = CACHE_IS_FREE; }
                    Refresh();
                }
            }
        }
        #endregion
    }
}
