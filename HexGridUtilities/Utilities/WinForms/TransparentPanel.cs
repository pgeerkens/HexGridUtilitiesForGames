#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms {
	/// <summary> Transparent Panel control.</summary>
	/// <remarks>
	/// <see cref="http://componentfactory.blogspot.ca/2005/06/net2-transparent-controls.html"/>
	/// <see cref="http://www.bobpowell.net/transcontrols.htm"/>
	/// </remarks>
	public class TransparentPanel : Panel {
		public TransparentPanel() : base() {
			SetStyle(ControlStyles.SupportsTransparentBackColor,true);
			BackColor  = Color.Transparent;
		}
		/// <summary>Make a truly transparent Panel control.</summary>
		/// <remarks>Change the behaviour of the window by giving it a WS_EX_TRANSPARENT style.
		/// <see cref="http://www.bobpowell.net/transcontrols.htm"/></remarks>
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
		/// <see cref="http://www.bobpowell.net/transcontrols.htm"/></remarks>
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
