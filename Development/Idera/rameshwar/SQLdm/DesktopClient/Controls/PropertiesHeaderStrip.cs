using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public class PropertiesHeaderStrip : UserControl
    {
        public PropertiesHeaderStrip()
        {
            Size = new Size(150, 25);
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            WordWrap = false;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                using (var brush = new LinearGradientBrush(e.ClipRectangle,
                                                           Color.FromArgb(218,218,218),
                                                           Color.FromArgb(225,225,225),
                                                           LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }

                //using (Pen pen = new Pen(Color.FromArgb(0,0,0)))
                //{
                //    e.Graphics.DrawLine(pen, ClientRectangle.X, ClientRectangle.Y + ClientRectangle.Height - 1,
                //                        ClientRectangle.X + ClientRectangle.Width,
                //                        ClientRectangle.Y + ClientRectangle.Height - 1);
                //}

                using (SolidBrush textBrush = new SolidBrush(ForeColor))
                {
                    StringFormat textFormat = new StringFormat();
                    textFormat.LineAlignment = StringAlignment.Center;
                    if (!WordWrap)
                    {
                        textFormat.FormatFlags |= StringFormatFlags.NoWrap;
                    }
                    textFormat.Trimming = StringTrimming.EllipsisCharacter;
                    Rectangle textBounds = ClientRectangle;
                    textBounds.X += 10;
                    e.Graphics.DrawString(Text, Font, textBrush, textBounds, textFormat);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool WordWrap { get; set; }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}