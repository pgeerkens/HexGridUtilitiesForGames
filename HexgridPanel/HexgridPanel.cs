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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;
using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridPanel {
    /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="Hexgrid"/> support.</summary>
    [Docking(DockingBehavior.AutoDock)]
    [Obsolete("Use PGNapoleonics.HexgridPanel.HexgridScrollable instead.")]
    public partial class HexgridPanel : TiltAwarePanel, ISupportInitialize {
        /// <summary>Creates a new instance of HexgridPanel.</summary>
        public HexgridPanel() : base() => InitializeComponent();
        
        #region ISupportInitialize implementation
        /// <summary>Signals the object that initialization is starting.</summary>
        public virtual void BeginInit() { 
            ScaleList   = DefaultScaleList;
            Model       = EmptyBoard.TheOne;
            HotspotHex  = HexCoords.EmptyUser;
        }
        /// <summary>Signals the object that initialization is complete.</summary>
        public virtual void EndInit() => this.MakeDoubleBuffered(true);
        
        #endregion

        #region Events
        /// <summary>Announces that the mouse is now over a new hex.</summary>
        public event EventHandler<HexEventArgs> HotspotHexChange;
        /// <summary>Announces occurrence of a mouse left-click with the <b>Alt</b> key depressed.</summary>
        public event EventHandler<HexEventArgs> MouseAltClick;
        /// <summary>Announces occurrence of a mouse left-click with the <b>Ctl</b> key depressed.</summary>
        public event EventHandler<HexEventArgs> MouseCtlClick;
        /// <summary>Announces a mouse left-click with no <i>shift</i> keys depressed.</summary>
        public event EventHandler<HexEventArgs> MouseLeftClick;
        /// <summary>Announces a mouse right-click. </summary>
        public event EventHandler<HexEventArgs> MouseRightClick;
        /// <summary>Announces a change of drawing scale on this HexgridPanel.</summary>
        public event EventHandler<EventArgs>    ScaleChange;
        #endregion

        #region Properties
        /// <summary>MapBoard hosting this panel.</summary>
        public MapDisplay<IHex> Model    { 
          get => _model;
          set {  if (_model != null) _model.Dispose(); 
                 _model = value; 
                 SetScrollLimits(_model);   
                 SetMapDirty();
              }
        } MapDisplay<IHex> _model = EmptyBoard.TheOne;

        /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
        public     HexCoords    HotspotHex     { get; private set; }

        /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
        protected static  bool  IsAltKeyDown   => ModifierKeys.HasFlag(Keys.Alt);
        /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
        protected static  bool  IsCtlKeyDown   => ModifierKeys.HasFlag(Keys.Control);
        /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
        protected static  bool  IsShiftKeyDown => ModifierKeys.HasFlag(Keys.Shift);

        /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
        [Browsable(true)]
        public     bool         IsTransposed   { 
            get => Model.IsTransposed;
            set { Model.IsTransposed = value;  SetScrollLimits(Model); }
        }

        /// <inheritdoc/>
        public     Size         MapSizePixels  => HexBoardExtensions.MapSizePixels(Model); // + MapMargin.Scale(2);} }

        /// <summary>Current scaling factor for map display.</summary>
        public     float        MapScale       { 
            get => Model.MapScale;
            private set { Model.MapScale = value;  SetScrollLimits(Model); } 
        }

        /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
        public     HexCoords    PanelCenterHex => 
            GetHexCoords( PointToClient(new Point(Size.Round(ClientSize.Scale(0.50F))) ) );

        /// <summary>Index into <code>Scales</code> of current map scale.</summary>
        public virtual int      ScaleIndex     { 
            get => _scaleIndex;
            set { var newValue = Math.Max(0, Math.Min(ScaleList.Count-1, value));
                if( _scaleIndex != newValue) {
                    _scaleIndex = newValue;
                    MapScale    = ScaleList[ScaleIndex];

                    OnScaleChange(EventArgs.Empty); 
                }
            } 
        } int _scaleIndex;

        /// <summary>Array of supported map scales  as IList {float}.</summary>
        public IReadOnlyList<float> ScaleList { get; private set; }

        IReadOnlyList<float> DefaultScaleList = new List<float>() {
           .177F, .210F, .250F, .297F, .354F, .420F, .5000F, .594F, 0.707F, 0.841F, 1.000F, 1.189F, 1.414F
        }.AsReadOnly();
        #endregion

        /// <summary>Force repaint of backing buffer for Map underlay.</summary>
        public         void SetMapDirty() => MapBuffer = null;

        /// <summary>Set property Scales (array of supported map scales as IList {float}.</summary>
        public         void SetScaleList(IReadOnlyList<float> scales) => ScaleList = scales;

        /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
        public virtual void SetScrollLimits(IMapDisplayWinForms<IHex> model) {
            if (model == null) return;
            var smallChange              = Size.Ceiling(model.GridSize.Scale(MapScale));
            HorizontalScroll.SmallChange = smallChange.Width;
            VerticalScroll.SmallChange   = smallChange.Height;

            var largeChange              = Size.Round(ClientSize.Scale(0.75F));
            HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
            VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

            var size                     = Hexgrid.GetSize(MapSizePixels,MapScale);
            if (AutoScrollMinSize != size) {
                AutoScrollMinSize        = size;
                HorizontalScroll.Maximum = Math.Min(1, Math.Max(1, Padding.Left + Padding.Right 
                                         + size.Width  - ClientSize.Width));
                VerticalScroll.Maximum   = Math.Min(1, Math.Max(1, Padding.Top + Padding.Bottom 
                                         + size.Height - ClientSize.Height));
                Invalidate();
            }
        }

        #region Grid Coordinates
        /// <inheritdoc/>
        protected IHexgrid    Hexgrid         => Model.Hexgrid;
        /// <summary>Gets the current Panel AutoScrollPosition.</summary>
        public    Point       ScrollPosition  => AutoScrollPosition;

        CoordsRectangle       GetClipInHexes(PointF point, SizeF size) =>
            Model.GetClipInHexes(point, size);

        /// <summary>Returns, as a Rectangle, the IUserCoords for the currently visible extent.</summary>
        public virtual CoordsRectangle VisibleRectangle =>
            GetClipInHexes(AutoScrollPosition.Scale(-1.0F/MapScale), 
                                   ClientSize.Scale( 1.0F/MapScale) );

        /// <summary><c>HexCoords</c> for a selected hex.</summary>
        /// <param name="point">Screen point specifying hex to be identified.</param>
        /// <returns>Coordinates for a hex specified by a screen point.</returns>
        /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
        public HexCoords GetHexCoords(Point point) =>
            Hexgrid.GetHexCoords(point, new Size(AutoScrollPosition));
        
        /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
        /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        public Point HexCenterPoint(HexCoords coordsNewULHex) =>
            Hexgrid.HexCenterPoint(coordsNewULHex);
        
        /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
        /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        protected Point ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex) =>
            Hexgrid.ScrollPositionToCenterOnHex(coordsNewCenterHex,VisibleRectangle);
        
        #endregion

        #region Painting
        /// <inheritdoc/>
        protected override void OnPaintBackground(PaintEventArgs e) { ; }
        /// <inheritdoc/>
        protected override void OnPaint(PaintEventArgs e) {
            if(IsHandleCreated)  e.Graphics.Contain(PaintMe);
            base.OnPaint(e);
        }

        private void PaintMe(Graphics graphics) {
            var scroll = Hexgrid.GetScrollPosition(AutoScrollPosition);
            if (DesignMode) { graphics.FillRectangle(Brushes.Gray, ClientRectangle);  return; }

            graphics.Clear(Color.Black);
            graphics.DrawRectangle(Pens.Black, ClientRectangle);

            if (IsTransposed) { graphics.Transform = TransposeMatrix; }
            graphics.TranslateTransform(scroll.X, scroll.Y);
            graphics.ScaleTransform(MapScale,MapScale);

            graphics.Contain(PaintMap);
            Model.PaintUnits(graphics);
            Model.PaintShading(graphics, Model.Fov, Model.ShadeBrushAlpha, Model.ShadeBrushColor);
            Model.PaintHighlight(graphics, Model.ShowRangeLine);
        }

        private void PaintMap(Graphics graphics) =>
            graphics.DrawImageUnscaled(MapBuffer, Point.Empty);
        
        static readonly Matrix TransposeMatrix = new Matrix(0F,1F, 1F,0F, 0F,0F);
        #endregion

        #region Double-Buffering
        /// <summary>Gets or sets the backing buffer for the map underlay. Setting to null  forces a repaint.</summary>
        Bitmap MapBuffer     { 
            get => _mapBuffer ?? ( _mapBuffer = PaintBuffer());
            set { if (_mapBuffer!=null) _mapBuffer.Dispose(); _mapBuffer = value; }
        } Bitmap _mapBuffer;

        /// <summary>Paint the backing store bitmap for the map underlay.</summary>
        /// <remarks>Uses <see cref="PixelFormat.Format32bppPArgb"/>, the fastest rendering format.</remarks>
        /// <a href="http://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls">How to fix flickering in user controls?</a>
        Bitmap PaintBuffer() {
            var size      = MapSizePixels;

            Bitmap buffer = null, tempBuffer = null;
            try {
                tempBuffer = new Bitmap(size.Width,size.Height, PixelFormat.Format32bppPArgb);
                using(var g = Graphics.FromImage(tempBuffer)) {
                    g.Clear(Color.White);
                    Model.PaintMap(g, Model.ShowHexgrid, Model.BoardHexes, Model.Landmarks);
                }
                buffer     = tempBuffer;
                tempBuffer = null;
            } finally { if (tempBuffer!=null) tempBuffer.Dispose(); }
            return buffer;
        }
        #endregion

        #region Mouse event handlers
        /// <inheritdoc/>
        protected override void OnMouseClick(MouseEventArgs e) {
            if (e == null) throw new ArgumentNullException("e");
            Tracing.Mouse.Trace(" - {0}.OnMouseClick - Shift: {1}; Ctl: {2}; Alt: {3}",
                                            Name, IsShiftKeyDown, IsCtlKeyDown, IsAltKeyDown);
            var eventArgs = new HexEventArgs(GetHexCoords(e.Location), ModifierKeys,e.Button,e.Clicks,e.X,e.Y,e.Delta);

            if (e.Button == MouseButtons.Middle) base.OnMouseClick(eventArgs);
            else if (e.Button == MouseButtons.Right) OnMouseRightClick(eventArgs);
            else if (IsAltKeyDown && !IsCtlKeyDown) OnMouseAltClick(eventArgs);
            else if (IsCtlKeyDown) OnMouseCtlClick(eventArgs);
            else OnMouseLeftClick(eventArgs);
        }
        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e) {
            if (e == null) throw new ArgumentNullException("e");
            var newHex = GetHexCoords(e.Location);
            if (newHex != HotspotHex) OnHotspotHexChange(new HexEventArgs(newHex));
                HotspotHex = newHex;

            base.OnMouseMove(e);
        }

        /// <summary>Raise the MouseAltClick event.</summary>
        protected virtual void OnMouseAltClick(HexEventArgs e) => MouseAltClick.Raise(this, e);
        /// <summary>Raise the MouseCtlClick event.</summary>
        protected virtual void OnMouseCtlClick(HexEventArgs e) => MouseCtlClick.Raise(this, e);
        /// <summary>Raise the MouseLeftClic event.</summary>
        protected virtual void OnMouseLeftClick(HexEventArgs e) => MouseLeftClick.Raise(this, e);
        /// <summary>Raise the MouseRightClick event.</summary>
        protected virtual void OnMouseRightClick(HexEventArgs e) => MouseRightClick.Raise(this, e);
        /// <summary>Raise the HotspotHexChange event.</summary>
        protected virtual void OnHotspotHexChange(HexEventArgs e) => HotspotHexChange.Raise(this, e);
        /// <summary>Raise the ScaleChange event.</summary>
        protected virtual void OnScaleChange(EventArgs e) {
            ScaleChange.Raise(this, e);
            Invalidate();
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (e == null) throw new ArgumentNullException("e");
            Tracing.ScrollEvents.Trace(" - {0}.OnMouseWheel: {1}", Model.Name, e.ToString());

            if (ModifierKeys.HasFlag(Keys.Control))   ScaleIndex += Math.Sign(e.Delta);
            else if (IsShiftKeyDown)                  base.OnMouseHwheel(e);
            else                                      base.OnMouseWheel(e);
        }
        #endregion
    }
}
