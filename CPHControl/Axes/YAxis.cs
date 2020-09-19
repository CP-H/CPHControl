using OpenTK;

namespace CPHControl
{
    /// <summary>
    /// This is a yAxis, meaning it is a vertical axis to the left
    /// of the <see cref="Chart"/> area of a <see cref="GraphPane"/>.
    /// </summary>
    /// <seealso cref="global::CPHControl.Axis" />
    public class YAxis : Axis
    {
        #region Fields
        /// <summary>
        /// The index of this yAxis, since there is a <see cref="YAxisList"/> 
        /// containing all <see cref="YAxis"/> Objects.
        /// </summary>
        protected int _index;
        /// <summary>
        /// The transformation matrix storing the transformation for all 
        /// <see cref="CurveItem"/> Objects that are connected to this <see cref="YAxis"/>
        /// </summary>
        protected Matrix4 _traFo;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets or sets the transformation matrix
        /// </summary>
        /// <value>
        /// The matrix
        /// </value>
        public Matrix4 TraFo
        {
            get { return _traFo; }
            set { _traFo = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="YAxis"/> class
        /// with the given index.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="TextInstance">The text instance.</param>
        /// <param name="index">The index.</param>
        public YAxis(string title, TextInstance TextInstance, int index)
            : base(title, TextInstance)
        {
            _index = index;
        }
        #endregion
    }
}
