# Test fixtures for Store and Alley Boundaries tests
## Minecraft map
Minecraft version: 1.20.1 (chunk data version: `3465`)

A Minecraft map configured as follows:
* 4 chunks in 2 regions
* A "store" zone materialized with blocks of a specific color

### Inside the "store" boundaries:

|Region|Chunk  |Container type|Contents                                        |
|------|-------|--------------|------------------------------------------------|
|0,0   |0,-4,0 |Chest         |1 of red sand + 1 shulker box with 1 of red sand|
|0,0   |0,-4,1 |Shulker box   |1 of red sand                                   |
|-1,0  |-1,-4,0|Barrel        |1 of red sand + 1 shulker box with 1 of red sand|
|-1,0  |-1,-4,1|Trapped chest |1 of red sand + 1 shulker box with 1 of red sand|

Total: 7 of red sand

### Outside the "store" boundaries:
Around the "store" boundaries (at the same Y level):

|Region|Chunk  |Container type|Contents        |
|------|-------|--------------|----------------|
|0,0   |0,-4,0 |Trapped chest |1 of slime block|
|0,0   |0,-4,1 |Shulker box   |1 of slime block|
|-1,0  |-1,-4,0|Chest         |1 of slime block|
|-1,0  |-1,-4,1|Chest         |1 of slime block|

Above the "store" vertical boundary:

|Region|Chunk  |Container type|Contents        |
|------|-------|--------------|----------------|
|0,0   |0,-4,0 |Chest         |2 of slime block|
|0,0   |0,-4,1 |Chest         |2 of slime block|
|-1,0  |-1,-4,0|Chest         |2 of slime block|
|-1,0  |-1,-4,1|Chest         |2 of slime block|

Total: 12 of slime block

## McMerchants database
* 1 store with bounds

## NBT database
Version 1.20.1