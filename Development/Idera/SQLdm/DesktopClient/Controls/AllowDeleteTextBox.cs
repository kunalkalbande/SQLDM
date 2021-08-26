using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public class AllowDeleteTextBox : TextBox
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
		private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AllowDeleteTextBox");

        private Color backColor = ColorTranslator.FromHtml(DarkThemeColorConstants.TextBoxBackColor);
        private Color foreColor = ColorTranslator.FromHtml(DarkThemeColorConstants.TextBoxForeColor);
        private Color borderColor = Color.Transparent;

        public AllowDeleteTextBox()
        {
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (Settings.Default.ColorScheme == "Dark")
            {
                if (Enabled)
                {
                    foreColor = ColorTranslator.FromHtml(DarkThemeColorConstants.FocusedTextBoxForeColor);
                    borderColor = ColorTranslator.FromHtml(DarkThemeColorConstants.FocusedTextBoxBorderColor);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (Settings.Default.ColorScheme == "Dark")
            {
                foreColor = ColorTranslator.FromHtml(DarkThemeColorConstants.TextBoxForeColor);
                borderColor = Color.Transparent;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case 0xf:
                    if (Settings.Default.ColorScheme == "Dark")
                    {
                        BorderStyle = BorderStyle.FixedSingle;

                        if (!Enabled)
                        {
                            backColor = ColorTranslator.FromHtml(DarkThemeColorConstants.DisabledTextBoxBackColor);
                            foreColor = ColorTranslator.FromHtml(DarkThemeColorConstants.DisabledTextBoxForeColor);
                        }

                        BackColor = backColor;
                        ForeColor = foreColor;

                        Graphics g = CreateGraphics();

                        g.DrawRectangle(new Pen(borderColor), ClientRectangle);
                        g.FillRectangle(new SolidBrush(backColor), ClientRectangle);
                        ControlPaint.DrawBorder(g, ClientRectangle, borderColor, 1, ButtonBorderStyle.Solid, borderColor, 1, ButtonBorderStyle.Solid, borderColor, 1, ButtonBorderStyle.Solid, borderColor, 1, ButtonBorderStyle.Solid);
                        SizeF stringSize = g.MeasureString(Text, this.Font);
                        g.DrawString(Text, this.Font, new SolidBrush(foreColor), new Point(0, (this.Height / 3) - ((int)stringSize.Height / 3)));
                    }
                    else
                    {
                        BackColor = Color.White;
                        ForeColor = Color.Black;
                    }
                    break;
                default:
                    break;
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public override bool PreProcessMessage(ref Message msg)
        {
			uint wParan = 0;
            try
            {
                wParan = (uint)msg.WParam;
            }
            catch (Exception ex)
            {
                LOG.Error("Error in casting Message.WParam : " + ex);
                msg.WParam = new IntPtr();
            }

            Keys keyCode = (Keys)wParan & Keys.KeyCode;
			
            if ((msg.Msg == WM_KEYDOWN || msg.Msg == WM_KEYUP)
                 && keyCode == Keys.Delete)
            {
                if (SelectionStart < TextLength)
                {
                    int selectionStart = SelectionStart;
                    Text = SelectionLength > 0
                               ? Text.Remove(SelectionStart, SelectionLength)
                               : Text.Remove(SelectionStart, 1);
                    SelectionStart = selectionStart;
                }
                
                return true;
            }

            return base.PreProcessMessage(ref msg);
        }
    } 
}
