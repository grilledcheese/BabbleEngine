using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BabbleEngine
{
    /// <summary>
    /// A block is a simple type of collision object that cannot be passed by proper
    /// WorldObject classes. 
    /// </summary>
    public struct Block : IFloor
    {
        public Vector2 position;
        public Vector2 size;

        public float Left { get { return position.X; } }
        public float Right { get { return (position.X + size.X); } }
        public float Top { get { return position.Y; } }
        public float Bottom { get { return (position.Y + size.Y); } }

        public Block(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            DrawHelper.DrawRectangleOutline(spriteBatch, position.X - camera.X, position.Y - camera.Y, size.X, size.Y, Color.Yellow);
        }

        public bool IntersectsWith(WorldObject other)
        {
            return (this.Right > other.Left && this.Left < other.Right && this.Bottom > other.Top && this.Top < other.Bottom);
        }

        public float GetSlopeSign()
        {
            return 1f;
        }

        public float GetSlopeSlow()
        {
            return 1;
        }
    }
}
