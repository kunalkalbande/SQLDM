using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class StaticViewPropertiesDialog : Form
    {
        private const string NewViewDialogText = "Create View";
        private const string UserViewPropertiesDialogText = "My View - {0}";

        private StaticUserView userView;

        public StaticViewPropertiesDialog() : this(null)
        {
        }

        public StaticViewPropertiesDialog(StaticUserView userView)
        {
            this.userView = userView;

            InitializeComponent();
            InitializeForm();
            AdaptFontSize();
        }

        public UserView UserView
        {
            get { return userView; }
        }

        public void InitializeForm()
        {
            if (userView == null)
            {
                Text = NewViewDialogText;
                okButton.Text = "Create";
            }
            else
            {
                Text = string.Format(UserViewPropertiesDialogText, userView.Name);
                viewNameTextBox.Text = userView.Name;

                foreach (int instanceId in userView.Instances)
                {
                    if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                    {
                        MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[instanceId];
                        viewInstancesListBox.Items.Add(instance);
                    }
                }
            }

            loadMonitoredInstancesBackgroundWorker.RunWorkerAsync();
        }

        private void loadMonitoredInstancesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "LoadMonitoredInstances";

            e.Result =
                RepositoryHelper.GetMonitoredSqlServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                        true);
        }

        private void loadMonitoredInstancesBackgroundWorker_RunWorkerCompleted(object sender,
                                                                               RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                IList<MonitoredSqlServer> monitoredInstances = e.Result as IList<MonitoredSqlServer>;

                if (monitoredInstances != null)
                {
                    monitoredInstancesListBox.BeginUpdate();
                    monitoredInstancesListBox.Items.Clear();

                    foreach (MonitoredSqlServer instance in monitoredInstances)
                    {
                        if (ApplicationModel.Default.UserToken.GetServerPermission(instance.Id) != PermissionType.None)
                        {
                            if (userView == null || !userView.Instances.Contains(instance.Id))
                            {
                                monitoredInstancesListBox.Items.Add(instance);
                            }
                        }
                    }

                    monitoredInstancesListBox.EndUpdate();
                }
            }
            else
            {
                monitoredInstancesStatusLabel.Visible = true;
                ApplicationMessageBox.ShowError(this,
                                                "Unable to load the monitored SQL Server instances from the SQLDM Repository.",
                                                e.Error);
            }
        }

        private void monitoredInstancesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void groupInstancesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void monitoredInstancesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelectedInstances();
        }

        private void groupInstancesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RemoveSelectedInstances();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddSelectedInstances();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedInstances();
        }

        private void AddSelectedInstances()
        {
            List<int> addedIndices = new List<int>();

            foreach (int selectedIndex in monitoredInstancesListBox.SelectedIndices)
            {
                addedIndices.Insert(0, selectedIndex);
                viewInstancesListBox.Items.Add(monitoredInstancesListBox.Items[selectedIndex]);
            }

            foreach (int removedIndex in addedIndices)
            {
                monitoredInstancesListBox.Items.RemoveAt(removedIndex);
            }

            UpdateForm();
        }

        private void RemoveSelectedInstances()
        {
            List<int> removedIndices = new List<int>();

            foreach (int selectedIndex in viewInstancesListBox.SelectedIndices)
            {
                removedIndices.Insert(0, selectedIndex);
                monitoredInstancesListBox.Items.Add(viewInstancesListBox.Items[selectedIndex]);
            }

            foreach (int removedIndex in removedIndices)
            {
                viewInstancesListBox.Items.RemoveAt(removedIndex);
            }

            UpdateForm();
        }

        private void UpdateForm()
        {
            addButton.Enabled = monitoredInstancesListBox.SelectedItems.Count > 0;
            removeButton.Enabled = viewInstancesListBox.SelectedItems.Count > 0;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string viewName = viewNameTextBox.Text.Trim();

            if (viewName.Length == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "The view name cannot be empty. Please specify a valid view name.");
                DialogResult = DialogResult.None;
                return;
            }

            // If the view is null, add a new static user view
            if (userView == null)
            {
                UserView existingView;

                if (string.Compare(viewName, "All Servers", true) == 0 ||
                    Settings.Default.ActiveRepositoryConnection.UserViews.TryGetValue(viewName, out existingView))
                {
                    ApplicationMessageBox.ShowInfo(this, "A view with the same name already exists. Please specify a unique name.");
                    DialogResult = DialogResult.None;
                    return;
                }
                else
                {
                    userView = new StaticUserView(viewName);
                    
                    foreach (MonitoredSqlServer instance in viewInstancesListBox.Items)
                    {
                        userView.AddInstance(instance.Id);
                    }
                    
                    Settings.Default.ActiveRepositoryConnection.UserViews.Add(userView);
                }
            }
            else
            {
                if (string.Compare(userView.Name, viewName, true) != 0)
                {
                    UserView existingView;

                    if (
                        Settings.Default.ActiveRepositoryConnection.UserViews.TryGetValue(viewName,
                                                                                             out existingView))
                    {
                        ApplicationMessageBox.ShowInfo(this,
                                                       "A view with the same name already exists. Please specify a unique name.");
                        DialogResult = DialogResult.None;
                        return;
                    }
                    else
                    {
                        userView.Name = viewName;
                    }
                }
                else
                {
                    userView.Name = viewName;
                }

                userView.ClearInstances();
                foreach (MonitoredSqlServer instance in viewInstancesListBox.Items)
                {
                    userView.AddInstance(instance.Id);
                }
            }
        }


        protected override void OnHelpButtonClicked(System.ComponentModel.CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureCustomView);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureCustomView);
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