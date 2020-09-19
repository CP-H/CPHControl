using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// The Class spanning the whole <see cref="global::CPHControl" /> area and containing
    /// the drawable objects. It inherits some Properties from <see cref="PaneBase" />
    /// </summary>
    /// <seealso cref="global::CPHControl.PaneBase" />
    public class GraphPane : PaneBase
    {
        #region Fields
        /// <summary>
        /// The List of Curves that shall be plotted
        /// </summary>
        internal CurveList _curveList;
        /// <summary>
        /// The Chart containing the area where the Curves shall be drawn
        /// </summary>
        internal Chart _chart;
        /// <summary>
        /// The xAxis, only a single one contrary to the yAxes
        /// </summary>
        internal XAxis _xAxis;
        /// <summary>
        /// The list of yAxes
        /// </summary>
        internal YAxisList _yAxisList;
        /// <summary>
        /// The reference to the Textinstance used for Text drawing
        /// </summary>
        internal TextInstance _textInstance;
        /// <summary>
        /// The frame storing the time to calculate FPS
        /// </summary>
        internal double _frameTime;
        /// <summary>
        /// The run time of the <see cref="global::CPHControl"/>
        /// </summary>
        internal double _runTime;
        #endregion

        #region Properties
        /// <summary>
        /// Gets <see cref="Chart"/> property.
        /// </summary>
        /// <value>
        /// The chart.
        /// </value>
        public Chart PChart
        {
            get { return _chart; }
        }
        /// <summary>
        /// Makes the <see cref="CurveList"/> public
        /// </summary>
        /// <value>
        /// The CurveList of this GraphPane
        /// </value>
        public CurveList PCurveList
        {
            get { return _curveList; }
            set { _curveList = value; }
        }
        /// <summary>
        /// Makes the <see cref="YAxisList"/> public
        /// </summary>
        /// <value>
        /// The YAxisList of this GraphPane
        /// </value>
        public YAxisList PYAxisList
        {
            get { return _yAxisList; }
            set { _yAxisList = value; }
        }
        /// <summary>
        /// 
        /// Makes the <see cref="XAxis"/> public
        /// </summary>
        /// <value>
        /// The xAxis of this GraphPane
        /// </value>
        public XAxis PXAxis
        {
            get { return _xAxis; }
            set { _xAxis = value; }
        }
        /// <summary>
        /// 
        /// Makes the <see cref="Legend"/> public
        /// </summary>
        /// <value>
        /// The Legend of this GraphPane
        /// </value>
        public Legend PLegend
        {
            get { return _legend; }
            set { _legend = value; }
        }
        /// <summary>
        /// Gets or sets the frame time.
        /// </summary>
        /// <value>
        /// The frame time.
        /// </value>
        public double FrameTime
        {
            get { return _frameTime; }
            set { _frameTime = value; }
        }
        /// <summary>
        /// Gets or sets the run time.
        /// </summary>
        /// <value>
        /// The run time.
        /// </value>
        public double RunTime
        {
            get { return _runTime; }
            set { _runTime = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphPane"/> class,
        /// If there is only a TextInstance given. This is used in the 
        /// <see cref="global::CPHControl"/> Constructor
        /// </summary>
        /// <param name="TextInstance">The text instance.</param>
        public GraphPane(TextInstance TextInstance)
            : this(new RectangleF(0, 0, 500, 375), TextInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphPane"/> class
        /// using a Rectangle describing the screen space.
        /// </summary>
        /// <param name="PaneRect">The pane rect.</param>
        /// <param name="TextInstance">The text instance.</param>
        public GraphPane(RectangleF PaneRect, TextInstance TextInstance) : base(PaneRect, TextInstance)
        {
            _chart = new Chart(TextInstance.Shaders);
            _curveList = new CurveList(this, TextInstance);
            _xAxis = new XAxis(TextInstance);
            _yAxisList = new YAxisList(TextInstance);
            _textInstance = TextInstance;
            _shaders = TextInstance.Shaders;
            _margin.Left = 2;
            _margin.Top = 8;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws the whole <see cref="global::CPHControl"/> one part after the other.
        /// </summary>
        /// <param name="zoomPanState">State of the zoom and pan.</param>
        public override void Draw(ZoomPanState zoomPanState)
        {
            base.Draw(zoomPanState);

            _chart.Rect = CalcChartRect();

            _chart.pGL_Border.Draw(_chart.Rect);

            _textInstance.DrawTimes(this, _frameTime, _runTime);

            _yAxisList.SetUpScales(this, zoomPanState, this.PXAxis);

            _curveList.Draw(this);

            _xAxis.Draw(this, _chart);

            _yAxisList.Draw(this, _chart);

            _legend.Draw(this);
        }
        /// <summary>
        /// Draws an empty <see cref="global::CPHControl"/> so the screen is not black
        /// if there are no points
        /// </summary>
        /// <param name="zoomPanState">State of the zoom pan.</param>
        public override void InitDraw(ZoomPanState zoomPanState)
        {
            base.InitDraw(zoomPanState);

            _chart.Rect = CalcChartRect();

            _chart.pGL_Border.Draw(_chart.Rect);

            _yAxisList.SetUpScales(this, zoomPanState, this.PXAxis);

            _xAxis.Draw(this, _chart);

            _yAxisList.Draw(this, _chart);
        }

        /// <summary>
        /// Calculates the chart rect.
        /// </summary>
        /// <returns>The rectangle where the <see cref="CurveList"/> can be drawn.
        /// It gets smaller the more axes and the more Legend entries exist.</returns>
        public GLRectangleF CalcChartRect()
        {
            // chart rect starts out at the full pane rect less the margins
            //   and less space for the Pane title
            int minSpaceL = 0;
            GLRectangleF clientRect = this.CalcClientRect();

            _xAxis.CalcSpace(out int minSpaceB);

            foreach (Axis axis in _yAxisList)
            {
                int tmp = axis.CalcSpace(out int fixedSpace);

                minSpaceL += fixedSpace;
            }
            clientRect.X += minSpaceL;
            clientRect.Y += minSpaceB;
            clientRect.Height -= minSpaceB;
            clientRect.Width -= minSpaceL;
            return clientRect;
        }

        /// <summary>
        /// Adds a Curve to the <see cref="GraphPane"/>, effectively
        /// generating an entry to the <see cref="Legend"/> and the new <see cref="RollingVec2List"/>
        /// to the <see cref="CurveList"/>.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="points">The points.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public CurveItem AddCurve(string label, RollingVec2List points, Color color)
        {
            CurveItem curve = new CurveItem(this, label, points, color, _shaders.LineShader);
            _curveList.Add(curve);
            _legend.AddItem(label, color);

            return curve;
        }

        /// <summary>
        /// Resets the Correction values that are generated because of small
        /// differences happening when zooming and panning.
        /// </summary>
        public void ResetZoomDiffs()
        {
            PXAxis.ZoomDiff = 0;
            PXAxis.ZoomCorrection = 0;

            foreach(YAxis axis in _yAxisList)
            {
                axis.ZoomCorrection = 0;
                axis.ZoomDiff = 0;
            }
        }
        #endregion
    }
}
