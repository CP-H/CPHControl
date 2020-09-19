using OpenTK.Graphics.OpenGL4;
using System;

namespace CPHControl
{
    /// <summary>
    /// Class storing information for OpenGL to draw one specific Glyph
    /// </summary>
    public class Character
    {
        #region Fields
        /// <summary>
        /// The Vertices containing Texture and Screen Coordinates
        /// </summary>
        private float[] _vertices;

        /// <summary>
        /// The actual char
        /// </summary>
        protected char _char;

        /// <summary>
        /// The VBO reference given by OpenGL
        /// </summary>
        protected int _VBO;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the VBO
        /// </summary>
        /// <value>
        /// The VBO
        /// </value>
        public int VBO
        {
            get { return _VBO; }
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <value>
        /// The character.
        /// </value>
        public char character
        {
            get { return _char; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// It combines the Coordinates of the bitmap containing the font 
        /// with the Coordinates of the screen
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="GlyphHeight">Height of the glyph.</param>
        /// <param name="GlyphWidth">Width of the glyph.</param>
        /// <param name="TextureWidth">Width of the texture.</param>
        /// <param name="TextureHeight">Height of the texture.</param>
        /// <param name="GlyphsPerLine">The glyphs per line.</param>
        /// <param name="rect">The rect.</param>
        public Character(char character, int GlyphHeight, int GlyphWidth, int TextureWidth, int TextureHeight, int GlyphsPerLine, GLRectangleF rect)
        {
            _char = character;//store the char for referencing
            float u_step = (float)GlyphWidth / (float)TextureWidth;//step sizes for stepping throught the bitmap
            float v_step = (float)GlyphHeight / (float)TextureHeight;
            float u = (float)(character % GlyphsPerLine) * u_step;//Starting positions of the character inside the bitmap
            float v = (float)(character / GlyphsPerLine) * v_step;
            float x = -GlyphWidth / 2, y = 0;//Screen coordinates, specifying the bottom mit point of the glyph

            _vertices = new float[]{//setting the vertices
                 x/rect.Width, y/rect.Height,    u,          v,
                -x/rect.Width, y/rect.Height,    u + u_step, v,
                -x/rect.Width, -GlyphHeight/rect.Height,               u + u_step, v + v_step,

                 x/rect.Width, y/rect.Height,    u,          v,
                -x/rect.Width, -GlyphHeight/rect.Height,               u + u_step, v + v_step,
                 x/rect.Width, -GlyphHeight/rect.Height,               u,          v + v_step
            };

            _VBO = GL.GenBuffer();//generate the VBO, binding it and putting the Vertices inside.
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * _vertices.Length), _vertices, BufferUsageHint.DynamicDraw);
        }
        #endregion
    }
}
