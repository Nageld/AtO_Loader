using System;
using System.IO;
using UnityEngine;

namespace AtO_Loader.Utils
{
    public static class SpriteUtils
    {
        public static void LoadSprite(this CardDataWrapper card, FileInfo cardFileInfo)
        {
            var filePath = string.IsNullOrWhiteSpace(card.ImageFileName) ?
                cardFileInfo.FullName.Replace("json", "png", StringComparison.OrdinalIgnoreCase) :
                Path.Combine(cardFileInfo.Directory.FullName, card.ImageFileName);

            if (File.Exists(filePath))
            {
                card.Sprite = PathToSprite(filePath);
            }
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

        private static Sprite TextureToSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
    }
}
