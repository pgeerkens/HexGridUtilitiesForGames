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
  public abstract class MapBoard : IMapBoard {
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

    public abstract IGridHex this[ICoordsCanon coords] { get; } // {return this[coords.User];} }
    public abstract IGridHex this[ICoordsUser coords]  { get; } // {return Board[coords.Y][coords.X];} }

    public string       HexText(ICoordsUser coords)   { return HexText(coords.X, coords.Y); }
    public bool         IsOnBoard(ICoordsUser coords) {
      return 0<=coords.X && coords.X < SizeHexes.Width
          && 0<=coords.Y && coords.Y < SizeHexes.Height;
    }
    public abstract int StepCost(ICoordsCanon coords, Hexside hexSide);

    protected abstract string[] Board  { get; }
    protected IFieldOfView      FOV    {
      get { return _fov ?? 
          (_fov = FieldOfView.GetFieldOfView(this, HotSpotHex, FoVRange)); }
      private set { _fov = value; }
    } IFieldOfView _fov;
    string HexText(int x, int y)       { return string.Format("{0,2}-{1,2}", x, y); }
  } 
}
