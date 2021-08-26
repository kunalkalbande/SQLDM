namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;
    using Dialogs;
    using Helpers;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.DesktopClient.Dialogs.Notification;
    using Idera.SQLdm.DesktopClient.Views.Alerts;
    using Properties;

    public partial class AlertsNavigationPane : UserControl
    {
        private bool activeViewChanging;

        public AlertsNavigationPane()
        {
            InitializeComponent();

            ApplicationController.Default.ActiveViewChanged += new EventHandler(ApplicationController_ActiveViewChanged);
            AdaptFontSize();
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "AlertsViewFilterOptionsPanelVisible":
                    ConfigureToggleFilterOptionsLinkLabel((bool)e.NewValue);
                    break;
                case "AlertsViewDetailsPanelExpanded":
                    ConfigureDetailsLink((bool)e.NewValue);
                    break;
            }
        }

        private void ConfigureToggleFilterOptionsLinkLabel(bool filterOptionsVisible)
        {
            if (filterOptionsVisible)
            {
                toggleFilterOptionsLinkLabel.Text = "Hide Filter Options";
            }
            else
            {
                toggleFilterOptionsLinkLabel.Text = "Show Filter Options";
            }
        }

        private void ConfigureDetailsLink(bool detailsVisible)
        {
            if (detailsVisible)
            {
                toggleDetailsLinkLabel.Text = "Hide Details";
            }
            else
            {
                toggleDetailsLinkLabel.Text = "Show Details";
            }
        }

        private void currentViewHeaderStrip_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleCurrentViewPanelExpanded();
            }
        }

        private void ConfigureCurrentViewPanelExpanded()
        {
            if (Settings.Default.AlertsNavigationPaneCurrentViewPanelExpanded)
            {
                toggleCurrentViewPanelButton.Image = Resources.HeaderStripSmallCollapse;
                currentViewPanel.Height = 270;
            }
            else
            {
                toggleCurrentViewPanelButton.Image = Resources.HeaderStripSmallExpand;
                currentViewPanel.Height = 19;
            }
        }

        private void ToggleCurrentViewPanelExpanded()
        {
            Settings.Default.AlertsNavigationPaneCurrentViewPanelExpanded =
                !Settings.Default.AlertsNavigationPaneCurrentViewPanelExpanded;
            ConfigureCurrentViewPanelExpanded();
        }

        private void AlertsNavigationPane_Load(object sender, EventArgs e)
        {
            ConfigureCurrentViewPanelExpanded();

            ConfigureToggleFilterOptionsLinkLabel(Settings.Default.AlertsViewFilterOptionsPanelVisible);
            ConfigureDetailsLink(Settings.Default.AlertsViewDetailsPanelExpanded);

            Settings.Default.SettingChanging += new SettingChangingEventHandler(Settings_SettingChanging);
        }

        void ApplicationController_ActiveViewChanged(object sender, EventArgs e)
        {
            AlertsView alertsView = ApplicationController.Default.ActiveView as AlertsView;
            if (alertsView != null)
            {
                try
                {
                    activeViewChanging = true;
                    switch (alertsView.GetView())
                    {
                        case StandardAlertsViews.Active:
                            activeAlertsRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.Custom:
                            customFilterRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.BlockedSessions:
                            blokcedProcessesRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.BombedJobs:
                            bombedJobsRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.ByInstance:
                            byInstanceRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.ByMetric:
                            byMetricRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.BySeverity:
                            bySeverityRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.OldestOpen:
                            oldestOpenRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.Fragmentation:
                            tableReorgRadioButton.Checked = true;
                            break;
                        case StandardAlertsViews.QueryMonitor:
                            worstPerformingRadioButton.Checked = true;
                            break;
                    }
                } finally
                {
                    activeViewChanging = false;
                }
            }
        }

        #region Radio Events

        private void activeAlertsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (activeAlertsRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Active, filter);
            }
        }

        private void bySeverityRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (bySeverityRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.BySeverity, filter);
            }
        }

        private void byInstanceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (byInstanceRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.ByInstance, filter);
            }
        }

        private void byMetricRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (byMetricRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.ByMetric, filter);
            }
        }

        #endregion

        #region Link Events

        private void toggleFilterOptionsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings.Default.AlertsViewFilterOptionsPanelVisible = !Settings.Default.AlertsViewFilterOptionsPanelVisible;
        }

        private void toggleDetailsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings.Default.AlertsViewDetailsPanelExpanded = !Settings.Default.AlertsViewDetailsPanelExpanded;
        }

        private void notificationRulesLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            NotificationRulesDialog notificationRulesDialog = new NotificationRulesDialog(managementService);
            notificationRulesDialog.ShowDialog(ParentForm);
        }
            
        #endregion

        private void bombedJobsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (bombedJobsRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.Metric = Metric.BombedJobs;
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.BombedJobs, filter);
            }
        }

        private void blokcedProcessesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (blokcedProcessesRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.Metric = Metric.BlockedSessions;
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.BlockedSessions, filter);
            }
        }

        private void oldestOpenRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oldestOpenRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.Metric = Metric.OldestOpenTransMinutes;
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.OldestOpen, filter);
            }
        }

        private void worstPerformingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (worstPerformingRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.Metric = Metric.QueryMonitorStatus;
                filter.ActiveOnly = true;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.QueryMonitor, filter);
            }
        }

        private void tableReorgRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tableReorgRadioButton.Checked && !activeViewChanging)
            {
                AlertFilter filter = new AlertFilter();
                filter.ActiveOnly = true;
                filter.Metric = Metric.ReorganisationPct;
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Fragmentation, filter);
            }
        }

        private void customFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customFilterRadioButton.Checked && !activeViewChanging)
            {
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Custom, new AlertFilter());
            }
        }

        private void defaultAlertConfigurationLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                using (AlertTemplateDialog alertTemplateDialog = new AlertTemplateDialog())
                {
                    alertTemplateDialog.ShowDialog(this);
                }
                //using (AlertConfigurationDialog alertConfigDialog = new AlertConfigurationDialog(0, true))
                //{
                //    alertConfigDialog.ShowDialog(this);
                //}
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                                                ex);
            }

        }

        public void UpdateUserTokenAttributes()
        {
            defaultAlertConfigurationLink.Visible = 
                notificationRulesLabel.Visible =
                    ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
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
