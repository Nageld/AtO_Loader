# Across The Obelisk Card Loader

## Setup (dev)
TODO
## Setup (Use)
TODO
## Usage

 1. Make a folder in BepInEx\plugins called `cards`
 2. Place files into:
	BepInEx\plugins\cards\{cardName}\{cardName}.json
	BepInEx\plugins\cards\{cardName}\{cardName}.png

## JSON template
Minimum for card to show up  
```json
{
	"cardName": "SampleCard",
	"id": "SampleCard",
	"showInTome": true
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
	BepInEx\plugins\cards\TestCard\TestCard.json
	BepInEx\plugins\cards\TestCard\TestCard.png
	