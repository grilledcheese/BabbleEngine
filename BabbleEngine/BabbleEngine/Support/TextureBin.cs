using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework;

namespace BabbleEngine
{
    public static class TextureBin
    {
        private static Dictionary<String, Texture2D> textureDic = new Dictionary<string, Texture2D>();
        private static List<String> textureNames = new List<string>() {
            "pixel" };
        public static SpriteFont mainFont;
        public static Texture2D Pixel { get { return GetTexture("pixel"); } }

        public static Texture2D GetTexture(String name)
        {
            return textureDic[name];
        }

        public static Dictionary<String, Texture2D> GetDictionary
        {
            get { return textureDic; }
        }

        public static void LoadTextures(ContentManager content)
        {
            // Load fonts.
            mainFont = content.Load<SpriteFont>("mainFont");

            // Load textures.
            foreach (String name in textureNames)
            {
                Texture2D t = content.Load<Texture2D>(name);
                t.Name = name;
                textureDic.Add(name, t);
            }
        }

        /// <summary>
        /// Returns the number of new textures loaded.
        /// </summary>
        public static int LoadTexturesFromDirectory(String path)
        {
            String[] files = Directory.GetFiles(path, "*.png");
            int count = 0;
            foreach (String filename in files)
            {
                if (LoadTextureFromStream(filename) == 0)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns 0 if the texture loads normally.
        /// Returns 1 if the texture already exists.
        /// Returns 2 if the file cannot be found.
        /// Returns 3 for any other failure.
        /// 
        /// This function loads textures without the content pipeline with alpha.
        /// This code is copied from the following tutorial:
        /// http://jakepoz.com/jake_poznanski__speeding_up_xna.html
        /// </summary>
        /// <param name="graphics">The graphics device for the game</param>
        /// <param name="path">The path to the file.</param>
        /// <returns>A texture with alpha values.</returns>
        public static int LoadTextureFromStream(String path)
        {
            Texture2D file = null;
            RenderTarget2D result = null;

            try
            {
                using (Stream titleStream = TitleContainer.OpenStream(path))
                {
                    file = Texture2D.FromStream(Engine.Instance.GraphicsDevice, titleStream);
                }
            }
            catch (FileNotFoundException)
            {
                return 2;
            }
            catch (Exception)
            {
                return 3;
            }

            // Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(Engine.Instance.GraphicsDevice, file.Width, file.Height);

            Engine.Instance.GraphicsDevice.SetRenderTarget(result);
            Engine.Instance.GraphicsDevice.Clear(Color.Black);

            // Multiply each color by the source alpha, and write in just the color values into the final texture
            BlendState blendColor = new BlendState();
            blendColor.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;

            blendColor.AlphaDestinationBlend = Blend.Zero;
            blendColor.ColorDestinationBlend = Blend.Zero;

            blendColor.AlphaSourceBlend = Blend.SourceAlpha;
            blendColor.ColorSourceBlend = Blend.SourceAlpha;

            SpriteBatch spriteBatch = new SpriteBatch(Engine.Instance.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End();

            // Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
            BlendState blendAlpha = new BlendState();
            blendAlpha.ColorWriteChannels = ColorWriteChannels.Alpha;

            blendAlpha.AlphaDestinationBlend = Blend.Zero;
            blendAlpha.ColorDestinationBlend = Blend.Zero;

            blendAlpha.AlphaSourceBlend = Blend.One;
            blendAlpha.ColorSourceBlend = Blend.One;

            spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End();

            // Release the GPU back to drawing to the screen
            Engine.Instance.GraphicsDevice.SetRenderTarget(null);

            Texture2D texture = result as Texture2D;
            texture.Name = Path.GetFileNameWithoutExtension(path);
            if (!textureDic.ContainsKey(texture.Name))
                textureDic.Add(texture.Name, texture);
            else
                return 1;
            return 0;
        }
    }
}
