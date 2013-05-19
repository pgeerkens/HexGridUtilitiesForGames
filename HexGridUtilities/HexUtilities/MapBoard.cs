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

namespace PG_Napoleonics.Utilities.HexUtilities {
  public abstract class MapBoard : IMapBoard {
    public MapBoard(Size sizeHexes) { SizeHexes = sizeHexes; }

    public          int        FovRadius      { get; set; }
    public virtual  IFov       FOV            {
      get { return _fov ?? 
                (_fov = FieldOfView.GetFieldOfView(this, HotSpotHex, FovRadius)); }
      protected set { _fov = value; }
    } IFov _fov;
    public          ICoords    GoalHex        { 
      get { return _goalHex??(_goalHex=HexCoords.EmptyUser); } 
      set { _goalHex=value; _path = null; } 
    } ICoords _goalHex;
    public          ICoords    HotSpotHex     { 
      get { return _hotSpotHex; }
      set { _hotSpotHex = value; FOV = null; }
    } ICoords _hotSpotHex;
    public          IPath2     Path           { get {return _path ?? (_path = SetPath());} } IPath2 _path;
    public virtual  Size       SizeHexes      { get; private set; }
    public virtual  ICoords    StartHex       { 
      get { return _startHex ?? (_startHex = HexCoords.EmptyUser); } 
      set { if (IsOnBoard(value)) _startHex = value; _path = null; } 
    } ICoords _startHex;
    public INavigableBoard     NavigableBoard { get { return this; } }
    public IFovBoard<IGridHex> FovBoard       { get { return this; } }

    public abstract int    Heuristic(int range);
    public          bool   IsOnBoard(ICoords coords)  {
      return 0<=coords.User.X && coords.User.X < SizeHexes.Width
          && 0<=coords.User.Y && coords.User.Y < SizeHexes.Height;
    }
    public virtual  bool   IsPassable(ICoords coords) { return IsOnBoard(coords); }
    public abstract int    StepCost(ICoords coords, Hexside hexSide);

    IGridHex IFovBoard<IGridHex>.this[ICoords coords]  { get { return GetGridHex(coords); } }

    protected abstract IGridHex GetGridHex(ICoords coords);

    private         IPath2 SetPath() { return PathFinder2.FindPath(StartHex, GoalHex, this); }
  } 
}
