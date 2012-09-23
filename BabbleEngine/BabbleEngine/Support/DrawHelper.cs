using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BabbleEngine
{
    /// <summary>
    /// A helper class for drawing objects and shapes.
    /// It is dependent on the TextureBin class.
    /// </summary>
    public static class DrawHelper
    {
        public static Rectangle BuildRectangle(Vector2 Position, Vector2 Size)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
        {
            spriteBatch.Draw(TextureBin.Pixel, new Rectangle(x, y, width, height), color);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
        {
            spriteBatch.Draw(TextureBin.Pixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
        }

        public static void DrawRectangleOutline(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
        {
            DrawLine(spriteBatch, new Vector2(x, y), new Vector2(x + width, y), color);
            DrawLine(spriteBatch, new Vector2(x + width, y), new Vector2(x + width, y + height), color);
            DrawLine(spriteBatch, new Vector2(x + width, y + height), new Vector2(x, y + height), color);
            DrawLine(spriteBatch, new Vector2(x, y + height), new Vector2(x, y), color);
        }

        public static void DrawRectangleOutline(SpriteBatch spriteBatch, Vector2 pos, Vector2 size, Color color)
        {
            DrawLine(spriteBatch, new Vector2(pos.X, pos.Y), new Vector2(pos.X + size.X, pos.Y), color);
            DrawLine(spriteBatch, new Vector2(pos.X + size.X, pos.Y), new Vector2(pos.X + size.X, pos.Y + size.Y), color);
            DrawLine(spriteBatch, new Vector2(pos.X + size.X, pos.Y + size.Y), new Vector2(pos.X, pos.Y + size.Y), color);
            DrawLine(spriteBatch, new Vector2(pos.X, pos.Y + size.Y), new Vector2(pos.X, pos.Y), color);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Color color, float width = 1)
        {
            float distance = Vector2.Distance(vector1, vector2) + width - 1;
            float angle = (float)Math.Atan2((double)(vector2.Y - vector1.Y), (double)(vector2.X - vector1.X));
            spriteBatch.Draw(TextureBin.Pixel, vector1, null, color, angle, Vector2.Zero, new Vector2(distance, width), SpriteEffects.None, 0);
        }
    }
}