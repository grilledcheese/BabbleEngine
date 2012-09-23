using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BabbleEngine
{
    /// <summary>
    /// A very crappy implementation of colour chooser. I'M SORRY!
    /// </summary>
    public class SelectColorBox : TextBox
    {
        ColorDial r;
        ColorDial b;
        ColorDial g;
        ColorDial a;

        public Color color;

        const float SPACE = 128;
        const float BARS = 4;

        public SelectColorBox(TextBoxEnterEvent e, Color c)
            : base("", "", e, -1, Keys.Enter)
        {
            this.size = new Vector2(320, 256);
            Center();
            r = new ColorDial(this.position + new Vector2((SPACE / (BARS + 1)) * 1 + 0 * (this.size.X - SPACE) / BARS , 8), new Vector2((this.size.X - SPACE) / BARS, this.size.Y - 48), Color.Red, c.ToVector4().X);
            g = new ColorDial(this.position + new Vector2((SPACE / (BARS + 1)) * 2 + 1 * (this.size.X - SPACE) / BARS, 8), new Vector2((this.size.X - SPACE) / BARS, this.size.Y - 48), Color.Green, c.ToVector4().Y);
            b = new ColorDial(this.position + new Vector2((SPACE / (BARS + 1)) * 3 + 2 * (this.size.X - SPACE) / BARS, 8), new Vector2((this.size.X - SPACE) / BARS, this.size.Y - 48), Color.Blue, c.ToVector4().Z);
            a = new ColorDial(this.position + new Vector2((SPACE / (BARS + 1)) * 4 + 3 * (this.size.X - SPACE) / BARS, 8), new Vector2((this.size.X - SPACE) / BARS, this.size.Y - 48), Color.Gray, c.ToVector4().W);
        }

        public override void Update()
        {
            base.Update();

            r.Update();
            b.Update();
            g.Update();
            a.Update();
            color = new Color(r.amount, g.amount, b.amount, a.amount);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, color);
            r.Draw(spriteBatch);
            g.Draw(spriteBatch);
            b.Draw(spriteBatch);
            a.Draw(spriteBatch);
        }

        private struct ColorDial
        {
            Vector2 position;
            Vector2 size;
            public float amount;
            Color color;

            public ColorDial(Vector2 position, Vector2 size, Color color, float amount = 1)
            {
                this.position = position;
                this.size = size;
                this.amount = amount;
                this.color = color;
            }

            public void Update()
            {
                if (Input.MousePosition.X > this.position.X && Input.MousePosition.X < this.position.X + this.size.X &&
                    Input.MousePosition.Y > this.position.Y - 8 && Input.MousePosition.Y < this.position.Y + this.size.Y + 8 &&
                    Input.MouseLeftButtonDown)
                {
                    amount = ((this.position.Y + this.size.Y) - Input.MousePosition.Y) / this.size.Y;
                    amount = (float)Math.Round(amount * 10) / 10f;
                }
            }

            public void Draw(SpriteBatch sb)
            {
                DrawHelper.DrawRectangle(sb, position, size, color);
                DrawHelper.DrawRectangle(sb, position + Vector2.UnitY * (this.size.Y * (1-amount) - 2), new Vector2(size.X, 4), Color.Black);
            }
        }
    }
}
