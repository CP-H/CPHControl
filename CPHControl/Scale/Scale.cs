using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Class that stores the scaling properties for axes.
    /// </summary>
    public class Scale
    {
        #region Fields
        /// <summary>
        /// The minimum of the Scale
        /// </summary>
        internal double _min,
                                /// <summary>
                                /// The maximum of the Scale
                                /// </summary>
                                _max,
                                /// <summary>
                                /// The temporary maximum of the Scale
                                /// </summary>
                                _tmpMax,
                                /// <summary>
                                /// The temporary minimum of the Scale
                                /// </summary>
                                _tmpMin,
                                /// <summary>
                                /// The major step of the Scale
                                /// </summary>
                                _majorStep,
                                /// <summary>
                                /// The minor step of the Scale
                                /// </summary>
                                _minorStep,
                                /// <summary>
                                /// The target x steps of the Scale
                                /// </summary>
                                _targetXSteps,
                                /// <summary>
                                /// The target y steps of the Scale
                                /// </summary>
                                _targetYSteps,
                                /// <summary>
                                /// The target minor x steps of the Scale
                                /// </summary>
                                _targetMinorXSteps,
                                /// <summary>
                                /// The target minor y steps of the Scale
                                /// </summary>
                                _targetMinorYSteps;
        /// <summary>
        /// The owner axis of this Scale
        /// </summary>
        internal Axis _ownerAxis;

        /// <summary>
        /// The minimum and maximum pix of the Scale
        /// </summary>
        internal double _minPix, _maxPix;

        /// <summary>
        /// The tics array, storing x- and y-Values of the single Tic.
        /// </summary>
        internal Vector2[] _ticsArray;

        /// <summary>
        /// The VBO of the Scale
        /// </summary>
        internal int _VBO;

        /// <summary>
        /// The VAO of the Scale
        /// </summary>
        internal int _VAO;

        /// <summary>
        /// The reference to the TextInstance Object of the Control
        /// </summary>
        internal TextInstance _textInstance;

        /// <summary>
        /// The reference to the Shader of this Class
        /// </summary>
        internal LinesShader _linesShader;

        /// <summary>
        /// The Matrix storing transformation values
        /// </summary>
        internal Matrix4 _mat;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Scale"/> class.
        /// </summary>
        /// <param name="ownerAxis">The owner axis.</param>
        /// <param name="TextInstance">The text instance.</param>
        public Scale(Axis ownerAxis, TextInstance TextInstance)
        {
            _ownerAxis = ownerAxis;
            _textInstance = TextInstance;

            _min = 0.0;
            _max = 10.0;

            _majorStep = 1;
            _minorStep = 0.1;
            _targetXSteps = 7;
            _targetYSteps = 7;
            _targetMinorXSteps = 5;
            _targetMinorYSteps = 5;

            _VBO = GL.GenBuffer(); //generate VBO, VAO and setting them up here
            _VAO = GL.GenVertexArray();

            _linesShader = TextInstance.Shaders.LineShader;
            _linesShader.Use();

            GL.BindVertexArray(_VAO);

            GL.EnableVertexAttribArray(_linesShader.VertexLocation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);

            _mat = Matrix4.Identity;
            GL.UniformMatrix4(_linesShader.MatrixLocation, false, ref _mat);

            GL.VertexAttribPointer(_linesShader.VertexLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }
        #endregion

        #region Properties
        public virtual double Max
        {
            get { return _max; }
            set { _max = value; }
        }
        public virtual double Min
        {
            get { return _min; }
            set { _min = value; }
        }
        public virtual double TmpMax
        {
            get { return _tmpMax; }
            set { _tmpMax = value; }
        }
        public virtual double TmpMin
        {
            get { return _tmpMin; }
            set { _tmpMin = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws the Scale in relation to the pChart, first setting up values, then
        /// calling the functions to draw the Tics
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        public void Draw(GraphPane pane, Chart chart)
        {
            SetupScaleData(chart, _ownerAxis);

            int nTics = CalcNumTics();

            double targetSteps = _ownerAxis is XAxis ?
                            _targetXSteps : _targetYSteps;

            _majorStep = CalcStepSize(_max - _min, targetSteps);
            _minorStep = CalcStepSize(_majorStep,
                     _ownerAxis is XAxis ? _targetMinorXSteps : _targetMinorYSteps);
            DrawMajorTics(nTics, pane, chart);
            DrawMinorTics(chart);
        }

        /// <summary>
        /// Draws the major tics. First their Number and their 
        /// Positions are calculated, then they are given to the 
        /// VBO as vertices to then be drawn.
        /// </summary>
        /// <param name="nTics">The n tics.</param>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        internal void DrawMajorTics(int nTics, GraphPane pane, Chart chart)
        {
            double dVal;
            float pixVal;
            MajorTic tic = _ownerAxis._majorTic;
            int firstTic = (int)(_min / _majorStep + 0.99);
            int k = 0;

            _ticsArray = new Vector2[2 * nTics + 2];

            for (int i = firstTic; i < nTics + firstTic; i++)
            {
                dVal = _majorStep * i;
                // If we're before the start of the scale, just go to the next tic
                if (dVal < _min)
                    continue;
                // if we've already past the end of the scale, then we're done
                if (dVal > _max)
                    break;

                pixVal = LocalTransform(dVal);
                if (this._ownerAxis is XAxis)//Add two x/y-tupels to the array, one above, one below the axis
                {
                    _ticsArray[k].X = -2 * pixVal / chart.Rect.Width + 1;
                    _ticsArray[k + 1].X = -2 * pixVal / chart.Rect.Width + 1;
                    _ticsArray[k].Y = -1;
                    _ticsArray[k + 1].Y = 1;
                }
                else if (this._ownerAxis is YAxis)
                {
                    _ticsArray[k].X = -1;
                    _ticsArray[k + 1].X = 1;
                    _ticsArray[k].Y = -2 * pixVal / chart.Rect.Height + 1;
                    _ticsArray[k + 1].Y = -2 * pixVal / chart.Rect.Height + 1;
                }
                DrawTicValue(pane, chart, this._ownerAxis, pixVal, dVal);//at the position of every majorTic, draw a number
                k += 2;
            }

            if (this._ownerAxis is XAxis)
            {//the Viewports are directly put over the axis, so the _ticsArray has numbers rangig from -1 to 1 int one direction, just fitting the rectangle
                GL.Viewport((int)chart.Rect.X, (int)(chart.Rect.Y - tic.Size), (int)chart.Rect.Width, (int)(2 * tic.Size));
            }
            else if (this._ownerAxis is YAxis)
            {
                GL.Viewport((int)(chart.Rect.X - tic.Size - _ownerAxis._fixedSpace * (this._ownerAxis as YAxis).Index), (int)chart.Rect.Y, (int)(2 * tic.Size), (int)chart.Rect.Height);
                if ((this._ownerAxis as YAxis).Index != 0)
                {//Draw the vertical line for yAxes that dont border the pChart area and so dont have one by default.
                    _ticsArray[k].X = 0;
                    _ticsArray[k + 1].X = 0;
                    _ticsArray[k].Y = -1;
                    _ticsArray[k + 1].Y = 1;
                    k += 2;
                }
            }

            _linesShader.Use();//use Shader, bind VAO, bind VBO and give the tic values to the VBO
            GL.BindVertexArray(_VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * _ticsArray.Length), _ticsArray, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(_linesShader.VertexLocation, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.Uniform4(_linesShader.ColorLocation, Color.Black);//set uniform variables Color and Transformationmatrix
            GL.UniformMatrix4(_linesShader.MatrixLocation, false, ref _mat);

            GL.DrawArrays(PrimitiveType.Lines, 0, k);//draw the lines
            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }
        /// <summary>
        /// Draws the minor tics.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void DrawMinorTics(Chart chart)
        {
            MinorTic tic = _ownerAxis._minorTic;

            // Minor tics start at the minimum value and step all the way through
            // the full scale.  This means that if the minor step size is not
            // an even division of the major step size, the minor tics won't
            // line up with all of the scale labels and major tics.
            double first = _min, last = _max;
            int k = 0;

            double dVal = first;
            float pixVal;

            int iTic = (int)(_min / _minorStep);
            int firstTic = iTic;
            _ticsArray = new Vector2[600];

            int MajorTic = 0;
            double majorVal = _majorStep * MajorTic;

            // Draw the minor tic marks
            while (dVal < last && iTic < 300 + firstTic)
            {
                // Calculate the scale value for the current tic
                dVal = _minorStep * iTic;
                // Maintain a value for the current major tic
                if (dVal > majorVal)
                    majorVal = _majorStep * (++MajorTic);

                // Make sure that the current value does not match up with a major tic
                if (((Math.Abs(dVal) < 1e-20 && Math.Abs(dVal - majorVal) > 1e-20) ||
                    (Math.Abs(dVal) > 1e-20 && Math.Abs((dVal - majorVal) / dVal) > 1e-10)))
                {
                    pixVal = LocalTransform(dVal);

                    if (this._ownerAxis is XAxis)
                    {
                        _ticsArray[k].X = -2 * pixVal / chart.Rect.Width + 1;
                        _ticsArray[k + 1].X = -2 * pixVal / chart.Rect.Width + 1;
                        _ticsArray[k].Y = -1;
                        _ticsArray[k + 1].Y = 1;
                    }
                    else if (this._ownerAxis is YAxis)
                    {
                        _ticsArray[k].X = -1;
                        _ticsArray[k + 1].X = 1;
                        _ticsArray[k].Y = -2 * pixVal / chart.Rect.Height + 1;
                        _ticsArray[k + 1].Y = -2 * pixVal / chart.Rect.Height + 1;
                    }
                    k += 2;
                }

                iTic++;
            }
            if (this._ownerAxis is XAxis)
            {//the Viewports are directly put over the axis, so the _ticsArray has numbers rangig from -1 to 1 int one direction, just fitting the rectangle
                GL.Viewport((int)chart.Rect.X, (int)(chart.Rect.Y - tic.Size), (int)chart.Rect.Width, (int)(2 * tic.Size));
            }
            else if (this._ownerAxis is YAxis)
            {
                GL.Viewport((int)(chart.Rect.X - tic.Size - _ownerAxis._fixedSpace * (this._ownerAxis as YAxis).Index), (int)chart.Rect.Y, (int)(2 * tic.Size), (int)chart.Rect.Height);
            }

            _linesShader.Use();
            GL.BindVertexArray(_VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * _ticsArray.Length), _ticsArray, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(_linesShader.VertexLocation, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.Uniform4(_linesShader.ColorLocation, Color.Black);
            GL.UniformMatrix4(_linesShader.MatrixLocation, false, ref _mat);

            GL.DrawArrays(PrimitiveType.Lines, 0, k);
            GL.BindVertexArray(0);
            _linesShader.StopUse();
        }

        /// <summary>
        /// Transforms the value containing the number of the tic into the pixel range
        /// of the scale.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public float LocalTransform(double x)
        {
            double ratio;
            float rv;

            ratio = (x - _min) / (_max - _min);

            rv = (float)((_maxPix - _minPix) * (1.0f - ratio));

            return rv;
        }

        /// <summary>
        /// Setups the scale min pix and max pix, depending on owner axis.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="axis">The axis.</param>
        virtual public void SetupScaleData(Chart chart, Axis axis)
        {
            // save the ChartRect data for transforming scale values to pixels
            if (axis is XAxis)
            {
                _minPix = chart._rect.Left;
                _maxPix = chart._rect.Right;
            }
            else
            {
                _minPix = chart._rect.Bottom;
                _maxPix = chart._rect.Top;
            }
        }

        /// <summary>
        /// Calculates the number of tics.
        /// </summary>
        /// <returns></returns>
        virtual internal int CalcNumTics()
        {
            int nTics = 1;

            // default behavior is for a linear or ordinal scale
            nTics = (int)((_max - _min) / _majorStep + 0.01) + 1;

            if (nTics < 1)
                nTics = 1;
            else if (nTics > 1000)
                nTics = 1000;

            return nTics;
        }
        /// <summary>
        /// Draws the tic value.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="pixValue">The pix value.</param>
        /// <param name="Value">The value.</param>
        internal void DrawTicValue(GraphPane pane, Chart chart, Axis axis, double pixValue, double Value)
        {
            float x;
            float y;
            string s_value;
            double diff = axis.Scale.Max - axis.Scale.Min;
            if (diff > 8)//Depending on the number Size, this determines the format
            {
                s_value = string.Format("{0:0.}", Value);
            }
            else if (diff > 0.8)
            {
                s_value = string.Format("{0:0.0}", Value);
            }
            else if (diff > 0.08)
            {
                s_value = string.Format("{0:0.00}", Value);
            }
            else
            {
                s_value = string.Format("{0:0.000}", Value);
            }

            if (axis is XAxis)
            {//calculate the x- and y-Values for the String to draw
                x = 1 - 2 * ((float)pixValue) / pane.Rect.Width - 2 * (pane.Rect.Width + pane.Rect.X - chart.Rect.X - chart.Rect.Width) / pane.Rect.Width;
                y = -1 + 2 * (chart.Rect.Y - 2.0f * axis.MajorTic.Size) / pane.Rect.Height;
            }
            else
            {
                x = -1 + 2 * (chart.Rect.X - axis.MajorTic.Size - (axis as YAxis).Index * axis.FixedSpace) / pane.Rect.Width;
                y = 1 - 2 * ((float)pixValue) / (pane.Rect.Height) - 2 * (pane.Rect.Height + pane.Rect.Y - chart.Rect.Y - chart.Rect.Height) / pane.Rect.Height;
            }
            _textInstance.DrawTicValues(x, y, pane, axis, s_value);//let the TextInstance do the actual drawing
        }
        /// <summary>
        /// Calculates the step sizes
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="targetSteps">The target steps.</param>
        /// <returns></returns>
        protected static double CalcStepSize(double range, double targetSteps)
        {
            // Calculate an initial guess at step size
            double tempStep = range / targetSteps;

            // Get the magnitude of the step size
            double mag = Math.Floor(Math.Log10(tempStep));
            double magPow = Math.Pow((double)10.0, mag);

            // Calculate most significant digit of the new step size
            double magMsd = ((int)(tempStep / magPow + .5));

            // promote the MSD to either 1, 2, or 5
            if (magMsd > 5.0)
                magMsd = 10.0;
            else if (magMsd > 2.0)
                magMsd = 5.0;
            else if (magMsd > 1.0)
                magMsd = 2.0;

            return magMsd * magPow;
        }
        #endregion
    }
}
