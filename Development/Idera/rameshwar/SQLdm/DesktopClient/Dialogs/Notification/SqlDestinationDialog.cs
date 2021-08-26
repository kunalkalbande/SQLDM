using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Helpers;
    using Common.Notification.Providers;
    using Common.Services;
    using Common.UI.Dialogs;
    using Infragistics.Win.Misc;
    using Objects;

    public partial class SqlDestinationDialog : Form
    {
        private SqlDestination destination;

        private CaretData current;
        private CaretData sqlCaretData;

        public SqlDestinationDialog()
        {
            InitializeComponent();
            sqlCaretData = new CaretData();
            sqlCaretData.editor = tSqlTextBox;

            // Auto scale font size.
            AdaptFontSize();
        }

        internal SqlDestination Destination
        {
            get { return destination;  }
            set { destination = value; }
        }

        private void SqlDestinationDialog_Load(object sender, EventArgs e)
        {
            descriptionTextBox.Text = destination.Description;
            tSqlTextBox.Text = FixupLineBreaks(destination.Sql);

            serverTextBox.Text = destination.Server.Trim();
            if (serverTextBox.Text.Length == 0)
            {
                serverTextBox.Text = "$(Instance)";
            }
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

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void serverTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            using (TestSqlActionDialog testDialog = new TestSqlActionDialog())
            {
                SqlDestination dest = new SqlDestination();
                UpdateDestination(dest);
                testDialog.Destination = dest;
                testDialog.ShowDialog(this);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateDestination())
            {
                DialogResult = DialogResult.None;
                return;
            }

            UpdateDestination(destination);
        }

        private void serverBrowseButton_Click(object sender, EventArgs e)
        {
            using (ServerSelectionDialog ssd = new ServerSelectionDialog(true))
            {
                ssd.Servers = GetMonitoredServers();

                List<string> selected = new List<string>();
                string serverName = serverTextBox.Text.Trim();
                if (serverName.Length > 0)
                    selected.Add(serverName);
                ssd.SelectedServers = selected;

                if (ssd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ssd.SelectedServers.Count > 0)
                        serverTextBox.Text = ssd.SelectedServers[0];
                }
            }
        }

        private IList<string> GetMonitoredServers()
        {
            List<string> list = new List<string>();

            foreach (MonitoredSqlServerWrapper server in ApplicationModel.Default.ActiveInstances)
            {
                list.Add(server.InstanceName);
            }

            return list;
        }

        private void UpdateControls()
        {
            
        }

        private bool ValidateDestination()
        {
            IManagementService ms = ManagementServiceHelper.GetDefaultService();
            try
            {
                string s = descriptionTextBox.Text.Trim();
                if (s.Length < 1)
                    throw new ApplicationException("Please enter a description for your sql action.");
                s = serverTextBox.Text.Trim();
                if (s.Length < 1)
                    throw new ApplicationException("Please enter a server for your sql action.");
                s = tSqlTextBox.Text.Trim();
                if (s.Length < 1)
                    throw new ApplicationException("Please enter a sql script for your sql action.");

                SqlDestination pd = new SqlDestination();
                UpdateDestination(pd);

                ms.ValidateDestination(pd);
            }
            catch (ApplicationException ae)
            {
                ApplicationMessageBox.ShowError(this, ae);
                return false;
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                    "The sql action does not appear to be valid.  Please correct the settings and try again.",
                    e);
                return false;
            }
            return true;
        }

        private void UpdateDestination(SqlDestination dest)
        {
            dest.Description = descriptionTextBox.Text.Trim();
            dest.Server = serverTextBox.Text.TrimStart();
            dest.Sql = tSqlTextBox.Text.Trim();
           
        }

        private void bodyButton_Click(object sender, EventArgs e)
        {
            string menu;
            if (sender == bodyButton)
            {
                menu = "BodyPopupMenu";
                current = sqlCaretData;
            }
            else
                return;

            UltraButton button = sender as UltraButton;
            if (button != null)
            {
                ultraToolbarsManager1.ShowPopup(menu, this.PointToScreen(new Point(button.Left, button.Bottom)));
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

        internal class CaretData
        {
            internal RichTextBox editor;
            internal int selectionStart;
            internal int selectionLength;
        }

        private void tSqlTextBox_Leave(object sender, EventArgs e)
        {
            if (sender == tSqlTextBox)
                current = sqlCaretData;
            else
                return;

            current.selectionStart = ((RichTextBox)sender).SelectionStart;
            current.selectionLength = ((RichTextBox)sender).SelectionLength;
        }

        private void tSqlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                //string paste = null;
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string text = FixupLineBreaks(Clipboard.GetText());
                    Clipboard.SetText(text);
                    tSqlTextBox.Paste(DataFormats.GetFormat(DataFormats.Text));
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
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