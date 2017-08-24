Let's change our Custom Coordinate system to move the origin from the upper-left to the lower-left. This requires the folloing code to be added t the bottom of the **HexGridExampleForm** constructor (or alternatively, in the **HexgridPanel** EndInit() method):

{{
      var matrix = new IntMatrix2D(2,0, 0,-2, 0,2*BoardSizeHexes.Height-1);
      Coords.SetCustomMatrices(matrix,matrix);
}}
The correct implementation of this transformation can be evidenced by modifying **HexGridExampleForm.hexgridPanel_MouseMove** to the following:

{{
    void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
      HotSpotHex = hexgridPanel.GetHexCoords(e.Location).User;
      Range = CurrentHex.Range(HotSpotHex);
      statusLabel.Text = "HotHex: " + HotSpotHex.ToString() 
                       + "/" + HotSpotHex.Canon.Custom.ToString()
                       + "; Range = " + Range
                       + "; Path Length = " + (Path==null ? 0 : Path.TotalCost);
    }
}}
Note the factor of 2 from what might be the expected transformation matrix. This is due to the odd-even assymmetry in the jagged axis of a rectangular coordinate system, on a hex grid, and is accounted for by the _divide-by-2_ that occurs at the end of a [coordinate transformation.](http://hexgridutilities.codeplex.com/wikipage?title=Coordinate%20Transformations%20Between%20Canonical%2c%20User%2c%20and%20Custom&referringTitle=Documentation).

Both of these changes are now present in Version 1.2, released Feb 22, 2013.