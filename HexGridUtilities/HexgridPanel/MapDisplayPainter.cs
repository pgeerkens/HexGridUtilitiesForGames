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
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridPanel {
  internal static class MapDisplayPainter {
    /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
    public static void PaintMap<THex>(this MapDisplay<THex> @this, Graphics g
      ) where THex : MapGridHex { 
      if (g==null) throw new ArgumentNullException("g");

      var container = g.BeginContainer(); // Set all transformations relative to current origin!
      var clipHexes  = @this.GetClipInHexes(g.VisibleClipBounds);
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      var font       = SystemFonts.MenuFont;
      var brush      = Brushes.Black;
      var textOffset = new Point((@this.GridSize.Scale(0.50F)
                     - new SizeF(font.Size,font.Size).Scale(0.8F)).ToSize());
      @this.PaintForEachHex(g, clipHexes, coords => {
        @this[coords].Paint(g);
        if (@this.ShowHexgrid) g.DrawPath(Pens.Black, @this.HexgridPath);
        if (@this.LandmarkToShow > 0) {
          g.DrawString(@this.LandmarkDistance(coords,@this.LandmarkToShow-1), font, brush, textOffset);
        }
      } );
      g.EndContainer(container); 
    }

    /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    public static void PaintHighlight<THex>(this MapDisplay<THex> @this, Graphics g
      ) where THex : MapGridHex {
      if (g==null) throw new ArgumentNullException("g");
      var container = g.BeginContainer();
      g.Transform = @this.TranslateToHex(@this.StartHex);
      g.DrawPath(Pens.Red, @this.HexgridPath);

      if (@this.ShowPath) {
        g.EndContainer(container); container = g.BeginContainer();
        @this.PaintPath(g,@this.Path);
      }

      if (@this.ShowRangeLine) {
        g.EndContainer(container); container = g.BeginContainer();
        var target = @this.CentreOfHex(@this.HotspotHex);
        g.DrawLine(Pens.Red, @this.CentreOfHex(@this.StartHex), target);
        g.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
        g.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
      }
      g.EndContainer(container); 
    }

    public static void PaintShading<THex>(this MapDisplay<THex> @this, Graphics g
      ) where THex : MapGridHex {
      if (g==null) throw new ArgumentNullException("g");
      var container = g.BeginContainer();

      if (@this.ShowFov) {
        var clipHexes  = @this.GetClipInHexes(g.VisibleClipBounds);
        using(var shadeBrush = new SolidBrush(Color.FromArgb(@this.ShadeBrushAlpha, @this.ShadeBrushColor))) {
          @this.PaintForEachHex(g, clipHexes, coords => {
            if (@this.Fov!=null && ! @this.Fov[coords]) { g.FillPath(shadeBrush, @this.HexgridPath);  }
          } );
        }
      }

      g.EndContainer(container); 
    }

    /// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
    /// for each hex on <paramref name="g"/>.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Graphics object for the canvas being painted.</param>
    /// <param name="clipHexes">Type: CoordRectangle - 
    /// The rectangular extent of hexes to be painted.</param>
    /// <param name="paintAction">Type: Action {HexCoords} - 
    /// The paint action to be performed for each hex.</param>
    public static void PaintForEachHex<THex>(this MapDisplay<THex> @this, Graphics g, 
      CoordsRectangle clipHexes, Action<HexCoords> paintAction
    ) where THex : MapGridHex {
      @this.BoardHexes.ForEach(hex => {
        if (clipHexes.Left <= hex.Coords.User.X  &&  hex.Coords.User.X <= clipHexes.Right
        &&  clipHexes.Top  <= hex.Coords.User.Y  &&  hex.Coords.User.Y <= clipHexes.Bottom) {
          g.Transform = @this.TranslateToHex(hex.Coords);
          paintAction(hex.Coords);
        }
      } );
      return;
    }

    /// <summary>Paint the current shortese path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
    /// A directed path (ie linked-list> of hexes to be painted.</param>
    public static void PaintPath<THex>(this MapDisplay<THex> @this, Graphics g, IDirectedPathCollection path
    ) where THex : MapGridHex {
      if (g==null) throw new ArgumentNullException("g");

      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        while (path != null) {
          var coords = path.PathStep.Hex.Coords;
          g.Transform = @this.TranslateToHex(coords);
          g.FillPath(brush, @this.HexgridPath);

          if (@this.ShowPathArrow) @this.PaintPathArrow(g, path);

          path = path.PathSoFar;
        }
      }
    }

    /// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
    /// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
    static void PaintPathArrow<THex>(this MapDisplay<THex> @this, Graphics g, IDirectedPathCollection path
    ) where THex : MapGridHex {
      if (g==null) throw new ArgumentNullException("g");
      if (path==null) throw new ArgumentNullException("path");

      g.TranslateTransform(@this.CentreOfHexOffset.Width, @this.CentreOfHexOffset.Height);
      if (path.PathSoFar == null)    @this.PaintPathDestination(g);
      else                           @this.PaintPathArrow(g, path.PathStep.HexsideEntry);
    }

    /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="hexside">Type: <see cref="Hexside"/> - 
    /// Direction from this hex in which the next step is made.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    static void PaintPathArrow<THex>(this MapDisplay<THex> @this, Graphics g, Hexside hexside
    ) where THex : MapGridHex {
      if (g==null) throw new ArgumentNullException("g");

      var unit = @this.GridSize.Height/8.0F;
      g.RotateTransform(60 * (int)hexside);
      g.DrawLine(Pens.Black, 0,unit*4,       0,  -unit);
      g.DrawLine(Pens.Black, 0,unit*4, -unit*3/2, unit*2);
      g.DrawLine(Pens.Black, 0,unit*4,  unit*3/2, unit*2);
    }

    /// <summary>Paint the destination indicator for the current shortest path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    static void PaintPathDestination<THex>(this MapDisplay<THex> @this, Graphics g
    ) where THex : MapGridHex {
      if (g==null) throw new ArgumentNullException("g");

      var unit = @this.GridSize.Height/8.0F;
      g.DrawLine(Pens.Black, -unit*2,-unit*2, unit*2, unit*2);
      g.DrawLine(Pens.Black, -unit*2, unit*2, unit*2,-unit*2);
    }

    /// <summary>Paint the intermediate layer of the display, graphics that changes infrequently between refreshes.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="g">Type: Graphics - Object representing the canvas being painted.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "g")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
    public static void PaintUnits<THex>(this MapDisplay<THex> @this, Graphics g) where THex : MapGridHex {
      ; /* NO=OP - nNot implemented in examples. */
    }
  }

  /// <summary>TODO</summary>
  public static partial class GraphicsExtensions {
    /// <summary>TODO</summary>
    public static void PreserveState(this Graphics @this, Action<Graphics> drawingCommands) {
      if (@this != null  &&  drawingCommands != null) {
        var state = @this.Save();
        drawingCommands(@this);
        @this.Restore(state);
      }
    }
  }
}
