using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class ServerPropertiesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("ServerPropertiesMetrics");
        private float? _windowsVersionNum = null;

        public ServerVersion ServerVersion { get; private set; }
        public bool IsWindows2008_Or_Newer { get { return (WindowsVersionNum >= 6.0); } }
        public float WindowsVersionNum
        {
            get
            {
                if (null != _windowsVersionNum) { if (_windowsVersionNum.HasValue) return (_windowsVersionNum.Value); }
                _windowsVersionNum = 0;
                if (null == WindowsVersion) return (_windowsVersionNum.Value);
                try
                {
                    string[] p = WindowsVersion.Trim().Split(' ');
                    if (p.Length > 0)
                    {
                        _windowsVersionNum = Convert.ToSingle(p[0].Trim());
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WindowsVersionNum Exception: ", ex);
                }
                return (_windowsVersionNum.Value);
            }
        }
        public string WindowsVersion { get; set; }
        public UInt64 MinutesRunning { get; set; }
        public UInt32 EncryptedConnections { get; set; }
        public UInt32 ServerProcessID { get; set; }
        public string TempDbRecoveryModel { get; set; }
        public string ServerEdition { get; set; }
        public UInt64 PhysicalMemory { get; set; }
        public string ProductVersion { get; set; }
        public string SQLServerServiceAccount { get; set; }
        public string SeManageVolumePrivilege { get; set; }

        public UInt64 MaxWorkerThreads { get; set; }
        public UInt64 MaxUserConnections { get; set; }
        public UInt32 DefaultFillFactor { get; set; }
        public bool C2AuditMode { get; set; }
        public UInt64 IndexCreationMemory { get; set; }
        public bool NoCountOn { get; set; }
        public UInt64 AffinityMask { get; set; }
        public UInt32 ParallelismThreshold { get; set; }
        public UInt32 MaxDOP { get; set; }
        public UInt64 MinServerMemory { get; set; }
        public UInt64 MaxServerMemory { get; set; }
        public bool LightweightPooling { get; set; }
        public bool AWEEnabled { get; set; }
        public bool OptimizeForAdHocWorkloads { get; set; }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("ServerPropertiesMetrics not added : " + snapshot.Error); return; }
            if (snapshot.ConfigurationSnapshotValueStartup == null) { return; }
            if (snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings != null && snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows.Count; index++)
                {
                    try
                    {
                        int id = (Int32)snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["ID"];
                        switch (id)
                        {
                            case 103:
                                {
                                    MaxUserConnections = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 503:
                                {
                                    MaxWorkerThreads = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 109:
                                {
                                    DefaultFillFactor = Convert.ToUInt32(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 544:
                                {
                                    C2AuditMode = ((Int32)snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"] == 1);
                                    break;
                                }
                            case 1505:
                                {
                                    IndexCreationMemory = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 1534:
                                {
                                    ulong userOption = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    NoCountOn = ((userOption & 512) == 512);
                                    break;
                                }
                            case 1535:
                                {
                                    AffinityMask = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 1538:
                                {
                                    ParallelismThreshold = Convert.ToUInt32(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 1539:
                                {
                                    MaxDOP = Convert.ToUInt32(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 1543:
                                {
                                    MinServerMemory = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 1544:
                                {
                                    MaxServerMemory = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"]);
                                    break;
                                }
                            case 1546:
                                {
                                    LightweightPooling = ((Int32)snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"] == 1);
                                    break;
                                }
                            case 1548:
                                {
                                    AWEEnabled = ((Int32)snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"] == 1);
                                    break;
                                }
                            case 1581:
                                {
                                    OptimizeForAdHocWorkloads = ((Int32)snapshot.ConfigurationSnapshotValueStartup.ConfigurationSettings.Rows[index]["Run Value"] == 1);
                                    break;
                                }
                        }
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                }
            }
            if (snapshot.ConfigurationSnapshotValueStartup.ServerProperties != null && snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows.Count; index++)
                {
                    try
                    {
                        string name = (string)snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Name"];
                        switch (name.Trim().ToLower())
                        {
                            case "windowsversion":
                                {
                                    WindowsVersion = (string)snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"];
                                    break;
                                }
                            case "minutesrunning":
                                {
                                    MinutesRunning = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "encryptedconnections":
                                {
                                    EncryptedConnections = Convert.ToUInt32(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "serverprocessID":
                                {
                                    ServerProcessID = Convert.ToUInt32(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "tempdbrecoverymodel":
                                {
                                    TempDbRecoveryModel = Convert.ToString(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "edition":
                                {
                                    ServerEdition = Convert.ToString(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "physicalmemory":
                                {
                                    PhysicalMemory = Convert.ToUInt64(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "productversion":
                                {
                                    ProductVersion = Convert.ToString(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "sqlserverserviceaccount":
                                {
                                    SQLServerServiceAccount = Convert.ToString(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                            case "semanagevolumeprivilege":
                                {
                                    SeManageVolumePrivilege = Convert.ToString(snapshot.ConfigurationSnapshotValueStartup.ServerProperties.Rows[index]["Value"]);
                                    break;
                                }
                        }
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                }
            }
        }

    }
}
