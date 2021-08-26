using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.DesktopClient.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class NotificationRulesDialog : Form
    {
        private bool providersInitialized;

        private IManagementService managementService;

        public NotificationRulesDialog(IManagementService managementService)
        {
            InitializeComponent();
            ultraTabControl1.DrawFilter = new HideFocusRectangleDrawFilter();
            this.managementService = managementService;
            notificationRulesViewPanel.LoadInstances(managementService);

            // Autoscale fontsize.
            AdaptFontSize();
        }

        public bool ReloadProviders
        {
            set { providersInitialized = !value; }
        }

        private void ultraTabControl1_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (e.Tab.TabPage == ultraTabPageControl2 && !providersInitialized)
            {
                InitializeNotificationProviders();
            } 
        }

        private void InitializeNotificationProviders()
        {
            this.notificationProviderViewPanel.ProviderEditorType = null;
// BMT            this.notificationProviderViewPanel.ProviderEditorType = typeof(SmtpProviderConfigDialog);
            this.notificationProviderViewPanel.LoadInstances(managementService);
            providersInitialized = true;
        }

        public DialogResult ShowAddNotificationProviderDialog(IWin32Window owner, ref NotificationProviderInfo newNotificationProviderInfo)
        {
            if (!providersInitialized)
            {
                InitializeNotificationProviders();
            }

            return this.notificationProviderViewPanel.DoAdd(owner, ref newNotificationProviderInfo);
        }

        public IList<NotificationProviderInfo> GetNotificationProviders()
        {
            if (!providersInitialized)
                InitializeNotificationProviders();

            return notificationProviderViewPanel.GetNotificationProviders();
        }

        public IList<NotificationRule> GetNotificationRules()
        {
            return notificationRulesViewPanel.GetRules();
        }

        public void ReloadNotificationRules()
        {
            notificationRulesViewPanel.Reload();
        }

        private void notificationRulesViewPanel_ApplyStateChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = notificationRulesViewPanel.IsApplyNeeded ||
                                  notificationProviderViewPanel.IsApplyNeeded;
            okButton.Enabled = applyButton.Enabled;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (notificationRulesViewPanel.IsApplyNeeded)
                notificationRulesViewPanel.ApplyChanges();
            if (notificationProviderViewPanel.IsApplyNeeded)
                notificationProviderViewPanel.ApplyChanges();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            applyButton_Click(sender, e);
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureNotificationRulesAndProviders);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConfigureNotificationRulesAndProviders);
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}