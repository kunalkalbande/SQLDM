using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class HeaderLabel : UserControl
    {
        public HeaderLabel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
            BackColor = SystemColors.Control;
        }

        [Category("Appearance")]
        public string HeaderText
        {
            get { return headerText.Text; }
            set { headerText.Text = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.DarkGray))
            {
                e.Graphics.DrawLine(pen, ClientRectangle.X, ClientRectangle.Y + ClientRectangle.Height - 1,
                                    ClientRectangle.X + ClientRectangle.Width,
                                    ClientRectangle.Y + ClientRectangle.Height - 1);
            }
        }
    }
}
