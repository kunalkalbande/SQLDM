using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class AnalysisViewControl : DashboardControl
    {
        ViewContainer vHost = null;
        public AnalysisViewControl(ViewContainer vHost) : base(DashboardPanel.AnalysisView)
        {
            this.vHost = vHost;
            InitializeComponent();
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);
            DefaultScreenAnalysisTab defaultAnalysisView = new DefaultScreenAnalysisTab(baseView.InstanceId, vHost, true);
            panelAnalysisView.Controls.Add(defaultAnalysisView);
        }

        private void panelAnalysisView_MouseClick(object sender, MouseEventArgs e)
        {
            if (designMode)
            {
                DashboardControl_MouseClick(sender, e);
                return;
            }
        }
    }
}