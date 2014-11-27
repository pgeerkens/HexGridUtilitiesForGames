#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.WinForms;

namespace PGNapoleonics.WinForms {
  using System;
  using System.Runtime.ConstrainedExecution;

  /// <summary>TODO</summary>
  public static partial class BufferedGraphicsExtensions {
    /// <summary>TODO</summary>
    /// <param name="this"></param>
    /// <param name="target"></param>
    /// <param name="scrollPosition"></param>
    /// <param name="virtualSize"></param>
    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)] 
    public static void Render(this BufferedGraphics @this, 
      Graphics target, 
      Point    scrollPosition, 
      Size     virtualSize
    ) {
      if (@this==null) throw new ArgumentNullException("this");

      if (target != null) {
        using (var targetDC = new GraphicsDeviceContext(target))
        using (var sourceDC = new GraphicsDeviceContext(@this.Graphics)) {
          NativeMethods.BitBlt(
                  targetDC.HandleRef,
                  scrollPosition.X,  scrollPosition.Y, 
                  virtualSize.Width, virtualSize.Height, 
                  sourceDC.HandleRef,
                                 0,                 0, 
                  GdiRasterOps.SrcCopy);
        }
      }
    }

    private class GraphicsDeviceContext : SafeHandle {
      public GraphicsDeviceContext(Graphics graphics) : base((IntPtr)0,true) {
        _graphics = graphics;
        handle    = graphics.GetHdc(); 
      }

      private Graphics  _graphics;

      public  HandleRef HandleRef { get {  return new HandleRef(_graphics,handle); } }

      public override bool IsInvalid { get { return handle == (IntPtr)0;; } }

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      override protected bool ReleaseHandle() {
        _graphics.ReleaseHdc(handle);
        return true;
      }
    }
  }
}
