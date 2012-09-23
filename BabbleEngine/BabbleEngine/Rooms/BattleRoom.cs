using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BabbleEngine
{
    public class RoomBattle : Room
    {
        public RoomBattle()
        {
            this.Load("Content/Levels/battle.lvl");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Block b in this.blocks)
            {
                DrawHelper.DrawRectangle(spriteBatch, b.position - camera, b.size, Color.LawnGreen); 
            }
            base.Draw(spriteBatch);
        }
    }
}
