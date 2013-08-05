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
#undef TraceFoV
using System;
using System.Linq;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities.Common;

/// <summary>Fast efficient <b>Shadow-Casting</b> 
/// implementation of 3D Field-of-View on a <see cref="Hexgrid"/> map.</summary>
namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>Enumeration of line-of-sight modes</summary>
  public enum FovTargetMode {
    /// <summary>Target height and observer height both set to the same constant value (ShadowCasting.DefaultHeight) above ground eleevation</summary>
    EqualHeights,
    /// <summary>Use actual observer height and ground level as target height.</summary>
    TargetHeightEqualZero,
    /// <summary>Use actual observer and target height.</summary>
    TargetHeightEqualActual
  }

  /// <summary>Credit: Eric Lippert</summary>
  /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2011/12/29/shadowcasting-in-c-part-six.aspx">Shadow Casting in C# Part Six</a>
  public static partial class ShadowCasting {
    /// <summary>Height used for observer and target when FovTargetMode = EqualHeights. </summary>
    public static int DefaultHeight { 
      get {return _defaultHeight;}
      set {_defaultHeight = value;}
    } static int _defaultHeight = 1;

    /// <summary>Calculate Field-of-View from a specified TargetMode.</summary>
    /// <remarks>
    /// It is important for realiism that observerHeight > board[observerCoords].ElevationASL, 
    /// or no parallax over the current ground elevation will be observed. TerrainHeight is the 
    /// ElevationASL of the hex, plus the height of any blocking in the hex, usually due to terrain 
    /// but sometimes occuppying units will block vision as well. Control this in the implementation 
    /// of IFovBoard&lt;IHex&gt;.</remarks>
    /// <param name="observerCoords">Cordinates of observer;s hex.</param>
    /// <param name="board">A reference to an IFovBoard&lt;IHex&gt; instance.</param>
    /// <param name="targetMode">TargetMode value for determining target visibility.</param>
    /// <param name="setFieldOfView">Sets a hex as visible in the Field-of-View.</param>
    public static void ComputeFieldOfView(
      HexCoords            observerCoords,
      IFovBoard<IHex>      board,
      FovTargetMode        targetMode, 
      Action<HexCoords>    setFieldOfView
    ) {
      Func<HexCoords,int> targetHeight;
      int                 observerHeight;
      switch (targetMode) {
        case FovTargetMode.EqualHeights: 
          targetHeight   = coords => board[coords].ElevationASL + DefaultHeight;
          observerHeight = targetHeight(observerCoords);
          break;
        case FovTargetMode.TargetHeightEqualZero:
          targetHeight   = coords => board[coords].ElevationASL;
          observerHeight = board[observerCoords].HeightObserver;
          break;
        default:
        case FovTargetMode.TargetHeightEqualActual:
          targetHeight   = coords => board[coords].HeightTarget;
          observerHeight = board[observerCoords].HeightObserver;
          break;
      }

      ShadowCasting.ComputeFieldOfView(
        observerCoords, 
        board.FovRadius, 
        observerHeight,
        board.IsOnboard,
        targetHeight,
        coords => board[coords].HeightTerrain,
        setFieldOfView
      );
    }

    /// <summary>Calculate Field-of-View from a detailed prescription.</summary>
    /// <remarks>
    /// It is important for realiism that observerHeight > board[observerCoords].ElevationASL, 
    /// or no parallax over the current ground elevation will be observed. TerrainHeight is the 
    /// ElevationASL of the hex, plus the height of any blocking in the hex, usually due to terrain 
    /// but sometimes occuppying units will block vision as well. Control this with the definition 
    /// of the observerHeight, targetHeight, and terrainHeight delegates.
    /// </remarks>
    /// <param name="observerCoords">Cordinates of observer;s hex.</param>
    /// <param name="radius">Maximum radius for Field-of-View.</param>
    /// <param name="observerHeight">Height (ASL) of the observer's eyes.</param>
    /// <param name="isOnboard">Returns whether this hex on the baoard.</param>
    /// <param name="targetHeight">Returns ground level (ASL) of supplied hex.</param>
    /// <param name="terrainHeight">Returns height (ASL) of terrain in supplied hex.</param>
    /// <param name="setFieldOfView">Sets a hex as visible in the Field-of-View.</param>
    public static void ComputeFieldOfView(
      HexCoords            observerCoords, 
      int                  radius, 
      int                  observerHeight,
      Func<HexCoords,bool> isOnboard,
      Func<HexCoords,int>  targetHeight, 
      Func<HexCoords,int>  terrainHeight,
      Action<HexCoords>    setFieldOfView
    ) {
      #if TraceFOV
        TraceFlag.FieldOfView.Trace(true, " - Coords = " + observerCoords.User.ToString());
      #endif
      var matrixOrigin = new IntMatrix2D(observerCoords.Canon);

      setFieldOfView(observerCoords);    // Always visible to self!
      #if TraceFoV
        int dodecant = 0;
        foreach (var matrix in matrices.Select(m => m*matrixOrigin)) {
          TraceFlags.FieldOfView.Trace(true," - Dodecant: {0}", dodecant++);
      #else
        Parallel.ForEach(_dodecantMatrices.Select(m => m*matrixOrigin), matrix => {
      #endif
          ComputeFieldOfViewInDodecantZero(
            radius,
            observerHeight,
            TranslateDodecant(matrix, isOnboard),
            TranslateDodecant(matrix, targetHeight),
            TranslateDodecant(matrix, terrainHeight),
            TranslateDodecant(matrix, setFieldOfView));
        }
      #if !TraceFoV
        );
      #endif
    }

    private static void ComputeFieldOfViewInDodecantZero(
      int                  radius,
      int                  observerHeight,
      Func<HexCoords,bool> isOnboard,
      Func<HexCoords, int> targetHeight,
      Func<HexCoords, int> terrainHeight,
      Action<HexCoords>    setFieldOfView)
    {
      #if TraceFOV
        radius = 16;
        DebugTracing.EnabledFags |= TraceFlag.FieldOfView;
      #endif

      var currentCoords = HexCoords.NewCanonCoords(0,1);
      if ( ! isOnboard(currentCoords) ) return;

      if (radius > 0) setFieldOfView(currentCoords);

      var queue   = new FovConeQueue();
      var current = new FovCone(
                    2, 
                    new IntVector2D(1,2), 
                    new IntVector2D(0,1), 
                    new RiseRun(terrainHeight(currentCoords) - observerHeight, 1) );
      while (current.Range <= radius) {
        current = ComputeFoVForRange(
          observerHeight,
          current,
          isOnboard,
          targetHeight,
          terrainHeight,
          setFieldOfView,
          queue);
      }
    }

    /// <summary>Processes the supplied FovCone and returns the next FovCone to process.</summary>
    /// <param name="observerHeight"></param>
    /// <param name="cone"></param>
    /// <param name="isOnboard"></param>
    /// <param name="targetHeight"></param>
    /// <param name="terrainHeight"></param>
    /// <param name="setFieldOfView"></param>
    /// <param name="queue"></param>
    /// <returns></returns>
    ///<remarks>
    /// This method: 
    /// (1) marks points inside the cone-arc that are within the radius of the field 
    ///     of view; and 
    /// (2) computes which portions of the following column are in the field of view, 
    ///     queueing them for later processing. 
    ///
    /// This algorithm is "center-to-center"; a more sophisticated algorithm 
    /// would say that a cell is visible if there is *any* straight line segment that 
    /// passes through *any* portion of the origin cell and any portion of the target 
    /// cell, passing through only transparent cells along the way. This is the 
    /// "Permissive Field Of View" algorithm, and it is much harder to implement.
    ///
    /// Search for transitions from opaque to transparent or transparent to opaque and 
    /// use those to determine what portions of the *next* column are visible from the 
    /// origin.
    ///</remarks>
    private static FovCone ComputeFoVForRange(
      int                  observerHeight,
      FovCone              cone,
      Func<HexCoords,bool> isOnboard,
      Func<HexCoords, int> targetHeight,
      Func<HexCoords, int> terrainHeight,
      Action<HexCoords>    setFieldOfView,
      FovConeQueue         queue)
    {
      Action<FovCone> enqueue = queue.Enqueue;

      var range         = cone.Range;
      var topVector     = cone.VectorTop;
      var topRiseRun    = cone.RiseRun;
      var bottomVector  = cone.VectorBottom;

      // track the overlap-cone between adjacent hexes as we move down.
      var overlapVector = cone.VectorTop;
      var hexX          = XFromVector(range, topVector);
      #if TraceFOV
        TraceFlag.FieldOfView.Trace(false, "DQ:   ({0}) from {1}", cone, hexX);
      #endif

      do {
        while (overlapVector.GT(bottomVector)) {
          var coordsCurrent   = HexCoords.NewCanonCoords(hexX,range);
          var hexVectorBottom = VectorHexBottom(coordsCurrent);
          if (isOnboard(coordsCurrent)) { 
            #region Set current hex parameters
            var hexVectorTop  = VectorHexTop(coordsCurrent);
            var hexElevation  = targetHeight(coordsCurrent);
            var hexHeight     = terrainHeight(coordsCurrent);
            var hexRiseRun    = new RiseRun(hexHeight-observerHeight, range);
            #endregion

            #region Check visibility of current hex
            var riseRun = new RiseRun(hexElevation-observerHeight, GetRange(coordsCurrent));
            if ( riseRun >= cone.RiseRun  
            && bottomVector.LE(coordsCurrent.Canon) && coordsCurrent.Canon.LE(topVector)  
            ) {
              setFieldOfView(coordsCurrent);
              #if TraceFOV
                TraceFlag.FieldOfView.Trace(false,"    Set visible: {0} / {1}; {2} >= {3}", 
                    MapCoordsDodecant(coordsCurrent), coordsCurrent.ToString(), riseRun, cone.RiseRun);
              #endif
            }
            #endregion

            #region Check hex transition
            if (hexRiseRun > topRiseRun) {
              topVector  = LogAndEnqueue(enqueue, range, topVector, hexVectorTop, topRiseRun, 0);
              topRiseRun = hexRiseRun;
            } else if (hexRiseRun > cone.RiseRun) {
              topVector  = LogAndEnqueue(enqueue, range, topVector, overlapVector, topRiseRun, 1);
              topRiseRun = hexRiseRun;
            } else if (hexRiseRun < cone.RiseRun) {
              topVector  = LogAndEnqueue(enqueue, range, topVector, overlapVector, topRiseRun, 2);
              topRiseRun = cone.RiseRun;
            }
            #endregion
          }

          overlapVector = VectorMax(hexVectorBottom, bottomVector); 
          if (hexVectorBottom.GT(bottomVector))      -- hexX;
        }

        #region Dequeue next cone
        if (queue.Count == 0) {
          topVector   = LogAndEnqueue(enqueue, range, topVector, bottomVector, topRiseRun, 3);
          cone        = queue.Dequeue();
          break;
        } else {
          cone        = queue.Dequeue();
          if(cone.Range != range) {
            topVector = LogAndEnqueue(enqueue, range, topVector, bottomVector, topRiseRun, 4);
            break;
          }
          #if TraceFOV
            TraceFlag.FieldOfView.Trace(false, "DQ:   ({0}) from {1}", cone, hexX);
          #endif
        }
        #endregion

        #region Check cone transition
        if (cone.RiseRun > topRiseRun) {
          topVector   = LogAndEnqueue(enqueue, range, topVector, cone.VectorTop, topRiseRun, 5);
          topRiseRun  = cone.RiseRun;
        } else if (cone.RiseRun < topRiseRun) {
          topVector   = LogAndEnqueue(enqueue, range, topVector, overlapVector, topRiseRun, 6);
          topRiseRun  = cone.RiseRun;
        }
        #endregion

        overlapVector = topVector;
        bottomVector  = cone.VectorBottom;
      } while(true);

      // Pick-up any cone portion at bottom of range still unprocessed
      if (topVector.GT(bottomVector))
        LogAndEnqueue(enqueue, range, topVector, bottomVector, cone.RiseRun, 7);

      return cone;
    }
  }
}
