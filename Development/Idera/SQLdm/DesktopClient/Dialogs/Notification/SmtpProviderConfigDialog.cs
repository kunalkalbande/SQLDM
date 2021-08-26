using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Text;
    using System.Windows.Forms;
    using Common.Notification;
    using Common.Notification.Providers;
    using Common.Services;
    using Common.UI.Dialogs;
    using Infragistics.Win.Misc;
    using System.ComponentModel;
    using Microsoft.SqlServer.MessageBox;

    public partial class SmtpProviderConfigDialog : BaseDialog, INotificationProviderConfigDialog
    {
        private SmtpNotificationProviderInfo providerInfo;
        private IManagementService managementService;
        private bool senderInfoChanged;

        //private bool nameValidated;
        private bool serverValidated;
        private bool authValidated;
        private bool senderValidated;

        public SmtpProviderConfigDialog()
        {
            this.DialogHeader = "SMTP Action Provider";

            InitializeComponent();

            // Autoscale FontSize.
            AdaptFontSize();
        }

        public SmtpProviderConfigDialog(IManagementService managementService) : this()
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
                providerInfo = value as SmtpNotificationProviderInfo;
                if (providerInfo != null)
                {
                    updateForm();
                }
            }
        }

        private void timeoutSlider_ValueChanged(object sender, EventArgs e)
        {
            timeoutEditor.Value = timeoutSlider.Value;
            timeoutLabel.Text = providerInfo.ReadableSeconds(int.Parse(timeoutEditor.Value.ToString()));
        }

        private void timeoutEditor_ValueChanged(object sender, EventArgs e)
        {
            if (timeoutEditor.Value == DBNull.Value)
                return;

            timeoutSlider.Value = (int)Convert.ChangeType(timeoutEditor.Value, typeof(int));
        }

        private void updateForm()
        {
            providerName.Text = GetSafeValue(providerInfo.Name, "");
            providerInfo.Enabled = true;
            serverAddress.Text = GetSafeValue(providerInfo.SmtpServer, "");
            serverPort.Value = GetSafeValue(providerInfo.SmtpServerPort, 25);
            timeoutEditor.Value = GetSafeValue(providerInfo.LoginTimeout, 90);
            requiresAuthentication.Checked = GetSafeValue(providerInfo.RequiresAuthentication, false);
            requiresSSL.Checked = GetSafeValue(providerInfo.RequiresSSL, false);
            if (providerInfo.RequiresAuthentication)
            {
                logonName.Text = GetSafeValue(providerInfo.UserName, "");
                password.Text = GetSafeValue(providerInfo.Password, "");
            }

            fromAddress.Text = GetSafeValue(providerInfo.SenderAddress, "");
            fromName.Text = GetSafeValue(providerInfo.SenderName, "");

            UpdateAuthenticationControls();

            senderInfoChanged = false;
        }


        private T GetSafeValue<T>(object value, T defaultValue)
        {
            if (value == null)
                return defaultValue;
            return (T)value;
        }

        private void updateProviderInfo()
        {
            providerInfo.Name = providerName.Text;
            providerInfo.Enabled = true;
            providerInfo.SmtpServer = serverAddress.Text;
            providerInfo.SmtpServerPort = (int)serverPort.Value;
            object tev = timeoutEditor.Value;
            providerInfo.LoginTimeout = tev == null ? 90 : (int)Convert.ChangeType(tev, typeof (int));
            providerInfo.RequiresAuthentication = requiresAuthentication.Checked;
            providerInfo.RequiresSSL = requiresSSL.Checked;
            if (providerInfo.RequiresAuthentication)
            {
                providerInfo.UserName = logonName.Text;
                providerInfo.Password = password.Text;
            }
            providerInfo.RetryAttempts = 3;

            if (!GetSafeValue(providerInfo.SenderAddress, "").Equals(fromAddress.Text.Trim()) ||
                !GetSafeValue(providerInfo.SenderName, "").Equals(fromName.Text.Trim()))
            {
                senderInfoChanged = true;
            }
            providerInfo.SenderAddress = fromAddress.Text;
            providerInfo.SenderName = fromName.Text;
        }

        private void testButton_Click(object sender, EventArgs args)
        {
            updateProviderInfo();
            
            SmtpDestinationsDialog sdd = new SmtpDestinationsDialog();
            sdd.SetNotificationProviders(new SmtpNotificationProviderInfo[] { providerInfo });
            sdd.Destination = new SmtpDestination();
            sdd.TestMode = true;

            if (sdd.ShowDialog(this) == DialogResult.OK)
            {
                this.Update();
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    if (managementService.TestAction(providerInfo, sdd.Destination, null) == 0)
                        ApplicationMessageBox.ShowInfo(this, "Message sent to the SMTP Server.");
                    else
                        ApplicationMessageBox.ShowWarning(this, "Attempt to send message failed but no exception was generated.");
                } catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(this, "Failed to send test email.", e);
                } finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
    
        private void okButton_Click(object sender, EventArgs args)
        {
                updateProviderInfo();

                if (NotificationProvider.Id == default(Guid))
                    DialogResult = AddNotificationProvider();
                else
                    DialogResult = UpdateNotificationProvider();
        }

        private DialogResult UpdateNotificationProvider()
        {
            DialogResult result = DialogResult.None;
            bool updateRules = false;
            try
            {
                if (senderInfoChanged)
                {
                    if (ApplicationMessageBox.ShowQuestion(this, 
                        "Would you like to update sender information in existing Notification Rules using this provider?", 
                        ExceptionMessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        updateRules = true;
                    }
                }

                managementService.UpdateNotificationProvider(NotificationProvider, updateRules);

                // reload rules because they could have changed
                if (updateRules)
                {
                    NotificationRulesDialog nrd = Owner as NotificationRulesDialog;
                    if (nrd != null)
                        nrd.ReloadNotificationRules();
                }

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

        private DialogResult AddNotificationProvider()
        {
            DialogResult result = DialogResult.None;
            try
            {
                NotificationProviderInfo newProvider = managementService.AddNotificationProvider(NotificationProvider);
                NotificationProvider = new SmtpNotificationProviderInfo(newProvider);
                result = DialogResult.OK;
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to add the notification provider at this time",
                                                e);
            }
            return result;
        }

        private void requiresAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAuthenticationControls();
        }

        private void UpdateAuthenticationControls()
        {
            bool check = requiresAuthentication.Checked;
            logonName.Enabled = check;
            password.Enabled = check;
            if (check)
                logonName.Focus();
            UpdateControls();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void SmtpProviderConfigDialog_Load(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            bool OK = true;

            Validation check = validator.Validate("Name", true, false);
            if (!check.IsValid)
                OK = false; ;

            if (OK || serverValidated)
            {
                check = validator.Validate("Server", true, false);
                if (!check.IsValid)
                    OK = false;
            } 

            if (OK || authValidated)
            {
                check = validator.Validate("Authentication", true, false);
                if (!check.IsValid)
                    OK = false;
            }

            if (OK || senderValidated)
            {
                check = validator.Validate("Sender", true, false);
                if (!check.IsValid)
                    OK = false;
            }

            okButton.Enabled = OK;
            testButton.Enabled = OK;
        }

        private void SmtpProviderConfigDialog_Shown(object sender, EventArgs e)
        {
            if (providerName.Text.Length == 0)
                providerName.Focus();
            else
                serverAddress.Focus();
        }

        private void serverPort_BeforeExitEditMode(object sender, Infragistics.Win.BeforeExitEditModeEventArgs e)
        {
            if (sender == serverPort)
            {
                if (serverPort.Value == DBNull.Value)
                    serverPort.Value = providerInfo.SmtpServerPort;
            } else
            if (sender == this.timeoutEditor)
            {
                if (timeoutEditor.Value == DBNull.Value)
                    timeoutEditor.Value = providerInfo.LoginTimeout;
            } 

        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardSMTP);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardSMTP);
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
                case "Server":
                    serverValidated = true;
                    e.IsValid = (UriHostNameType.Unknown != Uri.CheckHostName(value)) ? true : false;
                    break; ;
                case "Authentication":
                    authValidated = true;
                    if (!requiresAuthentication.Checked)
                        e.IsValid = true;
                    else
                        e.IsValid = !String.IsNullOrEmpty(value);
                    break;
                case "Sender":
                    senderValidated = true;
                    string from = fromName.Text.Trim();
                    from = (from.Length == 0) ? fromAddress.Text : String.Format("{0}<{1}>", from, fromAddress.Text);
                    if (String.IsNullOrEmpty(from) || !SmtpAddressHelpers.IsMailAddressValid(from, true))
                        e.IsValid = false;
                    else
                        e.IsValid = true;

                    break;
            }
        }

        private void timeoutEditor_ValidationError(object sender, Infragistics.Win.UltraWinEditors.ValidationErrorEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            error.AppendFormat("The timeout value must be between {0} and {1} seconds", timeoutEditor.MinValue, timeoutEditor.MaxValue);

            ApplicationMessageBox.ShowError(this, error.ToString());
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
