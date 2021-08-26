//------------------------------------------------------------------------------
// <copyright file="DatabaseFilesProbe.cs" company="Idera, Inc.">
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


    /// <summary>
    /// Returns database files and filegroups
    /// </summary>
    internal sealed class DatabaseFilesProbe : SqlBaseProbe
    {
        #region fields

        private DatabaseFilesSnapshot databaseFiles = null;
        private DatabaseFilesConfiguration config = null;
        private DiskCollectionSettings diskSettings;

        private DriveStatisticsWmiProbe driveProbe;
        private WmiConfiguration wmiConfig;
        private string machineName;
        private object driveProbeData;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFilesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DatabaseFilesProbe(SqlConnectionInfo connectionInfo, DatabaseFilesConfiguration config, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DatabaseFilesProbe");
            databaseFiles = new DatabaseFilesSnapshot(connectionInfo);
            this.config = config;
            this.diskSettings = diskSettings;
            wmiConfig = wmiConfiguration;
            // Skip permissions for CloudProviders
            this.cloudProviderId = cloudProviderId;
        }

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (config != null && config.ReadyForCollection && cloudProviderId!= CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {
                if (wmiConfig.DirectWmiEnabled)
                    StartMachineNameCollector();
                else
                    StartDatabaseFilesCollector();
            }
            else
            {
                FireCompletion(databaseFiles, Result.Success);
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
                this.databaseFiles.ProbeError = e.Snapshot.ProbeError;
                this.FireCompletion(this.databaseFiles, Result.PermissionViolation);
                ((IDisposable)sender).Dispose();
                return;
            }

            if (e.Snapshot != null) 
            {
            var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
            if (_machineSnapshot != null)
                machineName=_machineSnapshot.MachineName;
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
            driveProbe.Dispose();

            // start the next probe
            StartDatabaseFilesCollector();
        }


        /// <summary>
        /// Define the DatabaseFiles collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DatabaseFilesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildDatabaseFilesCommand(conn, ver, config, wmiConfig, diskSettings, driveProbeData,cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseFilesCallback));
        }

        /// <summary>
        /// Starts the Database Files collector.
        /// </summary>
        private void StartDatabaseFilesCollector()
        {
            //Arg params for DiskSize2005
            var driveProbeDataParam = (DriveStatisticsWmiProbe.DriveStatisticsWmiDetails)driveProbeData;
            bool isWMICallFailed = (driveProbeDataParam == null || driveProbeDataParam.DiskMap == null || driveProbeDataParam.DiskMap.Count == 0);

            StartGenericCollector(new Collector(DatabaseFilesCollector), databaseFiles, "StartDatabaseFilesCollector", "Database Files", DatabaseFilesCallback, new object[] { config.DatabaseNames, isWMICallFailed });
        }

        /// <summary>
        /// Define the DatabaseFiles callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseFilesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseFiles(rd);
            }
            FireCompletion(databaseFiles, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseFiles collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseFilesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseFilesCallback), databaseFiles, "DatabaseFilesCallback", "Database Files",
                            sender, e);
        }

        /// <summary>
        /// Interpret DatabaseFiles data
        /// </summary>
        private void InterpretDatabaseFiles(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseFiles"))
            {
                try
                {
                    ReadDiskDrives(dataReader);
                    while (dataReader.Read())
                    {
                        ReadDatabaseFile(dataReader);
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            ReadDatabaseFilegroups(dataReader);
                        }
                    }
                    if (databaseFiles.ProductVersion.Major >= 9 && config.DatabaseNames.Count == 1 && config.DatabaseNames[0] == "tempdb")
                    {
                        List<TempdbFileActivity> tempdbFiles = new List<TempdbFileActivity>();
                        TempdbStatistics tempdbStats = new TempdbStatistics();
                        ProbeHelpers.ReadTempdbStatistics(dataReader, databaseFiles, LOG, config.PreviousTempdbStatistics, 
                            ref tempdbStats, ref tempdbFiles, config.PreviousTempdbStatistics != null ? config.PreviousTempdbStatistics.TimeStamp : null, databaseFiles.TimeStamp);
                        databaseFiles.TempdbStatistics = tempdbStats;
                        foreach (TempdbFileActivity tempdbFile in tempdbFiles)
                        {
                            if (databaseFiles.DatabaseFiles.ContainsKey("tempdb" + tempdbFile.Filename))
                            {
                                databaseFiles.DatabaseFiles["tempdb" + tempdbFile.Filename].TempdbFileActivity = tempdbFile;
                            }
                           
                        }


                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseFiles, LOG, "Error interpreting Database Files Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(databaseFiles);
                }
            }
        }

        private void ReadDiskDrives(SqlDataReader dataReader)
        {
            try
            {
                bool IsFileSystemObjectDataAvailable;
                databaseFiles.DiskDrives = ProbeHelpers.ReadDiskDrives(dataReader, config.PreviousDiskDrives,
                    out IsFileSystemObjectDataAvailable, LOG);
                databaseFiles.IsFileSystemObjectDataAvailable = IsFileSystemObjectDataAvailable;
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(databaseFiles, LOG, "Error interpreting Disk Drives data: {0}", e,
                                                    false);
                GenericFailureDelegate(databaseFiles);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadDatabaseFile(SqlDataReader dataReader)
        {
            
            if (dataReader.HasRows)
            {
                DatabaseFile df = new DatabaseFile();
                if (dataReader.FieldCount == 13)
                {
                    if (!dataReader.IsDBNull(0)) df.DatabaseName = dataReader.GetString(0).Trim(); ;
                    if (!dataReader.IsDBNull(1)) df.LogicalFilename = dataReader.GetString(1).Trim(); ;
                    if (!dataReader.IsDBNull(2)) df.IsDataFile = dataReader.GetInt32(2) == 1?true:false;
                    if (!dataReader.IsDBNull(3)) df.FilegroupName = dataReader.GetString(3).Trim(); ;
                    if (!dataReader.IsDBNull(4)) df.ConfiguredGrowth = dataReader.GetInt32(4);
                    if (!dataReader.IsDBNull(5)) df.CurrentSize.Kilobytes = dataReader.GetDecimal(5);
                    if (!dataReader.IsDBNull(6)) df.CurrentUsedSize.Kilobytes = dataReader.GetDecimal(6);
                    if (!dataReader.IsDBNull(7)) df.ConfiguredMaxSize = dataReader.GetInt32(7);
                    if (!dataReader.IsDBNull(8)) df.ExpansionSpace.Kilobytes = dataReader.GetDecimal(8);
                    if (!dataReader.IsDBNull(9)) df.FreeSpaceOnDisk.Kilobytes = dataReader.GetDecimal(9);
                    if (!dataReader.IsDBNull(10)) df.FilePath = dataReader.GetString(10);
                    if (!dataReader.IsDBNull(11)) df.StatusBits = dataReader.GetInt32(11);
                    if (!dataReader.IsDBNull(12)) df.DriveName = dataReader.GetString(12);

                    databaseFiles.DatabaseFiles.Add(df.InternalIdentifier, df);
                }
            }
        }

        private void ReadDatabaseFilegroups(SqlDataReader dataReader)
        {

            if (dataReader.HasRows)
            {
                DatabaseFileGroup df = new DatabaseFileGroup();
                if (dataReader.FieldCount == 10)
                {
                    if (!dataReader.IsDBNull(0)) df.DatabaseName = dataReader.GetString(0).Trim(); ;
                    if (!dataReader.IsDBNull(1)) df.FilegroupName = dataReader.GetString(1).Trim(); ;
                    if (!dataReader.IsDBNull(2)) df.FileCount = dataReader.GetInt32(2);
                    if (!dataReader.IsDBNull(3)) df.CurrentSize.Kilobytes = dataReader.GetDecimal(3);
                    if (!dataReader.IsDBNull(4)) df.CurrentUsedSize.Kilobytes = dataReader.GetDecimal(4);
                    if (!dataReader.IsDBNull(5)) df.SetAutogrow(dataReader.GetInt32(5));
                    if (!dataReader.IsDBNull(6)) df.ExpansionSpace.Kilobytes = dataReader.GetDecimal(6);
                    if (!dataReader.IsDBNull(7)) df.DataSize.Kilobytes = dataReader.GetDecimal(7);
                    if (!dataReader.IsDBNull(8)) df.TextSize.Kilobytes = dataReader.GetDecimal(8);
                    if (!dataReader.IsDBNull(9)) df.IndexSize.Kilobytes = dataReader.GetDecimal(9);
                }
                databaseFiles.DatabaseFileGroups.Add(df.InternalIdentifier,df);
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
