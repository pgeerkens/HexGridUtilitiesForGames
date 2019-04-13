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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastList;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexUtilities.Storage {
    using HexPoint = System.Drawing.Point;
    using HexSize = System.Drawing.Size;

    using ILandmarks = ILandmarkCollection;
    using IBoardStorage = IBoardStorage<Maybe<HexsideCosts>>;

    /// <summary>Abstract implementation of a hexgrid map-board.</summary>
    /// <typeparam name="THex">TODO</typeparam>
    /// <remarks>No Finalizer is implemented as the class possesses no unmanaged resources.</remarks>
    public abstract class HexBoard<THex> : ILandmarkBoard, ILandmarkBoard<THex>, IFovBoard
    where THex:IHex {
        /// <summary>By default, landmark all four corners and midpoints of all 4 sides.</summary>
        /// <remarks>Pre-processing time on start-up can be reduced by decreasing the number of landmarks,
        /// though at the possible expense of longer path-finding times.</remarks>
        /// <param name="size"></param>
        protected static IFastList<HexCoords> DefaultLandmarks(HexSize size)
        => ( from point in new HexPoint[] { new HexPoint(           0,             0),  // top-left
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

        /// <summary>Signals completion of a ResetLandmarks request.</summary>
        public event EventHandler<EventArgs<ILandmarks>> LandmarksReady;

        #region Constructors
        /// <summary>Initializes the internal contents of <see cref="BoardStorage{T}"/> with landmarks as specified for pathfinding.</summary>
        /// <param name="sizeHexes">Extent in hexes of the board being initialized, as a <see cref="HexSize"/>.</param>
        /// <param name="gridSize">Extent in pixels of the layout grid for the hexagons, as a <see cref="HexSize"/>.</param>
        /// <param name="boardHexes">TODO</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected HexBoard(HexSize sizeHexes, HexSize gridSize, BoardStorage<Maybe<THex>> boardHexes) {
            BoardHexes      = boardHexes;
            MapScale        = 1.00F;
            IsTransposed    = false;
            MapSizeHexes    = sizeHexes;
            GridSize        = gridSize;
            HexCentreOffset = new HexSize(GridSize.Width * 2/3, GridSize.Height /2);
            GridSizePixels  = new IntMatrix2D(GridSize.Width,                0, 
                                                           0,   GridSize.Height, 
                                            GridSize.Width/3, GridSize.Height/2);

            EntryCosts = new BlockedBoardStorage32x32<Maybe<HexsideCosts>>(sizeHexes, 
                                    hexCoords => HexsideCosts.EntryCosts(boardHexes,hexCoords), 1);
            ExitCosts  = new BlockedBoardStorage32x32<Maybe<HexsideCosts>>(sizeHexes, 
                                    hexCoords => HexsideCosts.ExitCosts(boardHexes,hexCoords), 1);
        }

        /// <inheritdoc/>
        public async Task<Exception> ResetLandmarksAsync()
        => await Task.Run(() => ResetLandmarks(LandmarkCoords));

        /// <inheritdoc/>
        public void  ResetLandmarks() => ResetLandmarks(LandmarkCoords);

        /// <summary>TODO</summary>
        /// <param name="landmarkCoords"><see cref="IFastList{HexCoords}"/> of the hexes to be used as Path-Finding landmarks.</param>
        /// <returns></returns>
        protected Exception ResetLandmarks(IFastList<HexCoords> landmarkCoords) {
            try {
                #if true
                Landmarks = LandmarkCollection.New2(this, landmarkCoords);
                #else
                Landmarks = LandmarkCollection.New(this, landmarkCoords);
                #endif
                OnLandmarksReady(new EventArgs<ILandmarks>(Landmarks));
                return null;
            } 
            catch (Exception ex) { return ex; }
        }

        /// <inheritdoc/>
        protected virtual void OnLandmarksReady(EventArgs<ILandmarks> e) => LandmarksReady?.Invoke(this,e);
        #endregion

        /// <inheritdoc/>
        protected virtual IFastList<HexCoords> LandmarkCoords { get; }

        #region Properties & Fields
        /// <summary>TODO</summary>
        public BoardStorage<Maybe<THex>> BoardHexes { get; }
        /// <inheritdoc/>
        public    abstract int         ElevationBase   { get; }
        /// <inheritdoc/>
        public    abstract int         ElevationStep   { get; }
        /// <inheritdoc/>
        public    virtual  int         FovRadius       { get; set; }
        /// <summary>TODO </summary>
        public    virtual  int         HeightOfMan     => 1;   //!< Height in metres.
        /// <inheritdoc/>
        public             IHexgrid    Hexgrid         => new Hexgrid(IsTransposed,GridSize,MapScale);
        /// <summary>Gets the extent in pixels of the grid on which hexes are to be laid out. </summary>
        public             HexSize     GridSize        { get; }
        /// <summary>TODO</summary>
        public             IntMatrix2D GridSizePixels  { get; }
        /// <summary>Offset of hex centre from upper-left corner, as a <see cref="HexSize"/> struct.</summary>
        public             HexSize     HexCentreOffset { get; }
         ///  <inheritdoc/>
        public             bool        IsTransposed    { get; set; }
        /// <inheritdoc/>
        public             ILandmarks  Landmarks       { get; private set; }
        ///  <inheritdoc/>
        public             float       MapScale        { get; set; }
        /// <summary>The dimensions of the board as a <see cref="HexSize"/></summary>
        public             HexSize     MapSizeHexes    { get; }
        
        /// <summary>Range beyond which Fast PathFinding is used instead of Stable PathFinding.</summary>
        public             int         RangeCutoff     { get; set; }

        /// <summary>TODO</summary>
        protected virtual  int         MinimumStepCost => 2;
        /// <summary>TODO</summary>
        protected        IBoardStorage EntryCosts      { get; }
        /// <summary>TODO</summary>
        protected        IBoardStorage ExitCosts       { get; }

        THex INavigableBoard<THex>.this[HexCoords coords] => BoardHexes[coords].ElseDefault();

        /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public             Maybe<THex> this[HexCoords coords] => BoardHexes[coords];

        Maybe<IHex> IFovBoard.this[HexCoords coords] => this[coords].Bind(v => new Maybe<IHex>(v));
        #endregion

        #region Methods
        /// <summary>Perform <paramref name="action"/> for all neighbours of <paramref name="coords"/>.</summary>
        /// <param name="coords"></param>
        /// <param name="action"></param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public  void ForAllNeighbours(HexCoords coords, Action<Maybe<THex>,Hexside> action)
        => BoardHexes.ForAllNeighbours(coords,action);

        /// <inheritdoc/>
        public abstract  int? Heuristic(HexCoords source, HexCoords target);

        /// <inheritdoc/>
        public           Maybe<THex> Neighbour(HexCoords coords, Hexside hexside)
        => BoardHexes.Neighbour(coords,hexside);

        /// <inheritdoc/>
        public Maybe<IHex> Neighbour(IHex hex, Hexside hexside)
        => Neighbour(hex.Coords,hexside).Bind(h => (h as IHex).ToMaybe());

        /// <summary>TODO</summary>
        public int TryExitCost(HexCoords hexCoords, Hexside hexside)
        =>  (from x in ExitCosts[hexCoords] select x[hexside]).ElseDefault() ?? -1;

        /// <summary>TODO</summary>
        public int TryEntryCost(HexCoords hexCoords, Hexside hexside)
        => (from x in EntryCosts[hexCoords] select x[hexside]).ElseDefault() ?? -1;
        #endregion

        /// <inheritdoc/>
        public abstract int? Heuristic(int range);
        /// <inheritdoc/>
        public abstract int? Heuristic(IHex source,IHex target);
        /// <inheritdoc/>
        public bool IsOnboard(HexCoords coords) => this[coords].HasValue;
        /// <inheritdoc/>
        public void ForAllNeighbours(HexCoords coords,Action<THex,Hexside> action)
            => ForAllNeighbours(coords,(maybe,hexside) => maybe.IfHasValueDo(hex=>action(hex,hexside)));
        /// <inheritdoc/>
        public int  EntryCost(IHex hex,Hexside hexsideExit) => TryEntryCost(hex.Coords,hexsideExit);
        /// <inheritdoc/>
        public int  ExitCost(IHex hex,Hexside hexsideExit)  => TryExitCost(hex.Coords,hexsideExit);
        /// <inheritdoc/>
        public int  TryExitCost(IHex hex,Hexside hexside)  => TryExitCost(hex.Coords,hexside);
        /// <inheritdoc/>
        public int  TryEntryCost(IHex hex,Hexside hexside) => TryEntryCost(hex.Coords,hexside);
    }
}
