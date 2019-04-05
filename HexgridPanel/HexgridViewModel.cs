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
using System.Linq;
using System.Windows.Forms;

using PGNapoleonics.HexgridExampleCommon;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexgridPanel.WinForms;

#pragma warning disable 1587
/// <summary>Implementations of a scrollable tiltable Winforms Panel.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridPanel {
    using Model     = IMapDisplayWinForms<IHex>;
    using HexSize   = System.Drawing.Size;
    using HexSizeF  = System.Drawing.SizeF;
    using HexPointF = System.Drawing.PointF;

    /// <summary>.</summary>
    public class HexgridViewModel {
        /// <summary>TODO</summary>
        public HexgridViewModel(HexgridPanel panel) {
            Panel      = panel;

            Panel.HotspotHexChange  += HotspotHexChange;
            Panel.MarginChanged     += MarginChanged;
    //      Panel.MouseAltClick     += MouseAltClick;
    //      Panel.MouseCtlClick     += GoalHexChange;
    //      Panel.MouseRightClick   += MouseRightClick;
    //      Panel.ScaleChange       += ScaleChange;

            SetScales( new List<float>() { 0.250F, 0.297F, 0.354F, 0.420F,
                                           0.500F, 0.594F, 0.707F, 0.841F,
                                           1.000F, 1.189F, 1.414F, 1.684F,
                                           2.000F }.AsReadOnly());
            Grid  = GetHexgrid();
        }

        HexgridPanel Panel { get; set; }

        ///// <summary>Return new AutoScrollPosition for applied muse-wheel scroll.</summary>
        //static Point WheelPanel(ScrollProperties scroll, int delta, ref int remainder,
        //  Func<int,Point> newAutoScroll)
        //{
        //  if (Math.Sign(delta) != Math.Sign(remainder)) remainder = 0;
        //  var steps = (delta+remainder) 
        //            / (SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines);
        //  remainder = (delta+remainder) 
        //            % (SystemInformation.MouseWheelScrollDelta / SystemInformation.MouseWheelScrollLines);
        //  return newAutoScroll(scroll.SmallChange * steps);
        //}
        ///// <summary>TODO</summary>
        //static ScrollEventArgs GetScrollEventArgs(bool isHorizontal, Point oldScroll, Point newScroll) {
        //  return new ScrollEventArgs(
        //    ScrollEventType.ThumbTrack,
        //    isHorizontal ? -oldScroll.X : -oldScroll.Y,
        //    isHorizontal ? -newScroll.X : -newScroll.Y,
        //    isHorizontal ? ScrollOrientation.HorizontalScroll : ScrollOrientation.VerticalScroll
        //  );
        //}

        IHexgrid GetHexgrid()
        => new Hexgrid(IsTransposed,Model.GridSize,MapScale,Margin.OffsetSize());

        #region Properties
        /// <summary>MapBoard hosting this panel.</summary>
        public Model Model           {
            get => _model;
            set { if(_model is IDisposable model) model.Dispose(); _model = value; }
        } Model _model = EmptyBoard.TheOne;

        /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
        public HexCoords   HotspotHex      { get; set; } = HexCoords.EmptyUser;

        /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
        protected static bool IsAltKeyDown   => HexgridPanel.IsAltKeyDown;
        /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
        protected static bool IsCtlKeyDown   => HexgridPanel.IsCtlKeyDown;
        /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
        protected static bool IsShiftKeyDown => HexgridPanel.IsShiftKeyDown;

        /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
        public bool        IsTransposed {
            get => _isTransposed;
            set {
                _isTransposed = value;
                Grid = GetHexgrid();
                if (Panel.IsHandleCreated) Panel.SetScrollLimits(Model);
            }
        }
        bool _isTransposed;

        /// <inheritdoc/>
        public HexSize     MapSizePixels   => Model.MapSizePixels(); // + MapMargin.Scale(2);} }

        /// <summary>Current scaling factor for map display.</summary>
        public float       MapScale {
            get => Model.MapScale;
            private set => Model.MapScale = value;
        }

        /// <inheritdoc/>
        public Padding     Margin {
            get => _margin;
            set { _margin = value; Grid.Margin = new HexSize(_margin.Left,_margin.Top); }
        }
        Padding _margin;

        /// <summary>TODO</summary>
        public    bool     IsMapDirty {
            get => _isMapDirty;
            set {
                _isMapDirty = value;
                if (_isMapDirty) { IsUnitsDirty = true; }
            }
        }
        bool _isMapDirty;
        /// <summary>TODO</summary>
        public    bool     IsUnitsDirty {
            get => _isUnitsDirty;
            set {
                _isUnitsDirty = value;
                if (_isUnitsDirty) { Panel.Invalidate(); }
            }
        }
        bool _isUnitsDirty;

        /// <summary>Array of supported map scales  as IList {float}.</summary>
        public IReadOnlyList<float> Scales { get; private set; }

        /// <summary>Index into <code>Scales</code> of current map scale.</summary>
        public virtual int ScaleIndex {
            get => _scaleIndex;
            set {
                var newValue = Math.Max(0, Math.Min(Scales.Count-1, value));
                if (_scaleIndex != newValue) {
                    _scaleIndex = newValue;
                    MapScale    = Scales[ScaleIndex];
                    Grid        = new Hexgrid(IsTransposed,Model.GridSize,MapScale);
                    ScaleChange?.Invoke(this,EventArgs.Empty);
                }
            }
        }
        int _scaleIndex;
        #endregion

        /// <summary>TODO</summary>
        public void SetScales(IReadOnlyList<float> scales) {
            Scales = scales;
            ScaleIndex = Scales.Where((o,i) => Math.Abs(o-1.00F) < 0.01F).Select((o,i)=>i).FirstOrDefault();
        }
        #region Events
        /// <summary>TODO</summary>
        void MarginChanged(object sender,EventArgs e) => Margin = Panel.Margin;

        /// <summary>Announces that the mouse is now over a new hex.</summary>
        void HotspotHexChange(object sender, HexEventArgs e) {
          //if (e==null) throw new ArgumentNullException(nameof(e));
          if ( e.Coords != HotspotHex)    HotspotHex = e.Coords;
        }

    //    /// <summary>Announces that the Path-Goal hex has changed.</summary>
    //    public event EventHandler<HexEventArgs> GoalHexChange;
    //    /// <summary>Announces that the Path-Start hex has changed.</summary>
    //    public event EventHandler<HexEventArgs> StartHexChange;
    //    /// <summary>Announces occurrence of a mouse left-click with the <b>Alt</b> key depressed.</summary>
    //    public event EventHandler<HexEventArgs> MouseAltClick;
    //    /// <summary>Announces a mouse right-click. </summary>
    //    public event EventHandler<HexEventArgs> MouseRightClick;
        /// <summary>Announces a change of drawing scale on this HexgridPanel.</summary>
        public event EventHandler<EventArgs>    ScaleChange;
        #endregion

        #region Grid Coordinates
        /// <inheritdoc/>
        public IHexgrid   Grid    { get; set; }
        /// <summary>Gets a SizeF struct for the hex GridSize under the current scaling.</summary>
        public HexSizeF GridSizeF => Model.GridSize.Scale(MapScale);

        //CoordsRectangle  GetClipInHexes(PointF point, SizeF size) {
        //  return Model.GetClipInHexes(point, size);
        //}

        /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
        /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        public HexPointF HexCenterPoint(HexCoords coordsNewULHex)
        => Grid.HexCenterPoint(coordsNewULHex);
        #endregion
    }
}
