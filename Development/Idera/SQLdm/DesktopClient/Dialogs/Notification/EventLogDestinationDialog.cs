using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Notification;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Idera.SQLdm.DesktopClient.Helpers;

    public partial class EventLogDestinationDialog : Form
    {
        private EventLogDestination[] destinations;
        private IManagementService managementService;

        public EventLogDestinationDialog(IManagementService managementService)
        {
            InitializeComponent();
            this.managementService = managementService;
            AdaptFontSize();
        }

        public EventLogDestination[] Destinations
        {
            get
            {
                if (destinations == null)
                    destinations = new EventLogDestination[0];
                return destinations;
            }
            set { destinations = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckedListBox.CheckedItemCollection cic = lstDestinations.CheckedItems;
            destinations = new EventLogDestination[cic.Count];
            int x = 0;
            foreach (NotificationProviderInfo provider in cic)
            {
                destinations[x] = new EventLogDestination();
                destinations[x].ProviderID = provider.Id;
                destinations[x].Provider = provider;
                x++;
            }
        }

        private void EventLogDestinationDialog_Load(object sender, EventArgs e)
        {
            IList<NotificationProviderInfo> providers = managementService.GetNotificationProviders();
            foreach (NotificationProviderInfo provider in providers)
            {
                if (provider is EventLogNotificationProviderInfo)
                {
                    bool check = MatchDestination(provider.Id);
                    lstDestinations.Items.Add(provider, check);
                }
            }
        }

        bool MatchDestination(Guid providerId)
        {
            foreach (EventLogDestination destination in Destinations)
            {
                if (destination.ProviderID == providerId)
                    return true;
            }
            return false;
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
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}