//------------------------------------------------------------------------------
// <copyright file="LinkLabelRenderer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using System.Xml;
    using System;

    public class LinkLabelRenderer : IDisposable
    {
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(LinkLabelRenderer));

        private Color linkColor;
        private Color textColor;
        private Font normalFont;
        private Font linkFont;
        private Stack<DrawingState> stack;
        private DrawingState state;
        private bool makeLinksUnderlined;
        private bool makeLinksBold;
        private StringFormat stringFormat;
        
        public LinkLabelRenderer(Font normalFont)
        {
            makeLinksUnderlined = true;
            makeLinksBold = false;

            this.normalFont = normalFont;
            LinkFont = null;
            
            stack = new Stack<DrawingState>();
            state = new DrawingState();

            textColor = SystemColors.ControlText;
            linkColor = SystemColors.HotTrack;        
    
            stringFormat = new StringFormat(StringFormat.GenericTypographic);
            stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        }          

        public void Dispose()
        {
            if (stringFormat != null)
                stringFormat.Dispose();

            if (linkFont != null)
                linkFont.Dispose();
        }

        public Color LinkColor
        {
            get { return linkColor; }
            set { linkColor = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public Font NormalFont
        {
            get { return normalFont; }
            set { normalFont = value; }
        }

        public bool LinksBold
        {
            get { return makeLinksBold;  }
            set { makeLinksBold = value; }
        }

        public bool LinksUnderlined
        {
            get { return makeLinksUnderlined; }
            set { makeLinksUnderlined = value; }
        }

        private FontStyle GetLinkFontStyle()
        {
            FontStyle style = FontStyle.Regular;

            if (makeLinksBold)
            {
                style |= FontStyle.Bold;
            }
            if (makeLinksUnderlined)
            {
                style |= FontStyle.Underline;
            }
            return style;
        }

        public Font LinkFont
        {
            get
            {
                if (this.linkFont == null)
                {
                    this.linkFont = new Font(normalFont, GetLinkFontStyle());
                }
                return linkFont;
            }
            set
            {
                if (linkFont != null)
                {
                    if (value == null && normalFont != null)
                        linkFont.Dispose();
                }
                if (value == null)
                    linkFont = new Font(normalFont, GetLinkFontStyle());
                
                linkFont = value;
            }
        }
                
        public void Draw(Graphics graphics, string text, Rectangle bounds)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<document>").Append(text).Append("</document>");

            state = new DrawingState();
            state.font = NormalFont;
            state.color = TextColor;
            stack.Push(state);

            PointF location = bounds.Location;

            try
            {
                using (XmlTextReader reader = new XmlTextReader(xml.ToString(), XmlNodeType.Element, null))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                // push the current state
                                stack.Push(state);
                                if (reader.Name.ToLowerInvariant().Equals("a"))
                                {
                                    // setup for drawing an anchor
                                    state = new DrawingState();
                                    state.font = LinkFont;
                                    state.color = LinkColor;
                                }
                                break;
                            case XmlNodeType.Text:
                                string value = reader.Value;
                                // TextRenderer is easier to use and the output looks better but does not seem to work
                                // well on Winders 2000 (text is blank).

                                using (Brush brush = new SolidBrush(state.color))
                                {
                                    if (Application.RenderWithVisualStyles)
                                    {
                                        Size size =
                                            TextRenderer.MeasureText(graphics, value, state.font, bounds.Size, TextFormatFlags.NoPadding);
                                        TextRenderer.DrawText(graphics, value, state.font, Point.Round(location), state.color, TextFormatFlags.NoPadding);
                                        location.X += size.Width;
                                    }
                                    else
                                    {
                                        SizeF size = graphics.MeasureString(value, state.font, bounds.Width, stringFormat);
                                        graphics.DrawString(value, state.font, brush, location, stringFormat);
                                        location.X += size.Width;
                                    }
                                }
                                break;
                            case XmlNodeType.EndElement:
                                // revert to previous state
                                state = stack.Pop();
                                break;
                        }
                    }
                }
            } 
            catch (Exception e)
            {
                Log.Error("Error rendering label: " + text, e);
            }

            stack.Clear();
        }
        
        struct DrawingState
        {
            internal Font font;
            internal Color color;
        }
        
    }
}
