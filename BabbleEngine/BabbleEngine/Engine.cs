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
    /// <summary>
    /// This is the main game engine, it runs the main game loop and controls
    /// the screen drawing.
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        public static readonly Vector2 RESOLUTION = new Vector2(1024, 768);
        public static int WINDOW_WIDTH { get { return (int)RESOLUTION.X; } }
        public static int WINDOW_HEIGHT { get { return (int)RESOLUTION.Y; } }

        // This allows you to get singleton access to the Engine class.
        private static readonly Engine instance = new Engine();
        public static Engine Instance { get { return instance; } }

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Room currentRoom;

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

            // Load the current room here.
            currentRoom = new RoomBattle();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureBin.LoadTextures(this.Content);
        }

        /// <summary>
        /// The main update method for the entire game.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (Input.KeyboardTapped(Keys.Escape) && Input.IsKeyDown(Keys.Enter))
                this.Exit();

            if (Input.KeyboardTapped(Keys.F12) && Input.IsKeyDown(Keys.Enter))
                currentRoom = new RoomEditor();

            currentRoom.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// The main draw method for the game.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            currentRoom.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
