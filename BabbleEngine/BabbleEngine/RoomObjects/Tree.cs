using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BabbleEngine.RoomObjects
{
    public class Tree : WorldObject
    {
        // minimum number of branches
        const int MIN_BRANCHES = 4;
        const int MAX_BRANCHES = 10;

        // how far off the ground branches can start growing
        // from the tree
        const float BRANCH_STARTING_HEIGHT = 20;

        public float Height;
        List<Tree> branches = new List<Tree>();

        /// <summary>
        /// Makes a new tree at the given position in the given room.
        /// </summary>
        /// <param name="Position">The starting position of the tree.</param>
        /// <param name="Room">The room in which the tree is located.</param>
        public Tree(Vector2 Position, float Height, Room Room, float Rotation=0)
            : base(Position, Vector2.Zero, TextureBin.GetTexture("Objects/tree"), Room)
        {
            this.Height = Height;

            // add in a random number of branches
            Random random = new Random();
            int numBranches = random.Next(MIN_BRANCHES, MAX_BRANCHES);
            float branchSeparationDistance = (Height - BRANCH_STARTING_HEIGHT) / numBranches; 

            for (int i = 0; i < numBranches; i++)
            {
                // the side of the tree the branch is located on
                int facing = random.Next(0, 1) == 1 ? 1 : -1;
                
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            DrawHelper.DrawLine(spriteBatch, position, new Vector2(position.X, position.Y - Height), Color.Brown);
        }
    }
}
