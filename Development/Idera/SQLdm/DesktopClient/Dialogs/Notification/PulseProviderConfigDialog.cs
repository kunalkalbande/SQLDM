using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.Misc;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    public partial class PulseProviderConfigDialog : Form, INotificationProviderConfigDialog
    {
        private PulseNotificationProviderInfo providerInfo;
        private IManagementService managementService;
        private bool nameValidated;
        private bool addressValidated;
        private bool portValidated;

        public PulseProviderConfigDialog()
        {
            InitializeComponent();

            //editing the pulse provider from dm is currently not allowed, so disable these features
            // note: when enabling editing the server again, a check needs to be added to reset the news feed connection
            //          when the server is changed
            providerName.Enabled =
                pulseAddressTextBox.Enabled = false;
            testButton.Visible =
                okButton.Visible = false;
            AdaptFontSize();
        }

        public PulseProviderConfigDialog(IManagementService managementService) : this()
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
                providerInfo = value as PulseNotificationProviderInfo;
                if (providerInfo != null)
                {
                    updateForm();
                }
            }
        }

        private void updateForm()
        {
            providerName.Text = GetSafeValue(providerInfo.Name, "Newsfeed Action Provider");

            pulseAddressTextBox.Text = GetSafeValue(providerInfo.PulseServer, "");
            pulsePortSpinner.Value = GetSafeValue(providerInfo.PulseServerPort, 5168);
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
            providerInfo.PulseServer = pulseAddressTextBox.Text.Trim();
            providerInfo.Enabled = true;
            providerInfo.PulseServerPort = (int)pulsePortSpinner.Value;
        }

        private void updateControls()
        {
            bool OK = true;
            Validation check;

            check = validator.Validate("Name", true, false);
            OK = check.IsValid;

            if (OK || addressValidated)
            {
                check = validator.Validate("PulseAddress", true, false);
                OK = check.IsValid;
            }

            if (OK || portValidated)
            {
                check = validator.Validate("PulsePort", true, false);
                OK = check.IsValid;
            }

            okButton.Enabled =
                testButton.Enabled = OK;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            updateProviderInfo();

            PulseDestination destination = new PulseDestination();

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (managementService.TestAction(providerInfo, destination, null) == 0)
                    ApplicationMessageBox.ShowInfo(this, "The Newsfeed service was successfully contacted.");
                else
                    ApplicationMessageBox.ShowWarning(this, "Attempt to contact the Newsfeed service failed but no exception was generated.");
            }
            catch (Exception except)
            {
                ApplicationMessageBox.ShowError(this, "Failed to contact the Newsfeed service.", except);
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
            bool updateRules = false;
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

        private void PulseProviderConfigDialog_Load(object sender, EventArgs e)
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
                    nameValidated = true;
                    e.IsValid = !String.IsNullOrEmpty(value);
                    break;
                case "PulseAddress":
                    addressValidated = true;
                    e.IsValid = !String.IsNullOrEmpty(value);
                    break;
                case "PulsePort":
                    portValidated = true;
                    e.IsValid = true;
                    break; ;
            }
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