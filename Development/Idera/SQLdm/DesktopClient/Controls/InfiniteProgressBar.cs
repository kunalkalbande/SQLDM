using Idera.SQLdm.DesktopClient.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [ToolboxBitmap(typeof (ProgressBar))]
    public class InfiniteProgressBar : UserControl
    {
        private float pos;
        private float step;
        private Color color1;
        private Color color2;
        private System.Timers.Timer timer;

        public InfiniteProgressBar()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            if (Settings.Default.ColorScheme == "Dark")
            {
                color1 = ColorTranslator.FromHtml(DarkThemeColorConstants.ProgressBarForeColor);
                color2 = ColorTranslator.FromHtml(DarkThemeColorConstants.ProgressBarBackColor);
            }
            else
            {
                color1 = Color.White;
                color2 = Color.Orange;
            }           
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        #region Properties

        [Browsable(true), Category("Gradient")]
        public Color Color1
        {
            get { return color1; }
            set { color1 = value; }
        }

        [Browsable(true), Category("Gradient")]
        public Color Color2
        {
            get { return color2; }
            set { color2 = value; }
        }

        [Browsable(true), Category("Gradient"), DefaultValue(5)]
        public float Step
        {
            get { return step; }
            set { step = value; }
        }

        [Browsable(true), Category("Gradient"), DefaultValue(20)]
        public double Speed
        {
            set
            {
                if (timer.Enabled)
                {
                    timer.Stop();
                    timer.Interval = value;
                    timer.Start();
                }
                else
                {
                    timer.Interval = value;
                }
            }
            get { return timer.Interval; }
        }

        #endregion

        #region Event Handlers

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            pos = (pos <= Width)
                      ?
                  pos + step
                      : 0;

            Invalidate();
        }

        #endregion

        #region Methods

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            Reset();
        }

        private void Reset()
        {
            pos = 0;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer != null)
                {
                    timer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                Color darkcolor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.ProgressBarForeColor);
                Color darkcolor1 = ColorTranslator.FromHtml(DarkThemeColorConstants.ProgressBarBackColor);

                Color[] colors = new Color[] { darkcolor1, darkcolor2 };
                float l = Width / colors.Length + 1;
                float end = pos;
                int x = 0;
                for (int i = 0; i < colors.Length + 1; i++, end = (end + l) % (Width + l))
                {
                    RectangleF r = new RectangleF(end - l, 0, l, Height);
                    using (
                        Brush brush =
                            new LinearGradientBrush(r, colors[(i - x) % colors.Length], colors[(i - x + 1) % colors.Length], 0f)
                        )
                    using (Pen pen = new Pen(colors[(i - x) % colors.Length]))
                    {
                        e.Graphics.FillRectangle(brush, r);
                        e.Graphics.DrawLine(pen, r.Left, r.Top, r.Left, r.Bottom);
                        // needed due to a GDI+ bug in FillRectangle
                    }
                    if (end >= Width) x = 1;
                }
            }
            else{
                Color[] colors = new Color[] { color1, color2 };
                float l = Width / colors.Length + 1;
                float end = pos;
                int x = 0;
                for (int i = 0; i < colors.Length + 1; i++, end = (end + l) % (Width + l))
                {
                    RectangleF r = new RectangleF(end - l, 0, l, Height);
                    using (
                        Brush brush =
                            new LinearGradientBrush(r, colors[(i - x) % colors.Length], colors[(i - x + 1) % colors.Length], 0f)
                        )
                    using (Pen pen = new Pen(colors[(i - x) % colors.Length]))
                    {
                        e.Graphics.FillRectangle(brush, r);
                        e.Graphics.DrawLine(pen, r.Left, r.Top, r.Left, r.Bottom);
                        // needed due to a GDI+ bug in FillRectangle
                    }
                    if (end >= Width) x = 1;
                }

            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        #endregion
    }
}