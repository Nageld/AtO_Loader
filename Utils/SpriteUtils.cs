using System.IO;
using UnityEngine;

namespace AtO_Loader.Utils
{
    public static class SpriteUtils
    {
        public static void LoadSprite(this CardData card, string filePath)
        {
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

            return TextureToSprite(texture);
        }

        private static Sprite TextureToSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
    }
}
