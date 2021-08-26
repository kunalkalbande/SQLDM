using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Microsoft.SqlServer.MessageBox;
    using Properties;
    using Idera.SQLdm.DesktopClient.Views.Reports;

    public partial class ConfigureBaselineDialog : Form
    {
        private readonly int instanceId;
        private SqlConnectionInfo connectionInfo;
        private DateTime? earliestDataAvailable;
        private string instanceName;

        public ConfigureBaselineDialog(int instanceId)
        {
            this.instanceId = instanceId;
            InitializeComponent();

            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                instanceName = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
                Text = string.Format(Text, instanceName);
            }
            AdaptFontSize();
        }

        private void customBaselineRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            customDateFromLabel.Enabled = 
            customDateFromCombo.Enabled =
            customDateToLabel.Enabled = 
            customDateToCombo.Enabled = customBaselineRadioButton.Checked;
        }

        private void ConfigureBaselineDialog_Load(object sender, System.EventArgs e)
        {
            bool useDefaults;
            DateTime startDate;
            DateTime endDate;
            short days;

            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                BaselineHelpers.GetBaselineParameters(connection, 
                                        instanceId, 
                                        out useDefaults, 
                                        out startDate,              
                                        out endDate, 
                                        out days, 
                                        out earliestDataAvailable);
            }

            automaticBaselineRadioButton.Checked = useDefaults;
            customBaselineRadioButton.Checked = !useDefaults;
            customBaselineRadioButton_CheckedChanged(customBaselineRadioButton, EventArgs.Empty);

            customDateFromCombo.Value = startDate;
            customDateToCombo.Value = endDate;
            beginTimeCombo.Time = startDate.TimeOfDay;
            endTimeCombo.Time = endDate.TimeOfDay;

            sundayCheckbox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, days);
            mondayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, days);
            tuesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, days);
            wednesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, days);
            thursdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, days);
            fridayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, days);
            saturdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, days);
        }

        private void okButton_Click(object sender, EventArgs args)
        {
            bool useDefaults;
            DateTime startDate;
            DateTime endDate;
            short days = 0;

            try
            {
                if (sundayCheckbox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
                if (mondayCheckBox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
                if (tuesdayCheckBox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
                if (wednesdayCheckBox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
                if (thursdayCheckBox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
                if (fridayCheckBox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
                if (saturdayCheckBox.Checked)
                    days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);


                useDefaults = automaticBaselineRadioButton.Checked;

                startDate = (DateTime) customDateFromCombo.Value;
                endDate = (DateTime) customDateToCombo.Value;

                if (!useDefaults)
                {
                    if (startDate.Date > endDate.Date)
                    {
                        ApplicationMessageBox.ShowWarning(this,
                                                          "The start date must be before the end date for the baseline period.");
                        DialogResult = DialogResult.None;
                        return;
                    }
                }

                startDate = startDate.Date + beginTimeCombo.Time;
                endDate = endDate.Date + endTimeCombo.Time;

                if (days == 0)
                {
                    ApplicationMessageBox.ShowWarning(this,
                                                      "The days of the week to include in the baseline must be selected.");
                    DialogResult = DialogResult.None;
                    return;
                }
                
                if (!useDefaults)
                {
                    if (earliestDataAvailable.HasValue && startDate.Date < earliestDataAvailable.Value)
                    {
                        if (ApplicationMessageBox.ShowWarning(this,
                                                          "The start date selected occurs before the earliest available collected data.  Do you want to continue and use the selected dates?",
                                                          ExceptionMessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            DialogResult = DialogResult.None;
                            return;
                        }
                    }

                }


                using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    BaselineHelpers.SetBaselineParameters(connection, instanceId, useDefaults, startDate, endDate, days);
                }
            } catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                                                "There was a problem saving the baseline configuration.  Please resolve the error and try again.",
                                                e);

                DialogResult = DialogResult.None;
                return;
            }

            int instanceCount = ApplicationModel.Default.ActiveInstances.Count;
            if (instanceCount > 1)
            {
                ApplicationMessageBox box = new ApplicationMessageBox();
                box.Text = "Would you like to apply these same changes to other SQL Server instances?";
                box.ShowCheckBox = false;
                box.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Question;
                box.Caption = this.Text;
                box.Buttons = ExceptionMessageBoxButtons.YesNo;
                if (box.Show(this) == DialogResult.Yes)
                {
                    List<string> excluded = new List<string>();
                    excluded.Add(instanceName);
                    
                    foreach (ServerPermission instancePermission in ApplicationModel.Default.UserToken.AssignedServers)
                    {
                        if (instancePermission.PermissionType == PermissionType.View)
                        {
                            excluded.Add(instancePermission.Server.InstanceName);
                        }
                    }

                    using (ReportServersDialog serverDialog = new ReportServersDialog(
                        "Apply Baseline Configuration Changes",
                        "Check the SQL Server instance(s) that you wish to apply the baseline configuration changes to and press OK.",
                        excluded
                        ))
                    {
                        // This property is being set so that we suppress the reports related
                        // message that shows up when we select more then 5 servers.   Before
                        // going out of the use scope the flag is reset.
                        bool rememberManyServersSlow = Properties.Settings.Default.ShowMessage_ManyServersSlowsReporting;
                        Properties.Settings.Default.ShowMessage_ManyServersSlowsReporting = false;

                        MonitoredSqlServer selectedServer = null;
                        try
                        {
                            serverDialog.ActiveServersOnly = true;
                            if (serverDialog.ShowDialog(this) == DialogResult.OK)
                            {
                                List<MonitoredSqlServer> selectedServers = serverDialog.SelectedServers;
                                using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
                                {
                                    connection.Open();
                                    foreach (MonitoredSqlServer server in selectedServers)
                                    {
                                        BaselineHelpers.SetBaselineParameters(connection, server.Id, useDefaults, startDate, endDate, days);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (selectedServer != null)
                                ApplicationMessageBox.ShowError(this,
                                                            String.Format("There was a problem saving the baseline configuration for server {0}.  Please resolve the error and try again.", selectedServer.InstanceName),
                                                            e);
                            else
                                ApplicationMessageBox.ShowError(this,
                                                            "There was a problem saving the baseline configuration to the selected servers.  Please resolve the error and try again.",
                                                            e);
                        }
                        finally
                        {
                            Properties.Settings.Default.ShowMessage_ManyServersSlowsReporting = rememberManyServersSlow;
                        }
                    }
                }
            }
        }

        private void ConfigureBaselineDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureBaselineDialog);
        }

        private void ConfigureBaselineDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureBaselineDialog);
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