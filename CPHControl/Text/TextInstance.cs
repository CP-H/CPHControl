using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace CPHControl
{
    /// <summary>
    /// This contains the objects needed for drawing Text with OpenGL.
    /// First, this generates a bitmap of a font and VBOs to store positions
    /// on that bitmap to later use those positions to get single Glyphs
    /// and draw them onto the <see cref="global::CPHControl"/>.
    /// Because this is needed in many Classes, it also stores a reference
    /// to the <see cref="Shaders"/> Class.
    /// </summary>
    public class TextInstance
    {
        #region Fields
        /// <summary>
        /// The filename of the Bitmap font
        /// </summary>
        private string _fontBitmapFilename;

        /// <summary>
        /// A series of integers specifying glyph sizes and glyphs per line in the bitmap
        /// </summary>
        private int _glyphsPerLine,
                        _glyphLineCount,
                        _glyphWidth,
                        _glyphHeight;

        /// <summary>
        /// The space between characters in the bitmap
        /// </summary>
        private int _charXSpacing;

        /// <summary>
        /// The atlas offsets of the characters in the bitmap
        /// </summary>
        private int _atlasOffsetX, _atlasOffsetY;
        /// <summary>
        /// The font size
        /// </summary>
        private int _fontSize;
        /// <summary>
        /// Bool specifying if its a bitmap font
        /// </summary>
        private bool _bitmapFont;
        /// <summary>
        /// The font name
        /// </summary>
        private string _fontName;

        /// <summary>
        /// The font texture identifier storing the return value from OpenGL
        /// </summary>
        private int _fontTextureID;
        /// <summary>
        /// The texture width
        /// </summary>
        private int _textureWidth;
        /// <summary>
        /// The texture height
        /// </summary>
        private int _textureHeight;
        /// <summary>
        /// The character sheet storing all printable characters
        /// </summary>
        private CharSheet _charSheet;
        /// <summary>
        /// The integer storing the VAO reference from OpenGL
        /// </summary>
        private int _VAO;
        /// <summary>
        /// The shaders instance storing the reference to the Shaders Object
        /// </summary>
        private Shaders _shaders;
        /// <summary>
        /// An array storing multiple time values of how often the control refreshes for averaging
        /// </summary>
        private double[] _frameTimeSamples = new double[20];
        /// <summary>
        /// An array storing multiple time values of how controls code runs for averaging
        /// </summary>
        private double[] _runTimeSamples = new double[20];
        /// <summary>
        /// The number of samples taken for time averaging
        /// </summary>
        private int _sampleCount = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextInstance"/> class.
        /// </summary>
        /// <param name="shaders">The shaders.</param>
        public TextInstance(Shaders shaders)
        {
            _fontBitmapFilename = "Font_bitmap.png";
            _glyphsPerLine = 16;
            _glyphLineCount = 16;

            _atlasOffsetX = -3;
            _atlasOffsetY = -1;
            _fontSize = 24;
            _glyphWidth = (int)(0.7 * _fontSize);
            _glyphHeight = (int)(1.5 * _fontSize);
            _bitmapFont = false;
            _fontName = "Consolas";

            _charXSpacing = _glyphWidth - 1;

            _shaders = shaders;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInstance"/> class.
        /// </summary>
        /// <param name="FontName">Name of the font.</param>
        /// <param name="FontSize">Size of the font.</param>
        public TextInstance(string FontName, int FontSize) : base()
        {
            _fontSize = FontSize;
            _fontName = FontName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the glyph height in pixels
        /// </summary>
        /// <value>
        /// The glyph height in pixels.
        /// </value>
        public int GlyphHeightPix
        {
            get { return _glyphHeight; }
        }

        public CharSheet CharSheet
        {
            get { return _charSheet; }
        }

        public Shaders Shaders
        {
            get { return _shaders; }
        }
        /// <summary>
        /// Gets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize
        {
            get { return _fontSize; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates the font image, the VAO and the <see cref="CharSheet"/> storing all <see cref="Character"/>
        /// </summary>
        /// <param name="rect">The rect.</param>
        public void GenerateFontImage(GLRectangleF rect)
        {
            int bitmapWidth = _glyphsPerLine * _glyphWidth;     //calculate bitmap dimensions
            int bitmapHeight = _glyphLineCount * _glyphHeight;
            //generating the bitmap and setting it up first
            using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                Font font;
                font = new Font(new FontFamily(_fontName), _fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

                using (var g = Graphics.FromImage(bitmap))
                {
                    if (_bitmapFont)
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    }
                    else
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    }
                    //draw each character onto the bitmap
                    for (int p = 0; p < _glyphLineCount; p++)
                    {
                        for (int n = 0; n < _glyphsPerLine; n++)
                        {
                            char c = (char)(n + p * _glyphsPerLine);
                            g.DrawString(c.ToString(), font, Brushes.Black,
                                n * _glyphWidth + _atlasOffsetX, p * _glyphHeight + _atlasOffsetY);
                        }
                    }
                }
                bitmap.Save(_fontBitmapFilename);//save the bitmap
                _textureWidth = bitmap.Width; _textureHeight = bitmap.Height;
            }

            _shaders.TextShader.Use();
            //bind the bitmap to an OpenGL Texture
            _fontTextureID = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _fontTextureID);

            using (var image = new Bitmap(_fontBitmapFilename))
            {//load the bitmap into OpenGL
                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }
            //set up the filters for the bitmap
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            //generate the VAO and bind it
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);
            //enable the vertex attribute arrays, very important otherwise no vertix can be loaded into OpenGL
            GL.EnableVertexAttribArray(_shaders.TextShader.TexCoordLocation);
            GL.EnableVertexAttribArray(_shaders.TextShader.VertexLocation);
            //generate the CharSheet from the information about the bitmap
            _charSheet = new CharSheet(_glyphLineCount, _glyphsPerLine, _glyphHeight, _glyphWidth, _textureWidth, _textureHeight, rect);
            //set the attribpointers to tell the Shader how the Vertices will be passed.
            GL.VertexAttribPointer(_shaders.TextShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _fontTextureID);
            GL.VertexAttribPointer(_shaders.TextShader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            GL.BindVertexArray(0);
            _shaders.TextShader.StopUse();
            GL.Enable(EnableCap.Blend);//enable blending for the bitmap
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        /// <summary>
        /// Draws the tic values.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="pane">The pane.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="Value">The value.</param>
        public void DrawTicValues(float x, float y, GraphPane pane, Axis axis, string Value)
        {
            Matrix4 mat;

            GL.Viewport((int)pane.Rect.X, (int)pane.Rect.Y, (int)pane.Rect.Width, (int)pane.Rect.Height);

            _shaders.TextShader.Use();//start using the Shader and bind the VAO
            GL.BindVertexArray(_VAO);

            if (axis is XAxis)//Offsets are different depending on the axis
                x -= ((Value.Length - 1) * _charXSpacing) / (2 * pane.Rect.Width);
            else
            {
                x -= (Value.Length * _charXSpacing) / (pane.Rect.Width);
                y += _glyphHeight / (3 * pane.Rect.Height);
            }
            //iterate through each char of the to-print-string, bind it and draw it
            for (int n = 0; n < Value.Length; n++)
            {
                int idx = _charSheet.IndexOf(Value[n]);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _charSheet[idx].VBO);
                GL.VertexAttribPointer(_shaders.TextShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
                GL.VertexAttribPointer(_shaders.TextShader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

                mat = Matrix4.Identity * Matrix4.CreateTranslation(x, y, 0.0f);
                GL.UniformMatrix4(_shaders.TextShader.MatrixLocation, false, ref mat);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                x += _charXSpacing / pane.Rect.Width;
            }

            GL.BindVertexArray(0);
            _shaders.TextShader.StopUse();
        }

        /// <summary>
        /// Draws the axis title.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="title">The title.</param>
        public void DrawAxisTitle(GraphPane pane, Chart chart, Axis axis, string title)
        {
            float x = 0, y = 0;
            Matrix4 trans;
            Matrix4 rot;
            Matrix4 mat;

            GL.Viewport((int)pane.Rect.X, (int)pane.Rect.Y, (int)pane.Rect.Width, (int)pane.Rect.Height);

            _shaders.TextShader.Use();//start using the Shader and bind the VAO
            GL.BindVertexArray(_VAO);

            if (axis is XAxis)//for pXAxis place the title in the middle and dont rotate
            {
                x = -((title.Length - 1) * _charXSpacing - 2 * _atlasOffsetX) / (2 * pane.Rect.Width);
                x += 2 * (chart.Rect.X - pane.Rect.X + chart.Rect.Width / 2) / pane.Rect.Width - 1f;

                y = 2 * (chart.Rect.Y - 2.0f * axis.MajorTic.Size - _glyphHeight / 2) / pane.Rect.Height - 1;
                rot = Matrix4.Identity;
            }
            else        //put the yAxis in the middle and rotate the title by 90 degrees
            {
                x = -1 + 2 * (chart.Rect.X - axis.FixedSpace * ((axis as YAxis).Index + 1) + axis.MajorTic.Size) / pane.Rect.Width;

                y = -((title.Length - 1) * _charXSpacing - 2 * _atlasOffsetX) / (2 * pane.Rect.Height);
                y += 2 * (chart.Rect.Y - pane.Rect.Y + chart.Rect.Height / 2) / pane.Rect.Height - 1f;
                rot = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90.0f)) * Matrix4.CreateScale((float)_glyphWidth / (float)_fontSize, (float)_glyphHeight / (float)_fontSize, 1);
            }

            for (int n = 0; n < title.Length; n++)//draw each Glyph of the title
            {
                int idx = _charSheet.IndexOf(title[n]);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _charSheet[idx].VBO);
                GL.VertexAttribPointer(_shaders.TextShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
                GL.VertexAttribPointer(_shaders.TextShader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

                trans = Matrix4.CreateTranslation(x, y, 0.0f);
                mat = rot * trans;
                GL.UniformMatrix4(_shaders.TextShader.MatrixLocation, false, ref mat);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                if (axis is XAxis)
                    x += _charXSpacing / pane.Rect.Width;
                else
                    y += _charXSpacing / pane.Rect.Height;
            }

            GL.BindVertexArray(0);
            _shaders.TextShader.StopUse();
        }
        /// <summary>
        /// Draws the legend text.
        /// </summary>
        /// <param name="Items">The items.</param>
        /// <param name="ColorLineLen">Length of the color line.</param>
        /// <param name="TextLen">Length of the text.</param>
        /// <param name="pane">The pane.</param>
        public void DrawLegendText(List<LegendEntry> Items, float ColorLineLen, float TextLen, GraphPane pane)
        {
            Matrix4 trans;

            GL.Viewport((int)pane.Rect.X, (int)pane.Rect.Y, (int)pane.Rect.Width, (int)pane.Rect.Height);

            _shaders.TextShader.Use(); //start using the shader and bind the VAO
            GL.BindVertexArray(_VAO);

            foreach (LegendEntry Item in Items)//draw each legend item on its position
            {
                float x = Item.XPos + ColorLineLen, y = Item.YPos, templen = 0.0f;
                for (int n = 0; n < Item.Label.Length; n++)//draw each letter of each item
                {
                    int idx = _charSheet.IndexOf(Item.Label[n]);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, _charSheet[idx].VBO);
                    GL.VertexAttribPointer(_shaders.TextShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
                    GL.VertexAttribPointer(_shaders.TextShader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

                    trans = Matrix4.CreateTranslation(x, y, 0.0f);//create the translation matrix
                    GL.UniformMatrix4(_shaders.TextShader.MatrixLocation, false, ref trans);//put the translation matrix into the shader

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                    x += _charXSpacing / pane.Rect.Width;
                    templen += _charXSpacing / pane.Rect.Width;

                    if (templen > TextLen)
                        break;
                }
            }

            GL.BindVertexArray(0);
            _shaders.TextShader.StopUse();
        }

        /// <summary>
        /// Draws the times.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="FrameTime">The frame time.</param>
        /// <param name="RunTime">The run time.</param>
        public void DrawTimes(GraphPane pane, double FrameTime, double RunTime)
        {
            float x = 0, y = 0;
            Matrix4 trans;
            Matrix4 rot;
            Matrix4 mat;
            double avgFrameTime = 0;
            double avgRunTime = 0;
            _frameTimeSamples[_sampleCount] = FrameTime;
            _runTimeSamples[_sampleCount] = RunTime;
            _sampleCount += 1;
            if (_sampleCount == _runTimeSamples.Length)
                _sampleCount = 0;

            foreach (double d in _runTimeSamples)
            {
                avgRunTime += d;
            }
            foreach (double d in _frameTimeSamples)
            {
                avgFrameTime += d;
            }
            avgFrameTime /= _frameTimeSamples.Length;
            avgRunTime /= _runTimeSamples.Length;

            string FPS = string.Concat(string.Format("{0:0.}", 1 / avgFrameTime), " FPS");
            string RT = string.Concat(string.Format("{0:0.0}", avgRunTime), " ms Runtime");

            GL.Viewport((int)pane.Rect.X, (int)pane.Rect.Y, (int)pane.Rect.Width, (int)pane.Rect.Height);

            _shaders.TextShader.Use();
            GL.BindVertexArray(_VAO);

            x = -((FPS.Length - 1) * _charXSpacing - 10 * _atlasOffsetX - pane.Rect.Width) / pane.Rect.Width;

            y = (2 * _glyphHeight + 2 * pane.PChart.Rect.Y) / pane.Rect.Height - 1;
            rot = Matrix4.Identity;

            for (int n = 0; n < FPS.Length; n++)
            {
                int idx = _charSheet.IndexOf(FPS[n]);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _charSheet[idx].VBO);
                GL.VertexAttribPointer(_shaders.TextShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
                GL.VertexAttribPointer(_shaders.TextShader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

                trans = Matrix4.CreateTranslation(x, y, 0.0f);
                mat = rot * trans;
                GL.UniformMatrix4(_shaders.TextShader.MatrixLocation, false, ref mat);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                x += _charXSpacing / pane.Rect.Width;
            }

            x = -((RT.Length - 1) * _charXSpacing - 10 * _atlasOffsetX - pane.Rect.Width) / pane.Rect.Width;

            y = (3 * _glyphHeight + 2 * pane.PChart.Rect.Y) / pane.Rect.Height - 1;
            rot = Matrix4.Identity;

            for (int n = 0; n < RT.Length; n++)
            {
                int idx = _charSheet.IndexOf(RT[n]);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _charSheet[idx].VBO);
                GL.VertexAttribPointer(_shaders.TextShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
                GL.VertexAttribPointer(_shaders.TextShader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

                trans = Matrix4.CreateTranslation(x, y, 0.0f);
                mat = rot * trans;
                GL.UniformMatrix4(_shaders.TextShader.MatrixLocation, false, ref mat);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                x += _charXSpacing / pane.Rect.Width;
            }

            GL.BindVertexArray(0);
            _shaders.TextShader.StopUse();
        }
        #endregion
    }
}

