using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BabbleEngine
{
    class SelectTextureBox : TextBox
    {
        int rows = 8;
        int columns = 8;
        int scroll = 0;

        String[] keys;
        int spot;
        public Texture2D texture = null;

        public SelectTextureBox(TextBoxEnterEvent enter)
            : base(null, "", enter, -1, Keys.Enter)
        {
            this.size = new Vector2(512, 512);
            Center();
            keys = TextureBin.GetDictionary.Keys.ToArray<String>();
        }

        public override void Update()
        {
            if (Input.MouseScrollUp && scroll > 0)
                scroll--;
            if (Input.MouseScrollDown)
                scroll++;
            if (Input.KeyboardTapped(Keys.PageDown))
                scroll += columns;
            if (Input.KeyboardTapped(Keys.PageUp))
            {
                scroll -= columns;
                if (scroll < 0)
                    scroll = 0;
            }

            int xx = (int)Math.Floor((Input.MousePosition.X - position.X) / (size.X / columns));
            int yy = (int)Math.Floor((Input.MousePosition.Y - position.Y) / (size.Y / rows));
            spot = (xx + yy * columns) + scroll;
            if (Input.MousePosition.X > this.position.X && Input.MousePosition.X < this.position.X + this.size.X &&
                Input.MousePosition.Y > this.position.Y && Input.MousePosition.Y < this.position.Y + this.size.Y &&
                spot < keys.Length)
            {
                texture = TextureBin.GetTexture(keys[spot]);
            }
            else
            {
                texture = null;
            }

            if (Input.MouseLeftButtonTapped)
                enterEvent(this);

            base.Update();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            int w = (int)(size.X / columns);
            int h = (int)(size.Y / rows);

            for (int i = scroll; i < keys.Length && i - scroll < rows * columns; i++)
            {
                int xx = (i - scroll) % columns;
                int yy = (i - scroll) / columns;
                spriteBatch.Draw(TextureBin.GetTexture(keys[i]), new Rectangle(xx * w + (int)this.position.X, yy * h + (int)this.position.Y, w, h), Color.White);
            }
            if (texture != null)
            {
                int xx = (spot - scroll) % columns;
                int yy = (spot - scroll) / columns;
                spriteBatch.DrawString(TextureBin.mainFont, keys[spot], new Vector2(xx * w, yy * h) + position, Color.Red);
                DrawHelper.DrawRectangleOutline(spriteBatch, xx * w + (int)this.position.X, yy * h + (int)this.position.Y, w, h, Color.Red);
            }
        }
    }
}
