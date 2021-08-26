namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

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
            get { return boldLinks;  }
            set {   
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
             
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }
           
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
