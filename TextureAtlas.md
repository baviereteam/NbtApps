# Texture Atlas
## What's this stuff?
The texture atlas is a file that contains the icons for all the blocks and items in Minecraft.
McMerchants uses it to display these icons on the website.

## How do I get one?
* Open Minecraft and get into a game (single or multi-player, it doesn't matter)
* Press F3 + S
* A chat message appears, that says something akin to `[Debug]: Saved dynamic textures to screenshots\debug`
* You will find the files in your `.minecraft` directory, in `screenshots/debug`. You only need these two ones:
  * `minecraft_textures_atlas_blocks.png_0.png`
  * `minecraft_textures_atlas_blocks.png.txt`

## Then what?
* Put these two files somewhere on the server where McMerchants runs.
* Configure the paths in the `appsettings.json` file.