using System.Collections.Generic;
using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// List of <see cref="Character"/> Objects
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{global::CPHControl.Character}" />
    public class CharSheet : List<Character>
    {
        public CharSheet(int GlyphLineCount, int GlyphsPerLine, int GlyphHeight, int GlyphWidth, int TextureWidth, int TextureHeight, GLRectangleF rect)
        {
            for (int p = 0; p < GlyphLineCount; p++)
            {
                for (int n = 0; n < GlyphsPerLine; n++)
                {
                    Character character = new Character((char)(n + p * GlyphsPerLine), GlyphHeight, GlyphWidth, TextureWidth, TextureHeight, GlyphsPerLine, rect);
                    this.Add(character);
                }
            }
        }
        public int IndexOf(char character)
        {
            int index = 0;
            foreach (Character c in this)
            {
                if (c.character.Equals(character))
                    return index;
                index++;
            }

            return -1;
        }
        public int VBOOf(char character)
        {
            foreach (Character c in this)
            {
                if (c.character.Equals(character))
                    return c.VBO;
            }
            return -1;
        }
    }
}
