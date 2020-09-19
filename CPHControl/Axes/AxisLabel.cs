namespace CPHControl
{
    /// <summary>
    /// This is the Class specifying the Label of an <see cref="Axis"/> Object.
    /// It contains a string and a reference to the <see cref="TextInstance"/>
    /// to draw itself.
    /// </summary>
    public class AxisLabel
    {
        /// <summary>
        /// The title
        /// </summary>
        protected string _title;

        /// <summary>
        /// The <see cref="TextInstance"/>
        /// </summary>
        protected TextInstance _textInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisLabel"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="TextInstance">The text instance.</param>
        public AxisLabel(string title, TextInstance TextInstance)
        {
            _title = title;
            _textInstance = TextInstance;
        }

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        /// <value>
        /// The title text
        /// </value>
        public string Text
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// Draws the Title of the specified axis.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        /// <param name="axis">The axis.</param>
        public void Draw(GraphPane pane, Chart chart, Axis axis)
        {
            _textInstance.DrawAxisTitle(pane, chart, axis, _title);
        }
    }
}
