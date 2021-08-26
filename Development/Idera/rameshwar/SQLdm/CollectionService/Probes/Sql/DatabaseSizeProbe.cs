//------------------------------------------------------------------------------
// <copyright file="DatabaseSizeProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Threading;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using Idera.SQLdm.CollectionService.Helpers;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Services;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Thresholds;
    using System.Data;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.CollectionService.Monitoring;
    using Idera.SQLdm.Common.Snapshots.AlwaysOn;
    using System.Linq;


    /// <summary>
    /// Creates a new probe to collect database statistics for alerting purposes
    /// </summary>
    internal sealed class DatabaseSizeProbe : SqlBaseProbe
    {
        #region fields

        private DatabaseSizeSnapshot refresh = null;
        private DatabaseSizeConfiguration config = null;
        private DiskCollectionSettings diskSettings;
        private AlwaysOnAvailabilityGroupsSnapshot agSnapshot = null;

        //SQLDM-29041
        List<MonitoredSqlServer> monitoredSqlServers = null;
        //Dictionary<string, MonitoredSqlServer> monitoredSqlServers = null;
        AdvancedAlertConfigurationSettings lastBackupDatabaseAdvanced = null;
        private DriveStatisticsWmiProbe driveProbe;
        MetricThresholdEntry lastBackupDatabaseThresholdEntry;

        private WmiConfiguration wmiConfig;
        private string machineName;
        private object driveProbeData;
        private DateTime diskDriveUTCCollectionTime;//SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements -- Added new field for utc collection time
        private List<string> _cloudDBNames;
        private int numberOfCloudDbs;
        private object azureDbStatsLockObject = new object();

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSizeProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public DatabaseSizeProbe(SqlConnectionInfo connectionInfo, DatabaseSizeConfiguration config, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings,int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            LOG = Logger.GetLogger("DatabaseSizeProbe");
            refresh = new DatabaseSizeSnapshot(connectionInfo);
            refresh.Id = config.MonitoredServerId;
            this.config = config;
            this.diskSettings = diskSettings;
            wmiConfig = wmiConfiguration;
        }

        private void PopulateBackupSettings()
        {
            IManagementService mgmtSvc = RemotingHelper.GetObject<IManagementService>();
            monitoredSqlServers = mgmtSvc.GetMonitoredSqlServers();
            lastBackupDatabaseThresholdEntry = mgmtSvc.GetMetricThresholdEntry(GetMonitoredSqlServer(config.MonitoredServerId), (int)Metric.DatabaseLastBackupDate);
            lastBackupDatabaseAdvanced = GetAdvancedConfiguration(lastBackupDatabaseThresholdEntry);
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (config != null && config.ReadyForCollection)//sqldm-30299 changes
            {
                if (wmiConfig.DirectWmiEnabled && cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
                    StartMachineNameCollector();
                else
                    StartDatabaseSizeCollector();
            }
            else
            {
                refresh.TimeStamp = DateTime.UtcNow;
                FireCompletion(refresh, Result.Success);
            }

        }

        #region Direct WMI Collection

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for disk statistics which needs the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            // Passing CloudProvider information
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo, cloudProviderId);
            machineProbe.BeginProbe(MachineNameCallback);
        }

        /// <summary>
        /// Machine Name Collector Call back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNameCallback(object sender, SnapshotCompleteEventArgs e)
        {
            // This means we cancelled out the probe
            if (e.Result == Result.Failure)
                return;
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                {
                    // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
                    machineName = refresh.ServerHostName = _machineSnapshot.MachineName;
                }
            }

            ((IDisposable)sender).Dispose();

            // start the next probe
            StartDiskDriveCollector();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// When doing direct wmi, the database summary probe needs disk size information to use in expansion calculations.
        /// We have to collect the disk size info first and poke it into the temp table (#disk_drives) that the batch uses.
        /// </summary>
        private void StartDiskDriveCollector()
        {
            driveProbe = new DriveStatisticsWmiProbe(machineName, wmiConfig, diskSettings);
            //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Enforcing it to return all 
            driveProbe.ReturnAll = true;
            driveProbe.AutoDiscovery = diskSettings.AutoDiscover;
            if (!diskSettings.AutoDiscover && diskSettings.Drives != null && diskSettings.Drives.Length > 0)
            {
                driveProbe.IncludeAll = false;
                driveProbe.DriveList.AddRange(diskSettings.Drives);
            }
            driveProbe.BeginProbe(OnDiskStatsComplete);
        }

        private void OnDiskStatsComplete(object sender, ProbeCompleteEventArgs args)
        {
            //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type
            var probedata = args.Data as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;
            if (probedata != null && probedata.DiskMap != null && probedata.DiskMap.Count > 0)
            {
                AddDiskStatistics(probedata.DiskMap.Values);
            }
            //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type

            driveProbeData = probedata;

            // start the next probe
            StartDatabaseSizeCollector();
        }

        private void AddDiskStatistics(IEnumerable<DriveStatisticsWmiProbe.DiskStatistics> valueCollection)
        {
            var diskDrives = new Dictionary<string, DiskDrive>();
            var previousDiskDrives = (config.PreviousValues != null) ? config.PreviousValues.DiskDrives : null;

            // blow out the mount points for each volume
            foreach (var disk in valueCollection)
            {
                if (disk.DriveType == 4 || disk.DriveType == 5) continue;

                foreach (var mount in disk.Paths)
                {
                    var dd = new DiskDrive();
                    dd.DriveLetter = mount;
                    dd.UnusedSize.Bytes = disk.FreeSpace;
                    dd.TotalSize.Bytes = disk.TotalSize;
                    dd.DiskIdlePercentRaw = disk.PercentIdleTime;
                    dd.DiskIdlePercentBaseRaw = disk.PercentIdleTime_Base;
                    dd.AverageDiskQueueLengthRaw = disk.AvgDiskQueueLength;
                    dd.Timestamp_Sys100ns = disk.TimeStamp_Sys100NS;
                    dd.AvgDisksecPerReadRaw = disk.AvgDisksecPerRead;
                    dd.AvgDisksecPerRead_Base = disk.AvgDisksecPerRead_Base;
                    dd.AvgDisksecPerTransferRaw = disk.AvgDisksecPerTransfer;
                    dd.AvgDisksecPerTransfer_Base = disk.AvgDisksecPerTransfer_Base;
                    dd.AvgDisksecPerWriteRaw = disk.AvgDisksecPerWrite;
                    dd.AvgDisksecPerWrite_Base = disk.AvgDisksecPerWrite_Base;
                    dd.Frequency_Perftime = disk.Frequency_Perftime;
                    dd.DiskReadsPerSec_Raw = disk.DiskReadsPerSec;
                    dd.DiskTransfersPerSec_Raw = disk.DiskTransfersPerSec;
                    dd.DiskWritesPerSec_Raw = disk.DiskWritesPerSec;
                    dd.TimeStamp_PerfTime = disk.TimeStamp_PerfTime;
                    dd.Timestamp_utc = refresh.TimeStamp;
                    
                    ProbeHelpers.CalcDiskDrive(dd,previousDiskDrives);

                    diskDrives.Add(dd.DriveLetter, dd);
                }
            }

            refresh.DiskDrives = diskDrives;
        }

        #endregion

        /// <summary>
        /// Define the DatabaseSize collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DatabaseSizeCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildDatabaseSizeCommand(conn, ver, wmiConfig, diskSettings, driveProbeData, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseSizeCallback));
        }

        /// <summary>
        /// Starts the Database Files collector.
        /// </summary>
        private void StartDatabaseSizeCollector()
        {
            if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                numberOfCloudDbs = 0;
                _cloudDBNames = CollectionHelper.GetDatabases(connectionInfo, LOG);
                if (_cloudDBNames.Count > 0)
                {
                    StartGenericCollectorDatabase(AzureDatabaseStatisticsCollectorDb, refresh, "StartDatabaseSizeCollectorDb", "Database Size", AzureDatabaseStatisticsCallbackDb, _cloudDBNames[numberOfCloudDbs], new object[] { });
                }
                else
                {
                    refresh.TimeStamp = DateTime.UtcNow;
                    FireCompletion(refresh, Result.Success);
                }
                return;
            }
            //Arg params for DiskSize2005
            var driveProbeDataParam = (DriveStatisticsWmiProbe.DriveStatisticsWmiDetails)driveProbeData;
            bool isWMICallFailed = (driveProbeDataParam == null || driveProbeDataParam.DiskMap == null || driveProbeDataParam.DiskMap.Count == 0);

            // Passing Permission Violation Callback DatabaseSizeCallback
            StartGenericCollector(new Collector(DatabaseSizeCollector), refresh, "StartDatabaseSizeCollector", "Database Size", DatabaseSizeCallback, new object[] { isWMICallFailed });
        }


        private void AzureDatabaseStatisticsCallbackDb(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AzureDatabaseStatisticsCallbackDb), refresh,
                "StartDatabaseSizeCollectorDb", "Database Size",
                new FailureDelegate(GenericFailureDelegate), new FailureDelegate(GenericFailureDelegate),
                            null, sender, e, true, true);
            /*GenericCallback(new CollectorCallback(AzureDatabaseStatisticsCallbackDb), refresh, "StartDatabaseSizeCollectorDb", "Database Size",
                sender, e);*/
        }

        private void AzureDatabaseStatisticsCallbackDb(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretAzureDatabaseStatistics(rd);
                }
            }
                Interlocked.Increment(ref numberOfCloudDbs);
                if (numberOfCloudDbs >= _cloudDBNames.Count)
                {
                    if (refresh.DbStatistics != null && refresh.DbStatistics.Count > 0)
                    {
                        foreach (var dbFile in refresh.DbStatistics.SelectMany(dbStats => dbStats.Value.FileStatistics))
                        {
                            dbFile.UTCCollectionDateTime = refresh.TimeStamp;
                        }
                    }

                    FireCompletion(refresh, Result.Success);
                }
                else
                {
                    StartGenericCollectorDatabase(AzureDatabaseStatisticsCollectorDb, refresh, "StartDatabaseSizeCollectorDb", "Database Size", AzureDatabaseStatisticsCallbackDb, _cloudDBNames[numberOfCloudDbs], new object[] { });
                }
            //}
            /*else
            {
                FireCompletion(refresh, e.Result);
            }*/
        }

        private void AzureDatabaseStatisticsCollectorDb(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbName)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDatabaseSizeCommand(conn, ver, wmiConfig, diskSettings, driveProbeData, cloudProviderId);

            sdtCollector = new SqlCollector(cmd, true);

            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AzureDatabaseStatisticsCallbackDb));
        }

        private void InterpretAzureDatabaseStatistics(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAzureDatabaseStatistics"))
            {
                try
                {
                    if (config.PreviousValues != null)
                    {
                        refresh.TimeDelta = refresh.TimeStamp - config.PreviousValues.TimeStamp;
                    }

                    ReadDatabaseSizes(dataReader);

                    // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --call read functions for files and disk drive statistics
                    dataReader.NextResult();
                    ReadDatabaseFilesStats(dataReader);
                    // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --call read functions for files and disk drive statistics
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Azure Database Statistics collector.", e, false);
                }
            }
        }


        /// <summary>
        /// Define the DatabaseSize callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseSizeCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseSize(rd);
            }
            FireCompletion(refresh, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseSize collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseSizeCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseSizeCallback), refresh, "DatabaseSizeCallback", "Database Size",
                            sender, e);
        }

        #region Database Size

        #region Database Size Interpretation Methods

        private void InterpretDatabaseSize(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseSize"))
            {
                try
                {
                    if (config.PreviousValues != null)
                    {
                        refresh.TimeDelta = refresh.TimeStamp - config.PreviousValues.TimeStamp;
                    }
                   
                    ReadDiskDrives(dataReader);

                    dataReader.NextResult();
                    ReadDatabaseSizes(dataReader);

                    // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --call read functions for files and disk drive statistics
                    dataReader.NextResult();
                    ReadDiskDrivesStats(dataReader);
                    dataReader.NextResult();
                    ReadDatabaseFilesStats(dataReader);
                    // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --call read functions for files and disk drive statistics
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,LOG,"Error interpreting Database Size collector.",e,false);
                }
            }
        }

        private void ReadDiskDrives(SqlDataReader dataReader)
        {
            try
            {
                if (!wmiConfig.DirectWmiEnabled)
                {
                    bool IsFileSystemObjectDataAvailable = false;
                    refresh.DiskDrives = ProbeHelpers.ReadDiskDrives(dataReader, config.PreviousValues != null
                                                                     ? config.PreviousValues.DiskDrives : null,
                                                                     out IsFileSystemObjectDataAvailable, LOG);
                    if (!IsFileSystemObjectDataAvailable)
                        LOG.Info("Extended disk information is unavailable for server " + refresh.ServerName);
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Disk Drives data: {0}", exception,
                                                    false);
                return;
            }
        }

        private void ReadDatabaseSizes(SqlDataReader dataReader)
        {
            try
            {
                string dbName = "";
                while (dataReader.FieldCount > 1)
                {
                    while (dataReader.Read())
                    {
                        //SQLDM-28907. Availability Grop Read Databases.
                        // Database Statistics (13) or Database File Statistics (8)
                        if (dataReader.FieldCount == 13 || dataReader.FieldCount == 8)
                        {
                            try
                            {
                                if (!dataReader.IsDBNull(0)) dbName = dataReader.GetString(0); // TrimEnd removed by Tolga K for SQLDM-19203 .TrimEnd();
                            }
                            catch (Exception exception)
                            {
                                LOG.Error("Unable to get string from dataReader", exception);
                                dbName = null;
                            }
                        }
                        else if (cloudProviderId == Constants.MicrosoftAzureId)
                        {
                            break;
                        }

                        //Only continue if we have a real database name and it's not mssqlsystemresource (an internal database)
                        if (dbName != null && dbName.Length > 0 && dbName != "mssqlsystemresource")
                        {
                            //Add the database to our dictionary if we haven't already
                            if (!refresh.DbStatistics.ContainsKey(dbName))
                            {
                                refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName));
                            }

                            LOG.DebugFormat("ReadDatabaseSizeProbe - Field count: {0}", dataReader.FieldCount);

                            //Database size data
                            if (dataReader.FieldCount == 13)
                            {

                                try
                                {
                                    if (!dataReader.IsDBNull(1))
                                        refresh.DbStatistics[dbName].Status = (DatabaseStatus)dataReader.GetInt32(1);
                                }
                                catch
                                {
                                    refresh.DbStatistics[dbName].Status = DatabaseStatus.Undetermined;
                                }
                                if (!dataReader.IsDBNull(2))
                                    refresh.DbStatistics[dbName].DataFileSize.Kilobytes = dataReader.GetDecimal(2);
                                if (!dataReader.IsDBNull(3))
                                    refresh.DbStatistics[dbName].DataSize.Kilobytes = dataReader.GetDecimal(3);
                                if (!dataReader.IsDBNull(4))
                                    refresh.DbStatistics[dbName].TextSize.Kilobytes = dataReader.GetDecimal(4);
                                if (!dataReader.IsDBNull(5))
                                    refresh.DbStatistics[dbName].IndexSize.Kilobytes = dataReader.GetDecimal(5);
                                if (!dataReader.IsDBNull(6))
                                    refresh.DbStatistics[dbName].IsSystemDatabase = dataReader.GetBoolean(6);
                                if (!dataReader.IsDBNull(7))
                                    refresh.DbStatistics[dbName].LogExpansion.Kilobytes = dataReader.GetDecimal(7);
                                if (!dataReader.IsDBNull(8))
                                    refresh.DbStatistics[dbName].DatabaseExpansion.Kilobytes = dataReader.GetDecimal(8);
                                if (!dataReader.IsDBNull(9))
                                    refresh.DbStatistics[dbName].FileSpaceUsed.Kilobytes = dataReader.GetDecimal(9);
                                if (!dataReader.IsDBNull(10))
                                    refresh.DbStatistics[dbName].DateCreated = dataReader.GetDateTime(10);
                                if (!dataReader.IsDBNull(11))
                                    refresh.DbStatistics[dbName].LastBackup = dataReader.GetDateTime(11);
                                if (!dataReader.IsDBNull(12))
                                    refresh.DbStatistics[dbName].IsPrimary = Convert.ToBoolean(dataReader.GetInt32(12));

                                LOG.DebugFormat("DatabaseSizeProbe - databaseName: {0} " +
                                                ", Status {1} " +
                                                ", DataFileSize {2} " +
                                                ", DataSize {3} " +
                                                ", TextSize {4} " +
                                                ", IndexSize {5} " +
                                                ", IsSystemDatabase {6} " +
                                                ", LogExpansion {7} " +
                                                ", DatabaseExpansion {8} " +
                                                ", FileSpaceUsed {9} " +
                                                ", DateCreated {10} " +
                                                ", LastBackup {11} ."
                                                , refresh.DbStatistics[dbName].Name
                                                , refresh.DbStatistics[dbName].Status
                                                , refresh.DbStatistics[dbName].DataFileSize.Kilobytes
                                                , refresh.DbStatistics[dbName].DataSize.Kilobytes
                                                , refresh.DbStatistics[dbName].TextSize.Kilobytes
                                                , refresh.DbStatistics[dbName].IndexSize.Kilobytes
                                                , refresh.DbStatistics[dbName].IsSystemDatabase
                                                , refresh.DbStatistics[dbName].LogExpansion.Kilobytes
                                                , refresh.DbStatistics[dbName].DatabaseExpansion.Kilobytes
                                                , refresh.DbStatistics[dbName].FileSpaceUsed.Kilobytes
                                                , refresh.DbStatistics[dbName].DateCreated
                                                , refresh.DbStatistics[dbName].LastBackup);

                            }
                            else if (dataReader.FieldCount == 2) //Ag Group Info
                            {
                                //only read the data for SQL Server 2012 or higher
                                if (refresh.ProductVersion.Major >= 11)
                                {
                                    ReadAGGroupData(dataReader, dbName);
                                }
                            }
                            else if (dataReader.FieldCount == 3)  //AG Replicas
                            {
                                //only read the data for SQL Server 2012 or higher
                                if (refresh.ProductVersion.Major >= 11)
                                {
                                    ReadAGReplicaData(dataReader, dbName);
                                }
                            }
                            else if (dataReader.FieldCount == 6) //AG Database(s)
                            {
                                //only read the data for SQL Server 2012 or higher
                                if (refresh.ProductVersion.Major >= 11)
                                {
                                    if (ReadAGDBData(dataReader, dbName))
                                        HandleLastBackupDateAg(dbName);

                                }
                            }
                            else //Database file statistics
                            {
                                FileActivityFile file = new FileActivityFile();
                                if (!dataReader.IsDBNull(0)) file.DatabaseName = dataReader.GetString(0);
                                if (!dataReader.IsDBNull(1)) file.ReadsRaw = file.Reads = dataReader.GetDecimal(1);
                                if (!dataReader.IsDBNull(2)) file.WritesRaw = file.Writes = dataReader.GetDecimal(2);
                                if (!dataReader.IsDBNull(4)) file.Filename = dataReader.GetString(4);
                                if (!dataReader.IsDBNull(5)) file.FileType = dataReader.GetInt32(5) > 0 ? FileActivityFileType.Data : FileActivityFileType.Log;
                                if (!dataReader.IsDBNull(6)) file.Filepath = dataReader.GetString(6);
                                if (!dataReader.IsDBNull(7)) file.DriveName = dataReader.GetString(7);

                                if (config.PreviousValues != null && config.PreviousValues.DbStatistics != null && config.PreviousValues.DbStatistics.ContainsKey(dbName)
                                    && config.PreviousValues.DbStatistics[dbName].Files != null && config.PreviousValues.DbStatistics[dbName].Files.Count > 0 && config.PreviousValues.DbStatistics[dbName].Files.ContainsKey(file.Filepath))
                                {

                                    if (config.PreviousValues.DbStatistics[dbName].Files[file.Filepath] != null)
                                    {
                                        FileActivityFile prevActivity =
                                            config.PreviousValues.DbStatistics[dbName].Files[file.Filepath];

                                        if (file.ReadsRaw.HasValue && prevActivity.ReadsRaw.HasValue && file.ReadsRaw.Value >= prevActivity.ReadsRaw.Value)
                                        {
                                            file.Reads =
                                                (file.ReadsRaw.Value - prevActivity.ReadsRaw.Value);
                                        }

                                        if (file.WritesRaw.HasValue && prevActivity.WritesRaw.HasValue && file.WritesRaw.Value >= prevActivity.WritesRaw.Value)
                                        {
                                            file.Writes = (file.WritesRaw.Value - prevActivity.WritesRaw.Value);
                                        }
                                    }

                                }

                                if (refresh.DbStatistics[dbName].Files.ContainsKey(file.Filepath))
                                {
                                    if (refresh.DbStatistics[dbName].Files[file.Filepath].GetType() == typeof(TempdbFileActivity))
                                    {
                                        ((TempdbFileActivity)refresh.DbStatistics[dbName].Files[file.Filepath]).
                                            SetValuesFromFileActivity(file);
                                    }
                                    else
                                    {
                                        refresh.DbStatistics[dbName].Files[file.Filepath] = file;
                                    }
                                }
                                else
                                {
                                    refresh.DbStatistics[dbName].Files.Add(file.Filepath, file);
                                }
                            }
                        }

                    }
                    dataReader.NextResult();
                }
				dataReader.NextResult();
                
				if (cloudProviderId != Constants.MicrosoftAzureId)
                {
					dataReader.Read();
                    ServerOverviewInterpreter.ReadDatabaseSummary(dataReader, refresh.DatabaseSummary, refresh, LOG);
                }
                ReadLogSizes(dataReader);
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read database sizes failed: {0}", exception, false);
                return;
            }
        }

        //SQLDM-29041. Handle Last Backup Date for AG Environment 
        private void HandleLastBackupDateAg(String dbName)
        {
            String agOption = lastBackupDatabaseAdvanced.AvailabilityGroupOption;

            if (agOption.Equals("Default"))
            {
                return;
            }
            
            String replicaCollectionOption = lastBackupDatabaseAdvanced.ReplicaCollectionOption;

            List<String> serverNames = monitoredSqlServers.Select(ss => ss.InstanceName.ToLower()).ToList();

            var lastBackupTime = refresh.DbStatistics[dbName].LastBackup;
            refresh.DbStatistics[dbName].LastBackup = null;

            foreach (AvailabilityGroup ag in agSnapshot.AvailabilityGroupItems)
                {
                    var replicaItems = ag.ReplicaItems;
                    if (agOption.Equals("Primary") || agOption.Equals("Secondary"))
                    {
                        replicaItems.Where(ar => ar.ReplicaRole.Equals(agOption));
                    }


                    foreach (AvailabilityReplica ar in replicaItems)
                    {
                        if (ar.SqlServerId == config.MonitoredServerId)
                        {
                            UpdateLastBackupTime(dbName, lastBackupTime);
                            continue;
                        }

                        if (replicaCollectionOption.Equals("MonitoredOnly") && !serverNames.Contains(ar.ReplicaName))
                        {
                            continue;
                        }
                        if (serverNames.Contains(ar.ReplicaName.ToLower()))
                        {
                            MonitoredSqlServer monitoredSqlServer = GetMonitoredSqlServer(ar.ReplicaName.ToLower());
                            UpdateLastBackupTime(dbName, ag.GroupId, ar.ReplicaId, monitoredSqlServer);
                        }
                        else
                        {
                            UpdateLastBackupTime(dbName, ag.GroupId, ar.ReplicaId);
                        }
                    }

                }

            if (refresh.DbStatistics[dbName].LastBackup == null)
            {
                refresh.DbStatistics[dbName].LastBackup = lastBackupTime;
            }
        }

        private MonitoredSqlServer GetMonitoredSqlServer(string serverName)
        {
            MonitoredSqlServer monitoredSqlServer = null;

            foreach(MonitoredSqlServer sqlServer in monitoredSqlServers)
            {
                if (sqlServer.InstanceName.ToLower().Equals(serverName))
                {
                    monitoredSqlServer = sqlServer;
                }
            }
            return monitoredSqlServer;
        }

        //SQLDM-29041. Update Last Backup Date for AG Environment 
        private void UpdateLastBackupTime(String dbName, Guid groupId, Guid replicaId, MonitoredSqlServer monitoredSqlServer = null)
        {
            DateTime? newbackupdatetime = GetLastBackupTimeOfAvailabilityGroupDatabase(agSnapshot.AvailabilityGroups[groupId].Replicas[replicaId].ReplicaName, dbName, monitoredSqlServer);
            if ((newbackupdatetime != null && refresh.DbStatistics[dbName].LastBackup == null) ||
                (newbackupdatetime != null && refresh.DbStatistics[dbName].LastBackup != null &&
                newbackupdatetime > refresh.DbStatistics[dbName].LastBackup))
            {
                refresh.DbStatistics[dbName].LastBackup = newbackupdatetime;
            }
        }

        //SQLDM-29041. Update Last Backup Date for AG Environment 
        private void UpdateLastBackupTime(string dbName, DateTime? newbackupdatetime)
        {
            if ((newbackupdatetime != null && refresh.DbStatistics[dbName].LastBackup == null) ||
                            (newbackupdatetime != null && refresh.DbStatistics[dbName].LastBackup != null &&
                            newbackupdatetime > refresh.DbStatistics[dbName].LastBackup))
            {
                refresh.DbStatistics[dbName].LastBackup = newbackupdatetime;
            }
        }

        //SQLDM-29041. Get Advanced Threshold Settings for AG.
        private AdvancedAlertConfigurationSettings GetAdvancedConfiguration(MetricThresholdEntry lastBackupDatabaseThresholdEntry)
        {
            AdvancedAlertConfigurationSettings settings = null;
            if (lastBackupDatabaseThresholdEntry.Data != null)
            {
                settings = lastBackupDatabaseThresholdEntry.Data as AdvancedAlertConfigurationSettings;
            }
            if (settings == null)
            {
                settings = new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(lastBackupDatabaseThresholdEntry.MetricID), lastBackupDatabaseThresholdEntry.Data);
            }
            return settings;
        }

        //SQLDM-29041. Return Sqlserver.
        private MonitoredSqlServer GetMonitoredSqlServer(int serverId)
        {
            
            foreach(MonitoredSqlServer sqlServer in monitoredSqlServers)
            {
                if (sqlServer.Id == serverId)
                {
                    return sqlServer;
                }
            }
            return null;
        }


        /// <summary>
        /// SQLDM 10.1 (Tolga K) - Availability group related improvements - populate last backup date of a non-monitored server
        /// </summary>
        /// <param name="serverSourceName"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private DateTime? GetLastBackupTimeOfAvailabilityGroupDatabase(string serverSourceName, string dbName, MonitoredSqlServer sqlServer = null)
        {
            DateTime? newBackupDateTime = null;
            SqlConnectionInfo newConnection = null;
            if(sqlServer == null) {
                newConnection = connectionInfo.Clone();
            }
            else
            {
                newConnection = sqlServer.ConnectionInfo;
            }

            
            //newConnection.DatabaseName = dbName;
            newConnection.InstanceName = serverSourceName;
            using (SqlConnection conn = new SqlConnection(newConnection.ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = SqlCommandBuilder.BuildDatabaseLastBackupDateTimeCommand(conn, dbName);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        newBackupDateTime = reader.GetDateTime(0);
                    }
                }
                catch (Exception ex)
                {
                    LOG.DebugFormat("DatabaseSizeProbe - Could not connect to remote server {0} reading last backup time through availability group information ({1})", serverSourceName, ex.Message);
                }
                finally
                {
                    if (conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }

            return newBackupDateTime;
        }

        /// <summary>
        /// Read the AG Group Data
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dbName"></param>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        private bool ReadAGGroupData(SqlDataReader dataReader, string dbName)
        {
            Common.Snapshots.AlwaysOn.AvailabilityGroup avg;

            try
            {
                if (agSnapshot == null)
                    agSnapshot = new AlwaysOnAvailabilityGroupsSnapshot();

                // Fetch availability groups
                while (dataReader.Read())
                {
                    avg = new Common.Snapshots.AlwaysOn.AvailabilityGroup();
                    if (dataReader["group_id"] != System.DBNull.Value) avg.GroupId = (Guid)dataReader["group_id"];
                    if (dataReader["Groupname"] != System.DBNull.Value) avg.GroupName = (string)dataReader["GroupName"];

                    if (!agSnapshot.AvailabilityGroups.ContainsKey(avg.GroupId))
                    {
                        agSnapshot.AddAvailabilityGroup(avg);
                    }
                }
            }
            catch (Exception ex)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "error occurred in ReadAGGroupData. Details: {0}", ex, false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Read the AG Replica Data
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dbName"></param>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        private bool ReadAGReplicaData(SqlDataReader dataReader, string dbName)
        {
            Common.Snapshots.AlwaysOn.AvailabilityReplica replica;

            try
            {
                if (agSnapshot == null)
                    agSnapshot = new AlwaysOnAvailabilityGroupsSnapshot();

                while (dataReader.Read())
                {
                    replica = new Common.Snapshots.AlwaysOn.AvailabilityReplica((Guid)dataReader["group_id"]);

                    if (dataReader["replica_id"] != System.DBNull.Value) replica.ReplicaId = (Guid)dataReader["replica_id"];
                    if (dataReader["Replica_Name"] != System.DBNull.Value) replica.ReplicaName = (string)dataReader["Replica_Name"];

                    if (agSnapshot.AvailabilityGroups.ContainsKey(replica.GroupId))
                    {
                        if (!agSnapshot.AvailabilityGroups[replica.GroupId].Replicas.ContainsKey(replica.ReplicaId))
                        {
                            agSnapshot.AvailabilityGroups[replica.GroupId].AddReplica(replica);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "error occurred in ReadAGReplicaData. Details: {0}", ex, false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Read the AG Database Data
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dbName"></param>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        private bool ReadAGDBData(SqlDataReader dataReader, string dbName)
        {
            Common.Snapshots.AlwaysOn.AlwaysOnDatabase db;
            DateTime? newBackupDateTime = null;

            try
            {
                if (agSnapshot == null)
                    agSnapshot = new AlwaysOnAvailabilityGroupsSnapshot();

                while (dataReader.Read())
                {
                    db = new Common.Snapshots.AlwaysOn.AlwaysOnDatabase();
                    if (dataReader["replica_id"] != System.DBNull.Value) db.ReplicaId = (Guid)dataReader["replica_id"];
                    if (dataReader["group_id"] != System.DBNull.Value) db.GroupId = (Guid)dataReader["group_id"];
                    if (dataReader["group_database_id"] != System.DBNull.Value) db.GroupDatabaseId = (Guid)dataReader["group_database_id"];
                    if (dataReader["database_id"] != System.DBNull.Value) db.DatabaseId = (int)dataReader["database_id"];
                    if (dataReader["DatabaseName"] != System.DBNull.Value) db.DatabaseName = (string)dataReader["DatabaseName"];
                    if (dataReader["LastBackupTime"] != System.DBNull.Value) newBackupDateTime = (DateTime)dataReader["LastBackupTime"];

                    bool isPresent = false;
                    if ((newBackupDateTime != null && refresh.DbStatistics[dbName].LastBackup == null) ||
                                                    (newBackupDateTime != null && refresh.DbStatistics[dbName].LastBackup != null &&
                                                    newBackupDateTime > refresh.DbStatistics[dbName].LastBackup))
                    {
                        refresh.DbStatistics[dbName].LastBackup = newBackupDateTime;
                    }

                    if (agSnapshot.AvailabilityGroups.ContainsKey(db.GroupId) && agSnapshot.AvailabilityGroups[db.GroupId].Replicas.ContainsKey(db.ReplicaId))
                    {
                        foreach (Common.Snapshots.AlwaysOn.AlwaysOnDatabase pdb in agSnapshot.AvailabilityGroups[db.GroupId].Replicas[db.ReplicaId].DatabaseItems)
                        {
                            if (pdb.DatabaseId == db.DatabaseId)
                            {
                                isPresent = true;
                                break;
                            }
                        }

                        if (!isPresent)
                        {
                            agSnapshot.AvailabilityGroups[db.GroupId].Replicas[db.ReplicaId].AddDatabase(db);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "error occurred in ReadAGDBData. Details: {0}", ex, false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --populate the Disk Drives stats object 
        /// </summary>
        /// <param name="dataReader"></param>
        private void ReadDiskDrivesStats(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    if (refresh.DiskDriveStatistics == null)
                    {
                        refresh.DiskDriveStatistics = new List<DiskDriveStatistics>();
                    }
                    diskDriveUTCCollectionTime = new DateTime();
                    DiskDriveStatistics diskDrive = new DiskDriveStatistics();
                    if (!dataReader.IsDBNull(0)) diskDrive.DriveName = dataReader.GetString(0).Trim();
                    if (!dataReader.IsDBNull(1)) diskDrive.UnusedSizeKB = (double)(dataReader.GetDecimal(1))/1024;
                    if (!dataReader.IsDBNull(2)) diskDrive.TotalSizeKB = (double)(dataReader.GetDecimal(2)) / 1024;
                    if (!dataReader.IsDBNull(5)) diskDrive.UTCCollectionDateTime = diskDriveUTCCollectionTime=dataReader.GetDateTime(5);

                    if (!dataReader.IsDBNull(3)) diskDrive.DiskReadsPerSecond = Convert.ToDouble(dataReader.GetString(3));
                    if (!dataReader.IsDBNull(4)) diskDrive.DiskWritesPerSecond = Convert.ToDouble(dataReader.GetString(4));

                    refresh.DiskDriveStatistics.Add(diskDrive);
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read disk drives statistics failed: {0}", exception, false);
                return;
            }
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --populate the database files stats object 
        /// </summary>
        /// <param name="dataReader"></param>
        private void ReadDatabaseFilesStats(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    //Add the database to our dictionary if we haven't already
                    string dbName = Convert.ToString(dataReader["DatabaseName"]);
                    int currentOrdinal = -1;

                    if (!refresh.DbStatistics.ContainsKey(dbName))
                    {
                        refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName));
                    }

                    if (refresh.DbStatistics[dbName].FileStatistics ==null)
                    {
                        refresh.DbStatistics[dbName].FileStatistics= new List<DatabaseFileStatistics>();
                    }

                    DatabaseFileStatistics file = new DatabaseFileStatistics();

                    file.DatabaseName = dbName;
                    currentOrdinal = dataReader.GetOrdinal("FileName");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.FileName = Convert.ToString(dataReader[currentOrdinal]);
                    
                    currentOrdinal = dataReader.GetOrdinal("IsDataFile");
                    if (!dataReader.IsDBNull(currentOrdinal)) Convert.ToBoolean(dataReader[currentOrdinal]);

                    currentOrdinal = dataReader.GetOrdinal("FilegroupName");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.FileGroupName = Convert.ToString(dataReader[currentOrdinal]);

                    currentOrdinal = dataReader.GetOrdinal("InitialSizeMB");
                    if(!dataReader.IsDBNull(currentOrdinal)) file.InitialSize = Convert.ToDouble(dataReader[currentOrdinal]);

                    currentOrdinal = dataReader.GetOrdinal("SpaceUsedMB");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.UsedSpace = Convert.ToDouble(dataReader[currentOrdinal]);

                    currentOrdinal = dataReader.GetOrdinal("AvailableSpaceMB");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.AvailableSpace = Convert.ToDouble(dataReader[currentOrdinal]);

                    currentOrdinal = dataReader.GetOrdinal("MaxSizeMB");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.MaximumSize = Convert.ToDouble(dataReader[currentOrdinal]);
                    
                    currentOrdinal = dataReader.GetOrdinal("FreeDiskSpaceMB");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.FreeDiskSpace = dataReader[currentOrdinal] != DBNull.Value ? Convert.ToDouble(dataReader[currentOrdinal]) : 0.0;

                    currentOrdinal = dataReader.GetOrdinal("FilePath");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.FilePath = Convert.ToString(dataReader[currentOrdinal]);

                    currentOrdinal = dataReader.GetOrdinal("DriveName");
                    if (!dataReader.IsDBNull(currentOrdinal)) file.DriveName = Convert.ToString(dataReader[currentOrdinal]);


                    currentOrdinal = -1; //just resetting to -1 so that it does not connect

                    file.UTCCollectionDateTime = diskDriveUTCCollectionTime;

                    refresh.DbStatistics[dbName].FileStatistics.Add(file);
                }

            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read database files stats failed: {0}", exception, false);
                return;
            }
        }

        /// <summary>
        /// Interpret DatabaseSize data
        /// </summary>
        private void ReadLogSizes(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadLogSizes"))
            {
                try
                {
                    FileSize totalLogSize = new FileSize();
                    List<string> missingExpansion = new List<string>();
                    //Do not offset the fields by default
                    int sql2012FieldOffset = 0;

                    while (dataReader.Read())
                    {
                        //Build 11.0.1750 RC0 return 5 fields. This is fullyexpected to change back to 4 fields.
                        //On this build the second column is "Principal Filegroup Name"
                        if (dataReader.FieldCount == 5 & dataReader.GetName(1).Equals("Principal Filegroup Name", StringComparison.InvariantCultureIgnoreCase))
                        {
                            sql2012FieldOffset = 1;
                            LOG.Debug("build 11.0.1750 specific code for interpreting log sizes");
                        }
                        else
                            //This is not 2012. The filedcount must be 4
                            //Check for valid data
                            if (dataReader.FieldCount != 4)
                            {
                                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG,
                                                                    "Read log sizes failed - fieldcount was incorrect",
                                                                    false);
                                return;
                            }


                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                        {
                            string dbName = dataReader.GetString(0); // TrimEnd removed by Tolga K for SQLDM-19203 .TrimEnd();

                            //Add the database to our dictionary if we haven't already
                            if (!refresh.DbStatistics.ContainsKey(dbName) && dbName != "mssqlsystemresource")
                            {
                                refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName));
                            }

                            // Only attempt calculations for accessible databases
                            if (refresh.DbStatistics[dbName].IsAccessibleStatus &&
                                dbName != "mssqlsystemresource")
                            {
                                //Read log size
                                refresh.DbStatistics[dbName].LogFileSize.Megabytes = (decimal)dataReader.GetFloat(1 + sql2012FieldOffset);

                                //The log percent is read as a real percentage, so divide to get into decimal
                                decimal logPercent = (decimal)dataReader.GetFloat(2 + sql2012FieldOffset) / 100;

                                // Changed by Tolga K for SQLDM-19207
                                if (logPercent > 1)
                                {
                                    logPercent = 1;
                                }

                                //Used log size is log percent used multiplied by file size
                                refresh.DbStatistics[dbName].LogSizeUsed.Bytes =
                                    refresh.DbStatistics[dbName].LogFileSize.Bytes * logPercent;

                                if ((refresh.DbStatistics[dbName].LogSizeUsed.Bytes >= refresh.DbStatistics[dbName].LogFileSize.Bytes)
                                    || (refresh.DbStatistics[dbName].LogSizeUsed.Bytes < 0))
                                {
                                    LOG.Info(String.Format("Invalid log size detected for database {0}.  Log Size Used: {1}, Log File Size: {2}", dbName,
                                        refresh.DbStatistics[dbName].LogSizeUsed.Bytes, refresh.DbStatistics[dbName].LogFileSize.Bytes));
                                }
                                else
                                {
                                    if (totalLogSize.Bytes.HasValue)
                                    {
                                        totalLogSize.Bytes += refresh.DbStatistics[dbName].LogSizeUsed.Bytes;
                                    }
                                    else
                                    {
                                        totalLogSize.Bytes = refresh.DbStatistics[dbName].LogSizeUsed.Bytes;
                                    }
                                }

                                //Set up percent used according to expansion if expansion is allowed
                                if (refresh.DbStatistics[dbName].LogExpansion.Bytes.HasValue &&
                                    refresh.DbStatistics[dbName].LogExpansion.Bytes != 0 &&
                                    refresh.DbStatistics[dbName].LogSizeUsed.Bytes.HasValue)
                                {
                                    //Include expansion amount if expansion is allowed
                                    refresh.DbStatistics[dbName].PercentLogSpace = (float)(
                                                                                               refresh.DbStatistics[
                                                                                                   dbName].
                                                                                                   LogSizeUsed.Bytes /
                                                                                               (refresh.DbStatistics[
                                                                                                    dbName]
                                                                                                    .LogFileSize.Bytes +
                                                                                                refresh.DbStatistics[
                                                                                                    dbName]
                                                                                                    .LogExpansion.Bytes));
                                }
                                else
                                {
                                    if (logPercent <= 1)
                                    {
                                        //Use value from probe if expansion is not allowed
                                        refresh.DbStatistics[dbName].PercentLogSpace = (float)logPercent;
                                    }
                                    else
                                    {
                                        refresh.DbStatistics[dbName].PercentLogSpace = 1; // null; Tolga K for SQLDM-19207
                                    }
                                }

                                //We actually calculate database percent used here to make good use of the loop
                                if (refresh.DbStatistics[dbName].DataFileSize.Bytes.HasValue &&
                                    refresh.DbStatistics[dbName].DataFileSize.Bytes != 0)
                                {
                                    refresh.DbStatistics[dbName].PercentDataSize =
                                        (float)(refresh.DbStatistics[dbName].UsedSize.Bytes /
                                                 (refresh.DbStatistics[dbName].DataFileSize.Bytes +
                                                 (refresh.DbStatistics[dbName].DatabaseExpansion.Bytes.HasValue ? refresh.DbStatistics[dbName].DatabaseExpansion.Bytes.Value : 0)));
                                }

                                if (!refresh.DbStatistics[dbName].DatabaseExpansion.Bytes.HasValue || !refresh.DbStatistics[dbName].LogExpansion.Bytes.HasValue)
                                    missingExpansion.Add(dbName);
                            }
                        }
                    }

                    refresh.DatabaseSummary.LogFileSpaceUsed.Bytes = totalLogSize.Bytes;

                    if (missingExpansion.Count > 0)
                    {
                        StringBuilder missingExpansionMessage = new StringBuilder();
                        missingExpansionMessage.AppendLine("Database or log expansion could not be determined for the following databases.  This could mean that the relevant disk drives are not being monitored.");
                        foreach (string db in missingExpansion)
                        {
                            missingExpansionMessage.Append(db);
                            missingExpansionMessage.Append(",");
                        }
                        missingExpansionMessage.Remove(missingExpansionMessage.Length - 1, 1);
                        LOG.Warn(missingExpansionMessage);
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read log sizes failed: {0}", exception, false);
                    return;
                }
            }
        }


        #endregion

        #endregion


        #endregion

        #region interface implementations

        #endregion
    }
}
