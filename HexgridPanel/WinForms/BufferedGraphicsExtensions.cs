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
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace PGNapoleonics.WinForms {
  /// <summary>TODO</summary>
  public static partial class BufferedGraphicsExtensions {
    /// <summary>Performs a SrcCopy BitBLt from <paramref name="source"/> to <paramref name="target"/> of the rectangle <paramref name="clientRectangle"/>.</summary>
    /// <param name="source">BufferedGraphics source for the BitBlt.</param>
    /// <param name="target">Graphics target for the BitBlt.</param>
    /// <param name="clientRectangle">Rectangle in pixels in <paramref name="source"/> to BitBlt.</param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    public static void Render(this BufferedGraphics source,
      Graphics     target,
      Rectangle    clientRectangle
    ) { source.Render(target, clientRectangle.Location, clientRectangle.Size, GdiRasterOps.SrcCopy); }

    /// <summary>Performs a <paramref name="rasterOp"/> BitBLt from <paramref name="source"/> to <paramref name="target"/> of the rectangle <paramref name="clientRectangle"/>.</summary>
    /// <param name="source">BufferedGraphics source for the BitBlt.</param>
    /// <param name="target">Graphics target for the BitBlt.</param>
    /// <param name="clientRectangle">Rectangle in pixels in <paramref name="source"/> to BitBlt.</param>
    /// <param name="rasterOp">The raster-operation to be performed during the BitBlt.</param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    internal static void Render(this BufferedGraphics source,
      Graphics     target,
      Rectangle    clientRectangle,
      GdiRasterOps rasterOp
    ) { source.Render(target, clientRectangle.Location, clientRectangle.Size, rasterOp); }

    /// <summary>Performs a <paramref name="rasterOp"/> BitBLt from <paramref name="source"/> to <paramref name="target"/> of the rectangle at <paramref name="scrollPosition"/> with dimensions <paramref name="virtualSize"/>.</summary>
    /// <param name="source">BufferedGraphics source for the BitBlt.</param>
    /// <param name="target">Graphics target for the BitBlt.</param>
    /// <param name="scrollPosition">Location in pixels in <paramref name="source"/> of the rectangle to BitBlt.</param>
    /// <param name="virtualSize">Size in pixels of the rectangle to BitBlt.</param>
    /// <param name="rasterOp">The raster-operation to be performed during the BitBlt.</param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    internal static void Render(this BufferedGraphics source, 
      Graphics     target, 
      Point        scrollPosition, 
      Size         virtualSize,
      GdiRasterOps rasterOp
    ) {
      source.RequiredNotNull("source");

      if (target != null) {
        var targetDC = target.GetHdc();

        try {
          source.RenderInternal(new HandleRef(target,targetDC), scrollPosition, virtualSize, rasterOp);
        } finally {
          target.ReleaseHdc(targetDC);
        }
      }
    }

    /// <summary>Private method that renders the specified buffer into the target.</summary>
    /// <param name="source">BufferedGraphics source for the BitBlt.</param>
    /// <param name="targetDC">HandleRef for the Graphics target of the BitBlt.</param>
    /// <param name="scrollPosition">Location in pixels in <paramref name="source"/> of the rectangle to BitBlt.</param>
    /// <param name="virtualSize">Size in pixels of the rectangle to BitBlt.</param>
    /// <param name="rasterOp">The raster-operation to be performed during the BitBlt.</param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    private static void RenderInternal(this BufferedGraphics source, 
      HandleRef    targetDC, 
      Point        scrollPosition, 
      Size         virtualSize,
      GdiRasterOps rasterOp
    ) {
      source.RequiredNotNull("source");
     // targetDC.RequiredNotNull("targetDC");

      var sourceDC = source.Graphics.GetHdc();
      try {
        NativeMethods.BitBlt(
            targetDC,
            scrollPosition.X,  scrollPosition.Y, 
            virtualSize.Width, virtualSize.Height, 
            new HandleRef(source.Graphics, sourceDC),
                            0,                 0, 
            (int)rasterOp);
      } finally {
        source.Graphics.ReleaseHdc(sourceDC);
      }
    }
  }
}
