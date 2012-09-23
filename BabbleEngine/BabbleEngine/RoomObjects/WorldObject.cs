using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BabbleEngine
{
    /// <summary>
    /// An object that exists dynamically in the world.
    /// It should have movement and possibly collisions.
    /// </summary>
    public class WorldObject
    {
        // Public accessors 
        public Vector2 position;
        public Vector2 size;
        public Vector2 velocity;
        protected Vector2 prevPosition;

        // Accessors for specific dimensions of the object.
        public int Left { get { return (int)position.X; } set { this.position.X = value; } }
        public int Right { get { return (int)(position.X + size.X); } set { this.position.X = value - this.size.X; } }
        public int Top { get { return (int)position.Y; } set { this.position.Y = value; } }
        public int Bottom { get { return (int)(position.Y + size.Y); } set { this.position.Y = value - this.size.Y; } }
        public Vector2 Center { get { return this.position + this.size / 2; } }

        public Texture2D texture;
        protected Room room;

        public WorldObject(Vector2 position, Vector2 size, Texture2D texture, Room room)
        {
            this.position = position;
            this.size = size;
            this.texture = texture;
            this.room = room;
        }

        protected virtual void MoveX()
        {
            this.position.X += this.velocity.X;
            foreach (Block b in room.blocks)
            {
                if (b.IntersectsWith(this))
                {
                    if (this.velocity.X > 0)
                        this.Right = (int)b.Left;
                    else
                        this.Left = (int)b.Right;
                }
            }
        }

        protected virtual void MoveY()
        {
            this.position.Y += this.velocity.Y;

            IFloor floorHit = null;
            foreach (Block b in room.blocks)
            {
                if (b.IntersectsWith(this))
                {
                    if (!b.topSolidOnly)
                    {
                        // If it is a regular block.
                        if (this.velocity.Y > 0)
                        {
                            this.Bottom = (int)b.Top;
                            floorHit = b;
                        }
                        else
                        {
                            this.velocity.Y = 0;
                            this.Top = (int)b.Bottom;
                        }
                    }
                    else
                    {
                        // If it is a block that is only solid on the top.
                        if (this.velocity.Y > 0 && this.Top - this.velocity.Y > b.Top)
                        {
                            this.Bottom = (int)b.Top;
                            floorHit = b;
                        }
                    }
                }
            }

            // This 
            foreach (Floor floor in room.floors)
            {
                float? line = floor.FindYLineAboveFloor(this);

                if (line != null && this.Bottom > (float)line && this.Top <= (float)line)
                {
                    this.Bottom = (int)(float)line;
                    floorHit = floor;
                }
            }

            if (floorHit != null)
                TouchedFloor(floorHit);
        }

        protected virtual void TouchedFloor(IFloor floor)
        {
            this.velocity.Y = 0;
        }

        /// <summary>
        /// Make sure this is called at the end of overridden methods.
        /// </summary>
        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, DrawHelper.BuildRectangle(position - room.camera, size), Color.White);
            this.prevPosition = this.position;
        }
    }
}
