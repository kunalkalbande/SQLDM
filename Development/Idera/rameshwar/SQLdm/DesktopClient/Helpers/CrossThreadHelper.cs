using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    public static class CrossThreadHelper
    {
        static public void UIThread(this Control control, MethodInvoker code)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(code);
                return;
            }
            code.Invoke();
        }
    }
}

