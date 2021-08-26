namespace Idera.SQLdm.DesktopClient.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(ProgressBar))]
    public partial class InfiniteProgressCircle : Control, IDisposable
    {
        public enum Direction
        {
            Clockwise,
            CounterClockwise
        }

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(InfiniteProgressCircle));

        private const int NUMBER_OF_DEGREES_CIRCLE = 360;
        private const int NUMBER_OF_DEGREES_HALF_CIRCLE = NUMBER_OF_DEGREES_CIRCLE / 2;
        private const int DEFAULT_THICKNESS = 4;
        private const int DEFAULT_NUMBER_SEGMENTS = 4;
        private const int DEFAULT_ARROW_DEGREES = 5;
        private const int DEFAULT_SLOWEST_SPEED = 50;
        private const int DEFAULT_FASTEST_SPEED = 3;
        private static readonly Color DEFAULT_COLOR = Color.Green;

        private object sync = new object();

        private Direction direction = Direction.Clockwise;

        private Color colorStart;
        private Color colorEnd;

        private int aArrowDegrees;
        private int aThickness;
        private int aNumberSegments;
        private PointF aCenterPoint;
        private int origin = 0;
        private float sweepAngle;

        private RectangleF gageBounds;
        private LinearGradientBrush brush;
        private GraphicsPath graphicsPath;

        private double value = 0;
        private double maximumValue = 100;
        private Timer refreshTimer;

        private int timerSlowSpeed = DEFAULT_SLOWEST_SPEED;
        private int timerFastSpeed = DEFAULT_FASTEST_SPEED;

        public InfiniteProgressCircle()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.CacheText,
                true);

            direction = Direction.Clockwise;

            colorStart = DEFAULT_COLOR;
            colorEnd = Color.Transparent;

            InitializeComponent();

            OnGageChanged();

            this.Resize += new EventHandler(LoadingCircle_Resize);

            refreshTimer = new Timer();
            refreshTimer.SynchronizingObject = this;
            refreshTimer.Mode = TimerMode.Periodic;
            refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
            refreshTimer.Resolution = 10;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            switch (direction)
            {
                case Direction.Clockwise:
                    Origin++;
                    break;
                case Direction.CounterClockwise:
                    Origin--;
                    break;
            }
            Invalidate();
        }

        public double MaximumValue
        {
            get
            {
                return maximumValue;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("MaximumValue", "The MaximumValue cannot be less than zero");
                }

                maximumValue = value;
                SetTimerInterval();
            }
        }

        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value < 0)
                {
                    this.value = 0;
                }
                else if (value > maximumValue)
                {
                    this.value = maximumValue;
                }
                else
                {
                    this.value = value;
                }

                // TODO: This method has been commented out due to performance problems. We need to address the 
                // performance of the flow controls.
                //SetTimerInterval();
            }
        }

        [Category("Behavior")]
        [DefaultValue(DEFAULT_SLOWEST_SPEED)]
        public int SlowAnimationDelayMillis
        {
            get { return timerSlowSpeed; }
            set
            {
                if (value <= FastAnimationDelayMillis)
                {
                    throw new ArgumentOutOfRangeException("SlowAnimationDelayMillis", "Slow animation delay milliseconds must be greater than FastAnimationDelayMillis");
                }

                this.timerSlowSpeed = value;

                SetTimerInterval();
            }
        }

        [Category("Behavior")]
        [DefaultValue(DEFAULT_FASTEST_SPEED)]
        public int FastAnimationDelayMillis
        {
            get { return timerFastSpeed; }
            set
            {
                if (value <= 0 || value > SlowAnimationDelayMillis)
                {
                    throw new ArgumentOutOfRangeException("FastAnimationDelayMillis", "Fast animation delay milliseconds must be greater than zero and less than SlowAnimationDelayMillis");
                }

                this.timerFastSpeed = value;

                SetTimerInterval();
            }
        }

        private int Origin
        {
            get
            {
                lock (sync)
                {
                    return origin;
                }
            }
            set
            {
                lock (sync)
                {
                    if (value > 360)
                        origin = value % 360;
                    else
                        if (value < 0)
                            origin = 360 - (Math.Abs(value) % 360);
                        else
                            origin = value;
                }
            }
        }


        [Category("LoadingCircle"),
         Description("Sets the direction the segments turn.  (Clockwise is all that works right now)")]
        public Direction Rotation
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    OnGageChanged();
                    Invalidate();
                }
            }
        }

        [Category("LoadingCircle"),
         Description("Sets the number of degrees to use for creating the triangle at the start of a segment")]
        public int ArrowDegrees
        {
            get
            {
                if (aArrowDegrees == 0)
                    ArrowDegrees = DEFAULT_ARROW_DEGREES;
                return aArrowDegrees;
            }
            set
            {
                if (aArrowDegrees != value)
                {
                    aArrowDegrees = value;
                    OnGageChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the lightest color of the circle.
        /// </summary>
        /// <value>The lightest color of the circle.</value>
        [TypeConverter("System.Drawing.ColorConverter"),
         Category("LoadingCircle"),
         Description("Sets the starting color of a segment")]
        public Color ColorStart
        {
            get
            {
                return colorStart;
            }
            set
            {
                if (colorStart != value)
                {
                    colorStart = value;
                    OnGageChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the lightest color of the circle.
        /// </summary>
        /// <value>The lightest color of the circle.</value>
        [TypeConverter("System.Drawing.ColorConverter"),
         Category("LoadingCircle"),
         Description("Sets the ending color of a segment")]
        public Color ColorEnd
        {
            get
            {
                return colorEnd;
            }
            set
            {
                if (colorEnd != value)
                {
                    colorEnd = value;
                    OnGageChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the colored area.
        /// </summary>
        /// <value>added to radius.</value>
        [System.ComponentModel.Description("Thickness of the colored area."),
         System.ComponentModel.Category("LoadingCircle")]
        public int Thickness
        {
            get
            {
                if (aThickness == 0)
                    Thickness = DEFAULT_THICKNESS;

                return aThickness;
            }
            set
            {
                if (aThickness != value)
                {
                    aThickness = value;
                    OnGageChanged();
                    Invalidate();
                }
            }
        }

        [System.ComponentModel.Description("Gets or sets the number of segments."),
        System.ComponentModel.Category("LoadingCircle")]
        public int NumberSegments
        {
            get
            {
                if (aNumberSegments == 0)
                    NumberSegments = DEFAULT_NUMBER_SEGMENTS;

                return aNumberSegments;
            }
            set
            {
                if (aNumberSegments != value && value > 0)
                {
                    aNumberSegments = value;
                    OnGageChanged();
                    Invalidate();
                }
            }
        }


        protected void SetTimerInterval()
        {
            try
            {
                if (value == 0)
                {
                    if (refreshTimer.IsRunning)
                        refreshTimer.Stop();
                    return;
                }

                double invpct = 1.0d - (Value/MaximumValue);

                if (invpct >= 1.0d)
                    refreshTimer.Stop();
                else
                {
                    int period =
                        (int)
                        Math.Floor((SlowAnimationDelayMillis - FastAnimationDelayMillis)*invpct +
                                   FastAnimationDelayMillis);
                    if (period < Timer.Capabilities.periodMin)
                        period = Timer.Capabilities.periodMin;
                    else if (period > Timer.Capabilities.periodMax)
                        period = Timer.Capabilities.periodMax;

                    refreshTimer.Period = period;

                    if (!refreshTimer.IsRunning)
                        refreshTimer.Start();
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while configuring the timer.", e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (refreshTimer != null)
                {
                    refreshTimer.Dispose();
                }
                if (brush != null)
                {
                    brush.Dispose();
                }
                if (graphicsPath != null)
                {
                    graphicsPath.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public int GetRadius()
        {
            return (Math.Min(ClientSize.Width, ClientSize.Height) - (Thickness * 2)) / 2;
        }

        void OnGageChanged()
        {
            GetControlCenterPoint();

            int gradientAngle = Rotation == Direction.Clockwise ? 315 : 90;

            sweepAngle = 360 / NumberSegments;

            int radius = GetRadius();

            float outerRadius = radius + Thickness;

            PointF location = new PointF(aCenterPoint.X - outerRadius, aCenterPoint.Y - outerRadius);
            SizeF size = new SizeF(outerRadius * 2, outerRadius * 2);

            gageBounds = new RectangleF(location, size);

            if (graphicsPath != null)
            {
                graphicsPath.Dispose();
                graphicsPath = null;
            }

            graphicsPath = CreateGrahpicsPath(aCenterPoint, 0, sweepAngle, radius, Thickness);
            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }
            if (graphicsPath.PointCount > 0)
                brush = new LinearGradientBrush(graphicsPath.GetBounds(), ColorStart, ColorEnd, gradientAngle);
        }


        // Events ============================================================
        /// <summary>
        /// Handles the Resize event of the LoadingCircle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void LoadingCircle_Resize(object sender, EventArgs e)
        {
            OnGageChanged();
        }

        protected GraphicsPath CreateGrahpicsPath(PointF center, float startAngle, float sweepAngle, int radius, int thickness)
        {
            GraphicsPath path = new GraphicsPath();
            path.FillMode = FillMode.Alternate;

            if (radius < 1)
                return path;

            path.StartFigure();

            // outsize radius is radius plus the thickness
            int outerRadius = radius + thickness;

            // mid radius is a little past halfway between the inner and outer radius
            int midRadius = (int)Math.Floor(radius + (thickness * 0.50f));

            path.AddLine(GetCoordinate(center, midRadius, startAngle + ArrowDegrees),
                         GetCoordinate(center, outerRadius, startAngle));

            sweepAngle -= ArrowDegrees;

            float x = center.X - outerRadius;
            float y = center.Y - outerRadius;
            RectangleF obounds = new RectangleF(x, y, outerRadius * 2, outerRadius * 2);
            path.AddArc(obounds, startAngle, sweepAngle);

            midRadius = (int)Math.Ceiling(radius + (thickness * 0.55f));
            path.AddLine(path.GetLastPoint(),
                         GetCoordinate(center, midRadius, startAngle + sweepAngle + ArrowDegrees));

            x = center.X - radius;
            y = center.Y - radius;
            RectangleF ibounds = new RectangleF(x, y, radius * 2, radius * 2);
            path.AddArc(ibounds, startAngle + sweepAngle, sweepAngle * -1);

            path.CloseFigure();

            return path;
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            try
            {
                if (graphicsPath != null && brush != null)
                {
                    Graphics graphics = args.Graphics;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    for (int i = 0; i < aNumberSegments; i++)
                    {
                        Matrix matrix = new Matrix();
                        matrix.RotateAt(i * sweepAngle + Origin, aCenterPoint);
                        args.Graphics.Transform = matrix;
                        args.Graphics.FillPath(brush, graphicsPath);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private void GetControlCenterPoint()
        {
            aCenterPoint = GetControlCenterPoint(this);
        }

        private PointF GetControlCenterPoint(Control _objControl)
        {
            return new PointF(_objControl.Width / 2, _objControl.Height / 2);
        }

        private PointF GetCoordinate(PointF _objCircleCenter, int _intRadius, double _dblAngle)
        {
            double dblAngle = Math.PI * _dblAngle / NUMBER_OF_DEGREES_HALF_CIRCLE;

            return new PointF(_objCircleCenter.X + _intRadius * (float)Math.Cos(dblAngle),
                              _objCircleCenter.Y + _intRadius * (float)Math.Sin(dblAngle));
        }

    }
}