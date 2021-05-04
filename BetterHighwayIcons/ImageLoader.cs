using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ColossalFramework.UI.UIButton;

namespace BetterHighwayIcons
{
    // Adapted from the Metro Overhaul UI Helper Class
    class ImageLoader
    {
        public static readonly Texture2D Highway3Lane;
        public static readonly Texture2D Highway3LaneBarrier;
        public static readonly Texture2D Highway4LaneBarrier;
        public static readonly Texture2D HighwayRamp;

        static ImageLoader()
        {
            Highway3Lane        = LoadDllResource("Highway3Lane.png", 545, 100);
            Highway3LaneBarrier = LoadDllResource("Highway3LaneBarrier.png", 545, 100);
            Highway4LaneBarrier = LoadDllResource("Highway4LaneBarrier.png", 545, 100);
            HighwayRamp         = LoadDllResource("HighwayRamp.png", 545, 100);
        }

        private static Texture2D LoadDllResource(string resourceName, int width, int height)
        {
            try
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream("BetterHighwayIcons.Resources." + resourceName);

                Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                texture.LoadImage(ReadToEnd(myStream));

                return texture;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.StackTrace.ToString());
                return null;
            }
        }

        static byte[] ReadToEnd(Stream stream)
        {
            var originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                var readBuffer = new byte[4096];

                var totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead != readBuffer.Length)
                        continue;

                    var nextByte = stream.ReadByte();
                    if (nextByte == -1)
                        continue;

                    var temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }

                var buffer = readBuffer;
                if (readBuffer.Length == totalBytesRead)
                    return buffer;

                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace.ToString());
                return null;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        // Generates a texture atlas for a standard prefab button from a single image file.
        public static UITextureAtlas GenerateAtlas(string name, Texture2D texture)
        {
            // Generate Sprite Names
            string[] spriteStates = new[] { "", "Disabled", "Focused", "Hovered", "Pressed" };
            string[] spriteNames = new string[5];
            for (int i = 0; i < spriteNames.Length; i++)
            {
                spriteNames[i] = name + spriteStates[i].ToString();
            }

            // Create a new texture atlas
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            atlas.padding = 0;
            atlas.name = name;

            // Shader assignment
            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null)
                atlas.material = new Material(shader);
            atlas.material.mainTexture = texture;

            // Assign regions and names
            for (int i = 0; i < 5; i++)
            {
                float y = 1;
                float x = (float)i / (float)5;

                // Create a new sprite
                UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    region = new Rect(x, y, 0.2f, 1.0f)
                };

                // Define sprite size and offsets
                int spriteWidth = 109;
                int spriteHeight = 100;
                int spriteOffsetY = 0;
                int spriteOffsetX = (int)((float)texture.width * sprite.region.x);

                // Create and fill a new texture
                Texture2D spriteTexture = new Texture2D(spriteWidth, spriteHeight);
                spriteTexture.SetPixels(texture.GetPixels(spriteOffsetX, spriteOffsetY, spriteWidth, spriteHeight));
                sprite.texture = spriteTexture;

                // Add the finished sprite to the atlas
                atlas.AddSprite(sprite);
            }
            return atlas;
        }
    }
}
