using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Settings = Idera.SQLdm.DesktopClient.Properties.Settings;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.Common;
using System.Runtime.InteropServices;
using Infragistics.Win;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    public partial class AddActionProviderWizard : Form
    {
        Type typeOfProvider;
        NotificationProviderInfo newProviderInfo;
        IList<NotificationProviderInfo> existingProviders;
        private IManagementService managementService;

        private bool serverValidated;
        private bool authValidated;
        private bool senderValidated;

        private bool configurePulse = false;
        private string pulseName = "IDERA Newsfeed";

        #region Contructors

        public AddActionProviderWizard(IManagementService managementService, IList<NotificationProviderInfo> existingProviders)
        {
            this.managementService = managementService;
            this.existingProviders = existingProviders;
            InitializeComponent();

            // this prevents multiple pulse notification providers from being added to dm
            // however, initially, secondary applications are not being allowed
            // so I'm commenting out the check and just always removing the pulse provider

            //foreach (NotificationProviderInfo npi in existingProviders)
            //{
            //    if (npi is PulseNotificationProviderInfo)
            //    {
                    ValueListItem pulseItem = providerType.Items.ValueList.FindByDataValue("News");
                    if (pulseItem != null)
                    {
                        providerType.Items.Remove(pulseItem);
                        pulseItem.Dispose();
                    }
            //        break;
            //    }
            //}
            AdaptFontSize();
        }

        public AddActionProviderWizard(IManagementService managementService, string pulseProviderName)
        {
            this.managementService = managementService;
            this.existingProviders = new List<NotificationProviderInfo>();
            InitializeComponent();

            configurePulse = true;
            pulseName = pulseProviderName;
            AdaptFontSize();
        }

        #endregion

        #region Properties


        public NotificationProviderInfo NotificationProvider
        {
            get
            {
                return newProviderInfo;
            }
            set
            {
                if (value.GetType() == typeof(SmtpNotificationProviderInfo))
                {
                    newProviderInfo = value as SmtpNotificationProviderInfo;
                }
                else if (value.GetType() == typeof(SnmpNotificationProviderInfo))
                {
                    newProviderInfo = value as SnmpNotificationProviderInfo;
                }
                else if (value.GetType() == typeof(PulseNotificationProviderInfo))
                {
                    newProviderInfo = value as PulseNotificationProviderInfo;
                }
            }
        }

        private bool IsProviderTypePageComplete
        {
            get
            {
                if (((providerType.SelectedItem.DataValue.ToString() != "EMPTY") && (providerType.SelectedItem != null)) && (!string.IsNullOrEmpty(providerName.Text.Trim())))
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Wizard Methods

        // BMT - Need to create a single AddNotificationProvider method that accepts the Notification Provider object to 
        //        add and adds it.
        private void wizardFramework_Finish(object sender, EventArgs e)
        {

            if (newProviderInfo is SmtpNotificationProviderInfo)
            {
                UpdateSmtpProviderInfo();

                if (NotificationProvider.Id == default(Guid))
                    DialogResult = AddSmtpNotificationProvider();
            }
            else if (newProviderInfo is SnmpNotificationProviderInfo)
            {
                UpdateSnmpProviderInfo();

                if (NotificationProvider.Id == default(Guid))
                    DialogResult = AddSnmpNotificationProvider();

            }
            else if (newProviderInfo is PulseNotificationProviderInfo)
            {
                UpdatePulseProviderInfo();

                if (NotificationProvider.Id == default(Guid))
                    DialogResult = AddPulseNotificationProvider();
            }
        }

        private DialogResult AddPulseNotificationProvider()
        {
            DialogResult result = DialogResult.None;
            try
            {
                NotificationProviderInfo newProvider = (NotificationProviderInfo)NotificationProvider.Clone();

                GuidAttribute attribute = Attribute.GetCustomAttribute(typeof(PulseNotificationProviderInfo), 
                                                                        typeof(GuidAttribute)) as GuidAttribute;
                if (attribute != null)
                    newProvider.Id = new Guid(attribute.Value);

                newProvider = managementService.AddNotificationProvider(newProvider);
                NotificationProvider = new PulseNotificationProviderInfo(newProvider);
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


        private void 
            UpdatePulseProviderInfo()
        {
            newProviderInfo.Name = providerName.Text.Trim();
            newProviderInfo.Enabled = true;
            ((PulseNotificationProviderInfo)newProviderInfo).PulseServer = pulseAddressTextBox.Text.Trim();
            ((PulseNotificationProviderInfo)newProviderInfo).PulseServerPort = (int)pulsePortSpinner.Value;
//            ((PulseNotificationProviderInfo)newProviderInfo).AdminEmail = pulseEmailTextBox.Text.Trim();
        }

        private void AddActionProviderWizard_Load(object sender, EventArgs e)
        {
            hideWelcomePageCheckbox.Checked = Settings.Default.HideAddActionProviderWelcomePage;

            if (Settings.Default.HideAddActionProviderWelcomePage || configurePulse)
                wizardFramework.GoNext();
            if (configurePulse)
            {
                providerType.SelectedItem = providerType.Items.ValueList.FindByDataValue("News");
                providerName.Text = pulseName;
                wizardFramework.GoNext();
            }
        }

        #endregion

        #region Welcome Page Methods

        // BMT make sure this is working...
        private void hideWelcomePageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HideAddActionProviderWelcomePage = hideWelcomePageCheckbox.Checked;
        }

        #endregion

        #region Provider Type Methods

        private void providerType_SelectionChanged(object sender, EventArgs e)
        {
            switch ((string)providerType.SelectedItem.DataValue)
            {
                case "SMTP":  
                    chooseProviderTypePage.NextPage = smtpConfig;
                    break;
                case "SNMP":
                    chooseProviderTypePage.NextPage = snmpConfig;
                    break;
                case "News":
                    chooseProviderTypePage.NextPage = pulseConfig;
                    break;
                case "EMPTY":
                    chooseProviderTypePage.NextPage = smtpConfig;
                    break;
            }
            chooseProviderTypePage.AllowMoveNext = IsProviderTypePageComplete;
        }

        private void chooseProviderTypePage_BeforeDisplay(object sender, EventArgs e)
        {
            // I don't like this, but rather than having the providerType drop down default to NULL
            //   I had to create an entry in the collection for the drop down for an 'Empty' value because
            //   for some reason there was an issue with the drop down control in that if you open the 
            //   drop down and then close it without selecting a value (leaving the selected item as NULL)
            //   it would render the control buttons at the bottom of the dialog useless... 
            providerType.SelectedIndex = 0;
            chooseProviderTypePage.AllowMoveNext = IsProviderTypePageComplete;
        }

        // BMT - Need to make sure that the logic that checks the providerName against existing provider Names works
        private void chooseProviderTypePage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            foreach (NotificationProviderInfo npi in existingProviders)
            {
                if (npi.Name.Trim().ToLower() == providerName.Text.Trim().ToLower())
                {
                    ApplicationMessageBox.ShowError(this, "The provider name must be unique.");
                    e.Cancel = true;
                    return;
                }
            }

            if (providerType.SelectedItem.DataValue.ToString() == "SMTP")
            {
                NotificationProvider = new SmtpNotificationProviderInfo();
            }
            else 
            if (providerType.SelectedItem.DataValue.ToString() == "SNMP")
            {
                NotificationProvider = new SnmpNotificationProviderInfo();
            }
            else
            if (providerType.SelectedItem.DataValue.ToString() == "News")
            {
                NotificationProvider = new PulseNotificationProviderInfo();
            }

            typeOfProvider = newProviderInfo.GetType();
        }

        #endregion

        #region SMTP Config Methods

        #region SMTP Page Methods

        private void smtpConfig_BeforeDisplay(object sender, EventArgs e)
        {
            updateSmtpControls();
            requiresAuthentication.Checked = logonName.Enabled = password.Enabled = false;
            requiresSSL.Checked = false;
            serverPort.Value = GetSafeValue(((SmtpNotificationProviderInfo)newProviderInfo).SmtpServerPort, 25);
            timeoutEditor.Value = GetSafeValue(((SmtpNotificationProviderInfo)newProviderInfo).LoginTimeout, 90);

        }

        #endregion

        #region SMTP Control Methods

        private void timeoutSlider_ValueChanged(object sender, EventArgs e)
        {
            timeoutEditor.Value = timeoutSlider.Value;
            updateTimeoutLabel();
        }

        private void timeoutEditor_ValueChanged(object sender, EventArgs e)
        {
            if (timeoutEditor.Value == DBNull.Value)
                return;

            timeoutSlider.Value = (int)Convert.ChangeType(timeoutEditor.Value, typeof(int));
        }

        private void updateTimeoutLabel()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(timeoutSlider.Value);
            int minutes = timeout.Minutes;
            int seconds = timeout.Seconds;

            StringBuilder builder = new StringBuilder();

            if (minutes > 0)
            {
                builder.AppendFormat("{0} minute", minutes);
                if (minutes > 1)
                    builder.Append('s');
                builder.Append(' ');
            }
            if (seconds > 0)
            {
                builder.AppendFormat("{0} second", seconds);
                if (seconds > 1)
                    builder.Append('s');
            }

            timeoutLabel.Text = builder.ToString();
        }

        private void requiresAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAuthenticationControls();
        }

        private void UpdateAuthenticationControls()
        {
            logonName.Enabled = password.Enabled = requiresAuthentication.Checked;

            if (logonName.Enabled)
            {
                Validation check = validator.Validate("Authentication", true, false);
                logonName.Focus();
            }
            updateSmtpControls();
        }

        private void updateSmtpControls()
        {
            bool OK = true;
            Validation check;

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

            smtpConfig.AllowMoveNext = OK;
            testSmtpButton.Enabled = OK;
        }

        private void UpdateSmtpProviderInfo()
        {
            newProviderInfo.Name = providerName.Text.Trim();
            newProviderInfo.Enabled = true;
            ((SmtpNotificationProviderInfo)newProviderInfo).SmtpServer = serverAddress.Text.Trim();
            ((SmtpNotificationProviderInfo)newProviderInfo).SmtpServerPort = (int)serverPort.Value;
            object tev = timeoutEditor.Value;
            ((SmtpNotificationProviderInfo)newProviderInfo).LoginTimeout = tev == null ? 90 : (int)Convert.ChangeType(tev, typeof(int));
            ((SmtpNotificationProviderInfo)newProviderInfo).RequiresAuthentication = requiresAuthentication.Checked;
            ((SmtpNotificationProviderInfo)newProviderInfo).RequiresSSL = requiresSSL.Checked;
            if (((SmtpNotificationProviderInfo)newProviderInfo).RequiresAuthentication)
            {
                ((SmtpNotificationProviderInfo)newProviderInfo).UserName = logonName.Text.Trim();
                ((SmtpNotificationProviderInfo)newProviderInfo).Password = password.Text.Trim();
            }
            ((SmtpNotificationProviderInfo)newProviderInfo).RetryAttempts = 3;
            ((SmtpNotificationProviderInfo)newProviderInfo).SenderAddress = fromAddress.Text.Trim();
            ((SmtpNotificationProviderInfo)newProviderInfo).SenderName = fromName.Text.Trim();
        }

        private void timeoutEditor_ValidationError(object sender, Infragistics.Win.UltraWinEditors.ValidationErrorEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            error.AppendFormat("The timeout value must be between {0} and {1} seconds", timeoutEditor.MinValue, timeoutEditor.MaxValue);

            ApplicationMessageBox.ShowError(this, error.ToString());
        }

        #endregion

        #region SMTP Action methods

        private void testSmtpButton_Click(object sender, EventArgs args)
        {
            UpdateSmtpProviderInfo();

            SmtpDestinationsDialog sdd = new SmtpDestinationsDialog();
            sdd.SetNotificationProviders(new SmtpNotificationProviderInfo[] { (SmtpNotificationProviderInfo)newProviderInfo });
            sdd.Destination = new SmtpDestination();
            sdd.TestMode = true;

            if (sdd.ShowDialog(this) == DialogResult.OK)
            {
                this.Update();
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    if (managementService.TestAction((SmtpNotificationProviderInfo)newProviderInfo, sdd.Destination, null) == 0)
                        ApplicationMessageBox.ShowInfo(this, "Message sent to the SMTP Server.");
                    else
                        ApplicationMessageBox.ShowWarning(this, "Attempt to send message failed but no exception was generated.");
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(this, "Failed to send test email.", e);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }

        }

        private DialogResult AddSmtpNotificationProvider()
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

        #endregion

        #endregion

        #region SNMP Config Methods

        private void snmpConfig_BeforeDisplay(object sender, EventArgs e)
        {
            updateSnmpControls();
            //int port;
            
            //if (!Int32.TryParse(((SnmpNotificationProviderInfo)newProviderInfo).Port, out port))
            //    port = 162;

            managerPort.Value = GetSafeValue(((SnmpNotificationProviderInfo)newProviderInfo).Port, 162);
            managerCommunity.Text = GetSafeValue(((SnmpNotificationProviderInfo)newProviderInfo).Community, "public");
        }

        private void testSnmpButton_Click(object sender, EventArgs e)
        {
            UpdateSnmpProviderInfo();

            SnmpDestination destination = new SnmpDestination();

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (managementService.TestAction((SnmpNotificationProviderInfo)newProviderInfo, destination, null) == 0)
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

        private DialogResult AddSnmpNotificationProvider()
        {
            DialogResult result = DialogResult.None;
            try
            {
                NotificationProviderInfo newProvider = managementService.AddNotificationProvider(NotificationProvider);
                NotificationProvider = new SnmpNotificationProviderInfo(newProvider);
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


        private void UpdateSnmpProviderInfo()
        {
            newProviderInfo.Name = providerName.Text.Trim();
            newProviderInfo.Enabled = true;
            ((SnmpNotificationProviderInfo)newProviderInfo).Address = managerAddress.Text.Trim();
            ((SnmpNotificationProviderInfo)newProviderInfo).Port = (int)managerPort.Value;
            ((SnmpNotificationProviderInfo)newProviderInfo).Community = managerCommunity.Text.Trim();
        }

        private void updateSnmpControls()
        {
            Validation check;

            check = validator.Validate("Manager", true, false);

            snmpConfig.AllowMoveNext = check.IsValid;
            testSnmpButton.Enabled = check.IsValid;
            
        }

        #endregion

        #region Pulse Config Methods

        private void pulseConfig_BeforeDisplay(object sender, EventArgs e)
        {
            updatePulseControls();
        }

        private void testPulseProvider_Click(object sender, EventArgs args)
        {
            UpdatePulseProviderInfo();

            this.Update();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (managementService.TestAction((PulseNotificationProviderInfo)newProviderInfo, new PulseDestination(), null) == 0)
                    ApplicationMessageBox.ShowInfo(this, "Message sent to the IDERA News Server.");
                else
                    ApplicationMessageBox.ShowWarning(this, "Attempt to send message failed but no exception was generated.");
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "Failed to send test message to the IDERA News Server.", e);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void updatePulseControls()
        {
            Validation check;

            check = validator.Validate("PulseServer", true, false);

            pulseConfig.AllowMoveNext = check.IsValid;
            testPulseProvider.Enabled = check.IsValid;
        }

        #endregion

        #region Generic Utility Methods

        // BMT  Test this logic... I'm thinking that we either don't really need this, or maybe its just not working
        //          right... if these fields are blank, it should populate a default value.. but I don't think its 
        //          working for the server port
        private void generic_BeforeExitEditMode(object sender, Infragistics.Win.BeforeExitEditModeEventArgs e)
        {
            if (sender == serverPort)
            {
                if (serverPort.Value == DBNull.Value)
                    serverPort.Value = ((SmtpNotificationProviderInfo)newProviderInfo).SmtpServerPort;
            } else
            if (sender == this.timeoutEditor)
            {
                if (timeoutEditor.Value == DBNull.Value)
                    timeoutEditor.Value = ((SmtpNotificationProviderInfo)newProviderInfo).LoginTimeout;
            } 

        }

        private T GetSafeValue<T>(object value, T defaultValue)
        {
            if (value == null)
                return defaultValue;
            return (T)value;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (sender == this.providerName)
                chooseProviderTypePage.AllowMoveNext = IsProviderTypePageComplete;
            else if (sender == this.pulseAddressTextBox)
                updatePulseControls();
            else if (sender == this.managerAddress)
                updateSnmpControls();
            else
                updateSmtpControls();
        }

        private void validator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            string value = e.Value == null ? null : e.Value.ToString().Trim();

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "Server":
                    serverValidated = true;
                    e.IsValid = (UriHostNameType.Unknown == Uri.CheckHostName(value)) ? false : true;
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
                case "Manager":
                    e.IsValid = ((UriHostNameType.Unknown == Uri.CheckHostName(value)) ||
                                 (UriHostNameType.IPv6 == Uri.CheckHostName(value))) ? false : true;
                    break;
                case "PulseServer":
                    e.IsValid = string.IsNullOrEmpty(pulseAddressTextBox.Text) ? false : true;
                    break; ;
            }
        }

        #endregion

        #region Help

        private void AddActionProviderWizard_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            if (this.wizardFramework.SelectedPage == this.welcomePage) welcomePage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.chooseProviderTypePage) chooseProviderTypePage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.snmpConfig) snmpConfig_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.smtpConfig) smtpConfig_HelpRequested(null, null);
        }

        private void AddActionProviderWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (this.wizardFramework.SelectedPage == this.welcomePage) welcomePage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.chooseProviderTypePage) chooseProviderTypePage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.snmpConfig) snmpConfig_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.smtpConfig) smtpConfig_HelpRequested(sender, hlpevent);
        }

        private void welcomePage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizard);
        }

        private void chooseProviderTypePage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardType);
        }

        private void snmpConfig_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardSNMP);
        }

        private void smtpConfig_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddActionProviderWizardSMTP);
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        #endregion
    }
}
