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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Diagnostics.CodeAnalysis;

using PGNapoleonics.HexgridExampleCommon;
using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridExampleCommon {
  using MapGridHex      = Hex<System.Drawing.Graphics,System.Drawing.Drawing2D.GraphicsPath>;

  /// <summary>TODO</summary>
  public delegate MapDisplay<MapGridHex> MapExtractor();

  /// <summary>TODO</summary>
  public struct Map {
    /// <summary>TODO</summary>
    public string MapName { get; private set; }
    /// <summary>TODO</summary>
    public MapDisplay<MapGridHex> MapBoard { get { return _mapExtractor(); } } MapExtractor _mapExtractor;

    /// <summary>TODO</summary>
    public Map(string mapName, MapExtractor mapExtractor)
      : this() {
      MapName = mapName;
      _mapExtractor = mapExtractor;
    }

    private static IList<Map> _mapList = 
        new ReadOnlyCollection<Map>(new Map[] {
            new Map("Terrain Map",   () => new TerrainMap()),
            new Map("Maze Map",      () => new MazeMap()),
            new Map("AStar Bug Map", () => new AStarBugMap())
          } );

    /// <summary>TODO</summary>
    public static IList<Map> MapList { get { return _mapList; } }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) {
      var other = obj as Map?;
      return other.HasValue && this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return MapName.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(Map other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator !=(Map lhs, Map rhs) { return !(lhs == rhs); }

    /// <summary>Tests value-equality.</summary>
    public static bool operator ==(Map lhs, Map rhs) { return (lhs.MapName == rhs.MapName); }
    #endregion
  }
}
