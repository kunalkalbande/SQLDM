using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public enum InfiniteProgressFlowBarDirection
    {
        Right,
        Left
    }

    public partial class InfiniteProgressFlowBar : UserControl
    {
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(InfiniteProgressFlowBar));

        private const int SegmentWidthDefault = 40;
        private const int MaximumValueDefault = 100;
        private const int ValueDefault = 0;
        private const int SlowestSpeedDefault = 50;
        private const int FastestSpeedDefault = 3;

        private double maximumValue = MaximumValueDefault;
        private double value = ValueDefault;
        private int timerSlowSpeed = SlowestSpeedDefault;
        private int timerFastSpeed = FastestSpeedDefault;
        
        private int segmentWidth = SegmentWidthDefault;
        private InfiniteProgressFlowBarDirection flowDirection = InfiniteProgressFlowBarDirection.Right;
        private Color color1 = Color.FromArgb(156, 183, 228);
        private Color color2 = Color.Transparent;
        private int drawStartPosition = 0;
        private GraphicsPath segmentPath;
        private RectangleF segmentBounds;
        private LinearGradientBrush gradientBrush;
        private Timer refreshTimer;

        public InfiniteProgressFlowBar()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.CacheText,
                true);

            InitializeComponent();

            segmentBounds = new Rectangle(0, 0, SegmentWidth - 1, Height);
            ConfigureSegmentObjects();

            refreshTimer = new Timer();
            refreshTimer.SynchronizingObject = this;
            refreshTimer.Mode = TimerMode.Periodic;
            refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
            refreshTimer.Resolution = 10;
        }

        [Category("Appearance")]
        public Color Color1
        {
            get { return color1; }
            set
            {
                color1 = value;
                ConfigureSegmentObjects();
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color Color2
        {
            get { return color2; }
            set
            {
                color2 = value;
                ConfigureSegmentObjects();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(SegmentWidthDefault)]
        public int SegmentWidth
        {
            get { return segmentWidth; }
            set
            {
                segmentWidth = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public InfiniteProgressFlowBarDirection FlowDirection
        {
            get { return flowDirection; }
            set
            {
                if (flowDirection != value)
                {
                    flowDirection = value;
                    ConfigureSegmentObjects();
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(MaximumValueDefault)]
        public double MaximumValue
        {
            get { return maximumValue; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("MaximumValue", "The MaximumValue cannot be less than zero");
                }

                maximumValue = value;
                AdjustTimer();
            }
        }

        [Category("Behavior")]
        [DefaultValue(ValueDefault)]
        public double Value
        {
            get { return value; }
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
                //AdjustTimer();
            }
        }

        [Category("Behavior")]
        [DefaultValue(SlowestSpeedDefault)]
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

                AdjustTimer();
            }
        }

        [Category("Behavior")]
        [DefaultValue(FastestSpeedDefault)]
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

                AdjustTimer();
            }
        }

        private void AdjustTimer()
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


        private void ConfigureSegmentObjects()
        {
            if (segmentPath != null)
            {
                segmentPath.Dispose();
            }

            if (gradientBrush != null)
            {
                gradientBrush.Dispose();
            }

            segmentPath = new GraphicsPath();
            float halfHeight = Height/2;
            int arrowInset = 10;

            switch (flowDirection)
            {
                case InfiniteProgressFlowBarDirection.Right:
                    gradientBrush =
                        new LinearGradientBrush(segmentBounds, color2, color1, LinearGradientMode.Horizontal);
                    segmentPath.AddLine(segmentBounds.X, segmentBounds.Y,
                                        segmentBounds.X + segmentBounds.Width - arrowInset, segmentBounds.Y);
                    segmentPath.AddLine(segmentBounds.X + segmentBounds.Width - arrowInset, segmentBounds.Y,
                                        segmentBounds.X + segmentBounds.Width, segmentBounds.Y + halfHeight);
                    segmentPath.AddLine(segmentBounds.X + segmentBounds.Width, segmentBounds.Y + halfHeight,
                                        segmentBounds.X + segmentBounds.Width - arrowInset,
                                        segmentBounds.Y + segmentBounds.Height);
                    segmentPath.AddLine(segmentBounds.X + segmentBounds.Width - arrowInset,
                                        segmentBounds.Y + segmentBounds.Height, segmentBounds.X,
                                        segmentBounds.Y + segmentBounds.Height);
                    segmentPath.AddLine(segmentBounds.X, segmentBounds.Y + segmentBounds.Height,
                                        segmentBounds.X + arrowInset, segmentBounds.Y + halfHeight);
                    segmentPath.CloseFigure();
                    break;
                case InfiniteProgressFlowBarDirection.Left:
                    gradientBrush =
                        new LinearGradientBrush(segmentBounds, color1, color2, LinearGradientMode.Horizontal);
                    segmentPath.AddLine(segmentBounds.X, segmentBounds.Y + halfHeight, segmentBounds.X + arrowInset,
                                        segmentBounds.Y);
                    segmentPath.AddLine(segmentBounds.X + arrowInset, segmentBounds.Y,
                                        segmentBounds.X + segmentBounds.Width, segmentBounds.Y);
                    segmentPath.AddLine(segmentBounds.X + segmentBounds.Width, segmentBounds.Y,
                                        segmentBounds.X + segmentBounds.Width - arrowInset, segmentBounds.Y + halfHeight);
                    segmentPath.AddLine(segmentBounds.X + segmentBounds.Width - arrowInset, segmentBounds.Y + halfHeight,
                                        segmentBounds.X + segmentBounds.Width, segmentBounds.Y + segmentBounds.Height);
                    segmentPath.AddLine(segmentBounds.X + segmentBounds.Width, segmentBounds.Y + segmentBounds.Height,
                                        segmentBounds.X + arrowInset, segmentBounds.Y + segmentBounds.Height);
                    segmentPath.CloseFigure();
                    break;
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
                if (segmentPath != null)
                {
                    segmentPath.Dispose();
                }

                if (gradientBrush != null)
                {
                    gradientBrush.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            switch (flowDirection)
            {
                case InfiniteProgressFlowBarDirection.Right:
                    DrawFlowRight(e.Graphics);
                    break;
                case InfiniteProgressFlowBarDirection.Left:
                    DrawFlowLeft(e.Graphics);
                    break;
            }

            base.OnPaint(e);
        }

        private void DrawFlowRight(Graphics graphics)
        {
            if (graphics != null &&
                gradientBrush != null &&
                segmentPath != null &&
                Width > 0 && Height > 0)
            {
                int segmentDrawPosition = drawStartPosition;

                while (segmentDrawPosition < Width)
                {
                    graphics.TranslateTransform(segmentDrawPosition, 0);
                    graphics.FillPath(gradientBrush, segmentPath);
                    graphics.ResetTransform();
                    segmentDrawPosition += SegmentWidth;
                }
            }
        }

        private void DrawFlowLeft(Graphics graphics)
        {
            if (graphics != null &&
                gradientBrush != null &&
                segmentPath != null &&
                Width > 0 && Height > 0)
            {
                int segmentDrawPosition = drawStartPosition;

                while (segmentDrawPosition < Width)
                {
                    graphics.TranslateTransform(segmentDrawPosition, 0);
                    graphics.FillPath(gradientBrush, segmentPath);
                    graphics.ResetTransform();
                    segmentDrawPosition += SegmentWidth;
                }
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            switch (flowDirection)
            {
                case InfiniteProgressFlowBarDirection.Right:
                    drawStartPosition = drawStartPosition < 0 ? ++drawStartPosition : -SegmentWidth;
                    break;
                case InfiniteProgressFlowBarDirection.Left:
                    drawStartPosition = drawStartPosition < SegmentWidth ? --drawStartPosition : 0;
                    break;
            }

            Invalidate();
        }
    }
}