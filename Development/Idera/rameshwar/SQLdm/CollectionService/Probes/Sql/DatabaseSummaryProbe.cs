//------------------------------------------------------------------------------
// <copyright file="DatabaseSummaryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Idera.SQLdm.CollectionService.Probes.Wmi;

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
    using Idera.SQLdm.CollectionService.Helpers;
    using System.Threading;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class DatabaseSummaryProbe : SqlBaseProbe
    {
        #region fields

        private DatabaseSummary databaseSummary = null;
        DatabaseSummaryConfiguration config = null;
        private DiskCollectionSettings diskSettings;

        private DriveStatisticsWmiProbe driveProbe;
        private WmiConfiguration wmiConfig;
        private string machineName;
        private object driveProbeData;

        private List<string> _cloudDBNames;
        private int numberOfCloudDbs;
        private object azureDbStatsLockObject = new object();
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSummaryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public DatabaseSummaryProbe(SqlConnectionInfo connectionInfo, 
                                    DatabaseSummaryConfiguration config, 
                                    WmiConfiguration wmiConfiguration, 
                                    DiskCollectionSettings diskSettings, int? cloudProviderId) : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DatabaseSummaryProbe");
            databaseSummary = new DatabaseSummary(connectionInfo);
            this.config = config;
            this.diskSettings = diskSettings;
            wmiConfig = wmiConfiguration;
            this.cloudProviderId = cloudProviderId;
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
            if (config != null && config.ReadyForCollection && cloudProviderId!=CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {
                if (wmiConfig.DirectWmiEnabled)
                    StartMachineNameCollector();
                else
                    StartDatabaseSummaryCollector();
            }
            else if(config != null && config.ReadyForCollection && cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
            {
                StartDatabaseSummaryCollector();
            }
            else
            {
                FireCompletion(databaseSummary, Result.Success);
            }
        }

	
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

            if (e.Result == Result.PermissionViolation)
            {
                this.databaseSummary.ProbeError = e.Snapshot.ProbeError;
                this.FireCompletion(this.databaseSummary, Result.PermissionViolation);
                ((IDisposable)sender).Dispose();
                return;
            }

            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    machineName = _machineSnapshot.MachineName;
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
            driveProbeData = args.Data;

            // start the next probe
            StartDatabaseSummaryCollector();
        }
        
        /// <summary>
        /// Define the DatabaseSummary collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DatabaseSummaryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var cmd = SqlCommandBuilder.BuildDatabaseSummaryCommand(conn,ver,config.IncludeSummaryData,config.DatabaseNameFilter,
                                                                    config.IncludeSystemDatabases,wmiConfig,diskSettings, driveProbeData as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails,cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseSummaryCallback));
        }

        /// <summary>
        /// Starts the Database Summary collector.
        /// </summary>
        private void StartDatabaseSummaryCollector()
        {
            if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                numberOfCloudDbs = 0;
                _cloudDBNames = CollectionHelper.GetDatabases(connectionInfo, LOG);
                if (_cloudDBNames.Count > 0)
                { 
                    StartGenericCollectorDatabase(AzureDatabaseSummaryCollectorDb, databaseSummary, "StartDatabaseSummaryCollectorDb", "Database Summary", AzureDatabaseSummaryCallbackDb, _cloudDBNames[numberOfCloudDbs], new object[] { });
                }
                else
                {
                    databaseSummary.TimeStamp = DateTime.UtcNow;
                    FireCompletion(databaseSummary, Result.Success);
                }
                return;
            }

            //Arg params for DiskSize2005
            var driveProbeDataParam = (DriveStatisticsWmiProbe.DriveStatisticsWmiDetails)driveProbeData;
                bool isWMICallFailed = (driveProbeDataParam == null || driveProbeDataParam.DiskMap == null || driveProbeDataParam.DiskMap.Count == 0);


                StartGenericCollector(new Collector(DatabaseSummaryCollector), databaseSummary, "StartDatabaseSummaryCollector", "Database Summary", DatabaseSummaryCallback, new object[] { isWMICallFailed, config.IncludeSummaryData });
            
        }

        // SQLdm 11 Azure Support
        private void AzureDatabaseSummaryCallbackDb(object sender, CollectorCompleteEventArgs e)
        {

            GenericCallback(new CollectorCallback(AzureDatabaseSummaryCallbackDb),
                            databaseSummary,
                            "StartDatabaseSummaryCollectorDb", "Database Summary",
                            new FailureDelegate(GenericFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate),
                            null, sender, e, true, true, PermissionViolationDelegate);
            /*GenericCallback(new CollectorCallback(AzureDatabaseSummaryCallbackDb), databaseSummary, "StartDatabaseSummaryCollectorDb", "Database Summary",
                sender, e);*/
        }

        private void PermissionViolationDelegate(Snapshot snapshot, Exception e)
        {
            LOG.Error("DatabaseSummaryProbe: Ignoring the permissions exception for a database during collection", e);
        }

        private void AzureDatabaseSummaryCallbackDb(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretAzureDatabaseSummary(rd);
                }

                /*Interlocked.Increment(ref numberOfCloudDbs);
                if (numberOfCloudDbs >= _cloudDBNames.Count)
                {
                    FireCompletion(databaseSummary, Result.Success);
                }
                else
                {
                    StartGenericCollectorDatabase(AzureDatabaseSummaryCollectorDb, databaseSummary, "StartDatabaseSummaryCollectorDb", "Database Summary", AzureDatabaseSummaryCallbackDb, _cloudDBNames[numberOfCloudDbs], new object[] { });
                }*/
            }
            /*else
            {
                FireCompletion(databaseSummary, e.Result);
            }*/
            Interlocked.Increment(ref numberOfCloudDbs);
            if (numberOfCloudDbs >= _cloudDBNames.Count)
            {
                FireCompletion(databaseSummary, Result.Success);
            }
            else
            {
                StartGenericCollectorDatabase(AzureDatabaseSummaryCollectorDb, databaseSummary, "StartDatabaseSummaryCollectorDb", "Database Summary", AzureDatabaseSummaryCallbackDb, _cloudDBNames[numberOfCloudDbs], new object[] { });
            }
        }

        private void AzureDatabaseSummaryCollectorDb(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbName)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDatabaseSummaryCommand(conn, ver, config.IncludeSummaryData, config.DatabaseNameFilter,
                                                                    config.IncludeSystemDatabases, wmiConfig, diskSettings, driveProbeData as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails, cloudProviderId);

            sdtCollector = new SqlCollector(cmd, true);

            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AzureDatabaseSummaryCallbackDb));
        }

        private void InterpretAzureDatabaseSummary(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseSummary"))
            {
                try
                {
                    if (config != null && databaseSummary.ServerStartupTime != config.ServerStartupTime)
                    {
                        LOG.Info("Server restart detected.  Disposing of previous metrics.");
                        config.PreviousDatabaseStatistics = null;
                    }
                    if (config != null && config.LastRefresh.HasValue && databaseSummary.TimeStamp.HasValue
                            && config.LastRefresh < databaseSummary.TimeStamp)
                        databaseSummary.TimeDelta = databaseSummary.TimeStamp.Value.Subtract(config.LastRefresh.Value);

                    string databaseName = "";
                    if (dataReader.Read() && !dataReader.IsDBNull(0))
                    {
                        databaseName = dataReader.GetString(0);
                        dataReader.NextResult();
                    }
                    ReadDatabaseSizes(dataReader);
                    if(databaseName == "master")
                        ReadDatabaseStatistics(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG, "Error interpreting Database Summary Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(databaseSummary);
                }
            }
        }



        /// <summary>
        /// Define the DatabaseSummary callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseSummaryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseSummary(rd);
            }
            FireCompletion(databaseSummary, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseSummary collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseSummaryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseSummaryCallback), databaseSummary, "DatabaseSummaryCallback", "Database Summary",
                            sender, e);
        }

        /// <summary>
        /// Interpret DatabaseSummary data
        /// </summary>
        private void InterpretDatabaseSummary(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseSummary"))
            {
                try
                {
                    if (config != null && databaseSummary.ServerStartupTime != config.ServerStartupTime)
                    {
                        LOG.Info("Server restart detected.  Disposing of previous metrics.");
                        config.PreviousDatabaseStatistics = null;
                    }
                    if (config != null && config.LastRefresh.HasValue && databaseSummary.TimeStamp.HasValue
                            && config.LastRefresh < databaseSummary.TimeStamp)
                        databaseSummary.TimeDelta = databaseSummary.TimeStamp.Value.Subtract(config.LastRefresh.Value);

                    ReadDatabaseSizes(dataReader);
                    ReadDatabaseStatistics(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG, "Error interpreting Database Summary Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(databaseSummary);
                }
            }
        }

        private void AddAllDbNames()
        {
            for (int i = 0; i < _cloudDBNames.Count; i++)
            {
                if(!databaseSummary.Databases.ContainsKey(_cloudDBNames[i]))
                    databaseSummary.Databases.Add(_cloudDBNames[i], new DatabaseDetail(databaseSummary.ServerName,
                                                                             _cloudDBNames[i]));
            }
        }

        private void ReadDatabaseSizes(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadDatabaseSizes"))
            {
                try
                {
                    string dbName = "";
                    bool breakLoop = false;
                    do
                    {
                        while (dataReader.Read())
                        {
                            string director = dataReader.GetString(0).ToLower().Trim();
                            switch (director)
                            {
                                case "database":
                                    if (!dataReader.IsDBNull(1))
                                    {
                                        dbName = dataReader.GetString(1);
                                    }
                                    else
                                    {
                                        dbName = "Unknown";
                                    }

                                    if (dbName == "mssqlsystemresource")
                                        break;

                                    // If Azure, first add all db names
                                    if(cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                                    {
                                        if(_cloudDBNames.Count > 0 && databaseSummary.Databases.Count != _cloudDBNames.Count)
                                        {
                                            AddAllDbNames();
                                        }
                                    }

                                    //Add the database to our dictionary if we haven't already
                                    if (!databaseSummary.Databases.ContainsKey(dbName))
                                    {
                                        databaseSummary.Databases.Add(dbName,
                                                                      new DatabaseDetail(databaseSummary.ServerName,
                                                                                         dbName));
                                    }

                                    //Set the status
                                    try
                                    {
                                        if (!dataReader.IsDBNull(2))
                                        {
                                            var rawStatus = dataReader.GetInt32(2);
                                            LOG.Debug(String.Format("The state of the database: \"{0}\" is \"{1}\"", dbName, rawStatus));

                                            databaseSummary.Databases[dbName].Status = (DatabaseStatus)rawStatus;
                                        }
                                    }
                                    catch
                                    {
                                        databaseSummary.Databases[dbName].Status = DatabaseStatus.Undetermined;  
                                    }

                                    

                                    if (databaseSummary.Databases[dbName].IsAccessibleStatus &&   //Check accessible status
                                        dataReader.FieldCount > 4) //And make sure we actually did access the database (could be single user)
                                    {
                                        int options1 = 0;
                                        int options2 = 0;
                                        
                                        if (!dataReader.IsDBNull(3))
                                            options1 = dataReader.GetInt32(3);
                                        if (!dataReader.IsDBNull(4))
                                            options2 = dataReader.GetInt32(4);
                                        if (!dataReader.IsDBNull(5))
                                            databaseSummary.Databases[dbName].ReplicationCategory =
                                                new DatabaseReplicationCategories(dataReader.GetInt32(5));
                                        if (!dataReader.IsDBNull(6))
                                            databaseSummary.Databases[dbName].DateCreated =
                                                dataReader.GetDateTime(6);
                                        if (!dataReader.IsDBNull(7))
                                            databaseSummary.Databases[dbName].LastBackup =
                                                dataReader.GetDateTime(7);
                                        if (!dataReader.IsDBNull(8))
                                            databaseSummary.Databases[dbName].CompatibilityLevel =
                                                (float)dataReader.GetInt32(8) / 10f;
                                        if (!dataReader.IsDBNull(9))
                                            databaseSummary.Databases[dbName].SystemTables = dataReader.GetInt32(9);
                                        if (!dataReader.IsDBNull(10))
                                            databaseSummary.Databases[dbName].UserTables = dataReader.GetInt32(10);
                                        if (!dataReader.IsDBNull(11))
                                            databaseSummary.Databases[dbName].FileCount = dataReader.GetInt32(11);
                                        if (!dataReader.IsDBNull(12))
                                            databaseSummary.Databases[dbName].FileGroupCount =
                                                dataReader.GetInt32(12);
                                        if (!dataReader.IsDBNull(13))
                                            databaseSummary.Databases[dbName].DataFileSize.Kilobytes = dataReader.GetDecimal(13);
                                        if (!dataReader.IsDBNull(14))
                                            databaseSummary.Databases[dbName].DataSize.Kilobytes = dataReader.GetDecimal(14);
                                        if (!dataReader.IsDBNull(15))
                                            databaseSummary.Databases[dbName].TextSize.Kilobytes = dataReader.GetDecimal(15);
                                        if (!dataReader.IsDBNull(16))
                                            databaseSummary.Databases[dbName].IndexSize.Kilobytes = dataReader.GetDecimal(16);
                                        if (!dataReader.IsDBNull(17))
                                            databaseSummary.Databases[dbName].ProcessCount =
                                                dataReader.GetInt32(17);
                                        //if (!dataReader.IsDBNull(18))
                                        //    databaseSummary.Databases[dbName].LastCheckpoint =
                                        //        dataReader.GetDateTime(18);

                                        if (!dataReader.IsDBNull(18))
                                            databaseSummary.Databases[dbName].SetAutogrow(dataReader.GetInt32(18));

                                        if (!dataReader.IsDBNull(19))
                                            databaseSummary.Databases[dbName].RecoveryType =
                                                (RecoveryModel)dataReader.GetInt32(19);


                                        if (!dataReader.IsDBNull(20))
                                            databaseSummary.Databases[dbName].FileSpaceUsed.Kilobytes = dataReader.GetDecimal(20);

                                        // SQLdm11 InMemoryStorageUsage. Present only for azure
                                        if(cloudProviderId == CLOUD_PROVIDER_ID_AZURE||cloudProviderId== Constants.MicrosoftAzureManagedInstanceId)
                                        {
                                            if (!dataReader.IsDBNull(21))
                                                databaseSummary.Databases[dbName].InMemoryStorageUsage = dataReader.GetDecimal(21);
                                        }
                                        databaseSummary.Databases[dbName].Options =
                                            new DatabaseOptions(options1, options2);

                                    }
                                    else
                                    {
                                       if (dataReader.FieldCount == 4)
                                           if (!dataReader.IsDBNull(3))
                                               databaseSummary.Databases[dbName].RecoveryType =
                                                   (RecoveryModel)dataReader.GetInt32(3);
                                    }
                                    break;
                                case "log expansion":
                                    if (databaseSummary.Databases.ContainsKey(dbName))
                                        databaseSummary.Databases[dbName].LogExpansion.Kilobytes =
                                            dataReader.IsDBNull(1) ? 0 : dataReader.GetDecimal(1);
                                        // If Azure and db is not master, next row after log expansion is log
                                        if (cloudProviderId == Constants.MicrosoftAzureId && dbName != "master")
                                        {
                                            dataReader.NextResult();
                                            ReadLogSizes(dataReader);                                          
                                            breakLoop = true;
                                            continue;
                                        }
                                    break;
                                case "db expansion":
                                    if (databaseSummary.Databases.ContainsKey(dbName))
                                        databaseSummary.Databases[dbName].DatabaseExpansion.Kilobytes =
                                            dataReader.IsDBNull(1) ? 0 : dataReader.GetDecimal(1);                            
                                    break;
                                case "oldact_starttime":
                                    if (databaseSummary.Databases.ContainsKey(dbName))
                                        if (!dataReader.IsDBNull(1))
                                            databaseSummary.Databases[dbName].OldestOpenTransactionStartTime = dataReader.GetDateTime(1);
                                    break;
                                case "oldact_spid":
                                    if (databaseSummary.Databases.ContainsKey(dbName))
                                        if (!dataReader.IsDBNull(1))
                                        {
                                            string spid = dataReader.GetValue(1).ToString();
                                            if (spid != null && spid.Length > 0 && spid.LastIndexOf('s') > 0)
                                            {
                                                spid = spid.Remove(spid.LastIndexOf('s'));
                                            }
                                            int iSpid = 0;
                                            Int32.TryParse(spid, out iSpid);
                                            databaseSummary.Databases[dbName].OldestOpenTransactionSpid = iSpid;
                                        }
                                    break;
                                case "summary":
                                    ServerOverviewInterpreter.ReadDatabaseSummary(dataReader,
                                                                                  databaseSummary.SummaryData,
                                                                                  databaseSummary, LOG);
                                    ReadLogSizes(dataReader);
                                    breakLoop = true;
                                    continue;
                                default:
                                    break;
                            }
                        }
                    } while (!breakLoop && dataReader.NextResult());
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG, "Read database sizes failed: {0}",
                                                        exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        private void ReadLogSizes(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadLogSizes"))
            {
                try
                {
                    FileSize totalLogSize = new FileSize();
                    
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
                            ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG,
                                                                "Read log sizes failed - fieldcount was incorrect",
                                                                false);
                            return;
  
                        }

                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                        {
                            string dbName = dataReader.GetString(0);

                            if (databaseSummary.Databases.ContainsKey(dbName))
                            {
                                // Only attempt calculations for accessible databases
                                if (databaseSummary.Databases[dbName].IsAccessibleStatus &&
                                    dbName != "mssqlsystemresource")
                                {
                                    //Read log size
                                    if(cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                                        databaseSummary.Databases[dbName].LogFileSize.Megabytes =
                                            Convert.ToDecimal(dataReader.GetDouble(1 + sql2012FieldOffset));
                                    else
                                        databaseSummary.Databases[dbName].LogFileSize.Megabytes =
                                            (decimal)dataReader.GetFloat(1 + sql2012FieldOffset);

                                    //The log percent is read as a real percentage, so divide to get into decimal
                                    decimal logPercent;
                                    if (cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
                                        logPercent = (decimal)dataReader.GetFloat(2 + sql2012FieldOffset) / 100;
                                    else
                                        logPercent = Convert.ToDecimal(dataReader.GetDouble(2 + sql2012FieldOffset)/100);
                                    //Used log size is log percent used multiplied by file size
                                    databaseSummary.Databases[dbName].LogSizeUsed.Bytes =
                                        databaseSummary.Databases[dbName].LogFileSize.Bytes * logPercent;

                                    if (totalLogSize.Bytes.HasValue)
                                    {
                                        totalLogSize.Bytes += databaseSummary.Databases[dbName].LogSizeUsed.Bytes;
                                    }
                                    else
                                    {
                                        totalLogSize.Bytes = databaseSummary.Databases[dbName].LogSizeUsed.Bytes;
                                    }

                                    //Set up percent used according to expansion if expansion is allowed
                                    if (databaseSummary.Databases[dbName].LogExpansion.Bytes.HasValue &&
                                        databaseSummary.Databases[dbName].LogExpansion.Bytes != 0)
                                    {
                                        //Include expansion amount if expansion is allowed
                                        databaseSummary.Databases[dbName].PercentLogSpace = (float)(
                                                                                                        databaseSummary.
                                                                                                            Databases[
                                                                                                            dbName].
                                                                                                            LogSizeUsed.
                                                                                                            Bytes /
                                                                                                        (databaseSummary
                                                                                                             .Databases[
                                                                                                             dbName]
                                                                                                             .
                                                                                                             LogFileSize
                                                                                                             .Bytes +
                                                                                                         databaseSummary
                                                                                                             .Databases[
                                                                                                             dbName]
                                                                                                             .
                                                                                                             LogExpansion
                                                                                                             .Bytes));

                                    }
                                    else
                                    {
                                        //Use value from probe if expansion is not allowed
                                        databaseSummary.Databases[dbName].PercentLogSpace = (float)logPercent;
                                    }

                                    //We actually calculate database percent used here to make good use of the loop
                                    if (databaseSummary.Databases[dbName].DataFileSize.Bytes.HasValue &&
                                        databaseSummary.Databases[dbName].DataFileSize.Bytes != 0 )
                                    {
                                        databaseSummary.Databases[dbName].PercentDataSize =
                                            (float)(databaseSummary.Databases[dbName].UsedSize.Bytes /
                                                     (databaseSummary.Databases[dbName].DataFileSize.Bytes +
                                                     (databaseSummary.Databases[dbName].DatabaseExpansion.Bytes.HasValue ? databaseSummary.Databases[dbName].DatabaseExpansion.Bytes: 0)));

                                    }
                                }
                            }
                        }
                    }

                    databaseSummary.SummaryData.LogFileSpaceUsed.Bytes = totalLogSize.Bytes;
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG, "Read log sizes failed: {0}", exception, false);
                    return;
                }
            }
        }

        private void ReadDatabaseStatistics(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadDatabaseStatistics"))
            {
                string dbName = "";
                try
                {
                    while (dataReader.Read())
                    {
                        //Check for valid data
                        if (dataReader.FieldCount != 3)
                        {
                            ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG,
                                                                "Read database statistics failed - fieldcount was incorrect",
                                                                false);
                            return;
                        }

                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                        {
                            dbName = dataReader.GetString(2).TrimEnd();

                            // Do not add to list if it was already filtered out above
                            if (databaseSummary.Databases.ContainsKey(dbName))
                            {
                                //Assign the correct property according to the counter name
                                //We do not take stats on mssqlsystemresource
                                if (dbName != "mssqlsystemresource")
                                {
                                    switch (dataReader.GetString(0).Trim().ToLower())
                                    {
                                        case "transactions/sec":
                                            databaseSummary.Databases[dbName].Transactions_Raw = dataReader.GetInt64(1);
                                            break;
                                        case "log flushes/sec":
                                            databaseSummary.Databases[dbName].LogFlushes_Raw = dataReader.GetInt64(1);
                                            break;
                                        case "log bytes flushed/sec":
                                            //databaseSummary.Databases[dbName].LogSizeFlushed_Raw = new FileSize();
                                            databaseSummary.Databases[dbName].LogSizeFlushed_Raw.Kilobytes =
                                                dataReader.GetInt64(1);
                                            break;
                                        case "log flush waits/sec":
                                            databaseSummary.Databases[dbName].LogFlushWaits_Raw = dataReader.GetInt64(1);
                                            break;
                                        case "log cache reads/sec":
                                            databaseSummary.Databases[dbName].LogCacheReads_Raw = dataReader.GetInt64(1);
                                            break;
                                        case "log cache hit ratio":
                                            databaseSummary.Databases[dbName].LogCacheHitRatio_Raw =
                                                dataReader.GetInt64(1);
                                            break;
                                        case "log cache hit ratio base":
                                            databaseSummary.Databases[dbName].LogCacheHitRatio_Base =
                                                dataReader.GetInt64(1);
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    CalculateDatabaseStatistics();
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseSummary, LOG, "Read database statistics failed"
                                                                      + dbName != null
                                                                          ? " on database [" + dbName + "]"
                                                                          : "" + ": {0}", exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        private void CalculateDatabaseStatistics()
        {
            //Loop through all of the databases we've gathered stats for so far
            foreach (DatabaseDetail detail in databaseSummary.Databases.Values)
            {
                //Calculate the hit ratio even on the first refresh
                if (detail.LogCacheHitRatio_Base.HasValue)
                {
                    if (detail.LogCacheHitRatio_Base > 0)
                    {
                        detail.LogCacheHitRatio = 100 * (double)detail.LogCacheHitRatio_Raw /
                                                   detail.LogCacheHitRatio_Base;
                    }
                    else
                    {
                        detail.LogCacheHitRatio = 0;
                    }
                }

                //Once we have at least 2 refreshes, go ahead and calculate remaining DB stats
                if (config != null && config.PreviousDatabaseStatistics != null)
                {
                    //Look up the previous refresh of our current database
                    if (config.PreviousDatabaseStatistics.ContainsKey(detail.Name))
                    {
                        DatabaseStatistics prevStats = config.PreviousDatabaseStatistics[detail.Name];
                        detail.Transactions =
                            ProbeHelpers.CalculateCounterDelta(prevStats.Transactions_Raw, detail.Transactions_Raw);
                        detail.LogFlushes = ProbeHelpers.CalculateCounterDelta(prevStats.LogFlushes_Raw, detail.LogFlushes_Raw);
                        detail.LogSizeFlushed.Bytes =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogSizeFlushed_Raw, detail.LogSizeFlushed_Raw).Bytes;
                        detail.LogFlushWaits =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogFlushWaits_Raw, detail.LogFlushWaits_Raw);
                        detail.LogCacheReads =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogCacheReads_Raw, detail.LogCacheReads_Raw);
                    }
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
