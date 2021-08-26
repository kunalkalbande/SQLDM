using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Helpers;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Wintellect.PowerCollections;

    public partial class AddActionDialog : Form
    {
        private IList<NotificationProviderInfo> availableProviders;
        private List<NotificationProviderInfo> selectedProviders = new List<NotificationProviderInfo>();
        private ListViewItem newSmtpProviderItem;
        private bool _allowAddNew = true;

        public AddActionDialog(IList<NotificationProviderInfo> providerList)
        {
            InitializeComponent();
            if (!ApplicationModel.Default.IsTasksViewEnabled)
            {
                availableProviders = new List<NotificationProviderInfo>();
                foreach (NotificationProviderInfo npi in providerList)
                {
                    if (npi is TaskNotificationProviderInfo)
                        continue;
                    availableProviders.Add(npi);
                }
            }
            else
            {
                availableProviders = providerList;
            }

            // Auto scale fontsize.
            AdaptFontSize();
        }

        public AddActionDialog(IList<NotificationProviderInfo> providerList, bool allowAddNew) : this(providerList)
        {
            _allowAddNew = allowAddNew;
        }

        public IList<NotificationProviderInfo> SelectedProviders
        {
            get { return selectedProviders; }
        }

        private void AddActionDialog_Load(object sender, EventArgs e)
        {
            newSmtpProviderItem = new ListViewItem("&lt; New Action Provider &gt;");

            // add special items
            if (_allowAddNew)
                providerListBox.Items.Add(newSmtpProviderItem);

            Algorithms.SortInPlace(availableProviders, CompareProviders);

            foreach (NotificationProviderInfo npi in availableProviders)
            {
                ListViewItem item = new ListViewItem(npi.Name);
                item.Tag = npi;

                providerListBox.Items.Add(item);
            }
        }

        private int CompareProviders(NotificationProviderInfo left, NotificationProviderInfo right)
        {
            return left.Name.CompareTo(right.Name);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedProviders.Clear();

            if (newSmtpProviderItem.Checked)
            {
                AddNewSmtpProvider();
            }

            foreach (ListViewItem item in providerListBox.CheckedItems)
            {
                if (item == newSmtpProviderItem)
                    continue;
                NotificationProviderInfo npi = item.Tag as NotificationProviderInfo;
                if (npi != null)
                    selectedProviders.Add(npi);
            }
        }

        private void AddNewSmtpProvider()
        {
            NotificationProviderInfo newNotificationProviderInfo = null;
            NotificationRulesDialog owner = Owner.Owner as NotificationRulesDialog;
            if (owner != null)
            {
                DialogResult dr = owner.ShowAddNotificationProviderDialog(this, ref newNotificationProviderInfo);
                if (dr == DialogResult.OK && 
                    ((newNotificationProviderInfo is SmtpNotificationProviderInfo) || (newNotificationProviderInfo is SnmpNotificationProviderInfo)))
                    selectedProviders.Add(newNotificationProviderInfo);
            }
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