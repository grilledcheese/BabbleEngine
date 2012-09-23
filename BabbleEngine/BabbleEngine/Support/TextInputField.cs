using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BabbleEngine
{
    /// <summary>
    /// A class representing a text input field. 
    /// </summary>
    public class TextInputField
    {
        public Vector2 position;
        public Vector2 size;
        public bool enabled;
        public String inputString = "";

        private SpriteFont font;
        private short blink = 0;
        private short back = 0;
        private Keys enableKey;
        private String suffix;

        /// <summary>
        /// Constructs a text input field. Draw and update must be called to correctly implement it.
        /// By default, the field is deactivated.
        /// </summary>
        /// <param name="Font">The font to use for the field.</param>
        /// <param name="Position">The position for the filed.</param>
        /// <param name="EnableKey">The key that will activate/disactivate the field.</param>
        /// <param name="suffix"></param>
        public TextInputField(SpriteFont Font, Vector2 Position, Keys EnableKey, String suffix = "")
        {
            this.enabled = false;
            this.position = Position;
            this.font = Font;
            this.size = Font.MeasureString("THIS IS A LONG TEST STRING.");
            this.enableKey = EnableKey;
            this.suffix = suffix;
        }

        public void Update()
        {
            if (Input.KeyboardTapped(enableKey))
            {
                if (enabled == false)
                    inputString = "";
                enabled = !enabled;
            }

            blink++;
            if (blink > 10)
                blink = -10;

            if (enabled)
            {
                // This code was borrowed from Caleb.
                KeyboardState _ks = Input.GetKeyboardState();
                foreach (Keys key in _ks.GetPressedKeys())
                {
                    if (!Input.GetPreviousKeyboardState().IsKeyDown(key))
                    {
                        string _str = key.ToString();
                        if (_str.Length == 1)
                        {
                            if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                inputString += key.ToString();
                            else
                                inputString += key.ToString().ToLower();
                        }
                        else if (_str.Length == 2)
                        {
                            switch (_str)
                            {
                                case "D9":
                                    if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                        inputString += "(";
                                    else
                                        inputString += _str.Remove(0, 1);
                                    break;
                                case "D0":
                                    if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                        inputString += ")";
                                    else
                                        inputString += _str.Remove(0, 1);
                                    break;
                                default:
                                    inputString += _str.Remove(0, 1);
                                    break;
                            }
                        }
                        else if (_str == "Enter")
                        {
                            // Do nothing as of now.
                        }
                        else
                        {
                            switch (_str)
                            {
                                case "Space":
                                    inputString += " ";
                                    break;
                                case "Back":
                                    try { inputString = inputString.Remove(inputString.Length - 1); }
                                    catch { }
                                    break;
                                case "OemPeriod":
                                    inputString += ".";
                                    break;
                                case "OemQuestion":
                                    inputString += "/";
                                    break;
                                case "OemPipe":
                                    inputString += "\\";
                                    break;
                                case "OemSemicolon":
                                    inputString += ";";
                                    break;
                                case "OemPlus":
                                    inputString += "=";
                                    break;
                                case "OemMinus":
                                    if (_ks.IsKeyDown(Keys.LeftShift) || _ks.IsKeyDown(Keys.RightShift))
                                        inputString += "_";
                                    else
                                        inputString += "-";
                                    break;
                            }
                        }
                    }
                }

                if (_ks.IsKeyDown(Keys.Back))
                {
                    back++;
                    if (back == 30)
                        inputString = "";
                }
                else
                {
                    back = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D _blank = TextureBin.Pixel;

            spriteBatch.Draw(_blank,
                new Rectangle((int)(position.X), (int)(position.Y), (int)size.X * 2, (int)size.Y * 2),
                Color.Blue);

            String _str = inputString;
            if (enabled)
            {
                if (blink > 0 && suffix == "")
                    _str += "#";
                _str += suffix;
            }
            else
            {
                _str += " < Press " + enableKey.ToString();
            }

            spriteBatch.DrawString(font, _str, this.position, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
        }
    }
}
