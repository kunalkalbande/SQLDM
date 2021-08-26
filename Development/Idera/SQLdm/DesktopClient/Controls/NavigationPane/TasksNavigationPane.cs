namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.DesktopClient.Dialogs.Notification;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.DesktopClient.Views.Tasks;

    public partial class TasksNavigationPane : UserControl
    {
        public TasksNavigationPane()
        {
            InitializeComponent();

            ApplicationController.Default.ActiveViewChanged += new EventHandler(ApplicationController_ActiveViewChanged);
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName) {
                case "TasksViewFilterOptionsPanelVisible":
                    ConfigureToggleFilterOptionsLinkLabel((bool)e.NewValue);
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

        private void currentViewHeaderStrip_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleCurrentViewPanelExpanded();
            }
        }

        private void ConfigureCurrentViewPanelExpanded()
        {
            if (Settings.Default.TasksNavigationPaneCurrentViewPanelExpanded)
            {
                toggleCurrentViewPanelButton.Image = Resources.HeaderStripSmallCollapse;
                currentViewPanel.Height = 130;
            }
            else
            {
                toggleCurrentViewPanelButton.Image = Resources.HeaderStripSmallExpand;
                currentViewPanel.Height = 19;
            }
        }

        private void ToggleCurrentViewPanelExpanded()
        {
            Settings.Default.TasksNavigationPaneCurrentViewPanelExpanded =
                !Settings.Default.TasksNavigationPaneCurrentViewPanelExpanded;
            ConfigureCurrentViewPanelExpanded();
        }

        private void TasksNavigationPane_Load(object sender, EventArgs e)
        {
            ConfigureCurrentViewPanelExpanded();

            ConfigureToggleFilterOptionsLinkLabel(Settings.Default.TasksViewFilterOptionsPanelVisible);

            Settings.Default.SettingChanging += new SettingChangingEventHandler(Settings_SettingChanging);
        }

        private void toggleFilterOptionsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings.Default.TasksViewFilterOptionsPanelVisible = !Settings.Default.TasksViewFilterOptionsPanelVisible;
        }

        private void notificationRulesLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            NotificationRulesDialog notificationRulesDialog = new NotificationRulesDialog(managementService);
            notificationRulesDialog.ShowDialog(ParentForm);
        }

        void ApplicationController_ActiveViewChanged(object sender, EventArgs e)
        {
            TasksView taskView = ApplicationController.Default.ActiveView as TasksView;
            if (taskView != null)
            {
                try
                {
                    switch (taskView.GetSelectedView())
                    {
                        case StandardTasksViews.Active:
                            activeTasksRadioButton.Checked = true;
                            break;
                        case StandardTasksViews.Completed:
                            completedTasksRadioButton.Checked = true;
                            break;
                        case StandardTasksViews.ByOwner:
                            byOwnerRadioButton.Checked = true;
                            break;
                        case StandardTasksViews.ByStatus:
                            byStatusRadioButton.Checked = true;
                            break;
                        case StandardTasksViews.Custom:
                            customRadioButton.Checked = true;
                            break;
                        case StandardTasksViews.ShowDefaultOrExisting:
                        case StandardTasksViews.None:
                        default:
                            break;
                    }
                }
                finally
                {
                }
            }
        }


        #region Radio buttons
        private void activeTasksRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (activeTasksRadioButton.Checked)
            {
                ApplicationController.Default.ShowTasksView(StandardTasksViews.Active);
            }
        }

        private void completedTasksRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (completedTasksRadioButton.Checked)
            {
                ApplicationController.Default.ShowTasksView(StandardTasksViews.Completed);
            }
        }

        private void byOwnerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (byOwnerRadioButton.Checked)
            {
                ApplicationController.Default.ShowTasksView(StandardTasksViews.ByOwner);
            }
        }

        private void byStatusRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (byStatusRadioButton.Checked)
            {
                ApplicationController.Default.ShowTasksView(StandardTasksViews.ByStatus);
            }
        }

        private void customRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked)
            {
                ApplicationController.Default.ShowTasksView(StandardTasksViews.Custom);
            }
        }

        #endregion

        public void UpdateUserTokenAttributes()
        {
            notificationRulesLabel.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
        }
    }
}