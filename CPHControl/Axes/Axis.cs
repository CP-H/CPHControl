﻿using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Base Axis class containing properties for all axes.
    /// <seealso cref="YAxis"/>
    /// <seealso cref="XAxis"/>
    /// </summary>
    abstract public class Axis
    {
        #region Fields
        /// <summary>
        /// The minor tic property
        /// </summary>
        internal MinorTic _minorTic;

        /// <summary>
        /// The major tic property
        /// </summary>
        internal MajorTic _majorTic;

        /// <summary>
        /// The title property
        /// </summary>
        protected AxisLabel _title;

        /// <summary>
        /// The axis gap as space between axes
        /// </summary>
        private int _axisGap; //Platz zwischen Axen

        /// <summary>
        /// The scale of the axis
        /// </summary>
        internal Scale _scale;

        /// <summary>
        /// A reference to the <see cref="TextInstance"/>
        /// </summary>
        internal TextInstance _textInstance;

        /// <summary>
        /// The fixed space
        /// </summary>
        internal int _fixedSpace;

        /// <summary>
        /// The number space
        /// </summary>
        internal int _numSpace;

        /// <summary>
        /// The zoom difference storing an offset generated by zooming and panning
        /// </summary>
        private double _zoomDiff;

        /// <summary>
        /// The zoom correction storing a correction value to make
        /// zooming and panning smooth
        /// </summary>
        private double _zoomCorrection;
        #endregion

        #region Defaults
        /// <summary>
        /// Default values for the Axis
        /// </summary>
        public struct Default
        {
            public static int AxisGap = 2;
            public static int numSpace = 20;
            public static double zoomDiff = 0.0;
            public static double zoomCorrection = 0.0;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public Scale Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public AxisLabel Title
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// Gets the major tic.
        /// </summary>
        /// <value>
        /// The major tic.
        /// </value>
        public MajorTic MajorTic
        {
            get { return _majorTic; }
        }
        /// <summary>
        /// Gets the fixed space.
        /// </summary>
        /// <value>
        /// The fixed space.
        /// </value>
        public int FixedSpace
        {
            get { return _fixedSpace; }
        }
        /// <summary>
        /// Gets or sets the zoom difference.
        /// </summary>
        /// <value>
        /// The zoom difference.
        /// </value>
        public double ZoomDiff
        {
            get { return _zoomDiff; }
            set { _zoomDiff = value; }
        }
        /// <summary>
        /// Gets or sets the zoom correction.
        /// </summary>
        /// <value>
        /// The zoom correction.
        /// </value>
        public double ZoomCorrection
        {
            get { return _zoomCorrection; }
            set { _zoomCorrection = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> class
        /// mostly with default values.
        /// </summary>
        /// <param name="TextInstance">The text instance.</param>
        public Axis(TextInstance TextInstance)
        {
            _majorTic = new MajorTic();
            _minorTic = new MinorTic();

            _zoomCorrection = Default.zoomCorrection;
            _zoomDiff = Default.zoomDiff;

            _axisGap = Default.AxisGap;
            _numSpace = Default.numSpace;
            _scale = new Scale(this, TextInstance);

            _textInstance = TextInstance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> class
        /// with a given title string.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="TextInstance">The text instance.</param>
        public Axis(string title, TextInstance TextInstance) : this(TextInstance)
        {
            _title = new AxisLabel(title, TextInstance);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws the Axis scale and title.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        public void Draw(GraphPane pane, Chart chart)
        {
            _scale.Draw(pane, chart);
            _title.Draw(pane, chart, this);
        }
        /// <summary>
        /// Calculates the space for this axis, depending on the kind of axis it is.
        /// </summary>
        /// <param name="fixedSpace">The fixed space.</param>
        /// <returns></returns>
        public int CalcSpace(out int fixedSpace)
        {
            fixedSpace = _axisGap + _majorTic.Size + _textInstance.GlyphHeightPix;
            if (this is YAxis)
                fixedSpace += _numSpace;

            _fixedSpace = fixedSpace;
            return fixedSpace;
        }
        #endregion
    }
}
