using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BabbleEngine
{
    public class Room
    {
        public String name = "";

        public const float gravity = 0.5f;

        public BufferedList<WorldObject> objects = new BufferedList<WorldObject>();
        public List<Floor> floors = new List<Floor>();
        public List<Block> blocks = new List<Block>();
        public BufferedList<Decal> decalsFront = new BufferedList<Decal>();
        public BufferedList<Decal> decalsBack = new BufferedList<Decal>();
        public NodeManager nodes = new NodeManager();

        public Vector2 camera = -Engine.RESOLUTION / 2;
        public List<WorldObject> cameraTargets = new List<WorldObject>();

        public Vector2 boundsTopLeft = -Engine.RESOLUTION;
        public Vector2 boundsBottomRight = Engine.RESOLUTION;

        public virtual void Reset()
        {
            this.floors = new List<Floor>();
            this.blocks = new List<Block>();
            this.cameraTarget = null;
            this.objects = new BufferedList<WorldObject>();
        }

        public virtual void Update()
        {
            if (cameraTargets != null)
            {
                // Sum and average all of the camera targets.
                Vector2 dest = Vector2.Zero;
                foreach (WorldObject t in cameraTargets)
                    dest += t.Center;
                dest /= cameraTargets.Count;
                
                dest -= Engine.RESOLUTION / 2;
                camera += (dest - camera) / 8f;
            }

            if (camera.X < boundsTopLeft.X)
                camera.X = boundsTopLeft.X;
            if (camera.X + Engine.RESOLUTION.X > boundsBottomRight.X)
                camera.X = boundsBottomRight.X - Engine.RESOLUTION.X;
            if (boundsBottomRight.X - boundsTopLeft.X < Engine.RESOLUTION.X)
                camera.X = boundsTopLeft.X;

            if (camera.Y < boundsTopLeft.Y)
                camera.Y = boundsTopLeft.Y;
            if (camera.Y + Engine.WINDOW_HEIGHT > boundsBottomRight.Y)
                camera.Y = boundsBottomRight.Y - Engine.WINDOW_HEIGHT;
            if (boundsBottomRight.Y - boundsTopLeft.Y < Engine.RESOLUTION.Y)
                camera.Y = boundsTopLeft.Y;

            foreach (WorldObject obj in objects)
                obj.Update();

            objects.ApplyBuffers();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (Decal dec in decalsBack)
                dec.Draw(spriteBatch, camera);
            foreach (WorldObject obj in objects)
                obj.Draw(spriteBatch);
            foreach (Decal dec in decalsFront)
                dec.Draw(spriteBatch, camera);
        }

        /// <summary>
        /// This cameraTarget exists for compatiblity with older systems.
        /// </summary>
        public WorldObject cameraTarget
        {
            get { return cameraTargets.Count == 0 ? null : cameraTargets[0]; }
            set
            {
                cameraTargets.Clear();
                cameraTargets.Add(value);
            }
        }

        /// <summary>
        /// Returns 2 if cannot find the file.
        /// Returns 0 if everything is OK.
        /// Returns 3 if it had to load new textures.
        /// Returns 4 if we are missing textures.
        /// Returns -1 if the file is corrupt.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public int Load(string fname)
        {
            // Reads in the entire file.
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(fname);
            }
            catch
            {
                return 2;
            }

            Reset();

            // Sets file index to 10 (skip header)
            int i = 0;
            name = lines[i++];
            this.boundsTopLeft.X = float.Parse(lines[i++]);
            this.boundsTopLeft.Y = float.Parse(lines[i++]);
            this.boundsBottomRight.X = float.Parse(lines[i++]);
            this.boundsBottomRight.Y = float.Parse(lines[i++]);
            i = 10;
            int flag = 0;

            while (i < lines.Length)
            {
                string type = lines[i++];
                switch (type)
                {
                    case "Floor":
                        floors.Add(new Floor(new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++])), new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++]))));
                        break;
                    case "Block":
                        blocks.Add(new Block(new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++])), new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++]))));
                        break;
                    case "Player":
                        Player p = new Player(new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++])), this);
                        objects.Add(p);
                        this.cameraTarget = p;
                        break;
                    case "WorldObject":
                        objects.Add(new WorldObject(new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++])), new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++])), TextureBin.GetTexture(lines[i++]), this));
                        break;
                    case "Decal":
                        bool isFront = (lines[i++] == "F");
                        Vector2 pos = new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++]));
                        Color col = new Color(int.Parse(lines[i++]), int.Parse(lines[i++]), int.Parse(lines[i++]), int.Parse(lines[i++]));
                        Vector2 ori = new Vector2(float.Parse(lines[i++]), float.Parse(lines[i++]));

                        Texture2D tex;
                        if (!TextureBin.GetDictionary.ContainsKey(lines[i]))
                        {
                            if (TextureBin.LoadTextureFromStream("Content\\Decals\\" + lines[i] + ".png") == 0)
                            {
                                tex = TextureBin.GetTexture(lines[i]);
                                flag = 3;
                            }
                            else
                            {
                                tex = TextureBin.Pixel;
                                flag = 4;
                            }

                        }
                        else
                            tex = TextureBin.GetTexture(lines[i]);
                        i++;

                        Decal d = new Decal(pos, tex, ori, col, float.Parse(lines[i++]), float.Parse(lines[i++]), float.Parse(lines[i++]));
                        if (isFront)
                            decalsFront.Add(d);
                        else
                            decalsBack.Add(d);
                        break;
                    default:
                        i = lines.Length;
                        flag = -1;
                        break;
                }
            }

            return flag;
        }

        public int Save(string fname)
        {
            int flag = 0;
            List<string> lines = new List<string>();

            // Adds 10 lines of header that could be used later.
            lines.Add(name);
            lines.Add(boundsTopLeft.X.ToString());
            lines.Add(boundsTopLeft.Y.ToString());
            lines.Add(boundsBottomRight.X.ToString());
            lines.Add(boundsBottomRight.Y.ToString());
            for (int i = 5; i < 10; i++)
                lines.Add("headerline" + i.ToString());

            foreach (WorldObject obj in objects)
            {
                if (obj is Player)
                {
                    lines.Add("Player");
                    lines.Add(obj.position.X.ToString());
                    lines.Add(obj.position.Y.ToString());
                }
                else
                {
                    flag = 1;
                    lines.Add("WorldObject");
                    lines.Add(obj.position.X.ToString());
                    lines.Add(obj.position.Y.ToString());
                    lines.Add(obj.size.X.ToString());
                    lines.Add(obj.size.Y.ToString());
                    lines.Add(obj.texture.Name);
                }
            }
            foreach (Decal decal in decalsFront)
            {
                lines.Add("Decal");
                lines.Add("F");
                lines.Add(decal.position.X.ToString());
                lines.Add(decal.position.Y.ToString());
                lines.Add(decal.color.R.ToString());
                lines.Add(decal.color.G.ToString());
                lines.Add(decal.color.B.ToString());
                lines.Add(decal.color.A.ToString());
                lines.Add(decal.origin.X.ToString());
                lines.Add(decal.origin.Y.ToString());
                lines.Add(decal.texture.Name);
                lines.Add(decal.scale.ToString());
                lines.Add(decal.angle.ToString());
                lines.Add(decal.drift.ToString());
            }
            foreach (Decal decal in decalsBack)
            {
                lines.Add("Decal");
                lines.Add("B");
                lines.Add(decal.position.X.ToString());
                lines.Add(decal.position.Y.ToString());
                lines.Add(decal.color.R.ToString());
                lines.Add(decal.color.G.ToString());
                lines.Add(decal.color.B.ToString());
                lines.Add(decal.color.A.ToString());
                lines.Add(decal.origin.X.ToString());
                lines.Add(decal.origin.Y.ToString());
                lines.Add(decal.texture.Name);
                lines.Add(decal.scale.ToString());
                lines.Add(decal.angle.ToString());
                lines.Add(decal.drift.ToString());
            }
            foreach (Floor floor in floors)
            {
                lines.Add("Floor");
                lines.Add(floor.pointL.X.ToString());
                lines.Add(floor.pointL.Y.ToString());
                lines.Add(floor.pointR.X.ToString());
                lines.Add(floor.pointR.Y.ToString());
            }
            foreach (Block b in blocks)
            {
                lines.Add("Block");
                lines.Add(b.position.X.ToString());
                lines.Add(b.position.Y.ToString());
                lines.Add(b.size.X.ToString());
                lines.Add(b.size.Y.ToString());
            }
            System.IO.File.WriteAllLines(fname, lines.ToArray());
            return flag;
        }
    }
}
