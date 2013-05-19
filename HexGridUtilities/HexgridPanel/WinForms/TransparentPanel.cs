#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms {
	/// <summary> Transparent Panel control.</summary>
	/// <remarks>
	/// See "http://componentfactory.blogspot.ca/2005/06/net2-transparent-controls.html"
	/// See "http://www.bobpowell.net/transcontrols.htm"
	/// </remarks>
	public class TransparentPanel : Panel {
		public TransparentPanel() : base() {
			SetStyle(ControlStyles.SupportsTransparentBackColor,true);
			BackColor  = Color.Transparent;
		}
		/// <summary>Make a truly transparent Panel control.</summary>
		/// <remarks>Change the behaviour of the window by giving it a WS_EX_TRANSPARENT style.
		/// See "http://www.bobpowell.net/transcontrols.htm"</remarks>
		protected override CreateParams CreateParams { 
			get { 
				var cp=base.CreateParams; 
				cp.ExStyle |= (int)WindowStylesEx.WS_EX_TRANSPARENT;
				return cp; 
			} 
		}

		/// <summary> Invalidate entire parent control to redraw on our background.</summary>
		/// <remarks>Invalidate the parent of the control, not the control itself, whenever 
		/// we need to update the graphics. This ensures that whatever is behind the control 
		/// gets painted before we need to do our own graphics output.
		/// See "http://www.bobpowell.net/transcontrols.htm"</remarks>
		public virtual void InvalidateEx() { 
			InvalidateEx(new Rectangle(this.Location,this.Size));
		} 
		///<summary><inheritdoc cref="InvalidateEx()" /></summary>
		/// <param name="r"><c>Rectangle</c> to be invalidated.</param>
		public virtual void InvalidateEx(Rectangle r) { 
			if(Parent!=null  &&  Parent.IsHandleCreated) {
				try {
					Parent.Invoke((Action<Rectangle,bool>)((rc,b) => Parent.Invalidate(rc,b)), r,true); 
				} catch (InvalidOperationException e) { 
					MessageBox.Show("Why is " + e.Message + "\n occurring in\n" +
						"TransparentPanel.InvalidateEx(Rectangle r).");
				}
			}
		} 
		/// <summary> Prevent background painting from overwriting transparent background</summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent) { /* NO-OP */ } 
	}
}
