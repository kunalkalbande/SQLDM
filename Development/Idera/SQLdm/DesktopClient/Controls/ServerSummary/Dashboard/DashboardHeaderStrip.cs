using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public class DashboardHeaderStrip : ToolStrip
    {
        private static readonly CustomToolstripColorTable _ColorTable = new CustomToolstripColorTable();
        private Color backColor2;
        private Color hotTrackBackColor;
        private Color borderColor;
        private Image headerImage = null;
        private HeaderStripState state = HeaderStripState.Normal;
        private ToolStripProfessionalRenderer renderer = null;
        private bool mouseHover = false;
        private bool hotTrackEnabled = true;

        public DashboardHeaderStrip()
        {
            BackColor = Color.FromArgb(136, 137, 142);
            BackColor2 = Color.Silver;
            HotTrackBackColor = Color.FromArgb(210, 210, 210);
            BorderColor = Color.White;
            ForeColor = Color.White;
            Dock = DockStyle.Top;
            GripStyle = ToolStripGripStyle.Hidden;
            AutoSize = false;
            ConfigureRenderer();
            UpdateHeaderStyle();
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0x535353")]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color BackColor2
        {
            get { return backColor2; }
            set
            {
                backColor2 = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0xD2D2D2")]
        public Color HotTrackBackColor
        {
            get { return hotTrackBackColor; }
            set
            {
                hotTrackBackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public new Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                foreach (ToolStripItem item in Items)
                {
                    item.ForeColor = ForeColor;
                }
            }
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

        protected override void OnRendererChanged(EventArgs e)
        {
            base.OnRendererChanged(e);
            ConfigureRenderer();
        }

        private void UpdateHeaderStyle()
        {
            Height = 19;
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
                    renderer.RenderToolStripBorder +=  new ToolStripRenderEventHandler(renderer_RenderToolStripBorder);
                    renderer.RenderDropDownButtonBackground += new ToolStripItemRenderEventHandler(renderer_RenderDropDownButtonBackground);
                    renderer.RenderSplitButtonBackground += new ToolStripItemRenderEventHandler(renderer_RenderDropDownButtonBackground);
                    renderer.RenderButtonBackground += new ToolStripItemRenderEventHandler(renderer_RenderDropDownButtonBackground);
                }

                Renderer = renderer;
            }
        }

        void renderer_RenderDropDownButtonBackground(object sender, ToolStripItemRenderEventArgs e)
        {
            DrawDropDownButtonBackground(e.Graphics, e.Item);
        }

        void DrawDropDownButtonBackground(Graphics g, ToolStripItem item)
        {
            Color backColor1 = BackColor;
            Rectangle r = new Rectangle(0, 0, item.Size.Width - 1, item.Size.Height - 1);
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
                using (Brush b = new SolidBrush(backColor1))
                {
                    g.FillRectangle(b, r);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            //mouseHover = true;
            //Invalidate();

            //if (HotTrackEnabled)
            //{
            //    Cursor = Cursors.Hand;
            //}

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
                DrawSmallHeaderStripBackground(e.Graphics, bounds);
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
                DrawSmallHeaderStripBorder(e.Graphics, bounds);
            }
        }


        private void DrawSmallHeaderStripBackground(Graphics graphics, Rectangle bounds)
        {
            Color backColor1;
            if (mouseHover && hotTrackEnabled)
            {
                backColor1 = HotTrackBackColor;
            }
            else
            {
                backColor1 = BackColor;
            }

            using (Brush brush = new SolidBrush(backColor1))
            {
                graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
        }

        private void DrawSmallHeaderStripBorder(Graphics graphics, Rectangle bounds)
        {
            using (Pen pen = new Pen(BorderColor))
            {
                graphics.DrawRectangle(pen, bounds);
            }
        }
    }
}
