using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BabbleEngine
{
    public struct Floor : IFloor
    {
        public Vector2 pointL;
        public Vector2 pointR;

        public Floor(Vector2 point1, Vector2 point2)
        {
            // Ensures that the pointL is left of pointR.
            if (point1.X < point2.X)
            {
                this.pointL = point1;
                this.pointR = point2;
            }
            else if (point1.X > point2.X)
            {
                this.pointL = point2;
                this.pointR = point1;
            }
            else
            {
                throw new Exception("Cannot create a verticle floor.");
            }
        }

        /// <summary>
        /// Returns a Y-value that represents a flat line above the slope that
        /// the other object could stand on.
        /// </summary>
        public float? FindYLineAboveFloor(WorldObject other)
        {
            float line = float.PositiveInfinity;

            float h;
            if (other.Left >= this.pointL.X && other.Left <= this.pointR.X)
            {
                // The left edge if the object is within range.
                h = FindHeightGivenX(other.Left);
                line = line < h ? line : h;
            }
            if (other.Right >= this.pointL.X && other.Right <= this.pointR.X)
            {
                // The right edge of the object is within range.
                h = FindHeightGivenX(other.Right);
                line = line < h ? line : h;
            }
            if (this.pointL.X >= other.Left && this.pointL.X <= other.Right)
            {
                // The left point of the floor intersects with the object.
                line = line < this.pointL.Y ? line : this.pointL.Y;
            }
            if (this.pointR.X >= other.Left && this.pointR.X <= other.Right)
            {
                // The right point of the floor intersects with the object.
                line = line < this.pointR.Y ? line : this.pointR.Y;
            }

            if (float.IsInfinity(line))
                return null;
            else
                return line;
        }

        public float FindHeightGivenX(float x)
        {
            return pointL.Y + (pointR - pointL).Y * ((x - pointL.X) / (pointR.X - pointL.X));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            DrawHelper.DrawLine(spriteBatch, pointL - camera, pointR - camera, Color.Yellow);
        }

        public float GetSlopeSign()
        {
            return Math.Sign(pointR.Y - pointL.Y);
        }

        public float GetSlopeSlow()
        {
            return (float)Math.Max(1 - Math.Abs((pointR - pointL).Y / (pointR.X - pointL.X)) / 1.7f, 0.3f);
        }
    }
}
