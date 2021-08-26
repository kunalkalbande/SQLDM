using System;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdoctor.Common;
using Idera.SQLdoctor.StandardClient.Dialogs;
using Microsoft.SqlServer.MessageBox;
using Microsoft.Win32.TaskScheduler;
using TracerX;
using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdoctor.Common.Recommendations;
using Infragistics.Win;
using Idera.SQLdoctor.CommonGUI.Dialogs;

namespace Idera.SQLdoctor.StandardClient.Dialogs
{
    public partial class ScheduleSettingsDialog : Form
    {
        private static readonly Logger Log = Logger.GetLogger("ScheduleSettingsDialog");

        private string _scheduledServerName;
        private bool _settingsChanged;
        private bool _scheduleEnabled;
        private bool _updateInProgress;

        public ScheduleSettingsDialog()
        {
            InitializeComponent();
        }

        private void UpdateOneTimeAnalysisInfo()
        {
            try
            {
                var info = ScheduleHelper.GetTask(ScheduledTaskType.Analysis);

                if (info != null &&
                    info.NextRunTime.HasValue &&
                    info.NextRunTime.Value > DateTime.Now)
                {
                    _oneTimeAnalysisHeaderStrip.Text = String.Format("One time analysis is scheduled for server {0}", info.InstanceName);
                    _oneTimeAnalysisLinkLabel.Value =
                        string.Format(
                            "<font face='Arial' size='10pt'>An upcoming analysis is scheduled at {0}{1} and will run for approximately {2} {3}.  <a href='recommendations'>Click here to cancel the analysis.</a></font>",
                            info.NextRunTime.Value.ToShortTimeString(),
                            DateTime.Now.Day == info.NextRunTime.Value.Day ? "" : " tomorrow",
                            info.Duration < 60 ? info.Duration : info.Duration / 60,
                            info.Duration < 60 ? "minutes" : info.Duration == 60 ? "hour" : "hours");
                    _oneTimeAnalysisInfoPanel.Visible = true;
                }
                else
                {
                    _oneTimeAnalysisInfoPanel.Visible = false;
                }
            }
            catch (Exception exception)
            {
                _oneTimeAnalysisInfoPanel.Visible = false;
                ExceptionLogger.Log("An error occurred while retrieving the one time scheduled analysis task.", exception);
            }

            if (this.ClientRectangle.Bottom != panelButtons.Bounds.Bottom)
            {
                Size sz = this.Size;
                sz.Height = sz.Height - (this.ClientRectangle.Bottom - panelButtons.Bounds.Bottom);
                this.Size = sz;
            }
        }

        private void OnSettingsChanged(EventArgs e)
        {
            if (_updateInProgress) return;
            _settingsChanged = true;
        }

        private void UpdateScheduleSettings()
        {
            if (_settingsChanged) return;
            if (_scheduleEnabled != _scheduleEnabledCheckBox.Checked) return;

            _updateInProgress = true;

            _serverNameComboBox.Items.Clear();

            foreach (var connection in CommonSettings.Default.Servers)
            {
                _serverNameComboBox.Items.Add(connection.InstanceName);
            }
            TaskInfo info = ScheduleHelper.GetTask(ScheduledTaskType.Checkup);
            if (info != null)
            {
                _scheduleEnabledCheckBox.Checked = info.Enabled;
                _scheduledServerName = info.InstanceName.ToUpper();
                _serverNameComboBox.SelectedItem = _scheduledServerName;
                _scheduleSettingsPanel.Enabled = _scheduleEnabledCheckBox.Checked;
                _recurrenceComboBox.SelectedIndex = info.TriggerType == TaskTriggerType.Daily ? 0 : 1;

                if (info.TriggerType == TaskTriggerType.Weekly)
                {
                    _sundayCheckBox.Checked = info.Sunday;
                    _mondayCheckBox.Checked = info.Monday;
                    _tuesdayCheckBox.Checked = info.Tuesday;
                    _wednesdayCheckBox.Checked = info.Wednesday;
                    _thursdayCheckBox.Checked = info.Thursday;
                    _fridayCheckBox.Checked = info.Friday;
                    _saturdayCheckBox.Checked = info.Saturday;
                }

                _startTimePicker.Value = info.Start;
                if (info.Duration.HasValue) NumericAnalysisDuration.Value = info.Duration.Value;
                else NumericAnalysisDuration.Value = Properties.Settings.Default.DefaultSqlCollectorRuntimeMinutes;
            }
            else
            {
                Log.Info("A scheduled checkup could not be found.");
                _scheduleSettingsPanel.Enabled = _scheduleEnabledCheckBox.Checked = false;
                _recurrenceComboBox.SelectedIndex = 0;
            }

            UpdateLastRunStatus(info);
            _updateInProgress = false;

            _scheduleEnabled = _scheduleEnabledCheckBox.Checked;
        }

        private void UpdateLastRunStatus(TaskInfo info)
        {
            if (info == null)
            {
                _lastRunStatusLabel.Visible = false;
                return;
            }

            _lastRunStatusLabel.Visible = true;

            if (info.LastRunTime == null ||
                info.LastRunTime.Value.Year == 0001)
            {
                _lastRunStatusLabel.Text = "No run history";
                _lastRunStatusLabel.ForeColor = Color.Blue;
            }
            else if (info.State == TaskState.Running)
            {
                _lastRunStatusLabel.Text = "Running";
                _lastRunStatusLabel.ForeColor = Color.Green;
            }
            else if (info.LastTaskResult == 0)
            {
                _lastRunStatusLabel.Text = string.Format("Successfully ran on {0}", info.LastRunTime.Value.ToString("F"));
                _lastRunStatusLabel.ForeColor = Color.Green;
            }
            else
            {
                _lastRunStatusLabel.Text = string.Format("Failed on {0}", info.LastRunTime.Value.ToString("F"));
                _lastRunStatusLabel.ForeColor = Color.Red;
            }
        }

        private void _scheduleEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _scheduleSettingsPanel.Enabled = _scheduleEnabledCheckBox.Checked;
            OnSettingsChanged(EventArgs.Empty);
        }

        public bool SaveChanges()
        {
            if (!_settingsChanged) return (true);

            if (string.IsNullOrEmpty(_serverNameComboBox.Text) && !_scheduleEnabledCheckBox.Checked)
            {
                ScheduleHelper.DeleteTask(ScheduledTaskType.Checkup);
                return (true);
            }

            var taskInfo = new TaskInfo();
            taskInfo.Enabled = _scheduleEnabledCheckBox.Checked;
            taskInfo.TriggerType = _recurrenceComboBox.SelectedIndex == 0
                                       ? TaskTriggerType.Daily
                                       : TaskTriggerType.Weekly;

            taskInfo.Sunday = _recurrenceComboBox.SelectedIndex == 0 ? true : _sundayCheckBox.Checked;
            taskInfo.Monday = _recurrenceComboBox.SelectedIndex == 0 ? true : _mondayCheckBox.Checked;
            taskInfo.Tuesday = _recurrenceComboBox.SelectedIndex == 0 ? true : _tuesdayCheckBox.Checked;
            taskInfo.Wednesday = _recurrenceComboBox.SelectedIndex == 0 ? true : _wednesdayCheckBox.Checked;
            taskInfo.Thursday = _recurrenceComboBox.SelectedIndex == 0 ? true : _thursdayCheckBox.Checked;
            taskInfo.Friday = _recurrenceComboBox.SelectedIndex == 0 ? true : _fridayCheckBox.Checked;
            taskInfo.Saturday = _recurrenceComboBox.SelectedIndex == 0 ? true : _saturdayCheckBox.Checked;

            taskInfo.Start = _startTimePicker.Value;
            try
            {
                taskInfo.Duration = Convert.ToInt32(NumericAnalysisDuration.Value);
            }
            catch (Exception ex)
            {
                NumericAnalysisDuration.Focus();
                ApplicationMessageBox.ShowError(this, ex.Message);
                return (false);
            }
            taskInfo.InstanceName = _serverNameComboBox.Text;
            taskInfo.CheckForUpdates = false;
            
            if (!_scheduleEnabledCheckBox.Checked)
            {
                if (ApplicationMessageBox.ShowWarning(this, string.Format("You are about to disable the scheduled checkup for {0}. It is highly recommended that you run a scheduled checkup of your server to be informed of potential performance problems. Do you want to continue with this change?", _serverNameComboBox.Text), ExceptionMessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ScheduleHelper.DeleteTask(ScheduledTaskType.Checkup);
                }
                else
                {
                    _scheduleEnabledCheckBox.Checked = true;
                }
            }
            else
            {
                if (_scheduleEnabledCheckBox.Checked && string.IsNullOrEmpty(_serverNameComboBox.Text))
                {
                    ApplicationMessageBox.ShowError(this, "Please specify a SQL Server for the scheduled checkup.");
                    _serverNameComboBox.Focus();
                    return (false);
                }

                var credentialsDialog = new CredentialDialog();
                credentialsDialog.Caption = "Idera SQL doctor Credentials";
                credentialsDialog.Message = "Please enter the Windows credentials that should be used for the scheduled checkup task.";

                if (credentialsDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ScheduleHelper.UpdateTask(ScheduledTaskType.Checkup, taskInfo, credentialsDialog.User, credentialsDialog.Password);
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "You must supply valid Windows credentials in order to schedule your SQL Server checkup.");
                    return (false);
                }
            }
            _scheduleEnabled = _scheduleEnabledCheckBox.Checked;
            _settingsChanged = false;
            return (true);
        }

        private void _recurrenceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _weeklyRecurrencePanel.Visible = _recurrenceComboBox.SelectedIndex == 1;
            OnSettingsChanged(EventArgs.Empty);
        }

        private void SettingChanged(object sender, EventArgs e)
        {
            OnSettingsChanged(EventArgs.Empty);
        }

        private void _oneTimeAnalysisLinkLabel_LinkClicked(object sender, Infragistics.Win.FormattedLinkLabel.LinkClickedEventArgs arg)
        {
            var info = ScheduleHelper.GetTask(ScheduledTaskType.Analysis);
            if (info != null &&
                info.NextRunTime.HasValue &&
                info.NextRunTime.Value > DateTime.Now)
            {
                if (DialogResult.Yes != ApplicationMessageBox.ShowQuestion(this, String.Format("Are you sure you want to cancel the one time analysis scheduled for server {0}?", info.InstanceName)))
                {
                    return;
                }
                try
                {
                    ScheduleHelper.DeleteTask(ScheduledTaskType.Analysis);
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(this, "Error deleting scheduled analysis", e);
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this, "Scheduled analysis not found.");
            }

            UpdateOneTimeAnalysisInfo();
        }

        private void ScheduleSettingsDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
        }

        private void ScheduleSettingsDialog_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            UpdateScheduleSettings();
            UpdateOneTimeAnalysisInfo();
        }

        private void ScheduleSettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_settingsChanged && ((CloseReason.UserClosing == e.CloseReason) || (CloseReason.None == e.CloseReason)))
            {
                DialogResult dr = ApplicationMessageBox.ShowQuestion(this, "Changes have been made.\n\nWould you like to save these changes before exiting?", Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNoCancel, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1);
                switch (dr)
                {
                    case DialogResult.Yes: { if (!SaveChanges()) e.Cancel = true; break; }
                    case DialogResult.No: { break; }
                    case DialogResult.Cancel: { e.Cancel = true; break; }
                }
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!SaveChanges()) DialogResult = DialogResult.None;
        }

        private void ScheduleSettingsDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ShowHelp();
        }

        private void ShowHelp()
        {
            HelpTopics.ShowHelpTopic(HelpTopics.ScheduleAnalysis);
        }

        private void NumericAnalysisDuration_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                e.Handled = true;
            }
        }

        private void NumericAnalysisDuration_ValueChanged(object sender, EventArgs e)
        {
            OnSettingsChanged(EventArgs.Empty);
        }
    }
}
