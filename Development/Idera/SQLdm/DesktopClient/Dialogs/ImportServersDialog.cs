using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Import;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Helpers;
using Microsoft.Win32;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Dialogs {
    internal partial class ImportServersDialog : BaseDialog
    {
        // List of available servers displayed in the left-hand listbox.
        private readonly ListBox.ObjectCollection AvailableServers;

        // List of selected servers displayed in the right-hand listbox.
        private readonly ListBox.ObjectCollection SelectedServers;

        private ImportRequest _importRequest = new ImportRequest();

        private static BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(ImportServersDialog));

        /// <summary>
        /// Creates an instance of the dialog, displays it, and returns the DialogResult.
        /// Returns DialogResult.Abort if the dialog can't be shown (e.g. error loading servers).
        /// </summary>
        /// <returns></returns>
        internal static DialogResult Display() {
            using (Log.InfoCall()) {
                ImportServersDialog dlg = new ImportServersDialog();

                // LoadServerList will display an appropriate message if servers can't be loaded.
                if (dlg.LoadServerList()) {
                    return dlg.ShowDialog();
                } else {
                    return DialogResult.Abort;
                }
            }
        }

        /// <summary>
        /// Ctor is only called by the static Display method.
        /// </summary>
        private ImportServersDialog() {
            this.DialogHeader = "Import Servers";
            InitializeComponent();
            AvailableServers = dualListSelectorControl1.Available.Items;
            SelectedServers = dualListSelectorControl1.Selected.Items;
            AdaptFontSize();
        }

        // Populate the left-hand (available servers) listbox with the list of servers found in
        // the registry, omitting version 7 servers and those already being
        // monitored.  If there are no servers to display, display a message
        // and return false.
        private bool LoadServerList() {
            using (Log.InfoCall()) {
                string keyName = @"Software\Idera\DiagnosticManager\Servers";
                string errorKey = keyName; // Used if an exception is thrown reading a key.
                bool rc = true;

                try {
                    // Open the key with the list of servers.
                    errorKey = keyName;
                    RegistryKey serversKey = Registry.LocalMachine.OpenSubKey(keyName, false);

                    if (serversKey == null) {
                        // Key doesn't exist.  Report that 4.x is not installed.
                        ApplicationMessageBox.ShowInfo(this, "SQLDM 4.x is not installed on this computer.");
                        rc = false;
                    } else {
                        Log.Debug("Old servers from registry:");
                        Log.Debug(serversKey.GetSubKeyNames());

                        // Query the management service for the list of monitored servers.
                        // TODO: Remove the hardcoded server name.
                        // TODO: Handle exceptions/errors returned by remote call.
                        //ManagementServiceHelper.SetServerName("KGOOLSBEED", 5166);
                        //IList<MonitoredServerInfo> monitoredServers = ManagementServer.IManagementService.GetMonitoredServers();
                        IList<MonitoredServerInfo> monitoredServers = new List<MonitoredServerInfo>();

                        // Enumerate the subkeys.  Each subkey represents a SQL server.
                        foreach (string serverKeyName in serversKey.GetSubKeyNames()) {
                            // Convert "localhost" to local computer name.
                            string serverName = serverKeyName;
                            if (serverName.ToUpper() == "LOCALHOST") {
                                serverName = Environment.MachineName;
                                Log.Info("Converted 'localhost' to " + Environment.MachineName);
                            }

                            // Do not display servers that are already monitored.
                            if (!ServerIsInList(monitoredServers, serverName)) {
                                errorKey = Path.Combine(keyName, serverKeyName); // In case an exception occurs opening the key.
                                RegistryKey serverKey = serversKey.OpenSubKey(serverKeyName);
                                string version = serverKey.GetValue("SQL Version") as string;

                                if (version != null && !version.StartsWith("7")) {
                                   Log.Info("Adding " + serverName + " to list of available servers.");
                                   AvailableServers.Add(
                                        new ImportableServer(serverName, serverKey)
                                        );
                                }
                            }
                        }

                        if (AvailableServers.Count == 0) {
                            ApplicationMessageBox.ShowInfo(this, "No elegible servers were found to import.");
                            rc = false;
                        }
                    }
                } catch (SecurityException) {
                    ApplicationMessageBox.ShowError(this, string.Format("You do not have permission to read the registry key {0}.", errorKey), null);
                    rc = false;
                } catch (Exception ex) {
                    // This is unexpected, so just display the raw exception.
                    ApplicationMessageBox.ShowError(this, "An error occured while enumerating servers to be imported.", ex);
                    rc = false;
                }

                return rc;
            }
        }

        private bool ServerIsInList(IList<MonitoredServerInfo> list, string serverName) {
            string ucName = serverName.ToUpper();
            foreach (MonitoredServerInfo info in list) {
                if (info.Name.ToUpper() == ucName) {
                    Log.Info(serverName + " is already monitored.");
                    return true;
                }
            }

            return false;
        }

        // Read the configuration settings from the registry for each selected server.
        private void buttonOK_Click(object sender, EventArgs e) {
            using (Log.InfoCall()) {
                ReadGlobalSettings();
                
                foreach (ImportableServer item in SelectedServers) {
                    item.ReadSettings();
                    _importRequest.Servers.Add(item.ServerInfo);
                }
            }
        }

        private void ReadGlobalSettings() {
            using (Log.InfoCall()) {
                string dmKeyName = @"Software\Idera\DiagnosticManager";
                string serviceKeyName = @"Software\Idera\DiagnosticManager\Service";
                string errorKey = dmKeyName; // Used if an exception is thrown reading a key.

                // If an exception is thrown, the global settings properties in _importRequest
                // will be left with their default values as set in the ImportRequest ctor.
                try {
                    // Open the key with the global settings.
                    RegistryKey regKey = Registry.LocalMachine.OpenSubKey(dmKeyName, false);

                    string maxLoginStr = regKey.GetValue("Max Logon Time") as string;
                    string maxTimeoutStr = regKey.GetValue("Max Timeout Time") as string;

                    _importRequest.MaxLogonTime = Int32.Parse(maxLoginStr);
                    _importRequest.CommandTimeout = Int32.Parse(maxTimeoutStr);

                    // The server refresh interval comes from another key.
                    errorKey = serviceKeyName;
                    regKey.Close();
                    regKey = Registry.LocalMachine.OpenSubKey(serviceKeyName, false);
                    _importRequest.RefreshInterval = (int)regKey.GetValue("Server List Refresh");
                    regKey.Close();
                } 
                catch (SecurityException) 
                {
                    ApplicationMessageBox.ShowError(this, string.Format("You do not have permission to read the registry key {0}.", errorKey), null);
                } 
                catch
                {
                    // This may mean the registry has been tampered with.  Log it, but otherwise ignore.
                }
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