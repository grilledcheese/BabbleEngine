using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BabbleEngine
{
    public class RoomEditor : Room
    {
        private enum EditorMode
        {
            GeneralMode,
            FloorMode,
            SquareMode,
            DecalMode,
            NodeMode,
            CameraMode
        }

        private enum DecalLayer
        {
            Back,
            Front
        }

        RenderTarget2D editorSurface;
        float zoom = 1;

        float snapSize = 32;
        Vector2 snapMouse;
        Vector2 decalOrigin;

        Vector2? point1;
        Vector2 blockSize = Vector2.One * 32;
        Texture2D decal = null;
        float decalScale = 2;
        float decalAngle = 0;
        float decalDrift = 1;
        Color decalColor = Color.White;
        DecalLayer decalLayer = DecalLayer.Back;
        String nodeString = "";

        bool pauseGameplay = true;
        bool showCollision = true;
        EditorMode mode = EditorMode.GeneralMode;

        #region Mode Text
        List<String> modeText = new List<string> {
            "GENERAL CONTROLS:\nRight Mouse - Move camera\nRight Mouse + Scroll - Zoom camera\nKEY0 to KEY9 - Change mode\nTab - Unsnap from grid\n~ Key - Toggle edit/test mode\nF1 - Toggle debug information\n\n" +
                "GENERAL MODE CONTROLS:\nLeft Click - Place player\nS - Save\nL - Load\nF - See Saved Files",
            "Left Click - Start and finist lines\nBackspace - Undo last line\nLControl + Left Click - Delete line by endpoint",
            "Left Click - Place block\nLControl + Left Click - Delete block\nUp and Down - Adjust block size",
            "Enter - Select decal\nLeft Click - Place decal\nLControl + Left Click - Delete decal\n\nMouseWheel/Up/Down - Adjust scale\nA + MouseWheel/Up/Down - Adjust rotation\n" +
                "D + MouseWheel/Up/Down - Adjust drift\nS - Set colour\nLeft/Right - Change decal layer\n\nF - Open texture folder\n(H)elp - Custom Textures and Depth",
            "Left Click - Place node\nSpace - Select node key\nLeft Click + LControl - Delete node",
            "Arrow Keys - Select a new left/right/top/bottom boundary for the camera."
            };
        #endregion

        bool helpInfo = true;
        const float DEBUG_WINDOW_HEIGHT = 32;

        List<TextBox> textBoxStack = new List<TextBox>();

        public RoomEditor()
        {
            editorSurface = new RenderTarget2D(Engine.Instance.GraphicsDevice, Engine.WINDOW_WIDTH * 2, Engine.WINDOW_HEIGHT * 2);
        }

        public override void Update()
        {
            if (Input.KeyboardTapped(Keys.Home))
            {
                camera = -Engine.RESOLUTION / 2;
                zoom = 1;
            }

            if (Input.MouseRightButtonDown)
            {
                if (Input.MouseScrollDown && zoom < 1.99f)
                    zoom += 0.1f;
                else if (Input.MouseScrollUp && zoom > 0.501f)
                    zoom -= 0.1f;
            }

            // Toggle edit mode.
            if (Input.KeyboardTapped(Keys.OemTilde))
                pauseGameplay = !pauseGameplay;

            if (Input.KeyboardTapped(Keys.F1))
                helpInfo = !helpInfo;

            if (Input.KeyboardTapped(Keys.F2))
                showCollision = !showCollision;

            if (!pauseGameplay)
            {
                // Update normal game.
                base.Update();
            }
            else
            {
                // Updates textbox.
                if (textBoxStack.Count > 0)
                {
                    textBoxStack[textBoxStack.Count - 1].Update();
                    return;
                }

                // Update editor stuff.
                if (Input.KeyboardTapped(Keys.D0))
                    mode = EditorMode.GeneralMode;
                if (Input.KeyboardTapped(Keys.D1))
                    mode = EditorMode.FloorMode;
                if (Input.KeyboardTapped(Keys.D2))
                    mode = EditorMode.SquareMode;
                if (Input.KeyboardTapped(Keys.D3))
                    mode = EditorMode.DecalMode;
                if (Input.KeyboardTapped(Keys.D4))
                    mode = EditorMode.NodeMode;
                if (Input.KeyboardTapped(Keys.D5))
                    mode = EditorMode.CameraMode;

                SetSnapMouse();

                // Allows for camera moving
                if (Input.MouseRightButtonDown)
                    this.camera -= (Input.MousePosition - Input.MousePrevPosition);

                switch (mode)
                {
                    case EditorMode.GeneralMode:
                        UpdateGeneralMode();
                        break;

                    case EditorMode.FloorMode:
                        UpdateFloorMode();
                        break;

                    case EditorMode.SquareMode:
                        UpdateSquareMode();
                        break;

                    case EditorMode.DecalMode:
                        UpdateDecalMode();
                        break;

                    case EditorMode.NodeMode:
                        UpdateNodeMode();
                        break;

                    case EditorMode.CameraMode:
                        UpdateCameraMode();
                        break;
                }
            }
        }

        private void UpdateGeneralMode()
        {
            if (Input.MouseLeftButtonTapped)
            {
                this.cameraTarget.position = snapMouse;
                this.cameraTarget.velocity = Vector2.Zero;
            }
            if (Input.KeyboardTapped(Keys.S))
            {
                textBoxStack.Add(new TextInputBox(null, "SAVE: Please type in a filename.", SaveBoxEnter, name, ".lvl"));
            }
            if (Input.KeyboardTapped(Keys.L))
            {
                textBoxStack.Add(new TextInputBox(null, "LOAD: Please type in a filename.", LoadBoxEnter, name, ".lvl"));
            }
            if (Input.KeyboardTapped(Keys.F))
            {
                System.Diagnostics.Process.Start("explorer.exe", "Content\\Levels");
            }
        }

        private void UpdateFloorMode()
        {
            if (Input.MouseLeftButtonTapped)
            {
                if (!Input.IsKeyDown(Keys.LeftControl))
                {
                    if (point1 == null)
                        point1 = snapMouse;
                    else if (point1 == snapMouse)
                        point1 = null;
                    else if (point1.Value.X != snapMouse.X)
                    {
                        floors.Add(new Floor((Vector2)point1, snapMouse));
                        point1 = null;
                    }
                    else
                    {
                        textBoxStack.Add(new TextBox(null, "WARNING: You cannot create verticle lines,\nthey must be used strictly for floors.", TextBoxEnter));
                        point1 = null;
                    }
                }
                else
                {
                    point1 = null;
                    Floor? f = FindLineAtPosition(snapMouse);
                    if (f != null)
                        floors.Remove(f.Value);
                }
            }
            if (Input.KeyboardTapped(Keys.Back))
            {
                if (floors.Count > 0)
                    floors.RemoveAt(floors.Count - 1);
            }
        }

        private void UpdateSquareMode()
        {
            if (Input.KeyboardTapped(Keys.Up))
            {
                blockSize = Vector2.One * 16;
                snapSize = 16;
            }
            if (Input.KeyboardTapped(Keys.Down))
            {
                blockSize = Vector2.One * 32;
                snapSize = 32;
            }

            if (Input.MouseLeftButtonDown)
            {
                if (!Input.IsKeyDown(Keys.LeftControl))
                {
                    Block? b = FindBlockAtPosition(snapMouse);
                    if (b == null)
                    {
                        blocks.Add(new Block(snapMouse, blockSize));
                    }
                }
                else
                {
                    Block? b = FindBlockAtPosition(snapMouse);
                    if (b != null)
                    {
                        blocks.Remove((Block)b);
                    }
                }
            }
        }

        private void UpdateDecalMode()
        {
            if (Input.KeyboardTapped(Keys.S))
            {
                textBoxStack.Add(new SelectColorBox(ColorBoxEnter, decalColor));
            }
            if (Input.KeyboardTapped(Keys.U))
            {
                decalsBack.Sort();
                decalsFront.Sort();
            }
            if (Input.KeyboardTapped(Keys.L))
            {
                int num = TextureBin.LoadTexturesFromDirectory("Content\\Decals");
                textBoxStack.Add(new TextBox(null, "Loaded " + num.ToString() + " new textures.", TextBoxEnter));
            }
            if (Input.KeyboardTapped(Keys.Enter))
            {
                textBoxStack.Add(new SelectTextureBox(TextureBoxEnter));
            }
            if (Input.KeyboardTapped(Keys.F))
            {
                System.Diagnostics.Process.Start("explorer.exe", "Content\\Decals");
            }
            if (Input.KeyboardTapped(Keys.H))
            {
                textBoxStack.Add(new TextBox(null, "HOW TO ADD NEW DECALS: Open the decals folder by pressing F.\nThen drag .PNG images into the decals folder.\nThen return to the game, and press L to load the new images into the decals bin.\n\nFURTHER NOTES: Adjust the drift values on decals to make them look like they have depth.\nWhen placing drifting decals, hold LShift to ensure they are properly aligned\nto a grid.\nIf decals are overlapping incorrectly, press U to sort decals by drift.", TextBoxEnter));
            }
            if (Input.KeyboardTapped(Keys.Left))
            {
                decalLayer = DecalLayer.Back;
            }
            if (Input.KeyboardTapped(Keys.Right))
            {
                decalLayer = DecalLayer.Front;
            }
            if (Input.IsKeyDown(Keys.LeftControl))
            {
                if (Input.MouseLeftButtonTapped)
                {
                    foreach (Decal d in decalsFront)
                    {
                        Vector2 decalOnScreen = d.position - (d.origin + (camera - d.origin) * d.drift);
                        if ((decalOnScreen - (Input.MousePosition * zoom)).Length() < 14)
                            decalsFront.BufferRemove(d);
                    }
                    foreach (Decal d in decalsBack)
                    {
                        Vector2 decalOnScreen = d.position - (d.origin + (camera - d.origin) * d.drift);
                        if ((decalOnScreen - (Input.MousePosition * zoom)).Length() < 14)
                            decalsBack.BufferRemove(d);
                    }
                    decalsFront.ApplyBuffers();
                    decalsBack.ApplyBuffers();
                }
            }
            else
            {
                if (Input.IsKeyDown(Keys.A))
                {
                    if (Input.MouseScrollDown || Input.KeyboardTapped(Keys.Down))
                        decalAngle -= MathHelper.Pi / 12;
                    if (Input.MouseScrollUp || Input.KeyboardTapped(Keys.Up))
                        decalAngle += MathHelper.Pi / 12;
                }
                else if (Input.IsKeyDown(Keys.D))
                {
                    if (Input.MouseScrollDown || Input.KeyboardTapped(Keys.Down))
                        decalDrift -= 0.05f;
                    if (Input.MouseScrollUp || Input.KeyboardTapped(Keys.Up))
                        decalDrift += 0.05f;
                }
                else
                {
                    if (Input.MouseScrollDown || Input.KeyboardTapped(Keys.Down))
                        decalScale -= 0.25f;
                    if (Input.MouseScrollUp || Input.KeyboardTapped(Keys.Up))
                        decalScale += 0.25f;
                }

                if (Input.MouseLeftButtonTapped && decal != null)
                {
                    if (decalLayer == DecalLayer.Front)
                        this.decalsFront.Add(new Decal(snapMouse, decal, decalOrigin, decalColor, decalScale, decalAngle, decalDrift));
                    else
                        this.decalsBack.Add(new Decal(snapMouse, decal, decalOrigin, decalColor, decalScale, decalAngle, decalDrift));
                }
            }
        }

        private void UpdateNodeMode()
        {
            if (Input.KeyboardTapped(Keys.Space))
                textBoxStack.Add(new TextInputBox("", "Please type the name of the node.", NodeBoxEnter));

            if (Input.MouseLeftButtonTapped)
            {
                if (Input.IsKeyUp(Keys.LeftControl))
                {
                    if (nodes.FindFirstNodeAtPosition(snapMouse) == null && nodeString != "")
                        nodes.AddNode(nodeString, snapMouse);
                }
                else
                {
                    nodes.RemoveFirstNodeAtPosition(snapMouse);
                }
            }
        }

        private void UpdateCameraMode()
        {
            if (Input.IsKeyDown(Keys.Left))
                boundsTopLeft.X = snapMouse.X;
            if (Input.IsKeyDown(Keys.Up))
                boundsTopLeft.Y = snapMouse.Y;
            if (Input.IsKeyDown(Keys.Right))
                boundsBottomRight.X = snapMouse.X;
            if (Input.IsKeyDown(Keys.Down))
                boundsBottomRight.Y = snapMouse.Y;
        }

        private void ColorBoxEnter(TextBox box)
        {
            textBoxStack.RemoveAt(textBoxStack.Count - 1);
            decalColor = ((SelectColorBox)box).color;
        }

        private void NodeBoxEnter(TextBox box)
        {
            textBoxStack.RemoveAt(textBoxStack.Count - 1);
            TextInputBox tb = (TextInputBox)box;
            nodeString = tb.field.inputString;
        }

        private void TextureBoxEnter(TextBox box)
        {
            textBoxStack.RemoveAt(textBoxStack.Count - 1);
            SelectTextureBox tb = (SelectTextureBox)box;
            decal = tb.texture;
        }

        private void TextBoxEnter(TextBox box)
        {
            textBoxStack.RemoveAt(textBoxStack.Count - 1);
        }

        private void SaveBoxEnter(TextBox box)
        {
            // Set the name of the level.
            name = ((TextInputBox)box).field.inputString;

            textBoxStack.RemoveAt(textBoxStack.Count - 1);
            if (((TextInputBox)box).field.inputString == "")
            {
                textBoxStack.Add(new TextBox(null, "ERROR: Cannot save to blank filename.", TextBoxEnter));
                return;
            }
            if (this.Save("Content/Levels/" + ((TextInputBox)box).field.inputString + ".lvl") == 1)
            {
                textBoxStack.Add(new TextBox(null, "WARNING: Generic world objects saved.", TextBoxEnter));
            }
        }

        private void LoadBoxEnter(TextBox box)
        {
            textBoxStack.RemoveAt(textBoxStack.Count - 1);
            int flag = this.Load("Content/Levels/" + ((TextInputBox)box).field.inputString + ".lvl");
            if (flag == 2)
                textBoxStack.Add(new TextBox(null, "ERROR: Could not load file.", TextBoxEnter));
            else if (flag == 3)
                textBoxStack.Add(new TextBox(null, "WARNING: Needed to load in additional PNG files.", TextBoxEnter));
            else if (flag == 4)
                textBoxStack.Add(new TextBox(null, "WARNING: Missing textures on decals.", TextBoxEnter));
            else if (flag == -1)
                textBoxStack.Add(new TextBox(null, "ERROR: The file is corrupt.", TextBoxEnter));
        }

        private void SetSnapMouse()
        {
            // Allows for snapping to grid.
            if (!Input.IsKeyDown(Keys.Tab))
            {
                snapMouse = (Input.MousePosition * zoom + camera) / snapSize;
                if (mode == EditorMode.SquareMode || mode == EditorMode.GeneralMode)
                {
                    snapMouse.X = (float)Math.Floor(snapMouse.X) * snapSize;
                    snapMouse.Y = (float)Math.Floor(snapMouse.Y) * snapSize;
                }
                else
                {
                    snapMouse.X = (float)Math.Round(snapMouse.X) * snapSize;
                    snapMouse.Y = (float)Math.Round(snapMouse.Y) * snapSize;
                }
            }
            else
            {
                snapMouse = Input.MousePosition * zoom + camera;
            }

            if (Input.IsKeyDown(Keys.LeftShift) && mode == EditorMode.DecalMode)
            {
                decalOrigin = Vector2.Zero;
                Vector2 wantSnap = snapMouse;
                snapMouse = SnapToGrid(wantSnap + camera * (decalDrift - 1), 32);
            }
            else
                decalOrigin = this.camera;
        }

        public Vector2 SnapToGrid(Vector2 v, float grid)
        {
            Vector2 r;
            r.X = (float)Math.Round(v.X / grid) * grid;
            r.Y = (float)Math.Round(v.Y / grid) * grid;
            return r;
        }

        public Block? FindBlockAtPosition(Vector2 pos)
        {
            foreach (Block b in blocks)
                if (b.position == pos)
                    return b;
            return null;
        }

        public Floor? FindLineAtPosition(Vector2 pos)
        {
            foreach (Floor b in floors)
                if (b.pointL == pos || b.pointR == pos)
                    return b;
            return null;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!pauseGameplay)
                zoom = 1f;

            spriteBatch.End();

            Engine.Instance.GraphicsDevice.SetRenderTarget(editorSurface);
            Engine.Instance.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            // Draw world objects and decals.
            base.Draw(spriteBatch);

            // Draw collision markers.
            if (showCollision)
            {
                foreach (Floor f in floors)
                    f.Draw(spriteBatch, camera);
                foreach (Block b in blocks)
                    b.Draw(spriteBatch, camera);
                foreach (Tuple<String, Vector2> n in nodes.GetAllNodes())
                    spriteBatch.DrawString(TextureBin.mainFont, n.Item1, n.Item2 - camera, Color.OrangeRed);
            }

            if (pauseGameplay)
            {
                // Show locators and aligning lines.
                if (helpInfo)
                {
                    DrawHelper.DrawLine(spriteBatch, new Vector2(-camera.X, 0), new Vector2(-camera.X, Engine.WINDOW_HEIGHT * zoom), Color.Blue, 2);
                    DrawHelper.DrawLine(spriteBatch, new Vector2(0, -camera.Y), new Vector2(Engine.WINDOW_WIDTH * zoom, -camera.Y), Color.Green, 2);
                    DrawHelper.DrawRectangleOutline(spriteBatch, boundsTopLeft - camera, (boundsBottomRight - boundsTopLeft), Color.Purple);
                    DrawHelper.DrawRectangleOutline(spriteBatch, boundsTopLeft - camera - Vector2.One, (boundsBottomRight - boundsTopLeft) + Vector2.One, Color.Purple);
                }

                if (mode == EditorMode.FloorMode || (mode == EditorMode.DecalMode && Input.IsKeyUp(Keys.LeftControl)) || mode == EditorMode.NodeMode || mode == EditorMode.CameraMode)
                {
                    // Display X cursor.
                    spriteBatch.DrawString(TextureBin.mainFont, "x", snapMouse - camera + new Vector2(3, -3), Color.Red, 0f, TextureBin.mainFont.MeasureString("X") / 2, 4f, SpriteEffects.None, 0f);
                    if (point1 != null && mode == EditorMode.FloorMode)
                        DrawHelper.DrawLine(spriteBatch, (Vector2)point1 - camera, snapMouse - camera, Color.Red, 2);
                }
                else if (mode == EditorMode.SquareMode || mode == EditorMode.GeneralMode)
                {
                    // Display block cursor.
                    DrawHelper.DrawRectangleOutline(spriteBatch, snapMouse.X - camera.X, snapMouse.Y - camera.Y, blockSize.X, blockSize.Y, Color.Red);
                }

                // Show decal preview.
                if (mode == EditorMode.DecalMode)
                {
                    // Decal Delete Boxes.
                    if (Input.IsKeyDown(Keys.LeftControl))
                    {
                        List<Decal> all = new List<Decal>();
                        all.AddRange(decalsFront);
                        all.AddRange(decalsBack);
                        foreach (Decal d in all)
                        {
                            float alpha = 0.5f;
                            Vector2 decalOnScreen = d.position - (d.origin + (camera - d.origin) * d.drift);
                            if ((decalOnScreen - Input.MousePosition * zoom).Length() < 14)
                            {
                                spriteBatch.Draw(d.texture, decalOnScreen, null, Color.Red, d.angle, Vector2.Zero, d.scale, SpriteEffects.None, 0f);
                                alpha = 1f;
                            }
                            DrawHelper.DrawRectangleOutline(spriteBatch, decalOnScreen - Vector2.One * 8, Vector2.One * 16, Color.Red * alpha);
                        }
                    }

                    // Decal preview.
                    else if (decal != null)
                    {
                        spriteBatch.Draw(decal, snapMouse - (decalOrigin + (camera - decalOrigin) * decalDrift), null, decalColor * 0.5f, decalAngle, Vector2.Zero, decalScale, SpriteEffects.None, 0f);
                    }
                }
            }

            spriteBatch.End();

            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
            Engine.Instance.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(editorSurface, new Rectangle(0, 0, Engine.WINDOW_WIDTH, Engine.WINDOW_HEIGHT), new Rectangle(0, 0, (int)(Engine.WINDOW_WIDTH * zoom), (int)(Engine.WINDOW_HEIGHT * zoom)), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            // Draw HUD.
            if (pauseGameplay)
            {
                if (helpInfo)
                {
                    // Draw help text.
                    spriteBatch.DrawString(TextureBin.mainFont, ((int)mode).ToString() + ". " + mode.ToString() + "\n\n" + modeText[(int)mode], Vector2.One * 16, Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

                    // Draw info bar.
                    DrawHelper.DrawRectangle(spriteBatch, new Vector2(0, Engine.WINDOW_HEIGHT - DEBUG_WINDOW_HEIGHT), new Vector2(Engine.WINDOW_WIDTH, DEBUG_WINDOW_HEIGHT), Color.Black * 0.5f);
                    spriteBatch.DrawString(TextureBin.mainFont, "NAME: <" + this.name + "> - POS: " + snapMouse.ToString(), new Vector2(16, Engine.WINDOW_HEIGHT - DEBUG_WINDOW_HEIGHT), Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }

                // Draw specific details.
                if (mode == EditorMode.NodeMode)
                    spriteBatch.DrawString(TextureBin.mainFont, "Node Name: " + nodeString, new Vector2(Engine.WINDOW_WIDTH - 300, 16), Color.Red, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
                if (mode == EditorMode.DecalMode)
                    spriteBatch.DrawString(TextureBin.mainFont, "Decal Scale: " + decalScale.ToString() + "\nDecal Angle: " + decalAngle.ToString() + "\nDecal Drift: " + decalDrift.ToString() + "\nDecal Layer: " + decalLayer.ToString(), new Vector2(Engine.WINDOW_WIDTH - 300, 16), Color.Red, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);

                // Show textbox.
                if (textBoxStack.Count > 0)
                {
                    textBoxStack[textBoxStack.Count - 1].Draw(spriteBatch);
                }
            }
        }
    }
}
