## HexGridUtilitiesForGames

### Hex Grid Utilities for Board-Game Development in C#

#### A tool-kit of utilities for building board- and strategy-games on a hexagonal grid, providing these features:

- Lightning-fast **ALT Bidirectional Path-Finding** implementation;
  - Default landmarks are the 4 corners and 4 side midpoints of the map
  - To/from distances for all landmarks pre-computed on load using Dijkstra's agorithm
- **Fast Shadowcasting** implementation provides 3D Field-of-View;
  - optionally supports automatic visibility limiting due to Earth's curvature.
- Board storage as either a flat or blocked (providing better localization) array;
- Hexagonal grid with coordinates
  - **Hex-picking** (identifying the hex selected by the user);
  - All internal calculations performed with integer canonical (obtuse) coordinates;
  - All external interfaces and board storage erformed with user (standard rectangular) coordinates;
  - Built-in support for non-standard & transformed coordinate systems (such as for multi-map boards);
  - Automatic internal conversion between coordinate systems hidden in a single type HexCoords;
- WinForms Panel sub-class exposing all of the above plus:
  - Mouse-wheel zoom and scroll; and
  - Map transposition
  
