using CPHControl.Properties;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace CPHControl
{
    /// <summary>
    /// base Class, that sets up a Shader Program given two Files of Vertex and Fragment Shaders.
    /// </summary>
    abstract public class Shader
    {
        #region Fields
        /// <summary>
        /// The handle of the Shader Program, returned by OpenGL
        /// </summary>
        protected int _handle;
        /// <summary>
        /// indicates if the Shader Program has been disposed
        /// </summary>
        private bool disposedValue = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class.
        /// </summary>
        /// <param name="vertexPath">The vertex path.</param>
        /// <param name="fragmentPath">The fragment path.</param>
        public Shader(byte[] vertex, byte[] fragment)
        {//read the sourcefiles of the shaders and store them for processing
            string VertexShaderSource = Encoding.UTF8.GetString(vertex);
            string FragmentShaderSource = Encoding.UTF8.GetString(fragment);

            //create the shaders in OpenGL and compile them
            var VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);
            GL.CompileShader(FragmentShader);

            _handle = GL.CreateProgram();//create a shader program in OpenGL

            GL.AttachShader(_handle, VertexShader);//attach Shaders to the program.
            GL.AttachShader(_handle, FragmentShader);

            GL.LinkProgram(_handle);    //link and validate the Shader Program, then delete the Shaders
            GL.ValidateProgram(_handle);

            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Uses the Shader Program.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(_handle);
        }
        /// <summary>
        /// Stops the use of the Shader Program.
        /// </summary>
        public void StopUse()
        {
            GL.UseProgram(0);
        }
        /// <summary>
        /// Gets the attribute location of specified string
        /// </summary>
        /// <param name="attribName">Name of the attribute.</param>
        /// <returns>The position where the Attribute is stored in OpenGL.</returns>
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(_handle, attribName);
        }
        /// <summary>
        /// Gets the uniform location.
        /// </summary>
        /// <param name="uniformName">Name of the uniform.</param>
        /// <returns>The position where the Uniform is stored in OpenGL.</returns>
        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(_handle, uniformName);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(_handle);//delete the Shader Program

                disposedValue = true;
            }
        }
        #endregion
    }
}
