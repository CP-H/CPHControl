using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// Class that specifies a GL version of the rect windows class,
    /// because the x and y values start in the top left corner in the original.
    /// we want them bottom left.
    /// </summary>
    public class GLRectangleF
    {
        #region Fields
        protected float _x,
                        _y,
                        _height,
                        _width,
                        _top,
                        _bottom,
                        _left,
                        _right;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the x value, 
        /// meaning the distance to the left edge.
        /// Also setting the right and left value.
        /// </summary>
        public float X
        {
            get { return _x; }
            set 
            {
                _x = value;
                _right = value + _width;
                _left = value;
            }
        }
        /// <summary>
        /// Gets or sets the y value, 
        /// meaning the distance to the bottom edge.
        /// Also setting the top and bottom value.
        /// </summary>
        public float Y
        {
            get { return _y; }
            set 
            {
                _y = value;
                _top = value + _height;
                _bottom = value;
            }
        }
        /// <summary>
        /// Gets or sets the height, which also changes the y value.
        /// </summary>
        public float Height
        {
            get { return _height; }
            set 
            { 
                _height = value;
                _top = _y + value;
            }
        }
        /// <summary>
        /// Gets or sets the width, which also changes the x value.
        /// </summary>
        public float Width
        {
            get { return _width; }
            set 
            { 
                _width = value;
                _right = _x + value;
            }
        }
        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        public float Top
        {
            get { return _top; }
            set { _top = value; }
        }
        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        public float Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        public float Left
        {
            get { return _left; }
            set { _left = value; }
        }
        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        public float Right
        {
            get { return _right; }
            set { _right = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GLRectangleF"/> class.
        /// </summary>
        /// <param name="x">The x offset of the edge</param>
        /// <param name="y">The y of the edge</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public GLRectangleF(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _left = x;
            _right = x + width;
            _bottom = y;
            _top = y + height;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether this instance contains the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified point]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PointF point)
        {
            if(point.X > _x
                && point.Y > _y
                && point.X < _right 
                && point.Y < _top)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
