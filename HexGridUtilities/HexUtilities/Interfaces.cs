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
      /// <summary>Returns rectangular (90 degree axes at E and S) coordinates for this hex</summary>
      IntVector2D User    { get; }

      /// <summary>Returns canonical (120 degree axes at NE and S) coordinates for this hex.</summary>
      IntVector2D Canon   { get; }

      /// <summary><i>Manhattan</i> distance of specified hex from this one.</summary>
      int         Range(ICoords coords);

      /// <summary>Returns an <c>ICorods</c> for the hex in direction <c>hexside</c> from this one.</summary>
      ICoords     StepOut(Hexside hexside);

      /// <summary>Formattted string of the rectangular coordiantes for this hex.</summary>
      string      ToString();

      /// <summary>Returns set of hexes at direction(s) specified by <c>hexsides</c>, as IEnumerable<T>.</summary>
      IEnumerable<NeighbourCoords> GetNeighbours(Hexside hexsides);
    }

    public interface IBoard<TGridHex> : INavigableBoard, IFovBoard 
      where TGridHex : class, IHex {
      INavigableBoard     NavigableBoard { get; }
      IFovBoard           FovBoard       { get; }
    }

    /// <summary>Interface required to make use of A-* Path Finding utility.</summary>
    public interface INavigableBoard {
      /// <summary>The cost of entering the hex at location <c>coords</c> heading  <c>hexside</c>.</summary>
      int   StepCost(ICoords coords, Hexside hexside);

      /// <summary>Returns an A_* heuristic value from a hexagonal Manhattan distance<c>range</c>.</summary>
      int   Heuristic(int range);

      /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
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
    public interface IFovBoard {
      /// <summary>Distance in hexes out to which Field-of-View is to be calculated.</summary>
      int      FovRadius             { get; set; }

      /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
      Size     SizeHexes             { get; }

      /// <summary>Returns the <c>TGridHex</c> at location <c>coords</c>.</summary>
      IHex this[ICoords  coords] { get; }

      /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
      bool     IsOnBoard(ICoords coords);

      /// <summary>Returns whether the hex at location <c>coords</c> is passable.</summary>
      /// <param name="coords"></param>
      bool     IsPassable(ICoords coords);
    }

    /// <summary>Structure returned by the Field-of-View utility.</summary>
    public interface IFov {
      /// <summary>True if the hex at location <c>coords> is visible in this field-of-view.</summary>
      bool this[ICoords coords] { get; }
    }
  }
