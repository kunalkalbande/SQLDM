using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [ToolboxItem(true)]
    public partial class ReportSelectionButton : UserControl
    {
        private ReportButtonState state = ReportButtonState.Normal;

        private enum ReportButtonState
        {
            Normal,
            MouseOver,
            Clicked
        }

        public Color rsbBackColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
        public Color rsbForeColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ForeColor) : System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
        public Color rsbDarkHoverColor = ColorTranslator.FromHtml(DarkThemeColorConstants.SelectionColor);

        public ReportSelectionButton()
        {
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            rsbBackColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
            rsbForeColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ForeColor) : System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.BackColor = rsbBackColor;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
            this.DescriptionColor = rsbForeColor;
            this.TitleColor = rsbForeColor;
        }
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            rsbBackColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
            rsbForeColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ForeColor) : System.Drawing.Color.FromArgb(((int)(((byte)(00)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));

            this.BackColor = rsbBackColor;
            try
            {
                this.reportDescriptionLabel.BackColor = Color.Transparent;
                this.reportTitleLabel.BackColor = Color.Transparent;
            }
            catch(Exception exception) { }
            this.reportDescriptionLabel.ForeColor = rsbForeColor;
            this.reportTitleLabel.ForeColor = rsbForeColor;
        }

        [Category("Appearance")]
        public string Title
        {
            get { return reportTitleLabel.Text; }
            set { reportTitleLabel.Text = value; }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0x535353")]
        public Color TitleColor
        {
            get { return reportTitleLabel.ForeColor; }
            set { reportTitleLabel.ForeColor = value; }
        }
        [Category("Appearance")]
        public Font SetTitleFont
        {
            set
            {
                reportTitleLabel.Font = value;
            }
        }
        [Category("Appearance")]
        public Font SetDescriptionFont
        {
            set { reportDescriptionLabel.Font = value; }
        }
        [Category("Appearance")]
        public string Description
        {
            get { return reportDescriptionLabel.Text; }
            set { reportDescriptionLabel.Text = value; }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0x535353")]
        public Color DescriptionColor
        {
            get { return reportDescriptionLabel.ForeColor; }
            set { reportDescriptionLabel.ForeColor = value; }
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            foreach (Control control in Controls)
            {
                control.Enabled = Enabled;
            }

            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            state = ReportButtonState.MouseOver;
            Invalidate(false);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            state = ReportButtonState.Clicked;
            Invalidate(false);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            state = ReportButtonState.Normal;
            Invalidate(false);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (state != ReportButtonState.MouseOver)
            {
                state = ReportButtonState.MouseOver;
                Invalidate(false);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            state = ReportButtonState.Normal;
            Invalidate(false);
            base.OnMouseLeave(e);
        }

        private void ChildControl_MouseEnter(object sender, EventArgs e)
        {
            state = ReportButtonState.MouseOver;
            Invalidate(false);
        }

        private void ChildControl_MouseDown(object sender, MouseEventArgs e)
        {
            state = ReportButtonState.Clicked;
            Invalidate(false);
        }

        private void ChildControl_MouseUp(object sender, MouseEventArgs e)
        {
            state = ReportButtonState.Normal;
            Invalidate(false);
        }

        private void ChildControl_MouseLeave(object sender, EventArgs e)
        {
            state = ReportButtonState.Normal;
            Invalidate(false);
        }

        private void ChildControl_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void ChildControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (state != ReportButtonState.MouseOver)
            {
                state = ReportButtonState.MouseOver;
                Invalidate(false);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            switch (state)
            {
                case ReportButtonState.MouseOver:
                    DrawMouseOver(e.Graphics);
                    break;
                case ReportButtonState.Clicked:
                    DrawClicked(e.Graphics);
                    break;
                case ReportButtonState.Normal:
                    this.BackColor = rsbBackColor;
                    this.DescriptionColor = rsbForeColor;
                    this.TitleColor = rsbForeColor;
                    break;
            }
        }

        private void DrawMouseOver(Graphics graphics)
        {
            if (graphics != null)
            {
                //using (Pen pen = new Pen(Color.White))
                using (Pen pen = new Pen(Settings.Default.ColorScheme == "Dark" ? rsbBackColor : Color.White))
                {
                    Rectangle bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
                    
                    //DrawRoundRectangle(graphics, pen, Color.FromArgb(238, 239, 244), Color.FromArgb(228, 229, 234),
                    //                   bounds, 5);
                    DrawRoundRectangle(graphics, pen, Settings.Default.ColorScheme == "Dark" ? rsbDarkHoverColor : Color.FromArgb(238, 239, 244),
                        Settings.Default.ColorScheme == "Dark" ? rsbDarkHoverColor : Color.FromArgb(228, 229, 234), bounds, 5);

                }
            }
        }

        private void DrawClicked(Graphics graphics)
        {
            if (graphics != null)
            {
                using (Pen pen = new Pen(Settings.Default.ColorScheme == "Dark" ? rsbBackColor : Color.FromArgb(220, 220, 220)))
                {
                    Rectangle bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
                    //DrawRoundRectangle(graphics, pen, Color.FromArgb(243, 243, 243), Color.FromArgb(226, 226, 226),
                    //                   bounds, 5);

                    DrawRoundRectangle(graphics, pen, Settings.Default.ColorScheme == "Dark" ? rsbDarkHoverColor : Color.FromArgb(243, 243, 243),
                        Settings.Default.ColorScheme == "Dark" ? rsbDarkHoverColor : Color.FromArgb(226, 226, 226), bounds, 5);
                }
            }
        }

        private void DrawRoundRectangle(Graphics graphics, Pen pen, Color fillColor1, Color fillColor2,
                                        RectangleF bounds, float radius)
        {
            if (graphics != null)
            {
                //fillColor1 = Color.Red;
                //fillColor2 = Color.Red;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddLine(radius, 0, bounds.Width - (radius*2), 0);
                    gp.AddArc(bounds.Width - (radius*2), 0, radius*2, radius*2, 270, 90);
                    gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height - (radius*2));
                    gp.AddArc(bounds.Width - (radius*2), bounds.Height - (radius*2), radius*2, radius*2, 0, 90);
                    gp.AddLine(bounds.Width - (radius*2), bounds.Height, radius, bounds.Height);
                    gp.AddArc(0, bounds.Height - (radius*2), radius*2, radius*2, 90, 90);
                    gp.AddLine(0, bounds.Height - (radius*2), 0, radius);
                    gp.AddArc(0, 0, radius*2, radius*2, 180, 90);
                    gp.CloseFigure();

                    using (LinearGradientBrush fillBrush = new LinearGradientBrush(ClientRectangle, fillColor1,
                                                                                   fillColor2,
                                                                                   LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(fillBrush, gp);
                    }

                    graphics.DrawPath(pen, gp);
                }
            }
        }
    }
}