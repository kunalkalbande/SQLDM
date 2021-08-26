using System;
using System.Drawing;
using System.Windows.Forms;

using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class DashboardPanelDetails : UserControl
    {
        private DashboardPanel? dashboardPanel = null;
        private DashboardControl dashboardControl;

        public DashboardPanelDetails()
        {
            InitializeComponent();

            // Autoscale font size.
            AdaptFontSize();
        }

        public void ShowPanel(ServerBaseView baseView, DashboardPanel panel, ServerSummaryHistoryData history, ViewContainer vHost)
        {
            if (dashboardPanel != panel)
            {
                if (dashboardControl != null)
                {
                    mainTableLayoutPanel.Controls.Remove(dashboardControl);
                    dashboardControl.MouseDown -= dashboardControl_MouseDown;
                    dashboardControl.MouseClick -= Control_MouseClick;
                    dashboardControl.Dispose();
                }

                dashboardPanel = panel;
                dashboardControl = DashboardHelper.GetNewDashboardControl(panel, vHost);
                mainTableLayoutPanel.Controls.Add(dashboardControl, 0, mainTableLayoutPanel.RowCount);
                mainTableLayoutPanel.SetColumnSpan(dashboardControl, 2);
                dashboardControl.Margin = new Padding(3);
                dashboardControl.Size = new Size(256, 75);
                dashboardControl.MouseDown += dashboardControl_MouseDown;
                dashboardControl.MouseClick += Control_MouseClick;
                dashboardControl.SetPosition(-1, -1);
                dashboardControl.Dock = DockStyle.Fill;

                panelNameLabel.Text = dashboardControl.Caption;
                descriptionLabel.Text = DashboardHelper.GetDashboardDescription(panel);
            }

            dashboardControl.Initialize(baseView, history);
            // show vm data if available
            MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[baseView.InstanceId];
            dashboardControl.ShowVMData(instanceObject.Instance.IsVirtualized);
            // load the current data to the charts
            dashboardControl.ConfigureDataSource();
            dashboardControl.SetDesignMode(true);
            Visible = true;
        }

        public void ConfigureControlDataSource()
        {
            if (dashboardControl != null)
            {
                dashboardControl.ConfigureDataSource();
            }
        }

        private void ShowControlHelp()
        {
            if (dashboardPanel.HasValue)
            {
                string topic = DashboardHelper.GetDashboardHelpTopic(dashboardPanel.Value);

                ApplicationHelper.ShowHelpTopic(topic);
            }
        }

        private void dashboardControl_MouseDown(object sender, MouseEventArgs e)
        {
            dashboardControl.DoDragDrop(dashboardControl, DragDropEffects.Link);
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            Visible = false;
        }

        private void helpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowControlHelp();
        }

        private void DashboardPanelDetails_MouseLeave(object sender, EventArgs e)
        {
            Visible = false;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
