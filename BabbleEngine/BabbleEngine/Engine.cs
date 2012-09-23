using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BabbleEngine
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        public static readonly Vector2 RESOLUTION = new Vector2(1024, 768);
        public static int WINDOW_WIDTH { get { return (int)RESOLUTION.X; } }
        public static int WINDOW_HEIGHT { get { return (int)RESOLUTION.Y; } }

        private static readonly Engine instance = new Engine();
        public static Engine Instance { get { return instance; } }

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Room room;

        private Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            base.Initialize();
            this.IsMouseVisible = true;
            this.graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            this.graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            this.graphics.ApplyChanges();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureBin.LoadTextures(this.Content);

            room = new RoomEditor();
            Player p = new Player(Vector2.Zero, room);
            room.objects.Add(p);
            room.cameraTarget = p;
        }


        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            if (Input.KeyboardTapped(Keys.Escape) && Input.IsKeyDown(Keys.Enter))
                this.Exit();

            room.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            room.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
