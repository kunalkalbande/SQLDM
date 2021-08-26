using System;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class HistoryBrowserHotTrackLabel : Control
    {
        public Color hotTrackColor = Color.FromArgb(255, 231, 162);
        public Color mouseDownColor = Color.FromArgb(251, 140, 60);

        private State state = State.Normal;

        private enum State
        {
            Normal,
            MouseOver,
            MouseDown,
            Selected
        }

        public HistoryBrowserHotTrackLabel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
        }

        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        public Color HotTrackColor
        {
            get { return hotTrackColor; }
            set
            {
                hotTrackColor = value;
                Invalidate();
            }
        }

        public Color MouseDownColor
        {
            get { return mouseDownColor; }
            set
            {
                mouseDownColor = value;
                Invalidate();
            }
        }

        public void ShowSelected()
        {
            state = State.Selected;
            Invalidate();
        }

        public void ClearState()
        {
            state = State.Normal;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (state != State.Selected)
            {
                state = State.MouseOver;
                Invalidate();
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (state != State.Selected)
            {
                state = State.MouseDown;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (state != State.Selected)
            {
                state = State.MouseOver;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (state != State.MouseDown && state != State.Selected)
            {
                if (state != State.MouseOver)
                {
                    state = State.MouseOver;
                    Invalidate();
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (state != State.Selected)
            {
                state = State.Normal;
                Invalidate();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush brush;

            switch (state)
            {
                case State.Normal:
                    using (brush = new SolidBrush(BackColor))
                    {
                        e.Graphics.FillRectangle(brush, ClientRectangle);
                    }

                    using (Pen pen = new Pen(Color.White))
                    {
                        e.Graphics.DrawLine(pen, ClientRectangle.X, ClientRectangle.Y,
                                            ClientRectangle.X + ClientRectangle.Width, ClientRectangle.Y);
                    }

                    using (Pen pen = new Pen(Color.White))
                    {
                        e.Graphics.DrawLine(pen, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.X,
                                            ClientRectangle.Y + ClientRectangle.Height);
                    }
                    break;
                case State.MouseOver:
                    using (brush = new SolidBrush(hotTrackColor))
                    {
                        e.Graphics.FillRectangle(brush, ClientRectangle);
                    }
                    break;
                case State.MouseDown:
                    using (brush = new SolidBrush(mouseDownColor))
                    {
                        e.Graphics.FillRectangle(brush, ClientRectangle);
                    }
                    break;
                case State.Selected:
                    using (brush = new SolidBrush(hotTrackColor))
                    {
                        e.Graphics.FillRectangle(brush, ClientRectangle);
                    }

                    Rectangle selectionRect = ClientRectangle;
                    selectionRect.X += 1;
                    selectionRect.Y += 1;
                    selectionRect.Width -= 3;
                    selectionRect.Height -= 3;

                    using (Pen pen = new Pen(Color.FromArgb(255, 189, 105)))
                    {
                        e.Graphics.DrawRectangle(pen, selectionRect);
                    }
                    break;
            }

            using (brush = new SolidBrush(ForeColor))
            {
                StringFormat format = new StringFormat();
                format.FormatFlags |= StringFormatFlags.DirectionVertical;
                format.FormatFlags |= StringFormatFlags.NoWrap;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                e.Graphics.DrawString(Text, Font, brush, ClientRectangle, format);
            }

            base.OnPaint(e);
        }
    }
}
