Everything is provided, except (of course) the game-specific StepCost function. Invoke the PathFInder as shown here from **HexgridPanel.cs**:

{{
    protected override void OnMouseClick(MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        Host.CurrentHex = GetHexCoords(e.Location).User;
      } else {
        ost.HotSpotHex = GetHexCoords(e.Location).User;
      }

      Path = PathFinder.FindPath(
        Host.CurrentHex.Canon, 
        Host.HotSpotHex.Canon, 
        (c,hs) => Host.StepCost(c,hs), 
        c => Host.HotSpotHex.Canon.Range(c),
        c => Host.IsOnBoard(c.User)
      );

      Refresh();
    }
}}
with **FindPath** prototype:
{{
    public static IPath<ICoordsCanon> FindPath( 
      ICoordsCanon start, 
      ICoordsCanon goal,
      Func<ICoordsCanon,Hexside,int> stepCost,
      Func<ICoordsCanon,int>         estimate,
      Func<ICoordsCanon,bool>        isOnBoard
  }
}}
The example StepCost function implements a simple clear/blocked terrain, but any cost function of the hex being entered and the hexside being traversed can be provided. 
The algorithms is from [Eric Lippert's](http://blogs.msdn.com/b/ericlippert/archive/2007/10/02/path-finding-using-a-in-c-3-0.aspx) excellent blog articles on A-*, adapted to a hex-grid, and to to weight the most direct path favourably for better (visual) behaviour on a hexgrid.
## Note
One asumption is made that, in writing this documentation, I realize should be enforced by the code but isn't (yet).:
* StepCost must fit in a **short**.
The reason for this is the implementation of the weighting used to favour direct paths on a hex grid (excerpted from **Pathfinder.FindPath**:
{{
        foreach (var neighbour in path.LastStep.GetNeighbours(~Hexside.None)) {
          if (isOnBoard(neighbour.Coords.Canon)) {
            var preference    = (ushort)vectorGoal.VectorCross(goal.Vector - neighbour.Coords.Canon.Vector);
            var cost          = stepCost(path.LastStep, neighbour.Direction);
            if (cost > 0) {
              var newPath     = path.AddStep(neighbour.Coords.Canon, (ushort)cost, neighbour.Direction); 
              var newEstimate = ((uint)estimate(neighbour.Coords.Canon) + (uint)newPath.TotalCost) << 16;
              queue.Enqueue(newEstimate + preference, newPath);
              DebugTracing.Trace(TraceFlag.FindPath, "   Enqueue {0}: {1} / {2}; / {3}",
                      neighbour.Coords, newEstimate>>16, newEstimate & 0x0FFFF, preference);
            }
          }
        }
}}
using this definition of VectorCross:
{{
    public static int VectorCross (this IntVector2D lhs, IntVector2D rhs) {
      return Math.Abs(lhs.X**rhs.Y - lhs.Y**rhs.X);
    }
}}
Note how **newEstimate** and **preference** are placed in the high- and low-order 16 bits, respectively, of the step estimate to induce the desired weighting of paths with an equal actual cost.