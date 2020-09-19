using CPHControl.Properties;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace CPHControl
{
    /// <summary>
    /// Primary Class of this Control.
    /// </summary>
    /// <seealso cref="OpenTK.GLControl" />
    public partial class CPHControl : GLControl
    {
        #region Fields
        /// <summary>
        /// The rectangle giving space to this
        /// </summary>
        private Rectangle _rect;
        /// <summary>
        /// The graph pane managing the drawable Objects
        /// </summary>
        private GraphPane _graphPane;
        /// <summary>
        /// The <see cref="TextInstance"/> storing Shaders and all properties 
        /// needed to draw text
        /// </summary>
        protected TextInstance _textInstance;
        /// <summary>
        /// The shaders
        /// </summary>
        protected Shaders _shaders;

        /// <summary>
        /// The Object handling zooming and panning.
        /// Manipulates scales to apply zoom and pan actions.
        /// </summary>
        private ZoomPanState _zoomPanState;

        /// <summary>
        /// The timer setting the pace for redrawing
        /// </summary>
        protected Timer _FrameTimer = new Timer();
        protected Stopwatch _stopwatch;
        protected Stopwatch _stopwatch2;
        #endregion

        #region Properties
        /// <summary>
        /// Makes the <see cref="GraphPane"/> public so the User can edit it
        /// </summary>
        public GraphPane GraphPane
        {
            get
            {
                if (_graphPane != null)
                    return _graphPane;
                else return null;
            }
        }

        /// <summary>
        /// Makes the <see cref="TextInstance"/> public.
        /// </summary>
        public TextInstance TextInstance
        {
            get { return _textInstance; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CPHControl"/> class.
        /// Initializes OpenGL.
        /// </summary>
        public CPHControl()
        {
            SetStyle(ControlStyles.UserMouse, true);
            this.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);

            using (OpenTK.Toolkit.Init())
            {
                InitializeComponent();
            }

        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load" /> event.
        /// Sets all the properties up but only if the form is not in designer mode,
        /// because OpenGL calls in the Designer cause errors.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _rect = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            this.MakeCurrent();
            if (!(IsInDesignerMode()))
            {
                _shaders = new Shaders(Resources.LineShader_vert,Resources.LineShader_frag,Resources.TextShader_vert,Resources.TextShader_frag);
                _textInstance = new TextInstance(_shaders);

                _graphPane = new GraphPane(_rect, _textInstance);

                _graphPane.ReSize(new GLRectangleF(0, 0, (int)_rect.Width, (int)_rect.Height));
                _textInstance.GenerateFontImage(_graphPane.Rect);
                GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);

                _zoomPanState = new ZoomPanState(ref _graphPane);

                _FrameTimer.Tick += FrameTimer_Tick;
                _FrameTimer.Interval = 25;

                _FrameTimer.Start();
                _stopwatch = new Stopwatch();
                _stopwatch2 = new Stopwatch();
                _stopwatch.Reset();
                _stopwatch2.Reset();
                _stopwatch.Start();
            }
        }

        /// <summary>
        /// Handles the Tick event of the FrameTimer control.
        /// The value is used to display FPS (Frames Per Second).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            _graphPane.FrameTime = (double)(_stopwatch.ElapsedMilliseconds) / 1000.0;
            _stopwatch.Restart();
            this.Invalidate();//starts the OnPaint() function
        }
        /// <summary>
        /// Raises the System.Windows.Forms.Control.Paint event.
        /// If not in Designer mode, this draws the whole control.
        /// </summary>
        /// <param name="e">A System.Windows.Forms.PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!(IsInDesignerMode()))
                _stopwatch2.Restart();
            lock (this)
            {
                base.OnPaint(e);

                try
                {
                    if (!(IsInDesignerMode()))
                    {
                        DoZoomPanActions();//zoom or pan in this place and edit the scales if needed

                        GL.ClearDepth(1);
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                        GL.Enable(EnableCap.DepthTest);
                        _graphPane.Draw(_zoomPanState);//Draw!
                    }
                }
                catch 
                {
                    _graphPane.InitDraw(_zoomPanState);//if the usual draw command throws an error, do a minimal draw
                }
                SwapBuffers();//OpenGL now shows all the drawn items
            }
            if (!(IsInDesignerMode()))
                _graphPane.RunTime = (double)(_stopwatch2.ElapsedMilliseconds);
        }

        /// <summary>
        /// Raises the Resize event.
        /// Note: this method may be called before the OpenGL context is ready.
        /// Check that IsHandleCreated is true before using any OpenGL methods.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_graphPane == null)
                return;

            Size newSize = this.Size;
            _graphPane.ReSize(new GLRectangleF(0, 0, newSize.Width, newSize.Height));
        }
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            PointF GLPoint = SystemPointfToGL(new PointF(e.X, e.Y));
            if (_graphPane.PChart.Rect.Contains(GLPoint))
            {
                _zoomPanState.Zoom(GLPoint, ref _graphPane, e.Delta);
            }
        }

        /// <summary>
        /// Represents an element in an XML resource (.resx) file.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            PointF GLPoint = SystemPointfToGL(new PointF(e.X , e.Y));
            if (_graphPane.PChart.Rect.Contains(GLPoint))
            {
                _zoomPanState.StartPan(GLPoint, _graphPane);
            }
        }

        /// <summary>
        /// Does the zoom pan actions.
        /// </summary>
        protected void DoZoomPanActions()
        {
            if (Control.MouseButtons == MouseButtons.Left && _zoomPanState.Panning)
            {
                Point mousepoint = PointToClient(System.Windows.Forms.Control.MousePosition);

                PointF GLPoint = SystemPointfToGL(new PointF(mousepoint.X, mousepoint.Y));
                _zoomPanState.Pan(GLPoint, ref _graphPane);
            }
            else if (Mouse.LeftButton == MouseButtonState.Released && _zoomPanState.Panning)
            {
                _zoomPanState.Panning = false;
            }
            else if (_zoomPanState.IsAutoPan)
            {
                double xMax = _graphPane.PCurveList.GetXMax(_graphPane);
                _zoomPanState.AutoPan(xMax, ref _graphPane._xAxis);
            }
        }

        /// <summary>
        /// Systems the pointf to gl.
        /// </summary>
        /// <param name="SystemPoint">The system point.</param>
        /// <returns></returns>
        PointF SystemPointfToGL(PointF SystemPoint)
        {
            PointF NewPoint;
            NewPoint = SystemPoint;
            NewPoint.Y = this.Size.Height - SystemPoint.Y;
            return NewPoint;
        }

        /// <summary>
        /// Determines whether [is in designer mode].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is in designer mode]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInDesignerMode()
        {
            return AppDomain.CurrentDomain.FriendlyName.Contains("DefaultDomain");
        }
        #endregion
    }
}
