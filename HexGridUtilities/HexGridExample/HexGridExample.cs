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
  public sealed partial class HexGridExampleForm : Form, IMapPanelHost {
    public HexGridExampleForm() {
      InitializeComponent();

      hexgridPanel.Host = this;
      MapSizeMatrix      = new IntMatrix2D(GridSize.Width,                  0, 
                                                        0,    GridSize.Height, 
                                           GridSize.Width/3,GridSize.Height/2);

      HexgridPath = new GraphicsPath();
      HexgridPath.AddLines(new Point[] {
        new Point(GridSize.Width*1/3,                0), 
        new Point(GridSize.Width*3/3,                0),
        new Point(GridSize.Width*4/3,GridSize.Height/2),
        new Point(GridSize.Width*3/3,GridSize.Height  ),
        new Point(GridSize.Width*1/3,GridSize.Height  ),
        new Point(                 0,GridSize.Height/2),
        new Point(GridSize.Width*1/3,                0)
      });
      CurrentHex = Coords.EmptyUser;
      //Size = new Size(1525,1020);
    }
    protected override CreateParams CreateParams { 
			get { return this.SetCompositedStyle(base.CreateParams); }
		}

    #region IMapPanelHost
    public Size GridSize       { get {return new Size(27,30);} }
    public Size MapSizePixels  { get {return BoardSizeHexes * MapSizeMatrix;} }

    Rectangle  IMapPanelHost.GetClipCells(PointF point, SizeF size) {
      return GetClipCells( new RectangleF(point,size), BoardSizeHexes );
    }
    Rectangle  IMapPanelHost.GetClipCells(RectangleF visibleClipBounds) {
      return GetClipCells(visibleClipBounds, BoardSizeHexes);
    }

    void       IMapPanelHost.PaintHighlight(Graphics g) { 
      var state = g.Save();
      g.TranslateTransform(
        MapMargin.Width  + CurrentHex.X * GridSize.Width,
        MapMargin.Height + CurrentHex.Y * GridSize.Height + (CurrentHex.X+1)%2 * GridSize.Height/2
      );
      g.DrawPath(Pens.Red, HexgridPath);

      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        var path = Path;
        while (path != null) {
          g.Restore(state); state = g.Save();
          var step = path.LastStep.User;

          g.TranslateTransform(
            MapMargin.Width  + step.X * GridSize.Width,
            MapMargin.Height + step.Y * GridSize.Height + (step.X+1)%2 * GridSize.Height/2
          );
          g.FillPath(brush, HexgridPath);
          path = path.PreviousSteps;
        }
      }
    }
    void       IMapPanelHost.PaintMap(Graphics g) { 
      var clipCells = ((IMapPanelHost)this).GetClipCells(g.VisibleClipBounds);
      var state     = g.Save();
      var location  = new Point(GridSize.Width*2/3, GridSize.Height/2);

      g.TranslateTransform(MapMargin.Width + clipCells.Right*GridSize.Width, MapMargin.Height);

      using(var fillBrush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
      using(var font   = new Font("ArialNarrow", 8))
      using(var format = new StringFormat()) {
        format.Alignment = format.LineAlignment = StringAlignment.Center;
        for (int x=clipCells.Right; x-->clipCells.Left; ) {
          g.TranslateTransform(-GridSize.Width, 0);
          var container = g.BeginContainer();
          g.TranslateTransform(0,  clipCells.Top*GridSize.Height + (x+1)%2 * (GridSize.Height)/2);
          for (int y=clipCells.Top; y<clipCells.Bottom; y++) {
            var coords = Coords.NewUserCoords(x,y);
            if (this[coords]!='.') {
              g.FillPath(fillBrush, HexgridPath);
            }
            //g.DrawString(HexText(coords), font, Brushes.Gray, location, format);
            g.DrawPath(Pens.Black, HexgridPath);

            g.TranslateTransform(0,GridSize.Height);
          }
          g.EndContainer(container);
        }
      }
    }
    void       IMapPanelHost.PaintUnits(Graphics g) { ; }
    #endregion

    #region Board definition
    string[]     Board = new string[] {
      ".............|.........|.......|.........|.............",
      ".............|.........|.......|.........|.............",
      "....xxxxxxxxx|....|....|...|...|...|.....|.............",
      ".............|....|....|...|...|...|.....|.............",
      "xxxxxxxxx....|....|....|...|...|...|.....|.............",
      ".............|....|....|...|...|...|.....|.............",
      ".............|....|....|...|...|...|...................",
      ".....xxxxxxxx|....|........|.......|.......xxxxxxxx....",
      "..................|........|.......|.......|......|....",
      "xxxxxxxxx....xxxxxxxxxxxxxxxxxxxxxxxx......|......|....",
      "................|........|...|.............|...|..|....",
      "xxxxxxxxxxxx....|...|....|...|.............|...|...|...",
      "................|...|....|...|.............|...|...|...",
      "..xxxxxxxxxxxxxxx...|....|...|.............|...|...|...",
      "....................|....|.................|...|...|...",
      "xxxxxxxxxxxxxxxxx..xx....|.................|...|.......",
      ".........................|...xxxxxxxxxxxxxxx...|xxxxxxx",
      "xxxxxx...................|...|.................|.......",
      ".........xxxxxxxxxxxxxxxxx...|.................|.......",
      ".............................|...xxxxxxxxxxxxxx|.......",
      "xxxxxxxxxx......xxxxxxxxxxxxx|...|.....................",
      ".............................|...|.....................",
      ".............................|...|...xxxxxxxxxxxxx.....",
      "....xxxxxxxxxxxxxxxxxxxxxxxxx|...|...|.................",
      ".............................|...|...|..........xxxxxxx",
      "xxxxxxxxxxxxxxxxxxxxxxxxx....|...|...|.....|....|......",
      ".............................|...|...|.....|....|......",
      "...xxxxxxxxxxxxxxxxxxxxxxxxxx|...|...|.....|....|......",
      ".............................|.......|.....|...........",
      ".............................|.......|.....|..........."
    };
    #endregion
    Size                BoardSizeHexes { get {return new Size(Board[0].Length,Board.Length);} }
    GraphicsPath        HexgridPath    { get; set; }
    Size                MapMargin      { get {return hexgridPanel.MapMargin;} }
    float               MapScale       { get {return hexgridPanel.MapScale;} }
    IntMatrix2D         MapSizeMatrix  { get; set; }
    ICoordsUser         CurrentHex     { get; set; }
    ICoordsUser         HotSpotHex     { get; set; }
    IPath<ICoordsCanon> Path           { get; set; }
    int                 Range          { get; set; }

    Rectangle GetClipCells(RectangleF visibleClipBounds, Size boardSizeHexes) {
      var left    = Math.Max((int)visibleClipBounds.Left  /GridSize.Width  - 1, 0);
      var top     = Math.Max((int)visibleClipBounds.Top   /GridSize.Height - 1, 0);
      var right   = Math.Min((int)visibleClipBounds.Right /GridSize.Width  + 1, boardSizeHexes.Width);
      var bottom  = Math.Min((int)visibleClipBounds.Bottom/GridSize.Height + 1, boardSizeHexes.Height); 
      return new Rectangle (left, top, right-left, bottom-top);
    }

    char   this[ICoordsUser coords]     { get {return Board[coords.Y][coords.X];} }
    string HexText(ICoordsUser coords)  { return HexText(coords.X, coords.Y); }
    string HexText(int x, int y)        { return string.Format("{0,2}-{1,2}", x, y); }

    void SetPanelScale() {
      hexgridPanel.AutoScrollMinSize        = hexgridPanel.MapSizePixels;
      hexgridPanel.VerticalScroll.Maximum   = hexgridPanel.MapSizePixels.Height - ClientSize.Height;
      hexgridPanel.HorizontalScroll.Maximum = hexgridPanel.MapSizePixels.Width  - ClientSize.Width;
      hexgridPanel.Invalidate();
    }

    bool IsOnBoard(ICoordsUser coords) {
      return 0<=coords.X && coords.X < BoardSizeHexes.Width
          && 0<=coords.Y && coords.Y < BoardSizeHexes.Height;
    }

    void HexGridExampleForm_Load(object sender, EventArgs e) {
      hexgridPanel.ScaleIndex = 1; 
      SetPanelScale();

      Size = hexgridPanel.MapSizePixels + new Size(21,93);
    }

    void HexGridExampleForm_Resize(object sender, EventArgs e) {
      hexgridPanel.Invalidate();
    }

    void HexGridExampleForm_ResizeEnd(object sender, EventArgs e) {
      SetPanelScale();
    }

    void hexgridPanel_MouseClick(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        CurrentHex = hexgridPanel.GetHexCoords(e.Location).User;
      } else {
        HotSpotHex = hexgridPanel.GetHexCoords(e.Location).User;
      }

      Path = PathFinder.FindPath(
        CurrentHex.Canon, 
        HotSpotHex.Canon, 
        (c,hs) => (this[c.StepOut(hs).User]=='.') ? 1 : 999, 
        c => HotSpotHex.Canon.Range(c),
        c => IsOnBoard(c.User)
      );

      hexgridPanel.Refresh();
    }

    void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
      HotSpotHex = hexgridPanel.GetHexCoords(e.Location).User;
      Range = CurrentHex.Range(HotSpotHex);
      statusLabel.Text = "HotHex: " + HotSpotHex.ToString() 
                       + "; Range = " + Range
                       + "; Path Length = " + (Path==null ? 0 : Path.TotalCost);
    }
  }
}
