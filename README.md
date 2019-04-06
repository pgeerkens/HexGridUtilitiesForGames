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
  
#### Structure

- **HexUtilitites:** The core library with all functionality not dependent on either WinForms or WPF. THis library is moderatey to extremely stable at the present time.

- **HexgridPanel:** All core functionality depending on either WinForms or WPF. THis library is undergoing revision currently that may include breaking changes.

- **HexgridExamplesCommon:** Common structures used by the example libraries below.

- **HexgridExampleWinform:** An example MDI WInForm that opens two windows (forms) based around HexgridPanel (terrible performance) and HexgridBufferedPanel (moderately good performance). Planned:

  - Example based around a 5-layer buffered & cached subclass of HexgridPanel (HexgridCachedPanel - from proprietary code) that offers excellent performance even on maps very much larger than the examples.
  
- **HexgridExampleWpf:** A WPF example that opens two windows (forms) based around HexgridPanel (terrible performance) and HexgridBufferedPanel (moderately good performance).  Once HexgridCachedPanel is complete an example based around it will also be added.

- **HexgridExampleWinformw:** Work in progress.
  
### External Documentation

[Doxygen](http://www.doxygen.nl/) is used to generate external documentation for the entire solution, including diagrams using [Graphviz](https://www.graphviz.org/) [DOT](https://www.graphviz.org/doc/info/lang.html), in the file docs.zip. I endeavour to update this on every *push* to GitHub - but it is not yet automated.
  
### References:

Many thanks to:
- Eric Lippert for providing the original [Shadowcasting](https://blogs.msdn.microsoft.com/ericlippert/tag/shadowcasting/) implementation, and inspiring me with his [rectangular A* path-finding](https://blogs.msdn.microsoft.com/ericlippert/tag/astar/) algorithm;
- Wim Pijls & Henk Post for [Yet Another Bidirectional Algorithm for Shortest Paths](http://repub.eur.nl/res/pub/16100/ei2009-10.pdf);
- Luis Henrique Oliveira Rios & Luiz Chaimowicz for [PNBA{"*"} - A Parallel Bidirectional Heuristic Search Algorithm](https://homepages.dcc.ufmg.br/~chaimo/public/ENIA11.pdf); 
- Andrew V. Goldberg & Renato F. Werneck for [Computing Point-to-Point Shortest Paths from External Memory](http://www.cs.princeton.edu/courses/archive/spr06/cos423/Handouts/GW05.pdf), (the explanation of ALT that finally stuck.)
