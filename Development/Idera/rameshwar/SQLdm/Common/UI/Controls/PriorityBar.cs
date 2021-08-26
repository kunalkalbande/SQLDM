using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.Common.UI.Controls
{
    public partial class PriorityBar : UserControl
    {
        private readonly static float _max = 40.0f;
        private float _value = 20.0f;

        public float Value 
        {
            get { return (_value); }
            set
            {
                if (value >= 0)
                {
                    _value = (value > _max) ? _max : value;
                }
                else
                {
                    _value = 0;
                }
                Invalidate(false);
            }
        }
        public PriorityBar()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (null == e) return;
            if (null == e.Graphics) return;
            Draw(e.Graphics, ClientRectangle, _value);
        }

        public static void Draw(Graphics g, Rectangle rect, float val)
        {
            Rectangle r = new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var b = new LinearGradientBrush(r, Color.Yellow, Color.Red, LinearGradientMode.Horizontal))
            {
                RectangleF pr = r;
                float fill = val / _max;
                pr.Width = pr.Width * fill;
                g.FillRectangle(b, pr);
            }
            g.DrawRectangle(Pens.Gray, r);
            int step = (rect.Width / 5);
            int stop = r.Right;
            int x = r.Left + step;
            Point p1 = new Point(rect.Left, rect.Top);
            Point p2 = new Point(rect.Left, rect.Top + r.Height);
            while (x < stop)
            {
                p2.X = p1.X = x;
                g.DrawLine(Pens.Gray, p1, p2);
                x += step;
            }
        }

    }
}
