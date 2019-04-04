#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridExampleWinforms2 {
    using HexPoint = System.Drawing.Point;
    using HexSize = System.Drawing.Size;
    using MapGridHex = IHex;

    public class HexgridScrollViewerViewModel : ViewModelBase {
        public HexgridScrollViewerViewModel() : base("HexgridExampleWinforms2 Test (WPF)") {
          View.Render += HexgridScrollViewer_Rendering;
        }

        #region Properties
        public Color BackColor { get; set; }
        /// <summary>Mimic WinForms ScrollPosition to simpliify conversion.</summary>
        private Point ScrollPosition { 
          get => new Point(0,0); //View.HorizontalOffset,View.VerticalOffset); } 
          set => View.SetScrollPosition(value);
        }
        /// <summary>MapBoard hosting this panel.</summary>
        public MapDisplay<MapGridHex> Model    { 
          get => _model;
          set {  if (_model != null) _model.Dispose();  _model = value;  }
        } MapDisplay<MapGridHex> _model = EmptyBoard.TheOne;

        /// <summary>Gets or sets the coordinates of the hex currently underneath the mouse.</summary>
        public        HexCoords HotspotHex     { get; private set; }

        /// <summary>Gets whether the <b>Alt</b> <i>shift</i> key is depressed.</summary>
        protected static  bool  IsAltKeyDown   => Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
        /// <summary>Gets whether the <b>Ctl</b> <i>shift</i> key is depressed.</summary>
        protected static  bool  IsCtlKeyDown   => Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
        /// <summary>Gets whether the <b>Shift</b> <i>shift</i> key is depressed.</summary>
        protected static  bool  IsShiftKeyDown => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

        /// <summary>Gets or sets whether the board is transposed from flat-topped hexes to pointy-topped hexes.</summary>
        public     bool         IsTransposed  { 
          get => Model.IsTransposed;
          set => Model.IsTransposed = value; //SetScrollLimits(Model); }
        }

        /// <inheritdoc/>
        public     Size         MapSizePixels { get {return MapSizePixels;} }

        /// <summary>Current scaling factor for map display.</summary>
        public     float        MapScale      { 
          get => Model.MapScale;
          private set { Model.MapScale = value; } // SetScrollLimits(Model); } 
        }

        /// <summary>Returns <code>HexCoords</code> of the hex closest to the center of the current viewport.</summary>
        public     HexCoords    PanelCenterHex
          => GetHexCoords( View.PointFromScreen(new Point(View.Width*0.50, View.Height*0.50)) );

        /// <summary>Index into <code>Scales</code> of current map scale.</summary>
        public virtual int      ScaleIndex    { 
          get => _scaleIndex;
          set { var newValue = Math.Max(0, Math.Min(ScaleList.Count-1, value));
                if( _scaleIndex != newValue) {
                  _scaleIndex = newValue;
                  MapScale    = ScaleList[ScaleIndex];

    //              OnScaleChange(EventArgs.Empty); 
                }
              } 
        } int _scaleIndex;

        /// <summary>Array of supported map scales  as IList {float}.</summary>
        public IList<float>     ScaleList        { get; private set; }

        private HexgridScrollViewer View { get; set; }
        #endregion
  
        #region Grid Coordinates
        /// <inheritdoc/>
        protected IHexgrid    Hexgrid         { get {return Model.Hexgrid;} }
        /// <summary><c>HexCoords</c> for a selected hex.</summary>
        /// <param name="point">Screen point specifying hex to be identified.</param>
        /// <returns>Coordinates for a hex specified by a screen point.</returns>
        /// <remarks>See "file://Documentation/HexGridAlgorithm.mht"</remarks>
        public HexCoords GetHexCoords(Point point)
        => Hexgrid.GetHexCoords(new HexPoint((int)point.X, (int)point.Y), 
                                new HexSize((int)ScrollPosition.X,(int)ScrollPosition.Y));
        
        /// <summary>Returns ScrollPosition that places given hex in the upper-Left of viewport.</summary>
        /// <param name="coordsNewULHex"><c>HexCoords</c> for new upper-left hex</param>
        /// <returns>Pixel coordinates in Client reference frame.</returns>
        public Point HexCenterPoint(HexCoords coordsNewULHex) {
            var point = Hexgrid.HexCenterPoint(coordsNewULHex);
            return new Point(point.X,point.Y);
        }
        #endregion

        #region Painting
        /// <summary>TODO</summary>
        /// <param name="dc"></param>
        protected void HexgridScrollViewer_Rendering(object sender, RenderingEventArgs e) {
            if (e==null) throw new ArgumentNullException(nameof(e));

            var dc = e.DrawingContext;
    //        dc.Clip = new Region(e.ClipRectangle);
            if (IsTransposed) { dc.PushTransform(TransposeMatrix); }
            OnRenderViewDetail (dc);
            if (IsTransposed) dc.Pop();
            View.InvalidateVisual();
        }
        /// <summary>TODO</summary>
        /// <param name="dc"></param>
        protected virtual void OnRenderViewDetail(DrawingContext dc) {
            if (dc==null) throw new ArgumentNullException("dc");

            Point scroll; // = Hexgrid.GetScrollPosition(ScrollPosition);
            if (IsTransposed) scroll = ScrollPosition; 
            else scroll = new Point(ScrollPosition.Y, ScrollPosition.X);
            dc.PushTransform(new TranslateTransform(scroll.X, scroll.Y));
            dc.PushTransform(new TranslateTransform(View.Margin.Left,View.Margin.Top));
            dc.PushTransform(new ScaleTransform(MapScale,MapScale));
            //TraceFlags.PaintDetail.Trace("{0}.PaintPanel: ({1})", Name, dc.VisibleClipBounds);

            RenderMap(dc);
            RenderUnits(dc);
            RenderHighlight(dc);

            dc.Pop();dc.Pop();dc.Pop();
        }
        /// <summary>TODO</summary>
        protected virtual void RenderMap(DrawingContext dc) {
            if (dc == null) throw new ArgumentNullException(nameof(dc));

            var rectangle = new Rect(new Point(),new Vector(View.ActualWidth,View.ActualHeight));
            var brush = new SolidColorBrush(BackColor);
            dc.DrawRectangle(brush,null, rectangle);
            Model.PaintMap(dc, true, Model.BoardHexes, Model.Landmarks);
        }
        /// <summary>TODO</summary>
        protected virtual void RenderUnits(DrawingContext dc) {
            if (dc == null) throw new ArgumentNullException(nameof(dc));
            Model.PaintUnits(dc);
        }
        /// <summary>TODO</summary>
        protected virtual void RenderHighlight(DrawingContext dc) {
            if (dc == null) throw new ArgumentNullException(nameof(dc));
            Model.PaintHighlight(dc, Model.ShowRangeLine);
        }

        static readonly Transform TransposeMatrix = new MatrixTransform(new Matrix(0F,1F, 1F,0F, 0F,0F));
        #endregion
    }
}
