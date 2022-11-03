# Across The Obelisk Card Loader

## Setup (dev)
TODO
## Setup (Use)
TODO
## Usage

 1. Make a folder in BepInEx\plugins\{PluginName}\
 2. Place files into:
 ```
	BepInEx\plugins\{PluginName}\cards\{cardName}.json
	BepInEx\plugins\{PluginName}\cards\{cardName}.png
	BepInEx\plugins\{PluginName}\characters\{characterName}.json
	BepInEx\plugins\{PluginName}\items\{itemName}.json
```
You are allowed to organize the files however you want under those folders if you wish to have subfolders.

## JSON template
Minimum for card to show up  
```json
{
	"cardName": "SampleCard",
	"id": "SampleCard",
}
```
## JSON tags explained
TODO

## Card Images

Card images need to be 256x256px png for it to scale properly in game

There are two ways to specify a card image either implicitly or explicitly.

To implicitly set a card image, in the json file please set the field 'ImageFileName' to the file name of the image.
Example: "ImageFileName": "TestCard.png"
Implicit card images is generally used to set more then one card with the same image.

To explicitly set a card image, place the image with the same name as the json file in the same folder.
Example:
```
	BepInEx\plugins\{PluginName}\cards\TestCard.json
	BepInEx\plugins\{PluginName}\cards\TestCard.png
```
## Character Ids:
|Column 1|Column 2 | Column 3 | Column 4 | 
|--|--|--|--|
|mercenary|sentinel|berserker|warden| 
|ranger|assassin|archer|minstrel| 
|elementalist|pyromancer|loremaster|warlock| 
|cleric|priest|voodoowitch|prophet| 
