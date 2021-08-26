using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Import;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.Objects;
using Microsoft.Win32;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Dialogs {
    /// <summary>
    /// This class represents a server whose settings can be imported.
    /// Instances of this class are added to the listboxes on the Import Servers dialog.
    /// </summary>
    internal class ImportableServer {
        private static BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(ImportableServer));
        public string ServerName;
        public RegistryKey RegKey;

        public ImportableServer(string serverName, RegistryKey regKey) {
            ServerName = serverName;
            RegKey = regKey;
        }

        public ServerImport ServerInfo {
            get {
                if (_serverInfo == null) {
                    _serverInfo = new ServerImport();
                    //TODO need to fix this to use the MonitoredSQLServerConfiguration object 
                    _serverInfo.MonitoredServer = null; //new MonitoredSqlServer(0, ServerName, true);
                }

                return _serverInfo;
            }
        }
        private ServerImport _serverInfo;

        // ToString determines what the listboxes display to the user.
        public override string ToString() {
            return ServerName;
        }

        // Read the server's settings from the RegKey key.
        public bool ReadSettings() {
            using (Log.InfoCall()) {
                ReadServerInfo();
                ReadThresholdAlerts();
                ReadBlockingAlert();
                ReadResourceAlert();
                ReadJobsAlerts();
                ReadNtLogDestinations();
                ReadSnmpDestinations();
                ReadQueryMonitorSettings();
                ReadStatisticsSettings();
                ReadOdbcSettings();
                // TODO: Create notification rules.
                return true;
            }
        }

        private void ReadOdbcSettings() {
            using (Log.InfoCall()) {
                string loginTimeout = RegKey.GetValue("ODBCLoginTimeout") as string;
                string option = RegKey.GetValue("ODBCOption") as string;
                string queryTimeout = RegKey.GetValue("ODBCQueryTimeout") as string;
                byte[] cipherConnection = RegKey.GetValue("ODBCConnection2") as byte[];
                int errorCode;

                try {
                    // TODO: Put this stuff somewhere to be sent to the Management server.
                    ODBCOption odbcOption = (ODBCOption)(int.Parse(option) - 1);
                    string destination = RegKey.GetValue("ODBCDestination") as string;
                    string filename = RegKey.GetValue("ODBCFileDSNName") as string;
                    string odbcConnectionString =
                        Idera.SQLdm.DesktopClient.Helpers.NativeMethods.DecryptString(cipherConnection,
                                                                       "Idera, Inc. - Data Management & Security Tools",
                                                                       cipherConnection.Length, out errorCode);
                    int odbcLoginTimeout = int.Parse(loginTimeout);
                    int odbcQueryTimeout = int.Parse(queryTimeout);
                } catch {

                }
            }
        }

        private void ReadStatisticsSettings() {
            using (Log.InfoCall()) {
                string[] separator = new string[] { "~,^" };
                string quietTime = RegKey.GetValue("Server Quiet Time") as string;
                string growthDayList = RegKey.GetValue("GrowthCollectionDays") as string;
                string growthDbList = RegKey.GetValue("GrowthDBExclusions") as string;
                string growthExcSystem = RegKey.GetValue("GrowthStatsExcludeSystem") as string;
                string reorgDayList = RegKey.GetValue("ReorgCollectionDays") as string;
                string reorgDbList = RegKey.GetValue("ReorgDBExclusions") as string;
                string reorgExcSystem = RegKey.GetValue("ReorgStatsExcludeSystem") as string;
                string hours, mins;

                try {
                    // TODO: Save this stuff.
                    TimeSpan quietStart, quietEnd;
                    hours = quietTime.Substring(0, 2);
                    mins = quietTime.Substring(2, 2);
                    quietStart = new TimeSpan(int.Parse(hours), int.Parse(mins), 0);

                    hours = quietTime.Substring(5, 2);
                    mins = quietTime.Substring(7, 2);
                    quietEnd = new TimeSpan(int.Parse(hours), int.Parse(mins), 0);

                    bool[] growthDayFlags = SetStatisticsDayFlags(growthDayList);
                    string[] growthDbArray = growthDbList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    bool growthExcludeSystem = growthExcSystem == "True";

                    bool[] reorgDayFlags = SetStatisticsDayFlags(reorgDayList);
                    string[] reorgDbArray = reorgDbList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    bool reorgExcludeSystem = reorgExcSystem == "True";
                } catch {

                }
            }
        }

        private static bool[] SetStatisticsDayFlags(string rawList) {
            string[] separator = new string[] { "~,^" };
            bool[] dayFlags = new bool[7];
            foreach (string dayNum in rawList.Split(separator, StringSplitOptions.RemoveEmptyEntries)) {
                switch (dayNum) {
                    case "0":
                        // 0 is always present even when no days are selected.
                        break;
                    case "1":
                        dayFlags[(int)DayOfWeek.Monday] = true;
                        break;
                    case "2":
                        dayFlags[(int)DayOfWeek.Tuesday] = true;
                        break;
                    case "3":
                        dayFlags[(int)DayOfWeek.Wednesday] = true;
                        break;
                    case "4":
                        dayFlags[(int)DayOfWeek.Thursday] = true;
                        break;
                    case "5":
                        dayFlags[(int)DayOfWeek.Friday] = true;
                        break;
                    case "6":
                        dayFlags[(int)DayOfWeek.Saturday] = true;
                        break;
                    case "7":
                        dayFlags[(int)DayOfWeek.Sunday] = true;
                        break;
                }
            }

            return dayFlags;
        }

        private void ReadQueryMonitorSettings() {
            using (Log.InfoCall()) {
                string[] separator = new string[] { "~,^" };
                string exclusionList = RegKey.GetValue("Trace Exclusions") as string;
                string settingsList = RegKey.GetValue("Trace Settings") as string;

                try {
                    string[] excludeArray = exclusionList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    string[] settingsArray = settingsList.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    // TODO: Save this info somewhere.
                    bool enabled = 0 != (int)RegKey.GetValue("Worst Performing Enabled");
                    int cpuSeconds = int.Parse(settingsArray[0]);
                    int readsPerformed = int.Parse(settingsArray[1]);
                    int writesPerformed = int.Parse(settingsArray[2]);
                    double runLongerThan = double.Parse(settingsArray[3]);
                    bool singleXeq = settingsArray[4] == "True";
                    bool spAndTriggers = settingsArray[5] == "True";
                    bool batchXeq = settingsArray[6] == "True";
                    bool excludeDMO = excludeArray[0] == "True";
                    bool excludeProfiler = excludeArray[1] == "True";
                    bool excludeAgent = excludeArray[2] == "True";
                } catch {

                }
            }
        }

        private void ReadSnmpDestinations() {
            using (Log.InfoCall()) {
                try {
                    // TODO: Put all of this somewhere.
                    int retry = int.Parse(RegKey.GetValue("SMTPTimeout") as string);
                    int timeout = int.Parse(RegKey.GetValue("SMTPRetry") as string);
                    string senderEmail = RegKey.GetValue("SMTPSenderEmail") as string;
                    string senderName = RegKey.GetValue("SMTPSenderName") as string;
                    string serverName = RegKey.GetValue("SMTPServerName") as string;
                    string emailList = RegKey.GetValue("Email Recipient List") as string;

                    if (emailList != null) {
                        string[] separator = new string[] { "~,^" };
                        string[] emailArray = emailList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        string[] emailNames = new string[emailArray.Length / 2];
                        bool[] emailEnabled = new bool[emailArray.Length / 2];

                        for (int i = 0; i < emailNames.Length; ++i) {
                            emailNames[i] = emailArray[i * 2];
                            // Oddly, "T" means disabled, "F" means enabled.
                            emailEnabled[i] = (emailArray[1 + i * 2].StartsWith("F"));
                        }
                    }
                } catch {
                    // TODO: log something
                }
            }
        }

        private void ReadNtLogDestinations() {
            using (Log.InfoCall()) {
                string combinedNames = RegKey.GetValue("NTLog Path List") as string;
                if (combinedNames != null) {
                    string[] separator = new string[] { "~,^" };
                    string[] nameArray = combinedNames.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // TODO: Put this somewhere.
                }
            }
        }

        private string Decrypt(string cipher) {
            // Possibly another implementation at //sqldm/Legacy/Main/WebSync/Source/Idera/SQLdm/Common/EncryptionUtility.cs
            if (cipher == null) return null;
            if (cipher == string.Empty) return string.Empty;

            short c1 = (short)cipher[0];
            short c2 = (short)cipher[cipher.Length - 1];
            int diff = c1 - c2;
            string clear = string.Empty;

            for (int i = 1; i < cipher.Length - 1; i += 3) {
                int converted = int.Parse(cipher.Substring(i, 3)) - diff;
                clear += (char)converted;
            }

            return clear;
        }

        private void ReadServerInfo() {
            using (Log.InfoCall()) {
                ServerInfo.SqlUser = RegKey.GetValue("Logon") as string;
                ServerInfo.SqlComments = RegKey.GetValue("Comments") as string;
                ServerInfo.Department = RegKey.GetValue("Department") as string;
                ServerInfo.Location = RegKey.GetValue("Location") as string;
                object maintMode = RegKey.GetValue("Enable Maintenance Mode");

                if (ServerInfo.SqlUser == null) {
                    // Use NT authentication.
                    ServerInfo.SqlPassword = null;
                } else {
                    string pw = RegKey.GetValue("Password") as string;
                    ServerInfo.SqlPassword = Decrypt(pw);
                }

                ServerInfo.MaintenanceMode = false;
                if (maintMode is int) {
                    ServerInfo.MaintenanceMode = (int)maintMode != 0;
                }
            }
        }

        private void ReadJobsAlerts() {
            using (Log.InfoCall()) {
                string badJobsAlertsVal = RegKey.GetValue("BombedJobsAlertMethods") as string;
                string longJobsAlertsVal = RegKey.GetValue("LongJobsAlertMethods") as string;
                string longJobsPercentVal = RegKey.GetValue("LongJobsPercent") as string;
                string categoriesVal = RegKey.GetValue("Job Categories") as string;

                if (badJobsAlertsVal == null || longJobsPercentVal == null || longJobsAlertsVal == null || categoriesVal == null) {
                    // This just means they haven't been set.
                    // TODO: log something
                    return;
                }

                try {
                    Threshold warn, critical, info;
                    string[] separator = new string[] { "~,^" };
                    string[] categoriesArray = categoriesVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // TODO: Save this in the alert somewhere for sending to the management service.

                    // First process the "bad jobs" settings;

                    bool emailNotify = badJobsAlertsVal.Contains("E");
                    bool NtEventLogNotify = badJobsAlertsVal.Contains("N");
                    bool OdbcNotify = badJobsAlertsVal.Contains("O");
                    // TODO: Put this info somewhere to be sent to the server.

                    warn = new Threshold(false, true);
                    critical = new Threshold(emailNotify || NtEventLogNotify || OdbcNotify, true);
                    info = new Threshold(false, true);
                    ServerInfo.Thresholds.Add(new MetricThresholdEntry(ServerInfo.MonitoredServer, Metric.BombedJobs, warn, critical, info));

                    // Now process the "long jobs" settings.

                    int percent = Convert.ToInt32(longJobsPercentVal);
                    emailNotify = longJobsAlertsVal.Contains("E");
                    NtEventLogNotify = longJobsAlertsVal.Contains("N");
                    OdbcNotify = longJobsAlertsVal.Contains("O");
                    // TODO: Put this info somewhere to be sent to the server.

                    // Spec says to set the warning threshold to 75% of the specified value and disable it.
                    warn = new Threshold(false, (percent * 3) / 4);
                    critical = new Threshold(emailNotify || NtEventLogNotify || OdbcNotify, percent);
                    info = new Threshold(false, 0);
                    ServerInfo.Thresholds.Add(new MetricThresholdEntry(ServerInfo.MonitoredServer, Metric.LongJobs, warn, critical, info));
                } catch {
                    // TODO: log something
                }
            }
        }

        private void ReadResourceAlert() {
            using (Log.InfoCall()) {
                string optionVal = RegKey.GetValue("ResourceOption") as string;
                string valVal = RegKey.GetValue("ResourceValue") as string;
                string hostsVal = RegKey.GetValue("Resource Exclude Hosts") as string;
                string progsVal = RegKey.GetValue("Resource Exclude Programs") as string;
                string timesVal = RegKey.GetValue("ResourceCheckTimes") as string;
                string alertMethodVal = RegKey.GetValue("ResourceAlertMethods") as string;

                if (optionVal == null || valVal == null || hostsVal == null || progsVal == null || timesVal == null || alertMethodVal == null) {
                    // This just means they haven't been set.
                    // TODO: log something
                    return;
                }

                // Convert the seconds threshold value to an integer.
                try {
                    string[] separator = new string[] { "~,^" };
                    bool enabled = (optionVal != "0");
                    int seconds = Int32.Parse(valVal) / 1000; // Ms in registry
                    string hours, minutes;
                    TimeSpan startTime, endTime; // For the schedule.
                    bool[] dayFlags = new bool[7]; // The DayOfWeek enum ranges from 0 (Sunday) to 6 (Saturday).

                    // Get the schedule start time.
                    hours = timesVal.Substring(0, 2);
                    minutes = timesVal.Substring(2, 2);
                    startTime = new TimeSpan(Int32.Parse(hours), Int32.Parse(minutes), 0);

                    // Get the schedule end time.
                    hours = timesVal.Substring(4, 2);
                    minutes = timesVal.Substring(6, 2);
                    endTime = new TimeSpan(Int32.Parse(hours), Int32.Parse(minutes), 0);

                    // Get the scheduled days of the week as an array of bools.
                    for (int i = 8; i < 15; ++i) {
                        dayFlags[i - 8] = timesVal[i] == '1';
                    }

                    string[] hostsArray = hostsVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // TODO: Where to put this information?  It should apply to the alert.

                    string[] progsArray = progsVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // TODO: Where to put this information?  It should apply to the alert.

                    bool emailNotify = alertMethodVal.Contains("E");
                    bool NtEventLogNotify = alertMethodVal.Contains("N");
                    bool OdbcNotify = alertMethodVal.Contains("O");
                    // TODO: Where to put this information? .

                    // Spec says to set the warning threshold to 75% of the specified value and disable it.
                    Threshold warn = new Threshold(false, (seconds * 3) / 4);
                    Threshold critical = new Threshold(enabled, seconds);
                    Threshold info = new Threshold(false, 0);
                    ServerInfo.Thresholds.Add(new MetricThresholdEntry(ServerInfo.MonitoredServer, Metric.ResourceAlert, warn, critical, info));
                } catch {
                    // TODO: log something.
                }
            }
        }

        private void ReadBlockingAlert() {
            using (Log.InfoCall()) {
                string optionVal = RegKey.GetValue("BlockingOption") as string;
                string valVal = RegKey.GetValue("BlockingValue") as string;
                string alertMethodVal = RegKey.GetValue("BlockingAlertMethods") as string;

                if (optionVal == null || valVal == null || alertMethodVal == null) {
                    // This just means they haven't been set.
                    // TODO: log something
                    return;
                }

                try {
                    bool enabled = (optionVal != "0");
                    int seconds = Int32.Parse(valVal);
                    bool emailNotify = alertMethodVal.Contains("E");
                    bool NtEventLogNotify = alertMethodVal.Contains("N");
                    bool OdbcNotify = alertMethodVal.Contains("O");

                    // TODO: Where to save the notification settings?

                    // Spec says to set the warning threshold to 75% of the specified value and disable it.
                    Threshold warn = new Threshold(false, (seconds * 3) / 4);
                    Threshold critical = new Threshold(enabled, seconds);
                    Threshold info = new Threshold(false, 0);
                    ServerInfo.Thresholds.Add(new MetricThresholdEntry(ServerInfo.MonitoredServer, Metric.BlockingAlert, warn, critical, info));
                } catch {
                    // TODO: log something.
                }
            }
        }
        // Read the "AlertThresholds" value and parse out the thresholds for the alerts.
        private void ReadThresholdAlerts() {
            using (Log.InfoCall()) {
                string regVal = RegKey.GetValue("AlertThresholds") as string;
                if (regVal != null) {
                    char[] separator = new char[] { ' ' };
                    string[] alertArray = regVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    // The Nth entry in alertArray corresponds to the Nth Metric enum value.
                    for (int i = 0; i < alertArray.Length; ++i) {
                        try {
                            Metric metric = (Metric)i;
                            string alertStr = alertArray[i];
                            bool enabled = alertStr.Contains("T");
                            bool emailNotify = alertStr.Contains("E");
                            bool NtEventLogNotify = alertStr.Contains("N");
                            bool OdbcNotifiy = alertStr.Contains("O");
                            Threshold warning;
                            Threshold error;
                            Threshold info;
                            Debug.Print(alertStr + " " + metric);

                            // TODO: Create notification rules for the current metric.

                            // The state metrics are handled individually.
                            switch (metric) {
                                //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --specifying the threshold alerts for new services
                                case Metric.SQLBrowserServiceStatus:
                                case Metric.SQLActiveDirectoryHelperServiceStatus:
                                //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --specifying the threshold alerts for new services
                                case Metric.AgentServiceStatus:
                                case Metric.SqlServiceStatus:
                                case Metric.DtcServiceStatus:
                                case Metric.FullTextServiceStatus:
                                    // All the service status alerts use the same warning and critical states.
                                    warning = new Threshold(enabled, new Threshold.ComparableList(ServiceState.Paused, ServiceState.UnableToMonitor));
                                    error = new Threshold(enabled, new Threshold.ComparableList(ServiceState.NotInstalled, ServiceState.Stopped));
                                    info = new Threshold(false, new Threshold.ComparableList());
                                    break;
                                case Metric.DMStart:
                                    // Make it a warning if the DM service is started.  Disable the error.
                                    warning = new Threshold(enabled, true);
                                    error = new Threshold(false, true);
                                    info = new Threshold(false, true);
                                    break;
                                case Metric.DMStop:
                                    // Make it an error if the DM service is stopped. Disable the warning.
                                    // If the DM service is stopped, how can any alerts occur?
                                    warning = new Threshold(false, true);
                                    error = new Threshold(enabled, true);
                                    info = new Threshold(false, true);
                                    break;
                                case Metric.ReadWriteErrors:
                                    warning = new Threshold(false, true);
                                    error = new Threshold(enabled, true);
                                    info = new Threshold(false, true);
                                    break;
                                case Metric.DatabaseStatus:
                                    warning = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
                                    error = new Threshold(enabled, new Threshold.ComparableList(
                                                                               DatabaseStatus.EmergencyMode,
                                                                               DatabaseStatus.Suspect,
                                                                               DatabaseStatus.PreRecovery,
                                                                               DatabaseStatus.Recovering)
                                        );
                                    info = new Threshold(false, Threshold.Operator.EQ, new Threshold.ComparableList());
                                    break;
                                case Metric.OSMetricsStatus:
                                    warning = new Threshold(enabled, new Threshold.ComparableList(OSMetricsStatus.Disabled));
                                    error =
                                        new Threshold(enabled, new Threshold.ComparableList(
                                                                            OSMetricsStatus.OLEAutomationUnavailable,
                                                                            OSMetricsStatus.WMIServiceUnreachable));
                                    info = new Threshold(false, new Threshold.ComparableList());
                                    break;
                                case Metric.SQLMemoryUsagePct:
                                    warning = new Threshold(false, true);
                                    error = new Threshold(enabled, true);
                                    info = new Threshold(false, true);
                                    break;
                                default:
                                    // This method parses the threshold values out of the
                                    // specified string and sets the outputs to null if it fails.
                                    ParseThresholds(alertStr, enabled, out warning, out error, out info);
                                    break;
                            }

                            if (warning != null) {
                                ServerInfo.Thresholds.Add(new MetricThresholdEntry(ServerInfo.MonitoredServer, metric, warning, error, info));
                            }
                        } catch {
                            // TODO: log something.
                        }
                    }
                }
            }
        }

        private bool ParseThresholds(string rawData, bool enabled, out Threshold warning, out Threshold error, out Threshold info) {
            using (Log.InfoCall()) {
                bool rc;
                try {
                    int w = Convert.ToInt32(rawData.Substring(0, 4));
                    int e = Convert.ToInt32(rawData.Substring(4, 4));
                    int i = Convert.ToInt32(rawData.Substring(8, 4));
                    warning = new Threshold(enabled, w);
                    error = new Threshold(enabled, e);
                    info = new Threshold(false, i);
                    rc = true;
                } catch {
                    warning = null;
                    error = null;
                    info = null;
                    rc = false;
                }

                return rc;
            }
        }
    }
}