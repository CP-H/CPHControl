using System.Drawing;

namespace CPHControl
{
    /// <summary>
    /// 
    /// </summary>
    abstract public class PaneBase
    {
        #region Fields
        protected GLRectangleF _rect;
        internal Margin _margin;
        protected GL_Border _glBorder;
        protected Shaders _shaders;
        internal Legend _legend;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the rectangle of this
        /// </summary>
        public GLRectangleF Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PaneBase"/> class.
        /// </summary>
        /// <param name="paneRect">The pane rect.</param>
        /// <param name="textInstance">The text instance.</param>
        public PaneBase(RectangleF paneRect, TextInstance textInstance)
        {
            _rect = new GLRectangleF(0, 0, 0, 0);
            _rect.X = (int)paneRect.X;
            _rect.Height = (int)paneRect.Height;
            _rect.Width = (int)paneRect.Width;
            _rect.Y = (int)paneRect.Bottom;

            _margin = new Margin();

            _glBorder = new GL_Border(Color.Black, textInstance.Shaders);

            _legend = new Legend(textInstance);
        }
        #endregion

        #region Methods
        /// <summary>
        /// drawing method to override
        /// </summary>
        /// <param name="zoomPanState">State of the zoom pan.</param>
        public virtual void Draw(ZoomPanState zoomPanState)
        {
            if (_rect.Width <= 1 || _rect.Height <= 1)
                return;

            DrawPaneFrame();
        }
        /// <summary>
        /// drawing method to override
        /// </summary>
        /// <param name="zoomPanState">State of the zoom pan.</param>
        public virtual void InitDraw(ZoomPanState zoomPanState)
        {
            if (_rect.Width <= 1 || _rect.Height <= 1)
                return;

            DrawPaneFrame();
        }
        /// <summary>
        /// Calculates the client rect.
        /// </summary>
        /// <returns>a rectangle reduced by the <see cref="Margin"/> values</returns>
        public GLRectangleF CalcClientRect()
        {
            GLRectangleF innerRect = new GLRectangleF(
                            _rect.X + _margin.Left,
                            _rect.Y + _margin.Bottom,
                            _rect.Width -  (_margin.Left + _margin.Right),
                            _rect.Height - (_margin.Top + _margin.Bottom));

            this._legend.CalcRect(this, ref innerRect );

            return innerRect;
        }
        /// <summary>
        /// Draws the pane frame.
        /// </summary>
        public void DrawPaneFrame()
        {
            GLRectangleF rect = new GLRectangleF(_rect.X, _rect.Y, _rect.Width - 1, _rect.Height - 1);

            _glBorder.Draw(rect);
        }
        /// <summary>
        /// Resizes the Rectangle, setting the posset of the first <see cref="LegendEntry"/>
        /// so that the legend gets resized after this happens
        /// </summary>
        /// <param name="rect">The rect.</param>
        public virtual void ReSize(GLRectangleF rect)
        {
            _rect = rect;
            try
            {
                _legend[0].PosSet = false;
            }
            catch
            {

            }
        }
        #endregion
    }
}
