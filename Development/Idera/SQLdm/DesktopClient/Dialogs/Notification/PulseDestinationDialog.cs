using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.UltraWinEditors;
using Idera.SQLdm.Common;
using Infragistics.Win.Misc;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    public partial class PulseDestinationDialog : Form
    {
        private PulseDestination destination;
        private CaretData current;
        private CaretData bodyCaret;
        private CaretData subjectCaret;

        public PulseDestinationDialog()
        {
            InitializeComponent();
            bodyCaret = new CaretData();
            bodyCaret.editor = bodyTextBox;
            subjectCaret = new CaretData();
            subjectCaret.editor = subjectTextBox;
            AdaptFontSize();
        }

        public PulseDestination Destination
        {
            get
            {
                if (destination == null)
                    destination = new PulseDestination();
                return destination;
            }
            set { destination = value; }
        }

        private void subjectTextBox_BeforeExitEditMode(object sender, Infragistics.Win.BeforeExitEditModeEventArgs e)
        {
            if (sender == subjectTextBox)
                current = subjectCaret;
            else
                return;

            current.selectionStart = ((UltraTextEditor)sender).SelectionStart;
            current.selectionLength = ((UltraTextEditor)sender).SelectionLength;
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
                string paste = null;
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
                this.ultraToolbarsManager1.ShowPopup(menu, this.PointToScreen(new Point(button.Left, button.Bottom)));
            }
        }

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

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }


        internal class CaretData
        {
            internal Control editor;
            internal int selectionStart;
            internal int selectionLength;
        }

        private void PulseDestinationDialog_Load(object sender, EventArgs e)
        {
            subjectTextBox.Text = Destination.Subject;
            bodyTextBox.Text = Destination.Body;

            if (subjectTextBox.Text.Trim() == "")
            {
                //subjectTextBox.Text = "[$(Severity)] $(Instance) - $(Metric) ($(Value))";
                subjectTextBox.Text = "$(AlertSummary)";
            }
            if (bodyTextBox.Text.Trim() == "")
            {
                bodyTextBox.Text =
                    //"At $(Timestamp), the severity for $(Metric) on $(Instance) was $(Severity) and had a value of $(Value).";
                    "$(AlertText)";
                bodyTextBox.Text += "\r\n\r\n$(Metric): $(Description)";
            }

            bodyTextBox.Text = FixupLineBreaks(bodyTextBox.Text);


            subjectTextBox.Focus();
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

        private void okButton_Click(object sender, EventArgs e)
        {
            Destination.Subject = subjectTextBox.Text;
            Destination.Body = bodyTextBox.Text;
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