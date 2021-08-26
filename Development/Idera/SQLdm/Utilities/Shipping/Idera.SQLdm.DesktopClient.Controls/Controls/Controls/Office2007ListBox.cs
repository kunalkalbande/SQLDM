using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [ToolboxBitmap(typeof(ListBox))]
    public class Office2007ListBox : ListBox
    {
        private const string DividerText = "<DIVIDER>";
        private const int DividerItemHeight = 2;
        private const int NormalItemHeight = 26;
        private const int NormalItemGradientPivotPoint = 7;
        private const int NormalItemLeftPadding = 12;

        private int hoverIndex = NoMatches;
        private readonly Dictionary<int, Rectangle> itemBounds = new Dictionary<int, Rectangle>();
        public bool isDarkThemeSelected = false;

        public Office2007ListBox(bool isDarkThemeSelected = false)
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            this.isDarkThemeSelected = isDarkThemeSelected;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (Items.Count > 0)
            {
                // Draw divider
                if (string.Compare(Items[e.Index].ToString(), DividerText, true) == 0)
                {
                    using (Pen dividerPen1 = new Pen(Color.FromArgb(221, 231, 238)))
                    using (Pen dividerPen2 = new Pen(Color.FromArgb(197, 197, 197)))
                    {
                        e.Graphics.DrawLine(dividerPen1, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);
                        e.Graphics.DrawLine(dividerPen2, e.Bounds.X, e.Bounds.Y + 1, e.Bounds.X + e.Bounds.Width,
                                            e.Bounds.Y + 1);
                    }
                }
                // Draw normal list item
                else
                {
                    if (itemBounds.ContainsKey(e.Index))
                    {
                        itemBounds[e.Index] = e.Bounds;
                    }
                    else
                    {
                        itemBounds.Add(e.Index, e.Bounds);
                    }

                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        DrawSelectedItem(e.Graphics, e.Index, e.Bounds);
                    }
                    else if (hoverIndex == e.Index)
                    {
                        DrawHoverItem(e.Graphics, e.Index, e.Bounds);
                    }
                    else
                    {
                        DrawUnselectedItem(e.Graphics, e.Index, e.Bounds);
                    }
                }
            }
        }

        private void DrawItemText(Graphics graphics, int index, Rectangle bounds)
        {
            using (Font textFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(isDarkThemeSelected ? ColorTranslator.FromHtml("#E4E5EA") : Color.Black))
            {
                StringFormat textFormat = new StringFormat();
                textFormat.LineAlignment = StringAlignment.Center;
                textFormat.FormatFlags |= StringFormatFlags.NoWrap;
                textFormat.Trimming = StringTrimming.EllipsisCharacter;
                Rectangle textBounds = bounds;
                textBounds.X += NormalItemLeftPadding;
                graphics.DrawString(Items[index].ToString(), textFont, textBrush, textBounds, textFormat);
            }
        }

        private void DrawUnselectedItem(Graphics graphics, int index, Rectangle bounds)
        {
            graphics.FillRectangle(isDarkThemeSelected ? new SolidBrush(ColorTranslator.FromHtml("#006089")) : Brushes.White, bounds);
            DrawItemText(graphics, index, bounds);
        }

        private void DrawSelectedItem(Graphics graphics, int index, Rectangle bounds)
        {
            LinearGradientBrush gradientBackgroundBrush;

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(Bounds, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(139, 118, 84),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(195, 184, 166), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, bounds);
            }

            Rectangle deflatedRectangle = Rectangle.Inflate(bounds, -1, -1);

            RectangleF topRectangle =
                new RectangleF(deflatedRectangle.X, deflatedRectangle.Y, deflatedRectangle.Width, NormalItemGradientPivotPoint);

            RectangleF bottomRectangle =
                new RectangleF(deflatedRectangle.X, deflatedRectangle.Y + NormalItemGradientPivotPoint, deflatedRectangle.Width, deflatedRectangle.Height - NormalItemGradientPivotPoint);

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(topRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 247, 241),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(252, 175, 119), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, topRectangle);
            }

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(bottomRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(251, 151, 79),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(254, 189, 108), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, bottomRectangle);
            }

            deflatedRectangle = Rectangle.Inflate(deflatedRectangle, -1, -1);

            topRectangle =
                new RectangleF(deflatedRectangle.X, deflatedRectangle.Y, deflatedRectangle.Width, NormalItemGradientPivotPoint);

            bottomRectangle =
                new RectangleF(deflatedRectangle.X, deflatedRectangle.Y + NormalItemGradientPivotPoint, deflatedRectangle.Width, deflatedRectangle.Height - NormalItemGradientPivotPoint);

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(topRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 241, 231),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(252, 166, 104), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, topRectangle);
            }

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(bottomRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(251, 140, 60),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(254, 180, 91), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, bottomRectangle);
            }

            DrawItemText(graphics, index, bounds);
        }

        private void DrawHoverItem(Graphics graphics, int index, Rectangle bounds)
        {
            LinearGradientBrush gradientBackgroundBrush;

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(Bounds, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(192, 167, 118),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(211, 205, 184), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, bounds);
            }

            Rectangle deflatedRectangle = Rectangle.Inflate(bounds, -1, -1);

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(deflatedRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 254, 249),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 246, 215), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, deflatedRectangle);
            }

            deflatedRectangle = Rectangle.Inflate(deflatedRectangle, -1, -1);

            RectangleF topRectangle =
                new RectangleF(deflatedRectangle.X, deflatedRectangle.Y, deflatedRectangle.Width, NormalItemGradientPivotPoint);

            RectangleF bottomRectangle =
                new RectangleF(deflatedRectangle.X, deflatedRectangle.Y + NormalItemGradientPivotPoint, deflatedRectangle.Width, deflatedRectangle.Height - NormalItemGradientPivotPoint);

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(topRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 250, 234),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 224, 120), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, topRectangle);
            }

            using (gradientBackgroundBrush =
                    new LinearGradientBrush(bottomRectangle, isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 215, 80),
                    isDarkThemeSelected ? ColorTranslator.FromHtml("#00A1DD") : Color.FromArgb(255, 228, 127), 90f))
            {
                graphics.FillRectangle(gradientBackgroundBrush, bottomRectangle);
            }

            DrawItemText(graphics, index, bounds);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            if (Items.Count > 0)
            {
                if (string.Compare(Items[e.Index].ToString(), DividerText, true) == 0)
                {
                    e.ItemHeight = DividerItemHeight;
                }
                else
                {
                    e.ItemHeight = NormalItemHeight;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int index = HitTest(e.X, e.Y);

            if (index != hoverIndex)
            {
                int oldHoverIndex = hoverIndex;
                hoverIndex = index;

                if (oldHoverIndex != -1)
                {
                    Invalidate(GetItemRectangle(oldHoverIndex));
                }

                if (hoverIndex != -1)
                {
                    Invalidate(GetItemRectangle(hoverIndex));
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (hoverIndex != NoMatches)
            {
                Rectangle itemRect;

                if (itemBounds.TryGetValue(hoverIndex, out itemRect))
                {
                    hoverIndex = NoMatches;
                    Invalidate(itemRect);
                }
            }
        }

        /// <summary>
        /// Attemtps to get the index of the item at the specified point.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Item index if available; otherwise -1;</returns>
        public int HitTest(int x, int y)
        {
            //int index = IndexFromPoint(x, y);
            //return index >= 0 ? index : -1;
            int hitIndex = -1;

            if (Bounds.Contains(x, y))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (GetItemRectangle(i).Contains(x, y))
                    {
                        hitIndex = i;
                    }
                }
            }

            return hitIndex;
        }

        internal void DesignModeInvalidateHoverItem()
        {
            if (hoverIndex != -1)
            {
                Invalidate(GetItemRectangle(hoverIndex));
                hoverIndex = -1;
            }
        }
    }
}