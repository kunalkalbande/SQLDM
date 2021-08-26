using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Microsoft.SqlServer.MessageBox;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class EditConfigurationValueDialog : BaseDialog
    {
        private readonly int instanceId;
        private readonly string configurationOptionName;
        private readonly int minimumValue;
        private readonly int maximumValue;

        public EditConfigurationValueDialog(int instanceId, string configurationOptionName, int minimumValue, int maximumValue, bool restartRequired)
        {
            this.DialogHeader = "Edit Configuration Value";
            InitializeComponent();

            this.instanceId = instanceId;
            this.configurationOptionName = configurationOptionName;
            this.minimumValue = minimumValue;
            this.maximumValue = maximumValue;

            if (restartRequired)
            {
                informationBox.Text =
                    "Changes to this configuration value will not take effect until the SQL Server instance is restarted.";
            }
            else
            {
                informationBox.Text = "Changes to this configuration value will take effect immediately.";
            }

            AdaptFontSize();
        }

        private void newValueTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = newValueTextBox.Text.Trim().Length > 0;
        }

        private void newValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int newValue;

            try
            {
                newValue = Convert.ToInt32(newValueTextBox.Text);
            }
            catch
            {
                ApplicationMessageBox.ShowInfo(this, "The configuration value must be a 32-bit integer value.");
                return;
            }

            if (newValue < minimumValue)
            {
                DialogResult result = ApplicationMessageBox.ShowWarning(this,
                                                  "The configuration value specified is less than the minimum recommended value. Woud you like to continue?",
                                                  ExceptionMessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    return;
                }
            }
            else if (newValue > maximumValue)
            {
                DialogResult result = ApplicationMessageBox.ShowWarning(this,
                                                  "The configuration value specified is greater than the maximum recommended value. Woud you like to continue?",
                                                  ExceptionMessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            okButton.Enabled = false;
            cancelButton.Enabled = false;
            backgroundWorker.RunWorkerAsync(
                    new Pair<string, int>(configurationOptionName, newValue));
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "EditConfigurationValueWorker";
            
            if (e.Argument is Pair<string, int>)
            {
                Pair<string, int> argument = (Pair<string, int>)e.Argument;

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                ReconfigurationConfiguration configuration =
                    new ReconfigurationConfiguration(instanceId, argument.First, argument.Second);

                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                
                e.Result = managementService.SendReconfiguration(configuration);
            }

            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while applying the configuration change.",
                                                e.Error);
                okButton.Enabled = true;
                cancelButton.Enabled = true;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void EditConfigurationValueDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.EditConfigurationValue);
        }

        private void EditConfigurationValueDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.EditConfigurationValue);
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