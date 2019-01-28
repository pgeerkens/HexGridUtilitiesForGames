#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexUtilities.Storage;

namespace PGNapoleonics.HexUtilities {
    using HexPoint   = System.Drawing.Point;
    using HexSize    = System.Drawing.Size;
    using RectangleF = System.Drawing.RectangleF;

    using ILandmarks  = ILandmarkCollection;
    using CoordsRect  = CoordsRectangle;

    /// <summary>Abstract implementation of a hexgrid map-board.</summary>
    /// <typeparam name="THex">TODO</typeparam>
    /// <typeparam name="TPath">TODO</typeparam>
    /// <remarks>No Finalizer is implemented as the class possesses no unmanaged resources.</remarks>
    public abstract class HexBoard<THex,TPath> : ILandmarkBoard, IFovBoard,
    IForEachable<Maybe<THex>>, IDisposable
    where THex : IHex {
        /// <summary>By default, landmark all four corners and midpoints of all 4 sides.</summary>
        /// <remarks>Pre-processing time on start-up can be reduced by decreasing the number of landmarks,
        /// though at the possible expense of longer path-finding times.</remarks>
        /// <param name="size"></param>
        protected static IFastList<HexCoords> DefaultLandmarks(HexSize size) {
          //  Contract.Ensures(Contract.Result<IFastList<HexCoords>>() != null);
            return ( from point in new HexPoint[] { new HexPoint(           0,             0),  // top-left
                                                    new HexPoint(           0, size.Height/2),  // middle-left
                                                    new HexPoint(           0, size.Height-1),  // bottom-left
                                                    new HexPoint(size.Width/2, size.Height-1),  // bottom-centre
                                                    new HexPoint(size.Width-1, size.Height-1),  // bottom-right
                                                    new HexPoint(size.Width-1, size.Height/2),  // middle-right
                                                    new HexPoint(size.Width-1,             0),  // top-right
                                                    new HexPoint(size.Width/2,             0),  // top-centre
                                                  }
                     select HexCoords.NewUserCoords(point)
                   ).ToArray().ToFastList();
        }

        /// <summary>Signals completion of a ResetLandmarks request.</summary>
        public event EventHandler<ValueEventArgs<ILandmarks>> LandmarksReady;

        #region Constructors
        /// <summary>Initializes the internal contents of <see cref="HexBoard{THex,TPath}"/> with landmarks as specified for pathfinding.</summary>
        /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a <see cref="System.Drawing.Size"/>.</param>
        /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a <see cref="System.Drawing.Size"/>.</param>
        /// <param name="boardHexes">TODO</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected HexBoard(HexSize sizeHexes, HexSize gridSize, BoardStorage<Maybe<THex>> boardHexes) {
            boardHexes.RequiredNotNull("boardHexes");
          //  Contract.Requires(gridSize.Width  > 0);
          //  Contract.Requires(gridSize.Height > 0);

            MapScale        = 1.00F;
            IsTransposed    = false;
            MapSizeHexes    = sizeHexes;
            GridSize        = gridSize;
            BoardHexes      = boardHexes;

            EntryCosts = new BlockedBoardStorage32x32<Maybe<HexsideCosts>>(sizeHexes, 
                                    hexCoords => HexsideCosts.EntryCosts(boardHexes,hexCoords), 1);
            ExitCosts  = new BlockedBoardStorage32x32<Maybe<HexsideCosts>>(sizeHexes, 
                                    hexCoords => HexsideCosts.ExitCosts(boardHexes,hexCoords), 1);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void ObjectInvariant() {
          //  Contract.Invariant(GridSize.Width  > 0);
          //  Contract.Invariant(GridSize.Height > 0);
          //  Contract.Invariant(BoardHexes      != null);
        }

        /// <summary>TODO</summary>
        /// <param name="landmarkCoords"><see cref="IFastList{HexCoords}"/> of the hexes to be used as Path-Finding landmarks.</param>
        /// <returns></returns>
        protected async   Task<bool> ResetLandmarksAsync(IFastList<HexCoords> landmarkCoords) {
            landmarkCoords.RequiredNotNull("landmarkCoords");
            return await Task.Run(() => ResetLandmarks(landmarkCoords));
        }
        /// <summary>TODO</summary>
        /// <param name="landmarkCoords"><see cref="IFastList{HexCoords}"/> of the hexes to be used as Path-Finding landmarks.</param>
        /// <returns></returns>
        protected         bool       ResetLandmarks(IFastList<HexCoords> landmarkCoords) { 
            landmarkCoords.RequiredNotNull("landmarkCoords");

            Landmarks = LandmarkCollection.New(NavigableBoard, landmarkCoords);
            OnLandmarksReady(new ValueEventArgs<ILandmarks>(Landmarks));
            return true;
        }

        /// <inheritdoc/>
        protected virtual void OnLandmarksReady(ValueEventArgs<ILandmarks> e) => LandmarksReady.Raise(this,e);
        #endregion

        #region Properties & Fields
        /// <summary>Offset of hex centre from upper-left corner, as a <see cref="HexSize"/> struct.</summary>
        public             HexSize    CentreOfHexOffset => new HexSize(GridSize.Width * 2/3, GridSize.Height /2);
        /// <summary>TODO </summary>
        protected abstract int        ElevationBase     { get; } //!< Height in units of elevation level 0 (zero).
        /// <summary>TODO </summary>
        protected abstract int        ElevationStep     { get; } //!< Height increase in units of each elevation level.
        /// <inheritdoc/>
        public virtual     int        FovRadius         { get; set; }
        /// <summary>TODO </summary>
        public    virtual  int        HeightOfMan       => 1;   //!< Height in metres.
        /// <inheritdoc/>
        public             IHexgrid   Hexgrid           { 
          get {
          //  Contract.Ensures(Contract.Result<IHexgrid>() != null);
            return TransposableHexgrid.GetNewGrid(IsTransposed,GridSize,MapScale);
          }
        }
        /// <summary>Gets the extent in pixels of the grid on which hexes are to be laid out. </summary>
        public             HexSize    GridSize          { get; }
         ///  <inheritdoc/>
        public             bool       IsTransposed      { get; set; }
        /// <inheritdoc/>
        public             ILandmarks Landmarks         { get; private set; }
        ///  <inheritdoc/>
        public             float      MapScale          { get; set; }
        /// <summary>The dimensions of the board as a <see cref="System.Drawing.Size"/></summary>
        public             HexSize    MapSizeHexes      { get; }
        ///  <inheritdoc/>
        public             HexSize    MapSizePixels     => 
          MapSizeHexes * new IntMatrix2D(GridSize.Width,                0, 
                                                      0,   GridSize.Height, 
                                       GridSize.Width/3, GridSize.Height/2);
        
        /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
        public             int        RangeCutoff       { get; set; }
        /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public             Maybe<THex>  this[HexCoords coords] => BoardHexes[coords];
                  Maybe<IHex> IFovBoard.this[HexCoords coords] => this[coords].Bind(v => new Maybe<IHex>(v));
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected  BoardStorage<Maybe<THex>> BoardHexes        { get; }
        #endregion

        #region Methods
        private          int  ElevationASL(IHex hex) {
          hex.RequiredNotNull("hex");
          return ElevationBase + hex.ElevationLevel * ElevationStep;
        }
        /// <inheritdoc/>
        public           int  ElevationGroundASL(HexCoords coords) =>
            this[coords].Match(hex => ElevationASL(hex), () => int.MaxValue);
        /// <inheritdoc/>
        public           int  ElevationHexsideASL(HexCoords coords, Hexside hexside) =>
            this[coords].Match(hex => ElevationASL(hex) + hex.HeightHexside(hexside), () => int.MaxValue);
        /// <inheritdoc/>
        public           int  ElevationTargetASL(HexCoords coords) =>
            this[coords].Match(hex => ElevationASL(hex) + HeightOfMan, () => int.MaxValue);
        /// <inheritdoc/>
        public           int  ElevationObserverASL(HexCoords coords) =>
            this[coords].Match(hex => ElevationASL(hex) + HeightOfMan, () => int.MaxValue);
        /// <inheritdoc/>
        public           int  ElevationTerrainASL(HexCoords coords) =>
            this[coords].Match(hex => ElevationASL(hex) + hex.HeightTerrain, () => int.MaxValue);

        /// <summary>Perform <paramref name="action"/> for all neighbours of <paramref name="coords"/>.</summary>
        /// <param name="coords"></param>
        /// <param name="action"></param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public           void ForAllNeighbours(HexCoords coords, Action<Maybe<THex>,Hexside> action) {
            action.RequiredNotNull("action");
            BoardHexes.ForAllNeighbours(coords,action);
        }

        /// <inheritdoc/>
        public           void ForEach(Action<Maybe<THex>> action) => BoardHexes.ForEach(action);

        /// <summary>TODO</summary>
        /// <param name="action"></param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public           void ForEachHex(Action<Maybe<THex>> action) {
            action.RequiredNotNull("action");
            BoardHexes.ForEachSerial(action);
        }

        /// <summary>Returns the location and extent in hexes, as a <see cref="CoordsRectangle"/>, of the current clipping region.</summary>
        protected  CoordsRect GetClipInHexes(RectangleF visibleClipBounds, HexSize boardSizeHexes) {
            var left   = Math.Max((int)visibleClipBounds.Left   / GridSize.Width  - 1, 0);
            var top    = Math.Max((int)visibleClipBounds.Top    / GridSize.Height - 1, 0);
            var right  = Math.Min((int)visibleClipBounds.Right  / GridSize.Width  + 1, boardSizeHexes.Width);
            var bottom = Math.Min((int)visibleClipBounds.Bottom / GridSize.Height + 1, boardSizeHexes.Height); 
            return CoordsRectangle.New(left, top, right-left, bottom-top);
        }
    
        /// <inheritdoc/>
        public abstract  short? Heuristic(HexCoords source, HexCoords target);

        /// <inheritdoc/>
        public virtual   bool   IsOverseeable(HexCoords coords) => MapSizeHexes.IsOnboard(coords);

        /// <inheritdoc/>
        public           Maybe<THex> Neighbour(HexCoords coords, Hexside hexside) =>
            BoardHexes.Neighbour(coords,hexside);
        #endregion

        /// <summary>TODO</summary>
        public short? TryExitCost(HexCoords hexCoords, Hexside hexside) =>
            (from x in ExitCosts[hexCoords] from c in x[hexside].ToMaybe() select c).ToNullable();

        /// <summary>TODO</summary>
        public short? TryEntryCost(HexCoords hexCoords, Hexside hexside) =>
            (from x in EntryCosts[hexCoords] from c in x[hexside].ToMaybe() select c).ToNullable();

        /// <summary>TODO</summary>
        protected readonly IBoardStorage<Maybe<HexsideCosts>> EntryCosts;
        /// <summary>TODO</summary>
        protected readonly IBoardStorage<Maybe<HexsideCosts>> ExitCosts;

        /// <summary>TODO</summary>
        protected virtual int MinimumStepCost => 2;

        /// <summary>TODO</summary>
        public virtual   INavigableBoard NavigableBoard =>
            new NavigableBoard(MapSizeHexes, EntryCosts, ExitCosts, MinimumStepCost);

        #region IDisposable implementation (w/o Finalizer as the class possesses no unmanaged resources.)
        /// <summary>Clean up any resources being used, and suppress finalization.</summary>
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        /// <summary>True if already Disposed.</summary>
        private bool _isDisposed = false;
        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            if (! _isDisposed) {
                if (disposing) {
                  Landmarks?.Dispose();   Landmarks   = null;
                  BoardHexes.Dispose();
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
