using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Class that holds the properties for the border.
    /// </summary>
    public class GL_Border
    {
        #region Fields
        /// <summary>
        /// The VBO that stores the vertices
        /// </summary>
        internal int _VBO;

        /// <summary>
        /// The VAO that stores OpenGL states
        /// </summary>
        internal int _VAO;

        /// <summary>
        /// The reference to the Shaders
        /// </summary>
        internal Shaders _shaders;

        /// <summary>
        /// The border
        /// </summary>
        internal float[] _border;

        /// <summary>
        /// The color of the border
        /// </summary>
        internal Color _color;

        /// <summary>
        /// The transformation matrix, a unit matrix in this case
        /// </summary>
        internal Matrix4 _mat;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GL_Border"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="shaders">The shaders.</param>
        public GL_Border(Color color, Shaders shaders)
        {
            float db = 0.9995000f;
            _border = new float[8]
            {// border not at 1/-1 because it gets clipped by OpenGL otherwise
                 1.0f * db,  1.0f * db,
                 1.0f * db, -1.0f * db,
                -1.0f * db, -1.0f * db,
                -1.0f * db,  1.0f * db
            };

            _VBO = GL.GenBuffer();//Set up Shaders, VBO and VAO
            _VAO = GL.GenVertexArray();
            _shaders = shaders;
            _shaders.LineShader.Use();

            GL.BindVertexArray(_VAO);

            GL.EnableVertexAttribArray(_shaders.LineShader.VertexLocation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);// bind the VBO and put in the Vertices.
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * _border.Length), _border, BufferUsageHint.StaticDraw);
            _mat = Matrix4.Identity;//set the transformation to "no transformation"
            GL.UniformMatrix4(_shaders.LineShader.MatrixLocation,false,ref _mat);
            //set the Attribpointer for Vertex interpretation
            GL.VertexAttribPointer(_shaders.LineShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.BindVertexArray(0);
            _shaders.LineShader.StopUse();
            _color = color;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws the Border to the specified rectangle
        /// </summary>
        /// <param name="rect">The rect.</param>
        public void Draw(GLRectangleF rect)
        {
            GL.Viewport((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            _shaders.LineShader.Use();
            GL.BindVertexArray(_VAO);
            GL.Uniform4(_shaders.LineShader.ColorLocation, _color);
            GL.UniformMatrix4(_shaders.LineShader.MatrixLocation, false, ref _mat);

            GL.DrawArrays(PrimitiveType.LineLoop, 0, _border.Length / 2);
            GL.BindVertexArray(0);
            _shaders.LineShader.StopUse();
        }
        #endregion
    }
}
