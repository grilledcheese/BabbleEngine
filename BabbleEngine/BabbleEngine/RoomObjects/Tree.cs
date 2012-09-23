using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BabbleEngine.RoomObjects
{
    public class Tree : WorldObject
    {
        // how far off the ground branches can start growing
        // from the tree
        const float BRANCH_STARTING_HEIGHT = 20;
        
        // how much branches can be rotated by
        const float BRANCH_ROTATION_MIN = 0;
        const float BRANCH_ROTATION_MAX = MathHelper.Pi / 4;

        public float Height;
        float rotation;
        List<Tree> branches = new List<Tree>();

        /// <summary>
        /// Makes a new tree at the given position in the given room.
        /// </summary>
        /// <param name="Position">The starting position of the tree.</param>
        /// <param name="Room">The room in which the tree is located.</param>
        public Tree(Vector2 Position, float Height, float numBranches, Room Room, float Rotation=0)
            : base(Position, Vector2.Zero, TextureBin.GetTexture("Objects/tree"), Room)
        {
            this.Height = Height;

            // add in a random number of branches
            Random random = new Random();
            float branchSeparationDistance = (Height - BRANCH_STARTING_HEIGHT) / numBranches; 

            for (int i = 0; i < numBranches; i++)
            {
                // the side of the tree the branch is located on
                // 1 for left, 0 for right.
                int facing = random.Next(0, 1);

                Vector2 branchPosition = new Vector2(facing == 1 ? Position.X : Position.X + texture.Width,
                    i * branchSeparationDistance + BRANCH_STARTING_HEIGHT);
                branches.Add(new Tree(branchPosition, Height / 4, numBranches / 4, Room,
                    rotation + (float)(facing == 1 ? BRANCH_ROTATION_MIN + MathHelper.TwoPi - MathHelper.PiOver2 + random.NextDouble() * BRANCH_ROTATION_MAX :
                                                BRANCH_ROTATION_MIN + MathHelper.PiOver2 + random.NextDouble() * BRANCH_ROTATION_MAX)));
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            foreach (Tree branch in branches)
                branch.Draw(spriteBatch);
            DrawHelper.DrawLine(spriteBatch, position, new Vector2(position.X, position.Y - Height), Color.Brown);
        }
    }
}
