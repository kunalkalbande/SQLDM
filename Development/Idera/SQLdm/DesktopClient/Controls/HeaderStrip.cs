using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public class HeaderStrip : ToolStrip
    {
        private static readonly CustomToolstripColorTable _ColorTable = new CustomToolstripColorTable();

        private Image headerImage = null;
        private HeaderStripStyle style = HeaderStripStyle.Large;
        private HeaderStripState state = HeaderStripState.Normal;
        private ToolStripProfessionalRenderer renderer = null;
        private bool mouseHover = false;
        private bool hotTrackEnabled = true;

        public HeaderStrip()
        {
            Dock = DockStyle.Top;
            GripStyle = ToolStripGripStyle.Hidden;
            AutoSize = false;
            ConfigureRenderer();
            SystemEvents.UserPreferenceChanged +=
                new UserPreferenceChangedEventHandler(HeaderStrip_UserPreferenceChanged);
            UpdateHeaderStyle();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SystemEvents.UserPreferenceChanged -= HeaderStrip_UserPreferenceChanged;
            }

            base.Dispose(disposing);
        }

        [Category("Appearance")]
        [DefaultValue(null)]
        public Image HeaderImage
        {
            get { return headerImage; }
            set
            {
                headerImage = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(HeaderStripStyle.Large)]
        public HeaderStripStyle Style
        {
            get { return style; }
            set
            {
                if (style != value)
                {
                    style = value;
                    UpdateHeaderStyle();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(HeaderStripState.Normal)]
        public HeaderStripState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool HotTrackEnabled
        {
            get { return hotTrackEnabled; }
            set
            {
                if (hotTrackEnabled != value)
                {
                    hotTrackEnabled = value;
                    Invalidate();
                }
            }
        }

        private void HeaderStrip_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            UpdateHeaderStyle();
        }

        protected override void OnRendererChanged(EventArgs e)
        {
            base.OnRendererChanged(e);
            ConfigureRenderer();
        }

        private void UpdateHeaderStyle()
        {
            Font font = SystemFonts.MenuFont;

            switch (style)
            {
                case HeaderStripStyle.Large:
                    Font = new Font("Arial", font.SizeInPoints + 3.75F, FontStyle.Bold);
                    ForeColor = Color.FromArgb(75, 75, 75);
                    Height = 25;
                    break;
                case HeaderStripStyle.Small:
                case HeaderStripStyle.SmallSingle:
                    Font = font;
                    ForeColor = Color.FromArgb(75, 75, 75);
                    Height = 19;
                    break;
                case HeaderStripStyle.Properties:
                    Font = new Font("Arial", font.SizeInPoints, FontStyle.Bold);
                    ForeColor = Color.FromArgb(75, 75, 75);
                    Height = 25;
                    break;
            }
        }

        private void ConfigureRenderer()
        {
            if ((Renderer is ToolStripProfessionalRenderer) && (Renderer != renderer))
            {
                if (renderer == null)
                {
                    renderer = new ToolStripProfessionalRenderer(_ColorTable);
                    renderer.RoundedEdges = false;
                    renderer.RenderToolStripBackground += new ToolStripRenderEventHandler(renderer_RenderToolStripBackground);
                    renderer.RenderToolStripBorder += new ToolStripRenderEventHandler(renderer_RenderToolStripBorder);
                    renderer.RenderDropDownButtonBackground += new ToolStripItemRenderEventHandler(renderer_RenderDropDownButtonBackground);
                    renderer.RenderSplitButtonBackground += new ToolStripItemRenderEventHandler(renderer_RenderDropDownButtonBackground);
                    renderer.RenderButtonBackground += new ToolStripItemRenderEventHandler(renderer_RenderDropDownButtonBackground);
                }

                Renderer = renderer;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseHover = true;
            Invalidate();

            if (style == HeaderStripStyle.Small && HotTrackEnabled)
            {
                Cursor = Cursors.Hand;
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseHover = false;
            Invalidate();
            Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }

        private void renderer_RenderToolStripBackground(object sender, ToolStripRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(Point.Empty, e.ToolStrip.Size);
            if (e.ToolStrip.IsDropDown)
            {
                // allow drowdowns to render normally
                return;
            }
            
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                switch (style)
                {
                    case HeaderStripStyle.Large:
                        DrawLargeHeaderStripBackground(e.Graphics, bounds);
                        break;
                    case HeaderStripStyle.Small:
                        DrawSmallHeaderStripBackground(e.Graphics, bounds);
                        break;
                    case HeaderStripStyle.SmallSingle:
                        DrawSmallSingleHeaderStripBackground(e.Graphics, bounds);
                        break;
                    case HeaderStripStyle.Properties:
                        DrawPropertiesHeaderStripBackground(e.Graphics, bounds);
                        break;
                }
            }
        }

        void renderer_RenderDropDownButtonBackground(object sender, ToolStripItemRenderEventArgs e)
        {
            switch (style)
            {
                case HeaderStripStyle.Small:
                    DrawDropDownButtonBackground(e.Graphics, e.Item);
                    break;
                case HeaderStripStyle.Large:
                    if (e.Item.Pressed || e.Item.Selected)
                    {
                        Color backColor1;
                        Color backColor2;
                        if (state == HeaderStripState.Pressed)
                        {
                            backColor1 = Color.FromArgb(250, 250, 250);
                            backColor2 = Color.FromArgb(195, 195, 195);
                        }
                        else
                        {
                            backColor2 = Color.FromArgb(236, 236, 236);
                            backColor1 = Color.FromArgb(196, 196, 196);
                        }
                        Rectangle r = new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1);
                        using (
                            Brush b = new LinearGradientBrush(r, backColor1, backColor2,
                                                              LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(b, r);
                        }
                    }
                    break;
            }
        }

        void DrawDropDownButtonBackground(Graphics g, ToolStripItem item)
        {
            Color backColor1 = Color.FromArgb(83, 83, 83);
            Rectangle r = new Rectangle(0, 0, item.Size.Width - 10, item.Size.Height - 1);
            if (item.Selected)
            {
                using (Brush b = new SolidBrush(backColor1))
                {
                    g.FillRectangle(b, r);
                }
            }
            else
                if (item.Pressed)
                {
                    using (Brush b = new LinearGradientBrush(r, Color.Silver, backColor1, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(b, r);
                    }
                }
        }

        private void renderer_RenderToolStripBorder(object sender, ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip.IsDropDown)
            {
                // allow drowdowns to render normally
                return;
            }
            
            Rectangle bounds = new Rectangle(0, 0, e.ToolStrip.Size.Width - 1, e.ToolStrip.Size.Height - 1);
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                switch (style)
                {
                    case HeaderStripStyle.Large:
                        DrawLargeHeaderStripBorder(e.Graphics, bounds);
                        break;
                    case HeaderStripStyle.Small:
                    case HeaderStripStyle.SmallSingle:
                        DrawSmallHeaderStripBorder(e.Graphics, bounds);
                        break;
                    case HeaderStripStyle.Properties:
                        DrawPropertiesHeaderStripBorder(e.Graphics, bounds);
                        break;
                }
            }
        }

        private void DrawLargeHeaderStripBackground(Graphics graphics, Rectangle bounds)
        {
            Color backColor1, backColor2;

            if (state == HeaderStripState.Pressed)
            {
                backColor1 = Color.FromArgb(250, 250, 250);
                backColor2 = Color.FromArgb(195, 195, 195);
            }
            else
            {
                backColor1 = Color.FromArgb(236, 236, 236);
                backColor2 = Color.FromArgb(196, 196, 196);
            }

            using (Brush b = new LinearGradientBrush(bounds, backColor1, backColor2, LinearGradientMode.Vertical))
            {
                graphics.FillRectangle(b, bounds);
            }

            if (headerImage != null)
            {
                graphics.DrawImage(headerImage, 5, 5, 16, 16);
                Padding = new Padding(20, 2, 0, 0);
            }
            else
            {
                Padding = new Padding(0, 2, 0, 0);
            }
        }

        private void DrawLargeHeaderStripBorder(Graphics graphics, Rectangle bounds)
        {
            using (Pen pen = new Pen(Color.FromArgb(160,160,160)))
            {
                graphics.DrawRectangle(pen, bounds);
            }
        }

        private void DrawSmallHeaderStripBackground(Graphics graphics, Rectangle bounds)
        {
            if (mouseHover && hotTrackEnabled)
            {
                Color backColor1 = Color.FromArgb(136, 137, 142);
                Color backColor2 = Color.White;

                int halfWay = Convert.ToInt32(bounds.Width / 2);

                using (Brush brush = new SolidBrush(backColor1))
                {
                    graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width - halfWay, bounds.Height);
                }

                using (Brush brush = new SolidBrush(backColor1))
                {
                    graphics.FillRectangle(brush, bounds.X + halfWay, bounds.Y, bounds.Width - halfWay, bounds.Height);
                }
            }
            else
            {
                Color backColor1 = Color.FromArgb(136, 137, 142);
                Color backColor2 = Color.Silver;
                using (Brush brush = new SolidBrush(backColor1))
                {
                    graphics.FillRectangle(brush, bounds);
                }

//                using (SolidBrush brush = new SolidBrush(Color.FromArgb(231, 231, 231)))
//                {
//                    graphics.FillRectangle(brush, bounds);
//                }
            }
        }

        private void DrawSmallSingleHeaderStripBackground(Graphics graphics, Rectangle bounds)
        {
            if (mouseHover && hotTrackEnabled)
            {
                Color backColor1 = Color.FromArgb(235, 235, 235);
                Color backColor2 = Color.White;

                int halfWay = Convert.ToInt32(bounds.Width / 2);

                using (Brush brush = new LinearGradientBrush(bounds, backColor1, backColor2, LinearGradientMode.Horizontal))
                {
                    graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width - halfWay, bounds.Height);
                }

                using (Brush brush = new LinearGradientBrush(bounds, backColor2, backColor1, LinearGradientMode.Horizontal))
                {
                    graphics.FillRectangle(brush, bounds.X + halfWay, bounds.Y, bounds.Width - halfWay, bounds.Height);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(231, 231, 231)))
                {
                    graphics.FillRectangle(brush, bounds);
                }
            }
        }

        private void DrawSmallHeaderStripBorder(Graphics graphics, Rectangle bounds)
        {
            //using (Pen pen = new Pen(Color.White))
            //{
            //    graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y);
            //}

            //using (Pen pen = new Pen(Color.White))
            //{
            //    graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height);
            //}

            //using (Pen pen = new Pen(Color.FromArgb(173, 209, 255)))
            //{
            //    graphics.DrawLine(pen, bounds.X, bounds.Y + bounds.Height, bounds.X + bounds.Width, bounds.Y + bounds.Height);
            //}

            using (Pen pen = new Pen(Color.FromArgb(203, 203, 203)))
            {
                graphics.DrawRectangle(pen, bounds);
            }
        }

        private void DrawPropertiesHeaderStripBackground(Graphics graphics, Rectangle bounds)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(227, 227, 227)))
            {
                graphics.FillRectangle(brush, bounds);
            }

            using (Pen pen = new Pen(Color.FromArgb(197, 197, 197)))
            {
                graphics.DrawLine(pen, bounds.X, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width,
                                    bounds.Y + bounds.Height - 2);
            }
        }

        private void DrawPropertiesHeaderStripBorder(Graphics graphics, Rectangle bounds)
        {

        }
    }

    public enum HeaderStripStyle
    {
        Large,
        Small,
        Properties,
        SmallSingle
    }

    public enum HeaderStripState
    {
        Normal,
        Pressed
    }
}
