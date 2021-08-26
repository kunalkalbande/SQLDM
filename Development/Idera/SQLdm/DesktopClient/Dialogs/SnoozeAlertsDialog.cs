//------------------------------------------------------------------------------
// <copyright file="SnoozeAlertsDialog.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Helpers;
    using Common.Configuration;
    using Common.Events;
    using Common.Services;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Objects;
    using Properties;
    using Resources = Properties.Resources;

    public partial class SnoozeAlertsDialog : Form
    {
        private const string UNSNOOZE_MESSAGE = "You are about to resume alerts for the selected metrics. Alerts raised for specific metrics are reflected in the state of your monitored server(s). Do you wish to continue?";
        private const string SNOOZE_ALL_MESSAGE = "You can snooze all alerts for a specified period of time (10 minutes minimum). If snoozed, alerts are cleared and no longer impact the state of your monitored server(s) until the snooze period is over.";
        private const string SNOOZE_MESSAGE = "An alert can be snoozed for a specified period of time (10 minute minimum). If an alert is snoozed, it will be cleared and will no longer impact the state of your monitored server until the snooze period is over.";
        public enum SnoozeAction
        {
            Snooze,
            Unsnooze
        }

        private int instanceId;
        private IList<int> serverInstanceList = null; 
        private int? metricId;
        private string tagName = String.Empty;
        private SnoozeAction action;
        private SnoozeInfo result;
        private MetricDescription? metricDescription;
        private bool settingCheckBox = false;
        private AlertConfiguration unsnoozeAlertConfig;

        private readonly String alertDescription = String.Empty;

        /// <summary>
        /// SnoozeAlertsDialog Constructor.
        /// </summary>
        /// <param name="instanceId">The instance ID on which the snooze alert operation will be performed.</param>
        /// <param name="metricId">The ID of the metric to snooze</param>
        /// <param name="action">The action to perform in the metric</param>
        /// <param name="alertDescription">The alert description, this information is used to audit the operation.</param>
        private SnoozeAlertsDialog(int instanceId, int? metricId, SnoozeAction action, String alertDescription)
        {
            InitializeComponent();
            this.instanceId = instanceId;
            this.metricId = metricId;
            this.action = action;
            this.alertDescription = alertDescription;

            this.ShrinkDialog(action == SnoozeAction.Snooze ? true : false);

            // Autoscale font size.
            AdaptFontSize();
        }

        /// <summary>
        /// Resizes the dialog according to if it was called for Snooze or Unsnooze
        /// </summary>
        /// <param name="shrink"></param>
        private void ShrinkDialog(bool shrink)
        {
            if(shrink)
            {
                this.Height = 200;
            }
            else
            {
                this.Height = 361;
            }
        }

        /// <summary>
        /// SnoozeAlertsDialog Constructor.
        /// </summary>
        /// <param name="serverInstanceList">The List of Servers ID on which the snooze alert operation will be performed.</param>
        /// <param name="metricId">The ID of the metric to snooze</param>
        /// <param name="action">The action to perform in the metric</param>
        private SnoozeAlertsDialog(IList<int> serverInstanceList, int? metricId, SnoozeAction action, string tagNameSelected)
            : this(-1, metricId, action, String.Empty)
        {
            this.serverInstanceList = new List<int>(serverInstanceList);
            this.tagName = tagNameSelected;
        }

        /// <summary>
        /// Returns the metric description that match with the Metric ID.
        /// </summary>
        /// <param name="metricId">The ID of the Metric Description to found.</param>
        /// <returns>The metric description that match with the Metric ID.</returns>
        private  MetricDescription? GetMetricDescription(int metricId)
        {
            MetricDefinitions metricDefinitions = ApplicationModel.Default.MetricDefinitions;
            MetricDescription? metricDescription = null;

            if (metricDefinitions != null && metricId > -1)
            {
                // No negative metric ID.
                metricDescription = metricDefinitions.GetMetricDescription(metricId);
            }

            return metricDescription;
        }

        private MetricDescription? GetMetricDescription()
        {
            metricDescription = GetMetricDescription(metricId.HasValue? metricId.Value: -1);

            return metricDescription;
        }

        /// <summary>
        /// SnoozeAlertsDialog Constructor.
        /// </summary>
        /// <param name="instanceId">The instance ID on which the snooze alert operation will be performed.</param>
        /// <param name="metricId">The ID of the metric to snooze</param>
        /// <param name="action">The action to perform in the metric</param>
        private SnoozeAlertsDialog(int instanceId, int? metricId, SnoozeAction action): this(instanceId, metricId, action, String.Empty)
        {
            // No op.
        }

        public static SnoozeInfo SnoozeAllAlerts(IWin32Window owner, int instanceId, SnoozeAction action)
        {
            using (SnoozeAlertsDialog dialog = new SnoozeAlertsDialog(instanceId,null,action))
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    if (action == SnoozeAction.Snooze)
                    {
                        ApplicationController.Default.FireBackgroundRefreshCompleted();
                    }
                    return dialog.result;
                }
                return null;
            }
        }


        /// <summary>
        /// Static method to display the all server snooze alert dialog.
        /// </summary>
        /// <param name="owner">The form on which displays the snooze alert</param>
        /// <param name="serversList">The list instance ID of monitored SQL server</param>
        /// <param name="action">The description for the alert, used to audit the snooze operation</param>
        /// <returns></returns>
        public static SnoozeInfo SnoozeAllServerAlerts(IWin32Window owner, IList<int> serversList, SnoozeAction action, string tagNameSelected)
        {
            using (SnoozeAlertsDialog dialog = new SnoozeAlertsDialog(serversList, null, action, tagNameSelected))
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    if (action == SnoozeAction.Snooze)
                    {
                        ApplicationController.Default.FireBackgroundRefreshCompleted();
                    }
                    return dialog.result;
                }
                return null;
            }

        }

        /// <summary>
        /// The static method to display the Snooze Alert dialog.
        /// </summary>
        /// <param name="owner">The form on which displays the snooze alert</param>
        /// <param name="instanceId">The monitored SQL server instance ID</param>
        /// <param name="metricId">The metric ID in which will perform the snooze operation</param>
        /// <param name="action">The action to perform snooze / un-snooze</param>
        /// <param name="alertDescription">The description for the alert, used to audit the snooze operation</param>
        /// <returns></returns>
        public static SnoozeInfo SnoozeAlert(IWin32Window owner, int instanceId, int metricId, SnoozeAction action, String alertDescription)
        {
            using (SnoozeAlertsDialog dialog = new SnoozeAlertsDialog(instanceId, metricId, action, alertDescription))
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return dialog.result;
                }
                return null;
            }
        }

        public static DialogResult UnSnoozeAlerts(IWin32Window owner, AlertConfiguration alertConfig)
        {
            using (SnoozeAlertsDialog dialog = new SnoozeAlertsDialog(alertConfig.InstanceID, -1, SnoozeAction.Unsnooze))
            {
                dialog.unsnoozeAlertConfig = alertConfig;
                return dialog.ShowDialog(owner);
            }
        }

        private void SnoozeAlertsDialog_Load(object sender, EventArgs e)
        {
            //if we have a list of instances
            if(serverInstanceList != null)
            {
                Text = String.Format("{0} All Alerts - {1} ", action == SnoozeAction.Snooze ? "Snooze" : "Resume",tagName);
            }
            else
            {
                MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[instanceId];
                Text = String.Format("{0} {1} - {2}",
                                    action == SnoozeAction.Snooze ? "Snooze" : "Resume",
                                    metricId == null ? "Alerts" : "Alert",
                                    instance.InstanceName);
            }
           

            if (metricId != null)
            {
                this.metricDescription = this.GetMetricDescription();
            }

            if (action == SnoozeAction.Snooze)
                ConfigureSnooze();
            else
                ConfigureUnsnooze();
        }

        private void ConfigureUnsnooze()
        {
            unsnoozeImage.Image = Resources.AlarmClockStart32x32;
            unsnoozeInfoBox.Text = UNSNOOZE_MESSAGE;
            DataTable data;

            contentStackPanel.ActiveControl = unsnoozeAlertContentPanel;
            okButton.Text = "Yes";
            cancelButton.Text = "No";

            if (serverInstanceList == null)
            {
                data = RepositoryHelper.GetSnoozedAlerts(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,instanceId);
            }
            else
            {
                data = RepositoryHelper.GetAllTableSnoozedAlerts(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, serverInstanceList);
            }

            DataColumn selectedCol = data.Columns.Add("Selected", typeof(bool));

            //Select all metrics by defauld  
            resumeToggleCheckBox.Checked = true;
            foreach (DataRow row in data.Rows)
            {
                row[selectedCol] = true;
            }

            snoozedAlertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            snoozedAlertsGrid.DataSource = data;
            snoozedAlertsGrid.Visible = true;
        }

        private void ConfigureSnooze()
        {
            if (metricId == null)
            {
                snoozeInfoBox.Text = SNOOZE_ALL_MESSAGE;
            }
            else
            {
                string name = metricDescription.HasValue ? metricDescription.Value.Name : String.Empty;

                if (!String.IsNullOrEmpty(name))
                {
                    name = String.Format("for the '{0}' metric ", name);
                }

                snoozeInfoBox.Text = String.Format(SNOOZE_MESSAGE, name);
            }

            contentStackPanel.ActiveControl = snoozeAlertContentPanel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int countSnoozed = 0;
            string user = RepositoryHelper.GetRepositoryUser(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            IManagementService ms = ManagementServiceHelper.GetDefaultService();
            
            // The auditing engine.
            if (action == SnoozeAction.Snooze)
            {
                SnoozeActionOperation(user, ms);
            }
            else//UnSnooze part
            {
                UnsnoozeActionOperation(user, ms);
            }
            
        }


        /// <summary>
        /// work flow for snooze alerts
        /// </summary>
        /// <param name="user">User performing the operation</param>
        /// <param name="ms">Instance of IManagementService</param>
        private void SnoozeActionOperation(string user, IManagementService ms)
        {
            // Auditing the the snooze alert.
            AuditableEntity auditable = null;

            string sqlUser = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser;
            AuditingEngine.SetContextData(sqlUser);

            //if we have a list of instances
            if (serverInstanceList != null)
            {
                auditable = new AuditableEntity();
                auditable.Name = tagName;
                auditable.SqlUser = sqlUser;

                //Pass thru context to management service, the tagName and UserName that contains all server
                AuditingEngine.SetAuxiliarData("AuditSnnozeAllSeversAlerts",
                                               new AuditAuxiliar<AuditableEntity>(auditable));

                ms.SnoozeServersAlerts(serverInstanceList, metricId, Convert.ToInt32(snoozeMinutesSpinner.Value), user);

                foreach (int serverId in serverInstanceList)
                {
                    ApplicationModel.Default.UpdateInstanceSnoozedStatus(serverId, -2);
                }
            }
            else
            {
                if (metricId == null)
                {
                    auditable = GetAuditableEntityForAllAlerts(instanceId);
                }
                else
                {
                    auditable = GetAuditableEntity();
                }

                auditable.SqlUser = sqlUser;
                AuditingEngine.SetAuxiliarData("AuditSingleSnoozeAlert",
                                               new AuditAuxiliar<AuditableEntity>(auditable));

                AuditingEngine.SetAuxiliarData("isSimpleSnooze", new AuditAuxiliar<bool>(true));
                result = ms.SnoozeAlerts(instanceId, metricId, Convert.ToInt32(snoozeMinutesSpinner.Value), user);
                int countSnoozed = metricId.HasValue ? -1 : -2;
                ApplicationModel.Default.UpdateInstanceSnoozedStatus(instanceId, countSnoozed);
            }
        }

        /// <summary>
        /// work flow for Unsnooze alerts
        /// </summary>
        /// <param name="user">User performing the operation</param>
        /// <param name="ms">Instance of IManagementService</param>
        private void UnsnoozeActionOperation(string user, IManagementService ms)
        {
            int countSnoozed = 0;
            List<int> selectedMetrics = new List<int>();

            foreach (UltraGridRow row in snoozedAlertsGrid.Rows)
            {
                if ((bool) row.Cells["Selected"].Value)
                    selectedMetrics.Add((int) row.Cells["Metric"].Value);
                else
                    countSnoozed++;
            }

            bool allMetrics = countSnoozed == 0 && selectedMetrics.Count >= snoozedAlertsGrid.Rows.Count;

            //if we have a list of instances
            if (serverInstanceList != null)
            {
                // to audit Resume all server Alerts
                string sqlUser = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser;
                AuditingEngine.SetContextData(sqlUser);

                AuditableEntity auditable = new AuditableEntity();
                auditable.Name = tagName;
                auditable.SqlUser = sqlUser;

                //Pass thru context to management service, the tagName and UserName that contains all server
                AuditingEngine.SetAuxiliarData("AuditResumeAllSeversAlerts",
                                               new AuditAuxiliar<AuditableEntity>(auditable));

                ms.UnSnoozeServersAlerts(serverInstanceList, allMetrics ? null : selectedMetrics.ToArray(), user);

                foreach (int serverId in serverInstanceList)
                {
                    ApplicationModel.Default.UpdateInstanceSnoozedStatus(serverId, -2);
                }
            }
            else
            {
                SetUnSnoozeContextData(selectedMetrics, allMetrics);

                // get the number of thresholds for this instance
                // should we send the list or did they unsnooze all alerts
                result = ms.UnSnoozeAlerts(instanceId, allMetrics ? null : selectedMetrics.ToArray(), user);

                if (unsnoozeAlertConfig != null)
                {
                    if (allMetrics)
                    {
                        foreach (AlertConfigurationItem item in unsnoozeAlertConfig.ItemList)
                        {
                            UpdateSnoozeInfo(item, result);
                        }
                    }
                    else
                    {
                        foreach (int metricId in selectedMetrics)
                        {
                            Dictionary<string, AlertConfigurationItem> items =
                                unsnoozeAlertConfig.GetThresholdsForMetric(metricId);
                            foreach (var item in items.Values)
                            {
                                UpdateSnoozeInfo(item, result);
                            }
                        }
                    }
                }

                ApplicationModel.Default.UpdateInstanceSnoozedStatus(instanceId, countSnoozed);
            }
        }

        private void SetUnSnoozeContextData(List<int> selectedMetrics, bool allMetrics)
        {
            // Get the auditable entity.
            AuditableEntity entity = this.GetUnsnoozeAuditableEntity(selectedMetrics, allMetrics);
            String serverInstanceName = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
            entity.Name = serverInstanceName;
            entity.AddMetadataProperty("Monitored SQL Server instance", serverInstanceName);

            AuditingEngine.SetContextData(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("SnoozeEntity", entity);
        }


        private AuditableEntity GetUnsnoozeAuditableEntity(List<int> selectedMetrics, bool allMetrics)
        {
            var entity = new AuditableEntity();

            // The Name property is set out the box.
            if (allMetrics)
            {
                entity.AddMetadataProperty("Resumed alerts", String.Empty);
            }
            else
            {
                selectedMetrics.ForEach(selectedMetricId => entity.AddMetadataProperty("Metric name",
                    this.GetMetricDescription(selectedMetricId).Value.Name ?? String.Empty));

            }

            return entity;
        }

        private AuditableEntity GetAuditableEntityForAllAlerts(int instanceId)
        {
            var auditableEntity = new AuditableEntity();

            // Get server Instance name.
            auditableEntity.Name = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
            auditableEntity.AddMetadataProperty("Alerts snoozed", "All");
           
            return auditableEntity;
        }

        void UpdateSnoozeInfo(AlertConfigurationItem item, SnoozeInfo snoozeInfo)
        {
            AdvancedAlertConfigurationSettings settings = item.ThresholdEntry.Data as AdvancedAlertConfigurationSettings;
            if (settings != null && settings.SnoozeInfo != null)
            {
                settings.SnoozeInfo.Stop(snoozeInfo.StopSnoozing, snoozeInfo.UnsnoozedBy);                 
            }
        }

        private void resumeToggleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (settingCheckBox)
                return;

            bool newValue = resumeToggleCheckBox.Checked;
            foreach (UltraGridRow gridRow in snoozedAlertsGrid.Rows)
            {
                gridRow.Cells["Selected"].SetValue(newValue, false);
            }

            okButton.Enabled = newValue;
        }

        private void snoozedAlertsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (e.Button == MouseButtons.Left)
            {
                UIElement selectedElement = grid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                if (!(selectedElement is CheckIndicatorUIElement || selectedElement is ImageUIElement))
                    return;

                // logic to handle toggling a checkbox in a non-editable (no cell selection) column
                object contextObject = selectedElement.GetContext();
                if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
                {
                    if (((UltraGridColumn)contextObject).Key == "Selected")
                    {
                        UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;

                        if (selectedRow != null)
                        {
                            bool newValue = true;
                            DataRowView drv = selectedRow.ListObject as DataRowView;
                            if (drv != null)
                            {
                                object value = drv["Selected"];
                                if (value is bool)
                                    newValue = !((bool) value);

                                drv["Selected"] = newValue;

                                UpdateSelectAllCheckBox();
                            }

                            grid.Rows.Refresh(RefreshRow.RefreshDisplay);
                        }
                    }
                }
            }
        }

        private void UpdateSelectAllCheckBox()
        {
            bool checkIt = true;
            DataTable data = snoozedAlertsGrid.DataSource as DataTable;
            if (data != null)
            {
                foreach (UltraGridRow row in snoozedAlertsGrid.Rows)
                {
                    bool selected = (bool)row.Cells["Selected"].Value;
                    if (!selected)
                    {
                        checkIt = false;
                        break;
                    }
                }

                okButton.Enabled = snoozedAlertsGrid.Rows.Count > 0;
                settingCheckBox = true;
                resumeToggleCheckBox.Checked = checkIt;
                settingCheckBox = false;
            }
        }

        private void SnoozeAlertsDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.SnoozeAlertsDialog);
        }

        private void SnoozeAlertsDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.SnoozeAlertsDialog);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        /// <summary>
        /// Make the auditable entity, with the core information for the alert.
        /// </summary>
        /// <returns>The auditable entity</returns>
        private AuditableEntity GetAuditableEntity()
        {
            var auditableEntity = new AuditableEntity();

            // Get metric name.
            String metricName = this.metricDescription.HasValue ? this.metricDescription.Value.Name : String.Empty;
            auditableEntity.Name = metricName;
            auditableEntity.AddMetadataProperty("Description", String.Format("Snoozed Alert for the metric '{0}'", metricName));
            auditableEntity.AddMetadataProperty("Metric", metricName);

            // Get the metric description.
            String metricDescriptionValue = metricDescription.HasValue ? metricDescription.Value.Name : String.Empty;

            // Get the monitored SQL server on which is performed the snooze operation.
            String serverInstanceName = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;

            auditableEntity.AddMetadataProperty("Monitored SQL Server instance", serverInstanceName);
            auditableEntity.AddMetadataProperty("Metric description", metricDescriptionValue);
            auditableEntity.AddMetadataProperty("Alert summary", alertDescription);

            return auditableEntity;
        }
    }
}
