using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// The item representing a collection of Datapoints 
    /// which is drawn as a line onto the pChart area.
    /// </summary>
    public class CurveItem
    {
        /// <summary>The circular buffer containing the points</summary>
        protected internal RollingVec2List _rollingV2List;
        /// <summary>
        /// Array to store the sorted copy of the circular buffer
        /// needed for OpenGL interaction
        /// </summary>
        protected internal Vector2[] _GraphVertex;
        /// <summary>
        /// The VBO storing the OpenGL reference for placing the vertices
        /// </summary>
        protected internal int _VBO;
        /// <summary>
        /// The VAO storing all OpenGL states
        /// </summary>
        protected internal int _VAO;
        /// <summary>
        /// The yAxis index
        /// </summary>
        internal int _yAxisIndex;
        /// <summary>
        /// The label to be drawn inside the legend
        /// </summary>
        internal string _label;
        /// <summary>
        /// The color individualizing the line on the pChart
        /// </summary>
        internal Color _color;
        /// <summary>
        /// The reference to the <see cref="GraphPane"/>
        /// </summary>
        protected GraphPane _pane;
        /// <summary>
        /// The reference to the <see cref="LinesShader"/> of the <see cref="global::CPHControl"/>
        /// </summary>
        protected internal LinesShader _linesShader;
        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class.
        /// Sets up the OpenGL Objects VAO and VBO and AttribPointers
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="label">The label.</param>
        /// <param name="points">The points.</param>
        /// <param name="color">The color.</param>
        /// <param name="linesShader">The lines shader.</param>
        public CurveItem(GraphPane pane, string label, RollingVec2List points, Color color, LinesShader linesShader)
        {
            _rollingV2List = points;
            _GraphVertex = new Vector2[points.Capacity];
            _label = label;
            _color = color;
            _yAxisIndex = 0;
            _linesShader = linesShader;

            _pane = pane;

            _linesShader.Use();//use the Shader, generate and bindd VAO and VBO
            _VAO = GL.GenVertexArray();
            _VBO = GL.GenBuffer();

            GL.BindVertexArray(_VAO);
            GL.EnableVertexAttribArray(_linesShader.VertexLocation);//enable the in variable of the Shader, where the Vertices are put

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);//bind the VBO and give it an empty array
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * _GraphVertex.Length), _GraphVertex, BufferUsageHint.StreamDraw);
            //set up the Pointer for OpenGL to interpret Vertices
            GL.VertexAttribPointer(_linesShader.VertexLocation, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            
            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }

        /// <summary>
        /// Gets or sets the index of the yAxis.
        /// </summary>
        /// <value>
        /// The index of the y axis.
        /// </value>
        public int YAxisIndex
        {
            get { return _yAxisIndex; }
            set 
            { 
                _yAxisIndex = value;
            }
        }

        /// <summary>
        /// number of points inside this <see cref="CurveItem"/>
        /// </summary>
        /// <value>
        /// The number of points inside this item
        /// </value>
        public int NPts
        {
            get
            {
                if (_rollingV2List == null)
                    return 0;
                else
                    return _rollingV2List.Count;
            }
        }

        /// <summary>
        /// Draws the Item into the <see cref="GraphPane"/><see cref="Chart"/><see cref="GLRectangleF"/>
        /// </summary>
        /// <param name="pane">The <see cref="GraphPane"/> owning the <see cref="Chart"/> area</param>
        public void Draw(GraphPane pane)
        {//set the OpenGL focus onto the Chart rectangle
            GL.Viewport((int)pane.PChart.Rect.X, (int)pane.PChart.Rect.Y, (int)pane.PChart.Rect.Width, (int)pane.PChart.Rect.Height);
            YAxis tempYAxis = GetYAxis(pane);//fetching the yAxis and its transformation matrix
            Matrix4 TraFo = tempYAxis.TraFo;

            _linesShader.Use();//using the Shader and binding the VAO
            GL.BindVertexArray(_VAO);

            GL.Uniform4(_linesShader.ColorLocation, _color);//Set the Color and the Matrix inside the Shader
            GL.UniformMatrix4(_linesShader.MatrixLocation, false, ref TraFo);

            //Only put points into the VBO if there are new ones
            if (_rollingV2List.HasUndrawnPoint)
            {
                _GraphVertex = _rollingV2List.TailToHeadCopy(); //copy the circular buffer into the sorted array
                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);  //bind the VBO and give the sorted array to it
                GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * _GraphVertex.Length), _GraphVertex, BufferUsageHint.StreamDraw);
                //set the VertexAttribPointer so OpenGL refreshes its buffer
                GL.VertexAttribPointer(_linesShader.VertexLocation, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

                _rollingV2List.HasUndrawnPoint = false;
            }
            //draw all the Points in the VBO
            GL.DrawArrays(PrimitiveType.LineStrip, 0, _rollingV2List.Count - 1);
            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }

        /// <summary>
        /// Gets the yAxis which this <see cref="CurveItem"/> is attached to.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns></returns>
        public YAxis GetYAxis(GraphPane pane)
        {
            if (_yAxisIndex < pane.PYAxisList.Count)
                return pane.PYAxisList[_yAxisIndex];
            else
                return pane.PYAxisList[0];
        }
    }
}
