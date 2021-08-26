using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    /// <summary>
    /// Extends the standard Windows Forms panel to provide a border and gradient fill.
    /// </summary>
    [ToolboxBitmap(typeof (Panel))]
    public class GradientPanel : Panel
    {
        private GradientPanelFillStyle fillStyle = GradientPanelFillStyle.Gradient;
        private int gradientAngle = 0;
        private Color backColor2 = Color.White;
        private bool showBorder = true;
        private int borderWidth = 1;
        private Color borderColor = SystemColors.ControlDark;

        public GradientPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            Padding = new Padding(borderWidth);
        }

        [Category("Appearance")]
        public Color BackColor2
        {
            get { return backColor2; }
            set
            {
                backColor2 = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(GradientPanelFillStyle.Gradient)]
        public GradientPanelFillStyle FillStyle
        {
            get { return fillStyle; }
            set
            {
                fillStyle = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        public int GradientAngle
        {
            get { return gradientAngle; }
            set
            {
                if (value < 0 || value > 360)
                {
                    throw new ArgumentOutOfRangeException("The gradient angle must be a value between 0 and 360.");
                }

                gradientAngle = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowBorder
        {
            get { return showBorder; }
            set
            {
                showBorder = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return borderWidth; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("The border width must be greater than zero.");
                }

                borderWidth = value;
                Padding = new Padding(borderWidth);
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            int widthAccomodatingForBorder = showBorder 
                                                  ? ClientRectangle.Width - (borderWidth * 2) 
                                                  : ClientRectangle.Width;
            int heightAccomodatingForBorder = showBorder
                                                  ? ClientRectangle.Height - (borderWidth*2)
                                                  : ClientRectangle.Height;

            if (widthAccomodatingForBorder > 0 && heightAccomodatingForBorder > 0)
            {
                Rectangle backgroundRectangle = ClientRectangle;

                //
                // Draw border
                //
                if (showBorder && borderWidth > 0)
                {
                    backgroundRectangle = Rectangle.Inflate(backgroundRectangle, -borderWidth, -borderWidth);
                    RectangleF borderRectangle =
                        new Rectangle(ClientRectangle.X + (borderWidth/2), ClientRectangle.Y + (borderWidth/2), 
                                      ClientRectangle.Width - borderWidth, ClientRectangle.Height - borderWidth);

                    using (Pen borderPen = new Pen(borderColor, borderWidth))
                    {
                        e.Graphics.DrawRectangle(borderPen, borderRectangle.X, borderRectangle.Y, 
                                                 borderRectangle.Width, borderRectangle.Height);
                    }
                }

                //
                // Draw background
                //
                if (fillStyle == GradientPanelFillStyle.Gradient)
                {
                    using (
                        LinearGradientBrush gradientBackgroundBrush =
                            new LinearGradientBrush(backgroundRectangle, BackColor, backColor2, gradientAngle))
                    {
                        e.Graphics.FillRectangle(gradientBackgroundBrush, backgroundRectangle);
                    }
                }
                else if (fillStyle == GradientPanelFillStyle.RadialGradient)
                {                    
                    e.Graphics.Clear(BackColor);

                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(-this.Width * 1.6f, -this.Height * 1.6f, this.Width * 4, this.Height * 3);
                    e.Graphics.DrawPath(Pens.Transparent, path);

                    PathGradientBrush pathBrush = new PathGradientBrush(path);
                    pathBrush.CenterColor = BackColor2;
                    pathBrush.SurroundColors = new Color[] { BackColor };

                    e.Graphics.FillPath(pathBrush, path);
                }
                else
                {
                    using (SolidBrush solidBackgroundBrush = new SolidBrush(BackColor))
                    {
                        e.Graphics.FillRectangle(solidBackgroundBrush, backgroundRectangle);
                    }
                }
            }
            else if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                using (SolidBrush solidBackgroundBrush = new SolidBrush(showBorder ? borderColor : BackColor))
                {
                    e.Graphics.FillRectangle(solidBackgroundBrush, ClientRectangle);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }

    public enum GradientPanelFillStyle
    {
        Solid,
        Gradient,
        RadialGradient
    }
}