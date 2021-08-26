using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class ConfigureLogsDialog : BaseDialog
    {
        #region constants

        private const string ARCHIVE_INFO = @"Note: SQL Server archive files must be between 6 and 99";
        private const int MIN_VALUE = 6;
        private const int MAX_VALUE = 99;

        #endregion

        #region fields

        private readonly int instanceId;

        #endregion

        #region constructors

        public ConfigureLogsDialog(int instanceId)
        {
            this.DialogHeader = "Configure SQL Server Logs";
            InitializeComponent();
            AdaptFontSize();
            this.instanceId = instanceId;

            infoLabel.Text = ARCHIVE_INFO;

            archiveNumberNumericEditor.MinValue = MIN_VALUE;
            archiveNumberNumericEditor.MaxValue = MAX_VALUE;

            archiveNumberNumericEditor.Value = MIN_VALUE;
        }

        public ConfigureLogsDialog(int instanceId, int? maxLogs, bool unlimited)
        {
            this.DialogHeader = "Configure SQL Server Logs";
            InitializeComponent();
            AdaptFontSize();
            this.instanceId = instanceId;

            infoLabel.Text = ARCHIVE_INFO;

            archiveNumberNumericEditor.MinValue = MIN_VALUE;
            archiveNumberNumericEditor.MaxValue = MAX_VALUE;

            unlimitedCheckBox.Checked = unlimited;
            if (maxLogs.HasValue)
            {
                archiveNumberNumericEditor.Value = Math.Max(MIN_VALUE, Math.Min(MAX_VALUE, maxLogs.Value));
            }
            else
            {
                archiveNumberNumericEditor.Value = MIN_VALUE;
            }
            archiveNumberNumericEditor.Enabled = !unlimitedCheckBox.Checked;
        }

        #endregion

        #region properties

        #endregion

        #region events

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureLogs);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureLogs);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            SetNumberOfLogsConfiguration config = new SetNumberOfLogsConfiguration(instanceId, unlimitedCheckBox.Checked, (int)archiveNumberNumericEditor.Value );
            Snapshot snapshot = managementService.SendSetNumberOfLogs(config);

            if (snapshot.Error != null)
            {
                throw snapshot.Error;
                //DialogResult = DialogResult.None;
            }
        }

        private void unlimitedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            archiveNumberNumericEditor.Enabled = !unlimitedCheckBox.Checked;
        }


        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Control);
        }

        #endregion
    }
}
