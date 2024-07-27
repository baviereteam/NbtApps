# Test fixtures for Alley Boundaries tests
## Minecraft map
Minecraft version: 1.20.1 (chunk data version: `3465`)

A Minecraft map configured as follows:
* A "store" zone materialized with blocks of a specific color, between the following coordinates:

|x  |y   |z |
|---|----|--|
|-16|-61 |16|
|-7 |-55 |19|

* An alley inside the "store" zone, configured as such:
  * Direction: Z
  * Coordinate: 17 (the Z axis)
  * Low boundary (on the X axis): -12,
  * High boundary (on the X axis): -9,
  * StartY: -59,
  * EndY: -57

### Inside the "alley" boundaries:
3 containers containing a total of 3 honey blocks.

### Inside the "store" boundaries, outside the "alley" boundaries:
4 containers containing a total of 6 ice blocks.