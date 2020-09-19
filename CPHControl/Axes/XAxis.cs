namespace CPHControl
{
    /// <summary>
    /// This is the xAxis, meaning it is a horizontal axis at the bottom
    /// of the <see cref="Chart"/> area of a <see cref="GraphPane"/>.
    /// </summary>
    /// <seealso cref="global::CPHControl.Axis" />
    public class XAxis : Axis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XAxis"/> class.
        /// </summary>
        /// <param name="TextInstance">The text instance.</param>
        public XAxis(TextInstance TextInstance)
            : this("X Axis", TextInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAxis"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="TextInstance">The text instance.</param>
        public XAxis(string title, TextInstance TextInstance)
            : base(title, TextInstance)
        {
        }
    }
}
