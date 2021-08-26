using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public enum PulseHeaderPanelColorPalette
    {
        Blue,
        Yellow,
        Red
    }

    public partial class PulseHeaderPanel : Panel
    {
        private Color borderColor;
        private Color fillColor1;
        private Color fillColor2;

        public PulseHeaderPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
            ResetColors();
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        public Color FillColor1
        {
            get { return fillColor1; }
            set
            {
                fillColor1 = value;
                Invalidate();
            }
        }

        public Color FillColor2
        {
            get { return fillColor2; }
            set
            {
                fillColor2 = value;
                Invalidate();
            }
        }

        public void ResetColors()
        {
            SetColorPalette(PulseHeaderPanelColorPalette.Blue);
        }

        public void SetColorPalette(PulseHeaderPanelColorPalette palette)
        {
            switch(palette)
            {
                case PulseHeaderPanelColorPalette.Blue:
                    borderColor = Color.FromArgb(93, 140, 201);
                    fillColor1 = Color.FromArgb(252, 253, 254);
                    fillColor2 = Color.FromArgb(194, 212, 235);
                    break;
                case PulseHeaderPanelColorPalette.Yellow:
                    borderColor = Color.FromArgb(180, 128, 2);
                    fillColor1 = Color.FromArgb(255, 233, 168);
                    fillColor2 = Color.FromArgb(255, 203, 81);
                    break;
                case PulseHeaderPanelColorPalette.Red:
                    borderColor = Color.FromArgb(167, 29, 35);
                    fillColor1 = Color.FromArgb(242, 177, 178);
                    fillColor2 = Color.FromArgb(228, 103, 110);
                    break;
            }
            
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            float borderArcRadius = 3;

            Rectangle borderBounds = Rectangle.Inflate(ClientRectangle, -1, -1);
            DrawRoundRectangle(e.Graphics, borderBounds, borderArcRadius);

            base.OnPaint(e);
        }

        private void DrawRoundRectangle(Graphics graphics, RectangleF bounds, float radius)
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

                    if (fillColor1 != Color.Empty)
                    {
                        if (fillColor2 == Color.Empty)
                        {
                            using (SolidBrush fillBrush = new SolidBrush(fillColor1))
                            {
                                graphics.FillPath(fillBrush, gp);
                            }
                        }
                        else
                        {
                            using (LinearGradientBrush fillBrush = new LinearGradientBrush(bounds, fillColor1,
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