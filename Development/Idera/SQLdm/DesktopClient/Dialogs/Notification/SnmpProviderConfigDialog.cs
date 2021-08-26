using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Text;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win.Misc;
    using Infragistics.Win.UltraWinEditors;
    using System.ComponentModel;
    using Microsoft.SqlServer.MessageBox;
    using Idera.SQLdm.DesktopClient.Helpers;

    public partial class SnmpProviderConfigDialog : BaseDialog, INotificationProviderConfigDialog
    {
        private SnmpNotificationProviderInfo providerInfo;
        private SnmpNotificationProviderInfo oldProvider;
        private IManagementService managementService;

        //private bool nameValidated;
        private bool managerAddressValidated;

        public SnmpProviderConfigDialog()
        {
            this.DialogHeader = "SNMP Action Provider";
            InitializeComponent();
            AdaptFontSize();
        }

        public SnmpProviderConfigDialog(IManagementService managementService) : this()
        {
            this.managementService = managementService;
        }

        public void SetManagementService(IManagementService managementService)
        {
            this.managementService = managementService;
        }

        public NotificationProviderInfo NotificationProvider
        {
            get { return providerInfo;  }
            set
            {
                providerInfo = value as SnmpNotificationProviderInfo;
                if (providerInfo != null)
                {
                    oldProvider = (SnmpNotificationProviderInfo)providerInfo.Clone();
                    updateForm();
                }
            }
        }

        private void updateForm()
        {
            providerName.Text = GetSafeValue(providerInfo.Name, "");
            providerInfo.Enabled = true;
            managerAddress.Text = GetSafeValue(providerInfo.Address, "");
            managerPort.Value = GetSafeValue(providerInfo.Port, 162);
            managerCommunity.Text = GetSafeValue(providerInfo.Community, "public");

        }

        private T GetSafeValue<T>(object value, T defaultValue)
        {
            if (value == null)
                return defaultValue;
            return (T)value;
        }

        private void updateProviderInfo()
        {
            providerInfo.Name = providerName.Text.Trim();
            providerInfo.Enabled = true;
            providerInfo.Address = managerAddress.Text.Trim();
            providerInfo.Port = (int)managerPort.Value;
            providerInfo.Community = managerCommunity.Text.Trim();

        }

        private void updateControls()
        {
            bool OK = true;
            Validation check;

            check = validator.Validate("Name", true, false);
            OK = check.IsValid;

            if (OK || managerAddressValidated)
            {
                check = validator.Validate("ManagerAddress", true, false);
                OK = check.IsValid;
            }

            okButton.Enabled = testButton.Enabled = OK;

        }

        private void testButton_Click(object sender, EventArgs e)
        {
            updateProviderInfo();

            SnmpDestination destination = new SnmpDestination();

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (managementService.TestAction(providerInfo, destination, null) == 0)
                    ApplicationMessageBox.ShowInfo(this, "Trap sent to the SNMP Manager.");
                else
                    ApplicationMessageBox.ShowWarning(this, "Attempt to SNMP Trap failed but no exception was generated.");
            }
            catch (Exception except)
            {
                ApplicationMessageBox.ShowError(this, "Failed to send SNMP Trap.", except);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void okButton_Click(object sender, EventArgs args)
        {
            updateProviderInfo();

            if (NotificationProvider.Id != default(Guid))
                DialogResult = UpdateNotificationProvider();
            else
                ApplicationMessageBox.ShowWarning(this, "Unable to update the specified notification provider");

        }

        private DialogResult UpdateNotificationProvider()
        {
            DialogResult result = DialogResult.None;
            //bool updateRules = false;
            try
            {

                managementService.UpdateNotificationProvider(NotificationProvider, false);

                result = DialogResult.OK;
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to save the notification provider at this time",
                                                e);
            }
            return result;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        private void SnmpProviderConfigDialog_Load(object sender, EventArgs e)
        {
            updateControls();
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardSNMP);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardSNMP);
        }

        private void validator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            string value = e.Value == null ? null : e.Value.ToString().Trim();

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "Name":
                    //nameValidated = true;
                    e.IsValid = !String.IsNullOrEmpty(value);
                    break;
                case "ManagerAddress":
                    managerAddressValidated = true;
                    e.IsValid = ((UriHostNameType.Unknown == Uri.CheckHostName(value)) ||
                                 (UriHostNameType.IPv6 == Uri.CheckHostName(value))) ? false : true;
                    break; ;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            providerInfo.Name = oldProvider.Name;
            providerInfo.Port = oldProvider.Port;
            providerInfo.Address = oldProvider.Address;
            providerInfo.Community = oldProvider.Community;
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