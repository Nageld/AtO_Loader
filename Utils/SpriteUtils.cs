using AtO_Loader.DataLoader.DataWrapper;
using System;
using System.IO;
using AtO_Loader.DataLoader.DataWrapper;
using UnityEngine;

namespace AtO_Loader.Utils
{
    public static class SpriteUtils
    {
        public static void LoadSprite(this CardDataWrapper card, FileInfo cardFileInfo)
            => card.Sprite = LoadSprite(card.imageFileName, cardFileInfo);

        private static Sprite LoadSprite(string fileName, FileInfo fileInfo)
        {
            var filePath = string.IsNullOrWhiteSpace(fileName) ?
                fileInfo.FullName.Replace("json", "png", StringComparison.OrdinalIgnoreCase) :
                Path.Combine(fileInfo.Directory.FullName, fileName);

            if (File.Exists(filePath))
            {
                return PathToSprite(filePath);
            }

            return null;
        }

        private static Sprite PathToSprite(string filePath)
        {
            var rawData = File.ReadAllBytes(filePath);
            var texture = new Texture2D(0, 0);
            texture.LoadImage(rawData);
            texture.filterMode = FilterMode.Point;

            var sprite = TextureToSprite(texture);
            return sprite;
        }

        private static Sprite TextureToSprite(Texture2D texture)
            => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
    }
}
