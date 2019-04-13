#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexPoint      = System.Drawing.Point;
    using HexPointF     = System.Drawing.PointF;
    using HexSize       = System.Drawing.Size;
    using HexSizeF      = System.Drawing.SizeF;
    using Matrix        = System.Drawing.Drawing2D.Matrix;
    using RectangleF    = System.Drawing.RectangleF;
    using GraphicsPath  = System.Drawing.Drawing2D.GraphicsPath;
    using IDirectedPath = IDirectedPathCollection;
    using Int32ValueEventArgs = EventArgs<int>;

    using static System.Drawing.Drawing2D.PathPointType;

    /// <summary>.</summary>
    /// <typeparam name="THex"></typeparam>
    public abstract class MapDisplayFlat<THex> : MapDisplay<THex>, IDisposable
    where THex:class,IHex {
        /// <summary>Creates a new instance of the MapDisplay class.</summary>
        protected MapDisplayFlat(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex)
        : base(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes),
              new FlatBoardStorage<Maybe<THex>>(sizeHexes, coords => initializeHex(coords), false)) { }
    }

    /// <summary>.</summary>
    /// <typeparam name="THex"></typeparam>
    public abstract class MapDisplayBlocked<THex> : MapDisplay<THex>
    where THex:class,IHex {
        /// <summary>Creates a new instance of the MapDisplay class.</summary>
        protected MapDisplayBlocked(HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex)
        : base(sizeHexes, gridSize, initializeHex, DefaultLandmarks(sizeHexes),
              BlockedBoardStorage.New32x32<Maybe<THex>>(sizeHexes, coords => initializeHex(coords))) { }
    }

    /// <summary>Abstract class representing the basic game board.</summary>
    /// <typeparam name="THex">Type of the hex for which a game board is desired.</typeparam>
    public abstract class MapDisplay<THex> : HexBoard<THex>, IMapDisplayWinForms<THex>,
        IFovBoard, IPanelModel
    where THex: class,IHex {

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
            GoalHex         =
            HotspotHex      =
            StartHex        = HexCoords.EmptyUser;
            ShadeBrushAlpha = 78;
            ShowFov         = true;
            ShowHexgrid     = true;
            ShowPath        = true;
            ShowPathArrow   = true;
            HexgridPath     = Extensions.InitializeDisposable(() =>
                     new GraphicsPath(HexgridPathPoints(gridSize), _hexgridPathPointTypes));

            LandmarkCoords  = landmarkCoords;
        }

        /// <inheritdoc/>
        protected override IFastList<HexCoords> LandmarkCoords { get; }

        #region Properties
        /// <summary>Gets or sets the Field-of-View for the current <see cref="HotspotHex"/>, as an <see cref="IFov"/> object.</summary>
        public virtual  IShadingMask Fov {
            get           => _fov ?? (_fov = this.GetFieldOfView(ShowRangeLine ? StartHex : HotspotHex, FovRadius));
            protected set => _fov = value;
        } IShadingMask _fov;
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

        Maybe<IHex> IPanelModel.this[HexCoords coords] => BoardHexes[coords].Bind<IHex>(hex => hex);
        #endregion

        /// <summary>TODO</summary>
        public void PathSet()
        => _path = this[StartHex].Bind(source =>
                   this[GoalHex].Bind(target => {
                       if(! source.IsPassable || ! target.IsPassable) return null;
                       return source.Range(target) <= RangeCutoff
                            ? this.GetPathStandardAStar(source, target).DirectedPath
                            : this.GetPathBiDiAlt(source,target).DirectedPath;
                   } ));

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
        public override int? Heuristic(HexCoords source, HexCoords target)
        => 2 * source.Range(target);

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

        /// <inheritdoc/>
        void IPanelModel.ForEachHexSerial<THex>(Action<Maybe<THex>> action)
        => BoardHexes.ForEachSerial(hex => action(hex.Bind<THex>(h => h as THex)));

        #region Derived IDisposable implementation
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;
        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    HexgridPath?.Dispose();
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
