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
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1587
/// <summary>Extensions to the System.Windows.Forms technologies used by namespace PGNapoleonics.HexgridPanel.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexgridPanel.WinForms {
    /// <summary>Extern declarations from the Win32 API.</summary>
    internal static class NativeMethods {
        /// <summary>P/Invoke declaration for user32.dll.WindowFromPoint</summary>
		    /// <remarks><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633558(vector=vs.85).aspx"></a></remarks>
		    /// <param name="point">(Sign-extended) screen coordinates as a Point structure.</param>
		    /// <returns>Window handle (hWnd).</returns>
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0",
          Justification="Research suggests the Code Analysis message is incorrect.")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point point);

        /// <summary>Performs A Bit-Block-Transfer (ie BitBlt) of the color data corresponding to a rectangle 
        /// of pixels from the specified source device context into a destination device context.</summary>
        /// <param name="hDC">A handle to the destination device context.</param>
        /// <param name="x">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="y">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
        /// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
        /// <param name="hSrcDC">A handle to the source device context.</param>
        /// <param name="xSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
        /// <param name="ySrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
        /// <param name="dwRop">The raster-operation code (<see cref="GdiRasterOps"/>) specifying how to combine color data in the source with color data in the destination.</param>
        [DllImport("Gdi32.dll", SetLastError=true, ExactSpelling=true, CharSet=CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, 
                                         HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);

        /// <summary>Message Cracker for HiWord</summary>
        /// <param name="ptr">A Windows message IntPtr</param>
        /// <returns>Most significant 16 bits of <c>ptr</c> as Int32.</returns>
        public static int HiWord(this IntPtr ptr)
        => unchecked((short)unchecked((int)(((ulong)ptr & 0xFFFFFFFFFFFF0000UL) >> 16)));

        /// <summary>Message Cracker for LoWord</summary>
        /// <param name="ptr">A Windows message IntPtr</param>
        /// <returns>Least significant 16 bits of <c>ptr</c> as Int32.</returns>
        public static int LoWord(this IntPtr ptr)
        =>  unchecked((short)(int)((ulong)ptr & 0xFFFFUL));

        /// <summary>P/Invoke declaration for user32.dll.SendMessage</summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="msg">Windows message</param>
        /// <param name="wParam">WParam</param>
        /// <param name="lParam">LParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
