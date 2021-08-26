using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    class BaseDialog : Form
    {
        public BaseDialog()
        {
            Text = "Title";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ShowInTaskbar = false;
            HelpButton = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
        }
    }
}
