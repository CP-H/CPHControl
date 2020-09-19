using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Class that holds the specific properties for the minor tics.
    /// </summary>
    public class MinorTic
    {
        #region Fields
        /// <summary>
        /// The size of the tics.
        /// </summary>
        protected int _size;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MinorTic"/> class.
        /// </summary>
        public MinorTic()
        {
            _size = Default.Size;
        }
        #endregion

        #region Defaults
        /// <summary>
        /// 
        /// </summary>
        public struct Default
        {
            /// <summary>
            /// The size
            /// </summary>
            public static int Size = 3;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }
        #endregion
    }
}
