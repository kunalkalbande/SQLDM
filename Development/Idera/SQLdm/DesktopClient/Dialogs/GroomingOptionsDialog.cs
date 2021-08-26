using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using System.Data.SqlClient;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Controls.CustomControls;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.DesktopClient.Properties;
    using Infragistics.Windows.Themes;

    public partial class GroomingOptionsDialog : BaseDialog
    {
        string _timeFormat;
        TimeSpan _timeOffset;
        private bool isScheduleOutOfSync = false;
        private bool isAggregationScheduleOutOfSync = false;

        public GroomingOptionsDialog()
        {
            this.DialogHeader = "Grooming Options";
            InitializeComponent();

            _timeFormat = currentTimeLabel.Text;

            // Create the job if it doesn't exist.
            ManagementServiceHelper.GetDefaultService().CreateGroomJob();
            AdaptFontSize();
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            else
            {
                this.ClientSize = new Size(650, 600);
                this.containerPanel.Size = new Size(600, 550);
                this.containerPanel.AutoScrollMinSize = new Size(550, 750);
                this.containerPanel.AutoScroll = true;
            }
            FormBorderStyle = FormBorderStyle.Sizable;
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

        }

        private void GroomingOptionsDialog_Load(object sender, EventArgs e)
        {
            GroomingConfiguration configuration = ManagementServiceHelper.GetDefaultService().GetGrooming();
            groomingTimeEditor.Value = DateTime.Today + configuration.GroomTime;
            metricsGroomingThresholdNumericUpDown.Value = configuration.MetricsDays;
            alertsGroomingThresholdNumericUpDown.Value = configuration.AlertsDays;
            activityGroomingThresholdNumericUpDown.Value = configuration.ActivityDays;
            aggregationThresholdNumericUpDown.Value = configuration.QueriesDays;
            auditGroomingThresholdNumericUpDown.Value = configuration.AuditDays;
            //10.0 SQLdm srishti purohit
            //Prescriptive analysis old data grooming implementation
            GroomForecastNumericUpDown.Value = configuration.GroomForecastingDays;
            ForecastingAggregationThresholdNumericUpDown.Value = configuration.FADataDays;
            PAGroomingThresholdNumericUpDown.Value = configuration.PADataDays;
            UpdateSchedule(configuration, true);
            UpdateAggregationSchedule(configuration, true);
            currentlyRunning.Text = configuration.JobIsRunning ? "Yes" : "No";
            aggregationCurrentlyRunning.Text = configuration.AggregationJobIsRunning ? "Yes" : "No";
            updateLastRun(configuration.LastRun);
            updateAggregationLastRun(configuration.AggregationLastRun);
            completionStatus.Text = configuration.LastOutcome;
            aggregationCompletionStatus.Text = configuration.AggregationLastOutcome;
            _timeOffset = configuration.RepositoryTime - DateTime.Now;
            timer1_Tick(null, null);
            timer1.Enabled = true;
            apply.Enabled = false;


            if (!configuration.AgentIsRunning)
            {
                ShowAgentMessage();
            }
        }

        private void UpdateSchedule(GroomingConfiguration configuration, bool showWarningMsg)
        {
            if (configuration.ScheduleSubDayType != GroomingConfiguration.SubDayType.Once
                        && configuration.ScheduleSubDayType != GroomingConfiguration.SubDayType.Hours)
            {
                onceDailyButton.Checked = true;
                groomingTimeEditor.Value = DateTime.Today + new TimeSpan(3, 0, 0);
                groomTimeIntervalCombo.SelectedIndex = 3;
            }
            else if (configuration.ScheduleSubDayType == GroomingConfiguration.SubDayType.Once)
            {
                onceDailyButton.Checked = true;
                groomingTimeEditor.Value = DateTime.Today + configuration.GroomTime;
                groomTimeIntervalCombo.SelectedIndex = 3;
            }
            else
            {
                hourlyButton.Checked = true;
                groomingTimeEditor.Value = DateTime.Today + TimeSpan.FromHours(3);
                groomTimeIntervalCombo.SelectedIndex = configuration.GroomTime.Seconds != 0 ?
                    configuration.GroomTime.Seconds - 1 : 3;
            }

            isScheduleOutOfSync = !configuration.UpdateScheduleAllowed;
            if (isScheduleOutOfSync && showWarningMsg)
            {
                ApplicationMessageBox.ShowWarning(ParentForm,
                                                  "The grooming job schedule has been modified beyond the options provided in this dialog. The values displayed may not accurately reflect the current job schedule.");
            }
        }

        private void UpdateAggregationSchedule(GroomingConfiguration configuration, bool showWarningMsg)
        {
            if (configuration.AggregationSubDayType != GroomingConfiguration.SubDayType.Once
                        && configuration.AggregationSubDayType != GroomingConfiguration.SubDayType.Hours)
            {
                aggregationOnceDailyButton.Checked = true;
                aggregationTimeEditor.Value = DateTime.Today + new TimeSpan(2, 0, 0);
                aggregationTimeIntervalCombo.SelectedIndex = 3;
            }
            else if (configuration.AggregationSubDayType == GroomingConfiguration.SubDayType.Once)
            {
                aggregationOnceDailyButton.Checked = true;
                aggregationTimeEditor.Value = DateTime.Today + configuration.AggregationTime;
                aggregationTimeIntervalCombo.SelectedIndex = 3;
            }
            else
            {
                aggregationHourlyButton.Checked = true;
                aggregationTimeEditor.Value = DateTime.Today + TimeSpan.FromHours(3);
                aggregationTimeIntervalCombo.SelectedIndex = configuration.AggregationTime.Seconds != 0 ?
                    configuration.AggregationTime.Seconds - 1 : 3;
            }

            isAggregationScheduleOutOfSync = !configuration.UpdateAggregationScheduleAllowed;
            if (isScheduleOutOfSync && showWarningMsg)
            {
                ApplicationMessageBox.ShowWarning(ParentForm,
                                                  "The aggregation job schedule has been modified beyond the options provided in this dialog. The values displayed may not accurately reflect the current job schedule.");
            }
        }


        private void groomNow_Click(object sender, EventArgs e)
        {
            GroomingConfiguration configuration = null;
            try
            {
                configuration = ManagementServiceHelper.GetDefaultService().GetGrooming();
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Failed to read the status prior to starting the grooming job.   The grooming job will not be started.", ex);
            }

            if (configuration != null)
            {
                currentlyRunning.Text = configuration.JobIsRunning ? "Yes" : "No";
                updateLastRun(configuration.LastRun);
                completionStatus.Text = configuration.LastOutcome;

                if (configuration.AgentIsRunning)
                {
                    if (configuration.JobIsRunning)
                    {
                        ApplicationMessageBox.ShowError(this, "The grooming job cannot be started because it is already running.");
                    }
                    else
                    {
                        DialogResult msgBoxResult = DialogResult.None;
                        if (apply.Enabled)
                        {
                            ApplicationMessageBox box = new ApplicationMessageBox();
                            box.Text = "Do you want to apply your configuration changes before grooming?";
                            box.Buttons = ExceptionMessageBoxButtons.YesNoCancel;
                            msgBoxResult = box.Show(this);

                            if (msgBoxResult == DialogResult.Yes)
                            {
                                apply_Click(apply, null);
                            }
                        }

                        if (msgBoxResult != DialogResult.Cancel)
                        {
                            bool isJobStarted = false;
                            try
                            {
                                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                                AuditingEngine.SetAuxiliarData("GroomEntity", new AuditableEntity { Name = "SQLdm" });

                                ManagementServiceHelper.GetDefaultService().GroomNow();

                                isJobStarted = true;
                            }
                            catch (Exception ex)
                            {
                                ApplicationMessageBox.ShowError(this, "Failed to start the grooming job.", ex);
                            }

                            if (isJobStarted)
                            {
                                try
                                {
                                    configuration = ManagementServiceHelper.GetDefaultService().GetGrooming();
                                    currentlyRunning.Text = configuration.JobIsRunning ? "Yes" : "No";
                                    updateLastRun(configuration.LastRun);
                                    completionStatus.Text = configuration.LastOutcome;
                                }
                                catch (Exception ex)
                                {
                                    ApplicationMessageBox.ShowWarning(this, "Unable to read the status after the grooming job was started.", ex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ShowAgentMessage();
                }
            }
        }

        private void aggregateNow_Click(object sender, EventArgs e)
        {
            GroomingConfiguration configuration = null;
            try
            {
                configuration = ManagementServiceHelper.GetDefaultService().GetGrooming();
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Failed to read the status prior to starting the aggregation job.   The aggregation job will not be started.", ex);
            }

            if (configuration != null)
            {
                aggregationCurrentlyRunning.Text = configuration.AggregationJobIsRunning ? "Yes" : "No";
                updateAggregationLastRun(configuration.AggregationLastRun);
                aggregationCompletionStatus.Text = configuration.AggregationLastOutcome;

                if (configuration.AgentIsRunning)
                {
                    if (configuration.AggregationJobIsRunning)
                    {
                        ApplicationMessageBox.ShowError(this, "The aggregation job cannot be started because it is already running.");
                    }
                    else
                    {
                        DialogResult msgBoxResult = DialogResult.None;
                        if (apply.Enabled)
                        {
                            ApplicationMessageBox box = new ApplicationMessageBox();
                            box.Text = "Do you want to apply your configuration changes before aggregating?";
                            box.Buttons = ExceptionMessageBoxButtons.YesNoCancel;
                            msgBoxResult = box.Show(this);

                            if (msgBoxResult == DialogResult.Yes)
                            {
                                apply_Click(apply, null);
                            }
                        }

                        if (msgBoxResult != DialogResult.Cancel)
                        {
                            bool isJobStarted = false;
                            try
                            {
                                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                                AuditingEngine.SetAuxiliarData("AggregateEntity", new AuditableEntity { Name = "SQLdm" });

                                ManagementServiceHelper.GetDefaultService().AggregateNow();
                                isJobStarted = true;
                            }
                            catch (Exception ex)
                            {
                                ApplicationMessageBox.ShowError(this, "Failed to start the aggregation job.", ex);
                            }

                            if (isJobStarted)
                            {
                                try
                                {
                                    configuration = ManagementServiceHelper.GetDefaultService().GetGrooming();
                                    aggregationCurrentlyRunning.Text = configuration.AggregationJobIsRunning ? "Yes" : "No";
                                    updateAggregationLastRun(configuration.AggregationLastRun);
                                    aggregationCompletionStatus.Text = configuration.AggregationLastOutcome;
                                }
                                catch (Exception ex)
                                {
                                    ApplicationMessageBox.ShowWarning(this, "Unable to read the status after the aggregation job was started.", ex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ShowAgentMessage();
                }
            }
        }

        private void ShowAgentMessage()
        {
            ApplicationMessageBox.ShowWarning(this, "The SQL Server Agent is not running on the repository.  You can edit the grooming options, but grooming will not occur unless the SQL Server Agent is running.");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            apply_Click(okButton, null);
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.GroomingOptions);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.GroomingOptions);
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            try
            {
                GroomingConfiguration configuration = ManagementServiceHelper.GetDefaultService().GetGrooming();
                currentlyRunning.Text = configuration.JobIsRunning ? "Yes" : "No";
                updateLastRun(configuration.LastRun);
                completionStatus.Text = configuration.LastOutcome;
                aggregationCurrentlyRunning.Text = configuration.AggregationJobIsRunning ? "Yes" : "No";
                aggregationCompletionStatus.Text = configuration.AggregationLastOutcome;
                updateAggregationLastRun(configuration.AggregationLastRun);
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowWarning(this, "Unable to read the grooming job status, please try again.", ex);
            }
        }

        private void numericUpDown_Validating(object sender, CancelEventArgs e)
        {
            CustomNumericUpDown nud = sender as CustomNumericUpDown;
            if (nud != null && nud.Text == "")
            {
                nud.Text = nud.Value.ToString();
            }
        }

        private void ConfigValueChanged(object sender, EventArgs e)
        {
            apply.Enabled = true;
        }

        private void apply_Click(object sender, EventArgs e)
        {
            GroomingConfiguration configuration = new GroomingConfiguration();
            configuration.ActivityDays = (int)activityGroomingThresholdNumericUpDown.Value;
            //10.0 SQLdm srishti purohit
            //Prescriptive analysis old data grooming implementation
            configuration.PADataDays = (int)PAGroomingThresholdNumericUpDown.Value;
            configuration.AlertsDays = (int)alertsGroomingThresholdNumericUpDown.Value;
            configuration.MetricsDays = (int)metricsGroomingThresholdNumericUpDown.Value;
            configuration.AuditDays = (int)auditGroomingThresholdNumericUpDown.Value;
            configuration.GroomForecastingDays = (int)GroomForecastNumericUpDown.Value;
            configuration.FADataDays = (int)ForecastingAggregationThresholdNumericUpDown.Value;
            configuration.ScheduleSubDayType = onceDailyButton.Checked
                                                   ? GroomingConfiguration.SubDayType.Once
                                                   : GroomingConfiguration.SubDayType.Hours;

            configuration.GroomTime = onceDailyButton.Checked
                                          ? ((DateTime)groomingTimeEditor.Value).TimeOfDay
                                          : TimeSpan.FromHours(groomTimeIntervalCombo.SelectedIndex + 1);

            if (isScheduleOutOfSync)
            {
                configuration.UpdateScheduleAllowed =
                    ApplicationMessageBox.ShowQuestion(this,
                                                       "The grooming job schedule has been modified beyond the options provided in this dialog. Click OK to overwrite the job schedule or Cancel to only update the data retention options.",
                                                       ExceptionMessageBoxButtons.OKCancel) == DialogResult.OK;
            }
            else
                configuration.UpdateScheduleAllowed = true;

            configuration.QueriesDays = (int)aggregationThresholdNumericUpDown.Value;
            configuration.AggregationSubDayType = aggregationOnceDailyButton.Checked
                                                   ? GroomingConfiguration.SubDayType.Once
                                                   : GroomingConfiguration.SubDayType.Hours;

            configuration.AggregationTime = aggregationOnceDailyButton.Checked
                                          ? ((DateTime)aggregationTimeEditor.Value).TimeOfDay
                                          : TimeSpan.FromHours(aggregationTimeIntervalCombo.SelectedIndex + 1);

            if (isAggregationScheduleOutOfSync)
            {
                configuration.UpdateAggregationScheduleAllowed =
                    ApplicationMessageBox.ShowQuestion(this,
                                                       "The aggregation job schedule has been modified beyond the options provided in this dialog. Click OK to overwrite the job schedule or Cancel to only update the data retention options.",
                                                       ExceptionMessageBoxButtons.OKCancel) == DialogResult.OK;
            }
            else
                configuration.UpdateAggregationScheduleAllowed = true;

            try
            {
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                ManagementServiceHelper.GetDefaultService().UpdateGrooming(configuration);

                if (sender == apply)
                {
                    try
                    {
                        GroomingConfiguration tempConfiguration = ManagementServiceHelper.GetDefaultService().GetGrooming();
                        configuration = tempConfiguration;
                    }
                    catch (Exception)
                    {
                        // Eat exception.
                    }
                    UpdateSchedule(configuration, false);
                    UpdateAggregationSchedule(configuration, false);
                    apply.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Failed to update the grooming or aggregation job configuration.", ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime repositoryTime = DateTime.Now + _timeOffset;
            currentTimeLabel.Text = string.Format(_timeFormat, repositoryTime);
        }

        private void hourlyButton_CheckedChanged(object sender, EventArgs e)
        {
            groomingTimeEditor.Enabled = false;
            groomTimeIntervalCombo.Enabled = true;
            apply.Enabled = true;
        }

        private void aggregationHourlyButton_CheckedChanged(object sender, EventArgs e)
        {
            aggregationTimeEditor.Enabled = false;
            aggregationTimeIntervalCombo.Enabled = true;
            apply.Enabled = true;
        }

        private void onceDailyButton_CheckedChanged(object sender, EventArgs e)
        {
            groomingTimeEditor.Enabled = true;
            groomTimeIntervalCombo.Enabled = false;
            apply.Enabled = true;
        }

        private void aggregationOnceDailyButton_CheckedChanged(object sender, EventArgs e)
        {
            aggregationTimeEditor.Enabled = true;
            aggregationTimeIntervalCombo.Enabled = false;
            apply.Enabled = true;
        }

        private void updateLastRun(DateTime configVal)
        {
            lastRun.Text = (configVal == DateTime.MinValue) ? "" : configVal.ToString();
        }

        private void updateAggregationLastRun(DateTime configVal)
        {
            aggregationLastRun.Text = (configVal == DateTime.MinValue) ? "" : configVal.ToString();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ScaleControlsAsPerResolution()
        {
            if (AutoScaleSizeHelper.isLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel4, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 0.5F), false);
                this.ClientSize = new Size(950, 900);
                this.containerPanel.Size = new Size(900, 750);
                this.tableLayoutPanel4.Location = new Point(this.tableLayoutPanel4.Location.X, this.tableLayoutPanel4.Location.Y * 2);
                this.tableLayoutPanel3.Location = new Point(this.tableLayoutPanel3.Location.X, this.tableLayoutPanel3.Location.Y);
                this.containerPanel.AutoScroll = true;
                this.containerPanel.AutoScrollMinSize = new Size(850, 1250);
                this.okButton.Location = new Point(this.okButton.Location.X, this.okButton.Location.Y - 70);
                this.cancelButton.Location = new Point(this.cancelButton.Location.X, this.cancelButton.Location.Y - 70);
                this.groomNow.Location = new Point(this.groomNow.Location.X, this.groomNow.Location.Y - 70);
                this.apply.Location = new Point(this.apply.Location.X, this.apply.Location.Y - 70);
                this.aggregateNow.Location = new Point(this.aggregateNow.Location.X, this.aggregateNow.Location.Y - 70);
            }
            else if (AutoScaleSizeHelper.isXLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel4, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 0.5F), false);
                this.ClientSize = new Size(1050, 900);
                this.containerPanel.Size = new Size(1000, 750);
                this.tableLayoutPanel4.Location = new Point(this.tableLayoutPanel4.Location.X, this.tableLayoutPanel4.Location.Y * 2);
                this.tableLayoutPanel3.Location = new Point(this.tableLayoutPanel3.Location.X, this.tableLayoutPanel3.Location.Y);
                this.containerPanel.AutoScroll = true;
                this.containerPanel.AutoScrollMinSize = new Size(950, 1570);
                this.okButton.Location = new Point(this.okButton.Location.X, this.okButton.Location.Y - 70);
                this.cancelButton.Location = new Point(this.cancelButton.Location.X, this.cancelButton.Location.Y - 70);
                this.groomNow.Location = new Point(this.groomNow.Location.X, this.groomNow.Location.Y - 70);
                this.apply.Location = new Point(this.apply.Location.X, this.apply.Location.Y - 70);
                this.aggregateNow.Location = new Point(this.aggregateNow.Location.X, this.aggregateNow.Location.Y - 70);
            }
            else if (AutoScaleSizeHelper.isXXLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel4, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.0F, 0.5F), false);
                this.ClientSize = new Size(1050, 900);
                this.containerPanel.Size = new Size(1000, 750);
                this.tableLayoutPanel4.Location = new Point(this.tableLayoutPanel4.Location.X, this.tableLayoutPanel4.Location.Y * 2);
                this.tableLayoutPanel3.Location = new Point(this.tableLayoutPanel3.Location.X, this.tableLayoutPanel3.Location.Y);
                this.containerPanel.AutoScrollMinSize = new Size(950, 1730);
                this.okButton.Location = new Point(this.okButton.Location.X, this.okButton.Location.Y - 50);
                this.cancelButton.Location = new Point(this.cancelButton.Location.X, this.cancelButton.Location.Y - 50);
                this.groomNow.Location = new Point(this.groomNow.Location.X, this.groomNow.Location.Y - 50);
                this.apply.Location = new Point(this.apply.Location.X, this.apply.Location.Y - 50);
                this.aggregateNow.Location = new Point(this.aggregateNow.Location.X, this.aggregateNow.Location.Y - 50);
            }
            else
            {
                this.containerPanel.AutoScrollMinSize = new Size(500, 750);
                this.containerPanel.AutoScroll = true;
            }
        }
    }
}
