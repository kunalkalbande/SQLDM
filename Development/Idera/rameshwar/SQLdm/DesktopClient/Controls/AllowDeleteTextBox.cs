using System;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public class AllowDeleteTextBox : TextBox
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
		private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AllowDeleteTextBox");

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
