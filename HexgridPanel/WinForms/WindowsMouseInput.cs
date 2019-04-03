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

namespace PGNapoleonics.HexgridPanel.WinForms {
    using Point = System.Drawing.Point;

	/// <summary>Enumeration for buttons and modifiers in Windows Mouse messages.</summary>
 [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	[Flags]public enum MouseKeys : short {
		/// <summary>None.</summary>
		None		= 0x00,
		/// <summary>Left mouse button.</summary>
		Lbutton	= 0x01,
		/// <summary>Right mouse button.</summary>
		Rbutton	= 0x02,
		/// <summary>Shift key.</summary>
		Shift		= 0x04,
		/// <summary>Control key.</summary>
		Control	= 0x08,
		/// <summary>Middle mouse button.</summary>
		Mbutton	= 0x10,
		/// <summary>First mouse X button.</summary>
		Xbutton1	= 0x20,
		/// <summary>Second mouse X button.</summary>
		Xbutton2	= 0x40
	}

    /// <summary>TODO</summary>
    public static class WindowsMouseInput {
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static MouseKeys GetKeyStateWParam(IntPtr wParam) {
			return (MouseKeys)(wParam.ToInt64() & 0x0000ffff);
		}

		/// <summary> Determine (sign-extended for multiple monitors) screen coordinates at m.LParam.</summary>
		/// <param name="lParam"></param>
		/// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static Point GetPointLParam(IntPtr lParam) {
			return new Point(
                     (short)(lParam.ToInt64() & 0x0000ffff),
                     (short)(lParam.ToInt64() >> 16)
                );
		}

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static short WheelDelta(IntPtr wParam) {
			return (short)(wParam.ToInt64() >> 16);
		}

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr LParam(Point point) {
			if (point.X<short.MinValue || point.X > Int16.MaxValue
            ||  point.Y<short.MinValue || point.Y > Int16.MaxValue)
			throw new ArgumentOutOfRangeException("point",point,
					"Must be a valid Point struct.");
			return (IntPtr)((Int16)point.Y <<16 + (Int16)point.X);
		}

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr WParam (short wheelDelta, MouseKeys mouseKeys) {
			return IntPtr.Zero + (wheelDelta << 16) + (short)mouseKeys;
		}
	}
}
