namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using Idera.SQLdm.DesktopClient.Properties;

    public partial class CustomCheckedListBox : ListView
    {
        private Color linkColor = Color.FromKnownColor(KnownColor.HotTrack);
        private LinkLabelRenderer linkRenderer;
        private bool boldLinks = false;
        private bool underlineLinks = true;

        public CustomCheckedListBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        [DefaultValue(false)]
        public bool BoldLinks
        {
            get { return boldLinks; }
            set
            {
                boldLinks = value;
                if (linkRenderer != null)
                {
                    linkRenderer.LinksBold = value;
                }
            }
        }

        [DefaultValue(true)]
        public bool UnderlineLinks
        {
            get { return underlineLinks; }
            set
            {
                underlineLinks = value;
                if (linkRenderer != null)
                {
                    linkRenderer.LinksUnderlined = value;
                }
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            this.columnHeader1.Width = ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth + 8;
            base.OnClientSizeChanged(e);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (Parent == null)
            {   // if we have no parent then give up
                e.DrawDefault = true;
                base.OnDrawItem(e);
                return;
            }

            Color bgColor = BackColor;
            if (Settings.Default.ColorScheme == "Dark")
            {
                bgColor = ColorTranslator.FromHtml(DarkThemeColorConstants.ListViewBackColor);
            }

            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }
            if (Settings.Default.ColorScheme == "Dark")
            {
                ForeColor = Color.White;
                LinkColor = Color.AntiqueWhite;
                var b = e.Bounds;
                var state = e.Item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                Size glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
                int checkPad = (b.Height - glyphSize.Height) / 2;
                var pt = new Point(b.X + checkPad, b.Y + checkPad);

                Rectangle rect = new Rectangle(pt, new Size(10, 10));

                //This is for drawing string text

                if (!Enabled)
                {
                    using (Pen pen = new Pen(Color.Black))
                        e.Graphics.DrawRectangle(pen, rect);//This is for Checkbox rectangle

                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.White, rect);//This is for Checkbox rectangle
                    //using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml(DarkThemeColorConstants.ForeColor)))
                        //e.Graphics.DrawString(e.Item.Text, Font, brush, pt.X + 27f, pt.Y);
                }

                if (state == CheckBoxState.CheckedNormal)
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    using (Font wing = new Font("Wingdings", 8f, FontStyle.Bold))
                        e.Graphics.DrawString("ü", wing, brush, pt.X - 2, pt.Y - 1); //This is For tick mark
                    //using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml(DarkThemeColorConstants.CheckListBoxForeColor)))
                       // e.Graphics.DrawString(e.Item.Text, Font, brush, pt.X + 27f, pt.Y);
                }
                if (linkRenderer == null)
                {
                    linkRenderer = new LinkLabelRenderer(Font);
                    linkRenderer.LinksBold = boldLinks;
                    linkRenderer.LinksUnderlined = underlineLinks;
                    linkRenderer.TextColor = ForeColor;
                    linkRenderer.LinkColor = LinkColor;
                }
                var startingLocation = e.Bounds.Location;
                int itemHeight = e.Item.Bounds.Height;
                int offset = 2;
                if (itemHeight > glyphSize.Height)
                {
                    offset = (itemHeight - glyphSize.Height) / 2;
                    startingLocation.Offset(offset, offset);
                }
                else
                    startingLocation.Offset(offset, 0);
                startingLocation.Offset(glyphSize.Width + offset, 0);
                linkRenderer.Draw(e.Graphics, e.Item.Text, new Rectangle(startingLocation, e.Bounds.Size));
                base.OnDrawItem(e);
            }
            else
            {

                Point startingLocation = e.Bounds.Location;

                CheckBoxState state = e.Item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                int offset = 2;
                Size boxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);

                int itemHeight = e.Item.Bounds.Height;

                if (itemHeight > boxSize.Height)
                {
                    offset = (itemHeight - boxSize.Height) / 2;
                    startingLocation.Offset(offset, offset);
                }
                else
                    startingLocation.Offset(offset, 0);

                CheckBoxRenderer.DrawCheckBox(e.Graphics, startingLocation, state);


                startingLocation.Offset(boxSize.Width + offset, 0);

                if (linkRenderer == null)
                {
                    linkRenderer = new LinkLabelRenderer(Font);
                    linkRenderer.LinksBold = boldLinks;
                    linkRenderer.LinksUnderlined = underlineLinks;
                    linkRenderer.TextColor = ForeColor;
                    linkRenderer.LinkColor = LinkColor;
                }

                linkRenderer.Draw(e.Graphics, e.Item.Text, new Rectangle(startingLocation, e.Bounds.Size));
                base.OnDrawItem(e);
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (linkRenderer != null)
            {
                linkRenderer.TextColor = ForeColor;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (linkRenderer != null)
            {
                linkRenderer.NormalFont = Font;
                linkRenderer.LinkFont = null;
            }
        }

        public Color LinkColor
        {
            get { return linkColor; }
            set
            {
                linkColor = value;
                if (linkRenderer != null)
                {
                    linkRenderer.LinkColor = value;
                }
            }
        }
    }
}
