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

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexgridPanel {
  using MapGridHex      = Hex<Graphics,GraphicsPath>;

  internal static class MapDisplayPainter {
    /// <summary>Paint the base layer of the display, graphics that changes rarely between refreshes.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>For each visible hex: perform <c>paintAction</c> and then draw its hexgrid outline.</remarks>
    public static void PaintMap<THex>(this MapDisplay<THex> @this, Graphics graphics
      ) where THex : MapGridHex { 
      if (graphics==null) throw new ArgumentNullException("graphics");

      var container = graphics.BeginContainer(); // Set all transformations relative to current origin!
      var clipHexes  = @this.GetClipInHexes(graphics.VisibleClipBounds);
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      var font       = SystemFonts.MenuFont;
      var brush      = Brushes.Black;
      var textOffset = new Point((@this.GridSize.Scale(0.50F)
                     - new SizeF(font.Size,font.Size).Scale(0.8F)).ToSize());
      @this.PaintForEachHex(graphics, clipHexes, coords => {
        @this[coords].Paint(graphics);
        if (@this.ShowHexgrid) graphics.DrawPath(Pens.Black, @this.HexgridPath);
        if (@this.LandmarkToShow > 0) {
          graphics.DrawString(@this.LandmarkDistance(coords,@this.LandmarkToShow-1), font, brush, textOffset);
        }
      } );
      graphics.EndContainer(container); 
    }

    /// <summary>Paint the top layer of the display, graphics that changes frequently between refreshes.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Graphics object for the canvas being painted.</param>
    public static void PaintHighlight<THex>(this MapDisplay<THex> @this, Graphics graphics
      ) where THex : MapGridHex {
      if (graphics==null) throw new ArgumentNullException("graphics");
      var container = graphics.BeginContainer();
      graphics.Transform = @this.TranslateToHex(@this.StartHex);
      graphics.DrawPath(Pens.Red, @this.HexgridPath);

      if (@this.ShowPath) {
        graphics.EndContainer(container); container = graphics.BeginContainer();
        @this.PaintPath(graphics,@this.Path);
      }

      if (@this.ShowRangeLine) {
        graphics.EndContainer(container); container = graphics.BeginContainer();
        var target = @this.CentreOfHex(@this.HotspotHex);
        graphics.DrawLine(Pens.Red, @this.CentreOfHex(@this.StartHex), target);
        graphics.DrawLine(Pens.Red, target.X-8,target.Y-8, target.X+8,target.Y+8);
        graphics.DrawLine(Pens.Red, target.X-8,target.Y+8, target.X+8,target.Y-8);
      }
      graphics.EndContainer(container); 
    }

    public static void PaintShading<THex>(this MapDisplay<THex> @this, Graphics graphics
      ) where THex : MapGridHex {
      if (graphics==null) throw new ArgumentNullException("graphics");
      var container = graphics.BeginContainer();

      if (@this.ShowFov) {
        var clipHexes  = @this.GetClipInHexes(graphics.VisibleClipBounds);
        using(var shadeBrush = new SolidBrush(Color.FromArgb(@this.ShadeBrushAlpha, @this.ShadeBrushColor))) {
          @this.PaintForEachHex(graphics, clipHexes, coords => {
            if (@this.Fov!=null && ! @this.Fov[coords]) { graphics.FillPath(shadeBrush, @this.HexgridPath);  }
          } );
        }
      }

      graphics.EndContainer(container); 
    }

    /// <summary>Paints all the hexes in <paramref name="clipHexes"/> by executing <paramref name="paintAction"/>
    /// for each hex on <paramref name="graphics"/>.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Graphics object for the canvas being painted.</param>
    /// <param name="clipHexes">Type: CoordRectangle - 
    /// The rectangular extent of hexes to be painted.</param>
    /// <param name="paintAction">Type: Action {HexCoords} - 
    /// The paint action to be performed for each hex.</param>
    public static void PaintForEachHex<THex>(this MapDisplay<THex> @this, Graphics graphics, 
      CoordsRectangle clipHexes, Action<HexCoords> paintAction
    ) where THex : MapGridHex {
      @this.ForEachHex(hex => {
        if (clipHexes.Left <= hex.Coords.User.X  &&  hex.Coords.User.X <= clipHexes.Right
        &&  clipHexes.Top  <= hex.Coords.User.Y  &&  hex.Coords.User.Y <= clipHexes.Bottom) {
          graphics.Transform = @this.TranslateToHex(hex.Coords);
          paintAction(hex.Coords);
        }
      } );
      return;
    }

    /// <summary>Paint the current shortese path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
    /// A directed path (ie linked-list> of hexes to be painted.</param>
    public static void PaintPath<THex>(this MapDisplay<THex> @this, Graphics graphics, IDirectedPathCollection path
    ) where THex : MapGridHex {
      if (graphics==null) throw new ArgumentNullException("graphics");

      using(var brush = new SolidBrush(Color.FromArgb(78, Color.PaleGoldenrod))) {
        while (path != null) {
          var coords = path.PathStep.Hex.Coords;
          graphics.Transform = @this.TranslateToHex(coords);
          graphics.FillPath(brush, @this.HexgridPath);

          if (@this.ShowPathArrow) @this.PaintPathArrow(graphics, path);

          path = path.PathSoFar;
        }
      }
    }

    /// <summary>Paint the direction and destination indicators for each hex of the current shortest path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="path">Type: <see cref="IDirectedPathCollection"/> - 
    /// A directed path (ie linked-list> of hexes to be highlighted with a direction arrow.</param>
    static void PaintPathArrow<THex>(this MapDisplay<THex> @this, Graphics graphics, IDirectedPathCollection path
    ) where THex : MapGridHex {
      if (graphics==null) throw new ArgumentNullException("graphics");
      if (path==null) throw new ArgumentNullException("path");

      graphics.TranslateTransform(@this.CentreOfHexOffset.Width, @this.CentreOfHexOffset.Height);
      if (path.PathSoFar == null)    @this.PaintPathDestination(graphics);
      else                           @this.PaintPathArrow(graphics, path.PathStep.HexsideEntry);
    }

    /// <summary>Paint the direction arrow for each hex of the current shortest path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
    /// <param name="hexside">Type: <see cref="Hexside"/> - 
    /// Direction from this hex in which the next step is made.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    static void PaintPathArrow<THex>(this MapDisplay<THex> @this, Graphics graphics, Hexside hexside
    ) where THex : MapGridHex {
      if (graphics==null) throw new ArgumentNullException("graphics");

      var unit = @this.GridSize.Height/8.0F;
      graphics.RotateTransform(60 * (int)hexside);
      graphics.DrawLine(Pens.Black, 0,unit*4,       0,  -unit);
      graphics.DrawLine(Pens.Black, 0,unit*4, -unit*3/2, unit*2);
      graphics.DrawLine(Pens.Black, 0,unit*4,  unit*3/2, unit*2);
    }

    /// <summary>Paint the destination indicator for the current shortest path.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
    /// <remarks>The current graphics origin must be the centre of the current hex.</remarks>
    static void PaintPathDestination<THex>(this MapDisplay<THex> @this, Graphics graphics
    ) where THex : MapGridHex {
      if (graphics==null) throw new ArgumentNullException("graphics");

      var unit = @this.GridSize.Height/8.0F;
      graphics.DrawLine(Pens.Black, -unit*2,-unit*2, unit*2, unit*2);
      graphics.DrawLine(Pens.Black, -unit*2, unit*2, unit*2,-unit*2);
    }

    /// <summary>Paint the intermediate layer of the display, graphics that changes infrequently between refreshes.</summary>
    /// <param name="this">Type: MapDisplay{THex} - The map to be painted.</param>
    /// <param name="graphics">Type: Graphics - Object representing the canvas being painted.</param>
    public static void PaintUnits<THex>(this MapDisplay<THex> @this, Graphics graphics) where THex : MapGridHex {
      if (@this    == null) throw new ArgumentNullException("this");
      if (graphics == null) throw new ArgumentNullException("graphics");
      /* NO-OP - Not implemented in examples. */
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
