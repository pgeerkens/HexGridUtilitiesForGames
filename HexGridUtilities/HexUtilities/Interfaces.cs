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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

  namespace PG_Napoleonics.Utilities.HexUtilities {
    public interface ICoords {
      IntVector2D User    { get; }
      IntVector2D Canon   { get; }
      IntVector2D Custom  { get; }

      int         Range(ICoords coords);
      ICoords     StepOut(ICoords coords);
      ICoords     StepOut(Hexside hexside);
      string      ToString();

      IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);
    }

    public interface IMapBoard : IBoard<IGridHex> {
      /// <summary>Distance in hexes out to which Field-of-View is to be calculated.</summary>
      int     FovRadius   { get; set; }
      IFov    FOV         { get; }
      ICoords GoalHex     { get; set; }
      ICoords HotSpotHex  { get; set; }
      IPath2  Path        { get; }
      ICoords StartHex    { get; set; }
    }
    public interface IBoard<TGridHex> : INavigableBoard, IFovBoard<TGridHex> 
      where TGridHex : class, IGridHex {
      INavigableBoard     NavigableBoard { get; }
      IFovBoard<TGridHex> FovBoard       { get; }
    }

    /// <summary>Interface required to make use of A-* Path Finding utility.</summary>
    public interface INavigableBoard {
      /// <summary>The cost of leaving the hex with coordinates <c>coords</c>through <c>hexside</c>.</summary>
      int   StepCost(ICoords coords, Hexside hexside);
      /// <summary>Returns an A_* heuristic value from a hexagonal Manhattan distance<c>range</c>.</summary>
      int   Heuristic(int range);
      /// <summary>Returns whether the hex with coordinates <c>coords</c>is "on board".</summary>
      bool  IsOnBoard(ICoords coords);
    }
    /// <summary>Structure returned by the A-* Path Finding utility.</summary>
    public interface IPath2 : IEnumerable<ICoords>
    { 
      Hexside   LastDirection { get; }
      ICoords   LastStep      { get; }
      IPath2    PreviousSteps { get; }
      uint      TotalCost     { get; }
      uint      TotalSteps    { get; }
    }

    /// <summary>Interface required to make use of ShadowCasting Field-of-View calculation.</summary>
    /// <typeparam name="TGridHex"></typeparam>
    public interface IFovBoard<TGridHex> where TGridHex : class, IGridHex {
      /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
      Size     SizeHexes             { get; }
      /// <summary>Returns the <c>TGridHex</c> at location <c>coords</c>.</summary>
      TGridHex this[ICoords  coords] { get; }

      /// <summary>Returns whether the hex with coordinates <c>coords</c>is "on board".</summary>
      bool     IsOnBoard(ICoords coords);

      /// <summary>Returns whether the hex at location <c>coords</c> is passable.</summary>
      /// <param name="coords"></param>
      bool     IsPassable(ICoords coords);
    }
    /// <summary>Structure returned by the Field-of-View utility.</summary>
    public interface IFov {
      /// <summary>True if the hex at coordinates <c>coords> is visible in this field-of-view.</summary>
      bool this[ICoords coords] { get; }
    }
  }
