# NBT Apps
A webapp and its support libraries to do cool stuff with Minecraft data in .NET !

## What?
![My players have fun ideas!](doc/first-chat.png "My players suggesting we build a searchable inventory website where you search the item and it will tell you where to find it in the stores")

## How to use
* Build the solution
* Copy the `appsettings.json.dist` file to `appsettings.json` (and to `appsettings.Development.json` if needed) and edit the settings in the file to match your Minecraft worlds (check the Configuration section for more info!)

## Configuration
This section describes the important parts of the `appsettings.json` file:

### Map Paths
This section is where you will tell the app where to find your Minecraft server data.
You need to add one entry per world - the property name '`overworld`, `nether`, etc.) is not important for now.
The values need to be the path of the directory that contains the "region" and "entities" folders for the map.

For example, on a Paper server with an Overworld, a Nether, an End, and a custom Creative map:
```
  "MapPaths": {
    "overworld": "/home/minecraft/server/partywoop",
    "nether": "/home/minecraft/server/partywoop_nether/DIM-1",
    "end": "/home/minecraft/server/partywoop_the_end/DIM1",
    "creative": "/home/minecraft/server/creative_map"
  }
```

### Connection strings
This section lets you point the app to the two databases that it needs:
* `NbtDatabase` is the `nbt.db` file that is provided (in `NbtTools/Database`) to power the item search, stack sizes, and correct item names.
* `McMerchantsDatabase` is a database that contains the stores, factories, trading places... that you'll want the website to display!

An example:
```
  "ConnectionStrings": {
    "NbtDatabase": "Filename=/home/minecraft/mcmerchants/nbt.db",
    "McMerchantsDatabase": "Filename=/home/minecraft/mcmerchants/mcmerchants.db"
  }
```


## Architecture
Currently contains three projects: 
* `NbtTools`, a class library containing common elements to query NBT data:
  * Villagers and trades
  * Items stored in chests, barrels and shulkers
* `McMerchants`, a webapp that:
  * displays all trades of all villagers in given zones
  * lets you search for an item stored in chests/shulker boxes/barrels in given zones
* `McMerchantsLib`, a class library that carries the underlying concepts of McMerchants, so we can build other apps around them.