#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms {
	/// <summary>Enumeration for buttons and modifiers in Windows Mouse messages.</summary>
	[Flags]
	public enum MouseKeys : short {
		/// <summary>None.</summary>
		None		= 0x00,
		/// <summary>Left mouse button.</summary>
		LButton	= 0x01,
		/// <summary>Right mouse button.</summary>
		RButton	= 0x02,
		/// <summary>Shift key.</summary>
		Shift		= 0x04,
		/// <summary>Control key.</summary>
		Control	= 0x08,
		/// <summary>Middle mouse button.</summary>
		MButton	= 0x10,
		/// <summary>First mouse X button.</summary>
		XButton1	= 0x20,
		/// <summary>Second mouse X button.</summary>
		XButton2	= 0x40
	}
	public static class WindowsMouseInput {
		public static MouseKeys GetKeyStateWParam(IntPtr wParam) {
			return (MouseKeys)(wParam.ToInt64() & 0x0000ffff);
		}
		public static Int16 WheelDelta(IntPtr wParam) {
			return (Int16)(wParam.ToInt64() >> 16);
		}
		public static IntPtr WParam (Int16 wheelDelta, MouseKeys mouseKeys) {
			return IntPtr.Zero + (wheelDelta << 16) + (Int16)mouseKeys;
		}
		/// <summary> Determine (sign-extended for multiple monitors) screen coordinates at m.LParam.</summary>
		/// <param name="lParam"></param>
		/// <returns></returns>
		public static Point GetPointLParam(IntPtr lParam) {
			return new Point(
					 (int)(short)(lParam.ToInt64() & 0x0000ffff), 
					 (int)(short)(lParam.ToInt64() >> 16)
				);
		}
		public static IntPtr LParam(Point point) {
			if (point.X<Int16.MinValue || point.X > Int16.MaxValue)
				throw new ArgumentOutOfRangeException("point.X",point.X,
					"Must be a valid Int16 value.");
			if (point.Y<Int16.MinValue || point.Y > Int16.MaxValue)
				throw new ArgumentOutOfRangeException("point.Y",point.Y,
					"Must be a valid Int16 value.");
			return (IntPtr)((Int16)point.Y <<16 + (Int16)point.X);
		}
	}
}
