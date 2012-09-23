using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BabbleEngine
{
    public class Player : WorldObject
    {
        const float walkSpeed = 6f;
        const float jumpPower = -12;

        bool onGround = false;
        float slopeSlow = 1f;
        float slopeSign = 1f;

        public Keys KeyLeft = Keys.Left;
        public Keys KeyRight = Keys.Right;
        public Keys KeyJump = Keys.Up;

        public Player(Vector2 position, Room room)
            : base(position, Vector2.One * 32, TextureBin.Pixel, room)
        {

        }

        protected override void TouchedFloor(IFloor floor)
        {
            onGround = true;
            slopeSlow = floor.GetSlopeSlow();
            slopeSign = floor.GetSlopeSign();
            base.TouchedFloor(floor);
        }

        public override void Update()
        {
            // Handle X collisions.
            if (Input.IsKeyDown(KeyLeft))
                this.velocity.X -= slopeSign > 0 && onGround ? walkSpeed * slopeSlow : walkSpeed;
            if (Input.IsKeyDown(KeyRight))
                this.velocity.X += slopeSign < 0 && onGround ? walkSpeed * slopeSlow : walkSpeed;
            this.MoveX();
            this.velocity.X = 0;

            // Handle Y collisions.
            this.velocity.Y += Room.gravity;

            if (!onGround)
                this.MoveY();
            else
            {
                this.velocity.Y = 8;
                onGround = false;
                this.MoveY(); // Sets onGround to true if ground is found.
                if (!onGround)
                {
                    this.position.Y -= 8;
                    this.velocity.Y = 0;
                }
            }

            // Allows jumping.
            if (onGround && Input.IsKeyDown(KeyJump))
            {
                onGround = false;
                this.velocity.Y = jumpPower;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
