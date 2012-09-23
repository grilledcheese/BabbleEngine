using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BabbleEngine
{
    /// <summary>
    /// Decals represent background or forground objects in this world.
    /// They have a lot of properties to customize.
    /// </summary>
    public struct Decal : IComparable
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 origin;
        public float scale;
        public float angle;
        public float drift;
        public Color color;

        public Decal(Vector2 position, Texture2D texture, Vector2 origin, Color color, float scale = 2, float angle = 0f, float drift = 1f)
        {
            this.position = position;
            this.texture = texture;
            this.origin = origin;
            this.scale = scale;
            this.angle = angle;
            this.drift = drift;
            this.color = color;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            Vector2 newCamera = origin + (camera - origin) * drift;
            spriteBatch.Draw(texture, position - newCamera, null, color * (color.A / 255f), angle, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera, Color color)
        {
            Vector2 newCamera = origin + (camera - origin) * drift;
            spriteBatch.Draw(texture, position - newCamera, null, color, angle, Vector2.Zero, scale, SpriteEffects.None, 0f);   
        }

        public int CompareTo(object obj)
        {
            return Math.Sign(this.drift - ((Decal)obj).drift);
        }
    }
}
