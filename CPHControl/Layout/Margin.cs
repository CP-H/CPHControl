namespace CPHControl
{
    /// <summary>
    /// Class that stores margins
    /// </summary>
    public class Margin
    {
        protected int _left,
                                _right,
                                _top,
                                _bottom;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> class.
        /// storing margins around certain objects.
        /// </summary>
        public Margin()
        {
            _left = Default.Left;
            _right = Default.Right;
            _top = Default.Top;
            _bottom = Default.Bottom;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the left margin
        /// </summary>
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }
        /// <summary>
        /// Gets or sets the right margin
        /// </summary>
        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }
        /// <summary>
        /// Gets or sets the top margin
        /// </summary>
        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }
        /// <summary>
        /// Gets or sets the bottom margin
        /// </summary>
        public int Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }
        /// <summary>
        /// Sets all margins
        /// </summary>
        public int All
        {
            set
            {
                _bottom = value;
                _top = value;
                _left = value;
                _right = value;
            }
        }
        #endregion

        #region Defaults
        /// <summary>
        /// Default values used in the constructor
        /// </summary>
        public class Default
        {
            public static int Left = 10;
            public static int Right = 10;
            public static int Top = 5;
            public static int Bottom = 5;
        }
        #endregion
    }
}
