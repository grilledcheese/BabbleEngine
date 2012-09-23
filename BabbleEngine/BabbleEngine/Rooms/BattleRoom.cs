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
            this.FillNodes();
        }

        public void FillNodes()
        {
            foreach(Vector2 p in nodes.GetNodeList("player"))
            {
                Player pl = new Player(p, this);
                this.cameraTargets.Add(pl);
                objects.Add(pl);
            }
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
