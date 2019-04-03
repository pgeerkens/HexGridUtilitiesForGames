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
using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities.FastLists;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities.Common {
    using HexPoint      = System.Drawing.Point;
    using HexPointF     = System.Drawing.PointF;
    using HexSize       = System.Drawing.Size;
    using HexSizeF      = System.Drawing.SizeF;
    using RectangleF    = System.Drawing.RectangleF;
    using Color         = System.Drawing.Color;
    using GraphicsPath  = System.Drawing.Drawing2D.GraphicsPath;
    using IDirectedPath = IDirectedPathCollection;
    using Int32ValueEventArgs = ValueEventArgs<int>;

    using static System.Drawing.Drawing2D.PathPointType;

    /// <summary>.</summary>
    /// <typeparam name="THex"></typeparam>
    /// <remarks>
    /// Abstract members that require definition include:
    ///     public abstract int ElevationBase { get; }
    ///     public abstract int ElevationStep { get; }
    /// 
    ///     public abstract void PaintHighlight(Graphics graphics);
    ///     public abstract void PaintMap(Graphics graphics);
    ///     public abstract void PaintShading(Graphics graphics);
    ///     public abstract void PaintUnits(Graphics graphics);
    /// </remarks>
    public abstract class MapDisplayFlat<THex> : MapDisplay<THex>
    where THex:IHex {
        /// <summary>Creates a new instance of the MapDisplay class.</summary>
        protected MapDisplayFlat(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex)
        : base(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes),
              new FlatBoardStorage<Maybe<THex>>(sizeHexes, coords => initializeHex(coords), false)) { }
    }

    /// <summary>.</summary>
    /// <typeparam name="THex"></typeparam>
    /// <remarks>
    /// Abstract members that require definition include:
    ///     public abstract int ElevationBase { get; }
    ///     public abstract int ElevationStep { get; }
    /// 
    ///     public abstract void PaintHighlight(Graphics graphics);
    ///     public abstract void PaintMap(Graphics graphics);
    ///     public abstract void PaintShading(Graphics graphics);
    ///     public abstract void PaintUnits(Graphics graphics);
    /// </remarks>
    public abstract class MapDisplayBlocked<THex> : MapDisplay<THex>
    where THex:IHex {
        /// <summary>Creates a new instance of the MapDisplay class.</summary>
        protected MapDisplayBlocked(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex)
        : base(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes),
              BlockedBoardStorage.New32x32<Maybe<THex>>(sizeHexes, coords => initializeHex(coords))) { }
    }

    /// <summary>Abstract class representing the basic game board.</summary>
    /// <typeparam name="THex">Type of the hex for which a game board is desired.</typeparam>
    /// <remarks>
    /// Abstract members that require definition include:
    ///     public abstract int ElevationBase { get; }
    ///     public abstract int ElevationStep { get; }
    /// 
    ///     public abstract void PaintHighlight(Graphics graphics);
    ///     public abstract void PaintMap(Graphics graphics);
    ///     public abstract void PaintShading(Graphics graphics);
    ///     public abstract void PaintUnits(Graphics graphics);
    /// </remarks>
    public abstract class MapDisplay<THex> : HexBoard<THex>, IMapDisplayWinForms<THex>, IFovBoard
    where THex:IHex {

        /// <summary>TODO</summary>
        protected delegate THex InitializeHex(HexCoords coords);

        /// <summary>An array of PathTypes enum constants describing the elements of <see cref="MapDisplay{THex}.HexgridPathPoints"/>.</summary>
        private static readonly byte[] _hexgridPathPointTypes = {
                (byte)Start, (byte)Line, (byte)Line,
                (byte)Line,  (byte)Line, (byte)Line|(byte)CloseSubpath
            };

        /// <summary>Returns an array of six <see cref="HexPoint"/>s describing the corners of a hex on this <see cref="HexBoard{THex}"/>.</summary>
        /// <param name="gridSize">Dimensions of a hex on this <see cref="HexBoard{THex}"/> in pixels.</param>
        private static HexPoint[] HexgridPathPoints(HexSize gridSize)
        => new HexPoint[] { new HexPoint(gridSize.Width*1/3,               0  ),
                            new HexPoint(gridSize.Width*3/3,               0  ),
                            new HexPoint(gridSize.Width*4/3, gridSize.Height/2),
                            new HexPoint(gridSize.Width*3/3, gridSize.Height  ),
                            new HexPoint(gridSize.Width*1/3, gridSize.Height  ),
                            new HexPoint(             0,     gridSize.Height/2)
                          };

        /// <summary>Creates a new instance of the MapDisplay class.</summary>
        protected MapDisplay(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex,
                IFastList<HexCoords> landmarkCoords, BoardStorage<Maybe<THex>> storage)
        : base(sizeHexes, gridSize, storage) {
            ResetLandmarksAsync(landmarkCoords);

            GoalHex         =
            HotspotHex      =
            StartHex        = HexCoords.EmptyUser;
            ShadeBrushAlpha = 78;
            ShadeBrushColor = Color.Black;
            ShowFov         = true;
            ShowHexgrid     = true;
            ShowPath        = true;
            ShowPathArrow   = true;
            HexgridPath     = Extensions.InitializeDisposable(() =>
                     new GraphicsPath(HexgridPathPoints(gridSize), _hexgridPathPointTypes));
        }

        #region Properties
        /// <summary>Gets or sets the Field-of-View for the current <see cref="HotspotHex"/>, as an <see cref="IFov"/> object.</summary>
        public virtual  IFov         Fov {
            get           => _fov ?? (_fov = this.GetFieldOfView(ShowRangeLine ? StartHex : HotspotHex, FovRadius));
            protected set => _fov = value;
        } IFov _fov;
        /// <inheritdoc/>
        public override int          FovRadius { set { RangeCutoff = base.FovRadius = value; Fov = null; } }
        /// <inheritdoc/>
        public virtual  HexCoords    GoalHex {
            get => _goalHex;
            set { _goalHex = value; PathSet(); }
        } HexCoords _goalHex = HexCoords.EmptyUser;
        ///  <inheritdoc/>
        public          GraphicsPath HexgridPath { get; }
        /// <inheritdoc/>
        public virtual HexCoords     HotspotHex {
            get => _hotSpotHex;
            set { if (MapSizeHexes.IsOnboard(value)) _hotSpotHex = value; if (!ShowRangeLine) _fov = null; }
        } HexCoords _hotSpotHex = HexCoords.EmptyUser;
        /// <inheritdoc/>
        public          int          LandmarkToShow  { get; set; }
        /// <inheritdoc/>
        public          string       Name => "MapDisplay";
        /// <inheritdoc/>
        public          Maybe<IDirectedPath> Path => _path; Maybe<IDirectedPath> _path;
        /// <summary>Gets or sets the alpha component for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        public          byte         ShadeBrushAlpha { get; set; }
        /// <summary>Gets or sets the base color for the shading brush used by Field-of-View display to indicate non-visible hexes.</summary>
        public          Color        ShadeBrushColor { get; set; }
        /// <summary>Gets or sets whether to display the FIeld-of-View for <see cref="HotspotHex"/>.</summary>
        public          bool         ShowFov         { get; set; }
        /// <summary>Gets or sets whether to display the hexgrid.</summary>
        public          bool         ShowHexgrid     { get; set; }
        /// <summary>Gets or sets whether to display the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
        public          bool         ShowPath        { get; set; }
        /// <summary>Gets or sets whether to display direction indicators for the current path.</summary>
        public          bool         ShowPathArrow   { get; set; }
        /// <summary>Gets or sets whether to display the shortest path from <see cref="StartHex"/> to <see cref="GoalHex"/>.</summary>
        public          bool         ShowRangeLine {
            get => _showRangeLine;
            set { _showRangeLine = value; if (_showRangeLine) Fov = null; }
        } bool _showRangeLine = false;
        /// <inheritdoc/>
        public virtual  HexCoords    StartHex {
            get => _startHex;
            set { if (MapSizeHexes.IsOnboard(value)) _startHex = value; PathSet(); ; if (ShowRangeLine) _fov = null; }
        } HexCoords _startHex = HexCoords.EmptyUser;

        BoardStorage<Maybe<THex>> IMapDisplay<THex>.BoardHexes => BoardHexes;
        #endregion

        /// <summary>TODO</summary>
        public void PathSet()
        => _path = this[StartHex].Bind(source =>
                   this[GoalHex].Bind(target => source.Range(target) <= RangeCutoff
                        ? new StandardPathfinder(this).GetPath(source,target)
                        : new BidirectionalAltPathfinder(this).GetPath(source,target)
                   ));

        /// <summary>TODO</summary>
        public void PathClear() => _path = Maybe<IDirectedPath>.NoValue();

        #region Painting
        /// <inheritdoc/>
        public CoordsRectangle GetClipInHexes(HexPointF point, HexSizeF size)
        => this.GetClipInHexes(new RectangleF(point, size), MapSizeHexes);

        /// <inheritdoc/>
        public CoordsRectangle GetClipInHexes(RectangleF visibleClipBounds)
        => this.GetClipInHexes(visibleClipBounds, MapSizeHexes);
        #endregion

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "2*range")]
        public override short? Heuristic(HexCoords source, HexCoords target)
        => (short)(2 * source.Range(target));

        /// <summary>TODO</summary>
        private void Host_FovRadiusChanged(object sender, Int32ValueEventArgs e)
        => FovRadius = RangeCutoff = e.Value;

        /// <summary>TODO</summary>
        private void Host_RangeCutoffChanged(object sender, Int32ValueEventArgs e)
        => RangeCutoff = e.Value;

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        Maybe<THex> IMapDisplayWinForms<THex>.this[HexCoords coords]
        => BoardHexes[coords];

        #region Derived IDisposable implementation
        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;
        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    HexgridPath?.Dispose();
                }
                _isDisposed = true;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
