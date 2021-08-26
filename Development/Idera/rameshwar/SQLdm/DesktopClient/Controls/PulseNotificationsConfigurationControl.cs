using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Idera.Newsfeed.Plugins.UI.Controls;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class PulseNotificationsConfigurationControl : PulseConfigurationControlBase
    {
        private bool _suppressToggleCheck = false;
        private bool _suppressItemCheck = false;

        public PulseNotificationsConfigurationControl()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Choose the status updates you want to see"; }
        }

        public override string Description
        {
            get { return "Select which SQLDM alert notifications should display as status updates in your Newsfeed."; }
        }

        public override void Initialize()
        {
            if (_initializeWorker.IsBusy) return;

            _noRulesLabel.Visible = false;
            _statusCircle.Active = true;
            _statusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            _statusPanel.Visible = true;
            _initializeWorker.RunWorkerAsync();
        }

        private void _initializeWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            e.Result = managementService.GetNotificationRules();
        }

        private void _initializeWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            _statusPanel.Visible = false;
            _statusCircle.Active = false;

            if (e.Error == null)
            {
                IList<NotificationRule> rules = e.Result as IList<NotificationRule>;

                if (rules != null && rules.Count > 0)
                {
                    foreach (NotificationRule rule in rules)
                    {
                        int index = _notificationRulesListBox.Items.Add(rule);

                        if (GetPulseDestination(rule) != null)
                        {
                            _notificationRulesListBox.SetItemCheckState(index, CheckState.Checked);
                        }
                    }
                }
                else
                {
                    _noRulesLabel.Visible = true;
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while loading the notification rules.", e.Error);
            }
        }

        public override bool Save()
        {
            _initializeWorker.CancelAsync();

            try
            {
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                foreach (NotificationRule rule in _notificationRulesListBox.Items)
                {
                    NotificationDestinationInfo destination = GetPulseDestination(rule);

                    if (_notificationRulesListBox.CheckedItems.Contains(rule))
                    {
                        if (destination == null)
                        {
                            rule.Destinations.Add(new PulseDestination());
                            managementService.UpdateNotificationRule(rule);
                        }
                    }
                    else if (destination != null)
                    {
                        rule.Destinations.Remove(destination);
                        managementService.UpdateNotificationRule(rule);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while saving the Newsfeed notification rules.", ex);
                return false;
            }
        }

        private NotificationDestinationInfo GetPulseDestination(NotificationRule rule)
        {
            if (rule == null || ApplicationModel.Default.PulseProvider == null) return null;
            
            foreach (NotificationDestinationInfo destination in rule.Destinations)
            {
                if (destination.ProviderID == ApplicationModel.Default.PulseProvider.Id)
                {
                    return destination;
                }
            }

            return null;
        }

        private void _addRuleButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                SqlConnectionInfo ci = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                IManagementService managementService = ManagementServiceHelper.GetDefaultService(ci);

                MetricDefinitions metricDefinitions = new MetricDefinitions(false, false, true);
                metricDefinitions.Load(ci.ConnectionString);

                List<NotificationProviderInfo> providers = RepositoryHelper.GetNotificationProvider(ci, null);
                using (NotificationRuleDialog dialog = new NotificationRuleDialog(managementService, metricDefinitions, providers, false))
                {
                    dialog.NotificationRule = new NotificationRule();
                    foreach (NotificationProviderInfo npi in providers)
                    {
                        if (npi is PulseNotificationProviderInfo)
                        {
                            NotificationDestinationInfo ndi = (NotificationDestinationInfo)Activator.CreateInstance(npi.DestinationType);
                            ndi.Provider = (NotificationProviderInfo)npi.Clone();
                            ndi.Enabled = true;
                            dialog.NotificationRule.Destinations.Add(ndi);
                        }
                    }
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        int index = _notificationRulesListBox.Items.Add(dialog.NotificationRule);
                        _notificationRulesListBox.SetItemChecked(index, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while adding the notification rule.", ex);
            }
        }

        private void _selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleCheck) return;
            ToggleAll(_selectAllCheckBox.Checked);
        }

        private void ToggleAll(bool selected)
        {
            _suppressItemCheck = true;

            for (int i = 0; i < _notificationRulesListBox.Items.Count; i++)
            {
                _notificationRulesListBox.SetItemChecked(i, selected);
            }

            _suppressItemCheck = false;
        }

        private void _notificationRulesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressItemCheck) return;

            _suppressToggleCheck = true;
            _selectAllCheckBox.Checked = false;
            _suppressToggleCheck = false;
        }

        private void _editRuleButton_Click(object sender, EventArgs e)
        {
            if (_notificationRulesListBox.SelectedItems.Count == 0) return;

            NotificationRule selectedRule = _notificationRulesListBox.SelectedItems[0] as NotificationRule;
            bool isItemChecked = _notificationRulesListBox.CheckedItems.Contains(selectedRule);

            if (selectedRule == null) return;

            try
            {
                SqlConnectionInfo ci = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;

                IManagementService managementService = ManagementServiceHelper.GetDefaultService(ci);

                MetricDefinitions metricDefinitions = new MetricDefinitions(false, false, true);
                metricDefinitions.Load(ci.ConnectionString);

                List<NotificationProviderInfo> providers = RepositoryHelper.GetNotificationProvider(ci, null);
                using (NotificationRuleDialog dialog = new NotificationRuleDialog(managementService, metricDefinitions, providers, false))
                {
                    dialog.NotificationRule = (NotificationRule)selectedRule.Clone();
                    
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        _notificationRulesListBox.Items.Remove(selectedRule);
                        int index = _notificationRulesListBox.Items.Add(dialog.NotificationRule);
                        _notificationRulesListBox.SetItemChecked(index, isItemChecked);
                        _notificationRulesListBox.SelectedItems.Add(dialog.NotificationRule);
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while editing the selected notification rule.", exception);
            }
        }

        private void _deleteRuleButton_Click(object sender, EventArgs e)
        {
            if (_notificationRulesListBox.SelectedItems.Count == 0) return;

            if (ApplicationMessageBox.ShowQuestion(this, "Are you sure you want to delete the selected notification rule?") != DialogResult.Yes)
            {
                return;
            }

            NotificationRule selectedRule = _notificationRulesListBox.SelectedItems[0] as NotificationRule;

            if (selectedRule == null) return;

            try
            {
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                managementService.DeleteNotificationRule(selectedRule.Id);
                _notificationRulesListBox.Items.Remove(selectedRule);
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while deleting the selected notification rule.", exception);
            }
        }

        private void _notificationRulesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editRuleButton.Enabled = _deleteRuleButton.Enabled = _notificationRulesListBox.SelectedItems.Count > 0;
        }
    }
}
