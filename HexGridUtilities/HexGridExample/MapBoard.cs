#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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
    public IPath<ICoordsCanon> Path         { get; set; }
    public Size                SizeHexes    { get {return new Size(Board[0].Length,Board.Length);} }

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
    public abstract int  Range(ICoordsCanon goal, ICoordsCanon current);

    protected abstract string[] Board  { get; }
    protected IFieldOfView      FOV    {
      get { return _fov ?? 
                (_fov = FieldOfView.GetFieldOfView(this, HotSpotHex, FoVRange)); }
      private set { _fov = value; }
    } IFieldOfView _fov;
    string HexText(int x, int y)       { return string.Format("{0,2}-{1,2}", x, y); }
  } 
}
