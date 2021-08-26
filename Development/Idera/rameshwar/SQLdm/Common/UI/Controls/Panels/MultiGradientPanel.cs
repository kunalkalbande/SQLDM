using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

namespace Idera.SQLdm.Common.UI.Controls.Panels
{
    /// <summary>
    /// GradientPanel is just like a regular panel except it optionally  
    /// shows a gradient.
    /// </summary>
    [ToolboxBitmap(typeof(Panel))]
    public class MultiGradientPanel : Panel
    {
        private Brush brush = null;

        public MultiGradientPanel()
        {
            // Set up double buffering
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Property GradientColor (Color)
        /// </summary>
        private Color[] _gradientColors;
        public Color[] GradientColors
        {
            get { return this._gradientColors; }
            set { this._gradientColors = value; RecreateBrush(); }
        }

        /// <summary>
        /// Property Rotation (float)
        /// </summary>
        private float _rotation;
        public float Rotation
        {
            get { return this._rotation; }
            set { this._rotation = value; RecreateBrush(); }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { DestroyBrush(); }
            base.Dispose(disposing);
        }

        private void DestroyBrush()
        {
            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }
        }

        private void RecreateBrush()
        {
            DestroyBrush();
            CreateBrush();
            // make sure the control refreshes itself and its child controls when the brush changes
            this.Invalidate(true);
        }
        /// <summary>
        /// this is an optimization to (re)create the background paint brush
        /// only when its properties change, and not for every Paint event.
        /// </summary>
        private void CreateBrush()
        {
            //dispose of the brush if it exists
            if (brush != null) return;
            if (null == _gradientColors) { brush = new SolidBrush(BackColor); return; }
            if (_gradientColors.Length <= 0) { brush = new SolidBrush(BackColor); return; }
            if (_gradientColors.Length == 1) { brush = new SolidBrush(_gradientColors[0]); return; }
            ColorBlend blend = new ColorBlend();
            float[] pos = new float[_gradientColors.Length];
            float increment = 1.0f / (_gradientColors.Length - 1);
            float current = 0.0f;
            for (int n = 0; n < pos.Length; ++n)
            {
                pos[n] = current;
                current += increment;
            }
            pos[pos.Length - 1] = 1.0f;
            blend.Colors = _gradientColors;
            blend.Positions = pos;
            if (this.ClientRectangle.Width <= 0 || this.ClientRectangle.Height <= 0)
            {
                brush = new SolidBrush(BackColor);
            }
            else
            {
                var b = new LinearGradientBrush(this.ClientRectangle, BackColor, BackColor, _rotation, true);
                b.InterpolationColors = blend;
                brush = b;
            }
        }        

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            CreateBrush();
			pevent.Graphics.FillRectangle(brush, this.ClientRectangle);
		}

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DestroyBrush();
            Invalidate();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            RecreateBrush();
        }

    }
}
