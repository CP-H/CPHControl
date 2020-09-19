using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Area where the Graph draws its <see cref="CurveItem"/>s
    /// only containing a rectangle and a drawable border
    /// </summary>
    public class Chart
    {
        /// <summary>
        /// The Area on the <see cref="global::CPHControl"/>
        /// </summary>
        internal GLRectangleF _rect;

        /// <summary>
        /// The drawable Border
        /// </summary>
        internal GL_Border _glBorder;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// <param name="shaders">The shaders.</param>
        public Chart(Shaders shaders)
        {
            _glBorder = new GL_Border(Color.Black, shaders);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets rectangle
        /// </summary>
        /// <value>
        /// The rectangle
        /// </value>
        public GLRectangleF Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        /// <summary>
        /// Gets or sets the Border
        /// </summary>
        /// <value>
        /// The Border
        /// </value>
        public GL_Border pGL_Border
        {
            get { return _glBorder; }
            set { _glBorder = value; }
        }
        #endregion
    }
}
