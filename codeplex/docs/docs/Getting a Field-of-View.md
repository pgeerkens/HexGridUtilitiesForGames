The behaviour of the FOV determination is controlled by the arguments supplied to the method **ShadowCasting.ComputeFIeldOfView()**:
{{
    /// <summary></summary>
    /// <remarks>
    /// Takes a circle in the form of a center point and radius, and a function
    /// that can tell whether a given cell is opaque. Calls the setFoV action on
    /// every cell that is both within the radius and visible from the center. 
    /// </remarks>
    /// <param name="observerCoords">Cordinates of observer;s hex.</param>
    /// <param name="radius">Maximum radius for Field-of-View.</param>
    /// <param name="observerHeight">Height (ASL) of the observer's eyes.</param>
    /// <param name="targetHeight">Returns ground level (ASL) of supplied hex.</param>
    /// <param name="terrainHeight">Returns height (ASL) of terrain in supplied hex.</param>
    /// <param name="setFoV">Sets a hex as visible in the Field-of-View.</param>
    public static void ComputeFieldOfView(
      ICoordsCanon            observerCoords, 
      int                     radius, 
      int                     observerHeight,
      Func<ICoordsCanon,bool> isOnBoard,
      Func<ICoordsCanon,int>  targetHeight, 
      Func<ICoordsCanon,int>  terrainHeight,
      Action<ICoordsCanon>    setFoV
) { /* ... */ }
}}
* It is important for realisism that **observerHeight > board{"[observerCoords](observerCoords)"}.ElevationASL**, or no parallax over the current ground elevation will be observed. 
* _TerrainHeight_ is the ElevationASL plus the height of any blocking in the hex, usually due to terrain but sometimes occuppying units will block vision as well. Control this in the implementation of **IBoard**.
* If the height _unit_ is taken as 5', then the contour levels on the Terrain Map are at 50' (5' * 10) and the woods generate a terrain height of 35' (5' * 7). The observer and target will then be at ElevationASL + 5'.
* If symmetry is desired (which is normal when testing and perhaps for _speculative_ FOV determination in a game situation) then the function used to determine the **targetHeight** should be the same as that used to determine **observerHeight**
* By default the engine supplies three target modes, and uses the first in the examples:
{{
        switch (targetMode) {
          case FovTargetMode.EqualHeights: 
            observer = board[origin](origin).ElevationASL + 1;
            target   = canon => board[canon.User](canon.User).ElevationASL + 1;
            break;
          case FovTargetMode.TargetHeightEqualZero:
            observer = board[origin](origin).HeightObserver;
            target   = canon => board[canon.User](canon.User).ElevationASL;
            break;
          default:
          case FovTargetMode.TargetHeightEqualActual:
            observer = board[origin](origin).HeightObserver;
            target   = canon => board[canon.User](canon.User).HeightTarget;
            break;
        }
        if (observer > 0) 
          ShadowCasting.ComputeFieldOfView(
            origin.Canon, 
            range, 
            observer,
            canon=>board.IsOnBoard(canon.User),
            target,
            canon=>board[canon.User](canon.User).HeightTerrain,
            canon=>fov[canon.User](canon.User) = true
          );
}}