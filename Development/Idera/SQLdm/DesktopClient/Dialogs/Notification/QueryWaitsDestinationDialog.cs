using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common;
using System.ComponentModel;
//Query Waits Alert Action Response
//10.1 Srishti Purohit SQLdm
namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;

    public partial class QueryWaitsDestinationDialog : BaseDialog
    {
        private int durationInMinutes = -1;


        public QueryWaitsDestinationDialog()
        {
            this.DialogHeader = "Query Waits Settings";
            InitializeComponent();
            AdaptFontSize();
        }

        public int QWaitsDurationInMinutes
        {
            get
            {
                return durationInMinutes;
            }
            set
            {
                durationInMinutes = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (enableForeverButton.Checked)
                durationInMinutes = 0;
            else
            {
                durationInMinutes = (int) durationInMunitesSpinner.Value;
            }
        }

        private void QueryWaitsDestinationDialog_Load(object sender, EventArgs e)
        {
            if (durationInMinutes < 1)
            {
                durationInMunitesSpinner.Value = 15;
                enableForeverButton.Checked = true;
            }
            else
            {
                durationInMunitesSpinner.Value = durationInMinutes;
                enableForeverButton.Checked = false;
            }
            enableLimitedButton.Checked = !enableForeverButton.Checked;
        }

        private void enableLimitedButton_CheckedChanged(object sender, EventArgs e)
        {
            bool durationSpinnerEnabled = enableLimitedButton.Checked;
            durationInMunitesSpinner.Enabled = durationSpinnerEnabled;
            label1.Enabled = durationSpinnerEnabled;
            label4.Enabled = durationSpinnerEnabled;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
     
        /// <summary>
        /// Opens Wiki link on clicking of help button
        /// </summary>
        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            //SQLdm 10.1 (pulkit Puri) --sqldm 26072 fix
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }
    }
}