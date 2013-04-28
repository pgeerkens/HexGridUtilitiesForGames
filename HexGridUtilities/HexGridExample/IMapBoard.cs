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
  public interface IMapBoard : IBoard<IMapGridHex>{
    ICoordsUser         GoalHex        { get; set; }
    ICoordsUser         HotSpotHex     { get; set; }
    IPath<ICoordsCanon> Path           { get; set; }
    bool                ShowFov        { get; set; }
    ICoordsUser         StartHex       { get; set; }

    string HexText(ICoordsUser coords);
    int    Range(ICoordsCanon goal, ICoordsCanon current);
    int    StepCost(ICoordsCanon coords, Hexside hexSide);
  }
}
