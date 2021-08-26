using Idera.SQLdm.DesktopClient.Properties;
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
                Color backcolor1 = Color.FromArgb(218, 218, 218);
                Color backcolor2 = Color.FromArgb(225, 225, 225);

                if (Settings.Default.ColorScheme == "Dark")
                {
                    backcolor1 = ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor);
                    backcolor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.HeaderStripBackColor);
                    ForeColor = Color.White;
                }

                using (var brush = new LinearGradientBrush(e.ClipRectangle,
                                                       backcolor1,
                                                        backcolor2,
                                                        LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }


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