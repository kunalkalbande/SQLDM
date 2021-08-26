using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Notification;
    using System.ComponentModel;

    public partial class StateSelectionDialog : BaseDialog
    {
        private NotificationRule rule;

        public StateSelectionDialog()
        {
            this.DialogHeader = "Severity";
            InitializeComponent();
            AdaptFontSize();
        }

        public NotificationRule Rule
        {
            get { return rule; }
            set { rule = value; }
        }

        private void StateSelectionDialog_Load(object sender, EventArgs e)
        {
            MetricStateRule msr = rule.StateComparison;
            if (rule != null)
            {
                chkOK.Checked = msr.IsOK;
                chkInfo.Checked = msr.IsInformational;
                chkWarning.Checked = msr.IsWarning;
                chkCritical.Checked = msr.IsCritical;
            }
            UpdateControl();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MetricStateRule msr = rule.StateComparison;
            if (rule == null)
            {
                msr = new MetricStateRule();
                rule.StateComparison = msr;
            }
            msr.IsOK = chkOK.Checked;
            msr.IsInformational = chkInfo.Checked;
            msr.IsWarning = chkWarning.Checked;
            msr.IsCritical = chkCritical.Checked;
        }

        private void UpdateControl()
        {
            btnOK.Enabled = chkOK.Checked || chkWarning.Checked || chkCritical.Checked || chkInfo.Checked;
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            UpdateControl();
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
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