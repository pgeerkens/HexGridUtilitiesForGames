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

using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

//namespace PGNapoleonics.HexgridExampleCommon {
//  using MapGridHex      = Hex<Graphics,GraphicsPath>;

//  /// <summary>Abstract class for <c>MapGridHex</c> as used in the MazeGrid example.</summary>
//  internal abstract class MazeGridHex : MapGridHex {
//    /// <summary>Initializes a new instance of a <see cref="MazeGridHex"/>.</summary>
//    /// <param name="coords">Board location of this hex.</param>
//    /// <param name="elevation">Elevation of this hex.</param>
//    protected MazeGridHex(HexCoords coords, int elevation) : base(coords, elevation) { }
//  }

//  /// <summary>A <c>MazeGridHex</c> representing a passable hex in the maze.</summary>
//  internal sealed class PathMazeGridHex : MazeGridHex {
//    /// <summary>Create a new instance of a passable <c>MazeGridHex</c>.</summary>
//    /// <param name="coords">Location of the new hex.</param>
//    public PathMazeGridHex(HexCoords coords) : base(coords,0) { }

//    /// <inheritdoc/>
//    public override int  HeightTerrain  { get { return 0; } }
//    /// <inheritdoc/>
//    public override Maybe<short> TryStepCost(Hexside direction) { return 1; }
//    ///  <inheritdoc/>
//    public override void Paint(Graphics graphics, GraphicsPath graphicsPath) { ; }
//  }

//  /// <summary>A <c>MazeGridHex</c> representing an impassable hex, or wall, in the maze.</summary>
//  internal sealed class WallMazeGridHex : MazeGridHex {
//    /// <summary>Create a new instance of an impassable <c>MazeGridHex</c>.</summary>
//    /// <param name="coords">Location of the new hex.</param>
//    public WallMazeGridHex(HexCoords coords) : base(coords,1) { }

//    /// <inheritdoc/>
//    public override int  HeightTerrain  { get { return 10; } }
//    /// <inheritdoc/>
//    public override Maybe<short> TryStepCost(Hexside direction) { return Maybe<short>.NoValue(); }
//    ///  <inheritdoc/>
//    public override void Paint(Graphics graphics, GraphicsPath graphicsPath) {
//      if (graphics==null) throw new ArgumentNullException("graphics");
//      using(var brush = new SolidBrush(Color.FromArgb(78,Color.DarkGray)))
//        graphics.FillPath(brush, graphicsPath);
//    }
//  }
//}
