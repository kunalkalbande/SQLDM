using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    /// <summary>
    /// This dialog is used for selecting single or multiple tables for actions.
    /// </summary>
    internal partial class CycleLogsDialog : Idera.SQLdm.DesktopClient.Dialogs.BaseDialog
    {
        #region constants

        #endregion

        #region fields

        #endregion

        #region constructors

        public CycleLogsDialog(bool AgentSupported)
        {
            this.DialogHeader = " Cycle Logs";
            InitializeComponent();

            sqlAgentCheckBox.Enabled = AgentSupported;
            AdaptFontSize();
        }

        #endregion

        #region properties

        public bool CycleSqlServer
        {
            get { return sqlServerCheckBox.Checked; }
        }

        public bool CycleSqlAgent
        {
            get { return sqlAgentCheckBox.Checked; }
        }

        #endregion

        #region methods

        #endregion

        #region helpers

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        #endregion

        #region events

        private void sqlServerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            okButton.Enabled = sqlServerCheckBox.Checked || sqlAgentCheckBox.Checked;
        }

        private void sqlAgentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            okButton.Enabled = sqlServerCheckBox.Checked || sqlAgentCheckBox.Checked;
        }
                     
        protected override void OnHelpButtonClicked(System.ComponentModel.CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CycleLogs);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.CycleLogs);
        }

        #endregion
    }
}