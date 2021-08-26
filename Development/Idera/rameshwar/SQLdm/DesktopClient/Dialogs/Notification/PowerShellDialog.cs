//File added to support Alert Power Shell Provider
//SQLdm 10.1 Srishti Purohit
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.ServiceProcess;
using System.IO;
using Idera.SQLdm.Common;
using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Helpers;
    using Common.Notification.Providers;
    using Common.Services;
    using Common.UI.Dialogs;
    using Infragistics.Win.Misc;
    using Objects;
    using System.Diagnostics;
    using BBS.TracerX;
    using Microsoft.Win32;
    public partial class PowerShellDialog : Form
    {
        private static readonly Logger LOG = Logger.GetLogger("PowerShellDialog");
        private PowerShellDestination destination;

        private CaretData current;
        private CaretData sqlCaretData;
        string exCmdletResult = "";
        PowerShell ps = PowerShell.Create();

        public PowerShellDialog()
        {
            InitializeComponent();
            sqlCaretData = new CaretData();
            sqlCaretData.editor = tPowerShellTextBox;

            // Auto scale font size.
            AdaptFontSize();
        }

        internal PowerShellDestination Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        private void PowerShellDialog_Load(object sender, EventArgs e)
        {
            descriptionTextBox.Text = destination.Description;
            tPowerShellTextBox.Text = FixupLineBreaks(destination.Command);

            //serverTextBox.Text = destination.Server.Trim();
            //if (serverTextBox.Text.Length == 0)
            //{
            //    serverTextBox.Text = "$(Instance)";
            //}
        }
        private Collection<PSObject> CmdletResults(string PSScriptText)

        {

            exCmdletResult = "";

            Collection<PSObject> PSOutput = null;

            ps.AddScript(PSScriptText);

            try
            {
                PSOutput = ps.Invoke();
            }
            catch (Exception ex) { exCmdletResult = "Error: " + ex.Message; }

            finally { ps.Dispose(); }

            return PSOutput;

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
            RunScript();
        }

        /// <summary>
        /// Will open power shell in power shell ise.
        /// </summary>
        private void RunScript()
        {
            try
            {
                LOG.Info("Testing power shell alert response command.");
                string cmd_Script = FixupLineBreaks(tPowerShellTextBox.Text);
                string fileName =
                            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "tempPowerShell.ps1");
                using (StreamWriter stream = new StreamWriter(fileName, false))
                {
                    stream.Write(cmd_Script);

                }
                Process[] listOfPowerIse = Process.GetProcessesByName("powershell_ise");
                Process currentPowerIse = listOfPowerIse.Length > 0 ? Process.GetProcessesByName("powershell_ise")[0] : null;
                if (currentPowerIse != null)
                {
                    DialogResult dialogResult = MessageBox.Show("Your Power_ISE will be restarted. Please save all your work open in Power ISE application, then press OK.", "Warning", MessageBoxButtons.OKCancel);
                    if (dialogResult == DialogResult.OK)
                    {
                        currentPowerIse.Kill();
                        currentPowerIse.WaitForExit();
                    }
                }
                string exePathForPowerShell_ISE = GetExecutablePath();
                if (!string.IsNullOrEmpty(exePathForPowerShell_ISE) && File.Exists(exePathForPowerShell_ISE))
                {
                    LOG.Info("Openning "+ exePathForPowerShell_ISE + " to test command.");
                    //SQLdm 10.1 (Pulkit Puri)--to handle the filename with spaces in directory name
                    //SQLDM-25400 --Test button is not working fine
                    fileName = "\"\\\"" + fileName + "\\\"\"";
                    //SQLdm 10.1 (end)
                    Process obj = Process.Start(exePathForPowerShell_ISE, fileName);
                }
                else
                {
                    LOG.Error("Error : "+ exePathForPowerShell_ISE + " not found to test power shell alert response command.");
                    MessageBox.Show(exePathForPowerShell_ISE+" not found.");
                }
            }
            catch(Exception ex)
            {
                LOG.Error("Error while testing power shell command as alert response : " + ex);
            }
        }

        /// <summary>
        /// Search path where power shell ISE installed in the operating system.
        /// </summary>
        private string GetExecutablePath()
        {
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                RegistryKey powerShellRootKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\PowerShell");
                if (powerShellRootKey != null)
                    foreach (var subKeyName in powerShellRootKey.GetSubKeyNames())
                        if (powerShellRootKey.OpenSubKey(subKeyName) != null)
                            if (powerShellRootKey.OpenSubKey(subKeyName).OpenSubKey("PowerShellEngine").GetValue("ApplicationBase") != null)
                            {
                                string commandLineUri = (string)powerShellRootKey.OpenSubKey(subKeyName).OpenSubKey("PowerShellEngine").GetValue("ApplicationBase");
                                if (string.IsNullOrEmpty(commandLineUri))
                                    continue;
                                commandLineUri = commandLineUri.Trim("\"".ToCharArray()) + "\\powershell_ise.exe";
                                return commandLineUri;
                            }
            }
            return null;
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

        //private void serverBrowseButton_Click(object sender, EventArgs e)
        //{
        //    using (ServerSelectionDialog ssd = new ServerSelectionDialog(true))
        //    {
        //        ssd.Servers = GetMonitoredServers();

        //        List<string> selected = new List<string>();
        //        string serverName = serverTextBox.Text.Trim();
        //        if (serverName.Length > 0)
        //            selected.Add(serverName);
        //        ssd.SelectedServers = selected;

        //        if (ssd.ShowDialog(this) == DialogResult.OK)
        //        {
        //            if (ssd.SelectedServers.Count > 0)
        //                serverTextBox.Text = ssd.SelectedServers[0];
        //        }
        //    }
        //}

        //private IList<string> GetMonitoredServers()
        //{
        //    List<string> list = new List<string>();

        //    foreach (MonitoredSqlServerWrapper server in ApplicationModel.Default.ActiveInstances)
        //    {
        //        list.Add(server.InstanceName);
        //    }

        //    return list;
        //}

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
                    throw new ApplicationException("Please enter a description for your power shell action.");
                //s = serverTextBox.Text.Trim();
                //if (s.Length < 1)
                //    throw new ApplicationException("Please enter a server for your power shell action.");
                s = tPowerShellTextBox.Text.Trim();
                if (s.Length < 1)
                    throw new ApplicationException("Please enter a power shell command for your power shell action.");

                PowerShellDestination pd = new PowerShellDestination();
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

        private void UpdateDestination(PowerShellDestination dest)
        {
            dest.Description = descriptionTextBox.Text.Trim();
            //dest.Server = serverTextBox.Text.TrimStart();
            dest.Command = tPowerShellTextBox.Text.Trim();

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

        private void tPowerShellTextBox_Leave(object sender, EventArgs e)
        {
            if (sender == tPowerShellTextBox)
                current = sqlCaretData;
            else
                return;

            current.selectionStart = ((RichTextBox)sender).SelectionStart;
            current.selectionLength = ((RichTextBox)sender).SelectionLength;
        }

        private void tPowerShellTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                //string paste = null;
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string text = FixupLineBreaks(Clipboard.GetText());
                    Clipboard.SetText(text);
                    tPowerShellTextBox.Paste(DataFormats.GetFormat(DataFormats.Text));
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

        
        /// <summary>
        /// Opens Wiki link on clicking of help button
        /// </summary>
        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            //SQLdm 10.1 (pulkit Puri) --sqldm 26072 fix
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }
    }
}