using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public enum FrameOrientation
    {
        Horizontal,
        Vertical
    }

    public partial class ImageAnimationControl : Control
    {
        private const float MaxAspectRatioDefault = 1.0f;
        private const int FrameCountDefault = 1;

        private Image image = null;
        private FrameOrientation frameOrientaion = FrameOrientation.Horizontal;
        private Size frameDimensions = new Size(32, 32);
        private float maxAspectRatio = MaxAspectRatioDefault;
        private int frameCount = FrameCountDefault;
        private int currentFrameIndex = 0;
        private bool maintainAspectRatio = false;

        public ImageAnimationControl()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
        }

        [Category("Appearance")]
        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                Invalidate();
            }
        }

        [Category("Appearance"), DefaultValue(FrameOrientation.Horizontal)]
        public FrameOrientation FrameOrientation
        {
            get { return frameOrientaion; }
            set
            {
                frameOrientaion = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Size FrameDimensions
        {
            get { return frameDimensions; }
            set
            {
                frameDimensions = value;
                Invalidate();
            }
        }

        [Category("Appearance"), DefaultValue(MaxAspectRatioDefault)]
        public float MaxAspectRatio
        {
            get { return maxAspectRatio; }
            set
            {
                maxAspectRatio = value;
                Invalidate();
            }
        }

        [Category("Appearance"), DefaultValue(FrameCountDefault)]
        public int FrameCount
        {
            get { return frameCount; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("FrameCount", "FrameCount must be 1 or greater.");
                }

                frameCount = value;
            }
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool MaintainAspectRatio
        {
            get { return maintainAspectRatio; }
            set
            {
                maintainAspectRatio = value;
                Invalidate();
                Update();
            }
        }

        public void Reset()
        {
            currentFrameIndex = 0;
            Invalidate();
            Update();
        }

        public void MoveNext()
        {
            currentFrameIndex = currentFrameIndex != frameCount - 1 ? ++currentFrameIndex : 0;
            Invalidate();
            Update();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (image != null)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                RectangleF drawRegion = ClientRectangle;
                int x, y;

                if (frameOrientaion == FrameOrientation.Horizontal)
                {
                    x = frameDimensions.Width*currentFrameIndex;
                    y = 0;

                    if (x + frameDimensions.Width > image.Width)
                    {
                        return;
                    }
                }
                else
                {
                    x = 0;
                    y = frameDimensions.Height*currentFrameIndex;

                    if (y + frameDimensions.Height > image.Height)
                    {
                        return;
                    }
                }

                if (maintainAspectRatio)
                {
                    float smallestAspectRatio = Math.Min((float)ClientRectangle.Width / (float)frameDimensions.Width,
                                                         (float)ClientRectangle.Height / (float)frameDimensions.Height);

                    if (smallestAspectRatio > maxAspectRatio)
                    {
                        smallestAspectRatio = maxAspectRatio;
                    }
                    
                    float width = frameDimensions.Width*smallestAspectRatio;
                    float height = frameDimensions.Height*smallestAspectRatio;
                    float posX = ClientRectangle.Width / 2 - width / 2;
                    float posY = ClientRectangle.Height / 2 - height / 2;

                    drawRegion = new RectangleF(posX, posY, width, height);
                }

                e.Graphics.DrawImage(image, drawRegion,
                                     new Rectangle(x, y, frameDimensions.Width, frameDimensions.Height),
                                     GraphicsUnit.Pixel);
            }

            base.OnPaint(e);
        }
    }
}