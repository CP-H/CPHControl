namespace CPHControl
{
    /// <summary>
    /// This Class stores the specific properties for Line Shading. 
    /// That means this is the Shader, that is used for Lines.
    /// </summary>
    /// <seealso cref="CPHControl.Shader" />
    public class LinesShader : Shader
    {
        #region Fields
        /// <summary>
        /// The Shader Location of the "in" variable that receives the vertices.
        /// </summary>
        protected int _vertexLocation;
        /// <summary>
        /// The Shader uniform location that stores the color vector
        /// </summary>
        protected int _colorLocation;
        /// <summary>
        /// The Shader uniform location that stores the transformation matrix 
        /// </summary>
        protected int _matrixLocation;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the vertex location to use in calls to OpenGL using the <see cref="LinesShader"/>
        /// </summary>
        public int VertexLocation
        {
            get { return _vertexLocation; }
        }
        /// <summary>
        /// Gets the color location to use in calls to OpenGL using the <see cref="LinesShader"/>
        /// </summary>
        public int ColorLocation
        {
            get { return _colorLocation; }
        }
        /// <summary>
        /// Gets the matrix location to use in calls to OpenGL using the <see cref="LinesShader"/>
        /// </summary>
        public int MatrixLocation
        {
            get { return _matrixLocation; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LinesShader"/> class
        /// immediately asking the shader for the locations of the attributes and uniforms.
        /// it takes the paths to the vertex- and fragment-Shader for the line items as arguments.
        /// </summary>
        /// <param name="vertexPath">The vertex path.</param>
        /// <param name="fragmentPath">The fragment path.</param>
        public LinesShader(byte[] vertex, byte[] fragment) : base(vertex, fragment)
        {
            _vertexLocation = GetAttribLocation("aPosition");
            _colorLocation = GetUniformLocation("inColor");
            _matrixLocation = GetUniformLocation("transform");
        }
        #endregion
    }
}
