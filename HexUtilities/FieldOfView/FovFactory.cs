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
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>Extension methods for interface IFovBoard {IHex}.</summary>
    public static class FovFactory {
        /// <summary>Returns the field-of-view on <c>board</c> from the hex specified by coordinates <c>coords</c>.</summary>
        [Obsolete("Use GetFieldOfView(HexCoords origin, int fovRadius) instead.")]
        public static IFov GetFov(this IFovBoard @this, HexCoords origin, int fovRadius) {
            return @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0);
        }

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IFov> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin, int fovRadius) =>
            //@this.GetFieldOfViewAsync(origin, fovRadius, 1);
            Task.Run(() => @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IFov> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin, int fovRadius, int height) =>
            //@this.GetFieldOfViewAsync(origin, fovRadius, FovTargetMode.EqualHeights, height);
            Task.Run(() => @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IFov> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin, int fovRadius, FovTargetMode targetMode) =>
            //@this.GetFieldOfViewAsync(origin, fovRadius, targetMode, 1);
            Task.Run(() => @this.GetFieldOfView(origin, fovRadius, targetMode, 1, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously, assuming a flat earth.</summary>
        public static Task<IFov> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin, int fovRadius, FovTargetMode targetMode, int height) =>
            Task.Run(() => @this.GetFieldOfView(origin, fovRadius, targetMode, height, 0));

        /// <summary>Gets a Field-of-View for this board asynchronously.</summary>
        public static Task<IFov> GetFieldOfViewAsync(this IFovBoard @this, HexCoords origin, int fovRadius, FovTargetMode targetMode, int height, int hexesPerMile) =>
            Task.Run(() => @this.GetFieldOfView(origin, fovRadius, targetMode, height, hexesPerMile));

        /// <summary>Gets a Field-of-View for this board synchronously, assuming a flat earth.</summary>
        public static IFov GetFieldOfView(this IFovBoard @this, HexCoords origin, int fovRadius) {

            return @this.GetFieldOfView(origin, fovRadius, FovTargetMode.EqualHeights, 1, 0);
        }
        /// <summary>Gets a Field-of-View for this board synchronously.</summary>
        public static IFov GetFieldOfView(this IFovBoard @this, HexCoords origin, int fovRadius, FovTargetMode targetMode, int heightOfMan, int hexesPerMile) {
            Tracing.FieldOfView.Trace("GetFieldOfView");
            var fov = new ArrayFieldOfView(@this);
            if (@this.IsOverseeable(origin))
                ShadowCasting.ComputeFieldOfView(@this, origin, fovRadius, targetMode, coords => fov[coords] = true, heightOfMan, hexesPerMile);

            return fov;
        }
    }
}
