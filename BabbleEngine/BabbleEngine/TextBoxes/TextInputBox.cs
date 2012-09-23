using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BabbleEngine
{
    /// <summary>
    /// A textbox with an input field.
    /// </summary>
    public class TextInputBox : TextBox
    {
        private TextInputField field;
        public String InputText { get { return field.inputString; } set { field.inputString = value; } }

        public TextInputBox(String name, String text, TextBoxEnterEvent enterEvent)
            : base(name, text, enterEvent)
        {
            field = new TextInputField(TextureBin.mainFont, this.position - Vector2.UnitY * 48, Keys.F12);
            field.enabled = true;
        }

        public TextInputBox(String name, String text, TextBoxEnterEvent enterEvent, String defaultText, String suffix = "")
            : base(name, text, enterEvent)
        {
            field = new TextInputField(TextureBin.mainFont, this.position - Vector2.UnitY * 48, Keys.F12, suffix);
            field.enabled = true;
            field.inputString = defaultText;
            
        }

        public override void Update()
        {
            field.Update();
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            field.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
