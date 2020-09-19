using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Class that holds the properties of the major tics. Inherits from
    /// <see cref="MinorTic"/>
    /// </summary>
    /// <seealso cref="global::CPHControl.MinorTic" />
    public class MajorTic : MinorTic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MajorTic"/> class.
        /// </summary>
        public MajorTic()
        {
            _size = Default.Size;
        }

        #region Defaults
        /// <summary>
        /// 
        /// </summary>
        public new struct Default
        {
            /// <summary>
            /// The size
            /// </summary>
            public static int Size = 5;
        }
        #endregion
    }
}
