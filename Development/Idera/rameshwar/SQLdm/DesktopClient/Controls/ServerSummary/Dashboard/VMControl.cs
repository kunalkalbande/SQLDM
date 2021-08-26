using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class VMControl : DashboardControl
    {
        public VMControl()
            : base(DashboardPanel.VM)
        {
            InitializeComponent();
        }
    }
}
