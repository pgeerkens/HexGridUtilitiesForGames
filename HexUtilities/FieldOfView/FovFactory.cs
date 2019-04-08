#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>Extension methods for interface IFovBoard {IHex}.</summary>
    public static class FovFactory {
        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IShadingMask> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin,
                int fovRadius)
        => Task.Run(() => @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IShadingMask> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin,
                int fovRadius, int height)
        => Task.Run(() => @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IShadingMask> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin,
                int fovRadius, FovTargetMode targetMode)
        => Task.Run(() => @this.GetFieldOfView(origin, fovRadius, targetMode, 1, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IShadingMask> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin,
                int fovRadius, FovTargetMode targetMode, int height)
        => Task.Run(() => @this.GetFieldOfView(origin, fovRadius, targetMode, height, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously.</summary>
        public static Task<IShadingMask> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin,
                int fovRadius, FovTargetMode targetMode, int height, int hexesPerMile)
        => Task.Run(() => @this.GetFieldOfView(origin, fovRadius, targetMode, height, hexesPerMile));

        /// <summary>Gets a Field-of-View for this board synchronously, assuming a flat earth.</summary>
        public static IShadingMask GetFieldOfView(this IFovBoard @this, HexCoords origin,
                int fovRadius)
        => @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0);

        /// <summary>Gets a Field-of-View for this board synchronously.</summary>
        public static IShadingMask GetFieldOfView(this IFovBoard @this, HexCoords origin,
                int fovRadius, FovTargetMode targetMode, int heightOfMan, int hexesPerMile) {
            Tracing.FieldOfView.Trace("GetFieldOfView");
            var fov = new ArrayFieldOfView(@this);
            if (@this.IsOverseeable(origin))
                ShadowCasting.ComputeFieldOfView(@this, origin, fovRadius, targetMode,
                        coords => fov[coords] = true, heightOfMan, hexesPerMile);

            return fov;
        }
    }
}
