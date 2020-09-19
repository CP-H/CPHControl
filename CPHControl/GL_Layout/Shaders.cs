namespace CPHControl
{
    /// <summary>
    /// Class that constructs and stores all shader objects.
    /// </summary>
    public class Shaders
    {
        #region Fields
        /// <summary>
        /// The line shader 
        /// </summary>
        protected LinesShader _lineShader;
        /// <summary>
        /// The text shader
        /// </summary>
        protected TextShader _textShader;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Shaders"/> class.
        /// Setting up all Shaders and storing them.
        /// </summary>
        /// <param name="lineShaderVertPath">The line shader vert path.</param>
        /// <param name="lineShaderFragPath">The line shader frag path.</param>
        /// <param name="TextShaderVertPath">The text shader vert path.</param>
        /// <param name="TextShaderFragPath">The text shader frag path.</param>
        public Shaders(byte[] lineShaderVert, byte[] lineShaderFrag, byte[] TextShaderVert, byte[] TextShaderFrag)
        {
            _lineShader = new LinesShader(lineShaderVert, lineShaderFrag);
            _textShader = new TextShader(TextShaderVert, TextShaderFrag);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the line shader.
        /// </summary>
        public LinesShader LineShader
        {
            get { return _lineShader; }
        }
        /// <summary>
        /// Gets the text shader.
        /// </summary>
        public TextShader TextShader
        {
            get { return _textShader; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _lineShader.Dispose();
            _textShader.Dispose();
        }
        #endregion
    }
}
