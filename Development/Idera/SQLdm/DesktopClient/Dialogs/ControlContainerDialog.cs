using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class ControlContainerDialog : BaseDialog
    {
        private bool hideOnDeactivate = true;

        public ControlContainerDialog()
        {
            this.DialogHeader = "ControlContainerDialog";
            InitializeComponent();
        }

        public bool HideOnDeactivate
        {
            get { return hideOnDeactivate; }
            set { hideOnDeactivate = value; }
        }

        public Control Control
        {
            get { return controlContainerPanel.Controls.Count == 1 ? controlContainerPanel.Controls[0] : null; }
        }

        public void SetControl(Control control)
        {
            FreeControl();

            if (control != null)
            {
                control.Size = Size;
                control.Dock = DockStyle.Fill;
                controlContainerPanel.Controls.Add(control);
            }
        }

        public void FreeControl()
        {
            controlContainerPanel.Controls.Clear();
        }

        private void ControlContainerDialog_Deactivate(object sender, System.EventArgs e)
        {
            if (HideOnDeactivate)
            {
                Hide();
            }
        }
    }
}