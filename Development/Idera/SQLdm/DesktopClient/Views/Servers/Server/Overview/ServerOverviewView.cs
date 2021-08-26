using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal partial class ServerOverviewView : View
    {
        private int instanceId;
        private static readonly object updateLock = new object();
        private Control focused = null;

        public ServerOverviewView(int instanceId)
        {
            this.instanceId = instanceId;

            InitializeComponent();
        }

        #region Splitter Focus

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl != null ? focusedControl : controls[0];
        }

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void splitContainer2_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer2_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion
    }
}