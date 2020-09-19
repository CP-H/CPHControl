using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CPHControl
{
    /// <summary>
    /// Class that holds all legend properties, meaning the text, 
    /// colors and border to then draw the whole legend
    /// </summary>
    public class Legend
    {
        #region Fields
        /// <summary>
        /// The legend entries as a list
        /// </summary>
        List<LegendEntry> _legendEntries;
        TextInstance _textInstance;

        /// <summary>
        /// The space each entry gets in x direction
        /// </summary>
        protected int _entrySpaceX;

        /// <summary>
        /// The length of the colored line in front of each label
        /// </summary>
        protected int _colorLineLen;

        /// <summary>
        /// The text length of each label
        /// </summary>
        protected int _textLen;

        /// <summary>
        /// The gap between entries in x direction
        /// </summary>
        protected int _entryGapX;

        /// <summary>
        /// The number of entries in vertical direction
        /// </summary>
        protected int _nEntriesVert;

        /// <summary>
        /// The line height of the colored lines
        /// </summary>
        protected double _lineHeight;

        /// <summary>
        /// The rectangle on the <see cref="CPHControl"/> where this <see cref="Legend"/> is drawn
        /// </summary>
        protected GLRectangleF _rect;

        /// <summary>
        /// The inner rect, with spaces to the corner, where the 
        /// actual text and colored lines go
        /// </summary>
        protected GLRectangleF _innerRect;

        protected Margin _margin;
        protected LinesShader _linesShader;

        /// <summary>
        /// The VBO for the colored lines
        /// </summary>
        protected int _VBOLine;

        /// <summary>
        /// The VAO receiving the VBO for the colored lines
        /// </summary>
        protected int _VAOLine;

        /// <summary>
        /// The transformation matrix
        /// </summary>
        protected Matrix4 _mat;

        protected GL_Border _border;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of legend entries.
        /// </summary>
        public List<LegendEntry> LegendEntries
        {
            get { return _legendEntries; }
        }

        /// <summary>
        /// Gets or sets the rect.
        /// </summary>
        public GLRectangleF Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
        /// <summary>
        /// Gets or sets the <see cref="LegendEntry"/> with the specified label.
        /// </summary>
        /// <value>
        /// The <see cref="LegendEntry"/>.
        /// </value>
        /// <param name="label">The label.</param>
        /// <returns>A legend entry if there is more than one</returns>
        public LegendEntry this[string label]
        {
            get
            {
                int index = IndexOf(label);
                if (index >= 0)
                    return (_legendEntries[index]);
                else
                    return null;
            }
            set
            {
                int index = IndexOf(label);
                if (index >= 0)
                    _legendEntries[index] = value;
            }
        }
        /// <summary>
        /// Gets the <see cref="LegendEntry"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="LegendEntry"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="LegendEntry"/></returns>
        public LegendEntry this[int index]
        {
            get
            { return _legendEntries[index]; }
        }
        /// <summary>
        /// Gets the <see cref="LegendEntry"/> of the specified name.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>the <see cref="LegendEntry"/></returns>
        public int IndexOf(string label)
        {
            int index = 0;
            foreach (LegendEntry p in _legendEntries)
            {
                if (String.Compare(p.Label, label, true) == 0)
                    return index;
                index++;
            }

            return -1;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Legend"/> class.
        /// </summary>
        /// <param name="textInstance">The text instance.</param>
        public Legend(TextInstance textInstance)
        {
            _legendEntries = new List<LegendEntry>();
            _border = new GL_Border(Color.Black, textInstance.Shaders);
            _textInstance = textInstance;
            _linesShader = textInstance.Shaders.LineShader;
            _rect = new GLRectangleF(0, 0, 0, 0);
            _innerRect = new GLRectangleF(0, 0, 0, 0);
            _entryGapX = 2;
            _nEntriesVert = 5;
            _margin = new Margin();
            _margin.Left = 10;
            _margin.Right = 10;
            _margin.Top = 2;
            _margin.Bottom = 5;
            _lineHeight = textInstance.FontSize * 0.8;

            _VBOLine = GL.GenBuffer();//set up OpenGL by generating buffers and using the shader
            _VAOLine = GL.GenVertexArray();
            _linesShader.Use();

            GL.BindVertexArray(_VAOLine);//setting up VAO states and binding the VBO

            GL.EnableVertexAttribArray(_linesShader.VertexLocation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBOLine);
            _mat = Matrix4.Identity;
            GL.UniformMatrix4(_linesShader.MatrixLocation, false, ref _mat);

            GL.VertexAttribPointer(_linesShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a <see cref="LegendEntry"/> with given label and color.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="color">The color.</param>
        public void AddItem(string label, Color color)
        {
            _legendEntries.Add(new LegendEntry() { Label = label, Color = color });
        }
        /// <summary>
        /// Draws the whole legend
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void Draw(GraphPane pane)
        {
            int column = 0, row = 0;
            if (_legendEntries.Count != 0)
            {
                if (_legendEntries[0].PosSet == false)
                {
                    foreach (LegendEntry Item in _legendEntries)
                    {//calculate each entries position
                        Item.XPos = -1 + 2 * (_innerRect.X + column * (_entrySpaceX + _entryGapX)) / pane.Rect.Width;
                        Item.YPos = -1 + 2 * (_innerRect.Y + _innerRect.Height - (float)(row * _lineHeight)) / pane.Rect.Height;
                        Item.PosSet = true;
                        //generate the vertices for the colored lines
                        Item.line = new float[4] {
                            Item.XPos,                                          Item.YPos - (float)_lineHeight / (2 * pane.Rect.Height),
                            Item.XPos + _colorLineLen / pane.Rect.Width,    Item.YPos - (float)_lineHeight / (2 * pane.Rect.Height)
                        };
                        //step through rows and columns of the legend
                        column++;
                        if (column >= _nEntriesVert)
                        {
                            column = 0;
                            row++;
                        }
                    }
                }
            }

            _border.Draw(_rect);//draw the border, then the text, then the lines.
            _textInstance.DrawLegendText(_legendEntries, 2 * (float)_colorLineLen / pane.Rect.Width, 2 * _textLen / pane.Rect.Width, pane);
            DrawLegendLines(pane);
        }
        /// <summary>
        /// Draws the legend lines.
        /// </summary>
        /// <param name="pane">The <see cref="GraphPane"/>.</param>
        public void DrawLegendLines(GraphPane pane)
        {
            GL.Viewport((int)pane.Rect.X, (int)pane.Rect.Y, (int)pane.Rect.Width, (int)pane.Rect.Height);
            _linesShader.Use();
            GL.BindVertexArray(_VAOLine);//use Shader and bind VAO
            //put the transformation matrix into the Shader and bind the VBO
            GL.UniformMatrix4(_linesShader.MatrixLocation, false, ref _mat);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBOLine);

            GL.LineWidth(3.0f);//thicker line for better visibility
            foreach (LegendEntry Item in _legendEntries)
            {
                GL.Uniform4(_linesShader.ColorLocation, Item.Color);//set the items color, then put in the vertices
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * Item.line.Length), Item.line, BufferUsageHint.StaticDraw);
                //draw the lines
                GL.DrawArrays(PrimitiveType.Lines, 0, Item.line.Length / 2);
            }
            GL.LineWidth(1.0f);//reset line width

            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }
        /// <summary>
        /// Calculates the space for the legend
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="rect">The rect.</param>
        public void CalcRect(PaneBase pane, ref GLRectangleF rect)
        {
            if (pane is GraphPane)
            {
                int numLines = _legendEntries.Count / _nEntriesVert;

                if (numLines * _nEntriesVert != _legendEntries.Count)
                    numLines += 1;

                int LegendHeight = (int)(_lineHeight * numLines);

                _rect.Y = pane.Rect.Y + _margin.Bottom;
                _rect.X = pane.Rect.X + _margin.Left;

                _rect.Height = LegendHeight;
                _rect.Width = pane.Rect.Width - _margin.Left - _margin.Right;

                _innerRect.Height = _rect.Height * 0.95f;
                _innerRect.Width = _rect.Width * 0.98f;

                _innerRect.X = _rect.X + (_rect.Width - _innerRect.Width) / 2;
                _innerRect.Y = _rect.Y + (_rect.Height - _innerRect.Height) / 2;

                rect.Y += LegendHeight + _margin.Top + _margin.Bottom;
                rect.Height -= LegendHeight + _margin.Top + _margin.Bottom;

                _entrySpaceX = (int)((_innerRect.Width - (_nEntriesVert - 1) * _entryGapX) / _nEntriesVert);

                _colorLineLen = (int)(0.18f * _entrySpaceX);

                _textLen = (int)(_entrySpaceX * 0.8f);
            }
        }
        #endregion
    }

    /// <summary>
    /// Class that stores the properties of each legend entry.
    /// </summary>
    public class LegendEntry
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the line vertices
        /// </summary>
        public float[] line { get; set; }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        public float XPos { get; set; }
        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        public float YPos { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [position set].
        /// </summary>
        public bool PosSet { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="LegendEntry"/> class.
        /// </summary>
        public LegendEntry()
        {
            PosSet = false;
        }
    }
}
