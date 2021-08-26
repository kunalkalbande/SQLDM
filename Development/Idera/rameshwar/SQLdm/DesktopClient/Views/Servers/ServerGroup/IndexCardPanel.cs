using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    using System.Diagnostics;
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
    internal class IndexCardPanel : Panel
    {
        protected const int IndexCardPadding = 10;
        protected const int IndexCardShadowOffset = 7;
        protected const int HeaderHeight = 15;
        protected const int HeaderTitlePadding = 3;

        protected Rectangle selectionRectangleBounds;
        protected Rectangle indexCardBounds;
        protected Rectangle headerBounds;
        protected Rectangle contentAreaBounds;

        protected string title;
        private bool selected = false;
        private bool mouseOver = false;

        public EventHandler SelectedChanged;
        public EventHandler MouseOverChanged;

        protected int instanceId;
        public IndexCardPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            UpdateClientBounds();

            Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point, 0);
            Title = "Card Title";
            Margin = new Padding(0);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            e.Control.MouseMove += ReflectChildMouseMove;
            e.Control.MouseEnter += ReflectChildMouseEnter;
            base.OnControlAdded(e);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            e.Control.MouseMove -= ReflectChildMouseMove;
            e.Control.MouseEnter -= ReflectChildMouseEnter;
            base.OnControlRemoved(e);
        }

        void ReflectChildMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        void ReflectChildMouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }


        [Category("Appearance")]
        public virtual string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    Invalidate(false);
                    Update();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    Invalidate(false);
                    Update();
                    OnSelectedChanged(EventArgs.Empty);
                }
            }
        }

        public bool MouseOver
        {
            get { return mouseOver; }
            set
            {
                if (mouseOver != value)
                {
                    mouseOver = value;
                    Invalidate(false);
                    Update();
                    if (mouseOver)
                    {
                        OnMouseOverChanged(EventArgs.Empty);
                    }
                }
            }
        }

        private void OnSelectedChanged(EventArgs e)
        {
            if (SelectedChanged != null)
            {
                SelectedChanged(this, e);
            }
        }

        protected virtual void OnMouseOverChanged(EventArgs e)
        {
            if (MouseOverChanged != null)
            {
                MouseOverChanged(this, e);
            }
        }

        private void UpdateClientBounds()
        {
            selectionRectangleBounds =
                    new Rectangle(ClientRectangle.X,
                                  ClientRectangle.Y,
                                  ClientRectangle.Width - 1,
                                  ClientRectangle.Height - 1);

            indexCardBounds =
                    new Rectangle(ClientRectangle.X + IndexCardPadding, ClientRectangle.Y + IndexCardPadding,
                                  ClientRectangle.Width - IndexCardPadding * 2 - 1,
                                  ClientRectangle.Height - IndexCardPadding * 2 - 1);

            headerBounds = indexCardBounds;
            ++headerBounds.X;
            ++headerBounds.Y;
            --headerBounds.Width;
            headerBounds.Height = HeaderHeight;

            contentAreaBounds = indexCardBounds;
            ++contentAreaBounds.X;
            contentAreaBounds.Y += HeaderHeight + 2;
            --contentAreaBounds.Width;
            contentAreaBounds.Height -= HeaderHeight + 2;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (mouseOver)
            {
                DrawHoverRectangle(e.Graphics);
            }
            else if (selected)
            {
                DrawSelectionRectangle(e.Graphics);
            }

            DrawIndexCard(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateClientBounds();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Selected = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            MouseOver = true;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseOver = true;

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            if (!ClientRectangle.Contains(PointToClient(MousePosition)))
                MouseOver = false;

            base.OnMouseLeave(e);
        }

        private void DrawSelectionRectangle(Graphics graphics)
        {
            if (graphics != null)
            {
                Color borderColor = Color.FromArgb(242, 149, 54);
                Color selectionRectangle2ColorTop1 = Color.FromArgb(250, 227, 185);
                Color selectionRectangle2ColorTop2 = Color.FromArgb(246, 185, 100);
                Color selectionRectangle2ColorBottom1 = Color.FromArgb(244, 169, 65);
                Color selectionRectangle2ColorBottom2 = Color.FromArgb(236, 152, 49);

                using (Pen borderPen1 = new Pen(borderColor))
                {
                    graphics.DrawRectangle(borderPen1, selectionRectangleBounds);
                }

                float oneThirdHeight1 = selectionRectangleBounds.Height / 3;
                RectangleF topBounds1 = selectionRectangleBounds;
                topBounds1.X += 1;
                topBounds1.Y += 1;
                topBounds1.Width -= 1;
                topBounds1.Height = oneThirdHeight1;
                RectangleF bottomBounds1 = selectionRectangleBounds;
                bottomBounds1.X += 1;
                bottomBounds1.Y += oneThirdHeight1;
                bottomBounds1.Width -= 1;
                bottomBounds1.Height -= oneThirdHeight1;
                using (LinearGradientBrush selectionRectangleBrushTop = new LinearGradientBrush(topBounds1, selectionRectangle2ColorTop1, selectionRectangle2ColorTop2, LinearGradientMode.Vertical))
                using (LinearGradientBrush selectionRectangleBrushBottom = new LinearGradientBrush(bottomBounds1, selectionRectangle2ColorBottom1, selectionRectangle2ColorBottom2, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(selectionRectangleBrushTop, topBounds1);
                    graphics.FillRectangle(selectionRectangleBrushBottom, bottomBounds1);
                }
            }
        }

        private void DrawHoverRectangle(Graphics graphics)
        {
            if (graphics != null)
            {
                Color borderColor = Color.FromArgb(221, 208, 155);
                Color selectionRectangle1ColorTop1 = Color.FromArgb(255, 243, 203);
                Color selectionRectangle1ColorTop2 = Color.FromArgb(255, 206, 111);
                Color selectionRectangle1ColorBottom1 = Color.FromArgb(255, 195, 79);
                Color selectionRectangle1ColorBottom2 = Color.FromArgb(255, 199, 83);

                using (Pen borderPen1 = new Pen(borderColor))
                {
                    graphics.DrawRectangle(borderPen1, selectionRectangleBounds);
                }

                float oneThirdHeight1 = selectionRectangleBounds.Height / 3;
                RectangleF topBounds1 = selectionRectangleBounds;
                topBounds1.X += 1;
                topBounds1.Y += 1;
                topBounds1.Width -= 1;
                topBounds1.Height = oneThirdHeight1;
                RectangleF bottomBounds1 = selectionRectangleBounds;
                bottomBounds1.X += 1;
                bottomBounds1.Y += oneThirdHeight1;
                bottomBounds1.Width -= 1;
                bottomBounds1.Height -= oneThirdHeight1;
                using (LinearGradientBrush selectionRectangleBrushTop = new LinearGradientBrush(topBounds1, selectionRectangle1ColorTop1, selectionRectangle1ColorTop2, LinearGradientMode.Vertical))
                using (LinearGradientBrush selectionRectangleBrushBottom = new LinearGradientBrush(bottomBounds1, selectionRectangle1ColorBottom1, selectionRectangle1ColorBottom2, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(selectionRectangleBrushTop, topBounds1);
                    graphics.FillRectangle(selectionRectangleBrushBottom, bottomBounds1);
                }
            }
        }

        private void DrawIndexCard(Graphics graphics)
        {
            if (graphics != null)
            {
                Color borderColor = Color.FromArgb(140, 140, 140);
                Color contentBackColor1 = Color.FromArgb(255, 255, 255);
                Color contentBackColor2 = Color.FromArgb(241, 245, 249);

                // Draw the card shadow first so it's in the background
                DrawIndexCardShadow(graphics, indexCardBounds);

                // Draw borders
                using (Pen borderPen = new Pen(borderColor))
                {
                    graphics.DrawRectangle(borderPen, indexCardBounds);

                    graphics.DrawLine(borderPen, indexCardBounds.X, indexCardBounds.Y + HeaderHeight + 1,
                                      indexCardBounds.X + indexCardBounds.Width,
                                      indexCardBounds.Y + HeaderHeight + 1);
                }

                // Draw header
                DrawIndexCardHeader(graphics, headerBounds);

                // Draw content area background
                using (
                    LinearGradientBrush contentBackgroundBrush =
                        new LinearGradientBrush(contentAreaBounds, contentBackColor1, contentBackColor2,
                                                LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(contentBackgroundBrush, contentAreaBounds);
                }
            }
        }

        private void DrawIndexCardShadow(Graphics graphics, Rectangle bounds)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                using (GraphicsPath shadowPath = new GraphicsPath())
                {
                    GraphicsState preShadowState = graphics.Save();
                    graphics.TranslateTransform(IndexCardShadowOffset, IndexCardShadowOffset);
                    shadowPath.AddRectangle(bounds);

                    using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                    {
                        ColorBlend colorBlend = new ColorBlend(3);
                        colorBlend.Colors = new Color[]
                            {
                                Color.Transparent,
                                Color.FromArgb(180, Color.Black),
                                Color.FromArgb(180, Color.Black)
                            };
                        colorBlend.Positions = new float[] {0f, .1f, 1f};
                        shadowBrush.InterpolationColors = colorBlend;
                        graphics.FillPath(shadowBrush, shadowPath);
                        graphics.Restore(preShadowState);
                    }
                }
            }
        }

        private void DrawIndexCardHeader(Graphics graphics, Rectangle bounds)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                Color topColor1 = Color.FromArgb(232, 232, 232);
                Color topColor2 = Color.FromArgb(218, 218, 218);
                Color bottomColor1 = Color.FromArgb(203, 203, 203);
                Color bottomColor2 = Color.FromArgb(214, 214, 214);
                float oneThirdHeight = HeaderHeight/3;

                RectangleF topBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, oneThirdHeight);
                using (
                    LinearGradientBrush topBrush =
                        new LinearGradientBrush(topBounds, topColor1, topColor2, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(topBrush, topBounds);
                }

                RectangleF bottomBounds =
                    new RectangleF(bounds.X, bounds.Y + oneThirdHeight, bounds.Width, oneThirdHeight*2);
                using (
                    LinearGradientBrush bottomBrush =
                        new LinearGradientBrush(bottomBounds, bottomColor1, bottomColor2, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(bottomBrush, bottomBounds);
                }

                Rectangle titleBounds = bounds;
                titleBounds.X += HeaderTitlePadding;
                titleBounds.Width -= HeaderTitlePadding;
                StringFormat titleFormat = new StringFormat();

                titleFormat.LineAlignment = StringAlignment.Center;
                titleFormat.FormatFlags |= StringFormatFlags.NoWrap;
                titleFormat.Trimming = StringTrimming.EllipsisWord;
               
                graphics.DrawString(Title.Length> 35? Title.Substring(0,35)+"...":Title, Font, Brushes.Black, titleBounds, titleFormat);

                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                {
                    graphics.DrawImage(DesktopClient.Properties.Resources.CloudServerNormal, new Point(bounds.Width - 5, bounds.Y));
                }
            }
        }
    }
}