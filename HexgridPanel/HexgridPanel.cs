#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Storage;

using PGNapoleonics.HexgridPanel.WinForms;

namespace PGNapoleonics.HexgridPanel {
    using WpfInput = System.Windows.Input;
    using Model = IMapDisplayWinForms<IHex>;

    /// <summary>Sub-class implementation of a <b>WinForms</b> Panel with integrated <see cref="TransposableHexgrid"/> support.</summary>
    [Docking(DockingBehavior.AutoDock)]
    public partial class HexgridPanel : TiltAwareScrollableControl, ISupportInitialize {
        /// <summary>Creates a new instance of HexgridScrollable.</summary>
        public HexgridPanel() {
            DataContext = new HexgridViewModel(this);

            InitializeComponent();
        }

        /// <summary>Signals the object that initialization is starting.</summary>
        public virtual void BeginInit() { ; }
        /// <summary>Signals the object that initialization is complete.</summary>
        public virtual void EndInit() { ; }

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
        /// <summary>The map orientation in 90 degree increments, CCW.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true), Bindable(true), Category("Custom")]
        public MapOrientation        MapOrientation { get; set; }

        /// <summary>TODO</summary>
        public HexgridViewModel      DataContext    { get; }
        /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
        public SizeF                 GridSizeF      => DataContext.Model.GridSize.Scale(MapScale);
        /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
        public HexCoords             HotspotHex     => DataContext.HotspotHex;
        /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
        public static bool           IsAltKeyDown   => ModifierKeys.HasFlag(Keys.Alt);
        /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
        public static bool           IsCtlKeyDown   => ModifierKeys.HasFlag(Keys.Control);
        /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
        public static bool           IsShiftKeyDown => ModifierKeys.HasFlag(Keys.Shift);
        /// <summary>TODO</summary>
        public         bool          IsMapDirty {
            get => _isMapDirty;
            set {
                _isMapDirty = value;
                if(_isMapDirty) { IsUnitsDirty = true; }
            }
        }
        bool _isMapDirty = false;
        /// <summary>TODO</summary>
        public         bool          IsUnitsDirty {
            get => _isUnitsDirty;
            set {
                _isUnitsDirty = value;
                if(_isUnitsDirty) { Invalidate(); }
            }
        }
        bool _isUnitsDirty = false;
        /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
        [Browsable(true)] [Bindable(true)] [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Custom")]
        public         bool          IsTransposed      { get; set; }

        /// <inheritdoc/>
        public         Size          MapSizePixels     => DataContext.Model.MapSizePixels(); // + MapMargin.Scale(2);} }
        /// <summary>Current scaling factor for map display.</summary>
        [Browsable(false)]
        public         float         MapScale          => DataContext.MapScale;
        /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
        public HexCoords PanelCenterHex => GetHexCoords(Location + Size.Round(ClientSize.Scale(0.50F)));
        /// <summary>TODO</summary>
        public WpfInput.ICommand     RefreshCmd        => new RelayCommand(_refeshCmd);
        void _refeshCmd(object obj ) { if (obj != null) { SetMapDirty(); }  Refresh(); }

        /// <summary>Index into <code>Scales</code> of current map scale.</summary>
        [Browsable(false)]
        public virtual int           ScaleIndex {
            get => DataContext.ScaleIndex;
            set {
                var newValue = Math.Max(0, Math.Min(DataContext.Scales.Count-1, value));
                var CenterHex           = PanelCenterHex;
                DataContext.ScaleIndex = newValue;

                SetScroll(CenterHex);
                OnScaleChange(EventArgs.Empty);
            }
        }

        /// <summary>Array of supported map scales  as IList {float}.</summary>
        public IReadOnlyList<float>  Scales => DataContext.Scales;

        /// <summary>Returns, as a Rectangle, the IUserCoords for the currently visible extent.</summary>
        public virtual CoordsRectangle VisibleRectangle
        => GetClipInHexes(AutoScrollPosition.Scale(-1.0F / MapScale),
                            ClientSize.Scale(1.0F / MapScale));
        #endregion

        #region Methods
        /// <summary>TODO</summary>
        public         void CenterOnHex(HexCoords coords) {
            AutoScrollPosition = ScrollPositionToCenterOnHex(coords);
            IsMapDirty = true;
            Invalidate();
        }

        /// <summary>TODO</summary>
        CoordsRectangle GetClipInHexes(PointF point,SizeF size) => DataContext.Model.GetClipInHexes(point,size);
        /// <summary><c>HexCoords</c> for a selected hex.</summary>
        /// <param name="point">Screen point specifying hex to be identified.</param>
        /// <returns>Coordinates for a hex specified by a screen point.</returns>
        /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
        public HexCoords GetHexCoords(Point point) => DataContext.Grid.GetHexCoords(point, new Size(AutoScrollPosition));

        /// <summary>Force repaint of backing buffer for Map underlay.</summary>
        public virtual void SetMapDirty() => Invalidate(ClientRectangle);

        /// <summary>TODO</summary>
        public         void SetModel(Model model) {
            SetScrollLimits(DataContext.Model);   
            DataContext.Model = model;
            SetMapDirty();
        }

        /// <summary>TODO</summary>
        public         void SetPanelSize() {
            if(DesignMode || !IsHandleCreated) return;
            Tracing.Sizing.Trace(" - {0}.SetPanelSize; ClientSize = {1}", DataContext.Model.Name, ClientSize); 
            SetScroll(PanelCenterHex);
        }

        /// <summary>Sets ScrollBars, then centres on <c>newCenterHex</c>.</summary>
        public virtual void SetScroll(HexCoords newCenterHex) {
            if(DesignMode || !IsHandleCreated) return;
            Tracing.Sizing.Trace(" - {0}.SetPanelSize; Center Hex = {1}", DataContext.Model.Name, newCenterHex.ToString()); 

            SetScrollLimits(DataContext.Model);

            CenterOnHex(newCenterHex);
        }

        /// <summary>Set ScrollBar increments and bounds from map dimensions.</summary>
        public virtual void SetScrollLimits(Model model) {
            if (model == null  ||  !AutoScroll) return;
            var smallChange              = Size.Ceiling(model.GridSize.Scale(MapScale));
            HorizontalScroll.SmallChange = smallChange.Width;
            VerticalScroll.SmallChange   = smallChange.Height;

            var largeChange              = Size.Round(ClientSize.Scale(0.75F));
            HorizontalScroll.LargeChange = Math.Max(largeChange.Width,  smallChange.Width);
            VerticalScroll.LargeChange   = Math.Max(largeChange.Height, smallChange.Height);

            var size                     = DataContext.Grid.GetSize(MapSizePixels,MapScale)
                                         + Margin.Size;
            if (AutoScrollMinSize != size) {
                AutoScrollMinSize          = size;
                HorizontalScroll.Maximum   = Math.Max(HorizontalScroll.Minimum, 
                                             Math.Min(HorizontalScroll.Maximum, 
                                                      Margin.Horizontal  + size.Width - ClientSize.Width));
                VerticalScroll.Maximum     = Math.Max(VerticalScroll.Minimum, 
                                             Math.Min(VerticalScroll.Maximum, 
                                                      Margin.Vertical   + size.Height - ClientSize.Height));
                Invalidate();
            }
        }
        #endregion

        #region Grid Coordinates
        /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
        /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        public Point HexCenterPoint(HexCoords coordsNewULHex)
        => DataContext.Grid.HexCenterPoint(coordsNewULHex);

        /// <summary>Returns the scroll position to center a specified hex in viewport.</summary>
        /// <param name="coordsNewCenterHex"><c>HexCoords</c> for the hex to be centered in viewport.</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        protected Point ScrollPositionToCenterOnHex(HexCoords coordsNewCenterHex)
        => DataContext.Grid.ScrollPositionToCenterOnHex(coordsNewCenterHex,VisibleRectangle);
        #endregion

        #region Painting
        /// <inheritdoc/>
        protected override void OnPaintBackground(PaintEventArgs e) { ; }

        /// <inheritdoc/>
        protected override void OnPaint(PaintEventArgs e) {
            if(e==null) throw new ArgumentNullException(nameof(e));
            if (DesignMode) { e.Graphics.FillRectangle(Brushes.Gray, ClientRectangle); return; }

            if(IsHandleCreated) e.Graphics.Contain(PaintMe);
            base.OnPaint(e);
        }

        /// <summary>TODO</summary>
        /// <param name="graphics"></param>
        protected virtual void PaintMe(Graphics graphics) {
            if (graphics==null) throw new ArgumentNullException(nameof(graphics));

            if (IsTransposed) { graphics.Transform = TransposeMatrix; }

            var scroll = DataContext.Grid.GetScrollPosition(AutoScrollPosition);
            graphics.TranslateTransform(scroll.X + Margin.Left,  scroll.Y + Margin.Top);
            graphics.ScaleTransform(MapScale,MapScale);
            Tracing.PaintDetail.Trace($"{Name}.PaintPanel: ({graphics.VisibleClipBounds})");

            graphics.Contain(RenderMap);
            graphics.Contain(RenderUnits);
            graphics.Contain(RenderShading);
            graphics.Contain(RenderHighlight);
        }
        /// <inheritdoc/>
        protected virtual void RenderHighlight(Graphics graphics) {
            if (graphics == null) throw new ArgumentNullException("graphics");
            DataContext.Model.PaintHighlight(graphics);
        }
        /// <inheritdoc/>
        protected virtual void RenderMap(Graphics graphics) {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
            using(var brush = new SolidBrush(BackColor)) graphics.FillRectangle(brush, graphics.VisibleClipBounds);
            var model = DataContext.Model;
            model.PaintMap(graphics, true);
        }
        /// <inheritdoc/>
        protected virtual void RenderShading(Graphics graphics) {
            if (graphics == null) throw new ArgumentNullException("graphics");
            DataContext.Model.PaintShading(graphics,DataContext.Model?.Fov);
        }
        /// <inheritdoc/>
        protected virtual void RenderUnits(Graphics graphics) {
            if (graphics == null) throw new ArgumentNullException("graphics");
            DataContext.Model.PaintUnits(graphics);
        }

        /// <summary>TODO</summary>
        static protected Matrix TransposeMatrix => new Matrix(0F,1F,1F,0F,0F,0F);
        #endregion

        /// <summary>TODO</summary>
        protected override void OnMarginChanged(EventArgs e) {
            if (e == null) throw new ArgumentNullException(nameof(e));
            base.OnMarginChanged(e);
            DataContext.Margin = Margin;
        }

        #region Mouse event handlers
        /// <inheritdoc/>
        protected override void OnMouseClick(MouseEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));
            Tracing.Mouse.Trace($" - {Name}.OnMouseClick - Shift: {IsShiftKeyDown}; Ctl: {IsCtlKeyDown}; Alt: {IsAltKeyDown}");

            var coords    = GetHexCoords(e.Location);
            var eventArgs = new HexEventArgs(coords, ModifierKeys,e.Button,e.Clicks,e.X,e.Y,e.Delta);

                 if (e.Button == MouseButtons.Middle) base.OnMouseClick(eventArgs);
            else if (e.Button == MouseButtons.Right)  this.OnTargetHexSelected(eventArgs);
            else if (IsAltKeyDown  && !IsCtlKeyDown)  this.OnMouseAltClick(eventArgs);
            else if (IsCtlKeyDown)                    this.OnTargetHexSelected(eventArgs);
            else                                      this.OnStartHexSelected(eventArgs);
        }
        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));
            OnHotspotHexChange(new HexEventArgs(GetHexCoords(e.Location - Margin.OffsetSize())));

            base.OnMouseMove(e);
        }

        /// <summary>Raise the MouseAltClick event.</summary>
        protected virtual void OnMouseAltClick(HexEventArgs e) => MouseAltClick?.Invoke(this,e);
        /// <summary>Raise the MouseCtlClick event.</summary>
        protected virtual void OnTargetHexSelected(HexEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));
            DataContext.Model.GoalHex = e.Coords;
            MouseCtlClick?.Invoke(this,e);
            Refresh();
        }
        /// <summary>Raise the MouseLeftClick event.</summary>
        protected virtual void OnStartHexSelected(HexEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));
            DataContext.Model.StartHex = e.Coords;
            MouseLeftClick?.Invoke(this,e);
            Refresh();
        }

        /// <summary>Raise the HotspotHexChange event.</summary>
        protected virtual void OnHotspotHexChange(HexEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));
            DataContext.Model.HotspotHex = e.Coords;
            HotspotHexChange?.Invoke(this,e);
            Refresh();
        }

        /// <summary>Raise the ScaleChange event.</summary>
        protected virtual void OnScaleChange(EventArgs e) {
            SetMapDirty();
            OnResize(e);
            Invalidate();
            ScaleChange?.Invoke(this, e);
        }

        /// <inheritdoc/>
        protected override void OnResize(EventArgs e) {
            SetScrollLimits(DataContext.Model);
            base.OnResize(e);
        }
        #endregion

        #region MouseWheel & Scroll event handlers
        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (e == null) throw new ArgumentNullException(nameof(e));
            Tracing.ScrollEvents.Trace($" - {Name}.OnMouseWheel: {e.ToString()}");

            if (ModifierKeys.HasFlag(Keys.Control)) { ScaleIndex += Math.Sign(e.Delta); }
            else if (IsShiftKeyDown) {
                base.OnMouseHWheel(e);
            } else{
                base.OnMouseWheel(e);
            }
            if(e is HandledMouseEventArgs he) { he.Handled = true; }
        }

        /// <summary>TODO</summary>
        public void ScrollPanelVertical(ScrollEventType type, int sign)
        =>  ScrollPanelCommon(type, sign, VerticalScroll);

        /// <summary>TODO</summary>
        public void ScrollPanelHorizontal(ScrollEventType type, int sign)
        =>  ScrollPanelCommon(type, sign, HorizontalScroll);

        /// <summary>TODO</summary>
        private void ScrollPanelCommon(ScrollEventType type, int sign, ScrollProperties scroll) {
            if (sign == 0) return;
            Point func(Point p,int step) => new Point(-p.X,-p.Y + step * sign);
            AutoScrollPosition = func(AutoScrollPosition,
                type.HasFlag(ScrollEventType.LargeDecrement) ? scroll.LargeChange : scroll.SmallChange);
        }
        #endregion
    }
}
