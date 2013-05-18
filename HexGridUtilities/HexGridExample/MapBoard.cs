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

using PG_Napoleonics.Utilities;
using PG_Napoleonics.Utilities.HexUtilities;

namespace PG_Napoleonics.HexGridExample {
  public abstract class MapBoard :  IBoard<IGridHex> {
    public static int FoVRange = 40;

    public bool                ShowFov      { get; set; }
    public ICoordsUser         StartHex     { get; set; }
    public ICoordsUser         GoalHex      { get; set; }
    public ICoordsUser         HotSpotHex   { 
      get { return _hotSpotHex; }
      set { _hotSpotHex = value; FOV = null; }
    } ICoordsUser _hotSpotHex;
    public IPath2 Path         { get; set; }
    public Size   SizeHexes    { get {return new Size(Board[0].Length,Board.Length);} }

    IGridHex IBoard<IGridHex>.this[ICoordsCanon coords] { get { return this[coords]; } }
    IGridHex IBoard<IGridHex>.this[ICoordsUser coords]  { get { return this[coords]; } }
    public abstract IMapGridHex this[ICoordsCanon coords] { get; }
    public abstract IMapGridHex this[ICoordsUser coords]  { get; }

    public string        HexText(ICoordsUser coords)   { return HexText(coords.X, coords.Y); }
    public bool          IsOnBoard(ICoordsUser coords) {
      return 0<=coords.X && coords.X < SizeHexes.Width
          && 0<=coords.Y && coords.Y < SizeHexes.Height;
    }
    public virtual bool  IsPassable(ICoordsUser coords) { return IsOnBoard(coords); }
    public abstract int  StepCost(ICoordsCanon coords, Hexside hexSide);
    public abstract int Heuristic(int range);

    protected abstract string[] Board  { get; }
    protected IFieldOfView      FOV    {
      get { return _fov ?? 
                (_fov = FieldOfView.GetFieldOfView(this, HotSpotHex, FoVRange)); }
      private set { _fov = value; }
    } IFieldOfView _fov;
    string HexText(int x, int y)       { return string.Format("{0,2}-{1,2}", x, y); }
  } 
}
