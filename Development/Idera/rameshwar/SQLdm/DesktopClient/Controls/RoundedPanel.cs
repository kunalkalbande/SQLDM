using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class RoundedPanel : Panel
    {
        private Color borderColor = Color.Gray;
        private Color fillColor = Color.White;
        private Color fillColor2 = Color.Empty;
        private float radius = 3;

        public RoundedPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color FillColor2
        {
            get { return fillColor2; }
            set
            {
                fillColor2 = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle borderBounds = Rectangle.Inflate(ClientRectangle, -1, -1);
            DrawRoundRectangle(e.Graphics, borderBounds);
            base.OnPaint(e);
        }

        private void DrawRoundRectangle(Graphics graphics, RectangleF bounds)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddLine(radius, 0, bounds.Width - (radius * 2), 0);
                    gp.AddArc(bounds.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                    gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height - (radius * 2));
                    gp.AddArc(bounds.Width - (radius * 2), bounds.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                    gp.AddLine(bounds.Width - (radius * 2), bounds.Height, radius, bounds.Height);
                    gp.AddArc(0, bounds.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
                    gp.AddLine(0, bounds.Height - (radius * 2), 0, radius);
                    gp.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                    gp.CloseFigure();

                    if (fillColor != Color.Empty)
                    {
                        if (fillColor2 == Color.Empty)
                        {
                            using (SolidBrush fillBrush = new SolidBrush(fillColor))
                            {
                                graphics.FillPath(fillBrush, gp);
                            }
                        }
                        else
                        {
                            using (LinearGradientBrush fillBrush = new LinearGradientBrush(bounds, fillColor,
                                                                                           fillColor2,
                                                                                           LinearGradientMode.Vertical))
                            {
                                graphics.FillPath(fillBrush, gp);
                            }
                        }
                    }

                    using (Pen pen = new Pen(borderColor, 1))
                    {
                        graphics.DrawPath(pen, gp);
                    }
                }
            }
        }
    }
}
