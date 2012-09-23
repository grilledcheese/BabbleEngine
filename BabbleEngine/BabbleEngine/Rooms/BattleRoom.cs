using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
                Player p2 = new Player(p, this);
                p2.KeyJump = Keys.W;
                p2.KeyLeft = Keys.A;
                p2.KeyRight = Keys.D;
                this.cameraTargets.Add(p2);
                objects.Add(p2);
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
