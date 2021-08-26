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
    public partial class FeatureButton : UserControl
    {
        private FeatureButtonState state = FeatureButtonState.Normal;
        private Color backColorHoverDark = ColorTranslator.FromHtml(DarkThemeColorConstants.SelectionColor);

        private enum FeatureButtonState
        {
            Normal,
            MouseOver,
            Clicked
        }

        public FeatureButton()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            InitializeComponent();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        [Category("Appearance")]
        public string HeaderText
        {
            get { return featureHeaderLabel.Text; }
            set { featureHeaderLabel.Text = value; }
        }

        [Category("Appearance")]
        public Color HeaderColor
        {
            get { return featureHeaderLabel.ForeColor; }
            set { featureHeaderLabel.ForeColor = value; }
        }

        [Category("Appearance")]
        public string DescriptionText
        {
            get { return featureDescriptionLabel.Text; }
            set { featureDescriptionLabel.Text = value; }
        }

        [Category("Appearance")]
        public Image Image
        {
            get { return featureImagePictureBox.Image; }
            set { featureImagePictureBox.Image = value; }
        }

        [Category("Appearance")]
        public Font HeaderFont
        {
            get { return featureHeaderLabel.Font; }
            set { featureHeaderLabel.Font = value; }
        }

        [Category("Appearance")]
        public Font DescriptionFont
        {
            get { return featureDescriptionLabel.Font; }
            set { featureDescriptionLabel.Font = value; }
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
            state = FeatureButtonState.MouseOver;
            Invalidate(false);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            state = FeatureButtonState.Clicked;
            Invalidate(false);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            state = FeatureButtonState.Normal;
            Invalidate(false);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (state != FeatureButtonState.MouseOver)
            {
                state = FeatureButtonState.MouseOver;
                Invalidate(false);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            state = FeatureButtonState.Normal;
            Invalidate(false);
            base.OnMouseLeave(e);
        }

        private void ChildControl_MouseEnter(object sender, EventArgs e)
        {
            state = FeatureButtonState.MouseOver;
            Invalidate(false);
        }

        private void ChildControl_MouseDown(object sender, MouseEventArgs e)
        {
            state = FeatureButtonState.Clicked;
            Invalidate(false);
        }

        private void ChildControl_MouseUp(object sender, MouseEventArgs e)
        {
            state = FeatureButtonState.Normal;
            Invalidate(false);
        }

        private void ChildControl_MouseLeave(object sender, EventArgs e)
        {
            state = FeatureButtonState.Normal;
            Invalidate(false);
        }

        private void ChildControl_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void ChildControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (state != FeatureButtonState.MouseOver)
            {
                 state = FeatureButtonState.MouseOver;
                 Invalidate(false);
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            state = FeatureButtonState.Normal;
            Invalidate(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            switch (state)
            {
                case FeatureButtonState.MouseOver:
                    DrawMouseOver(e.Graphics);
                    break;
                case FeatureButtonState.Clicked:
                    DrawClicked(e.Graphics);
                    break;
                case FeatureButtonState.Normal:
                    DrawNormal(e.Graphics);
                    break;
            }
        }

        private void DrawNormal(Graphics graphics)
        {
            this.BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : Color.White;
            try
            {
                this.featureImagePictureBox.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void DrawMouseOver(Graphics graphics)
        {
            if (graphics != null)
            {
                using (Pen pen = new Pen(Settings.Default.ColorScheme == "Dark" ? backColorHoverDark : Color.FromArgb(255, 232, 166)))
                {
                    Rectangle bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
                    DrawRoundRectangle(graphics, pen, Settings.Default.ColorScheme == "Dark" ? backColorHoverDark : Color.FromArgb(255, 255, 255),
                        Settings.Default.ColorScheme == "Dark" ? backColorHoverDark : Color.FromArgb(255, 243, 205), bounds, 5);
                }
            }
        }

        private void DrawClicked(Graphics graphics)
        {
            if (graphics != null)
            {
                using (Pen pen = new Pen(Settings.Default.ColorScheme == "Dark" ? backColorHoverDark : Color.FromArgb(220, 220, 220)))
                {
                    Rectangle bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
                    DrawRoundRectangle(graphics, pen, Settings.Default.ColorScheme == "Dark" ? backColorHoverDark : Color.FromArgb(243, 243, 243),
                        Settings.Default.ColorScheme == "Dark" ? backColorHoverDark : Color.FromArgb(226, 226, 226), bounds, 5);
                }
            }
        }

        private void DrawRoundRectangle(Graphics graphics, Pen pen, Color fillColor1, Color fillColor2,
                                        RectangleF bounds, float radius)
        {
            if (graphics != null)
            {
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
