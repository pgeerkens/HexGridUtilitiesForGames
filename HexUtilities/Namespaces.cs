#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@hotmail.com)
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

#pragma warning disable 1587
/// <summary>A library of utilities and controls for buillding strategy simulation games on hexagonal-grid 
/// mapboards.</summary>
/// <remarks>
/// To get started, explore how the two sample maps <see cref="TerrainMap"/> and <see cref="MazeMap"/> override
/// the base class <see cref="MapDisplay{THex}"/>, and how the sample form <see cref="HexgridExampleForm"/>
/// overrides the base control <see cref="HexgridPanel"/>. The Collaboration diagram for MapDisplay{THex}
/// provides a good overview fof the library structure.
/// 
/// Brought to you by PG Software Solutions Inc., a quality software provider.
/// 
/// Our products are more than Pretty Good Software; ... they're Pretty Great Solutions!
/// </remarks>
namespace PGNapoleonics { }

/// <summary>Display-technology-independent utilities for implementation of hex-grids..</summary>
namespace PGNapoleonics.HexUtilities { }

/// <summary>Shared technoloiges across the library, and useful gadgets.</summary>
namespace PGNapoleonics.HexUtilities.Common { }

/// <summary>Fast efficient <b>Shadow-Casting</b> 
/// implementation of 3D Field-of-View on a <see cref="Hexgrid"/> map.</summary>
namespace PGNapoleonics.HexUtilities.FieldOfView { }

/// <summary>A fast efficient serial implementation of <b>Bidirectional ALT</b> (<b>A*</b> with <b>L</b>andmarks
/// and <b>T</b>riangle-inequality heuristic) <b>path-finding</b> on a <see cref="Hexgrid"/> map.</summary>
namespace PGNapoleonics.HexUtilities.Pathfinding { }
#pragma warning restore 1587
