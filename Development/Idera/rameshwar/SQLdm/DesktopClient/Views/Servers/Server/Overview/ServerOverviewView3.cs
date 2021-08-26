using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal partial class ServerOverviewView3 : Idera.SQLdm.DesktopClient.Views.View
    {
        private int instanceId;
        private static readonly object updateLock = new object();
        private Control focused = null;

        public ServerOverviewView3(int instanceId)
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

