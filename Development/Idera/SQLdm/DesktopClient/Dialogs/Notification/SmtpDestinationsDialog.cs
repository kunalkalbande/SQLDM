using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Net.Mail;
    using System.Text;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win.Misc;
    using Infragistics.Win.UltraWinEditors;
    using System.ComponentModel;

    public partial class SmtpDestinationsDialog : BaseDialog
    {
        private SmtpDestination destination;
        private SmtpNotificationProviderInfo currentProvider;
        private CaretData current;
        private CaretData bodyCaret;
        private CaretData subjectCaret;
        private bool test;

        public SmtpDestinationsDialog()
        {
            this.DialogHeader = "Smtp Action";
            InitializeComponent();
            bodyCaret = new CaretData();
            bodyCaret.editor = bodyTextBox;
            subjectCaret = new CaretData();
            subjectCaret.editor = subjectTextBox;
            AdaptFontSize();
        }

        public SmtpDestination Destination
        {
            get
            {
                if (destination == null)
                    destination = new SmtpDestination();
                return destination;
            }
            set { destination = value; }
        }

        public bool TestMode
        {
            get { return test; }
            set { test = value; }
        }

        public void SetNotificationProviders(IEnumerable<NotificationProviderInfo> providers)
        {
            providerList.Items.Clear();
            foreach (NotificationProviderInfo npi in providers)
            {
                if (npi is SmtpNotificationProviderInfo)
                {
                    providerList.Items.Add(npi);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            UpdateDestination(destination, true);
        }

        private void UpdateDestination(SmtpDestination destination, bool validate) 
        {
            NotificationProviderInfo npi = providerList.SelectedItem as NotificationProviderInfo;
            if (validate && npi == null)
            {
                ApplicationMessageBox.ShowInfo(this, "You must select an action provider.");
                providerList.Focus();
                DialogResult = DialogResult.None;
                return;
            }
            destination.Provider = npi;
            destination.From = txtFrom.Text.TrimEnd();
            destination.To = txtTo.Text.TrimEnd();
            if (validate && String.IsNullOrEmpty(destination.To))
            {
                ApplicationMessageBox.ShowInfo(this, "To is a required field.");
                txtTo.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            destination.Subject = subjectTextBox.Text.TrimEnd();
            if (validate && String.IsNullOrEmpty(destination.To))
            {
                ApplicationMessageBox.ShowInfo(this, "Subject is a required field.");
                subjectTextBox.Focus();
                DialogResult = DialogResult.None;
                return;
            }
            destination.Body = bodyTextBox.Text.TrimEnd();
        }

        private void SmtpDestinationsDialog_Load(object sender, EventArgs e)
        {
            int ix = 0;

            int x = 0;
            foreach (NotificationProviderInfo npi in providerList.Items)
            {
                if (npi.Id == Destination.ProviderID)
                {
                    ix = x;
                    break;
                }
                x++;
            }

            if (providerList.Items.Count > 0)
            {
                this.providerList.SelectedItem = providerList.Items[ix];
                currentProvider = providerList.Items[ix] as SmtpNotificationProviderInfo;
            }
            txtFrom.Text = Destination.From;
            if (txtFrom.Text.Trim()  == "") 
            {
                SmtpNotificationProviderInfo npi = this.providerList.SelectedItem as SmtpNotificationProviderInfo;
                if (npi != null)
                {
                    MailAddress address = new MailAddress(npi.SenderAddress, npi.SenderName);
                    txtFrom.Text = FormatAddress(npi.SenderName, npi.SenderAddress);
                }
            }

            txtTo.Text = Destination.To;
            subjectTextBox.Text = Destination.Subject;
            bodyTextBox.Text = Destination.Body;

            if (test)
            {
                subjectTextBox.Text = "SQLDM SMTP Action Test";
                bodyTextBox.Text = "It worked!";
                subjectButton.Visible = false;
                bodyButton.Visible = false;
                // Using test button from the SMTP Provider Config Dialog
                testButton.Visible = false;
            }
            else
            {
                if (subjectTextBox.Text.Trim() == "")
                {
                    //subjectTextBox.Text = "[$(Severity)] $(Instance) - $(Metric) ($(Value))";
                    // SQLDM-28044 Restrict the subject length at later stages
                    subjectTextBox.Text = "SQLDM Alert ($(Severity)) - $(AlertSummary) on $(Instance)";
                }
                if (bodyTextBox.Text.Trim() == "")
                {
                    // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
                    bodyTextBox.Text =
                        "$(Timestamp), $(Metric) on $(Instance) and host $(Hostname) is $(Severity).\r\n\r\n$(AlertText)";
                    //"At $(Timestamp), the severity for $(Metric) on $(Instance) was $(Severity) and had a value of $(Value).";
                    bodyTextBox.Text += "\r\n\r\n$(Metric): $(Description)";
                }
            }

            bodyTextBox.Text = FixupLineBreaks(bodyTextBox.Text);

            txtTo.Focus();

            UpdateControls();
        }

        private string FixupLineBreaks(string input)
        {
            StringBuilder result = new StringBuilder();
            char pch = ' ';
            foreach (char ch in input)
            {
                if (ch == '\n' && pch != '\r')
                    result.Append('\r');
                if (ch == '\t')
                {
                    result.Append("   ");
                    pch = ' ';
                    continue;
                }
                result.Append(ch);
                pch = ch;
            }
            return result.ToString();
        }

        private string FormatAddress(string name, string address)
        {
            if (!String.IsNullOrEmpty(name))
                return String.Format("{0}<{1}>", name, address);

            return address;
        }

        private void providerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SmtpNotificationProviderInfo newProvider = providerList.SelectedItem as SmtpNotificationProviderInfo;
            if (currentProvider == null || newProvider.Id != currentProvider.Id)
            {
                string from = txtFrom.Text.Trim();
                if (from == "")
                    txtFrom.Text = FormatAddress(newProvider.SenderName, newProvider.SenderAddress);
                else if (currentProvider != null)
                {
                    if (from == FormatAddress(currentProvider.SenderName, currentProvider.SenderAddress))
                    {
                        txtFrom.Text = FormatAddress(newProvider.SenderName, newProvider.SenderAddress);
                    }
                }
            }
            currentProvider = newProvider;
        }

        #region Substitution Variables

        private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string varName;
            switch (e.Tool.Key)
            {
                case "Metric":
                    varName = "$(Metric)";
                    break;
                case "Severity":
                    varName = "$(Severity)";
                    break;
                case "Value":
                    varName = "$(Value)";
                    break;
                case "Instance Name":
                    varName = "$(Instance)";
                    break;
                case "Database":
                    varName = "$(Database)";
                    break;
                case "Table":
                    varName = "$(Table)";
                    break;
                case "Timestamp":
                    varName = "$(Timestamp)";
                    break;
                case "Comments":
                    varName = "$(Comments)";
                    break;
                case "AlertText":
                    varName = "$(AlertText)";
                    break;
                case "AlertDescription":
                    varName = "$(Description)";
                    break;
                case "AlertSummary":
                    varName = "$(AlertSummary)";
                    break;
                case "Resource":
                    varName = "$(Resource)";
                    break;

                default:
                    return;
            }
            current.editor.Text = ReplaceText(current.editor.Text, current.selectionStart, current.selectionLength, varName);
            current.selectionStart += varName.Length;
            current.selectionLength = 0;
        }

        public string ReplaceText(string source, int start, int len, string newValue)
        {
            StringBuilder builder = new StringBuilder(source);
            if (len == 0)
            {
                builder.Insert(start, newValue);
                return builder.ToString();
            }
            string oldValue = source.Substring(start, len);
            builder.Replace(oldValue, newValue, start, len);
            return builder.ToString();
        }

        private void bodyTextBox_BeforeExitEditMode(object sender, Infragistics.Win.BeforeExitEditModeEventArgs e)
        {
            if (sender == subjectTextBox)
                current = subjectCaret;
            else
            if (sender == bodyTextBox)
                current = bodyCaret;
            else
                return;

            current.selectionStart = ((UltraTextEditor)sender).SelectionStart;
            current.selectionLength = ((UltraTextEditor)sender).SelectionLength;
        }

        private void DropDownListButtonClicked(object sender, EventArgs e)
        {
            string menu;
            if (sender == bodyButton)
            {
                menu = "BodyPopupMenu";
                current = bodyCaret;
            }
            else
            if (sender == subjectButton)
            {
                menu = "SubjectPopupMenu";
                current = subjectCaret;
            }
            else
                return;

            UltraButton button = sender as UltraButton;
            if (button != null)
            {
                this.ultraToolbarsManager1.ShowPopup(menu,this.PointToScreen(new Point(button.Left, button.Bottom)));
            }
        }

        internal class CaretData
        {
            internal Control editor;
            internal int selectionStart;
            internal int selectionLength;
        }

        #endregion


        private void OnValueChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }


        private void UpdateControls()
        {
            bool OK = false;

            if (providerList.SelectedIndex >= 0)
            {
                string text = txtFrom.Text.Trim();
                if (!String.IsNullOrEmpty(text) && SmtpAddressHelpers.IsMailAddressValid(text, true))
                {
                    text = txtTo.Text.Trim();
                    if (!String.IsNullOrEmpty(text) && SmtpAddressHelpers.IsMailAddressValid(text, false))
                    {
                        text = subjectTextBox.Text.Trim();
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = bodyTextBox.Text.Trim();
                            if (!String.IsNullOrEmpty(text))
                            {
                                OK = true;
                            }
                        }
                    }
                }
            }

            testButton.Enabled = OK;
            btnOK.Enabled = OK;
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        private void testButton_Click(object sender, EventArgs args)
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            SmtpDestination dest = (SmtpDestination)destination.Clone();
            UpdateDestination(dest, false);

            if (dest.Provider == null)
            {
                ApplicationMessageBox.ShowError(this, "You must select an SMTP Action Provider.");
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (managementService.TestAction(dest.Provider, dest, null) == 0)
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

        private void bodyTextBox_Leave(object sender, EventArgs e)
        {
            if (sender == bodyTextBox)
                current = bodyCaret;
            else
                return;

            current.selectionStart = ((RichTextBox)sender).SelectionStart;
            current.selectionLength = ((RichTextBox)sender).SelectionLength;
        }

        private void bodyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                //string paste = null;
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string text = FixupLineBreaks(Clipboard.GetText());
                    Clipboard.SetText(text);
                    bodyTextBox.Paste(DataFormats.GetFormat(DataFormats.Text));
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
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