using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace BabbleEngine
{
    public delegate void TextBoxEnterEvent(TextBox returnedBox);

    public class TextBox
    {
        public String name;
        public String text;
        public Vector2 position;
        public Vector2 size;

        SpriteFont font = TextureBin.mainFont;
        protected TextBoxEnterEvent enterEvent;
        int timer = -1;

        int blink = 30;
        Keys closeKey = Keys.Enter;

        public TextBox(String name, String text, TextBoxEnterEvent enterEvent, int timer = -1, Keys closeKey = Keys.Enter)
        {
            this.name = name;
            this.text = text;
            this.enterEvent = enterEvent;
            this.closeKey = closeKey;

            this.timer = timer;
            Scale();
            Center();
        }

        private void Scale()
        {
            Vector2 stringSize = font.MeasureString(text);
            this.size = stringSize * 2 + Vector2.One * 32;
        }

        protected void Center()
        {
            this.position = Engine.RESOLUTION / 2 - size / 2;
            this.position.X = (float)Math.Floor(this.position.X);
            this.position.Y = (float)Math.Floor(this.position.Y);
        }


        public virtual void Update()
        {
            blink--;
            if (blink < -30)
                blink = 30;

            if (timer >= 0)
            {
                timer--;
                if (timer == 0)
                    enterEvent(this);
            }
            else if (timer == -1 && Input.KeyboardTapped(closeKey))
                enterEvent(this);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawHelper.DrawRectangle(spriteBatch, this.position, this.size, Color.Blue);
            spriteBatch.DrawString(font, text, position + Vector2.One * 16, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            if (blink > 0)
            {
                Vector2 enterSize = font.MeasureString(closeKey.ToString());
                DrawHelper.DrawRectangle(spriteBatch, position + size - enterSize * 2, enterSize, Color.Blue);
                spriteBatch.DrawString(font, closeKey.ToString(), position + size - enterSize * 2, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color color)
        {
            DrawHelper.DrawRectangle(spriteBatch, this.position, this.size, color * (color.A / 255f));
            spriteBatch.DrawString(font, text, position + Vector2.One * 16, Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            if (blink > 0)
            {
                Vector2 enterSize = font.MeasureString(closeKey.ToString());
                DrawHelper.DrawRectangle(spriteBatch, position + size - enterSize * 2, enterSize, color * (color.A / 255f));
                spriteBatch.DrawString(font, closeKey.ToString(), position + size - enterSize * 2, Color.Black , 0, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            }
        }
    }
}
