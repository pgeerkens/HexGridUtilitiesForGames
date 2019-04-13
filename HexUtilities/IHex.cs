#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Drawing.Drawing2D;

namespace PGNapoleonics.HexUtilities {
    /// <summary>External interface exposed by individual hexes.</summary>
    public interface IHex {
        /// <summary>The <c>HexCoords</c> coordinates for this hex on <c>Board</c>.</summary>
        HexCoords Coords         { get; }

        /// <summary>Elevation of this hex in "steps" above the minimum elevation of the board.</summary>
        int       ElevationLevel { get; }

        /// <summary>Height ASL in <i>game units</i> of observer's eyes for FOV calculations.</summary>
        int       HeightObserver { get; }

        /// <summary>Height ASL in <i>game units</i> of target above ground level to be spotted.</summary>
        int       HeightTarget   { get; }

        /// <summary>Height ASL in <i>game units</i> of any blocking terrian in this hex.</summary>
        int       HeightTerrain  { get; }
 
        /// <summary>Char code for the tYpe of the terrain in this hex.</summary>
        char      TerrainType    { get; }

        /// <summary>Returns true exactly when thhis hex is passable.</summary>
        bool      IsPassable     { get; }

        /// <summary>Cost to extend the path with the hex located across the <c>Hexside</c> at <c>direction</c>.</summary>
        int       EntryCost(Hexside hexsideExit);

        /// <summary>Cost to extend the path with the hex located across the <c>Hexside</c> at <c>direction</c>.</summary>
        int       ExitCost(Hexside hexsideExit);

        /// <summary>Height ASL in <i>game units</i> of any blocking terrain in this hex and the specified Hexside.</summary>
        int       HeightHexside(Hexside hexside);
    }
}
namespace PGNapoleonics.HexUtilities{
    using HexPoint   = System.Drawing.Point;
    using HexPointF  = System.Drawing.PointF;
    using HexSize    = System.Drawing.Size;
    using HexSizeF   = System.Drawing.SizeF;
    using RectangleF = System.Drawing.RectangleF;

    using CoordsRect = CoordsRectangle;
    using ILandmarks = Pathfinding.ILandmarkCollection;
    using IPath      = Maybe<Pathfinding.IDirectedPathCollection>;
    using IFovBoard  = FieldOfView.IFovBoard;
    /// <summary>.</summary>
    public interface IPanelModel : IFovBoard {
        /// <summary>.</summary>
        new Maybe<IHex>  this[HexCoords coords] { get; }
        /// <summary>.</summary>
        IShadingMask Fov            { get;}
        /// <summary>.</summary>
        int         FovRadius       { get; set; }
        /// <summary>.</summary>
        HexCoords   GoalHex         { get; set; }
        /// <summary>.</summary>
        HexSize     GridSize        { get; }
        /// <summary>.</summary>
        IntMatrix2D GridSizePixels  { get; }
        /// <summary>.</summary>
        HexSize     HexCentreOffset { get; }
        /// <summary>.</summary>
        GraphicsPath HexgridPath    { get; }
        /// <summary>.</summary>
        HexCoords   HotspotHex      { get; set; }
        /// <summary>.</summary>
        ILandmarks  Landmarks       { get; }
        /// <summary>.</summary>
        int         LandmarkToShow  { get; set; }
        /// <summary>.</summary>
        float       MapScale        { get; set; }
        ///// <summary>.</summary>
        //HexSize      MapSizeHexes   { get; }
        /// <summary>.</summary>
        string      Name            { get; }
        /// <summary>.</summary>
        IPath       Path            { get; }
        /// <summary>.</summary>
        int         RangeCutoff     { get; set; }
        /// <summary>.</summary>
        byte        ShadeBrushAlpha { get; }
        /// <summary>.</summary>
        bool        ShowFov         { get; set; }
        /// <summary>.</summary>
        bool        ShowPathArrow   { get; set; }
        /// <summary>.</summary>
        bool        ShowRangeLine   { get; set; }
        /// <summary>.</summary>
        HexCoords   StartHex        { get; set; }    

        ///// <summary>.</summary>
        //int  ElevationGroundASL(HexCoords coords);
        ///// <summary>.</summary>
        //int  ElevationObserverASL(HexCoords coords);
        ///// <summary>.</summary>
        //int  ElevationTargetASL(HexCoords coords);

        /// <summary>.</summary>
        void ForEachHexSerial<THex>(Action<Maybe<THex>> action) where THex:class,IHex;

        ///// <summary>.</summary>
        //CoordsRect  GetClipInHexes(RectangleF clip);
        ///// <summary>.</summary>
        //CoordsRect  GetClipInHexes(HexPointF point, HexSizeF size);
    }
}
