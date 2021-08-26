using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public enum GroupPanelStyle
    {
        Dashboard,
        Space
    }

    public partial class GroupPanel : Panel
    {
        private GroupPanelStyle style = GroupPanelStyle.Space;
        private Color groupBoxBackColor = Color.White;
        private Color headerColor1 = Color.FromArgb(197, 213, 242);

        public GroupPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        [Category("Appearance")]
        public GroupPanelStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                Invalidate(false);
            }
        }

        [Category("Appearance")]
        public Color GroupBoxBackColor
        {
            get { return groupBoxBackColor; }
            set
            {
                groupBoxBackColor = value;
                Invalidate(false);
            }
        }

        public Color HeaderColor1
        {
            get { return headerColor1; }
            set
            {
                headerColor1 = value;
                Invalidate(false);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            switch (style)
            {
                case GroupPanelStyle.Dashboard:
                    {
                        Color borderColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.GroupPanelBorderColor) : Color.FromArgb(163, 175, 201);
                        float arcRadius = 15;

                        DrawRoundRectangleShadow(e.Graphics, Rectangle.Inflate(ClientRectangle, -1, -1), arcRadius);
                        DrawRoundRectangle(e.Graphics, borderColor, groupBoxBackColor, Color.Empty,
                                           Rectangle.Inflate(ClientRectangle, -3, -3), arcRadius);

                        Rectangle headerBounds = Rectangle.Inflate(ClientRectangle, -4, -4);
                        headerBounds.X = 1;
                        headerBounds.Y = 1;
                        headerBounds.Height = 25;
                        DrawHeader2(e.Graphics, headerColor1, Color.White, headerBounds, arcRadius);

                        break;
                    }
                case GroupPanelStyle.Space:
                    {
                        Color borderColor = Color.FromArgb(93, 140, 201);
                        float arcRadius = 10;

                        Rectangle borderBounds = Rectangle.Inflate(ClientRectangle, -1, -1);
                        DrawRoundRectangle(e.Graphics, borderColor, groupBoxBackColor, Color.Empty, borderBounds, arcRadius);

                        Rectangle headerBounds = Rectangle.Inflate(ClientRectangle, -1, -1);
                        headerBounds.Height = 25;
                        DrawHeader1(e.Graphics, borderColor, headerBounds, arcRadius);
                        break;
                    }
            }
            
            base.OnPaint(e);
            this.groupBoxBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.GroupPanelGroupBoxBackColor) : Color.White;
        }

        private void DrawHeader1(Graphics graphics, Color borderColor, Rectangle bounds, float radius)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                Color backColor1 = Color.FromArgb(227, 239, 255);
                Color backColor2 = Color.FromArgb(196, 221, 255);
                Color backColor3 = Color.FromArgb(173, 209, 255);
                Color backColor4 = Color.FromArgb(192, 219, 255);
                float gradientPivot = bounds.Height/3;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddLine(0, radius, 0, bounds.Height);
                    gp.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                    gp.AddLine(radius, 0, bounds.Width - radius, 0);
                    gp.AddArc(bounds.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                    gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height);
                    gp.AddLine(bounds.Width, bounds.Height, 0, bounds.Height);
                    gp.CloseFigure();

                    RectangleF topBounds = bounds;
                    topBounds.Height = gradientPivot;
                    using (LinearGradientBrush fillBrush = new LinearGradientBrush(topBounds, backColor1,
                                                                                   backColor2,
                                                                                   LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(fillBrush, gp);
                    }

                    RectangleF bottomBounds = bounds;
                    bottomBounds.Y += gradientPivot;
                    bottomBounds.Height -= gradientPivot;
                    using (LinearGradientBrush fillBrush = new LinearGradientBrush(bottomBounds, backColor3,
                                                                                   backColor4,
                                                                                   LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(fillBrush, gp);
                    }

                    using (Pen pen = new Pen(borderColor, 1))
                    {
                        graphics.DrawPath(pen, gp);
                    }
                }
            }
        }

        private void DrawHeader2(Graphics graphics, Color fillColor1, Color fillColor2, Rectangle bounds, float radius)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    if(Settings.Default.ColorScheme == "Dark")
                    {
                        fillColor1 = ColorTranslator.FromHtml(DarkThemeColorConstants.GroupPanelHeaderColor);
                        fillColor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.GroupPanelHeaderColor1);
                    }
                    gp.AddLine(bounds.X, radius, bounds.X, bounds.Y + bounds.Height);
                    gp.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
                    gp.AddLine(radius, bounds.Y, bounds.X + bounds.Width - radius, bounds.Y);
                    gp.AddArc(bounds.X + bounds.Width - (radius * 2), bounds.Y, radius * 2, radius * 2, 270, 90);
                    gp.AddLine(bounds.X + bounds.Width, radius, bounds.X + bounds.Width, bounds.Y + bounds.Height);
                    gp.AddLine(bounds.X + bounds.Width, bounds.Y + bounds.Height, bounds.X, bounds.Y + bounds.Height);
                    gp.CloseFigure();

                    using (LinearGradientBrush fillBrush = new LinearGradientBrush(bounds, fillColor1,
                                                                                   fillColor2,
                                                                                   LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(fillBrush, gp);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundRectangleGraphicsPath(RectangleF bounds, float radius)
        {
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                GraphicsPath gp = new GraphicsPath();

                gp.AddLine(radius, 0, bounds.Width - (radius * 2), 0);
                gp.AddArc(bounds.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height - (radius * 2));
                gp.AddArc(bounds.Width - (radius * 2), bounds.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                gp.AddLine(bounds.Width - (radius * 2), bounds.Height, radius, bounds.Height);
                gp.AddArc(0, bounds.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
                gp.AddLine(0, bounds.Height - (radius * 2), 0, radius);
                gp.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                gp.CloseFigure();

                return gp;
            }
            else
            {
                return null;
            }
        }

        private void DrawRoundRectangleShadow(Graphics graphics, RectangleF bounds, float radius)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                using (GraphicsPath gp = CreateRoundRectangleGraphicsPath(bounds, radius))
                {
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(gp))
                    {
                        ColorBlend colorBlend = new ColorBlend(3);
                        colorBlend.Colors = new Color[]
                            {
                                Color.Transparent,
                                Color.FromArgb(180, Color.Black),
                                Color.FromArgb(180, Color.Black)
                            };
                        colorBlend.Positions = new float[] { 0f, .1f, 1f };
                        shadowBrush.InterpolationColors = colorBlend;
                        graphics.FillPath(shadowBrush, gp);
                    }
                }
            }
        }

        private void DrawRoundRectangle(Graphics graphics, Color borderColor, Color fillColor1, Color fillColor2,
                                        RectangleF bounds, float radius)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                using (GraphicsPath gp = CreateRoundRectangleGraphicsPath(bounds, radius))
                {
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

                    if (borderColor != Color.Empty)
                    {
                        using (Pen pen = new Pen(borderColor, 1))
                        {
                            graphics.DrawPath(pen, gp);
                        }
                    }
                }
            }
        }
    }
}