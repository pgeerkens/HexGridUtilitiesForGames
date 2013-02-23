#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.HexGridExample {
  public sealed partial class HexGridExampleForm : Form {
    public HexGridExampleForm() {
      InitializeComponent();

      hexgridPanel.Host = 
      MapBoard          = new MapDisplay();
      var matrix        = new IntMatrix2D(2,0, 0,-2, 0,2*MapBoard.SizeHexes.Height-1, 2);
      Coords.SetCustomMatrices(matrix,matrix);
    }
    protected override CreateParams CreateParams { 
			get { return this.SetCompositedStyle(base.CreateParams); }
		}

    IMapDisplay MapBoard     { get; set; }

    #region Event handlers
    void HexGridExampleForm_Load(object sender, EventArgs e) {
      hexgridPanel.ScaleIndex = 1; 

      Size = hexgridPanel.MapSizePixels + new Size(21,93);
    }

    void HexGridExampleForm_Resize(object sender, EventArgs e) { hexgridPanel.Invalidate(); }

    void HexGridExampleForm_ResizeEnd(object sender, EventArgs e) { hexgridPanel.SetScroll(); }

    void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
      var hotHex       = MapBoard.HotSpotHex;
      statusLabel.Text = "HotHex: " + hotHex.ToString() 
                       + "/" + hotHex.Canon.Custom.ToString()
                       + "; Range = " + MapBoard.CurrentHex.Range(hotHex)
                       + "; Path Length = " + (MapBoard.Path==null ? 0 : MapBoard.Path.TotalCost);
    }
    #endregion

    void buttonTransposeMap_Click(object sender, EventArgs e) {
      hexgridPanel.IsTransposed = buttonTransposeMap.Checked;
    }
  }
}
