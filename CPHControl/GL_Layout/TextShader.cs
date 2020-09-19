namespace CPHControl
{
    /// <summary>
    /// This Class stores the specific properties for Text Shading. 
    /// That means this is the Shader, that is used for Text.
    /// </summary>
    /// <seealso cref="CPHControl.Shader" />
    public class TextShader : Shader
    {
        #region Fields
        /// <summary>
        /// The Shader Location of the "in" variable that receives the vertices of screen coordinates.
        /// </summary>
        protected int _vertexLocation;
        /// <summary>
        /// The Shader Location of the "in" variable that receives the vertices of texture coordinates.
        /// </summary>
        protected int _texCoordLocation;
        /// <summary>
        /// The Shader uniform location that stores the transformation matrix 
        /// </summary>
        protected int _matrixLocation;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the vertex location of the screen coordinates to use in calls to OpenGL using the <see cref="TextShader"/>
        /// </summary>
        public int VertexLocation
        {
            get { return _vertexLocation; }
        }
        /// <summary>
        /// Gets the matrix location to use in calls to OpenGL using the <see cref="TextShader"/>
        /// </summary>
        public int MatrixLocation
        {
            get { return _matrixLocation; }
        }
        /// <summary>
        /// Gets the vertex location of the texture coordinates to use in calls to OpenGL using the <see cref="TextShader"/>
        /// </summary>
        public int TexCoordLocation
        {
            get { return _texCoordLocation; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextShader"/> class
        /// immediately asking the shader for the locations of the attributes and uniforms.
        /// it takes the paths to the vertex- and fragment-Shader for the text items as arguments.
        /// </summary>
        /// <param name="vertexPath">The vertex path.</param>
        /// <param name="fragmentPath">The fragment path.</param>
        public TextShader(byte[] vertex, byte[] fragment) : base(vertex, fragment)
        {
            _vertexLocation = GetAttribLocation("aPosition");
            _texCoordLocation = GetAttribLocation("aTexCoord");
            _matrixLocation = GetUniformLocation("transform");
        }
        #endregion
    }
}
